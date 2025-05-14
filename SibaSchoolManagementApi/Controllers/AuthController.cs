using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SibaSchoolManagementApi.DTOs;
using SibaSchoolManagementApi.Services;

namespace SibaSchoolManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly ILogger<AuthController> _logger = logger;

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            try
            {
                var response = await _authService.LoginAsync(loginDto);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Login failed for user {Username}", loginDto.Username);
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {Username}", loginDto.Username);
                return StatusCode(500, "An error occurred during login");
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
        {
            try
            {
                var response = await _authService.RegisterAsync(registerDto);
                return CreatedAtAction(nameof(Login), response);
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning(ex, "Registration failed for user {Username}", registerDto.Username);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for user {Username}", registerDto.Username);
                return StatusCode(500, "An error occurred during registration");
            }
        }
    }
}