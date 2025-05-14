using Microsoft.EntityFrameworkCore;
using SibaSchoolManagementApi.Data;
using SibaSchoolManagementApi.DTOs;
using SibaSchoolManagementApi.Models;

namespace SibaSchoolManagementApi.Services
{
    public class CourseService(SchoolDbContext context) : ICourseService
    {
        private readonly SchoolDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync()
        {
            return await _context.Courses
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Code = c.Code ?? string.Empty,
                    Name = c.Name ?? string.Empty,
                    Description = c.Description,
                    CreditHours = c.CreditHours,
                    IsActive = c.IsActive
                })
                .ToListAsync();
        }

        public async Task<CourseDto?> GetCourseByIdAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            return course is null ? null : MapToDto(course);
        }

        public async Task<CourseDto> CreateCourseAsync(CreateCourseDto courseDto)
        {
            ArgumentNullException.ThrowIfNull(courseDto);

            var course = new Course
            {
                Code = courseDto.Code ?? throw new ArgumentException("Course code is required"),
                Name = courseDto.Name ?? throw new ArgumentException("Course name is required"),
                Description = courseDto.Description,
                CreditHours = courseDto.CreditHours,
                IsActive = true
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return MapToDto(course);
        }

        public async Task UpdateCourseAsync(int id, UpdateCourseDto courseDto)
        {
            ArgumentNullException.ThrowIfNull(courseDto);

            var course = await _context.Courses.FindAsync(id)
                ?? throw new ArgumentException($"Course with ID {id} not found");

            course.Code = courseDto.Code ?? course.Code;
            course.Name = courseDto.Name ?? course.Name;
            course.Description = courseDto.Description ?? course.Description;
            course.CreditHours = courseDto.CreditHours;
            course.IsActive = courseDto.IsActive;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteCourseAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id)
                ?? throw new ArgumentException($"Course with ID {id} not found");

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<StudentDto>> GetCourseStudentsAsync(int courseId)
        {
            return await _context.StudentCourses
                .Where(sc => sc.CourseId == courseId)
                .Select(sc => new StudentDto
                {
                    Id = sc.Student!.Id,
                    StudentId = sc.Student.StudentId ?? string.Empty,
                    FirstName = sc.Student.FirstName ?? string.Empty,
                    LastName = sc.Student.LastName ?? string.Empty,
                    DateOfBirth = sc.Student.DateOfBirth,
                    Gender = sc.Student.Gender ?? string.Empty,
                    RegistrationDate = sc.Student.RegistrationDate,
                    IsActive = sc.Student.IsActive
                })
                .ToListAsync();
        }

        private static CourseDto MapToDto(Course course) => new()
        {
            Id = course.Id,
            Code = course.Code ?? string.Empty,
            Name = course.Name ?? string.Empty,
            Description = course.Description,
            CreditHours = course.CreditHours,
            IsActive = course.IsActive
        };
    }
}