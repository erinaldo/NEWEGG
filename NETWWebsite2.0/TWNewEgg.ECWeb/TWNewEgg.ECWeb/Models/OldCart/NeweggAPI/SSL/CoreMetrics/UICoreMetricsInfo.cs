using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UICoreMetricsInfo class.
    /// </summary>
    [DataContract]
	public class UICoreMetricsInfo
	{
        /// <summary>
        /// Initializes a new instance of the UICoreMetricsInfo class.
        /// </summary>
		public UICoreMetricsInfo()
		{
			this.BaseTagInfo = new UIBaseTagInfo();
			this.RegistrationTagInfo = new UIRegistrationTagInfo();
		}

        /// <summary>
        /// Gets or sets ShopAction5TagInfoList.
        /// </summary>
        [DataMember(Name = "ShopAction5TagInfoList")]
        public List<UIShopAction5TagInfo> ShopAction5TagInfoList { get; set; }
		
        /// <summary>
        /// Gets or sets ShopAction9TagInfoList.
        /// </summary>
        [DataMember(Name = "ShopAction9TagInfoList")]
        public List<UIShopAction9TagInfo> ShopAction9TagInfoList { get; set; }
		
        /// <summary>
        /// Gets or sets OrderTagInfoList.
        /// </summary>
        [DataMember(Name = "OrderTagInfoList")]
        public List<UIOrderTagInfo> OrderTagInfoList { get; set; }
		
        /// <summary>
        /// Gets or sets PromoCodeInfoList.
        /// </summary>
        [DataMember(Name = "PromoCodeInfoList")]
        public List<UIConversionEventTagInfo> PromoCodeInfoList { get; set; }
		
        /// <summary>
        /// Gets or sets PaymentMethodList.
        /// </summary>
        [DataMember(Name = "PaymentMethodList")]
        public List<UIConversionEventTagInfo> PaymentMethodList { get; set; }
		
        /// <summary>
        /// Gets or sets ShippingMethodList.
        /// </summary>
        [DataMember(Name = "ShippingMethodList")]
        public List<UIConversionEventTagInfo> ShippingMethodList { get; set; }
		
        /// <summary>
        /// Gets or sets ServerName.
        /// </summary>
        [DataMember(Name = "ServerName")]
        public string ServerName { get; set; }
		
        /// <summary>
        /// Gets or sets CustomerID.
        /// </summary>
        [DataMember(Name = "CustomerID")]
        public string CustomerID { get; set; }
		
        /// <summary>
        /// Gets or sets CustomerEmail.
        /// </summary>
        [DataMember(Name = "CustomerEmail")]
        public string CustomerEmail { get; set; }
		
        /// <summary>
        /// Gets or sets CustomerCity.
        /// </summary>
        [DataMember(Name = "CustomerCity")]
        public string CustomerCity { get; set; }
		
        /// <summary>
        /// Gets or sets CustomerState.
        /// </summary>
        [DataMember(Name = "CustomerState")]
        public string CustomerState { get; set; }
		
        /// <summary>
        /// Gets or sets CustomerZip.
        /// </summary>
        [DataMember(Name = "CustomerZip")]
        public string CustomerZip { get; set; }
		
        /// <summary>
        /// Gets or sets NewsletterName.
        /// </summary>
        [DataMember(Name = "NewsletterName")]
        public string NewsletterName { get; set; }
		
        /// <summary>
        /// Gets or sets SubscribedFlag.
        /// </summary>
        [DataMember(Name = "SubscribedFlag")]
        public string SubscribedFlag { get; set; }
		
        /// <summary>
        /// Gets or sets BaseTagInfo.
        /// </summary>
        [DataMember(Name = "BaseTagInfo")]
        public UIBaseTagInfo BaseTagInfo { get; set; }
		
        /// <summary>
        /// Gets or sets RegistrationTagInfo.
        /// </summary>
        [DataMember(Name = "RegistrationTagInfo")]
        public UIRegistrationTagInfo RegistrationTagInfo { get; set; }
	}
}
