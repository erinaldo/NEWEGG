using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UINewRMAHistoryItemSummary class.
    /// </summary>
    [DataContract]
    public class UINewRMAHistoryItemSummary
    {
        /// <summary>
        /// Gets or sets Quantity.
        /// </summary>
        [DataMember(Name = "Quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets ItemCatalog.
        /// </summary>
        [DataMember(Name = "ItemCatalog")]
        public int ItemCatalog { get; set; }

        /// <summary>
        /// Gets or sets SellerId.
        /// </summary>
        [DataMember(Name = "SellerId")]
        public string SellerId { get; set; }

        /// <summary>
        /// Gets or sets SellerName.
        /// </summary>
        [DataMember(Name = "SellerName")]
        public string SellerName { get; set; }

        /// <summary>
        /// Gets or sets ItemNumber.
        /// </summary>
        [DataMember(Name = "ItemNumber")]
        public string ItemNumber { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        [DataMember(Name = "Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets ItemProperty.
        /// </summary>
        [DataMember(Name = "ItemProperty")]
        public string ItemProperty { get; set; }

        /// <summary>
        /// Gets or sets ItemImage.
        /// </summary>
        [DataMember(Name = "ItemImage")]
        public UIImageInfo ItemImage { get; set; }
    }
}
