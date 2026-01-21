using ApungLourdesWebApi.DTOs;
using ApungLourdesWebApi.Models;
using AutoMapper;

namespace ApungLourdesWebApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Users
            CreateMap<User, UserDto>().ReverseMap();

            // Amanu
            CreateMap<Amanu, AmanuDto>().ReverseMap();

            // ServiceSchedules
            CreateMap<Serviceschedule, ServiceScheduleDto>().ReverseMap();

            // ServiceScheduleRequirements
            CreateMap<Serviceschedulerequirement, ServiceScheduleRequirementDto>().ReverseMap();

            // DocumentRequests
            CreateMap<Documentrequest, DocumentRequestDto>().ReverseMap();

            // ✅ Donations (ADD THIS)
            // DonationDto should match your Donation model fields:
            // DonationId, UserId, Amount, DonationType, ReferenceNo, Remarks, CreatedAt, etc.
            CreateMap<Donation, DonationDto>().ReverseMap();

            // ✅ Transactions (OPTIONAL but recommended if you also create TransactionDto)
            // If you don't have TransactionDto, you can remove this part.
            // CreateMap<Transaction, TransactionDto>().ReverseMap();
        }
    }
}
