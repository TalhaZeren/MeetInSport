
namespace MeetInSport.Domain.Entities
{
    using MeetInSport.Domain.Common;

    public class User : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        // The password that will be stored in the database should be hashed for security reasons.
        public int RoleId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime KvkkAcceptedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsEmailVerified { get; set; } = false;
        public virtual Role Role { get; set; } = null!;
    }
}