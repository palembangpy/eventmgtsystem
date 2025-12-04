using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using EventManagementSystem.Services.Interfaces;

public class UniqueEmailAttribute : ValidationAttribute, IClientModelValidator
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var userService = (IUserService)validationContext.GetService(typeof(IUserService));
        var email = value?.ToString();

        if (!string.IsNullOrEmpty(email) && userService?.EmailExistsAsync(email).Result == true)
        {
            return new ValidationResult("Email sudah terdaftar.");
        }

        return ValidationResult.Success;
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        // optional client-side validation
    }
}