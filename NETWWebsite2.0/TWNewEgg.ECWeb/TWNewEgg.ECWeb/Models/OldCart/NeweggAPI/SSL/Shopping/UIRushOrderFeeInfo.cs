using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIRushOrderFeeInfo class.
    /// </summary>
    [DataContract]
    public class UIRushOrderFeeInfo
    {
        /// <summary>
        /// Gets or sets AdjustShippingTime.
        /// </summary>
        [DataMember(Name = "AdjustShippingTime")]
        public string AdjustShippingTime { get; set; }
        
        /// <summary>
        /// Gets or sets Fee.
        /// </summary>
        [DataMember(Name = "Fee")]
        public decimal Fee { get; set; }
        
        /// <summary>
        /// Gets or sets Guid.
        /// </summary>
        [DataMember(Name = "Guid")]
        public Guid Guid { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether IsFreeRushOrder.
        /// </summary>
        [DataMember(Name = "IsFreeRushOrder")]
        public bool IsFreeRushOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsChargedRushOrder.
        /// </summary>
        [DataMember(Name = "IsChargedRushOrder")]
        public bool IsChargedRushOrder { get; set; }

        /// <summary>
        /// Gets or sets FreeRushOrderText.
        /// </summary>
        [DataMember(Name = "FreeRushOrderText")]
        public string FreeRushOrderText { get; set; }

        /// <summary>
        /// Gets or sets ChargedRushOrderText.
        /// </summary>
        [DataMember(Name = "ChargedRushOrderText")]
        public string ChargedRushOrderText { get; set; }
    }
}