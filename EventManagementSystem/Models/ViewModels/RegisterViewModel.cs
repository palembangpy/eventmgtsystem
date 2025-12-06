using System.ComponentModel.DataAnnotations;
using EventManagementSystem.Helper.Validation;
using EventManagementSystem.Security;

namespace EventManagementSystem.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full Name wajib diisi")]
        [Display(Name = "Full Name")]
        [Sanitize(LogicalMaxLength = 100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email wajib diisi")]
        [EmailAddress]
        [Display(Name = "Email")]
        [UniqueEmail]
        [StrictEmail]
        [Sanitize(LogicalMaxLength = 150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password wajib diisi")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Sanitize(LogicalMaxLength = 100)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password wajib diisi")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        [Sanitize(LogicalMaxLength = 100)]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}