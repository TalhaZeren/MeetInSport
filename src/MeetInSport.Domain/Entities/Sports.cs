using MeetInSport.Domain.Common;

namespace MeetInSport.Domain.Entities;

public class Sports : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    // Navigation Property.
    public ICollection<Coach> Coaches { get; set; } = new List<Coach>();
}
