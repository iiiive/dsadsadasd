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
    public class DocumentRequestController : ControllerBase
    {
        private readonly IDocumentRequestService _service;

        public DocumentRequestController(IDocumentRequestService service)
        {
            _service = service;
        }

        // ✅ more robust: supports multiple claim keys
        private bool TryGetUserId(out int userId)
        {
            userId = 0;

            var raw =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("nameid") ??
                User.FindFirstValue("sub") ??
                User.FindFirstValue("id") ??
                User.FindFirstValue("userid") ??
                User.FindFirstValue("UserId");

            return int.TryParse(raw, out userId);
        }

        private bool IsAdminOrSuperAdmin()
        {
            var role =
                User.FindFirstValue(ClaimTypes.Role) ??
                User.FindFirstValue("role") ??
                User.FindFirstValue("http://schemas.microsoft.com/ws/2008/06/identity/claims/role") ??
                "";

            return role.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                || role.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase);
        }

        // ✅ Admin/SuperAdmin: view all
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentRequestDto>>> GetAll()
        {
            if (!IsAdminOrSuperAdmin())
                return Forbid("Admins only.");

            return Ok(await _service.GetAllAsync());
        }

        // ✅ User: view own
        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<DocumentRequestDto>>> GetMy()
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { message = "Invalid user id in token." });

            var list = await _service.GetByUserIdAsync(userId);
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DocumentRequestDto>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        // ✅ Create (any logged-in user)
        [HttpPost]
        public async Task<ActionResult<DocumentRequestDto>> Create([FromBody] DocumentRequestDto dto)
        {
            // ApiController will auto 400 on invalid modelstate,
            // but we want clearer message.
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid payload.", errors = ModelState });

            if (!TryGetUserId(out var userId))
                return Unauthorized(new { message = "Invalid user id in token." });

            // ✅ attach ownership
            dto.UserId = userId;

            // ✅ set safe defaults so DB-required fields won't be null
            dto.DocumentType = (dto.DocumentType ?? "").Trim();
            dto.EmailAddress = (dto.EmailAddress ?? "").Trim();
            dto.ContactPhone = (dto.ContactPhone ?? "").Trim();
            dto.CreatedBy = string.IsNullOrWhiteSpace(dto.CreatedBy) ? "user" : dto.CreatedBy.Trim();
            dto.ModifiedBy = string.IsNullOrWhiteSpace(dto.ModifiedBy) ? null : dto.ModifiedBy.Trim();

            if (string.IsNullOrWhiteSpace(dto.DocumentType))
                return BadRequest(new { message = "DocumentType is required." });

            if (string.IsNullOrWhiteSpace(dto.EmailAddress))
                return BadRequest(new { message = "EmailAddress is required." });

            // ✅ default status
            if (string.IsNullOrWhiteSpace(dto.Status))
                dto.Status = "Pending";

            // ✅ timestamps (service will also set, but safe here)
            if (dto.CreatedAt == default)
                dto.CreatedAt = DateTime.UtcNow;
            dto.ModifiedAt = DateTime.UtcNow;

            var item = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<DocumentRequestDto>> Update(int id, [FromBody] DocumentRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid payload.", errors = ModelState });

            var updated = await _service.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        // ✅ SOFT DELETE (Admin/SuperAdmin)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdminOrSuperAdmin())
                return Forbid("Admins only.");

            await _service.DeleteAsync(id);
            return NoContent();
        }

        public class UpdateStatusRequest
        {
            public string Status { get; set; } = "";
        }

        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest body)
        {
            if (!IsAdminOrSuperAdmin())
                return Forbid("Admins only.");

            if (body == null || string.IsNullOrWhiteSpace(body.Status))
                return BadRequest(new { message = "Status is required." });

            var updated = await _service.UpdateStatusAsync(id, body.Status.Trim());
            if (updated == null) return NotFound();

            return Ok(updated);
        }
    }
}
