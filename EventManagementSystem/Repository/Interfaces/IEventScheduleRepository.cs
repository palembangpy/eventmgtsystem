using EventManagementSystem.Models.Entities;

namespace EventManagementSystem.Repository.Interfaces;

public interface IEventScheduleRepository : IBaseRepository<EventSchedule>
{
    Task<IEnumerable<EventSchedule>> GetActiveEventsAsync();
    Task<IEnumerable<EventSchedule>> GetUpcomingEventsAsync();
    Task<IEnumerable<EventSchedule>> GetPastEventsAsync();
    Task<IEnumerable<EventSchedule>> GetEventsBySpeakerAsync(Guid speakerId);
    Task<IEnumerable<EventSchedule>> GetEventsByStatusAsync(EventStatus status);
    Task<Dictionary<string, int>> GetEventCountByStatusAsync();
    Task<int> GetParticipantCountAsync(Guid eventId);
}