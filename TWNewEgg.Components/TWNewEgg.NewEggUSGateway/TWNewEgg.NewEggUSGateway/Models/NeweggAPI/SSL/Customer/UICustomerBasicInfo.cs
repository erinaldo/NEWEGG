using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UICustomerBasicInfo class.
    /// </summary>
    [DataContract]
    public class UICustomerBasicInfo : UICustomerInfo
    {
        /// <summary>
        /// Gets or sets DefaultShippingAddress.
        /// </summary>
        [DataMember(Name = "DefaultShippingAddress")]
        public UIAddressInfo DefaultShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets DefaultBillingAddress.
        /// </summary>
        [DataMember(Name = "DefaultBillingAddress")]
        public UIAddressInfo DefaultBillingAddress { get; set; }

        /// <summary>
        /// Gets or sets DefaultPaymentInfo.
        /// </summary>
        [DataMember(Name = "DefaultPaymentInfo")]
        public UICreditCardPaymentInfo DefaultPaymentInfo { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether IsNewOrRepeat.
        /// </summary>
        [DataMember(Name = "IsNewOrRepeat")]
        public bool IsNewOrRepeat { get; set; }
    }
}