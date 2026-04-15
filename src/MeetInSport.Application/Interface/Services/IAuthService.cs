using MeetInSport.Application.DTOs.Auth;

namespace MeetInSport.Application.Interface.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequestDto);
    Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto);
}