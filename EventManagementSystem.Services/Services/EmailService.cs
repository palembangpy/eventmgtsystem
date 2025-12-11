using EventManagementSystem.Core.Interfaces.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
namespace EventManagementSystem.Services.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendEmailVerificationAsync(string email, string userName, string verificationUrl)
    {
        var subject = "Verify Your Email - Event Management System";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; padding: 20px;'>
                <div style='max-width: 600px; margin: 0 auto; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px; border-radius: 10px;'>
                    <h1 style='color: white; text-align: center;'>Welcome to Event Management System!</h1>
                </div>
                <div style='max-width: 600px; margin: 20px auto; padding: 30px; background: #f9f9f9; border-radius: 10px;'>
                    <h2 style='color: #333;'>Hello {userName},</h2>
                    <p style='color: #666; font-size: 16px; line-height: 1.6;'>
                        Thank you for registering! Please verify your email address to access your dashboard.
                    </p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{verificationUrl}' 
                           style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); 
                                  color: white; 
                                  padding: 15px 40px; 
                                  text-decoration: none; 
                                  border-radius: 5px; 
                                  font-weight: bold;
                                  display: inline-block;'>
                            Verify Email Address
                        </a>
                    </div>
                    <p style='color: #666; font-size: 14px;'>
                        Or copy and paste this link into your browser:<br>
                        <a href='{verificationUrl}' style='color: #667eea;'>{verificationUrl}</a>
                    </p>
                    <p style='color: #999; font-size: 12px; margin-top: 30px;'>
                        This link will expire in 24 hours. If you didn't create an account, please ignore this email.
                    </p>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendPasswordResetAsync(string email, string userName, string resetUrl)
    {
        var subject = "Reset Your Password - Event Management System";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; padding: 20px;'>
                <div style='max-width: 600px; margin: 0 auto; background: #f44336; padding: 40px; border-radius: 10px;'>
                    <h1 style='color: white; text-align: center;'>Password Reset Request</h1>
                </div>
                <div style='max-width: 600px; margin: 20px auto; padding: 30px; background: #f9f9f9; border-radius: 10px;'>
                    <h2 style='color: #333;'>Hello {userName},</h2>
                    <p style='color: #666; font-size: 16px; line-height: 1.6;'>
                        We received a request to reset your password. Click the button below to create a new password.
                    </p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{resetUrl}' 
                           style='background: #f44336; 
                                  color: white; 
                                  padding: 15px 40px; 
                                  text-decoration: none; 
                                  border-radius: 5px; 
                                  font-weight: bold;
                                  display: inline-block;'>
                            Reset Password
                        </a>
                    </div>
                    <p style='color: #999; font-size: 12px; margin-top: 30px;'>
                        This link will expire in 1 hour. If you didn't request a password reset, please ignore this email.
                    </p>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendWelcomeEmailAsync(string email, string userName)
    {
        var subject = "Welcome to Event Management System!";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; padding: 20px;'>
                <div style='max-width: 600px; margin: 0 auto; background: linear-gradient(135deg, #11998e 0%, #38ef7d 100%); padding: 40px; border-radius: 10px;'>
                    <h1 style='color: white; text-align: center;'>Email Verified Successfully! ✓</h1>
                </div>
                <div style='max-width: 600px; margin: 20px auto; padding: 30px; background: #f9f9f9; border-radius: 10px;'>
                    <h2 style='color: #333;'>Welcome aboard, {userName}! 🎉</h2>
                    <p style='color: #666; font-size: 16px; line-height: 1.6;'>
                        Your email has been verified successfully. You now have full access to all features:
                    </p>
                    <ul style='color: #666; font-size: 16px; line-height: 2;'>
                        <li>Create and manage events</li>
                        <li>Generate certificates</li>
                        <li>Track participants</li>
                        <li>Access dashboard analytics</li>
                    </ul>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{_config["AppSettings:BaseUrl"]}/Account/Login' 
                           style='background: linear-gradient(135deg, #11998e 0%, #38ef7d 100%); 
                                  color: white; 
                                  padding: 15px 40px; 
                                  text-decoration: none; 
                                  border-radius: 5px; 
                                  font-weight: bold;
                                  display: inline-block;'>
                            Go to Dashboard
                        </a>
                    </div>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendMfaCodeAsync(string email, string userName, string code)
    {
        var subject = "Your MFA Code - Event Management System";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; padding: 20px;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 30px; background: #f9f9f9; border-radius: 10px;'>
                    <h2 style='color: #333;'>Hello {userName},</h2>
                    <p style='color: #666; font-size: 16px;'>
                        Your Multi-Factor Authentication code is:
                    </p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); 
                                    color: white; 
                                    padding: 20px; 
                                    border-radius: 10px; 
                                    font-size: 32px; 
                                    font-weight: bold; 
                                    letter-spacing: 10px;
                                    display: inline-block;'>
                            {code}
                        </div>
                    </div>
                    <p style='color: #999; font-size: 12px; margin-top: 30px;'>
                        This code will expire in 5 minutes. Do not share this code with anyone.
                    </p>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(email, subject, body);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(
                _config["EmailSettings:SenderName"], 
                _config["EmailSettings:SenderEmail"]
            ));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                _config["EmailSettings:SmtpServer"], 
                int.Parse(_config["EmailSettings:SmtpPort"] ?? "587"), 
                SecureSocketOptions.StartTls
            );
            
            await smtp.AuthenticateAsync(
                _config["EmailSettings:SmtpUsername"], 
                _config["EmailSettings:SmtpPassword"]
            );
            
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            throw;
        }
    }
}