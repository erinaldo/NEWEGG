using System.ComponentModel.DataAnnotations;
using Global = Newegg.Mobile.MvcApplication.Globalization.Models.Account;

namespace Newegg.Mobile.MvcApplication.Models.Account
{
    /// <summary>
    /// Logon model.
    /// </summary>
    public class LogOnModel
    {
        /// <summary>
        /// Gets or sets user name.
        /// </summary>
        [Required]
        [Display(ResourceType = typeof(Global.LogOnModel), Name = "UserNameDisplay")]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Global.LogOnModel), Name = "PasswordDisplay")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether allow site to remember me.
        /// </summary>
        [Display(ResourceType = typeof(Global.LogOnModel), Name = "RemeberMeDisplay")]
        public bool RememberMe { get; set; }
    }
}
