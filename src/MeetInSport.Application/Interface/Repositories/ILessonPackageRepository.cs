using MeetInSport.Application.Interface.Repositories;
using MeetInSport.Domain.Entities;

namespace MeetInSport.Application.Interface.Repositories;

public interface ILessonPackageRepository : IGenericRepository<LessonPackage>
{
    Task<IReadOnlyList<LessonPackage>> GetPackagesByCoachIdAsync(Guid coachId);
}