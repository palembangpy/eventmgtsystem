using EventManagementSystem.Core.DTO;
using EventManagementSystem.Core.Models.Entities;
using EventManagementSystem.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystem.Web.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    // GET: Users
    public async Task<IActionResult> Index(string? userType, string? search)
    {
        try
        {
            IEnumerable<UserDto> users;

            if (!string.IsNullOrEmpty(userType) && Enum.TryParse<UserType>(userType, out var type))
            {
                users = await _userService.GetUsersByTypeAsync(type);
                ViewBag.SelectedType = userType;
            }
            else
            {
                users = await _userService.GetAllUsersAsync();
            }

            // Search filter
            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(u => 
                    u.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(search, StringComparison.OrdinalIgnoreCase));
                ViewBag.SearchTerm = search;
            }

            ViewBag.UserTypes = Enum.GetNames(typeof(UserType));
            ViewBag.TotalCount = users.Count();
            
            return View(users.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            TempData["Error"] = "Failed to load users.";
            return View(new List<UserDto>());
        }
    }

    // GET: Users/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user {UserId}", id);
            return NotFound();
        }
    }

    // GET: Users/Create
    public IActionResult Create()
    {
        ViewBag.UserTypes = Enum.GetNames(typeof(UserType));
        return View();
    }

    // POST: Users/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserDto model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.UserTypes = Enum.GetNames(typeof(UserType));
            return View(model);
        }

        try
        {
            await _userService.CreateUserAsync(model);
            TempData["Success"] = $"User '{model.Name}' created successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("Email", ex.Message);
            ViewBag.UserTypes = Enum.GetNames(typeof(UserType));
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            TempData["Error"] = "Failed to create user.";
            ViewBag.UserTypes = Enum.GetNames(typeof(UserType));
            return View(model);
        }
    }

    // GET: Users/Edit/5
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            var updateDto = new UpdateUserDto
            {
                Name = user.Name,
                UserType = Enum.Parse<UserType>(user.UserType),
                Phone = user.Phone,
                Address = user.Address,
                ProfilePicture = user.ProfilePicture,
                IsActive = user.IsActive
            };

            ViewBag.UserTypes = Enum.GetNames(typeof(UserType));
            ViewBag.UserId = id;
            ViewBag.UserEmail = user.Email;
            return View(updateDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user for edit");
            return NotFound();
        }
    }

    // POST: Users/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateUserDto model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.UserTypes = Enum.GetNames(typeof(UserType));
            ViewBag.UserId = id;
            return View(model);
        }

        try
        {
            await _userService.UpdateUserAsync(id, model);
            TempData["Success"] = "User updated successfully!";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user");
            TempData["Error"] = "Failed to update user.";
            ViewBag.UserTypes = Enum.GetNames(typeof(UserType));
            ViewBag.UserId = id;
            return View(model);
        }
    }

    // GET: Users/Delete/5
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user for delete");
            return NotFound();
        }
    }

    // POST: Users/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            TempData["Success"] = "User deactivated successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user");
            TempData["Error"] = "Failed to deactivate user.";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Users/Export
    public async Task<IActionResult> Export()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            var csv = GenerateCsv(users);
            return File(csv, "text/csv", $"users-{DateTime.Now:yyyyMMdd}.csv");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting users");
            TempData["Error"] = "Failed to export users.";
            return RedirectToAction(nameof(Index));
        }
    }

    private byte[] GenerateCsv(IEnumerable<UserDto> users)
    {
        var csv = new System.Text.StringBuilder();
        csv.AppendLine("Id,Name,Email,Type,Phone,Active,Created");
        
        foreach (var user in users)
        {
            csv.AppendLine($"{user.Id},{user.Name},{user.Email},{user.UserType}," +
                          $"{user.Phone},{user.IsActive},{user.CreatedAt:yyyy-MM-dd}");
        }
        
        return System.Text.Encoding.UTF8.GetBytes(csv.ToString());
    }
}
