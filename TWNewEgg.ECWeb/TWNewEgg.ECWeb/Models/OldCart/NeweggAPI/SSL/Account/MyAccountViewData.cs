using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the MyAccountViewData class.
    /// </summary>
    [DataContract]
    public class MyAccountViewData
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
        /// Gets or sets DefaultCreditCard.
        /// </summary>
        [DataMember(Name = "DefaultCreditCard")]
        public UICreditCardPaymentInfo DefaultCreditCard { get; set; }
    }
}
