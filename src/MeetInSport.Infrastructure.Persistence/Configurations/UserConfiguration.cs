using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeetInSport.Domain.Entities;


public class UserConfiguration : IEntityTypeConfiguration<User>
// This class is used to configure the User entity. i takes TEntity.
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.Name).IsRequired().HasMaxLength(100);

        builder.HasOne(u => u.CoachProfile)
        .WithOne(c => c.User)
        .HasForeignKey<Coach>(c => c.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    }
}