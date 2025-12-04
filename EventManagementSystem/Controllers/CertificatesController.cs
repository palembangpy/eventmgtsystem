using EventManagementSystem.Models.DTO;
using EventManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystem.Controllers;

[Authorize]
public class CertificatesController : Controller
{
    private readonly ICertificateService _certService;
    private readonly IUserService _userService;
    private readonly IEventScheduleService _eventService;
    private readonly ILogger<CertificatesController> _logger;

    public CertificatesController(
        ICertificateService certService,
        IUserService userService,
        IEventScheduleService eventService,
        ILogger<CertificatesController> logger)
    {
        _certService = certService;
        _userService = userService;
        _eventService = eventService;
        _logger = logger;
    }

    // GET: Certificates
    public async Task<IActionResult> Index(string? search)
    {
        try
        {
            var certs = await _certService.GetAllCertificatesAsync();

            // Search filter
            if (!string.IsNullOrEmpty(search))
            {
                certs = certs.Where(c => 
                    c.UserName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    c.CertificateNumber.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    c.EventTitle.Contains(search, StringComparison.OrdinalIgnoreCase));
                ViewBag.SearchTerm = search;
            }

            ViewBag.TotalCount = certs.Count();
            return View(certs.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting certificates");
            TempData["Error"] = "Failed to load certificates.";
            return View(new List<CertificateDto>());
        }
    }

    // GET: Certificates/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var cert = await _certService.GetCertificateByIdAsync(id);
            if (cert == null)
                return NotFound();

            return View(cert);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting certificate {CertId}", id);
            return NotFound();
        }
    }

    // GET: Certificates/Generate
    public async Task<IActionResult> Generate()
    {
        await LoadUsersAndEvents();
        return View();
    }

    // POST: Certificates/Generate
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate(GenerateCertificateDto model)
    {
        if (!ModelState.IsValid)
        {
            await LoadUsersAndEvents();
            return View(model);
        }

        try
        {
            var cert = await _certService.GenerateCertificateAsync(model);
            TempData["Success"] = $"Certificate {cert.CertificateNumber} generated successfully!";
            return RedirectToAction(nameof(Details), new { id = cert.CertificateId });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            await LoadUsersAndEvents();
            return View(model);
        }
        catch (KeyNotFoundException ex)
        {
            ModelState.AddModelError("", ex.Message);
            await LoadUsersAndEvents();
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating certificate");
            TempData["Error"] = "Failed to generate certificate.";
            await LoadUsersAndEvents();
            return View(model);
        }
    }

    // GET: Certificates/BulkGenerate
    public async Task<IActionResult> BulkGenerate()
    {
        var events = await _eventService.GetAllEventsAsync();
        ViewBag.Events = events;
        return View();
    }

    // POST: Certificates/BulkGenerate
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BulkGenerate(BulkGenerateCertificatesDto model)
    {
        if (!ModelState.IsValid || !model.UserIds.Any())
        {
            var events = await _eventService.GetAllEventsAsync();
            ViewBag.Events = events;
            ModelState.AddModelError("", "Please select at least one user.");
            return View(model);
        }

        try
        {
            var certs = await _certService.BulkGenerateCertificatesAsync(model);
            TempData["Success"] = $"Generated {certs.Count} certificates successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk generating certificates");
            TempData["Error"] = "Failed to generate certificates.";
            var events = await _eventService.GetAllEventsAsync();
            ViewBag.Events = events;
            return View(model);
        }
    }

    // GET: Certificates/Download/5
    public async Task<IActionResult> Download(Guid id)
    {
        try
        {
            var cert = await _certService.GetCertificateByIdAsync(id);
            if (cert == null)
                return NotFound();

            var fileBytes = await _certService.DownloadCertificateAsync(id);
            return File(fileBytes, "application/pdf", $"{cert.CertificateNumber}.pdf");
        }
        catch (FileNotFoundException)
        {
            TempData["Error"] = "Certificate file not found.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading certificate");
            TempData["Error"] = "Failed to download certificate.";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Certificates/Verify
    public IActionResult Verify() => View();

    // POST: Certificates/Verify
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Verify(string certificateNumber)
    {
        if (string.IsNullOrEmpty(certificateNumber))
        {
            ViewBag.Error = "Please enter a certificate number.";
            return View();
        }

        try
        {
            var cert = await _certService.GetByNumberAsync(certificateNumber);
            if (cert == null)
            {
                ViewBag.Error = "Certificate not found.";
                return View();
            }

            ViewBag.Certificate = cert;
            ViewBag.Success = true;
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying certificate");
            ViewBag.Error = "Error verifying certificate.";
            return View();
        }
    }

    // GET: Certificates/ByUser/5
    public async Task<IActionResult> ByUser(Guid userId)
    {
        try
        {
            var certs = await _certService.GetCertificatesByUserAsync(userId);
            var user = await _userService.GetUserByIdAsync(userId);
            ViewBag.UserName = user?.Name ?? "Unknown";
            return View("Index", certs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting certificates by user");
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Certificates/ByEvent/5
    public async Task<IActionResult> ByEvent(Guid eventId)
    {
        try
        {
            var certs = await _certService.GetCertificatesByEventAsync(eventId);
            var evt = await _eventService.GetEventByIdAsync(eventId);
            ViewBag.EventTitle = evt?.Title ?? "Unknown";
            return View("Index", certs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting certificates by event");
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Certificates/Delete/5
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var cert = await _certService.GetCertificateByIdAsync(id);
            if (cert == null)
                return NotFound();

            return View(cert);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading certificate for delete");
            return NotFound();
        }
    }

    // POST: Certificates/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            await _certService.DeleteCertificateAsync(id);
            TempData["Success"] = "Certificate deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting certificate");
            TempData["Error"] = "Failed to delete certificate.";
            return RedirectToAction(nameof(Index));
        }
    }

    private async Task LoadUsersAndEvents()
    {
        var users = await _userService.GetAllUsersAsync();
        var events = await _eventService.GetAllEventsAsync();
        
        ViewBag.Users = users;
        ViewBag.Events = events;
    }
}