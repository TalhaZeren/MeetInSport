using MeetInSport.Domain.Enum;

namespace MeetInSport.Application.DTOs.LessonPackage;

public class LessonPackageResponseDto
{
    public Guid Id { get; set; }
    public Guid CoachId { get; set; } // We return it so the frontend knows who it belongs. After responsing, frontend side is able know who this package belongs to.
    public string PackageName { get; set; } = string.Empty;
    public string PackageDescription { get; set; } = string.Empty;
    public decimal DurationInMinutes { get; set; }
    public decimal PackagePrice { get; set; }
    public List<string> Requirements { get; set; } = new List<string>();
    public string LocationType { get; set; } = string.Empty;
    public string LessonModel { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public bool IsActive { get; set; }
}