using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIOrderDetailInfo class.
    /// </summary>
    [DataContract]
    public class UIOrderDetailInfo : UIOrderInfo
	{
        /// <summary>
        /// Gets or sets CustomerShippingInfo.
        /// </summary>
        [DataMember(Name = "CustomerShippingInfo")]
        public UIAddressInfo CustomerShippingInfo { get; set; }
		
        /// <summary>
        /// Gets or sets CustomerBillingInfo.
        /// </summary>
        [DataMember(Name = "CustomerBillingInfo")]
        public UIAddressInfo CustomerBillingInfo { get; set; }
		
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
        /// Gets or sets a value indicating whether IsShowShippingPromotionMessage.
        /// </summary>
        [DataMember(Name = "IsShowShippingPromotionMessage")]
        public bool IsShowShippingPromotionMessage { get; set; }
	}
}
