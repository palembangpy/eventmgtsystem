using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EventManagementSystem.Models.Entities;
using EventManagementSystem.Models.ViewModels;
using EventManagementSystem.Services.Interfaces;
using EventManagementSystem.Models.DTO;
using EventManagementSystem.Helper.SignatureEmail.Interfaces;
using System.Security;

namespace EventManagementSystem.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserService _userService;
    private readonly IEmailSignature _emailSignature;
    private readonly IEmailService _emailService;
    private readonly IMfaService _mfaService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IUserService service,
        IEmailSignature emailSignature,
        IEmailService emailService,
        IMfaService mfaService,
        ILogger<AccountController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userService = service;
        _emailSignature = emailSignature;
        _emailService = emailService;
        _mfaService = mfaService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);


        var user = await _userManager.FindByEmailAsync(model.Email);
        var IsActive = await _userService.IsUserActiveAsync(model.Email);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "User does not exist.");
            return View(model);
        }

        if (!user.IsEmailVerified && IsActive == false) 
        {
            ModelState.AddModelError(string.Empty, "Please verify your email before logging in for activate account.");
            TempData["Error"] = "Your email is not verified. Please check your inbox for the verification link.";
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            if (user.IsMfaEnabled)
            {
                HttpContext.Session.SetString("MfaUserId", user.Id);
                return RedirectToAction(nameof(VerifyMfa));
            }
            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("User {Email} logged in", model.Email);
            return RedirectToLocal(returnUrl);
        }

        if (result.IsLockedOut)
        {
            _logger.LogWarning("User {Email} account locked out", model.Email);
            return RedirectToAction(nameof(Lockout));
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    [HttpGet]
    public IActionResult Register(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation("User created a new account");
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var nonce = Guid.NewGuid().ToString("N").Substring(0, 12);
            var expiresAt = DateTime.UtcNow.AddHours(1);

             user.EmailVerificationId = nonce;
            user.EmailVerificationExpiresAt = expiresAt;
            user.IsEmailVerified = false;
            await _userManager.UpdateAsync(user);

            var signature = _emailSignature.CreateSignature(user.Id, token, nonce, expiresAt);

            var verifyUrl = Url.Action(
                    nameof(VerifyEmail),
                    "Account",
                    new { sig = signature },
                    protocol: Request.Scheme);

            await _emailService.SendEmailVerificationAsync(user.Email!, user.FullName, verifyUrl!);

            var existingUser = await _userService.EmailExistsAsync(user.Email);
            if (!existingUser)
            {
                await _userService.CreateUserAsync(new CreateUserDto
                {
                    Name = user.FullName,
                    Email = user.Email,
                    UserType = UserType.User,
                    IsActive = false
                });
            }

            TempData["Success"] = "Registration successful. Check your email to verify.";
            return RedirectToAction(nameof(RegistrationConfirmation));
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }

    [HttpGet]
    public IActionResult RegistrationConfirmation()
    {
        return View();
    }

    [HttpGet]
    public IActionResult EmailVerified()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ResendVerificationEmail()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> VerifyEmail(string sig)
    {
        if (string.IsNullOrWhiteSpace(sig))
        {
            TempData["Error"] = "Invalid verification link.";
            return RedirectToAction(nameof(Login));
        }

        try
        {
            var (userId, token, nonce, expiresAt) = _emailSignature.DecodeSignature(sig);
            if (DateTime.UtcNow > expiresAt)
            {
                TempData["Error"] = "Verification link has expired. Please request a new one.";
                return RedirectToAction(nameof(Login));
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "Invalid verification request.";
                return RedirectToAction(nameof(Login));
            }
            if (string.IsNullOrEmpty(user.EmailVerificationId) || !string.Equals(user.EmailVerificationId, nonce, StringComparison.Ordinal))
            {
                TempData["Error"] = "Verification link was revoked or is invalid. Please request a new verification email.";
                return RedirectToAction(nameof(Login));
            }
            if (user.EmailVerificationExpiresAt.HasValue && DateTime.UtcNow > user.EmailVerificationExpiresAt.Value)
            {
                TempData["Error"] = "Verification link has expired. Please request a new one.";
                return RedirectToAction(nameof(Login));
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                TempData["Error"] = "Email verification failed. The token may be invalid.";
                return RedirectToAction(nameof(Login));
            }
            user.IsEmailVerified = true;
            user.EmailVerifiedAt = DateTime.UtcNow;
            user.EmailVerificationId = null;
            user.EmailVerificationExpiresAt = null;
            await _userManager.UpdateAsync(user);
            await _signInManager.SignInAsync(user, isPersistent: false);

            var existingUser = await _userService.GetUserByEmailAsync(user.Email);
            if (existingUser != null)
            {
                await _userService.UpdateUserAsync(existingUser.Id, new UpdateUserDto
                {
                    IsActive = true
                });
            }

            await _emailService.SendWelcomeEmailAsync(user.Email!, user.FullName);

            TempData["Success"] = "Email verified successfully!";
            return RedirectToAction(nameof(EmailVerified));
        }
        catch (SecurityException)
        {
            TempData["Error"] = "Invalid verification link.";
            return RedirectToAction(nameof(Login));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying email link");
            TempData["Error"] = "Verification failed. Please request a new verification link.";
            return RedirectToAction(nameof(Login));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendVerificationEmail(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || user.EmailConfirmed) return RedirectToAction(nameof(Login));

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var nonce = Guid.NewGuid().ToString("N").Substring(0, 12);
        var expiresAt = DateTime.UtcNow.AddHours(24);

        user.EmailVerificationId = nonce;
        user.EmailVerificationExpiresAt = expiresAt;
        await _userManager.UpdateAsync(user);

        var signature = _emailSignature.CreateSignature(user.Id, token, nonce, expiresAt);
        var verifyUrl = Url.Action(nameof(VerifyEmail), "Account", new { sig = signature }, protocol: Request.Scheme);
        await _emailService.SendEmailVerificationAsync(user.Email!, user.FullName, verifyUrl!);
        TempData["Success"] = "A new verification email has been sent.";
        return RedirectToAction(nameof(RegistrationConfirmation));
    }

    [HttpGet]
    public IActionResult VerifyMfa()
    {
        var userId = HttpContext.Session.GetString("MfaUserId");
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction(nameof(Login));

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyMfa(string code)
    {
        var userId = HttpContext.Session.GetString("MfaUserId");
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction(nameof(Login));

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || string.IsNullOrEmpty(user.MfaSecret))
            return RedirectToAction(nameof(Login));

        if (_mfaService.ValidateCode(user.MfaSecret, code))
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            HttpContext.Session.Remove("MfaUserId");

            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("User {Email} completed MFA verification", user.Email);
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Invalid code. Please try again.");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out");
        return RedirectToAction("Login");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ExternalLogin(string provider, string? returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    [HttpGet]
    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
    {
        if (remoteError != null)
        {
            _logger.LogError($"Error from external provider: {remoteError}");
            TempData["Error"] = $"Error from external provider: {remoteError}";
            return RedirectToAction(nameof(Login));
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            _logger.LogWarning("Could not get external login info");
            return RedirectToAction(nameof(Login));
        }

        var result = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: false);

        if (result.Succeeded)
        {
            _logger.LogInformation($"User logged in with {info.LoginProvider}");
            return RedirectToLocal(returnUrl);
        }

        var email = info.Principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var name = info.Principal.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(email))
        {
            TempData["Error"] = "Email not received from provider.";
            return RedirectToAction(nameof(Login));
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = name ?? email,
            IsEmailVerified = true,
            EmailVerifiedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(user);
        if (createResult.Succeeded)
        {
            createResult = await _userManager.AddLoginAsync(user, info);
            if (createResult.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                await _emailService.SendWelcomeEmailAsync(user.Email!, user.FullName);
                _logger.LogInformation($"User created account using {info.LoginProvider}");
            
                var existingUser = await _userService.EmailExistsAsync(user.Email);
                if (!existingUser)
                {
                    await _userService.CreateUserAsync(new CreateUserDto
                    {
                        Name = user.FullName,
                        Email = user.Email,
                        UserType = UserType.User,
                        IsActive = true
                    });
                }

                return RedirectToLocal(returnUrl);
            }
        }

        foreach (var error in createResult.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View("Login");
    }

    [HttpGet]
    public IActionResult Lockout()
    {
        return View();
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
        return RedirectToAction("Index", "Home");
    }
}/*  */