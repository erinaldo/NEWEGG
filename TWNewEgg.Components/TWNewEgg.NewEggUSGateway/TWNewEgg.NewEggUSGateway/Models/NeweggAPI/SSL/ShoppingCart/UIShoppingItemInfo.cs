using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIShoppingItemInfo class.
    /// </summary>
    [DataContract]
    public class UIShoppingItemInfo
    {
        /// <summary>
        /// Gets or sets ItemNumber.
        /// </summary>
        [DataMember(Name = "ItemNumber")]
        public string ItemNumber { get; set; }

        /// <summary>
        /// Gets or sets Title.
        /// </summary>
        [DataMember(Name = "Title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets PropertyValues.
        /// </summary>
        [DataMember(Name = "PropertyValues")]
        public string PropertyValues { get; set; }

        /// <summary>
        /// Gets or sets SellerInfo.
        /// </summary>
        [DataMember(Name = "SellerInfo")]
        public UISellerInfo SellerInfo { get; set; }

        /// <summary>
        /// Gets or sets ImageUrl.
        /// </summary>
        [DataMember(Name = "ImageUrl")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets ItemAttribute.
        /// </summary>
        [DataMember(Name = "ItemAttribute")]
        public int ItemAttribute { get; set; }

        /// <summary>
        /// Gets or sets WarningList.
        /// </summary>
        [DataMember(Name = "WarningList")]
        public List<UIWarningInfo> WarningList { get; set; }

        /// <summary>
        /// Gets or sets WarrantyGroupList.
        /// </summary>
        [DataMember(Name = "WarrantyGroupList")]
        public List<UIWarrantyGroupInfo> WarrantyGroupList { get; set; }

        /// <summary>
        /// Gets or sets DriveSaverList.
        /// </summary>
        [DataMember(Name = "DriveSaverList")]
        public List<UIWarrantyGroupInfo> DriveSaverList { get; set; }

        /// <summary>
        /// Gets or sets ItemStockInformation.
        /// </summary>
        [DataMember(Name = "ItemStockInformation")]
        public string ItemStockInformation { get; set; }

        /// <summary>
        /// Gets or sets PromotionalGiftCartInformation.
        /// </summary>
        [DataMember(Name = "PromotionalGiftCartInformation")]
        public string PromotionalGiftCartInformation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsFreeGift.
        /// </summary>
        [DataMember(Name = "IsFreeGift")]
        public bool IsFreeGift { get; set; }

        /// <summary>
        /// Gets or sets PreviewIndex.
        /// </summary>
        [DataMember(Name = "PreviewIndex")]
        public int PreviewIndex { get; set; }

        /// <summary>
        /// Gets or sets PreSelectComboId.
        /// </summary>
        [DataMember(Name = "PreSelectComboId")]
        public int PreSelectComboId { get; set; }

        /// <summary>
        /// Gets or sets ProhibitedInfo.
        /// </summary>
        [DataMember(Name = "ProhibitedInfo")]
        public UIProhibitedInfo ProhibitedInfo { get; set; }

        /// <summary>
        /// Gets or sets ItemImage.
        /// </summary>
        [DataMember(Name = "ItemImage")]
        public UIImageInfo ItemImage { get; set; }
    }
}
