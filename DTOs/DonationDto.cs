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
        // REQUIRED
        public decimal Amount { get; set; }

        // REQUIRED: matches DB column DonationType
        // e.g. "Offering", "Tithe", "Other"
        public string DonationType { get; set; } = null!;

        // OPTIONAL: only used when DonationType == "Other"
        public string? CustomDonationType { get; set; }

        // OPTIONAL
        public string? ReferenceNo { get; set; }

        // OPTIONAL: maps to Remarks column
        public string? Remarks { get; set; }
    }
}
