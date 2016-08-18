using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UICustomerInfo class.
    /// </summary>
    [DataContract]
    public class UICustomerInfo
    {
        /// <summary>
        /// Gets or sets CustomerNumber.
        /// </summary>
        [DataMember(Name = "CustomerNumber")]
        public int CustomerNumber { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        [DataMember(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets LoginName.
        /// </summary>
        [DataMember(Name = "LoginName")]
        public string LoginName { get; set; }

        /// <summary>
        /// Gets or sets ZipCode.
        /// </summary>
        [DataMember(Name = "ZipCode")]
        public string ZipCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether AuthToken.
        /// </summary>
        [DataMember(Name = "AuthToken")]
        public string AuthToken { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsNewOrRepeat.
        /// </summary>
        [DataMember(Name = "IsNewOrRepeat")]
        public bool IsNewOrRepeat { get; set; }
    }
}