using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeetInSport.Domain.Entities;
namespace MeetInSport.Infrastructure.Persistence.Configurations;

public class LessonPackageConfiguration : IEntityTypeConfiguration<LessonPackage>
{
    public void Configure(EntityTypeBuilder<LessonPackage> builder)
    {
        builder.HasKey(p => p.Id);
        // Unique index: A coach cannot have duplicate package titles
        builder.HasIndex(p => new { p.CoachId, p.PackageName }).IsUnique();
        builder.Property(p => p.PackageName).IsRequired().HasMaxLength(200);
        builder.Property(p => p.PackageDescription).HasMaxLength(500);
        builder.Property(p => p.CoverImageUrl).HasMaxLength(500);
        builder.Property(p => p.PackagePrice).IsRequired().HasPrecision(18, 2);
    }
}