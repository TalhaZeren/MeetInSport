using MeetInSport.Application.Interface.Repositories;
using MeetInSport.Domain.Entities;
using MeetInSport.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace MeetInSport.Infrastructure.Persistence.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{

    public UserRepository(AppDbContext context) : base(context)
    { }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _dbSet.Include(r => r.Role)
        .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        return !await _dbSet.AnyAsync(u => u.Email == email);
    }

    

}