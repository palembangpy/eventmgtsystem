using System.ComponentModel.DataAnnotations;
namespace EventManagementSystem.Core.Validation;
public sealed class NotEmptyGuidAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is Guid guid && guid == Guid.Empty)
            return new ValidationResult(ErrorMessage ?? "Guid tidak boleh kosong");

        return ValidationResult.Success;
    }
}
