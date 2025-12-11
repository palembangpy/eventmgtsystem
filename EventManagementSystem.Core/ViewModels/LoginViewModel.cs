using System.ComponentModel.DataAnnotations;
using EventManagementSystem.Core.Validation;
using EventManagementSystem.Core.Security;

namespace EventManagementSystem.Core.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email wajib diisi")]
        [EmailAddress]
        [StrictEmail]
        [Sanitize(LogicalMaxLength = 150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password wajib diisi")]
        [DataType(DataType.Password)]
        [Sanitize(LogicalMaxLength = 150)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class LoginCheckViewModel
    {
        [Required(ErrorMessage = "Email wajib diisi")]
        [EmailAddress]
        [StrictEmail]
        [Sanitize(LogicalMaxLength = 150)]
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}