using EventManagementSystem.Models.DTO;

namespace EventManagementSystem.Models.ViewModels;

public record DashboardViewModel
{
    public int TotalUsers { get; init; }
    public int TotalEvents { get; init; }
    public int TotalCertificates { get; init; }
    public int TotalParticipants { get; init; }
    public int UpcomingEvents { get; init; }
    public int ActiveUsers { get; init; }
    public List<EventScheduleDto> RecentEvents { get; init; } = new List<EventScheduleDto>();
    public List<UserDto> RecentUsers { get; init; } = new List<UserDto>();
    public Dictionary<string, int> EventsByStatus { get; init; } = new Dictionary<string, int>();
    public Dictionary<string, int> UsersByType { get; init; } = new Dictionary<string, int>();
}
