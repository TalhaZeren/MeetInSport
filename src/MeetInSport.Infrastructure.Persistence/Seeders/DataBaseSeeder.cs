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
            // 1. Always apply pending migrations automatically when the app starts
            if (_context.Database.IsRelational())
            {
                await _context.Database.MigrateAsync();
                _logger.LogInformation("Database migrations applied successfully.");
            }

            // 2. Seed System Roles
            if (!await _context.Roles.AnyAsync())
            {
                _logger.LogInformation("Seeding default Roles...");
                await _context.Roles.AddRangeAsync(new List<Role>
                {
                    new Role { Id = 1, RoleName = "Admin", Description = "System Administrator" },
                    new Role { Id = 2, RoleName = "Coach", Description = "Professional Sports Coach" },
                    new Role { Id = 3, RoleName = "Student", Description = "Athlete / Student" }
                });
                await _context.SaveChangesAsync();
            }

            // 3. Seed System Sports (The new table we just created!)
            if (!await _context.Sports.AnyAsync())
            {
                _logger.LogInformation("Seeding supported Sports...");

                var defaultSports = new List<string>
                {
                    "Tennis", "Basketball", "Football", "Volleyball",
                    "Swimming", "Boxing", "Yoga", "Pilates",
                    "Golf", "Martial Arts", "Running", "Cycling",
                    "Chess", "Table Tennis", "Badminton"
                };

                var sportsToAdd = defaultSports.Select(sportName => new Sports
                {
                    Id = Guid.NewGuid(),
                    Name = sportName,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                await _context.Sports.AddRangeAsync(sportsToAdd);
                await _context.SaveChangesAsync();
            }

            _logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }
}