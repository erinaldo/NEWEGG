using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIAddressType class.
    /// </summary>
    [DataContract]
    public enum UIAddressType
    {
        /// <summary>
        /// Gets or sets CustomerShipping.
        /// </summary>
        [EnumMember]
        CustomerShipping = 1,

        /// <summary>
        /// Gets or sets CustomerBilling.
        /// </summary>
        [EnumMember]
        CustomerBilling = 2,

        /// <summary>
        /// Gets or sets MerchantBilling.
        /// </summary>
        [EnumMember]
        MerchantBilling = 3,

        /// <summary>
        /// Gets or sets MerchantCorporation.
        /// </summary>
        [EnumMember]
        MerchantCorporation = 4,
    }
}
