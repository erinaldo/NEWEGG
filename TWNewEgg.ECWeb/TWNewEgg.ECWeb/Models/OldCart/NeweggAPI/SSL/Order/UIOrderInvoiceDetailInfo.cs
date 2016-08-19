using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIOrderInvoiceDetailInfo class.
    /// </summary>
    [DataContract]
    public class UIOrderInvoiceDetailInfo : UIOrderInfo
    {
        /// <summary>
        /// Gets or sets ShipTo.
        /// </summary>
        [DataMember(Name = "ShipTo")]
        public List<string> ShipTo { get; set; }
        
        /// <summary>
        /// Gets or sets BillingTo.
        /// </summary>
        [DataMember(Name = "BillingTo")]
        public List<string> BillingTo { get; set; }
        
        /// <summary>
        /// Gets or sets CustomerCreditCardInfo.
        /// </summary>
        [DataMember(Name = "CustomerCreditCardInfo")]
        public UICreditCardPaymentInfo CustomerCreditCardInfo { get; set; }
        
        /// <summary>
        /// Gets or sets OrderTrackInfoList.
        /// </summary>
        [DataMember(Name = "OrderTrackInfoList")]
        public List<UIOrderTrackInfo> OrderTrackInfoList { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether IsShowAddToCart.
        /// </summary>
        [DataMember(Name = "IsShowAddToCart")]
        public bool IsShowAddToCart { get; set; }
    }
}
