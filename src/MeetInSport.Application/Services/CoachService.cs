using AutoMapper;
using MeetInSport.Application.DTOs.Coach;
using MeetInSport.Application.Exceptions;
using MeetInSport.Application.Interface.Repositories;
using MeetInSport.Application.Interface.Services;
using MeetInSport.Domain.Entities;

namespace MeetInSport.Application.Services;

public class CoachService : ICoachService
{
    private readonly ICoachRepository _coachRepository;
    private readonly IMapper _mapper;
    public CoachService(ICoachRepository coachRepository, IMapper mapper)
    {
        _coachRepository = coachRepository;
        _mapper = mapper;
    }

    // By defining with Enumerable, We can actually iterate through the collection of coaches.
    public async Task<IEnumerable<CoachResponseDto>> GetAllCoachesAsync()
    {
        var coaches = await _coachRepository.GetAllCoachesWithDetailsAsync();
        return _mapper.Map<IEnumerable<CoachResponseDto>>(coaches);
    }

    public async Task<CoachResponseDto?> GetCoachByIdAsync(Guid coachId)
    {
        var coach = await _coachRepository.GetByIdAsync(coachId);
        if (coach == null) return null;

        return _mapper.Map<CoachResponseDto>(coach);
    }

    public async Task<IEnumerable<CoachResponseDto>> GetCoachesBySportAsync(string sport)
    {
        var coaches = await _coachRepository.GetCoachesBySportAsync(sport);
        if (coaches == null || !coaches.Any()) return []; // Simplified null check and return an empty array if no coaches are found.

        return _mapper.Map<IEnumerable<CoachResponseDto>>(coaches);
    }


    public async Task<CoachResponseDto> UpdateProfileAsync(Guid userId, UpdateCoachProfileDto updateCoachProfileDto)
    {
        var coach = await _coachRepository.GetCoachByUserIdAsync(userId) ?? throw new NotFoundException("Coach Profile", userId);

        coach.Sport = updateCoachProfileDto.Sport;
        coach.Bio = updateCoachProfileDto.Bio;
        coach.HourlyRate = updateCoachProfileDto.HourlyRate;
        coach.Experience = updateCoachProfileDto.Experience;
        coach.Location = updateCoachProfileDto.Location;
        coach.Iban = updateCoachProfileDto.Iban;
        coach.UpdatedAt = DateTime.UtcNow;

        _coachRepository.Update(coach);
        await _coachRepository.SaveChangesAsync();

        var updatedCoach = await _coachRepository.GetCoachByUserIdAsync(userId);
        return _mapper.Map<CoachResponseDto>(updatedCoach);
    }
}