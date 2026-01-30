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

        private bool TryGetUserId(out int userId)
        {
            var userIdStr =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                User.FindFirst("nameid")?.Value ??
                User.FindFirst("sub")?.Value ??
                "";

            return int.TryParse(userIdStr, out userId);
        }

        private bool IsAdminOrSuperAdmin()
        {
            var role =
                User.FindFirst(ClaimTypes.Role)?.Value ??
                User.FindFirst("role")?.Value ??
                User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value ??
                "";

            return string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase)
                || string.Equals(role, "SuperAdmin", StringComparison.OrdinalIgnoreCase);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentRequestDto>>> GetAll()
        {
            if (!IsAdminOrSuperAdmin())
                return Forbid("Admins only.");

            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<DocumentRequestDto>>> GetMy()
        {
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                User.FindFirst("nameid")?.Value ??
                User.FindFirst("sub")?.Value ??
                User.FindFirst("id")?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user id in token.");

            var list = await _service.GetByUserIdAsync(userId);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentRequestDto>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<DocumentRequestDto>> Create(DocumentRequestDto dto)
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized("Invalid user id in token.");

            dto.UserId = userId;

            var item = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DocumentRequestDto>> Update(int id, DocumentRequestDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        // ✅ SOFT DELETE (Admin/SuperAdmin)
        [HttpDelete("{id}")]
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

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest body)
        {
            if (!IsAdminOrSuperAdmin())
                return Forbid("Admins only.");

            if (string.IsNullOrWhiteSpace(body.Status))
                return BadRequest("Status is required.");

            var updated = await _service.UpdateStatusAsync(id, body.Status.Trim());
            if (updated == null) return NotFound();

            return Ok(updated);
        }
    }
}
