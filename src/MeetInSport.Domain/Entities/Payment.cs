using MeetInSport.Domain.Common;
using MeetInSport.Domain.Enum;

namespace MeetInSport.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid ReservationId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TRY";
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? TransactionId { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public virtual Reservation Reservation { get; set; } = null!;
}