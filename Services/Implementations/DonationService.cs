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
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Amount <= 0) throw new ArgumentException("Amount must be greater than zero.");
            if (string.IsNullOrWhiteSpace(dto.DonationType))
                throw new ArgumentException("DonationType is required.");

            // If DonationType == Other, require CustomDonationType
            if (dto.DonationType.Trim().Equals("Other", StringComparison.OrdinalIgnoreCase) &&
                string.IsNullOrWhiteSpace(dto.CustomDonationType))
            {
                throw new ArgumentException("CustomDonationType is required when DonationType is Other.");
            }

            // Ensure reference no if Transactions.ReferenceNo is NOT NULL
            var safeRefNo = string.IsNullOrWhiteSpace(dto.ReferenceNo)
                ? $"AUTO-{Guid.NewGuid():N}".ToUpper()
                : dto.ReferenceNo!.Trim();

            // -----------------------
            // 1) Insert Donation
            // -----------------------
            var donation = new Donation
            {
                UserId = userId,
                Amount = dto.Amount,
                DonationType = dto.DonationType.Trim(),
                CustomDonationType = string.IsNullOrWhiteSpace(dto.CustomDonationType)
                    ? null
                    : dto.CustomDonationType.Trim(),
                ReferenceNo = safeRefNo,
                Remarks = string.IsNullOrWhiteSpace(dto.Remarks) ? null : dto.Remarks.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _db.Donations.Add(donation);
            await _db.SaveChangesAsync(); // get DonationId

            // -----------------------
            // 2) Insert Transaction
            // -----------------------
            // dto.PaymentMethod was removed, so set a safe default.
            // Change "GCash" if your system uses another default.
            var tx = new Transaction
            {
                DonationId = donation.DonationId,
                PaymentMethod = "GCash",     // ✅ default (since DTO removed it)
                ReferenceNo = safeRefNo,     // ✅ NOT NULL-safe
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

            // delete related transactions first
            var txs = await _db.Transactions.Where(t => t.DonationId == id).ToListAsync();
            _db.Transactions.RemoveRange(txs);

            _db.Donations.Remove(donation);
            await _db.SaveChangesAsync();
        }
    }
}
