using EventManagementSystem.Core.DTO;

namespace EventManagementSystem.Core.Interfaces.Services;

public interface ICertificateService
{
    Task<IEnumerable<CertificateDto>> GetAllCertificatesAsync();
    Task<CertificateDto?> GetCertificateByIdAsync(Guid id);
    Task<CertificateDto> GenerateCertificateAsync(GenerateCertificateDto dto);
    Task<List<CertificateDto>> BulkGenerateCertificatesAsync(BulkGenerateCertificatesDto dto);
    Task DeleteCertificateAsync(Guid id);
    Task<IEnumerable<CertificateDto>> GetCertificatesByUserAsync(Guid userId);
    Task<IEnumerable<CertificateDto>> GetCertificatesByEventAsync(Guid eventId);
    Task<CertificateDto?> GetByNumberAsync(string number);
    Task<byte[]> DownloadCertificateAsync(Guid id);
}
