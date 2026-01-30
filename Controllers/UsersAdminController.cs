using ApungLourdesWebApi.Models;
using ApungLourdesWebApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApungLourdesWebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/admin/users")]
    public class AdminUsersController : ControllerBase
    {
        private readonly IRepository<User> _userRepo;

        public AdminUsersController(IRepository<User> userRepo)
        {
            _userRepo = userRepo;
        }

        // ✅ GET: /api/admin/users/pending
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingUsers()
        {
            if (!IsAdminOrSuperAdmin()) return Forbid();

            var users = await _userRepo.GetAllAsync();

            var pending = users
                .Where(u => (u.Status ?? "").Trim().Equals("Pending", StringComparison.OrdinalIgnoreCase))
                .Select(u => new
                {
                    u.UserId,
                    u.FullName,
                    u.Email,
                    u.RoleId,
                    u.CreatedAt,
                    u.IsApproved,
                    u.Status
                })
                .OrderByDescending(u => u.CreatedAt);

            return Ok(pending);
        }

        // ✅ PUT: /api/admin/users/{id}/approve
        [HttpPut("{id:int}/approve")]
        public async Task<IActionResult> ApproveUser(int id)
        {
            if (!IsAdminOrSuperAdmin()) return Forbid();

            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found." });

            user.IsApproved = true;
            user.Status = "Approved";
            user.ModifiedAt = DateTime.UtcNow;

            await _userRepo.UpdateAsync(user);

            return Ok(new { message = "User approved." });
        }

        // ✅ PUT: /api/admin/users/{id}/decline  -> HARD DELETE FROM DB
        [HttpPut("{id:int}/decline")]
        public async Task<IActionResult> DeclineUser(int id)
        {
            if (!IsAdminOrSuperAdmin()) return Forbid();

            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found." });

            await _userRepo.DeleteAsync(id);
            return Ok(new { message = "User declined and deleted." });
        }

        private bool IsAdminOrSuperAdmin()
        {
            var role =
                User.FindFirst(ClaimTypes.Role)?.Value ??
                User.FindFirst("role")?.Value ??
                User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value ??
                "";

            return role.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                || role.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
