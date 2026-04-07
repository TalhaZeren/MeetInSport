using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeetInSport.Domain.Entities;


namespace MeetInSport.Infrastructure.Persistence.Configurations;

public class CoachConfiguration : IEntityTypeConfiguration<Coach>
{
    public void Configure(EntityTypeBuilder<Coach> builder)
    {
        builder.Property(c => c.Sport).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Location).IsRequired().HasMaxLength(255);
        builder.Property(c => c.Iban).HasMaxLength(34); // Ibans Are max 34 characters long.
        builder.Property(c => c.Bio).HasMaxLength(1000);
        builder.Property(c => c.HourlyRate).HasPrecision(18, 2);
        builder.Property(c => c.AverageRating).HasPrecision(3, 2);

        builder.HasMany(c => c.Packages)
        .WithOne(p => p.Coach)
        .HasForeignKey(p => p.CoachId)
        .OnDelete(DeleteBehavior.Cascade);

    }
}




