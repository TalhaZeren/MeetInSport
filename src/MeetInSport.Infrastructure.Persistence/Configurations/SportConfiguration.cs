using MeetInSport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetInSport.Infrastructure.Persistence.Configurations;

public class SportConfiguration : IEntityTypeConfiguration<Sports>
{
    public void Configure(EntityTypeBuilder<Sports> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
        .IsRequired()
        .HasMaxLength(100);

        builder.HasIndex(s => s.Name).IsUnique();

        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}