using AutoMapper;
using MeetInSport.Application.DTOs.Reservation;
using MeetInSport.Domain.Entities;

namespace MeetInSport.Application.Mappings;

public class ReservationProfile : Profile
{
    public ReservationProfile()
    {
        CreateMap<Reservation, ReservationResponseDto>()
        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
        .ForMember(dest => dest.LocationType, opt => opt.MapFrom(src => src.LocationType.ToString())); // int to string #LocationType.cs (Enum)
    }
}