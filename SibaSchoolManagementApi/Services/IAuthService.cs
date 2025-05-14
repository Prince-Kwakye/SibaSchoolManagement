using SibaSchoolManagementApi.DTOs;

namespace SibaSchoolManagementApi.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    }
}