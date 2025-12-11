namespace EventManagementSystem.Core.Models.Entities;

public class Tokenizer
{
    public Guid TokenId { get; set; }
    public string TokenName { get; set; } = string.Empty;
    public string TokenHash { get; set; } = string.Empty; 
    public string Salt { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string CreatedBy { get; set; } = string.Empty; 
    public string AllowedEndpoints { get; set; } = string.Empty; 
    public int UsageCount { get; set; } = 0;
    public DateTime? LastUsedAt { get; set; }
}
