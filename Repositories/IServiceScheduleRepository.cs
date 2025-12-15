using ApungLourdesWebApi.Models;

namespace ApungLourdesWebApi.Repositories
{
    public interface IServiceScheduleRepository : IRepository<Serviceschedule>
    {
        Task<List<Serviceschedulerequirement>> GetRequirementsAsync(int scheduleId);
        Task<Serviceschedulerequirement> AddRequirementAsync(int scheduleId, Serviceschedulerequirement req);
        Task<bool> DeleteRequirementAsync(int scheduleId, int reqId);
    }
}
