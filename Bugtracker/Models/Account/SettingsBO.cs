﻿using Bugtracker.Models.Common.Account;
using System.ComponentModel.DataAnnotations;

namespace Bugtracker.Models.Account
{
    public class SettingsBO
    {
        [Display(Name = "Username")]
        public string? UserName { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [EmailAddress]
        [Display(Name = "New email")]
        public string? NewEmail { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        public SettingTypeEnum SettingType { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string? OldPassword { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
