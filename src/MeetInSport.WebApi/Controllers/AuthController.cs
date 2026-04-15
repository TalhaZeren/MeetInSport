using System.Runtime.Versioning;
using MeetInSport.Application.DTOs.Auth;
using MeetInSport.Application.Interface.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeetInSport.WebApi.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST : api/v1/auth/register
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto registerRequestDto)
    {
        var response = await _authService.RegisterAsync(registerRequestDto);
        return Created("", response);
    }

}