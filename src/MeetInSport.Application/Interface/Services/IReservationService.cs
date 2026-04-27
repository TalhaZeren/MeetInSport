using MeetInSport.Application.DTOs.Reservation;


namespace MeetInSport.Application.Interface.Services;

public interface IReservationService
{
    Task<ReservationResponseDto> CreateReservationAsync(CreateReservationDto createReservationDto, Guid studentId);
    Task<IEnumerable<ReservationResponseDto>> GetMyReservationsAsync(Guid userId, string role);
    Task<ReservationResponseDto> CancelReservationAsync(Guid reservationId, Guid userId, string role, CancelReservationDto cancelReservationDto);

}