using System.Collections.Generic;
using System.Threading.Tasks;
using ApungLourdesWebApi.DTOs;

namespace ApungLourdesWebApi.Services.Interfaces
{
    public interface IServiceScheduleService
    {
        Task<ServiceScheduleDto> AddAsync(ServiceScheduleDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<ServiceScheduleDto>> GetAllAsync();

        // ✅ Give default value so controller can call GetByIdAsync(id) or GetByIdAsync(id, true)
        Task<ServiceScheduleDto?> GetByIdAsync(int id, bool includeRequirements = false);

        Task<ServiceScheduleDto?> UpdateAsync(int id, ServiceScheduleDto dto);

        // Requirements
        Task<List<ServiceScheduleRequirementDto>> GetRequirementsAsync(int scheduleId);
        Task<ServiceScheduleRequirementDto> AddRequirementAsync(int scheduleId, ServiceScheduleRequirementDto dto);
        Task<bool> DeleteRequirementAsync(int scheduleId, int reqId);
    }
}
