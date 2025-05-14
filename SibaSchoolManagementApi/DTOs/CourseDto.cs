namespace SibaSchoolManagementApi.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int CreditHours { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCourseDto
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int CreditHours { get; set; }
    }

    public class UpdateCourseDto
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int CreditHours { get; set; }
        public bool IsActive { get; set; }
    }
}