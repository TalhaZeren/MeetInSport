using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeetInSport.Domain.Entities;

public class LessonPackageConfiguration : IEntityTypeConfiguration<LessongPackage>
{
    public void Configure(EntityTypeBuilder<LessongPackage> builder)
    {
        builder.Property(p => p.PackageName).IsRequired().HasMaxLength(200);

        builder.HasIndex(p => new { p.CoachId, p.PackageName }).IsUnique();

        builder.Property(p => p.PackagePrice).HasPrecision(18, 2);
    }
}