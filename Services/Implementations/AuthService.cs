using ApungLourdesWebApi.Models;
using ApungLourdesWebApi.Repositories;
using ApungLourdesWebApi.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApungLourdesWebApi.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User> _userRepo;
        private readonly IConfiguration _config;

        public AuthService(IRepository<User> userRepo, IConfiguration config)
        {
            _userRepo = userRepo;
            _config = config;
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var normalizedEmail = NormalizeEmail(email);
            var rawPassword = password ?? string.Empty;

            // ✅ use normalized email lookup
            var user = await _userRepo.GetAsync(u => u.Email == normalizedEmail);

            // ✅ invalid credentials (keep generic)
            if (user == null || string.IsNullOrWhiteSpace(user.PasswordHash) ||
                !BCrypt.Net.BCrypt.Verify(rawPassword, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Wrong email or password.");
            }

            // ✅ Declined accounts are blocked
            if (string.Equals(user.Status, "Declined", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Your account was declined. Please contact the administrator.");

            // ✅ Pending / Not approved accounts are blocked
            // treat null/empty status as Pending by default for safety
            var status = string.IsNullOrWhiteSpace(user.Status) ? "Pending" : user.Status;

            if (!user.IsApproved || string.Equals(status, "Pending", StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("Account not approved yet. Please wait for admin approval.");

            // ✅ JWT config
            var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT key missing.");
            var jwtIssuer = _config["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer missing.");
            var jwtAudience = _config["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience missing.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var roleName = user.RoleId == 1 ? "Admin" : "User";

            var claims = new List<Claim>
            {
                // ✅ best practice: include BOTH sub + nameidentifier so controllers can read either
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),

                new Claim(ClaimTypes.Name, user.FullName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Role, roleName),
                new Claim("roleId", user.RoleId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string NormalizeEmail(string? email)
        {
            // ✅ trim + lowercase so duplicates are prevented consistently
            return (email ?? string.Empty).Trim().ToLowerInvariant();
        }
    }
}
