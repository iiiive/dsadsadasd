using ApungLourdesWebApi.DTOs;
using ApungLourdesWebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApungLourdesWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceScheduleController : ControllerBase
    {
        private readonly IServiceScheduleService _service;

        public ServiceScheduleController(IServiceScheduleService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceScheduleDto>>> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceScheduleDto>> GetById(int id, [FromQuery] bool includeRequirements = false)
        {
            var item = await _service.GetByIdAsync(id, includeRequirements);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceScheduleDto>> Create(ServiceScheduleDto dto)
        {
            var item = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ServiceScheduleDto>> Update(int id, ServiceScheduleDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("{id}/requirements")]
        public async Task<ActionResult<IEnumerable<ServiceScheduleRequirementDto>>> GetRequirements(int id)
        {
            return Ok(await _service.GetRequirementsAsync(id));
        }

        [HttpPost("{id}/requirements")]
        public async Task<ActionResult<ServiceScheduleRequirementDto>> AddRequirement(int id, [FromBody] ServiceScheduleRequirementDto dto)
        {
            var saved = await _service.AddRequirementAsync(id, dto);
            return CreatedAtAction(nameof(GetRequirements), new { id }, saved);
        }

        [HttpDelete("{id}/requirements/{reqId}")]
        public async Task<IActionResult> DeleteRequirement(int id, int reqId)
        {
            var success = await _service.DeleteRequirementAsync(id, reqId);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/requirements/upload")]
        public async Task<IActionResult> UploadRequirement(int id, IFormFile file, [FromForm] string requirementType)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Create uploads folder if not exists
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "requirements");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // Save file
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(folderPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Save DB record
            var requirement = new ServiceScheduleRequirementDto
            {
                ServiceScheduleId = id,
                RequirementType = requirementType, // "couple_picture" | "valid_id" | "certificate"
                FilePath = $"/uploads/requirements/{fileName}",
                CreatedBy = "test-user",
                CreatedAt = DateTime.UtcNow
            };

            await _service.AddRequirementAsync(id, requirement);           

            return Ok(requirement);
        }

    }
}
