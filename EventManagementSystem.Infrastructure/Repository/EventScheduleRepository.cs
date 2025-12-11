using EventManagementSystem.Core.Interfaces.Repository;
using EventManagementSystem.Core.Models.Entities;
using EventManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventManagementSystem.Infrastructure.Repository;

public class EventScheduleRepository : BaseRepository<EventSchedule>, IEventScheduleRepository
{
    public EventScheduleRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<EventSchedule>> GetActiveEventsAsync()
        => await _dbSet
            .Include(e => e.Speaker)
            .Include(e => e.Participants)
            .Where(e => e.IsActive)
            .OrderBy(e => e.StartDate)
            .ToListAsync();

    public async Task<IEnumerable<EventSchedule>> GetUpcomingEventsAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Include(e => e.Speaker)
            .Include(e => e.Participants)
            .Where(e => e.IsActive && e.StartDate > now)
            .OrderBy(e => e.StartDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<EventSchedule>> GetPastEventsAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Include(e => e.Speaker)
            .Include(e => e.Participants)
            .Where(e => e.EndDate < now)
            .OrderByDescending(e => e.EndDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<EventSchedule>> GetEventsBySpeakerAsync(Guid speakerId)
        => await _dbSet
            .Include(e => e.Speaker)
            .Include(e => e.Participants)
            .Where(e => e.SpeakerId == speakerId)
            .OrderByDescending(e => e.StartDate)
            .ToListAsync();

    public async Task<IEnumerable<EventSchedule>> GetEventsByStatusAsync(EventStatus status)
        => await _dbSet
            .Include(e => e.Speaker)
            .Include(e => e.Participants)
            .Where(e => e.Status == status && e.IsActive)
            .OrderBy(e => e.StartDate)
            .ToListAsync();

    public async Task<Dictionary<string, int>> GetEventCountByStatusAsync()
        => await _dbSet
            .Where(e => e.IsActive)
            .GroupBy(e => e.Status)
            .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count);

    public async Task<int> GetParticipantCountAsync(Guid eventId)
        => await _context.Set<EventParticipant>()
            .CountAsync(p => p.EventScheduleId == eventId && p.Status == ParticipantStatus.Attended);

    public override async Task<IEnumerable<EventSchedule>> GetAllAsync()
        => await _dbSet
            .Include(e => e.Speaker)
            .Include(e => e.Participants)
            .Include(e => e.Certificates)
            .Where(e => e.IsActive)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();

    public override async Task<EventSchedule?> GetByIdAsync(Guid id)
        => await _dbSet
            .Include(e => e.Speaker)
            .Include(e => e.Participants).ThenInclude(p => p.User)
            .Include(e => e.Certificates).ThenInclude(c => c.User)
            .FirstOrDefaultAsync(e => e.EventScheduleId == id);
}
