using EventManagementSystem.Core.Enums;
using Microsoft.AspNetCore.Identity;
namespace EventManagementSystem.Core.Models.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public SystemRole systemRole {get; set;} = SystemRole.User;
    public bool IsEmailVerified { get; set; } = false;
    public DateTime? EmailVerifiedAt { get; set; }

    public string? EmailVerificationId { get; set; }
    public DateTime? EmailVerificationExpiresAt { get; set; }

    public bool IsMfaEnabled { get; set; } = false;
    public string? MfaSecret { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
}
