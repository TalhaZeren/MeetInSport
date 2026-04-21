using MeetInSport.Domain.Enum;

namespace MeetInSport.Application.DTOs.LessonPackage;

public class CreateLessonPackageDto
{
    // We didn't add coachId in here because it will be taken from the JWT token.
    public string PackageName { get; set; } = string.Empty;
    public string PackageDescription { get; set; } = string.Empty;
    public decimal DurationInMinutes { get; set; }
    public decimal PackagePrice { get; set; }
    public List<string> Requirements { get; set; } = new List<string>();
    public LocationType LocationType { get; set; } = LocationType.CoachLocation;
    public LessonModel LessonModel { get; set; } = LessonModel.OneOnOne;
    public string? CoverImageUrl { get; set; }
}