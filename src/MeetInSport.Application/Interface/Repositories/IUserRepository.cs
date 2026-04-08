using MeetInSport.Application.Interface.Repositories;
using MeetInSport.Domain.Entities;
namespace MeetInSport.Application.Interface.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<bool> IsEmailUniqueAsync(string email);
}