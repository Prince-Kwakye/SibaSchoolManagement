using Microsoft.EntityFrameworkCore;
using SibaSchoolManagementApi.Data;
using SibaSchoolManagementApi.DTOs;
using SibaSchoolManagementApi.Models;

namespace SibaSchoolManagementApi.Services
{
    public class StudentService(SchoolDbContext context) : IStudentService
    {
        private readonly SchoolDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task<IEnumerable<StudentDto?>> GetAllStudentsAsync()
        {
            return await _context.Students
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    StudentId = s.StudentId,
                    FirstName = s.FirstName ?? string.Empty,
                    LastName = s.LastName ?? string.Empty,
                    DateOfBirth = s.DateOfBirth,
                    Gender = s.Gender ?? string.Empty,
                    Address = s.Address,
                    Email = s.Email,
                    Phone = s.Phone,
                    RegistrationDate = s.RegistrationDate,
                    IsActive = s.IsActive
                })
                .ToListAsync();
        }

        public async Task<StudentDto?> GetStudentByIdAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student is null)
            {
                return null;
            }

            return new StudentDto
            {
                Id = student.Id,
                StudentId = student.StudentId ?? string.Empty,
                FirstName = student.FirstName ?? string.Empty,
                LastName = student.LastName ?? string.Empty,
                DateOfBirth = student.DateOfBirth,
                Gender = student.Gender ?? string.Empty,
                Address = student.Address,
                Email = student.Email,
                Phone = student.Phone,
                RegistrationDate = student.RegistrationDate,
                IsActive = student.IsActive
            };
        }

        public async Task<StudentDto?> CreateStudentAsync(CreateStudentDto studentDto)
        {
            ArgumentNullException.ThrowIfNull(studentDto);

            var student = new Student
            {
                StudentId = studentDto.StudentId ?? throw new ArgumentException("Student ID is required"),
                FirstName = studentDto.FirstName ?? throw new ArgumentException("First name is required"),
                LastName = studentDto.LastName ?? throw new ArgumentException("Last name is required"),
                DateOfBirth = studentDto.DateOfBirth,
                Gender = studentDto.Gender ?? "Unknown",
                Address = studentDto.Address,
                Email = studentDto.Email,
                Phone = studentDto.Phone,
                RegistrationDate = DateTime.UtcNow,
                IsActive = true
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return new StudentDto
            {
                Id = student.Id,
                StudentId = student.StudentId,
                FirstName = student.FirstName,
                LastName = student.LastName,
                DateOfBirth = student.DateOfBirth,
                Gender = student.Gender,
                Address = student.Address,
                Email = student.Email,
                Phone = student.Phone,
                RegistrationDate = student.RegistrationDate,
                IsActive = student.IsActive
            };
        }

        public async Task UpdateStudentAsync(int id, UpdateStudentDto studentDto)
        {
            ArgumentNullException.ThrowIfNull(studentDto);

            var student = await _context.Students.FindAsync(id)
                ?? throw new ArgumentException($"Student with ID {id} not found");

            student.StudentId = studentDto.StudentId ?? student.StudentId;
            student.FirstName = studentDto.FirstName ?? student.FirstName;
            student.LastName = studentDto.LastName ?? student.LastName;
            student.DateOfBirth = studentDto.DateOfBirth;
            student.Gender = studentDto.Gender ?? student.Gender;
            student.Address = studentDto.Address ?? student.Address;
            student.Email = studentDto.Email ?? student.Email;
            student.Phone = studentDto.Phone ?? student.Phone;
            student.IsActive = studentDto.IsActive;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteStudentAsync(int id)
        {
            var student = await _context.Students.FindAsync(id)
                ?? throw new ArgumentException($"Student with ID {id} not found");

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CourseDto?>> GetStudentCoursesAsync(int studentId)
        {
            return await _context.StudentCourses
                .Where(sc => sc.StudentId == studentId)
                .Select(sc => new CourseDto
                {
                    Id = sc.Course!.Id,
                    Code = sc.Course.Code ?? string.Empty,
                    Name = sc.Course.Name ?? string.Empty,
                    Description = sc.Course.Description,
                    CreditHours = sc.Course.CreditHours,
                    IsActive = sc.Course.IsActive
                })
                .ToListAsync();
        }

        public async Task AssignCoursesToStudentAsync(int studentId, IEnumerable<int> courseIds)
        {
            ArgumentNullException.ThrowIfNull(courseIds);

            // Verify student exists
            var studentExists = await _context.Students.AnyAsync(s => s.Id == studentId);
            if (!studentExists)
            {
                throw new ArgumentException($"Student with ID {studentId} not found");
            }

            // Verify all courses exist
            var invalidCourseIds = courseIds.Except(await _context.Courses.Select(c => c.Id).ToListAsync());
            if (invalidCourseIds.Any())
            {
                throw new ArgumentException($"Invalid course IDs: {string.Join(", ", invalidCourseIds)}");
            }

            // Remove existing courses
            var existingCourses = await _context.StudentCourses
                .Where(sc => sc.StudentId == studentId)
                .ToListAsync();

            _context.StudentCourses.RemoveRange(existingCourses);

            // Add new courses
            foreach (var courseId in courseIds)
            {
                _context.StudentCourses.Add(new StudentCourse
                {
                    StudentId = studentId,
                    CourseId = courseId,
                    EnrollmentDate = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}