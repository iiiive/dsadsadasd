using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApungLourdesWebApi.DTOs;
using ApungLourdesWebApi.Models;
using ApungLourdesWebApi.Repositories;
using ApungLourdesWebApi.Services.Interfaces;
using AutoMapper;

namespace ApungLourdesWebApi.Services.Implementations
{
    public class ServiceScheduleService : IServiceScheduleService
    {
        private readonly IRepository<Serviceschedule> _repo;
        private readonly IServiceScheduleRepository _serviceScheduleRepository;
        private readonly IMapper _mapper;

        public ServiceScheduleService(
            IRepository<Serviceschedule> repo,
            IServiceScheduleRepository serviceScheduleRepository,
            IMapper mapper)
        {
            _repo = repo;
            _serviceScheduleRepository = serviceScheduleRepository;
            _mapper = mapper;
        }

        // =====================
        // CRUD for Schedules
        // =====================

        public async Task<IEnumerable<ServiceScheduleDto>> GetAllAsync()
        {
            var all = await _repo.GetAllAsync();

            // ✅ exclude soft-deleted by default
            all = all.Where(x => x.IsDeleted != true).ToList();

            return _mapper.Map<IEnumerable<ServiceScheduleDto>>(all);
        }

        public async Task<ServiceScheduleDto?> GetByIdAsync(int id, bool includeRequirements = false)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            var dto = _mapper.Map<ServiceScheduleDto>(entity);

            if (includeRequirements)
            {
                var reqs = await _serviceScheduleRepository.GetRequirementsAsync(id);
                dto.Requirements = _mapper.Map<List<ServiceScheduleRequirementDto>>(reqs);
            }

            return dto;
        }

        public async Task<ServiceScheduleDto> AddAsync(ServiceScheduleDto dto)
        {
            // ✅ normalize ServiceType to match ENUM in DB
            if (!string.IsNullOrWhiteSpace(dto.ServiceType))
                dto.ServiceType = dto.ServiceType.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(dto.Status))
                dto.Status = "Pending";

            // created timestamps
            if (dto.CreatedAt == default)
                dto.CreatedAt = DateTime.UtcNow;

            // ✅ ensure not deleted by default
            dto.IsDeleted = false;
            dto.DeletedAt = null;
            dto.DeletedBy = null;

            var entity = _mapper.Map<Serviceschedule>(dto);

            var added = await _repo.AddAsync(entity);
            return _mapper.Map<ServiceScheduleDto>(added);
        }

        public async Task<ServiceScheduleDto?> UpdateAsync(int id, ServiceScheduleDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            // ✅ preserve ownership + created details
            var preservedUserId = entity.UserId;
            var preservedCreatedBy = entity.CreatedBy;
            var preservedCreatedAt = entity.CreatedAt;

            // ✅ normalize ServiceType to match ENUM in DB
            if (!string.IsNullOrWhiteSpace(dto.ServiceType))
                entity.ServiceType = dto.ServiceType.Trim().ToLowerInvariant();

            if (!string.IsNullOrWhiteSpace(dto.ClientFirstName)) entity.ClientFirstName = dto.ClientFirstName;
            if (!string.IsNullOrWhiteSpace(dto.ClientLastName)) entity.ClientLastName = dto.ClientLastName;
            if (!string.IsNullOrWhiteSpace(dto.ClientPhone)) entity.ClientPhone = dto.ClientPhone;
            if (!string.IsNullOrWhiteSpace(dto.ClientEmail)) entity.ClientEmail = dto.ClientEmail;

            if (dto.ServiceDate != default) entity.ServiceDate = dto.ServiceDate;
            if (!string.IsNullOrWhiteSpace(dto.ServiceTime)) entity.ServiceTime = dto.ServiceTime;

            // optional
            entity.Partner1FullName = dto.Partner1FullName;
            entity.Partner2FullName = dto.Partner2FullName;
            entity.SpecialRequests = dto.SpecialRequests;
            entity.AddressLine = dto.AddressLine;

            // ✅ status update (only if provided)
            if (!string.IsNullOrWhiteSpace(dto.Status))
                entity.Status = dto.Status.Trim();

            // ✅ allow modified fields
            if (!string.IsNullOrWhiteSpace(dto.ModifiedBy))
                entity.ModifiedBy = dto.ModifiedBy;

            entity.ModifiedAt = DateTime.UtcNow;

            // ✅ Only set delete flags when explicitly deleting
            if (dto.IsDeleted)
            {
                entity.IsDeleted = true;
                entity.DeletedAt = dto.DeletedAt ?? DateTime.UtcNow;
                entity.DeletedBy = dto.DeletedBy ?? (entity.DeletedBy ?? "admin");
                entity.Status = "Deleted";
            }

            // restore preserved
            entity.UserId = preservedUserId;
            entity.CreatedBy = preservedCreatedBy;
            entity.CreatedAt = preservedCreatedAt;

            var updated = await _repo.UpdateAsync(entity);
            return _mapper.Map<ServiceScheduleDto>(updated);
        }

        // ✅ SOFT DELETE
        public async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return;

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedBy = entity.DeletedBy ?? "admin";

            entity.Status = "Deleted";
            entity.ModifiedAt = DateTime.UtcNow;
            entity.ModifiedBy = "admin";

            await _repo.UpdateAsync(entity);
        }

        // =====================
        // Requirements handling
        // =====================

        public async Task<List<ServiceScheduleRequirementDto>> GetRequirementsAsync(int scheduleId)
        {
            var reqs = await _serviceScheduleRepository.GetRequirementsAsync(scheduleId);
            return _mapper.Map<List<ServiceScheduleRequirementDto>>(reqs);
        }

        public async Task<ServiceScheduleRequirementDto> AddRequirementAsync(int scheduleId, ServiceScheduleRequirementDto dto)
        {
            var entity = _mapper.Map<Serviceschedulerequirement>(dto);
            var saved = await _serviceScheduleRepository.AddRequirementAsync(scheduleId, entity);
            return _mapper.Map<ServiceScheduleRequirementDto>(saved);
        }

        public async Task<bool> DeleteRequirementAsync(int scheduleId, int reqId)
        {
            return await _serviceScheduleRepository.DeleteRequirementAsync(scheduleId, reqId);
        }
    }
}
