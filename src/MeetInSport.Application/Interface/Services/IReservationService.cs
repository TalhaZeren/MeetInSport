using MeetInSport.Application.DTOs.Reservation;


namespace MeetInSport.Application.Interface.Services;

public interface IReservationService
{
    Task<ReservationResponseDto> CreateReservationAsync(CreateReservationDto createReservationDto, Guid studentId);
}