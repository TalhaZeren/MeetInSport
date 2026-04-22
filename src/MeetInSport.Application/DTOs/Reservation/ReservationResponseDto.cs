namespace MeetInSport.Application.DTOs.Reservation;


public class ReservationResponseDto
{
    public Guid Id { get; set; }
    public Guid PackageId { get; set; }
    public Guid CoachId { get; set; }
    public DateTime ScheduleAt { get; set; }
    public string Status { get; set; } = string.Empty; // This a process between student and coach
    public string LocationType { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedtAt { get; set; } // Add this value coming from BaseEntity class every time. We need to get to know when is created.

}