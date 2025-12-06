namespace EventManagementSystem.Services.Interfaces;

public interface IEmailService
{
    Task SendEmailVerificationAsync(string email, string userName, string verificationUrl);
    Task SendPasswordResetAsync(string email, string userName, string resetUrl);
    Task SendWelcomeEmailAsync(string email, string userName);
    Task SendMfaCodeAsync(string email, string userName, string code);
}