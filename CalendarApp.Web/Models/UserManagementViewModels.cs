using System.ComponentModel.DataAnnotations;
using CalendarApp.Application.Abstraction;

namespace CalendarApp.Web.Models
{
    public class UserCreateViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;
    }

    public class UserListViewModel
    {
        public IList<UserWithRoleDto> Users { get; set; } = new List<UserWithRoleDto>();
    }
}