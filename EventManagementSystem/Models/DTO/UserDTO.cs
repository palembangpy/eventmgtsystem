using EventManagementSystem.Helper.Validation;
using EventManagementSystem.Models.Entities;
using EventManagementSystem.Security;
using System.ComponentModel.DataAnnotations;
namespace EventManagementSystem.Models.DTO;

public record UserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string UserType { get; init; } = string.Empty;
    public string? ProfilePicture { get; init; }
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public int TotalCertificates { get; init; }
    public int TotalEvents { get; init; }
}

public record CreateUserDto
{
    [Required(ErrorMessage = "Email wajib diisi")]
    [StrictEmail]
    [StringLength(150)]
    [Sanitize(LogicalMaxLength = 150)]
    [UniqueEmail]
    public string Email { get; init; } = string.Empty;
    [Required(ErrorMessage = "Name wajib diisi")]
    [StringLength(150, MinimumLength = 2)]
    [Sanitize(LogicalMaxLength = 150)]
    public string Name { get; init; } = string.Empty;
    [Required(ErrorMessage = "Tipe User wajib diisi")]
    [EnumDataType(typeof(UserType), ErrorMessage = "Tipe User tidak valid")]
    public UserType UserType { get; init; }
    [Phone]
    [Sanitize(LogicalMaxLength = 20)]
    public string? Phone { get; init; }
    [Sanitize(LogicalMaxLength = 255)]
    public string? Address { get; init; }
}

public record UpdateUserDto
{
    [Required(ErrorMessage = "Name wajib diisi")]
    [StringLength(150, MinimumLength = 2)]
    [Sanitize(LogicalMaxLength = 150)]
    public string Name { get; init; } = string.Empty;
    [Required(ErrorMessage = "Tipe User wajib diisi")]
    [EnumDataType(typeof(UserType), ErrorMessage = "Tipe User tidak valid")]
    public UserType UserType { get; init; }
    [Phone]
    [Sanitize(LogicalMaxLength = 20)]
    public string? Phone { get; init; }
    [Sanitize(LogicalMaxLength = 255)]
    public string? Address { get; init; }
    [Url(ErrorMessage = "Format URL foto tidak valid")]
    [Sanitize(LogicalMaxLength = 255)]
    public string? ProfilePicture { get; init; }
    public bool IsActive { get; init; }
}