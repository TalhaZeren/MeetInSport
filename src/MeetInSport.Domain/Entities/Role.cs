namespace MeetInSport.Domain.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public virtual ICollection<User> Users { get; set; } = new List<User>(); // ? 
    }
}