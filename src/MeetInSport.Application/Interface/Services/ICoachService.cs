using MeetInSport.Application.DTOs.Coach;
using MeetInSport.Domain.Entities;

namespace MeetInSport.Application.Interface.Services;

public interface ICoachService
{
    Task<IEnumerable<CoachResponseDto>> GetAllCoachesAsync();
    Task<CoachResponseDto?> GetCoachByIdAsync(Guid coachId);
    Task<IEnumerable<CoachResponseDto>> GetCoachesBySportAsync(string sport);

}



