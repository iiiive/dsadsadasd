using ApungLourdesWebApi.DTOs;

namespace ApungLourdesWebApi.Services.Interfaces
{
    public interface IDonationService
    {
        Task<IEnumerable<DonationDto>> GetAllAsync();
        Task<DonationDto?> GetByIdAsync(int id);

        Task<DonationDto> AddAsync(int userId, CreateDonationDto dto);

        // ✅ NEW (for PUT edit)
        Task<DonationDto?> UpdateAsync(int id, UpdateDonationDto dto);

        Task DeleteAsync(int id);
    }
}
