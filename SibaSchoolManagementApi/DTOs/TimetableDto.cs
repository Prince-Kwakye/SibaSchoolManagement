namespace SibaSchoolManagementApi.DTOs
{
    public class TimetableDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string? CourseName { get; set; }
        public int DayOfWeek { get; set; }
        public string? DayName { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? RoomNumber { get; set; }
        public string? AcademicYear { get; set; }
        public string? Semester { get; set; }
    }

    public class CreateTimetableDto
    {
        public int CourseId { get; set; }
        public int DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? RoomNumber { get; set; }
        public string? AcademicYear { get; set; }
        public string? Semester { get; set; }
    }

    public class UpdateTimetableDto
    {
        public int CourseId { get; set; }
        public int DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? RoomNumber { get; set; }
        public string? AcademicYear { get; set; }
        public string? Semester { get; set; }
        public bool IsActive { get; set; }
    }
}