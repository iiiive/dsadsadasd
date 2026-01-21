using ApungLourdesWebApi.DTOs;
using ApungLourdesWebApi.Models;
using ApungLourdesWebApi.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApungLourdesWebApi.Services.Implementations
{
    public class DonationService : IDonationService
    {
        private readonly ApunglourdesDbContext _db;
        private readonly IMapper _mapper;

        public DonationService(ApunglourdesDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DonationDto>> GetAllAsync()
        {
            var donations = await _db.Donations
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DonationDto>>(donations);
        }

        public async Task<DonationDto?> GetByIdAsync(int id)
        {
            var donation = await _db.Donations.FirstOrDefaultAsync(d => d.DonationId == id);
            return donation == null ? null : _mapper.Map<DonationDto>(donation);
        }

        public async Task<DonationDto> AddAsync(int userId, CreateDonationDto dto)
        {
            // Donation table requires DonationType + Amount
            var donation = new Donation
            {
                UserId = userId,
                Amount = dto.Amount,
                DonationType = "OnlineGiving",
                ReferenceNo = dto.ReferenceNo,
                Remarks = dto.Message,
                CreatedAt = DateTime.UtcNow
            };

            _db.Donations.Add(donation);
            await _db.SaveChangesAsync(); // to get DonationId

            // Transaction table requires PaymentMethod + ReferenceNo (NOT NULL!)
            var tx = new Transaction
            {
                DonationId = donation.DonationId,
                PaymentMethod = dto.PaymentMethod,
                ReferenceNo = string.IsNullOrWhiteSpace(dto.ReferenceNo)
                    ? $"AUTO-{Guid.NewGuid():N}".ToUpper()
                    : dto.ReferenceNo!,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _db.Transactions.Add(tx);
            await _db.SaveChangesAsync();

            return _mapper.Map<DonationDto>(donation);
        }

        public async Task DeleteAsync(int id)
        {
            var donation = await _db.Donations.FirstOrDefaultAsync(d => d.DonationId == id);
            if (donation == null) return;

            // optional: delete related transactions first
            var txs = await _db.Transactions.Where(t => t.DonationId == id).ToListAsync();
            _db.Transactions.RemoveRange(txs);

            _db.Donations.Remove(donation);
            await _db.SaveChangesAsync();
        }
    }
}
