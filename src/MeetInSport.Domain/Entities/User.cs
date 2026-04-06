
namespace MeetInSport.Domain.Entities
{
    using MeetInSport.Domain.Common;
    using SportPlatform.Domain.Entities;

    public class User : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        // The password that will be stored in the database should be hashed for security reasons.
        public int RoleId { get; set; } // The class definiton of the Role is in the down below.
        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime KvkkAcceptedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsEmailVerified { get; set; } = false;
        public virtual Role Role { get; set; } = null!; // Role is got from the RoleId foreign key. Accordingly we are able to know the user type.
        public virtual Coach? CoachProfile { get; set; } // 1 to 1 relationship between User and Coach. A user can be a coach, but it is not mandatory. If the user is a coach, then the CoachProfile property will be filled with the corresponding Coach entity. But We are indicating like this might be null because not all users are coaches.
        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>(); // 1 to N relationship between User and Reservation. A user can have multiple reservations.

        public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    }
}