namespace EventManagementSystem.Services.Helper.Tokenizer.Interfaces;

public interface IJWTGen
{
    string GenerateToken(string tokenName, string[] allowedEndpoints, int expirationDays, string createdBy);
    bool ValidateToken(string token, out string? tokenHash);
    string HashToken(string token, string salt);
    string GenerateSalt();
}
