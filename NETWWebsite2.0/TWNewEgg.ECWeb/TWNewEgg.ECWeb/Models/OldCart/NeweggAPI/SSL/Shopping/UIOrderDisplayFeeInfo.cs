using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIOrderDisplayFeeInfo class.
    /// </summary>
    [DataContract]
    public class UIOrderDisplayFeeInfo
	{
        /// <summary>
        /// Gets or sets EwraTotal.
        /// </summary>
        [DataMember(Name = "EwraTotal")]
        public string EwraTotal { get; set; }
		
        /// <summary>
        /// Gets or sets WarrantyTotal.
        /// </summary>
        [DataMember(Name = "WarrantyTotal")]
        public string WarrantyTotal { get; set; }
		
        /// <summary>
        /// Gets or sets ItemTotal.
        /// </summary>
        [DataMember(Name = "ItemTotal")]
        public string ItemTotal { get; set; }
		
        /// <summary>
        /// Gets or sets ShippingMethodDescription.
        /// </summary>
        [DataMember(Name = "ShippingMethodDescription")]
        public string ShippingMethodDescription { get; set; }
		
        /// <summary>
        /// Gets or sets ShippingCharge.
        /// </summary>
        [DataMember(Name = "ShippingCharge")]
        public string ShippingCharge { get; set; }
		
        /// <summary>
        /// Gets or sets GCTotal.
        /// </summary>
        [DataMember(Name = "GCTotal")]
        public string GCTotal { get; set; }
		
        /// <summary>
        /// Gets or sets DiscountTotal.
        /// </summary>
        [DataMember(Name = "DiscountTotal")]
        public string DiscountTotal { get; set; }
		
        /// <summary>
        /// Gets or sets tax.
        /// </summary>
        [DataMember(Name = "Tax")]
        public string Tax { get; set; }
		
        /// <summary>
        /// Gets or sets RushOrderFee.
        /// </summary>
        [DataMember(Name = "RushOrderFee")]
        public string RushOrderFee { get; set; }
		
        /// <summary>
        /// Gets or sets RushProcessingPA.
        /// </summary>
        [DataMember(Name = "RushProcessingPA")]
        public string RushProcessingPA { get; set; }
		
        /// <summary>
        /// Gets or sets NPADiscountRate.
        /// </summary>
        [DataMember(Name = "NPADiscountRate")]
        public string NPADiscountRate { get; set; }
		
        /// <summary>
        /// Gets or sets NPADiscount.
        /// </summary>
        [DataMember(Name = "NPADiscount")]
        public string NPADiscount { get; set; }
		
        /// <summary>
        /// Gets or sets Total.
        /// </summary>
        [DataMember(Name = "Total")]
        public string Total { get; set; }
		
        /// <summary>
        /// Gets or sets PromotionCodeDiscount.
        /// </summary>
        [DataMember(Name = "PromotionCodeDiscount")]
        public string PromotionCodeDiscount { get; set; }
        
        /// <summary>
        /// Gets or sets OrderAdjustment.
        /// </summary>
        [DataMember(Name = "OrderAdjustment")]
        public string OrderAdjustment { get; set; }
        
        /// <summary>
        /// Gets or sets ShippingAdjustment.
        /// </summary>
        [DataMember(Name = "ShippingAdjustment")]
        public string ShippingAdjustment { get; set; }
	}
}
