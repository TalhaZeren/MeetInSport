namespace MeetInSport.Application.DTOs.Coach;

public class UpdateCoachProfileDto
{
    public string Sport { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public decimal HourlyRate { get; set; }
    public int Experience { get; set; }
    public string? Location { get; set; }
    public string? Iban { get; set; }
}
