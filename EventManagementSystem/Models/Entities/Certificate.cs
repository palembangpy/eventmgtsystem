namespace EventManagementSystem.Models.Entities;


public class Certificate
{
    public Guid CertificateId { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid EventScheduleId { get; set; }
    public string CertificateNumber { get; set; } = string.Empty;
    public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;
    public string PdfPath { get; set; } = string.Empty;
    public CertificateStatus Status { get; set; } = CertificateStatus.Generated;
    public int DownloadCount { get; set; } = 0;
    public DateTime? LastDownloadedAt { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual EventSchedule EventSchedule { get; set; } = null!;
}

public enum CertificateStatus
{
    Generated = 0,
    Downloaded = 1,
    Sent = 2,
    Expired = 3
}