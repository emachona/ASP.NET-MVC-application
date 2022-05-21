using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using University.Areas.Identity.Data;
using University.Models;

namespace University.Data 
{ 
    public class UniversityContext : IdentityDbContext<UniversityUser>
    {
        public UniversityContext(DbContextOptions<UniversityContext> options)
            : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Course>().ToTable("Course");
            builder.Entity<Enrollment>().ToTable("Enrollment");
            builder.Entity<Student>().ToTable("Student");
            builder.Entity<Teacher>().ToTable("Teacher");

            builder.Entity<Course>()
                .HasOne<Teacher>(p => p.FirstTeacher)
                .WithMany(p => p.Course1)
                .HasForeignKey(p => p.FirstTeacherID);//.OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Course>()
                .HasOne<Teacher>(p => p.SecondTeacher)
                .WithMany(p => p.Course2)
                .HasForeignKey(p => p.SecondTeacherID);//.OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Enrollment>()
                .HasOne(p => p.Course)
                .WithMany(p => p.Enrollments)
                .HasForeignKey(p => p.CourseID);//.OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Enrollment>()
                .HasOne(p => p.Student)
                .WithMany(p => p.Enrollments)
                .HasForeignKey(p => p.StudentID);//.OnDelete(DeleteBehavior.NoAction);
        }
    }
}
