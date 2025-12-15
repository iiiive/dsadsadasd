using ApungLourdesWebApi.DTOs;

namespace ApungLourdesWebApi.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto> CreateUserAsync(UserDto dto);
        Task<UserDto?> UpdateUserAsync(int id, UserDto dto);
        Task<bool> DeleteUserAsync(int id);
    }
}
