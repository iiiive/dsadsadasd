using ApungLourdesWebApi.DTOs;
using ApungLourdesWebApi.Models;
using ApungLourdesWebApi.Repositories;
using ApungLourdesWebApi.Services.Interfaces;
using AutoMapper;

namespace ApungLourdesWebApi.Services.Implementations
{
    public class DocumentRequestService : IDocumentRequestService
    {
        private readonly IRepository<Documentrequest> _repo;
        private readonly IMapper _mapper;

        public DocumentRequestService(IRepository<Documentrequest> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DocumentRequestDto>> GetAllAsync() =>
            _mapper.Map<IEnumerable<DocumentRequestDto>>(await _repo.GetAllAsync());

        public async Task<DocumentRequestDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<DocumentRequestDto>(entity);
        }

        public async Task<DocumentRequestDto> AddAsync(DocumentRequestDto dto)
        {
            var entity = _mapper.Map<Documentrequest>(dto);
            var added = await _repo.AddAsync(entity);
            return _mapper.Map<DocumentRequestDto>(added);
        }

        public async Task<DocumentRequestDto?> UpdateAsync(int id, DocumentRequestDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            _mapper.Map(dto, entity);
            var updated = await _repo.UpdateAsync(entity);
            return _mapper.Map<DocumentRequestDto>(updated);
        }

        public async Task DeleteAsync(int id) => await _repo.DeleteAsync(id);
    }
}
