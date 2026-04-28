using AutoMapper;
using MeetInSport.Application.DTOs.LessonPackage;
using MeetInSport.Application.Exceptions;
using MeetInSport.Application.Interface.Repositories;
using MeetInSport.Application.Interface.Services;
using MeetInSport.Domain.Entities;
namespace MeetInSport.Application.Services;


public class LessonPackageService : ILessonPackageService
{
    private readonly ILessonPackageRepository _lessonPackageRepository;
    private readonly ICoachRepository _coachRepository;
    private readonly IMapper _mapper;

    public LessonPackageService(ILessonPackageRepository lessonPackageRepository, ICoachRepository coachRepository, IMapper mapper)
    {
        _lessonPackageRepository = lessonPackageRepository;
        _coachRepository = coachRepository;
        _mapper = mapper;
    }

    public async Task<LessonPackageResponseDto> CreatePackageAsync(CreateLessonPackageDto createLessonPackageDto, Guid currentUserId)
    {
        var coach = await _coachRepository.GetCoachByUserIdAsync(currentUserId)
        ?? throw new Exception("Only registered coaches can create lesson packages.");

        var packageEntity = new LessonPackage
        {
            Id = Guid.NewGuid(),
            CoachId = coach.Id, // Safely assigned
            PackageName = createLessonPackageDto.PackageName,
            PackageDescription = createLessonPackageDto.PackageDescription,
            DurationInMinutes = createLessonPackageDto.DurationInMinutes,
            PackagePrice = createLessonPackageDto.PackagePrice,
            Requirements = createLessonPackageDto.Requirements,
            LocationType = createLessonPackageDto.LocationType,
            LessonModel = createLessonPackageDto.LessonModel,
            CoverImageUrl = createLessonPackageDto.CoverImageUrl,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        // Saving package to database.
        await _lessonPackageRepository.AddAsync(packageEntity);
        await _lessonPackageRepository.SaveChangesAsync();

        // Return the safe response DTO
        return _mapper.Map<LessonPackageResponseDto>(packageEntity);
    }

    public async Task DeletePackageAsync(Guid packageId, Guid userId)
    {
        var package = await _lessonPackageRepository.GetByIdAsync(packageId) ?? throw new NotFoundException(nameof(LessonPackage), packageId);

        var coach = await _coachRepository.GetCoachByUserIdAsync(userId);

        if (coach == null || package.CoachId != coach.Id)
        {
            throw new UnauthorizedAccessException("Bu pakete sahip değilsiniz.");
        }
        // Even though we wrote Delete method which means package will be deleted,
        //  AddDbContext will understand what it means and will intercept it.
        _lessonPackageRepository.Delete(package);
        await _lessonPackageRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<LessonPackageResponseDto>> GetPackagesByCoachIdAsync(Guid coachId)
    {
        var packages = await _lessonPackageRepository.GetPackagesByCoachIdAsync(coachId);
        return _mapper.Map<IEnumerable<LessonPackageResponseDto>>(packages);
    }
}