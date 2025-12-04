namespace EventManagementSystem.Models.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public UserType UserType { get; set; }
    public string? ProfilePicture { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
    public virtual ICollection<EventSchedule> EventsAsSpeaker { get; set; } = new List<EventSchedule>();
    public virtual ICollection<EventParticipant> EventParticipants { get; set; } = new List<EventParticipant>();
}

public enum UserType
{
    User = 0,
    Volunteer = 1,
    Speaker = 2
}
