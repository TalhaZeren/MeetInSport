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

}