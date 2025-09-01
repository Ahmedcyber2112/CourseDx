using CourseDx.Entity;
using CourseDx.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CourseDx.Data
{
    public class CourseDxContext : IdentityDbContext<AppUser> 
    {
        public CourseDxContext(DbContextOptions<CourseDxContext> options) : base(options) { }
         public DbSet<Student> Students { get; set; }
        public DbSet<Instractor> Instractor { get; set; }
        public DbSet<Course> Courses { get; set; }
        
        public DbSet<CourseEnrollment> CourseEnrollment { get; set; }

        public DbSet<CourseDetals> CourseDetals { get; set; }
        public DbSet<InstractorCourses>  InstractorCourses { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(modelBuilder);
            base.OnModelCreating(modelBuilder);

            // تحديد الدقة والسكيل للـ Price لتجنب التحذيرات والأخطاء
            modelBuilder.Entity<CourseDetals>()
                .Property(cd => cd.Price)
                .HasPrecision(18, 2);


            // جعل الايميل Unique

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>()
                   .HasIndex(u => u.Email)
                   .IsUnique();
        }

    }
}
