using MeetInSport.Domain.Common;

namespace MeetInSport.Domain.Entities
{
    public class Coach : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Sport { get; set; } = string.Empty;
        public string? Bio { get; set; } // 
        public decimal HourlyRate { get; set; }
        public int Experience { get; set; } // that is a years of epxerience
        public bool IsApproved { get; set; } = false;
        public decimal AverageRating { get; set; }
        public string? Location { get; set; }
        public string? Iban { get; set; }
        // Navigation Propertiy
        public virtual User User { get; set; } = null!;
        public virtual ICollection<LessonPackage> Packages { get; set; } = new List<LessonPackage>();
        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();


    }
}