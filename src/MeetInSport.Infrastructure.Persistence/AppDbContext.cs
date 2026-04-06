using Microsoft.EntityFrameworkCore;
using MeetInSport.Domain.Entities;
using System.Reflection;
using SportPlatform.Domain.Entities;

namespace MeetInSport.Infrastructure.Persistance;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // The tables that is going to be created in the database.
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Coach> Coaches => Set<Coach>();
    public DbSet<LessongPackage> LessongPackages => Set<LessongPackage>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder child)
    {
        base.OnModelCreating(child);

        child.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

}