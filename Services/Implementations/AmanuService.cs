using ApungLourdesWebApi.DTOs;
using ApungLourdesWebApi.Models;
using ApungLourdesWebApi.Repositories;
using ApungLourdesWebApi.Services.Interfaces;
using AutoMapper;

namespace ApungLourdesWebApi.Services.Implementations
{
    public class AmanuService : IAmanuService
    {
        private readonly IRepository<Amanu> _repo;
        private readonly IMapper _mapper;

        public AmanuService(IRepository<Amanu> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AmanuDto>> GetAllAsync() =>
            _mapper.Map<IEnumerable<AmanuDto>>(await _repo.GetAllAsync());

        public async Task<AmanuDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<AmanuDto>(entity);
        }

        public async Task<AmanuDto> AddAsync(AmanuDto dto)
        {
            var entity = _mapper.Map<Amanu>(dto);
            var added = await _repo.AddAsync(entity);
            return _mapper.Map<AmanuDto>(added);
        }

        public async Task<AmanuDto?> UpdateAsync(int id, AmanuDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            _mapper.Map(dto, entity);
            var updated = await _repo.UpdateAsync(entity);
            return _mapper.Map<AmanuDto>(updated);
        }

        public async Task DeleteAsync(int id) => await _repo.DeleteAsync(id);
    }
}
