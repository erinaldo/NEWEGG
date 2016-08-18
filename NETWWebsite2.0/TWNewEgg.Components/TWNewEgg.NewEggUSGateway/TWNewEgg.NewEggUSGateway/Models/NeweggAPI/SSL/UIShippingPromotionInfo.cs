using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIShippingPromotionInfo class.
    /// </summary>
    [DataContract]
    public class UIShippingPromotionInfo
    {
        /// <summary>
        /// Gets or sets ShippingPromotionText.
        /// </summary>
        [DataMember(Name = "ShippingPromotionText")]
        public string ShippingPromotionText { get; set; }

        /// <summary>
        /// Gets or sets ShippingPromotionLandingText.
        /// </summary>
        [DataMember(Name = "ShippingPromotionLandingText")]
        public string ShippingPromotionLandingText { get; set; }

        /// <summary>
        /// Gets or sets ShippingPromotionLandingPageURL.
        /// </summary>
        [DataMember(Name = "ShippingPromotionLandingPageURL")]
        public string ShippingPromotionLandingPageURL { get; set; }
    }
}
