using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text;
namespace EventManagementSystem.Core.Security;

public interface ISecurityLogger
{
    void Log(string level, string eventType, string message, string? details = null);
}

public class SecurityLogger : ISecurityLogger
{
    private readonly string _logDirectory;
    private readonly bool _isLogEnabled;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private static readonly object _lock = new();

    public SecurityLogger(IConfiguration configuration, IHttpContextAccessor accessor)
    {
        _httpContextAccessor = accessor;
        _isLogEnabled = Convert.ToBoolean(configuration["LoggingSecurity:IsLogEnabled"]);
        _logDirectory = Path.Combine(
            Directory.GetCurrentDirectory(),
            configuration["LoggingSecurity:LogPath"] ?? "Logs/Security"
        );
    }

    public void Log(string level, string eventType, string message, string? details = null)
    {
        if (!_isLogEnabled) return;

        try
        {
            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);

            string logFile = Path.Combine(
                _logDirectory,
                $"log_{DateTime.UtcNow:yyyyMMdd}.txt"
            );

            var context = _httpContextAccessor.HttpContext;

            string userIp = context?.Connection?.RemoteIpAddress?.ToString() ?? "unknown";
            string url = context?.Request?.Path.Value ?? "unknown";
            string user = context?.User?.Identity?.Name ?? "anonymous";

            string logEntry =
                $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] " +
                $"Level: {level.ToUpper()} | " +
                $"Type: {eventType} | " +
                $"User: {user} | " +
                $"IP: {userIp} | " +
                $"URL: {url} | " +
                $"Message: {message}" +
                (string.IsNullOrWhiteSpace(details) ? "" : $" | Details: {details}") +
                Environment.NewLine;

            lock (_lock)
            {
                File.AppendAllText(logFile, logEntry, Encoding.UTF8);
            }

            System.Diagnostics.Debug.WriteLine(logEntry);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LoggerError] {ex.Message}");
        }
    }
}
