using CourseDx.Data;
using CourseDx.Models;
using CourseDx.Middleware;
using CourseDx.Repositories;
using CourseDx.Services;
using CourseDx.Mapping;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using WebOptimizer;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using Serilog;
using AspNetCoreRateLimit;
using HealthChecks.UI.Client;

namespace CourseDx
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .Enrich.WithMachineName()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: "Logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            try
            {
                Log.Information("Starting CourseDx application");
                
                var builder = WebApplication.CreateBuilder(args);
                
                // Use Serilog for logging
                builder.Host.UseSerilog();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            #region Connection With Data Base

            builder.Services.AddDbContext<CourseDxContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            #endregion

            builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<CourseDxContext>()
                .AddDefaultTokenProviders();

            #region Repository and Services Registration

            // Register Unit of Work and Generic Repository
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Register Services
            builder.Services.AddScoped<IStudentService, StudentService>();
            builder.Services.AddScoped<ICourseService, CourseService>();
            builder.Services.AddScoped<IInstructorService, InstructorService>();
            builder.Services.AddSingleton<ICacheService, CacheService>();
            builder.Services.AddScoped<IEmailService, EmailService>();

            // Register AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Register FluentValidation
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            #endregion

            #region Rate Limiting Configuration

            // Configure Rate Limiting
            builder.Services.AddMemoryCache();
            builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
            builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
            builder.Services.AddInMemoryRateLimiting();
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            #endregion

            #region Caching Configuration

            // Memory Cache is already added above for Rate Limiting
            // It can be used throughout the application for caching

            #endregion

            #region Health Checks Configuration

            builder.Services.AddHealthChecks()
                .AddSqlServer(
                    connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
                    name: "SQL Server",
                    tags: new[] { "database", "sql" })
                .AddCheck("Application", () => 
                    Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Application is running"));

            #endregion

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = ".AspNetCore.Identity.Application";
                // Redirect unauthenticated users to the Account/Login page
                options.LoginPath = "/Account/Login";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                options.SlidingExpiration = true;
            });

            // ✅ Enable Response Compression (gzip, br)
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
                options.Providers.Add<BrotliCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    "image/svg+xml", "application/javascript", "application/json", "text/css"
                });
            });

            builder.Services.Configure<GzipCompressionProviderOptions>(opts =>
            {
                opts.Level = CompressionLevel.Fastest;
            });

            builder.Services.Configure<BrotliCompressionProviderOptions>(opts =>
            {
                opts.Level = CompressionLevel.Fastest;
            });

            var app = builder.Build();

            // Rate Limiting Middleware (should be early in the pipeline)
            app.UseIpRateLimiting();

            // Global exception handling middleware
            app.UseExceptionHandling();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // ✅ Static files with caching
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    // Cache for 30 days
                    ctx.Context.Response.Headers["Cache-Control"] = "public,max-age=2592000";
                }
            });

            // ✅ Enable compression
            app.UseResponseCompression();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
              name: "Admin",
              pattern: "{controller=Admin}/{action=AdminHome}/{id?}");

            // Health Check Endpoints
            app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("database"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = _ => false // No checks, just confirms app is running
            });

            // Configure Rotativa
            RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");

            // Seeding admin user
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                await DbSeeder.SeedAdminUserAsync(services, configuration);
            }

            app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }

    // Seeder class for initial admin user
    public static class DbSeeder
    {
        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Read admin settings from configuration
            string adminRole = "Admin";
            string adminEmail = configuration["AdminSettings:Email"] ?? "admin@coursedx.com";
            string adminUserName = configuration["AdminSettings:UserName"] ?? "admin";
            string adminPassword = configuration["AdminSettings:Password"] ?? "Admin_123";
            string adminFirstName = configuration["AdminSettings:FirstName"] ?? "Admin";
            string adminLastName = configuration["AdminSettings:LastName"] ?? "Admin";

            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            var adminUserEntity = await userManager.FindByNameAsync(adminUserName);
            if (adminUserEntity == null)
            {
                adminUserEntity = new AppUser
                {
                    FirstName = adminFirstName,
                    LastName = adminLastName,
                    UserName = adminUserName,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    IsAdmin = true
                };

                var createUser = await userManager.CreateAsync(adminUserEntity, adminPassword);
                if (createUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUserEntity, adminRole);
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(adminUserEntity, adminRole))
                {
                    await userManager.AddToRoleAsync(adminUserEntity, adminRole);
                }
            }
        }
    }
}
