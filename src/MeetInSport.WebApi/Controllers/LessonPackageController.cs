using System.Security.Claims;
using MeetInSport.Application.DTOs.LessonPackage;
using MeetInSport.Application.Interface.Services;
using MeetInSport.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;


namespace MeetInSport.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/packages")]
public class LessonPackageController : ControllerBase
{
    private readonly ILessonPackageService _lessonPackageService;

    public LessonPackageController(ILessonPackageService lessonPackageService)
    {
        _lessonPackageService = lessonPackageService;
    }

    [HttpPost]
    public async Task<ActionResult<LessonPackageResponseDto>> CreatePackageAsync([FromBody] CreateLessonPackageDto createLessonPackageDto)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // it is so important to get the id from JWT without getting from frontend to be able to hide datas from hacker attack.
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid currentUserId))
        {
            return Unauthorized(new { message = "Invalid token data" });
        }
        var response = await _lessonPackageService.CreatePackageAsync(createLessonPackageDto, currentUserId);
        return Created("", response);
    }

    // GET : api/v1/packages/coach/{coachId}

    [HttpGet("coach/{coachId:guid}")]
    public async Task<ActionResult<IEnumerable<LessonPackageResponseDto>>> GetPackagesByCoachIdAsync(Guid coachId)
    {

        var packages = await _lessonPackageService.GetPackagesByCoachIdAsync(coachId);
        return Ok(packages);
    }
    // DELETE: api/v1/packages/{id}
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeletePackage(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            return Unauthorized();
        }
        try
        {
            await _lessonPackageService.DeletePackageAsync(id, userId);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
    }
}