using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SibaSchoolManagementApi.Models;
using System;
using Microsoft.AspNetCore.Identity;

namespace SibaSchoolManagementApi.Data
{
    public class SchoolDbContext(DbContextOptions<SchoolDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, int>(options)
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Timetable> Timetable { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 

            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.StudentId, sc.CourseId });

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.StudentId);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseId);

            modelBuilder.Entity<Timetable>()
                .HasOne(t => t.Course)
                .WithMany(c => c.TimetableSlots)
                .HasForeignKey(t => t.CourseId);

            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = 1,
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    Email = "admin@siba.com",
                    NormalizedEmail = "ADMIN@SIBA.COM",
                    EmailConfirmed = true,
                    PasswordHash = "$2a$12$uYxO/VpU7vRCGVrZyJYm/.DrbcAvbKuj8RMAWTQ94zLeBpqGgzvUq",
                    CustomRole = "Admin",
                    CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    SecurityStamp = Guid.NewGuid().ToString("D")
                },
                new ApplicationUser
                {
                    Id = 2,
                    UserName = "staff",
                    NormalizedUserName = "STAFF",
                    Email = "staff@siba.com",
                    NormalizedEmail = "STAFF@SIBA.COM",
                    EmailConfirmed = true,
                    PasswordHash = "$2a$12$fQHgWkNqORlj6jXgb9TTzOWKZUkMPTz3yW3LtDVoPUKZFFBk0hzqW",
                    CustomRole = "Staff",
                    CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    SecurityStamp = Guid.NewGuid().ToString("D")
                }
            );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }
    }
}
