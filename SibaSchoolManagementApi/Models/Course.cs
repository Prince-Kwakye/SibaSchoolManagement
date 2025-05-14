using System.ComponentModel.DataAnnotations;
using System.Xml;

namespace SibaSchoolManagementApi.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string? Code { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public int CreditHours { get; set; }

        public bool IsActive { get; set; } = true;

       
        public ICollection<StudentCourse>? StudentCourses { get; set; }
        public ICollection<Timetable>? TimetableSlots { get; set; }
    }
}