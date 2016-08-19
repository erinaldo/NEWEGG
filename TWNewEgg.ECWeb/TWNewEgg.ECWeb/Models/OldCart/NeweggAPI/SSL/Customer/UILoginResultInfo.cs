using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UILoginResultInfo class.
    /// </summary>
    [DataContract]
    public class UILoginResultInfo
    {
        /// <summary>
        /// Gets or sets Customer.
        /// </summary>
        [DataMember(Name = "Customer")]
        public UICustomerInfo Customer { get; set; }

        /// <summary>
        /// Gets or sets LoginStatus.
        /// </summary>
        [DataMember(Name = "LoginStatus")]
        public UILoginStatus LoginStatus { get; set; }

        /// <summary>
        /// Gets or sets ErrorMessage.
        /// </summary>
        [DataMember(Name = "ErrorMessage")]
        public string ErrorMessage { get; set; }
    }
}