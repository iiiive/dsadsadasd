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

            // Donations
            CreateMap<Donation, DonationDto>().ReverseMap();
        }
    }
}
