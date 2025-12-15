using ApungLourdesWebApi.DTOs;
using ApungLourdesWebApi.Models;
using ApungLourdesWebApi.Repositories;
using ApungLourdesWebApi.Services.Interfaces;
using AutoMapper;

namespace ApungLourdesWebApi.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _repo;
        private readonly IMapper _mapper;

        public UserService(IRepository<User> repo, IMapper mapper) { _repo = repo; _mapper = mapper; }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) return null;
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> CreateUserAsync(UserDto dto)
        {
            var user = _mapper.Map<User>(dto);
            var created = await _repo.AddAsync(user);
            return _mapper.Map<UserDto>(created);
        }

        public async Task<UserDto?> UpdateUserAsync(int id, UserDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;
            _mapper.Map(dto, existing);
            var updated = await _repo.UpdateAsync(existing);
            return _mapper.Map<UserDto>(updated);
        }

        public async Task<bool> DeleteUserAsync(int id) => await _repo.DeleteAsync(id);
    }
}
