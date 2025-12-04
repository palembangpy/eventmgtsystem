using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Helper.Validation;

public sealed class NoEmptyGuidInCollectionAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not IEnumerable<Guid> list)
            return ValidationResult.Success;

        if (list.Any(x => x == Guid.Empty))
            return new ValidationResult(ErrorMessage ?? "Guid kosong tidak diperbolehkan");

        return ValidationResult.Success;
    }
}