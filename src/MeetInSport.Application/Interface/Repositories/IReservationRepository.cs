using MeetInSport.Application.Interface.Repositories;
using MeetInSport.Domain.Entities;

public interface IReservationRepository : IGenericRepository<Reservation>
{
    Task<IReadOnlyList<Reservation>> GetReservationsByUserIdAsync(Guid userId);
    Task<IReadOnlyList<Reservation>> GetReservationsByCoachIdAsync(Guid coachId);

}