using AutoMapper;
using MeetInSport.Application.DTOs.Coach;
using MeetInSport.Domain.Entities;

namespace MeetInSport.Application.Mappings;

public class CoachProfile : Profile
{
    public CoachProfile()
    {
        CreateMap<Coach, CoachResponseDto>()
        .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User != null ? src.User.Name : "Unknown Coach"))
        .ForMember(dest => dest.Sport, opt => opt.MapFrom(src => src.Sports.Name))
        .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User != null ? src.User.Email : "No Email"));


        // Most properties have the same name, so AutoMapper can handle them automatically. But "FullName" is different, so we need to specify how to map it.
    }
}