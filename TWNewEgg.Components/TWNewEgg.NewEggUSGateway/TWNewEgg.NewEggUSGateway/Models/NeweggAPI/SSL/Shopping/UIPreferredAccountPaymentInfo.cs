using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIPreferredAccountPaymentInfo class.
    /// </summary>
    [DataContract]
    public class UIPreferredAccountPaymentInfo
    {
        /// <summary>
        /// Gets or sets a value indicating whether EnableNPA.
        /// </summary>
        [DataMember(Name = "EnableNPA")]
        public bool EnableNPA { get; set; }

        /// <summary>
        /// Gets or sets ErrorDescription.
        /// </summary>
        [DataMember(Name = "ErrorDescription")]
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Gets or sets NPADiscountRate.
        /// </summary>
        [DataMember(Name = "NPADiscountRate")]
        public decimal NPADiscountRate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether OnlyApplyNPADiscount.
        /// </summary>
        [DataMember(Name = "OnlyApplyNPADiscount")]
        public bool OnlyApplyNPADiscount { get; set; }

        /// <summary>
        /// Gets or sets PromotionInfo.
        /// </summary>
        [DataMember(Name = "PromotionInfo")]
        public UIPreferredAccountPromotionInfo PromotionInfo { get; set; }

        /// <summary>
        /// Gets or sets BillingTitle.
        /// </summary>
        [DataMember(Name = "BillingTitle")]
        public string BillingTitle { get; set; }

        /// <summary>
        /// Gets or sets BillingDescription.
        /// </summary>
        [DataMember(Name = "BillingDescription")]
        public string BillingDescription { get; set; }
    }
}
