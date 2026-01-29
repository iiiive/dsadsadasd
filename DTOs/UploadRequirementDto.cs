using Microsoft.AspNetCore.Http;

namespace ApungLourdesWebApi.DTOs
{
    public class UploadRequirementDto
    {
        public IFormFile File { get; set; } = null!;
        public string RequirementType { get; set; } = null!;
    }
}
