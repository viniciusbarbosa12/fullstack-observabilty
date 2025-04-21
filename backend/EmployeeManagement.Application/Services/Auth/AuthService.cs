using EmployeeManagement.Application.Dtos.Auth;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Shared.Config;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmployeeManagement.Application.Service.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<AppUser> userManager,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            using var span = Telemetry.ActivitySource.StartActivity("Register User");

            _logger.LogInformation("Attempting to register user {Email}", dto.Email);
            span?.SetTag("user.email", dto.Email);

            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                span?.SetStatus(ActivityStatusCode.Error, "User creation failed");
                span?.SetTag("identity.errors", errors);
                _logger.LogWarning("Failed to register user {Email}: {Errors}", dto.Email, errors);
                throw new Exception(errors);
            }

            _logger.LogInformation("User {Email} registered successfully", dto.Email);

            return new AuthResponseDto
            {
                Email = user.Email,
                Token = await GenerateJwt(user)
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            using var span = Telemetry.ActivitySource.StartActivity("Login User");

            _logger.LogInformation("User login attempt: {Email}", dto.Email);
            span?.SetTag("user.email", dto.Email);

            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                span?.SetStatus(ActivityStatusCode.Error, "Invalid credentials");
                _logger.LogWarning("Invalid login for user {Email}", dto.Email);
                throw new Exception("Invalid credentials");
            }

            _logger.LogInformation("User {Email} logged in successfully", dto.Email);

            return new AuthResponseDto
            {
                Email = user.Email,
                Token = await GenerateJwt(user)
            };
        }

        private async Task<string> GenerateJwt(AppUser user)
        {
            using var span = Telemetry.ActivitySource.StartActivity("Generate JWT");

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            span?.SetTag("user.id", user.Id);
            span?.SetTag("user.roles", string.Join(",", roles));
            span?.SetTag("jwt.length", tokenString.Length);

            return tokenString;
        }
    }
}
