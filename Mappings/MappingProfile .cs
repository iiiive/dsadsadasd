using ApungLourdesWebApi.DTOs;
using ApungLourdesWebApi.Models;
using AutoMapper;

namespace ApungLourdesWebApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            //CreateMap<User, UserDto>()
            //    .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName));

            //CreateMap<Donation, DonationDto>()
            //    .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User!.FullName));

            //CreateMap<Amanu, AmanuDto>();
            //CreateMap<Serviceschedule, ServiceScheduleDto>();
            //CreateMap<Serviceschedulerequirement, ServiceScheduleRequirementDto>();
            //CreateMap<Documentrequest, DocumentRequestDto>();

            //// Reverse maps for create/update
            //CreateMap<UserDto, User>();
            //CreateMap<AmanuDto, Amanu>();
            //CreateMap<ServiceScheduleDto, Serviceschedule>();
            //CreateMap<ServiceScheduleRequirementDto, Serviceschedulerequirement>();
            //CreateMap<DocumentRequestDto, Documentrequest>();

            // Amanu
            CreateMap<Amanu, AmanuDto>().ReverseMap();

            // ServiceSchedules
            CreateMap<Serviceschedule, ServiceScheduleDto>().ReverseMap();

            // ServiceScheduleRequirements
            CreateMap<Serviceschedulerequirement, ServiceScheduleRequirementDto>().ReverseMap();

            // DocumentRequests
            CreateMap<Documentrequest, DocumentRequestDto>().ReverseMap();
        }
    }
}
