using EventManagementSystem.Core.DTO;
using EventManagementSystem.Core.Models.Entities;

namespace EventManagementSystem.Core.Interfaces.Services;

public interface IEventScheduleService
{
    Task<IEnumerable<EventScheduleDto>> GetAllEventsAsync();
    Task<EventScheduleDto?> GetEventByIdAsync(Guid id);
    Task<EventScheduleDto> CreateEventAsync(CreateEventScheduleDto dto);
    Task UpdateEventAsync(Guid id, UpdateEventScheduleDto dto);
    Task DeleteEventAsync(Guid id);
    Task<IEnumerable<EventScheduleDto>> GetActiveEventsAsync();
    Task<IEnumerable<EventScheduleDto>> GetUpcomingEventsAsync();
    Task<IEnumerable<EventScheduleDto>> GetPastEventsAsync();
    Task<IEnumerable<EventScheduleDto>> GetEventsBySpeakerAsync(Guid speakerId);
    Task<IEnumerable<EventScheduleDto>> GetEventsByStatusAsync(EventStatus status);
    Task<Dictionary<string, int>> GetEventStatsAsync();
}