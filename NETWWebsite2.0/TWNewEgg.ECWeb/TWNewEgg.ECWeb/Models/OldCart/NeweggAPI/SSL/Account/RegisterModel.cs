using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Global = Newegg.Mobile.MvcApplication.Globalization.Models.Account;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Register View Model.
    /// </summary>
    public class RegisterModel
    {
        /// <summary>
        /// Gets or sets Email Address.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Global.RegisterModel), ErrorMessageResourceName = "EmailAddressMustFill")]
        [StringLength(40, MinimumLength = 1, ErrorMessageResourceType = typeof(Global.RegisterModel), ErrorMessageResourceName = "EmailAddressLength")]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(Global.RegisterModel), ErrorMessageResourceName = "EmailAddressIsinvalid")]
        [RegularExpression(@"^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$", ErrorMessageResourceType = typeof(Global.RegisterModel), ErrorMessageResourceName = "EmailAddressIsinvalid")]
        [Display(ResourceType = typeof(Global.RegisterModel), Name = "EmailAddress")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets Confirm Email Address.
        /// </summary>
        [DataType(DataType.EmailAddress)]
        [System.ComponentModel.DataAnnotations.Compare("EmailAddress", ErrorMessageResourceType = typeof(Global.RegisterModel), ErrorMessageResourceName = "ComepareEmailAddress")]
        [Display(ResourceType = typeof(Global.RegisterModel), Name = "ConfirmEmailAddress")]
        public string ConfirmEmailAddress { get; set; }

        /// <summary>
        /// Gets or sets Password.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Global.RegisterModel), ErrorMessageResourceName = "PasswordMustFill")]
        [StringLength(30, MinimumLength = 6, ErrorMessageResourceType = typeof(Global.RegisterModel), ErrorMessageResourceName = "PasswordLength")]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Global.RegisterModel), Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets Confirm password.
        /// </summary>
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessageResourceType = typeof(Global.RegisterModel), ErrorMessageResourceName = "ComparePassword")]
        [Display(ResourceType = typeof(Global.RegisterModel), Name = "ConfirmPassword")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Gets or sets Next page url.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether logon for check out.
        /// </summary>
        public bool ForCheckOut { get; set; }
    }
}
