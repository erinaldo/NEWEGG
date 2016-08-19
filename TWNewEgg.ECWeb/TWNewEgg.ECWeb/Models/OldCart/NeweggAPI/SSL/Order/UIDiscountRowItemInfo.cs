using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIDiscountRowItemInfo class.
    /// </summary>
    public class UIDiscountRowItemInfo : UICheckoutDiscountRowInfo
    {
        /// <summary>
        /// Gets or sets Quantity.
        /// </summary>
        [DataMember(Name = "Quantity")]
        public string Quantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Desc.
        /// </summary>
        [DataMember(Name = "Desc")]
        public bool Desc { get; set; }

        /// <summary>
        /// Gets or sets Money.
        /// </summary>
        [DataMember(Name = "Money")]
        public string Money { get; set; }

        /// <summary>
        /// Gets or sets ItemNumber.
        /// </summary>
        [DataMember(Name = "ItemNumber")]
        public string ItemNumber { get; set; }

        /// <summary>
        /// Gets or sets GiftType.
        /// </summary>
        [DataMember(Name = "GiftType")]
        public UIGiftType GiftType { get; set; }
    }
}
