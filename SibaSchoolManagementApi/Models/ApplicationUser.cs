using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SibaSchoolManagementApi.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        [Required]
        [StringLength(50)]
        public override string? UserName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public override string? Email { get; set; }

        [StringLength(20)]
        public string CustomRole { get; set; } = "Staff";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        public string? FullName { get; set; }
    }
}
