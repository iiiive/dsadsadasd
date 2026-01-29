using ApungLourdesWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ApungLourdesWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly ApunglourdesDbContext _db;
        private const int DASHBOARD_LIMIT = 10;

        public DashboardController(ApunglourdesDbContext db)
        {
            _db = db;
        }

        private int? GetUserIdFromToken()
        {
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                User.FindFirst("sub")?.Value;

            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }

        private string GetEmailFromToken()
        {
            return (User.FindFirst(ClaimTypes.Email)?.Value
                ?? User.FindFirst("email")?.Value
                ?? "").Trim().ToLower();
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyDashboard()
        {
            var userId = GetUserIdFromToken();
            if (userId == null) return Unauthorized("Invalid user token.");

            var email = GetEmailFromToken();
            if (string.IsNullOrWhiteSpace(email))
            {
                email = (await _db.Users
                    .Where(u => u.UserId == userId.Value)
                    .Select(u => u.Email)
                    .FirstOrDefaultAsync() ?? "")
                    .Trim().ToLower();
            }

            // DONATIONS
            var donationsQuery = _db.Donations.Where(d => d.UserId == userId.Value);
            var donationsCount = await donationsQuery.CountAsync();
            var donationsTotal = await donationsQuery.SumAsync(d => (decimal?)d.Amount) ?? 0;

            var recentDonations = await donationsQuery
                .OrderByDescending(d => d.CreatedAt)
                .Take(DASHBOARD_LIMIT)
                .Select(d => new
                {
                    d.DonationId,
                    d.DonationType,
                    d.CustomDonationType,
                    d.Amount,
                    d.ReferenceNo,
                    d.CreatedAt
                })
                .ToListAsync();

            // DOCUMENT REQUESTS
            var documentsQuery = _db.Documentrequests.Where(r => r.UserId == userId.Value);
            var documentsCount = await documentsQuery.CountAsync();

            var recentDocuments = await documentsQuery
                .OrderByDescending(r => r.CreatedAt)
                .Take(DASHBOARD_LIMIT)
                .Select(r => new
                {
                    r.Id,
                    r.DocumentType,
                    r.NumberOfCopies,
                    r.Status,
                    r.CreatedAt
                })
                .ToListAsync();

            // ✅ SERVICE SCHEDULES (include deleted too)
            var schedulesQuery = _db.Serviceschedules
                .Where(s => s.UserId == userId.Value);

            var schedulesCount = await schedulesQuery.CountAsync();

            var recentSchedules = await schedulesQuery
                .OrderByDescending(s => s.CreatedAt)
                .Take(DASHBOARD_LIMIT)
                .Select(s => new
                {
                    s.Id,
                    s.ServiceType,
                    s.ServiceDate,
                    s.ServiceTime,
                    s.Status,

                    // ✅ include deleted fields so frontend can display "Deleted"
                    s.IsDeleted,
                    s.DeletedAt,
                    s.DeletedBy,

                    s.CreatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                emailUsed = email,
                donations = new { count = donationsCount, totalAmount = donationsTotal, recent = recentDonations },
                documents = new { count = documentsCount, recent = recentDocuments },
                scheduling = new { count = schedulesCount, recent = recentSchedules }
            });
        }
    }
}
