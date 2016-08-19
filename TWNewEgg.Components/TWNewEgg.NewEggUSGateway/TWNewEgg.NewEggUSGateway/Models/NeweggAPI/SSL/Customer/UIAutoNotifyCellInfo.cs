using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIAutoNotifyCellInfo class.
    /// </summary>
    [DataContract]
    public class UIAutoNotifyCellInfo
    {
        /// <summary>
        /// Gets or sets TransactionNumber.
        /// </summary>
        [DataMember(Name = "TransactionNumber")]
        public string TransactionNumber { get; set; }

        /// <summary>
        /// Gets or sets ItemDescription.
        /// </summary>
        [DataMember(Name = "ItemDescription")]
        public string ItemDescription { get; set; }

        /// <summary>
        /// Gets or sets ItemNumber.
        /// </summary>
        [DataMember(Name = "ItemNumber")]
        public string ItemNumber { get; set; }

        /// <summary>
        /// Gets or sets RelatedItemNumber.
        /// </summary>
        [DataMember(Name = "RelatedItemNumber")]
        public string RelatedItemNumber { get; set; }

        /// <summary>
        /// Gets or sets RequestedDate.
        /// </summary>
        [DataMember(Name = "RequestedDate")]
        public string RequestedDate { get; set; }

        /// <summary>
        /// Gets or sets StockInfo.
        /// </summary>
        [DataMember(Name = "StockInfo")]
        public string StockInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether InStock.
        /// </summary>
        [DataMember(Name = "InStock")]
        public bool InStock { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsSellerItem.
        /// </summary>
        [DataMember(Name = "IsSellerItem")]
        public bool IsSellerItem { get; set; }

        /// <summary>
        /// Gets or sets SellerItemProperties.
        /// </summary>
        [DataMember(Name = "SellerItemProperties")]
        public string SellerItemProperties { get; set; }

        /// <summary>
        /// Gets or sets SellerLink.
        /// </summary>
        [DataMember(Name = "SellerLink")]
        public string SellerLink { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsCombo.
        /// </summary>
        [DataMember(Name = "IsCombo")]
        public bool IsCombo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsComboBundle.
        /// </summary>
        [DataMember(Name = "IsComboBundle")]
        public bool IsComboBundle { get; set; }

        /// <summary>
        /// Gets or sets ComboID.
        /// </summary>
        [DataMember(Name = "ComboID")]
        public string ComboID { get; set; }

        /// <summary>
        /// Gets or sets ListImage.
        /// </summary>
        [DataMember(Name = "ListImage")]
        public string ListImage { get; set; }

        /// <summary>
        /// Gets or sets ItemImage.
        /// </summary>
        [DataMember(Name = "ItemImage")]
        public UIImageInfo ItemImage { get; set; }
    }
}
