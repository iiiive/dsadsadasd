using System;
using System.Collections.Generic;

namespace ApungLourdesWebApi.DTOs
{
    public class ServiceScheduleDto
    {
        public int Id { get; set; }

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

        // 🔹 NEW: status persisted in DB and used by Angular
        //   ("Pending", "Approved", etc.)
        public string Status { get; set; } = "Pending";

        public string CreatedBy { get; set; } = null!;

        public string? ModifiedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        // optional list of requirements (only populated if needed)
        public List<ServiceScheduleRequirementDto>? Requirements { get; set; }
    }
}
