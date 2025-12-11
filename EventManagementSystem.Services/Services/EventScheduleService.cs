using AutoMapper;
using EventManagementSystem.Core.DTO;
using EventManagementSystem.Core.Models.Entities;
using EventManagementSystem.Core.Interfaces.Repository;
using EventManagementSystem.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace EventManagementSystem.Services.Services;

public class EventScheduleService : IEventScheduleService
{
    private readonly IEventScheduleRepository _eventRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<EventScheduleService> _logger;

    public EventScheduleService(IEventScheduleRepository eventRepo, IMapper mapper, ILogger<EventScheduleService> logger)
    {
        _eventRepo = eventRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<EventScheduleDto>> GetAllEventsAsync()
    {
        var events = await _eventRepo.GetAllAsync();
        return events.Select(e => MapToDto(e));
    }

    public async Task<EventScheduleDto?> GetEventByIdAsync(Guid id)
    {
        var evt = await _eventRepo.GetByIdAsync(id);
        return evt != null ? MapToDto(evt) : null;
    }

    public async Task<EventScheduleDto> CreateEventAsync(CreateEventScheduleDto dto)
    {
        ValidateDates(dto.StartDate, dto.EndDate);

        var evt = _mapper.Map<EventSchedule>(dto);
        evt.CreatedAt = DateTime.UtcNow;
        evt.UpdatedAt = DateTime.UtcNow;
        evt.Status = EventStatus.Upcoming;
        evt.IsActive = true;

        var created = await _eventRepo.AddAsync(evt);
        _logger.LogInformation("Event created: {EventId} - {EventTitle}", created.EventScheduleId, created.Title);
        
        return MapToDto(created);
    }

    public async Task UpdateEventAsync(Guid id, UpdateEventScheduleDto dto)
    {
        var evt = await _eventRepo.GetByIdAsync(id);
        if (evt == null)
            throw new KeyNotFoundException($"Event with ID {id} not found.");

        ValidateDates(dto.StartDate, dto.EndDate);

        _mapper.Map(dto, evt);
        evt.UpdatedAt = DateTime.UtcNow;

        await _eventRepo.UpdateAsync(evt);
        _logger.LogInformation("Event updated: {EventId}", id);
    }

    public async Task DeleteEventAsync(Guid id)
    {
        var evt = await _eventRepo.GetByIdAsync(id);
        if (evt == null)
            throw new KeyNotFoundException($"Event with ID {id} not found.");

        evt.IsActive = false;
        evt.UpdatedAt = DateTime.UtcNow;
        await _eventRepo.UpdateAsync(evt);
        
        _logger.LogInformation("Event soft deleted: {EventId}", id);
    }

    public async Task<IEnumerable<EventScheduleDto>> GetActiveEventsAsync()
    {
        var events = await _eventRepo.GetActiveEventsAsync();
        return events.Select(e => MapToDto(e));
    }

    public async Task<IEnumerable<EventScheduleDto>> GetUpcomingEventsAsync()
    {
        var events = await _eventRepo.GetUpcomingEventsAsync();
        return events.Select(e => MapToDto(e));
    }

    public async Task<IEnumerable<EventScheduleDto>> GetPastEventsAsync()
    {
        var events = await _eventRepo.GetPastEventsAsync();
        return events.Select(e => MapToDto(e));
    }

    public async Task<IEnumerable<EventScheduleDto>> GetEventsBySpeakerAsync(Guid speakerId)
    {
        var events = await _eventRepo.GetEventsBySpeakerAsync(speakerId);
        return events.Select(e => MapToDto(e));
    }

    public async Task<IEnumerable<EventScheduleDto>> GetEventsByStatusAsync(EventStatus status)
    {
        var events = await _eventRepo.GetEventsByStatusAsync(status);
        return events.Select(e => MapToDto(e));
    }

    public async Task<Dictionary<string, int>> GetEventStatsAsync()
        => await _eventRepo.GetEventCountByStatusAsync();

    private void ValidateDates(DateTime start, DateTime end)
    {
        if (end <= start)
            throw new InvalidOperationException("End date must be after start date.");
        if (start < DateTime.UtcNow.Date)
            throw new InvalidOperationException("Start date cannot be in the past.");
    }

    private EventScheduleDto MapToDto(EventSchedule evt)
    {
        var now = DateTime.UtcNow;
        var participantCount = evt.Participants?.Count(p => p.Status == ParticipantStatus.Attended) ?? 0;
        
        return new EventScheduleDto
        {
            EventScheduleId = evt.EventScheduleId,
            Title = evt.Title,
            Description = evt.Description,
            StartDate = evt.StartDate,
            EndDate = evt.EndDate,
            Location = evt.Location,
            MaxParticipants = evt.MaxParticipants,
            CurrentParticipants = participantCount,
            ImageUrl = evt.ImageUrl,
            Tags = evt.Tags,
            SpeakerName = evt.Speaker?.Name,
            SpeakerId = evt.SpeakerId,
            Status = evt.Status.ToString(),
            IsActive = evt.IsActive,
            IsFull = participantCount >= evt.MaxParticipants,
            DaysUntilEvent = (evt.StartDate - now).Days
        };
    }
}
