using MeetInSport.Domain.Entities;
using MeetInSport.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MeetInSport.Infrastructure.Persistence.Seeders;

public class DataBaseSeeder
{
    private readonly AppDbContext _context;
    private readonly ILogger<DataBaseSeeder> _logger;


    public DataBaseSeeder(AppDbContext context, ILogger<DataBaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            await _context.Database.MigrateAsync(); // Check if database is actually created and migrations are applied.

            if (!await _context.Set<Role>().AnyAsync())
            {
                _logger.LogInformation("Seeding role data into the database...");

                var studentRole = new Role { Id = 1, RoleName = "Student" };
                var coachRole = new Role { Id = 2, RoleName = "Coach" };
                var adminRole = new Role { Id = 3, RoleName = "Admin" };

                await _context.Set<Role>().AddRangeAsync(studentRole, coachRole, adminRole);
                await _context.SaveChangesAsync();
            }

            if (await _context.Set<User>().AnyAsync())
            {
                _logger.LogInformation("Database already seeded. Skipping user and coach seeding.");
                return;
            }

            _logger.LogInformation("Seeding initial data into the database...");

            const int coachRoleId = 2;

            var user1 = new User
            {
                Id = Guid.NewGuid(),
                Name = "Talha Zeren",
                Email = "talhazeren00@gmail.com",
                RoleId = coachRoleId,
                PasswordHash = "dummyHash123",
                CreatedAt = DateTime.UtcNow

            };
            var user2 = new User
            {
                Id = Guid.NewGuid(),
                Name = "Serena Williams",
                Email = "serena@meetinsport.com",
                RoleId = coachRoleId,
                PasswordHash = "dummyHash123",
                CreatedAt = DateTime.UtcNow
            };

            // 4. Coaches that linked to Users above is creating...

            var coach1 = new Coach
            {
                Id = Guid.NewGuid(),
                UserId = user1.Id,
                User = user1,
                Sport = "Tennis",
                Bio = "3-tieme Grand Slam Champion",
                HourlyRate = 150.00m,
                Experience = 24,
                AverageRating = 4.9m,
                IsApproved = true,
                Location = "Istanbul, Turkey"
            };
            var coach2 = new Coach
            {
                Id = Guid.NewGuid(),
                UserId = user2.Id,
                User = user2,
                Sport = "Tennis",
                Bio = "23-time Grand Slam Champion.",
                HourlyRate = 200.00m,
                Experience = 27,
                AverageRating = 5.0m,
                IsApproved = true,
                Location = "Los Angeles, USA"
            };

            await _context.Set<User>().AddRangeAsync(user1, user2);
            await _context.Set<Coach>().AddRangeAsync(coach1, coach2);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Datavase seeding completed succesfully!");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while seeding the database.");
            throw;

        }

    }
}