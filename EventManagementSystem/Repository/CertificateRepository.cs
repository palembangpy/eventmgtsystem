using EventManagementSystem.Data;
using EventManagementSystem.Models.Entities;
using EventManagementSystem.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventManagementSystem.Repository;

public class CertificateRepository : BaseRepository<Certificate>, ICertificateRepository
{
    public CertificateRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Certificate>> GetCertificatesByUserAsync(Guid userId)
        => await _dbSet
            .Include(c => c.User)
            .Include(c => c.EventSchedule)
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.GeneratedDate)
            .ToListAsync();

    public async Task<IEnumerable<Certificate>> GetCertificatesByEventAsync(Guid eventId)
        => await _dbSet
            .Include(c => c.User)
            .Include(c => c.EventSchedule)
            .Where(c => c.EventScheduleId == eventId)
            .OrderByDescending(c => c.GeneratedDate)
            .ToListAsync();

    public async Task<Certificate?> GetByNumberAsync(string certificateNumber)
        => await _dbSet
            .Include(c => c.User)
            .Include(c => c.EventSchedule)
            .FirstOrDefaultAsync(c => c.CertificateNumber == certificateNumber);

    public async Task<bool> CertificateExistsAsync(Guid userId, Guid eventId)
        => await _dbSet.AnyAsync(c => c.UserId == userId && c.EventScheduleId == eventId);

    public async Task<IEnumerable<Certificate>> GetRecentCertificatesAsync(int count)
        => await _dbSet
            .Include(c => c.User)
            .Include(c => c.EventSchedule)
            .OrderByDescending(c => c.GeneratedDate)
            .Take(count)
            .ToListAsync();

    public async Task IncrementDownloadCountAsync(Guid certificateId)
    {
        var certificate = await GetByIdAsync(certificateId);
        if (certificate != null)
        {
            certificate.DownloadCount++;
            certificate.LastDownloadedAt = DateTime.UtcNow;
            certificate.Status = CertificateStatus.Downloaded;
            await UpdateAsync(certificate);
        }
    }

    public override async Task<IEnumerable<Certificate>> GetAllAsync()
        => await _dbSet
            .Include(c => c.User)
            .Include(c => c.EventSchedule)
            .OrderByDescending(c => c.GeneratedDate)
            .ToListAsync();

    public override async Task<Certificate?> GetByIdAsync(Guid id)
        => await _dbSet
            .Include(c => c.User)
            .Include(c => c.EventSchedule).ThenInclude(e => e.Speaker)
            .FirstOrDefaultAsync(c => c.CertificateId == id);
}
