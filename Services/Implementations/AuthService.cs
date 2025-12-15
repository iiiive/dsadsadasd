using ApungLourdesWebApi.Models;
using ApungLourdesWebApi.Repositories;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using ApungLourdesWebApi.Services.Interfaces;

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
            // Include Role in query
            var user = await _userRepo.GetAsync(u => u.Email == email);
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            if (user == null)
                throw new Exception("Invalid credentials");

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                throw new Exception("Invalid credentials");

            // Read JWT configuration
            var jwtKey = _config.GetValue<string>("Jwt:Key")
                         ?? throw new InvalidOperationException("JWT key missing");
            var jwtIssuer = _config.GetValue<string>("Jwt:Issuer")
                            ?? throw new InvalidOperationException("JWT issuer missing");
            var jwtAudience = _config.GetValue<string>("Jwt:Audience")
                               ?? throw new InvalidOperationException("JWT audience missing");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Add claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.RoleId == 1 ? "Admin" : "?User") // default to "user"
            };

            // Generate token
            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
