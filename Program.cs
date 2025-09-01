using CourseDx.Data;
using CourseDx.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using WebOptimizer;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

namespace CourseDx
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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

            // Configure Rotativa
            RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");

            // ✅ Seeding admin user
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await DbSeeder.SeedAdminUserAsync(services);
            }

            app.Run();
        }
    }

    // ✅ Seeder
    public static class DbSeeder
    {
        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string adminRole = "Admin";
            string adminEmail = "admin@coursedx.com";
            string adminUserName = "admin";
            string adminPassword = "Admin_123";

            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            var adminUserEntity = await userManager.FindByNameAsync(adminUserName);
            if (adminUserEntity == null)
            {
                adminUserEntity = new AppUser
                {
                    FirstName = "Admin",
                    LastName = "Admin",
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
