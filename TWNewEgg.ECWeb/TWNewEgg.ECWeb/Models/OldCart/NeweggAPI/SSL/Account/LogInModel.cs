using System.ComponentModel.DataAnnotations;
using Global = Newegg.Mobile.MvcApplication.Globalization.Models.Account;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Login model.
    /// </summary>
    public class LogInModel
    {
        /// <summary>
        /// Gets or sets user name.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Global.LogOnModel), ErrorMessageResourceName = "UserNameMustFill")]
        [Display(ResourceType = typeof(Global.LogOnModel), Name = "UserName")]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets password.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Global.LogOnModel), ErrorMessageResourceName = "PasswordMustFill")]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Global.LogOnModel), Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether allow site to remember me.
        /// </summary>
        [Display(ResourceType = typeof(Global.LogOnModel), Name = "RemeberMe")]
        public bool RememberMe { get; set; }

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
