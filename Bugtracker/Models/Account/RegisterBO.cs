using System.ComponentModel.DataAnnotations;

namespace Bugtracker.Models.Account
{
    public class RegisterBO
    {
        [Display(Name = "Username")]
        public string? UserName { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string? ConfirmPassword { get; set; }
    }
}
