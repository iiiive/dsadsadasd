using ApungLourdesWebApi.DTOs;
using ApungLourdesWebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApungLourdesWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DonationController : ControllerBase
    {
        private readonly IDonationService _service;

        public DonationController(IDonationService service)
        {
            _service = service;
        }

        private bool IsAdminOrSuperAdmin()
        {
            // supports standard + custom
            var role =
                User.FindFirstValue(ClaimTypes.Role) ??
                User.FindFirstValue("role") ??
                User.FindFirstValue("http://schemas.microsoft.com/ws/2008/06/identity/claims/role") ??
                "";

            return role.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                || role.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase);
        }

        private bool TryGetUserId(out int userId)
        {
            userId = 0;

            var raw =
                User.FindFirstValue("sub") ??
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("UserId") ??
                User.FindFirstValue("userid");

            return int.TryParse(raw, out userId);
        }

        // ✅ Admin/SuperAdmin can view all donations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DonationDto>>> GetAll()
        {
            if (!IsAdminOrSuperAdmin())
                return Forbid();

            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        // ✅ Admin/SuperAdmin can view any donation by id
        [HttpGet("{id:int}")]
        public async Task<ActionResult<DonationDto>> GetById(int id)
        {
            if (!IsAdminOrSuperAdmin())
                return Forbid();

            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        // ✅ User submits donation
        [HttpPost]
        public async Task<ActionResult<DonationDto>> Create([FromBody] CreateDonationDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid payload.");

            if (dto.Amount <= 0)
                return BadRequest("Amount must be greater than zero.");

            if (!TryGetUserId(out var userId))
                return Unauthorized("Invalid user token (no user id).");

            var created = await _service.AddAsync(userId, dto);
            return CreatedAtAction(nameof(GetById), new { id = created.DonationId }, created);
        }

        // ✅ Admin/SuperAdmin only delete
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdminOrSuperAdmin())
                return Forbid();

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
