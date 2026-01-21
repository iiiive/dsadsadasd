namespace ApungLourdesWebApi.DTOs
{
    // What admin will see (list/view)
    public class DonationDto
    {
        public int DonationId { get; set; }
        public int? UserId { get; set; }

        public decimal Amount { get; set; }
        public string DonationType { get; set; } = null!;
        public string? ReferenceNo { get; set; }
        public string? Remarks { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    // What USER will submit (Online Giving form)
    public class CreateDonationDto
    {
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = "GCash";

        public string? Message { get; set; }
        public string? ReferenceNo { get; set; } // optional
    }
}
