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

        // -----------------------------
        // Helpers
        // -----------------------------
        private bool IsAdmin()
        {
            // supports both Role claim types
            return User.IsInRole("Admin")
                || User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin")
                || User.Claims.Any(c => c.Type == "role" && c.Value == "Admin");
        }

        private bool TryGetUserId(out int userId)
        {
            userId = 0;

            // Most common claim sources
            var raw =
                User.FindFirstValue("sub") ??                       // JWT standard subject
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??   // ASP.NET identity
                User.FindFirstValue("UserId") ??                    // custom
                User.FindFirstValue("userid");                      // custom alt

            return int.TryParse(raw, out userId);
        }

        // -----------------------------
        // Endpoints
        // -----------------------------

        // ✅ Admin can view all donations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DonationDto>>> GetAll()
        {
            if (!IsAdmin())
                return Forbid();

            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        // ✅ Admin can view any donation by id
        [HttpGet("{id:int}")]
        public async Task<ActionResult<DonationDto>> GetById(int id)
        {
            if (!IsAdmin())
                return Forbid();

            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        // ✅ User submits donation (ties to logged-in UserId)
        [HttpPost]
        public async Task<ActionResult<DonationDto>> Create([FromBody] CreateDonationDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid payload.");

            if (dto.Amount <= 0)
                return BadRequest("Amount must be greater than zero.");

            if (!TryGetUserId(out var userId))
                return Unauthorized("Invalid user token (no user id).");

            // ✅ Critical: pass userId to service so it sets Donations.UserId
            var created = await _service.AddAsync(userId, dto);

            // Best practice response (still safe even if you keep Ok())
            return CreatedAtAction(nameof(GetById), new { id = created.DonationId }, created);
        }

        // ✅ Admin only delete
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin())
                return Forbid();

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
