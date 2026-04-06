using MeetInSport.Domain.Common;
using MeetInSport.Domain.Enum;

namespace MeetInSport.Domain.Entities
{
    public class Reservation : BaseEntity
    {
        public Guid StudentId { get; set; }
        public Guid CoachId { get; set; }
        public Guid PackageId { get; set; }

        public DateTime ScheduledAt { get; set; }
        public LocationType locationType { get; set; }
        public ReservationStatus Status { get; set; }
        public string? Notes { get; set; }

        public DateTime CancelledAt { get; set; }
        public string? CancelReason { get; set; }

        public virtual User Student { get; set; } = null!;
        public virtual Coach Coach { get; set; } = null!;
        public virtual LessongPackage Package { get; set; } = null!;
        public virtual Payment? Payment { get; set; } // 1 to 1 relationship between Reservation and Payment. A reservation might have a payment, but it is not mandatory. If the reservation has a payment, then the Payment property will be filled with the corresponding Payment entity. But We are indicating like this because not all reservations might have a payment, especially if the reservation is cancelled before the payment is made. // this is very important step to be able manage the logic path.


    }

}