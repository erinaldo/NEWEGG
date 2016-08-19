using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIRMAHistoryDetailItemBase class.
    /// </summary>
    [DataContract]
    public class UIRMAHistoryDetailItemBase
    {
        /// <summary>
        /// Gets or sets Quantity.
        /// </summary>
        [DataMember(Name = "Quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets ItemNumber.
        /// </summary>
        [DataMember(Name = "ItemNumber")]
        public string ItemNumber { get; set; }

        /// <summary>
        /// Gets or sets FinalPrice.
        /// </summary>
        [DataMember(Name = "FinalPrice")]
        public string FinalPrice { get; set; }
    }
}
