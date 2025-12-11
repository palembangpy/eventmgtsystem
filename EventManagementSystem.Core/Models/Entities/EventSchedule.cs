namespace EventManagementSystem.Core.Models.Entities;

public class EventSchedule
{
    public Guid EventScheduleId { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public int MaxParticipants { get; set; }
    public string? ImageUrl { get; set; }
    public string? Tags { get; set; }
    public Guid SpeakerId { get; set; }
    public EventStatus Status { get; set; } = EventStatus.Upcoming;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public virtual User? Speaker { get; set; }
    public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
    public virtual ICollection<EventParticipant> Participants { get; set; } = new List<EventParticipant>();
}

public enum EventStatus
{
    Upcoming = 0,
    Ongoing = 1,
    Completed = 2,
    Cancelled = 3
}