using MeetInSport.Application.Interfaces.Repositories;
using MeetInSport.Domain.Entities;
using MeetInSport.Infrastructure.Persistance;
using MeetInSport.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;


public class ReservationRepository : GenericRepository<Reservation>, IReservationRepository
{

    public ReservationRepository(AppDbContext context) : base(context)
    { }
    public async Task<IReadOnlyList<Reservation>> GetReservationsByUserIdAsync(Guid userId)
    {
        // Include() is used to load related entities (Coach and Package) when fetching reservations for a specific user. This allows you to access the details of the coach and package associated with each reservation without needing additional queries.
        return await _dbSet.Include(r => r.Coach)
        .Include(p => p.Package)
        .Where(u => u.StudentId == userId).ToListAsync();
    }
    public async Task<IReadOnlyList<Reservation>> GetReservationsByCoachIdAsync(Guid coachId)
    {
        //  Include() is used to load related entities (Student and Package) when fetching reservations for a specific coach. This allows you to access the details of the student and package associated with each reservation without needing additional queries.
        return await _dbSet.Include(r => r.Student)
        .Include(p => p.Package)
        .Where(c => c.CoachId == coachId).ToListAsync();
    }
}