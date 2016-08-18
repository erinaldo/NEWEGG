using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIQASAddressVerify class.
    /// </summary>
    [DataContract]
    public enum UIQASAddressVerify
    {
        /// <summary>
        /// Gets or sets NoneVerify.
        /// </summary>
        [EnumMember]
        NoneVerify = 0,

        /// <summary>
        /// Gets or sets UnVerify.
        /// </summary>
        [EnumMember]
        UnVerify = 1,

        /// <summary>
        /// Gets or sets Verified.
        /// </summary>
        [EnumMember]
        Verified = 2,

        /// <summary>
        /// Gets or sets UseEntered.
        /// </summary>
        [EnumMember]
		UseEntered = 3
    }
}
