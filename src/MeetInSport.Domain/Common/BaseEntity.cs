namespace MeetInSport.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        // Automatically generate a new GUID for each entity. OOP inheritance method, takes id from base entity class and use it in other entities.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}