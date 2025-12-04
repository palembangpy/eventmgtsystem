using System.ComponentModel.DataAnnotations;
using EventManagementSystem.Helper.Validation;
using EventManagementSystem.Security;

namespace EventManagementSystem.Models.DTO;

public record CertificateDto
{
    public Guid CertificateId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string UserEmail { get; init; } = string.Empty;
    public string EventTitle { get; init; } = string.Empty;
    public string CertificateNumber { get; init; } = string.Empty;
    public DateTime GeneratedDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public string PdfPath { get; init; } = string.Empty;
    public int DownloadCount { get; init; }
    public DateTime? LastDownloadedAt { get; init; }
}

public record GenerateCertificateDto
{
    [Required(ErrorMessage = "UserId wajib diisi")]
    [NotEmptyGuid(ErrorMessage = "UserId tidak boleh kosong")]
    public Guid UserId { get; init; }

    [Required(ErrorMessage = "EventScheduleId wajib diisi")]
    [NotEmptyGuid(ErrorMessage = "EventScheduleId tidak boleh kosong")]
    public Guid EventScheduleId { get; init; }
}

public record BulkGenerateCertificatesDto
{
    [Required(ErrorMessage = "EventScheduleId wajib diisi")]
    [NotEmptyGuid(ErrorMessage = "EventScheduleId tidak boleh kosong")]
    public Guid EventScheduleId { get; init; }

    [Required(ErrorMessage = "UserIds wajib diisi")]
    [MinLength(1, ErrorMessage = "Minimal 1 user")]
    [MaxLength(500, ErrorMessage = "Maksimal 500 user dalam satu request")]
    [NoDuplicateGuids(ErrorMessage = "UserIds mengandung duplikasi")]
    [NoEmptyGuidInCollection(ErrorMessage = "UserIds tidak boleh mengandung Guid kosong")]
    public List<Guid> UserIds { get; init; } = new List<Guid>();
}