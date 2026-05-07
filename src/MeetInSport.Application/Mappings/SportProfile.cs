using AutoMapper;
using MeetInSport.Application.DTOs.Sports;
using MeetInSport.Domain.Entities;

namespace MeetInSport.Application.Mappings;

public class SportProfile : Profile
{
    public SportProfile()
    {
        CreateMap<Sports, SportResponseDto>();
    }
}
