using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIMyAccountData class.
    /// </summary>
    [DataContract]
    public class UIMyAccountData
    {
        /// <summary>
        /// Gets or sets DefaultShippingAddress.
        /// </summary>
        [DataMember(Name = "DefaultShippingAddress")]
        public UIAddressInfo DefaultShippingAddress { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether HasShippingAddress.
        /// </summary>
        [DataMember(Name = "HasShippingAddress")]
        public bool HasShippingAddress { get; set; }
        
        /// <summary>
        /// Gets or sets DefaultBillingAddress.
        /// </summary>
        [DataMember(Name = "DefaultBillingAddress")]
        public UIAddressInfo DefaultBillingAddress { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether HasBillingAddress.
        /// </summary>
        [DataMember(Name = "HasBillingAddress")]
        public bool HasBillingAddress { get; set; }
        
        /// <summary>
        /// Gets or sets DefaultCreditCard.
        /// </summary>
        [DataMember(Name = "DefaultCreditCard")]
        public UICreditCardPaymentInfo DefaultCreditCard { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether HasCreditCard.
        /// </summary>
        [DataMember(Name = "HasCreditCard")]
        public bool HasCreditCard { get; set; }
    }
}
