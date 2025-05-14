using SibaSchoolManagementApi.DTOs;

namespace SibaSchoolManagementApi.Services
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentDto?>> GetAllStudentsAsync();
        Task<StudentDto?> GetStudentByIdAsync(int id);
        Task<StudentDto?> CreateStudentAsync(CreateStudentDto studentDto);
        Task UpdateStudentAsync(int id, UpdateStudentDto studentDto);
        Task DeleteStudentAsync(int id);
        Task<IEnumerable<CourseDto?>> GetStudentCoursesAsync(int studentId);
        Task AssignCoursesToStudentAsync(int studentId, IEnumerable<int> courseIds);
    }
}