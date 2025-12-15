using ApungLourdesWebApi.DTOs;

namespace ApungLourdesWebApi.Services.Interfaces
{
    public interface IAmanuService
    {
        Task<AmanuDto> AddAsync(AmanuDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<AmanuDto>> GetAllAsync();
        Task<AmanuDto?> GetByIdAsync(int id);
        Task<AmanuDto?> UpdateAsync(int id, AmanuDto dto);
    }
}