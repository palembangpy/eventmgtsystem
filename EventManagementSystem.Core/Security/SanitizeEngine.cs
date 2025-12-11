namespace EventManagementSystem.Core.Security;

using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Net;

public static class SanitizerEngine
{
    public static ISecurityLogger? Logger { get; set; }

    public static string? SanitizeInput(
        string input,
        int maxDecodeRounds = 10,
        int absoluteMaxLength = 2000,
        int logicalMaxLength = 255)
    {
        try
        {
            
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            input = WebUtility.UrlDecode(input);
            string original = input.Trim();

            if (Regex.IsMatch(original, @"(?i)(%25){0,}\s*%3C|%3C|%253C|%u003C"))
            {
                Logger?.Log("SECURITY", "SanitizeInput","Encoded angle bracket detected", original);
                return null;
            }

            string current = original;
            int rounds = 0;

            while (rounds < maxDecodeRounds)
            {
                string urlDecoded = WebUtility.UrlDecode(current);
                string htmlDecoded = WebUtility.HtmlDecode(urlDecoded);

                if (string.Equals(htmlDecoded, current, StringComparison.Ordinal))
                    break;

                current = htmlDecoded;

                if (current.Length > absoluteMaxLength)
                {
                    current = current[..absoluteMaxLength];
                    Logger?.Log("SECURITY", "SanitizeInput","Input truncated after expansion", original);
                }

                rounds++;
            }

            if (rounds >= maxDecodeRounds)
            {
                Logger?.Log("SECURITY", "SanitizeInput","Too many decode rounds", original);
                return null;
            }

            string lower = current.ToLowerInvariant();

            if (Regex.IsMatch(lower,
                @"<\s*script\b|on\w+\s*=|javascript:|vbscript:|eval\(|document\.cookie|window\.location|<\s*iframe\b",
                RegexOptions.IgnoreCase))
            {
                Logger?.Log("SECURITY", "SanitizeInput","Dangerous script token found", current);
                return null;
            }

            if (current.Length > logicalMaxLength)
                current = current[..logicalMaxLength];
            current = Regex.Replace(current, @"\p{C}+", string.Empty);
            current = Regex.Replace(current, @"<[^>]*>", string.Empty, RegexOptions.Singleline);
            current = Regex.Replace(current,
                @"(?i)(javascript:|vbscript:|data:text\/html|on\w+\s*=|document\.|window\.|eval\(|alert\(|cookie)",
                string.Empty);
            current = Regex.Replace(current, @"[<>""'`%&\(\)]", string.Empty);
            string safe = HtmlEncoder.Default.Encode(current);
            return safe;
        }
        catch (Exception ex)
        {
            Logger?.Log("ERROR", "SanitizeInput","Sanitization failure", ex.Message);
            return string.Empty;
        }
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class SanitizeAttribute : Attribute
{
    public int LogicalMaxLength { get; set; } = 255;
}
