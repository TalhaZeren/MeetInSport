namespace MeetInSport.Domain.Entities;

public class AuditLog
{
    public long Id { get; set; } // For 64-bit. Because it fill up fast in the system. 
    public Guid UserId { get; set; }
    public String Action { get; set; } = string.Empty; // We will see which proccess is used such as CREATE,UPDATE et.c
    public string EntityType { get; set; } = string.Empty; // in which entity is used.
    public Guid? EntityId { get; set; }

    public string? OldValue { get; set; }
    public string? NewValue { get; set; }

    public string? IpAddress { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public virtual User? User { get; set; }

}