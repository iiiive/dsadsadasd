using ApungLourdesWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ApungLourdesWebApi.Repositories
{
    public class ServiceScheduleRepository : Repository<Serviceschedule>, IServiceScheduleRepository
    {
        private readonly ApunglourdesDbContext _context;

        public ServiceScheduleRepository(ApunglourdesDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Serviceschedulerequirement>> GetRequirementsAsync(int scheduleId)
        {
            return await _context.Serviceschedulerequirements
                .Where(r => r.ServiceScheduleId == scheduleId)
            .ToListAsync();
        }

        public async Task<Serviceschedulerequirement> AddRequirementAsync(int scheduleId, Serviceschedulerequirement req)
        {
            req.ServiceScheduleId = scheduleId;
            _context.Serviceschedulerequirements.Add(req);
            await _context.SaveChangesAsync();
            return req;
        }

        public async Task<bool> DeleteRequirementAsync(int scheduleId, int reqId)
        {
            var req = await _context.Serviceschedulerequirements
                .FirstOrDefaultAsync(r => r.Id == reqId && r.ServiceScheduleId == scheduleId);

            if (req == null) return false;

            _context.Serviceschedulerequirements.Remove(req);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
