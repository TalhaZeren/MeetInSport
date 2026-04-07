using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeetInSport.Domain.Entities;

namespace MeetInSport.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.RoleName).IsRequired().HasMaxLength(50);
        builder.Property(r => r.Description).HasMaxLength(200);

        builder.HasMany(r => r.Users)
        .WithOne(u => u.Role)
        .HasForeignKey(u => u.RoleId)
        .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Role { Id = 1, RoleName = "Admin", Description = "System Mamager" },
            new Role { Id = 2, RoleName = "Coach", Description = "Can Create packages and give lessons" },
            new Role { Id = 3, RoleName = "Student", Description = "Can book lessons and view their progress" }
        );
    }
}

