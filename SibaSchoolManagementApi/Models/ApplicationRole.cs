using Microsoft.AspNetCore.Identity;

namespace SibaSchoolManagementApi.Models
{
    public class ApplicationRole : IdentityRole<int>
    {
        public string Description { get; set; } = string.Empty;
    }
}