using Microsoft.EntityFrameworkCore;
using StudentSystem.Data.Models;

namespace StudentSystem.Data
{
    public class StudentSystemDbContext : DbContext
    {
        public StudentSystemDbContext(DbContextOptions<StudentSystemDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Homework> Homeworks { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.StudentId);

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(true);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsRequired(false);

                entity.Property(e => e.RegisteredOn)
                    .IsRequired(true);

                entity.Property(e => e.Birthday)
                    .IsRequired(false);
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.CourseId);

                entity.Property(e => e.Name)
                    .HasMaxLength(80)
                    .IsUnicode(true);

                entity.Property(e => e.Description)
                    .IsUnicode(true)
                    .IsRequired(false);

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18,2)");
            });



            modelBuilder.Entity<Resource>(entity =>
            {
                entity.HasKey(e => e.ResourceId);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.Url)
                    .IsUnicode(false);

                entity.HasOne(e => e.Course)
                    .WithMany(c => c.Resources)
                    .HasForeignKey(e => e.CourseId);
            });

            modelBuilder.Entity<Homework>(entity =>
            {
                entity.HasKey(e => e.HomeworkId);

                entity.Property(e => e.Content)
                    .IsUnicode(false);

                entity.HasOne(e => e.Student)
                    .WithMany(s => s.Homeworks)
                    .HasForeignKey(e => e.StudentId);

                entity.HasOne(e => e.Course)
                    .WithMany(c => c.Homeworks)
                    .HasForeignKey(e => e.CourseId);
            });

            modelBuilder.Entity<StudentCourse>(entity =>
            {
                entity.HasKey(e => new { e.StudentId, e.CourseId });

                entity.HasOne(e => e.Student)
                    .WithMany(s => s.StudentCourses)
                    .HasForeignKey(e => e.StudentId);

                entity.HasOne(e => e.Course)
                    .WithMany(c => c.StudentCourses)
                    .HasForeignKey(e => e.CourseId);
            });
        }
    }
}
