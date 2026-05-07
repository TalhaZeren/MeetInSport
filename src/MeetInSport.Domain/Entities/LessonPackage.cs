using System.Runtime.CompilerServices;
using MeetInSport.Domain.Common;
using MeetInSport.Domain.Enum;
namespace MeetInSport.Domain.Entities
{
    public class LessonPackage : BaseEntity
    {
        public Guid CoachId { get; set; }
        public string PackageName { get; set; } = string.Empty; // Title
        public string PackageDescription { get; set; } = string.Empty;
        public decimal DurationInMinutes { get; set; }
        public decimal PackagePrice { get; set; } // Price
        public List<string> Requirements { get; set; } = new List<string>();
        public LocationType LocationType { get; set; } = LocationType.CoachLocation;
        public bool IsActive { get; set; }
        public LessonModel LessonModel { get; set; } = LessonModel.OneOnOne;
        public string? CoverImageUrl { get; set; }
        public virtual Coach Coach { get; set; } = null!;
        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>(); // 1 to N relationship between LessonPackage and. Reservation.

    }
}