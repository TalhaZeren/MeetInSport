using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeetInSport.Domain.Entities;

namespace MeetInSport.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Action).IsRequired().HasMaxLength(50);
        builder.Property(a => a.EntityType).IsRequired().HasMaxLength(100);
        builder.Property(a => a.IpAddress).IsRequired().HasMaxLength(45);

        builder.HasOne(a => a.User)
        .WithMany(u => u.AuditLogs)
        .HasForeignKey(a => a.UserId)
        .OnDelete(DeleteBehavior.SetNull);
        // If user is deleted, keep log but set UserId null. Eventhouht user is deleted logs should be kept for audit purposes.
    }

}