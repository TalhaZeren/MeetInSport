using MeetInSport.Application.Interface.Repositories;
using MeetInSport.Domain.Entities;

namespace MeetInSport.Application.Interface.Repositories;

public interface ICoachRepository : IGenericRepository<Coach>
{
    // When required to fetch coaches with User information.
    Task<IReadOnlyList<Coach>> GetAllCoachesWithDetailsAsync();
    // For the mobile search Page.
    Task<IReadOnlyList<Coach>> GetCoachesBySportAsync(string sport);
    //For the detailed profile page
    Task<Coach?> GetCoachWithPackagesAsync(Guid coachId);
    // Top Rated Coaches for the homepage
    Task<IReadOnlyList<Coach>> GetTopRatedCoachesAsync(int count);
    Task<Coach?> GetCoachByUserIdAsync(Guid userId);
}
