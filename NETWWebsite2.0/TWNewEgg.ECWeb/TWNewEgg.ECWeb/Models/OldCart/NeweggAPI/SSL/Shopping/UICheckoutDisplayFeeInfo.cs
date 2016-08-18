using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UICheckoutDisplayFeeInfo class.
    /// </summary>
    [DataContract]
    public class UICheckoutDisplayFeeInfo
	{
        /// <summary>
        /// Gets or sets SubTotalAmount.
        /// </summary>
        [DataMember(Name = "SubTotalAmount")]
        public string SubTotalAmount { get; set; }
		
        /// <summary>
        /// Gets or sets TaxTotalAmount.
        /// </summary>
        [DataMember(Name = "TaxTotalAmount")]
        public string TaxTotalAmount { get; set; }
		
        /// <summary>
        /// Gets or sets ShippingChargeTotalAmount.
        /// </summary>
        [DataMember(Name = "ShippingChargeTotalAmount")]
        public string ShippingChargeTotalAmount { get; set; }
		
        /// <summary>
        /// Gets or sets EWTTotalAmount.
        /// </summary>
        [DataMember(Name = "EWTTotalAmount")]
        public string EWTTotalAmount { get; set; }
		
        /// <summary>
        /// Gets or sets RushOrderAmount.
        /// </summary>
        [DataMember(Name = "RushOrderAmount")]
        public string RushOrderAmount { get; set; }
		
        /// <summary>
        /// Gets or sets NPARushOrderValue.
        /// </summary>
        [DataMember(Name = "NPARushOrderValue")]
        public string NPARushOrderValue { get; set; }
		
        /// <summary>
        /// Gets or sets EWRATotalAmount.
        /// </summary>
        [DataMember(Name = "EWRATotalAmount")]
        public string EWRATotalAmount { get; set; }
		
        /// <summary>
        /// Gets or sets HandlingFee.
        /// </summary>
        [DataMember(Name = "HandlingFee")]
        public string HandlingFee { get; set; }
		
        /// <summary>
        /// Gets or sets GstHstFee.
        /// </summary>
        [DataMember(Name = "GstHstFee")]
        public string GstHstFee { get; set; }
		
        /// <summary>
        /// Gets or sets PSTFee.
        /// </summary>
        [DataMember(Name = "PSTFee")]
        public string PSTFee { get; set; }
		
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
        /// Gets or sets GiftCertificateRedeemTotalAmount.
        /// </summary>
        [DataMember(Name = "GiftCertificateRedeemTotalAmount")]
        public string GiftCertificateRedeemTotalAmount { get; set; }
		
        /// <summary>
        /// Gets or sets PromotionCodeAmount.
        /// </summary>
        [DataMember(Name = "PromotionCodeAmount")]
        public string PromotionCodeAmount { get; set; }
		
        /// <summary>
        /// Gets or sets TotalAmount.
        /// </summary>
        [DataMember(Name = "TotalAmount")]
        public string TotalAmount { get; set; }
		
        /// <summary>
        /// Gets or sets TotalAmountDescription.
        /// </summary>
        [DataMember(Name = "TotalAmountDescription")]
        public string TotalAmountDescription { get; set; }
	}
}
