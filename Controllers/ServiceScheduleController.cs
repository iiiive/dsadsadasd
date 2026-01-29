using ApungLourdesWebApi.DTOs;
using ApungLourdesWebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;
using System.IO;

namespace ApungLourdesWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceScheduleController : ControllerBase
    {
        private readonly IServiceScheduleService _service;
        private readonly IWebHostEnvironment _env;

        public ServiceScheduleController(IServiceScheduleService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }

        private int? GetUserIdFromToken()
        {
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                User.FindFirst("sub")?.Value;

            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }

        // GET: /api/ServiceSchedule
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(data);
        }

        // GET: /api/ServiceSchedule/5?includeRequirements=true
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] bool includeRequirements = false)
        {
            var data = await _service.GetByIdAsync(id, includeRequirements);
            if (data == null) return NotFound();
            return Ok(data);
        }

        // POST: /api/ServiceSchedule
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ServiceScheduleDto dto)
        {
            var userId = GetUserIdFromToken();
            if (userId == null) return Unauthorized("Invalid token.");

            dto.UserId = userId.Value;

            if (string.IsNullOrWhiteSpace(dto.Status))
                dto.Status = "Pending";

            var created = await _service.AddAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = created.Id, includeRequirements = false }, created);
        }

        // PUT: /api/ServiceSchedule/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ServiceScheduleDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        // PATCH: /api/ServiceSchedule/5/status
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] StatusUpdateDto body)
        {
            if (body == null || string.IsNullOrWhiteSpace(body.Status))
                return BadRequest("Status is required.");

            var existing = await _service.GetByIdAsync(id, includeRequirements: false);
            if (existing == null) return NotFound();

            existing.Status = body.Status.Trim();
            existing.ModifiedBy = "admin";
            existing.ModifiedAt = DateTime.UtcNow;

            var updated = await _service.UpdateAsync(id, existing);
            return Ok(updated);
        }

        // DELETE: /api/ServiceSchedule/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok(new { message = "Schedule soft-deleted." });
        }

        // ==========================================================
        // ✅ Wedding Requirement Photo Upload Endpoint (Swagger-safe)
        // POST: /api/ServiceSchedule/{scheduleId}/requirements/upload
        // Content-Type: multipart/form-data
        // fields: file, requirementType
        // ==========================================================
        [HttpPost("{scheduleId:int}/requirements/upload")]
        [RequestSizeLimit(20_000_000)]
        public async Task<IActionResult> UploadRequirement(int scheduleId)
        {
            // ✅ accept both key styles
            var file = Request.Form.Files.GetFile("File") ?? Request.Form.Files.GetFile("file");
            var requirementType = Request.Form["RequirementType"].FirstOrDefault()
                               ?? Request.Form["requirementType"].FirstOrDefault();

            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded (expected key: File or file)." });

            if (string.IsNullOrWhiteSpace(requirementType))
                return BadRequest(new { message = "RequirementType is missing (expected key: RequirementType or requirementType)." });

            var allowedExt = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExt.Contains(ext))
                return BadRequest(new { message = "Invalid file type. Allowed: JPG, PNG, WEBP." });

            var uploadsRoot = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "schedules", scheduleId.ToString());
            Directory.CreateDirectory(uploadsRoot);

            var safeType = new string(requirementType.Trim().Where(char.IsLetterOrDigit).ToArray());
            if (string.IsNullOrWhiteSpace(safeType)) safeType = "requirement";

            var fileName = $"{safeType}_{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid():N}{ext}";
            var filePath = Path.Combine(uploadsRoot, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativeUrl = $"/uploads/schedules/{scheduleId}/{fileName}";
            var absoluteUrl = $"{Request.Scheme}://{Request.Host}{relativeUrl}";

            return Ok(new
            {
                scheduleId,
                requirementType,
                fileName,
                url = absoluteUrl,
                path = relativeUrl
            });
        }


    }

    public class StatusUpdateDto
    {
        public string? Status { get; set; }
    }

    // ✅ Swagger-friendly form model for file upload
    public class RequirementUploadForm
    {
        public IFormFile File { get; set; } = default!;
        public string RequirementType { get; set; } = string.Empty;
    }
}
