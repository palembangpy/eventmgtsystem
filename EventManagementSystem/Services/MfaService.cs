namespace EventManagementSystem.Services;

using System.Security.Cryptography;
using EventManagementSystem.Services.Interfaces;
using OtpNet;
using QRCoder;

public class MfaService : IMfaService
{
    private readonly IConfiguration _config;

    public MfaService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateSecret()
    {
        var bytes = new byte[20];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        return Base32Encoding.ToString(bytes);
    }

    public string GenerateQrCodeUri(string email, string secret)
    {
        var appName = _config["AppSettings:AppName"] ?? "Event Management System";
        return $"otpauth://totp/{appName}:{email}?secret={secret}&issuer={appName}";
    }

    public byte[] GenerateQrCode(string uri)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        return qrCode.GetGraphic(20);
    }

    public bool ValidateCode(string secret, string code)
    {
        var totp = new Totp(Base32Encoding.ToBytes(secret));
        return totp.VerifyTotp(code, out _, new VerificationWindow(2, 2));
    }

    public string GenerateBackupCode()
    {
        var bytes = new byte[8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        return Convert.ToBase64String(bytes).Replace("+", "").Replace("/", "").Replace("=", "")[..12];
    }
}