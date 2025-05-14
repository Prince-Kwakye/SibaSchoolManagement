using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SibaSchoolManagementApi.DTOs;
using SibaSchoolManagementApi.Services;

namespace SibaSchoolManagementApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController(IStudentService studentService) : ControllerBase
    {
        private readonly IStudentService _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));

        [Authorize(Policy = "AdminOrStaff")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents()
        {
            var students = await _studentService.GetAllStudentsAsync();
            return Ok(students);
        }

        [Authorize(Policy = "AdminOrStaff")]
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> GetStudent(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);

            if (student is null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        [Authorize(Policy = "AdminOrStaff")]
        [HttpPost]
        public async Task<ActionResult<StudentDto>> CreateStudent([FromBody] CreateStudentDto studentDto)
        {
            if (studentDto is null)
            {
                return BadRequest("Student data cannot be null");
            }

            var student = await _studentService.CreateStudentAsync(studentDto);

            if (student is null)
            {
                return Problem("Failed to create student");
            }

            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
        }

        [Authorize(Policy = "AdminOrStaff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentDto studentDto)
        {
            if (studentDto is null)
            {
                return BadRequest("Student data cannot be null");
            }

            try
            {
                await _studentService.UpdateStudentAsync(id, studentDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Policy = "AdminOrStaff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            try
            {
                await _studentService.DeleteStudentAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Policy = "AdminOrStaff")]
        [HttpGet("{studentId}/courses")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetStudentCourses(int studentId)
        {
            var courses = await _studentService.GetStudentCoursesAsync(studentId);
            return Ok(courses ?? []);
        }

        [Authorize(Policy = "AdminOrStaff")]
        [HttpPost("{studentId}/courses")]
        public async Task<IActionResult> AssignCoursesToStudent(
            int studentId,
            [FromBody] IEnumerable<int>? courseIds)
        {
            if (courseIds is null)
            {
                return BadRequest("Course IDs cannot be null");
            }

            await _studentService.AssignCoursesToStudentAsync(studentId, courseIds);
            return NoContent();
        }
    }
}