using System.Security.Claims;
using MeetInSport.Application.DTOs.Coach;
using MeetInSport.Application.Interface.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace MeetInSport.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/coaches")]
public class CoachController : ControllerBase
{
    private readonly ICoachService _coachService;

    public CoachController(ICoachService coachService)
    {
        _coachService = coachService;
    }
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CoachResponseDto>>> GetAllCoaches()
    {
        var coaches = await _coachService.GetAllCoachesAsync();
        return Ok(coaches);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CoachResponseDto>> GetCoachById(Guid id)
    {
        var coach = await _coachService.GetCoachByIdAsync(id);
        if (coach == null)
        {
            return NotFound(new { message = "Coach not found." });
        }
        return Ok(coach);
    }
    // GET: api/v1/coaches/sport/{sport}
    [AllowAnonymous]
    [HttpGet("sport/{sport}")]
    public async Task<ActionResult<IEnumerable<CoachResponseDto>>> GetCoachesBySport(string sport)
    {
        var coach = await _coachService.GetCoachesBySportAsync(sport);
        return Ok(coach);
    }
    [HttpPut("profile")]
    public async Task<ActionResult<CoachResponseDto>> UpdateProfile([FromBody] UpdateCoachProfileDto updateCoachProfileDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            return Unauthorized(new { message = "Invalid token claims." });
        }
        var response = await _coachService.UpdateProfileAsync(userId, updateCoachProfileDto);
        return Ok(response);
    }

    [HttpGet("me")]
    [Authorize(Roles = "Coach")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            return Unauthorized(new { message = "Invalid token claims." });
        }
        var profile = await _coachService.GetMyProfileAsync(userId);
        return Ok(profile);
    }
}