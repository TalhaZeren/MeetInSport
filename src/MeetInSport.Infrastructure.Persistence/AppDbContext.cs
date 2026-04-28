using Microsoft.EntityFrameworkCore;
using MeetInSport.Domain.Entities;
using System.Reflection;

namespace MeetInSport.Infrastructure.Persistance;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // The tables that is going to be created in the database.
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Coach> Coaches => Set<Coach>();
    public DbSet<LessonPackage> LessonPackages => Set<LessonPackage>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        // Global Query Filters is used to filter out deleted entities automatically.
        builder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        builder.Entity<Coach>().HasQueryFilter(c => !c.IsDeleted);
        builder.Entity<LessonPackage>().HasQueryFilter(p => !p.IsDeleted);
        builder.Entity<Reservation>().HasQueryFilter(r => !r.IsDeleted);
        builder.Entity<Payment>().HasQueryFilter(p => !p.IsDeleted);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
       foreach (var entry in ChangeTracker.Entries<MeetInSport.Domain.Common.BaseEntity>())
       {
           switch(entry.State){
            case EntityState.Added:
            
                entry.Entity.CreatedAt = DateTime.UtcNow;
                break;

            case EntityState.Modified:
                entry.Entity.UpdatedAt = DateTime.UtcNow;
                break;

            case EntityState.Deleted: 
                // First we change the process as Modified instead of Deleted 
                entry.State = EntityState.Modified;
                // we have assigned as true to IsDeleted.
                entry.Entity.IsDeleted = true;
                // we have assigned as DateTime.Now to UpdatedAt.
                entry.Entity.UpdatedAt = DateTime.UtcNow;
                break;
           }
       }
       return base.SaveChangesAsync(cancellationToken);
    }


}