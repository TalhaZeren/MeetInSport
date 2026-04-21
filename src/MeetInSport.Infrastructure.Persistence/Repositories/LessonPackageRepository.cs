using MeetInSport.Application.Interface.Repositories;
using MeetInSport.Domain.Entities;
using MeetInSport.Infrastructure.Persistance; // (Keeping your specific typo namespace!)
using Microsoft.EntityFrameworkCore;

namespace MeetInSport.Infrastructure.Persistence.Repositories;

public class LessonPackageRepository : GenericRepository<LessonPackage>, ILessonPackageRepository
{
    public LessonPackageRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<LessonPackage>> GetPackagesByCoachIdAsync(Guid coachId)
    {
        return await _dbSet.Where(p => p.CoachId == coachId).ToListAsync();
    }

}