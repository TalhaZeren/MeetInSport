using MeetInSport.Application.Interface.Repositories;
using MeetInSport.Domain.Entities;

namespace MeetInSport.Application.Interface.Repositories;

public interface IReservationRepository : IGenericRepository<Reservation>
{
    Task<IReadOnlyList<Reservation>> GetReservationsByUserIdAsync(Guid userId);
    Task<IReadOnlyList<Reservation>> GetReservationsByCoachIdAsync(Guid coachId);

}