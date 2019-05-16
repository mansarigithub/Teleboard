using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Teleboard.Attributes;
using Teleboard.Validation.Attribute;

namespace Teleboard.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [RequiredField]
        [LocalizedDisplayName(Name: "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [RequiredField]
        public string Provider { get; set; }

        [RequiredField]
        [LocalizedDisplayName(Name: "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [LocalizedDisplayName(Name: "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [RequiredField]
        [LocalizedDisplayName(Name: "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [RequiredField]
        [LocalizedDisplayName(Name: "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [RequiredField]
        [DataType(DataType.Password)]
        [LocalizedDisplayName(Name: "Password")]
        public string Password { get; set; }

        [LocalizedDisplayName(Name: "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [RequiredField]
        [EmailAddress]
        [LocalizedDisplayName(Name: "Email")]
        public string Email { get; set; }

        [RequiredField]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [LocalizedDisplayName(Name: "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [LocalizedDisplayName(Name: "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }

    public class ResetPasswordViewModel
    {
        [RequiredField]
        [EmailAddress]
        [LocalizedDisplayName(Name: "Email")]
        public string Email { get; set; }

        [RequiredField]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [LocalizedDisplayName(Name: "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [LocalizedDisplayName(Name: "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }

        public bool IsInvalidEmail { get; set; }

        public bool IsInvalidCode { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [RequiredField]
        [EmailAddress]
        [LocalizedDisplayName(Name: "Email")]
        public string Email { get; set; }

        public bool ShowEmailSentMessage { get; set; }
    }
}
