using System.ComponentModel.DataAnnotations;
namespace EventManagementSystem.Core.Validation;

public class StrictEmailAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return new ValidationResult("Email wajib diisi");

        var email = value.ToString();

        if (!System.Text.RegularExpressions.Regex.IsMatch(email!,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            return new ValidationResult("Format email tidak valid");
        }

        return ValidationResult.Success;
    }
}
