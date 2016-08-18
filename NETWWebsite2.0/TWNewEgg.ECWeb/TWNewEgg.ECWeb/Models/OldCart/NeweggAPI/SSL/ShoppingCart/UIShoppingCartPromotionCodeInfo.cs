using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIShoppingCartPromotionCodeInfo class.
    /// </summary>
    [DataContract]
    public class UIShoppingCartPromotionCodeInfo
    {
        /// <summary>
        /// Gets or sets PromotionCode.
        /// </summary>
        [DataMember(Name = "PromotionCode")]
        public string PromotionCode { get; set; }

        /// <summary>
        /// Gets or sets PromotionType.
        /// </summary>
        [DataMember(Name = "PromotionType")]
        public string PromotionType { get; set; }

        /// <summary>
        /// Gets or sets Discount.
        /// </summary>
        [DataMember(Name = "Discount")]
        public string Discount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether HidePromoCodeDiscount.
        /// </summary>
        [DataMember(Name = "HidePromoCodeDiscount")]
        public bool HidePromoCodeDiscount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsSuccessfulApplied.
        /// </summary>
        [DataMember(Name = "IsSuccessfulApplied")]
        public bool IsSuccessfulApplied { get; set; }
    }
}
