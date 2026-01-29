using ApungLourdesWebApi.DTOs;
using ApungLourdesWebApi.Models;
using ApungLourdesWebApi.Repositories;
using ApungLourdesWebApi.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApungLourdesWebApi.Services.Implementations
{
    public class DocumentRequestService : IDocumentRequestService
    {
        private readonly ApunglourdesDbContext _db;
        private readonly IRepository<Documentrequest> _repo;
        private readonly IMapper _mapper;

        public DocumentRequestService(
            ApunglourdesDbContext db,
            IRepository<Documentrequest> repo,
            IMapper mapper)
        {
            _db = db;
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DocumentRequestDto>> GetAllAsync()
            => _mapper.Map<IEnumerable<DocumentRequestDto>>(await _repo.GetAllAsync());

        public async Task<IEnumerable<DocumentRequestDto>> GetByUserIdAsync(int userId)
        {
            var list = await _db.Documentrequests
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DocumentRequestDto>>(list);
        }

        public async Task<DocumentRequestDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<DocumentRequestDto>(entity);
        }

        public async Task<DocumentRequestDto> AddAsync(DocumentRequestDto dto)
        {
            var entity = _mapper.Map<Documentrequest>(dto);

            entity.Status = "Pending";
            entity.ModifiedAt = DateTime.UtcNow;

            var added = await _repo.AddAsync(entity);
            return _mapper.Map<DocumentRequestDto>(added);
        }

        public async Task<DocumentRequestDto?> UpdateAsync(int id, DocumentRequestDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            _mapper.Map(dto, entity);
            entity.ModifiedAt = DateTime.UtcNow;

            var updated = await _repo.UpdateAsync(entity);
            return _mapper.Map<DocumentRequestDto>(updated);
        }

        public async Task<DocumentRequestDto?> UpdateStatusAsync(int id, string status)
        {
            var entity = await _db.Documentrequests.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return null;

            entity.Status = (status ?? "").Trim();
            entity.ModifiedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return _mapper.Map<DocumentRequestDto>(entity);
        }

        // ✅ SOFT DELETE: do NOT remove DB record
        public async Task DeleteAsync(int id)
        {
            var entity = await _db.Documentrequests.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return;

            entity.Status = "Deleted";
            entity.ModifiedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }

    }
}
