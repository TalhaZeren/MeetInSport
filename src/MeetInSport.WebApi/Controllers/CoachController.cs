using MeetInSport.Application.DTOs.Coach;
using MeetInSport.Application.Interface.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeetInSport.WebApi.Controllers;

[ApiController]
[Route("api/v1/coaches")]
public class CoachController : ControllerBase
{
    private readonly ICoachService _coachService;

    public CoachController(ICoachService coachService)
    {
        _coachService = coachService;
    }

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
    [HttpGet("sport/{sport}")]
    public async Task<ActionResult<IEnumerable<CoachResponseDto>>> GetCoachesBySport(string sport)
    {
        var coach = await _coachService.GetCoachesBySportAsync(sport);
        return Ok(coach);
    }
}