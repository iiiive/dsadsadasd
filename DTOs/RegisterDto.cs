namespace ApungLourdesWebApi.DTOs
{
    public class RegisterDto
    {
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";

        public int RoleId { get; set; } = 2; // 1=Admin, 2=User (allowed)

        public string? CompleteAddress { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
