namespace SibaSchoolManagementApi.DTOs
{
    public class LoginDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public class RegisterDto
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string FullName { get; set; }
        public required string Password { get; set; }
        public string? Role { get; set; } = "Staff";
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Role { get; set; } = null!;
        public DateTime Expiration { get; set; }
    }
}