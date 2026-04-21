using MeetInSport.Application.DTOs.LessonPackage;

namespace MeetInSport.Application.Interface.Services;

public interface ILessonPackageService
{
    Task<LessonPackageResponseDto> CreatePackageAsync(CreateLessonPackageDto createLessonPackageDto, Guid currentUserId);
    Task<IEnumerable<LessonPackageResponseDto>> GetPackagesByCoachIdAsync(Guid coachId);
}

