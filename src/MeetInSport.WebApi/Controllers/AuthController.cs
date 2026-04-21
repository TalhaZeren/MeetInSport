using System.Runtime.Versioning;
using MeetInSport.Application.DTOs.Auth;
using MeetInSport.Application.Interface.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Add this!

namespace MeetInSport.WebApi.Controllers;

[Authorize]
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
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto registerRequestDto)
    {
        var response = await _authService.RegisterAsync(registerRequestDto);
        return Created("", response);
    }
    // POST : api/v1/auth/login
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto loginRequestDto)
    {
        var response = await _authService.LoginAsync(loginRequestDto);
        return Ok(response);
    }
}