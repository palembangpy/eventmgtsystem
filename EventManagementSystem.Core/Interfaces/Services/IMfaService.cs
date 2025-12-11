namespace EventManagementSystem.Core.Interfaces.Services;

public interface IMfaService
{
    string GenerateSecret();
    string GenerateQrCodeUri(string email, string secret);
    byte[] GenerateQrCode(string uri);
    bool ValidateCode(string secret, string code);
    string GenerateBackupCode();
}
