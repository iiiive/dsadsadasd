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

        // ✅ FIX: Use EF query directly (avoids generic repo mapping pitfalls)
        public async Task<IEnumerable<DocumentRequestDto>> GetAllAsync()
        {
            var list = await _db.Documentrequests
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            // ✅ map after materializing
            return _mapper.Map<IEnumerable<DocumentRequestDto>>(list);
        }

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
            var entity = await _db.Documentrequests.FirstOrDefaultAsync(x => x.Id == id);
            return entity == null ? null : _mapper.Map<DocumentRequestDto>(entity);
        }

        public async Task<DocumentRequestDto> AddAsync(DocumentRequestDto dto)
        {
            var entity = _mapper.Map<Documentrequest>(dto);

            entity.Status = "Pending";
            entity.CreatedAt = DateTime.UtcNow;
            entity.ModifiedAt = DateTime.UtcNow;

            var added = await _repo.AddAsync(entity);
            return _mapper.Map<DocumentRequestDto>(added);
        }

        public async Task<DocumentRequestDto?> UpdateAsync(int id, DocumentRequestDto dto)
        {
            var entity = await _db.Documentrequests.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return null;

            _mapper.Map(dto, entity);
            entity.ModifiedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return _mapper.Map<DocumentRequestDto>(entity);
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
