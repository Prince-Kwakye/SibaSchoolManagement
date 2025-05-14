using System.ComponentModel.DataAnnotations;

namespace SibaSchoolManagementApi.Models
{
    public class Timetable
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [Range(1, 7)]
        public int DayOfWeek { get; set; } // 1=Monday, 7=Sunday

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [StringLength(20)]
        public string? RoomNumber { get; set; }

        [Required]
        [StringLength(20)]
        public string? AcademicYear { get; set; }

        [Required]
        [StringLength(20)]
        public string? Semester { get; set; }

        // Navigation property
        public Course? Course { get; set; }
    }
}