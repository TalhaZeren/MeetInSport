using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeetInSport.Domain.Entities;


public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        // For Student navigation property in Reservation entity, We are configuring a one-to-many relationship between User and Reservation. A user might have multiple reservations, but each reservation is associated with only one user (Student).
        builder.HasOne(r => r.Student)
        .WithMany(u => u.Reservations)
        .HasForeignKey(r => r.StudentId);

        // A Student might have multiple reservations, but vice versa is not possible. A reservation is just associated with one Student.

        builder.HasOne(r => r.Coach)
        .WithMany(c => c.Reservations)
        .HasForeignKey(r => r.CoachId);

        // A Coach might have multiple reservations, but vice versa is not possible. A reservation is just associated with one Coach.

        builder.HasOne(r => r.Payment)
        .WithOne(p => p.Reservation)
        .HasForeignKey<Payment>(p => p.ReservationId);

        // Payment is needed to be associated with a reservation and vice versa. But the status of Payment (null or not Null) shouldn't be forgotten because of the logic path (Pending, Completed, Cancelled).)
    }
}