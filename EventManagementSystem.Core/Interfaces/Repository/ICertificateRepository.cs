using EventManagementSystem.Core.Models.Entities;

namespace EventManagementSystem.Core.Interfaces.Repository;

public interface ICertificateRepository : IBaseRepository<Certificate>
{
    Task<IEnumerable<Certificate>> GetCertificatesByUserAsync(Guid userId);
    Task<IEnumerable<Certificate>> GetCertificatesByEventAsync(Guid eventId);
    Task<Certificate?> GetByNumberAsync(string certificateNumber);
    Task<bool> CertificateExistsAsync(Guid userId, Guid eventId);
    Task<IEnumerable<Certificate>> GetRecentCertificatesAsync(int count);
    Task IncrementDownloadCountAsync(Guid certificateId);
}
