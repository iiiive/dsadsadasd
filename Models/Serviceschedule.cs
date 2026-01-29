using System;

namespace ApungLourdesWebApi.Models
{
    public partial class Serviceschedule
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public string ServiceType { get; set; } = null!;

        public string ClientFirstName { get; set; } = null!;

        public string ClientLastName { get; set; } = null!;

        public string ClientPhone { get; set; } = null!;

        public string ClientEmail { get; set; } = null!;

        public DateTime ServiceDate { get; set; }

        public string ServiceTime { get; set; } = null!;

        public string? Partner1FullName { get; set; }

        public string? Partner2FullName { get; set; }

        public string? SpecialRequests { get; set; }

        public string? AddressLine { get; set; }

        // 🔹 NEW: status column in DB
        public string Status { get; set; } = "Pending";

        public string CreatedBy { get; set; } = null!;

        public string? ModifiedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

    }
}
