namespace ApungLourdesWebApi.Models;

public partial class Serviceschedulerequirement
{
    public int Id { get; set; }

    public int ServiceScheduleId { get; set; }

    public string RequirementType { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public string? ModifiedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }
}
