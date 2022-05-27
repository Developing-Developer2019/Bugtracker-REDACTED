using Bugtracker.Models.Common.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Bugtracker.Models.Account
{
    public class LoginBO : BaseEntity<int>
    {
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
