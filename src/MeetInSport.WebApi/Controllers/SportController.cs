using MeetInSport.Application.Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MeetInSport.WebApi.Controllers;

[Route("api/v1/sports")]
[ApiController]
[AllowAnonymous]
public class SportController : ControllerBase
{
    private readonly ISportService _sportService;

    public SportController(ISportService sportService)
    {
        _sportService = sportService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSports()
    {
        var sports = await _sportService.GetAllSportsAsync();
        return Ok(sports);
    }
}