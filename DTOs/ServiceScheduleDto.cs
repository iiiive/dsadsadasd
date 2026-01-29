using System;
using System.Collections.Generic;

namespace ApungLourdesWebApi.DTOs
{
    public class ServiceScheduleDto
    {
        public int Id { get; set; }
        public int? UserId { get; set; }

        public string? ServiceType { get; set; }
        public string? ClientFirstName { get; set; }
        public string? ClientLastName { get; set; }
        public string? ClientPhone { get; set; }
        public string? ClientEmail { get; set; }

        public DateTime ServiceDate { get; set; }
        public string? ServiceTime { get; set; }

        public string? Partner1FullName { get; set; }
        public string? Partner2FullName { get; set; }

        public string? SpecialRequests { get; set; }
        public string? AddressLine { get; set; }

        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        // ✅ IMPORTANT
        public string? Status { get; set; } = "Pending";

        // ✅ SOFT DELETE FIELDS (Option A)
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        public List<ServiceScheduleRequirementDto>? Requirements { get; set; }
    }
}
