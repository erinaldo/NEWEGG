using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UICustomerDetailInfo class.
    /// </summary>
    [DataContract]
    public class UICustomerDetailInfo : UICustomerBasicInfo
    {
        /// <summary>
        /// Gets or sets ShippingAddressInfos.
        /// </summary>
        [DataMember(Name = "ShippingAddressInfos")]
        public List<UIAddressInfo> ShippingAddressInfos { get; set; }

        /// <summary>
        /// Gets or sets CreditCardPaymentInfos.
        /// </summary>
        [DataMember(Name = "CreditCardPaymentInfos")]
        public List<UICreditCardPaymentInfo> CreditCardPaymentInfos { get; set; }
    }
}