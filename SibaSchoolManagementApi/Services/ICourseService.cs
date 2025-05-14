using SibaSchoolManagementApi.DTOs;

namespace SibaSchoolManagementApi.Services
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseDto>> GetAllCoursesAsync();
        Task<CourseDto?> GetCourseByIdAsync(int id);
        Task<CourseDto> CreateCourseAsync(CreateCourseDto courseDto);
        Task UpdateCourseAsync(int id, UpdateCourseDto courseDto);
        Task DeleteCourseAsync(int id);
        Task<IEnumerable<StudentDto>> GetCourseStudentsAsync(int courseId);
    }
}