using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<ServiceScheduleDto>> GetAllAsync() =>
            _mapper.Map<IEnumerable<ServiceScheduleDto>>(await _repo.GetAllAsync());

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
            // 🔹 ensure status has a value
            if (string.IsNullOrWhiteSpace(dto.Status))
            {
                dto.Status = "Pending";
            }

            var entity = _mapper.Map<Serviceschedule>(dto);
            var added = await _repo.AddAsync(entity);
            return _mapper.Map<ServiceScheduleDto>(added);
        }

        public async Task<ServiceScheduleDto?> UpdateAsync(int id, ServiceScheduleDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            // 🔹 If no status was provided from client, keep existing status
            if (string.IsNullOrWhiteSpace(dto.Status))
            {
                dto.Status = entity.Status;
            }

            // Map dto → entity, including Status
            _mapper.Map(dto, entity);

            var updated = await _repo.UpdateAsync(entity);
            return _mapper.Map<ServiceScheduleDto>(updated);
        }

        public async Task DeleteAsync(int id) =>
            await _repo.DeleteAsync(id);

        // =====================
        // Requirements handling
        // =====================

        public async Task<List<ServiceScheduleRequirementDto>> GetRequirementsAsync(int scheduleId)
        {
            var reqs = await _serviceScheduleRepository.GetRequirementsAsync(scheduleId);
            return _mapper.Map<List<ServiceScheduleRequirementDto>>(reqs);
        }

        public async Task<ServiceScheduleRequirementDto> AddRequirementAsync(
            int scheduleId,
            ServiceScheduleRequirementDto dto)
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
