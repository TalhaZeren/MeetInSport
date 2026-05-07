namespace MeetInSport.Application.DTOs.Coach;


public class CoachResponseDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty; // It is not a same name in the coach entity.
    public string Email { get; set; } = string.Empty;
    public string Sport { get; set; } = string.Empty;
    public string? Bio { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public int Experience { get; set; }
    public decimal AverageRating { get; set; }
    public string? Location { get; set; }

}