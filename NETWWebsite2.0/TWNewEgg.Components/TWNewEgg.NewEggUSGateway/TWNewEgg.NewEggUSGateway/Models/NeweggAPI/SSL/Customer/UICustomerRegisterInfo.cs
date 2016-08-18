using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UICustomerRegisterInfo class.
    /// </summary>
    [DataContract]
    public class UICustomerRegisterInfo
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

        /// <summary>
        /// Gets or sets AccountType.
        /// </summary>
        [DataMember(Name = "AccountType")]
        public int AccountType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether AllowNewsLetter.
        /// </summary>
        [DataMember(Name = "AllowNewsLetter")]
        public bool AllowNewsLetter { get; set; }
    }
}
