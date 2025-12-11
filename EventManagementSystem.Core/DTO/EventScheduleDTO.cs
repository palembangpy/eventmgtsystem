using System.ComponentModel.DataAnnotations;
using EventManagementSystem.Core.Validation;
using EventManagementSystem.Core.Security;
using EventManagementSystem.Core.Models.Entities;

namespace EventManagementSystem.Core.DTO;

public record EventScheduleDto
{
    public Guid EventScheduleId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string Location { get; init; } = string.Empty;
    public int MaxParticipants { get; init; }
    public int CurrentParticipants { get; init; }
    public string? ImageUrl { get; init; }
    public string? Tags { get; init; }
    public string? SpeakerName { get; init; }
    public Guid SpeakerId { get; init; }
    public string Status { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public bool IsFull { get; init; }
    public int DaysUntilEvent { get; init; }
}

public record CreateEventScheduleDto
{
    [Required(ErrorMessage = "Title wajib diisi")]
    [StringLength(50, MinimumLength = 5)]
    [Sanitize(LogicalMaxLength = 50)]
    public string Title { get; init; } = string.Empty;
    [Required(ErrorMessage = "Deskripsi wajib diisi")]
    [StringLength(255, MinimumLength = 10)]
    [Sanitize(LogicalMaxLength = 255)]
    public string Description { get; init; } = string.Empty;
    [Required(ErrorMessage = "Tanggal mulai wajib diisi")]
    public DateTime StartDate { get; init; }
    [Required(ErrorMessage = "Tanggal selesai wajib diisi")]
    [DateGreaterThan(nameof(StartDate), ErrorMessage = "EndDate harus lebih besar dari StartDate")]
    public DateTime EndDate { get; init; }
    [Required(ErrorMessage = "Lokasi wajib diisi")]
    [StringLength(150)]
    [Sanitize(LogicalMaxLength = 150)]
    public string Location { get; init; } = string.Empty;
    [Range(1, 100000, ErrorMessage = "MaxParticipants harus antara 1 - 100000")]
    public int MaxParticipants { get; init; } = 100;
    [Url(ErrorMessage = "Format URL image tidak valid")]
    [Sanitize(LogicalMaxLength = 255)]
    public string? ImageUrl { get; init; }
    [Sanitize(LogicalMaxLength = 100)]
    public string? Tags { get; init; }
    [Required(ErrorMessage = "SpeakerId wajib diisi")]
    public Guid SpeakerId { get; init; }
}

public record UpdateEventScheduleDto
{
    [Required(ErrorMessage = "Title wajib diisi")]
    [StringLength(50, MinimumLength = 5)]
    [Sanitize(LogicalMaxLength = 50)]
    public string Title { get; init; } = string.Empty;
    [Required(ErrorMessage = "Deskripsi wajib diisi")]
    [StringLength(255, MinimumLength = 10)]
    [Sanitize(LogicalMaxLength = 255)]
    public string Description { get; init; } = string.Empty;
    [Required(ErrorMessage = "Tanggal mulai wajib diisi")]
    public DateTime StartDate { get; init; }
    [Required(ErrorMessage = "Tanggal selesai wajib diisi")]
    [DateGreaterThan(nameof(StartDate), ErrorMessage = "EndDate harus lebih besar dari StartDate")]
    public DateTime EndDate { get; init; }
    [Required(ErrorMessage = "Lokasi wajib diisi")]
    [StringLength(150)]
    [Sanitize(LogicalMaxLength = 150)]
    public string Location { get; init; } = string.Empty;
    [Range(1, 100000, ErrorMessage = "MaxParticipants harus antara 1 - 100000")]
    public int MaxParticipants { get; init; }
    [Url(ErrorMessage = "Format URL image tidak valid")]
    [Sanitize(LogicalMaxLength = 255)]
    public string? ImageUrl { get; init; }
    [Sanitize(LogicalMaxLength = 100)]
    public string? Tags { get; init; }
    public Guid? SpeakerId { get; init; }    
    [Required]
    [EnumDataType(typeof(EventStatus), ErrorMessage = "Status tidak valid")]
    public EventStatus Status { get; init; }
    public bool IsActive { get; init; }

}
