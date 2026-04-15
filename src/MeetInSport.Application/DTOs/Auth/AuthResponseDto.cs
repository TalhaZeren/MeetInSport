namespace MeetInSport.Application.DTOs.Auth;

public class AuthResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public string Sport { get; set; } = string.Empty;
}