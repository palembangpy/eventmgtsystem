using AutoMapper;
using EventManagementSystem.Models.DTO;
using EventManagementSystem.Models.Entities;
using EventManagementSystem.Repository.Interfaces;
using EventManagementSystem.Services.Interfaces;

namespace EventManagementSystem.Services;

public class CertificateService : ICertificateService
{
    private readonly ICertificateRepository _certRepo;
    private readonly IUserRepository _userRepo;
    private readonly IEventScheduleRepository _eventRepo;
    private readonly ICertificateGeneratorService _generator;
    private readonly IMapper _mapper;
    private readonly ILogger<CertificateService> _logger;

    public CertificateService(
        ICertificateRepository certRepo,
        IUserRepository userRepo,
        IEventScheduleRepository eventRepo,
        ICertificateGeneratorService generator,
        IMapper mapper,
        ILogger<CertificateService> logger)
    {
        _certRepo = certRepo;
        _userRepo = userRepo;
        _eventRepo = eventRepo;
        _generator = generator;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<CertificateDto>> GetAllCertificatesAsync()
    {
        var certs = await _certRepo.GetAllAsync();
        return certs.Select(c => MapToDto(c));
    }

    public async Task<CertificateDto?> GetCertificateByIdAsync(Guid id)
    {
        var cert = await _certRepo.GetByIdAsync(id);
        return cert != null ? MapToDto(cert) : null;
    }

    public async Task<CertificateDto> GenerateCertificateAsync(GenerateCertificateDto dto)
    {
        if (await _certRepo.CertificateExistsAsync(dto.UserId, dto.EventScheduleId))
            throw new InvalidOperationException("Certificate already exists for this user and event.");

        var user = await _userRepo.GetByIdAsync(dto.UserId)
            ?? throw new KeyNotFoundException($"User {dto.UserId} not found.");
        
        var evt = await _eventRepo.GetByIdAsync(dto.EventScheduleId)
            ?? throw new KeyNotFoundException($"Event {dto.EventScheduleId} not found.");

        var pdfPath = await _generator.GenerateCertificateAsync(user, evt);

        var certificate = new Certificate
        {
            UserId = dto.UserId,
            EventScheduleId = dto.EventScheduleId,
            CertificateNumber = GenerateCertificateNumber(),
            GeneratedDate = DateTime.UtcNow,
            PdfPath = pdfPath,
            Status = CertificateStatus.Generated,
            DownloadCount = 0
        };

        var created = await _certRepo.AddAsync(certificate);
        _logger.LogInformation("Certificate generated: {CertId} - {CertNumber}", created.CertificateId, created.CertificateNumber);
        
        return MapToDto(created);
    }

    public async Task<List<CertificateDto>> BulkGenerateCertificatesAsync(BulkGenerateCertificatesDto dto)
    {
        var certificates = new List<CertificateDto>();
        
        foreach (var userId in dto.UserIds)
        {
            try
            {
                var cert = await GenerateCertificateAsync(new GenerateCertificateDto 
                { 
                    UserId = userId, 
                    EventScheduleId = dto.EventScheduleId 
                });
                certificates.Add(cert);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to generate certificate for user {UserId}", userId);
            }
        }

        _logger.LogInformation("Bulk generated {Count} certificates for event {EventId}", 
            certificates.Count, dto.EventScheduleId);
        
        return certificates;
    }

    public async Task DeleteCertificateAsync(Guid id)
    {
        var cert = await _certRepo.GetByIdAsync(id);
        if (cert == null)
            throw new KeyNotFoundException($"Certificate {id} not found.");

        var filePath = Path.Combine("wwwroot", cert.PdfPath);
        if (File.Exists(filePath))
            File.Delete(filePath);

        await _certRepo.DeleteAsync(id);
        _logger.LogInformation("Certificate deleted: {CertId}", id);
    }

    public async Task<IEnumerable<CertificateDto>> GetCertificatesByUserAsync(Guid userId)
    {
        var certs = await _certRepo.GetCertificatesByUserAsync(userId);
        return certs.Select(c => MapToDto(c));
    }

    public async Task<IEnumerable<CertificateDto>> GetCertificatesByEventAsync(Guid eventId)
    {
        var certs = await _certRepo.GetCertificatesByEventAsync(eventId);
        return certs.Select(c => MapToDto(c));
    }

    public async Task<CertificateDto?> GetByNumberAsync(string number)
    {
        var cert = await _certRepo.GetByNumberAsync(number);
        return cert != null ? MapToDto(cert) : null;
    }

    public async Task<byte[]> DownloadCertificateAsync(Guid id)
    {
        var cert = await _certRepo.GetByIdAsync(id);
        if (cert == null)
            throw new KeyNotFoundException($"Certificate {id} not found.");

        var filePath = Path.Combine("wwwroot", cert.PdfPath);
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Certificate file not found.");

        await _certRepo.IncrementDownloadCountAsync(id);
        _logger.LogInformation("Certificate downloaded: {CertId} (Count: {Count})", 
            id, cert.DownloadCount + 1);

        return await File.ReadAllBytesAsync(filePath);
    }

    private string GenerateCertificateNumber()
        => $"CERT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";

    private CertificateDto MapToDto(Certificate cert) => new()
    {
        CertificateId = cert.CertificateId,
        UserName = cert.User.Name,
        UserEmail = cert.User.Email,
        EventTitle = cert.EventSchedule.Title,
        CertificateNumber = cert.CertificateNumber,
        GeneratedDate = cert.GeneratedDate,
        Status = cert.Status.ToString(),
        PdfPath = cert.PdfPath,
        DownloadCount = cert.DownloadCount,
        LastDownloadedAt = cert.LastDownloadedAt
    };
}