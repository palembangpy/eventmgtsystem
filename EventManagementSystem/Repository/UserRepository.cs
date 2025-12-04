using EventManagementSystem.Data;
using EventManagementSystem.Models.Entities;
using EventManagementSystem.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventManagementSystem.Repository;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<User>> GetUsersByTypeAsync(UserType userType)
        => await _dbSet
            .Where(u => u.UserType == userType && u.IsActive)
            .Include(u => u.Certificates)
            .Include(u => u.EventsAsSpeaker)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

    public async Task<User?> GetByEmailAsync(string email)
        => await _dbSet
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

    public async Task<IEnumerable<User>> GetSpeakersAsync()
        => await _dbSet
            .Where(u => u.UserType == UserType.Speaker && u.IsActive)
            .OrderBy(u => u.Name)
            .ToListAsync();

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
        => await _dbSet
            .Where(u => u.IsActive)
            .Include(u => u.Certificates)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeId = null)
    {
        var query = _dbSet.Where(u => u.Email.ToLower() == email.ToLower());
        if (excludeId.HasValue)
            query = query.Where(u => u.Id != excludeId.Value);
        return await query.AnyAsync();
    }

    public async Task<Dictionary<string, int>> GetUserCountByTypeAsync()
        => await _dbSet
            .Where(u => u.IsActive)
            .GroupBy(u => u.UserType)
            .Select(g => new { Type = g.Key.ToString(), Count = g.Count() })
            .ToDictionaryAsync(x => x.Type, x => x.Count);

    public override async Task<IEnumerable<User>> GetAllAsync()
        => await _dbSet
            .Include(u => u.Certificates)
            .Include(u => u.EventsAsSpeaker)
            .Where(u => u.IsActive)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

    public override async Task<User?> GetByIdAsync(Guid id)
        => await _dbSet
            .Include(u => u.Certificates)
            .Include(u => u.EventsAsSpeaker)
            .Include(u => u.EventParticipants)
            .FirstOrDefaultAsync(u => u.Id == id);
}
