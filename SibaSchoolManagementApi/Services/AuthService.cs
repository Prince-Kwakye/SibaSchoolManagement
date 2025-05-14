using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SibaSchoolManagementApi.Models;
using SibaSchoolManagementApi.DTOs;

namespace SibaSchoolManagementApi.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IConfiguration _configuration = configuration;

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            
            var username = user.UserName ?? throw new InvalidOperationException("UserName is null.");
            var role = user.CustomRole ?? "Staff"; 

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, username),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.Role, role)
            };

            var token = GenerateToken(authClaims);

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Username = username,
                Role = role,
                Expiration = token.ValidTo
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var userExists = await _userManager.FindByNameAsync(registerDto.Username);
            if (userExists != null)
            {
                throw new ApplicationException("User already exists.");
            }

            var user = new ApplicationUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                FullName = registerDto.FullName,
                CustomRole = registerDto.Role ?? "Staff",
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                throw new ApplicationException("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return await LoginAsync(new LoginDto
            {
                Username = registerDto.Username,
                Password = registerDto.Password
            });
        }

        private JwtSecurityToken GenerateToken(IEnumerable<Claim> claims)
        {
            var secret = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT secret is not configured.");
            var issuer = _configuration["Jwt:Issuer"] ?? "SibaSchoolApi";
            var audience = _configuration["Jwt:Audience"] ?? "SibaSchoolClient";


            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var authSigningKey = new SymmetricSecurityKey(keyBytes);

            return new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires: DateTime.UtcNow.AddHours(3),
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
        }
    }
}
