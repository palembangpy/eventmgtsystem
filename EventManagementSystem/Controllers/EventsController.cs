using EventManagementSystem.Core.DTO;
using EventManagementSystem.Core.Models.Entities;
using EventManagementSystem.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystem.Web.Controllers;

[Authorize]
public class EventsController : Controller
{
    private readonly IEventScheduleService _eventService;
    private readonly IUserService _userService;
    private readonly ILogger<EventsController> _logger;

    public EventsController(
        IEventScheduleService eventService,
        IUserService userService,
        ILogger<EventsController> logger)
    {
        _eventService = eventService;
        _userService = userService;
        _logger = logger;
    }

    // GET: Events
    public async Task<IActionResult> Index(string? filter, string? search)
    {
        try
        {
            IEnumerable<EventScheduleDto> events;

            events = filter?.ToLower() switch
            {
                "upcoming" => await _eventService.GetUpcomingEventsAsync(),
                "past" => await _eventService.GetPastEventsAsync(),
                "active" => await _eventService.GetActiveEventsAsync(),
                _ => await _eventService.GetAllEventsAsync()
            };

            // Search filter
            if (!string.IsNullOrEmpty(search))
            {
                events = events.Where(e => 
                    e.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    e.Location.Contains(search, StringComparison.OrdinalIgnoreCase));
                ViewBag.SearchTerm = search;
            }

            ViewBag.Filter = filter ?? "all";
            ViewBag.TotalCount = events.Count();
            
            return View(events.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting events");
            TempData["Error"] = "Failed to load events.";
            return View(new List<EventScheduleDto>());
        }
    }

    // GET: Events/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var evt = await _eventService.GetEventByIdAsync(id);
            if (evt == null)
                return NotFound();

            return View(evt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting event {EventId}", id);
            return NotFound();
        }
    }

    // GET: Events/Create
    public async Task<IActionResult> Create()
    {
        await LoadSpeakers();
        return View();
    }

    // POST: Events/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEventScheduleDto model)
    {
        if (!ModelState.IsValid)
        {
            await LoadSpeakers();
            return View(model);
        }

        try
        {
            var created = await _eventService.CreateEventAsync(model);
            TempData["Success"] = $"Event '{model.Title}' created successfully!";
            return RedirectToAction(nameof(Details), new { id = created.EventScheduleId });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            await LoadSpeakers();
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event");
            TempData["Error"] = "Failed to create event.";
            await LoadSpeakers();
            return View(model);
        }
    }

    // GET: Events/Edit/5
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var evt = await _eventService.GetEventByIdAsync(id);
            if (evt == null)
                return NotFound();

            var updateDto = new UpdateEventScheduleDto
            {
                Title = evt.Title,
                Description = evt.Description,
                StartDate = evt.StartDate,
                EndDate = evt.EndDate,
                Location = evt.Location,
                MaxParticipants = evt.MaxParticipants,
                ImageUrl = evt.ImageUrl,
                Tags = evt.Tags,
                SpeakerId = evt.SpeakerId,
                Status = Enum.Parse<EventStatus>(evt.Status),
                IsActive = evt.IsActive
            };

            await LoadSpeakers();
            ViewBag.EventId = id;
            ViewBag.EventStatuses = Enum.GetNames(typeof(EventStatus));
            return View(updateDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading event for edit");
            return NotFound();
        }
    }

    // POST: Events/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateEventScheduleDto model)
    {
        if (!ModelState.IsValid)
        {
            await LoadSpeakers();
            ViewBag.EventId = id;
            ViewBag.EventStatuses = Enum.GetNames(typeof(EventStatus));
            return View(model);
        }

        try
        {
            await _eventService.UpdateEventAsync(id, model);
            TempData["Success"] = "Event updated successfully!";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            await LoadSpeakers();
            ViewBag.EventId = id;
            ViewBag.EventStatuses = Enum.GetNames(typeof(EventStatus));
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event");
            TempData["Error"] = "Failed to update event.";
            await LoadSpeakers();
            ViewBag.EventId = id;
            return View(model);
        }
    }

    // GET: Events/Delete/5
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var evt = await _eventService.GetEventByIdAsync(id);
            if (evt == null)
                return NotFound();

            return View(evt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading event for delete");
            return NotFound();
        }
    }

    // POST: Events/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            await _eventService.DeleteEventAsync(id);
            TempData["Success"] = "Event deactivated successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting event");
            TempData["Error"] = "Failed to deactivate event.";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Events/Calendar
    public async Task<IActionResult> Calendar()
    {
        try
        {
            var events = await _eventService.GetActiveEventsAsync();
            return View(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading calendar");
            return View(new List<EventScheduleDto>());
        }
    }

    private async Task LoadSpeakers()
    {
        var speakers = await _userService.GetSpeakersAsync();
        ViewBag.Speakers = speakers;
    }
}
