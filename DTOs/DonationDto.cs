namespace ApungLourdesWebApi.DTOs
{
    // ----------------------------
    // What ADMIN & DASHBOARD see
    // ----------------------------
    public class DonationDto
    {
        public int DonationId { get; set; }
        public int? UserId { get; set; }

        public decimal Amount { get; set; }

        public string DonationType { get; set; } = null!;

        // ✅ ADDED (matches DB)
        public string? CustomDonationType { get; set; }

        public string? ReferenceNo { get; set; }
        public string? Remarks { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    // ----------------------------
    // What USER submits (Online Giving)
    // ----------------------------
    public class CreateDonationDto
    {
        public decimal Amount { get; set; }
        public string DonationType { get; set; } = null!;
        public string? CustomDonationType { get; set; }
        public string? ReferenceNo { get; set; }
        public string? Remarks { get; set; }
    }

    // ----------------------------
    // ✅ NEW: What ADMIN updates (Edit modal)
    // Only allow safe fields to be edited
    // ----------------------------
    public class UpdateDonationDto
    {
        public decimal Amount { get; set; }
        public string? Remarks { get; set; }
    }
}
