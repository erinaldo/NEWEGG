using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIOrderContent class.
    /// </summary>
    [DataContract]
    public class UIOrderContent
    {
        /// <summary>
        /// Gets or sets SourceType.
        /// </summary>
        [DataMember(Name = "SourceType")]
        public string SourceType { get; set; }        
        
        /// <summary>
        /// Gets or sets ShippingAddressInfo.
        /// </summary>
        [DataMember(Name = "ShippingAddressInfo")]
        public UIAddressInfo ShippingAddressInfo { get; set; }
        
        /// <summary>
        /// Gets or sets BillingAddressInfo.
        /// </summary>
        [DataMember(Name = "BillingAddressInfo")]
        public UIAddressInfo BillingAddressInfo { get; set; }
        
        /// <summary>
        /// Gets or sets OrderInfos.
        /// </summary>
        [DataMember(Name = "OrderInfos")]
        public List<UIOrderInfo> OrderInfos { get; set; }
        
        /// <summary>
        /// Gets or sets PaymentInfo.
        /// </summary>
        [DataMember(Name = "PaymentInfo")]
        public UICreditCardPaymentInfo PaymentInfo { get; set; }
    }
}