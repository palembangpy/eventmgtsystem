using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EventManagementSystem.Services.Interfaces;
using EventManagementSystem.Models.ViewModels;

namespace EventManagementSystem.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IUserService _userService;
    private readonly IEventScheduleService _eventService;
    private readonly ICertificateService _certService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IUserService userService,
        IEventScheduleService eventService,
        ICertificateService certService,
        ILogger<HomeController> logger)
    {
        _userService = userService;
        _eventService = eventService;
        _certService = certService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var allUsers = await _userService.GetAllUsersAsync();
            var allEvents = await _eventService.GetAllEventsAsync();
            var allCerts = await _certService.GetAllCertificatesAsync();
            var upcomingEvents = await _eventService.GetUpcomingEventsAsync();
            var activeUsers = await _userService.GetActiveUsersAsync();

            var recentEvents = upcomingEvents.Take(5).ToList();
            var recentUsers = activeUsers.OrderByDescending(u => u.CreatedAt).Take(5).ToList();

            var eventStats = await _eventService.GetEventStatsAsync();
            var userStats = await _userService.GetUserStatsAsync();

            var viewModel = new DashboardViewModel
            {
                TotalUsers = allUsers.Count(),
                TotalEvents = allEvents.Count(),
                TotalCertificates = allCerts.Count(),
                TotalParticipants = allUsers.Sum(u => u.TotalEvents),
                UpcomingEvents = upcomingEvents.Count(),
                ActiveUsers = activeUsers.Count(),
                RecentEvents = recentEvents,
                RecentUsers = recentUsers,
                EventsByStatus = eventStats,
                UsersByType = userStats
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard");
            TempData["Error"] = "Failed to load dashboard data.";
            return View(new DashboardViewModel());
        }
    }

    [AllowAnonymous]
    public IActionResult Privacy() => View();

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}