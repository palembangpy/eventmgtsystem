namespace EventManagementSystem.Models.Entities;

public class EventParticipant
{
    public Guid EventParticipantId { get; set; } = Guid.NewGuid();
    public Guid EventScheduleId { get; set; }
    public Guid UserId { get; set; }
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public ParticipantStatus Status { get; set; } = ParticipantStatus.Registered;
    
    public virtual EventSchedule EventSchedule { get; set; } = null!;
    public virtual User User { get; set; } = null!;

}
public enum ParticipantStatus
{
    Registered = 0,
    Attended = 1,
    NoShow = 2,
    Cancelled = 3
}