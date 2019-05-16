using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Teleboard.Attributes;
using Teleboard.Validation.Attribute;

namespace Teleboard.Models
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }
    }

    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [RequiredField]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [LocalizedDisplayName(Name: "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [LocalizedDisplayName(Name: "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [RequiredField]
        [DataType(DataType.Password)]
        [LocalizedDisplayName(Name: "Current password")]
        public string OldPassword { get; set; }

        [RequiredField]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [LocalizedDisplayName(Name: "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [LocalizedDisplayName(Name: "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [RequiredField]
        [Phone]
        [LocalizedDisplayName(Name: "Phone Number")]
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [RequiredField]
        [LocalizedDisplayName(Name: "Code")]
        public string Code { get; set; }

        [RequiredField]
        [Phone]
        [LocalizedDisplayName(Name: "Phone Number")]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }
}