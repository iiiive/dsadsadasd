using ApungLourdesWebApi.Models;

namespace ApungLourdesWebApi.DTOs
{
    public class UserDto
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public int RoleId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public virtual ICollection<Donation> Donations { get; set; } = new List<Donation>();

        public virtual Role Role { get; set; } = null!;


    }
}
