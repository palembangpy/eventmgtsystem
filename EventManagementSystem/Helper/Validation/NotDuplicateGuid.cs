using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Helper.Validation;

public sealed class NoDuplicateGuidsAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not IEnumerable<Guid> list)
            return ValidationResult.Success;

        if (list.Count() != list.Distinct().Count())
            return new ValidationResult(ErrorMessage ?? "Duplikasi Guid terdeteksi");

        return ValidationResult.Success;
    }
}