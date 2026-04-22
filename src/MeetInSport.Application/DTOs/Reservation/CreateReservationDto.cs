using MeetInSport.Domain.Entities;
using MeetInSport.Domain.Enum;


namespace MeetInSport.Application.DTOs.Reservation;


public class CreateReservationDto
{
    public Guid PackageId { get; set; }
    public DateTime ScheduleAt { get; set; }
    public LocationType LocationType { get; set; }
    public string? Notes { get; set; }

}