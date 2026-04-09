using AutoMapper;
using MeetInSport.Application.DTOs.Coach;
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
        var coaches = await _coachRepository.GetAllAsync();
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
}