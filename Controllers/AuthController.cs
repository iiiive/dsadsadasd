using ApungLourdesWebApi.DTOs;
using ApungLourdesWebApi.Models;
using ApungLourdesWebApi.Repositories;
using ApungLourdesWebApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace ApungLourdesWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IRepository<User> _userRepo;

        public AuthController(
            IAuthService authService,
            IRepository<User> userRepo
        )
        {
            _authService = authService;
            _userRepo = userRepo;
        }

        // =========================
        // LOGIN
        // =========================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var token = await _authService.LoginAsync(request.Email, request.Password);
                return Ok(new { token });
            }
            catch (InvalidOperationException ex)
            {
                // Declined
                return StatusCode(403, new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                // Pending or wrong credentials
                return Unauthorized(new { message = ex.Message });
            }
        }

        // =========================
        // PUBLIC REGISTER (NO LOGIN)
        // =========================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest(new { message = "All fields are required." });
            }

            var existingUser = await _userRepo.GetAsync(u => u.Email == dto.Email);
            if (existingUser != null)
            {
                return Conflict(new { message = "Email already exists." });
            }

            // SECURITY RULE:
            // Even if user selects Admin, it is STILL pending approval
            if (dto.RoleId != 1 && dto.RoleId != 2)
                dto.RoleId = 2;

            var user = new User
            {
                FullName = dto.FullName.Trim(),
                Email = dto.Email.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),

                RoleId = dto.RoleId,     // User or Admin
                IsApproved = false,      // ALWAYS false
                Status = "Pending",

                CreatedAt = DateTime.UtcNow
            };

            await _userRepo.AddAsync(user);

            return Ok(new
            {
                message = "Account created successfully. Please wait for admin approval."
            });
        }

    }
}
