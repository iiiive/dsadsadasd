namespace ApungLourdesWebApi.DTOs
{
    public class DocumentRequestDto
    {
        public int Id { get; set; }

        public string DocumentType { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string ContactPhone { get; set; } = null!;

        public string EmailAddress { get; set; } = null!;

        public string? PurposeOfRequest { get; set; }

        public string? NumberOfCopies { get; set; }

        public string? ChildName { get; set; }

        public DateOnly? DocumentDate { get; set; }

        public string? GroomsFullName { get; set; }

        public string? BridesFullName { get; set; }

        public string? Address { get; set; }

        public string? FullNameDeceased { get; set; }

        public string? RelationRequestor { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public string CreatedBy { get; set; } = null!;

        public string? ModifiedBy { get; set; }
    }
}
