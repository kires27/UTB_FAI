using System.ComponentModel.DataAnnotations;

namespace CalendarApp.Application.Models
{
    public class RegisterViewModel
    {
        [Required]
        [RegularExpression("^[a-zA-Z0-9_.-]{3,}$", ErrorMessage = "Username must be at least 3 characters and may only contain letters, digits, '.', '_' and '-'.")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username or email")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}