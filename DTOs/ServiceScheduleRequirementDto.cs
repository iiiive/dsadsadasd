namespace ApungLourdesWebApi.DTOs
{
    public class ServiceScheduleRequirementDto
    {
        public int Id { get; set; }

        public int ServiceScheduleId { get; set; }

        public string? RequirementType { get; set; }
        public string? FilePath { get; set; }


        public string CreatedBy { get; set; } = null!;

        public string? ModifiedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}
