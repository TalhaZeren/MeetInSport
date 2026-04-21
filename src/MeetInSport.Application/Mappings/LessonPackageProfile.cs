using AutoMapper;
using MeetInSport.Application.DTOs.LessonPackage;
using MeetInSport.Domain.Entities;

namespace MeetInSport.Application.Mappings;

public class LessonPackageProfile : Profile
{

    public LessonPackageProfile()
    {
        CreateMap<LessonPackage, LessonPackageResponseDto>()
        .ForMember(dest => dest.LocationType, opt => opt.MapFrom(src => src.LocationType.ToString()))
        .ForMember(dest => dest.LessonModel, opt => opt.MapFrom(src => src.LessonModel.ToString()));
    }
}
