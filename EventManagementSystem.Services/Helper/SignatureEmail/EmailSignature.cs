using System.Security;
using System.Security.Cryptography;
using System.Text;
using EventManagementSystem.Services.Helper.SignatureEmail.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace EventManagementSystem.Services.Helper.SignatureEmail;

public class EmailSignature : IEmailSignature
{
    private readonly string _salt;
    private readonly RSA _publicRsa;   
    private readonly RSA _privateRsa; 

    public EmailSignature(IConfiguration config)
    {
        _salt = config["Security:EmailSalt"] ?? throw new InvalidOperationException("EmailSalt missing");

        _publicRsa = RSA.Create();
        _publicRsa.ImportFromPem(File.ReadAllText(config["PathKey:Public"] ?? throw new InvalidOperationException("Public Key missing")));

        _privateRsa = RSA.Create();
        _privateRsa.ImportFromPem(File.ReadAllText(config["PathKey:Private"] ?? throw new InvalidOperationException("Private Key missing")));
    }

    public string CreateSignature(string userId, string token, string nonce, DateTime expiresAt)
    {
        var payload = $"{userId}|{nonce}|{expiresAt.Ticks}|{token}|{_salt}";
        var payloadBytes = Encoding.UTF8.GetBytes(payload);

        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.GenerateKey();
        aes.GenerateIV();

        byte[] encryptedPayload;
        using (var encryptor = aes.CreateEncryptor())
        using (var ms = new MemoryStream())
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
            cs.Write(payloadBytes, 0, payloadBytes.Length);
            cs.FlushFinalBlock();
            encryptedPayload = ms.ToArray();
        }

        var encryptedAesKey = _publicRsa.Encrypt(aes.Key, RSAEncryptionPadding.OaepSHA256);

        using var finalStream = new MemoryStream();
        finalStream.Write(encryptedAesKey);
        finalStream.Write(aes.IV);
        finalStream.Write(encryptedPayload);

        return WebEncoders.Base64UrlEncode(finalStream.ToArray());
    }

    public (string userId, string token, string nonce, DateTime expiresAt) DecodeSignature(string signature)
    {
        var fullData = WebEncoders.Base64UrlDecode(signature);
        var encryptedAesKey = fullData[..256];
        var iv = fullData[256..(256 + 16)];
        var encryptedPayload = fullData[(256 + 16)..];

        var aesKey = _privateRsa.Decrypt(encryptedAesKey, RSAEncryptionPadding.OaepSHA256);

        using var aes = Aes.Create();
        aes.Key = aesKey;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        using var ms = new MemoryStream(encryptedPayload);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        var raw = sr.ReadToEnd();
        var parts = raw.Split('|');

        if (parts.Length < 5)
            throw new SecurityException("Invalid verification signature");

        var userId = parts[0];
        var nonce = parts[1];
        if (!long.TryParse(parts[2], out var ticks))
            throw new SecurityException("Invalid expiry in signature");

        var expiresAt = new DateTime(ticks, DateTimeKind.Utc);
        var salt = parts[^1];
        var token = string.Join("|", parts.Skip(3).Take(parts.Length - 4));

        if (salt != _salt)
            throw new SecurityException("Invalid signature salt");

        return (userId, token, nonce, expiresAt);
    }
}
