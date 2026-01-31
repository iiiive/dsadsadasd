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
            return User.IsInRole("Admin")
                || User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin")
                || User.Claims.Any(c => c.Type == "role" && c.Value == "Admin");
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

        // -----------------------------
        // Endpoints
        // -----------------------------

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DonationDto>>> GetAll()
        {
            if (!IsAdmin())
                return Forbid();

            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DonationDto>> GetById(int id)
        {
            if (!IsAdmin())
                return Forbid();

            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

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

        // ✅ NEW: PUT for Edit (Admin only)
        [HttpPut("{id:int}")]
        public async Task<ActionResult<DonationDto>> Update(int id, [FromBody] UpdateDonationDto dto)
        {
            if (!IsAdmin())
                return Forbid();

            if (dto == null)
                return BadRequest("Invalid payload.");

            if (dto.Amount <= 0)
                return BadRequest("Amount must be greater than zero.");

            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

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
