using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeetInSport.Domain.Entities;

namespace MeetInSport.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Amount).IsRequired().HasPrecision(18, 2);
        builder.Property(p => p.Currency).IsRequired().HasMaxLength(3); // TRY ,USD etc.
        builder.Property(p => p.TransactionId).HasMaxLength(255);
    }
}