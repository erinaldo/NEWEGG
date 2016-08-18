using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UICheckAddressType class.
    /// </summary>
    [DataContract]
    public enum UICheckAddressType
    {
        /// <summary>
        /// Gets or sets Shipping.
        /// </summary>
        [EnumMember]
        Shipping = 1,

        /// <summary>
        /// Gets or sets Billing.
        /// </summary>
        [EnumMember]
        Billing = 2,

        /// <summary>
        /// Gets or sets ShippingAndBilling.
        /// </summary>
        [EnumMember]
        ShippingAndBilling = 3,
    }
}
