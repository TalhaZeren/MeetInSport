using System.Security.Claims;
using MeetInSport.Application.DTOs.Reservation;
using MeetInSport.Application.Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetInSport.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/reservation")]
public class ReservationController : ControllerBase
{

    private readonly IReservationService _reservationService;

    public ReservationController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }
    [HttpPost]
    public async Task<ActionResult<ReservationResponseDto>> CreateReservationAsync([FromBody] CreateReservationDto createReservationDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid studentId))
        {
            return Unauthorized(new { message = "Invalid tokeb claims." });
        }

        var response = await _reservationService.CreateReservationAsync(createReservationDto, studentId);
        return Created("", response);
    }

    [HttpGet("me")]
    public async Task<ActionResult<IEnumerable<ReservationResponseDto>>> GetMyReservation()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId) || string.IsNullOrEmpty(roleClaim))
        {
            return Unauthorized(new { message = "Invalid token claims." });
        }
        var reservations = await _reservationService.GetMyReservationsAsync(userId, roleClaim);
        return Ok(reservations);
    }
    [HttpPut("{id:guid}/cancel")]
    public async Task<ActionResult<ReservationResponseDto>> CancelReservationAsync(Guid id, [FromBody] CancelReservationDto cancelReservationDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId) || string.IsNullOrEmpty(roleClaim))
        {

            return Unauthorized(new { message = "Geçersiz Token Isteği" });
        }
        try
        {
            var response = await _reservationService.CancelReservationAsync(id, userId, roleClaim, cancelReservationDto);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

}