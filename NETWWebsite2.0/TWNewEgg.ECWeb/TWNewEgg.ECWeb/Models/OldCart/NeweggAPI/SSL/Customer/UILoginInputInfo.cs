using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UILoginInputInfo class.
    /// </summary>
    [DataContract]
    public class UILoginInputInfo
    {
        /// <summary>
        /// Gets or sets LoginName.
        /// </summary>
        [DataMember(Name = "LoginName")]
        public string LoginName { get; set; }

        /// <summary>
        /// Gets or sets Password.
        /// </summary>
        [DataMember(Name = "Password")]
        public string Password { get; set; }
    }
}
