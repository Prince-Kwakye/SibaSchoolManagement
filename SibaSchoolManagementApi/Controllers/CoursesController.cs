using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SibaSchoolManagementApi.DTOs;
using SibaSchoolManagementApi.Services;

namespace SibaSchoolManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController(ICourseService courseService) : ControllerBase
    {
        private readonly ICourseService _courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
        
        [Authorize(Policy = "AdminOrStaff")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return Ok(courses);
        }

        [Authorize(Policy ="AdminOrStaff")]
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetCourse(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            return course is null ? NotFound() : Ok(course);
        }

        [Authorize(Policy = "AdminOrStaff")]
        [HttpPost]
        public async Task<ActionResult<CourseDto>> CreateCourse(CreateCourseDto courseDto)
        {
            var course = await _courseService.CreateCourseAsync(courseDto);
            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
        }

        [Authorize(Policy = "AdminOrStaff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, UpdateCourseDto courseDto)
        {
            try
            {
                await _courseService.UpdateCourseAsync(id, courseDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Policy = "AdminOrStaff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            try
            {
                await _courseService.DeleteCourseAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("{courseId}/students")]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetCourseStudents(int courseId)
        {
            var students = await _courseService.GetCourseStudentsAsync(courseId);
            return Ok(students);
        }
    }
}