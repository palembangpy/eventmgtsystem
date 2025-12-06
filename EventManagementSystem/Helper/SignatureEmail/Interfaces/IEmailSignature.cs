namespace EventManagementSystem.Helper.SignatureEmail.Interfaces;

public interface IEmailSignature
{
    string CreateSignature(string userId, string token, string nonce, DateTime expiresAt);
    (string userId, string token, string nonce, DateTime expiresAt) DecodeSignature(string signature);
}
