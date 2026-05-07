using MeetInSport.Application.Interface.Repositories;
using MeetInSport.Domain.Entities;
using MeetInSport.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace MeetInSport.Infrastructure.Persistence.Repositories;

public class CoachRepository : GenericRepository<Coach>, ICoachRepository
{
    public CoachRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Coach>> GetCoachesBySportAsync(string sport)
    {
        return await _dbSet.Include(c => c.Sports).Where(c => c.Sports.Name.ToLower() == sport.ToLower() && c.IsApproved).ToListAsync();
    }

    public async Task<Coach?> GetCoachWithPackagesAsync(Guid coachId)
    {
        //.Include() is critical because It telss EF to load the related packages when fetching the coach.This is a sort of SQL JOIN operation.
        return await _dbSet
        .Include(c => c.Packages)
        .Include(c => c.User)
        .FirstOrDefaultAsync(c => c.Id == coachId);

        // We list the packages and User information on the packages according to the coachId by joining them.
    }

    public async Task<IReadOnlyList<Coach>> GetTopRatedCoachesAsync(int count)
    {
        return await _dbSet.Where(c => c.IsApproved)
        .OrderByDescending(c => c.AverageRating)
        .Take(count)
        .ToListAsync();
    }

    public async Task<IReadOnlyList<Coach>> GetAllCoachesWithDetailsAsync()
    {
        // We tell the EF core to do a SQL JOIN on the Users table!
        return await _dbSet.Include(c => c.User).Include(c => c.Sports).ToListAsync();
    }

    public new async Task<Coach?> GetByIdAsync(Guid id)
    {
        return await _dbSet.Include(c => c.User).Include(c => c.Sports).FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Coach?> GetCoachByUserIdAsync(Guid userId)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<Coach?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Set<Coach>()
        .Include(c => c.Sports)
        .Include(c => c.Packages)
        .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);
    }
}
