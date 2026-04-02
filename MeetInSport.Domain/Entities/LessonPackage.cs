using System.Runtime.CompilerServices;
using MeetInSport.Domain.Common;
using MeetInSport.Domain.Enum;
namespace MeetInSport.Domain.Entities
{
    public class LessongPackage : BaseEntity
    {
        public Guid CoachId { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public string PackageDescription { get; set; } = string.Empty;
        public decimal DurationInHours { get; set; }
        public decimal PackagePrice { get; set; }
        public ICollection<string> Requirements { get; set; } = new List<string>();
        public LocationType LocationType { get; set; } = LocationType.CoachLocation;
        public bool IsActive { get; set; }
        public LessonModel LessonModel { get; set; } = LessonModel.OneOnOne;
        public string? CoverImageUrl { get; set; }
        public virtual Coach Coach { get; set; } = null!;
        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();









    }
}