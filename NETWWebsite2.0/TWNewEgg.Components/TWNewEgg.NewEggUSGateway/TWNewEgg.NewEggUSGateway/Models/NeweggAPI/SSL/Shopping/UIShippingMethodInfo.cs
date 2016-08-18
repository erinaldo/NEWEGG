using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIShippingMethodInfo class.
    /// </summary>
    [DataContract]
    public class UIShippingMethodInfo
	{
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        [DataMember(Name = "Code")]
        public string Code { get; set; }
		
        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        [DataMember(Name = "Name")]
        public string Name { get; set; }
		
        /// <summary>
        /// Gets or sets ShippingChargeInfo.
        /// </summary>
        [DataMember(Name = "ShippingChargeInfo")]
        public string ShippingChargeInfo { get; set; }
		
        /// <summary>
        /// Gets or sets BrokerFeeInfo.
        /// </summary>
        [DataMember(Name = "BrokerFeeInfo")]
        public string BrokerFeeInfo { get; set; }
		
        /// <summary>
        /// Gets or sets a value indicating whether IsDefault.
        /// </summary>
        [DataMember(Name = "IsDefault")]
        public bool IsDefault { get; set; }
		
        /// <summary>
        /// Gets or sets a value indicating whether IsSamDayOrder.
        /// </summary>
        [DataMember(Name = "IsSamDayOrder")]
        public bool IsSamDayOrder { get; set; }
        
        /// <summary>
        /// Gets or sets SellerID.
        /// </summary>
        [DataMember(Name = "SellerID")]
        public string SellerID { get; set; }
	}
}