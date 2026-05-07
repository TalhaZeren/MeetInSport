using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeetInSport.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/images")]
public class ImageController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly IHttpContextAccessor _httpContextAccessor;

    // We inject IWebHostEnvironment to know where to save the file on the server
    public ImageController(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
    {
        _environment = environment;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file) // This parameter must be matched with the "file" parameter in React Service 
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No file was uploaded." });
        }

        // 1. Create a unique filename so images don't overwrite each other
        var extension = Path.GetExtension(file.FileName);
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";

        // 2. Define the path: wwwroot/uploads
        var uploadsFolder = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
        
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        // 3. Save the file to the server
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // 4. This is for Frontend to generate the public URL to return to React
        var request = _httpContextAccessor.HttpContext!.Request;
        var baseUrl = $"{request.Scheme}://{request.Host.Value}";
        var imageUrl = $"{baseUrl}/uploads/{uniqueFileName}";

        // Return the URL as a JSON object
        return Ok(new { url = imageUrl });
    }
}