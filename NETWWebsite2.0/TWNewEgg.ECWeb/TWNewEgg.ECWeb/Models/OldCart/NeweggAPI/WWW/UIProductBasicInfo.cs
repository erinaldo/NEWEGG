using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIProductBasicInfo class.
    /// </summary>
    [DataContract]
    public class UIProductBasicInfo
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
        /// Gets or sets OriginalPrice.
        /// </summary>
        [DataMember(Name = "OriginalPrice")]
        public string OriginalPrice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsShowOriginalPrice.
        /// </summary>
        [DataMember(Name = "IsShowOriginalPrice")]
        public bool IsShowOriginalPrice { get; set; }

        /// <summary>
        /// Gets or sets FinalPrice.
        /// </summary>
        [DataMember(Name = "FinalPrice")]
        public string FinalPrice { get; set; }

        /// <summary>
        /// Gets or sets ItemImage.
        /// </summary>
        [DataMember(Name = "ItemImage")]
        public UIImageInfo ItemImage { get; set; }

        /// <summary>
        /// Gets or sets ItemBrand.
        /// </summary>
        [DataMember(Name = "ItemBrand")]
        public UIBrandInfo ItemBrand { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Instock.
        /// </summary>
        [DataMember(Name = "Instock")]
        public bool Instock { get; set; }

        /// <summary>
        /// Gets or sets AfterRebate.
        /// </summary>
        [DataMember(Name = "AfterRebate")]
        public string AfterRebate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsFreeShipping.
        /// </summary>
        [DataMember(Name = "IsFreeShipping")]
        public bool IsFreeShipping { get; set; }

        /// <summary>
        /// Gets or sets ReviewSummary.
        /// </summary>
        [DataMember(Name = "ReviewSummary")]
        public UIReviewSummary ReviewSummary { get; set; }

        /// <summary>
        /// Gets or sets MappingFinalPrice.
        /// </summary>
        [DataMember(Name = "MappingFinalPrice")]
        public string MappingFinalPrice { get; set; }

        /// <summary>
        /// Gets or sets ItemMapPriceMarkType.
        /// </summary>
        [DataMember(Name = "ItemMapPriceMarkType")]
        public UIMapPriceMarkType ItemMapPriceMarkType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsMicrosoftDownloadItem.
        /// </summary>
        [DataMember(Name = "IsMicrosoftDownloadItem")]
        public bool IsMicrosoftDownloadItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsProductKeyOnly.
        /// </summary>
        [DataMember(Name = "IsProductKeyOnly")]
        public bool IsProductKeyOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsHot.
        /// </summary>
        [DataMember(Name = "IsHot")]
        public bool IsHot { get; set; }

        /// <summary>
        ///  Gets or sets ItemImages.
        /// </summary>
        [DataMember(Name = "ItemImages")]
        public List<UIImageInfo> ItemImages { get; set; }

        /// <summary>
        ///  Gets or sets ItemType.
        /// </summary>
        [DataMember(Name = "ItemType")]
        public VProductItemType ItemType { get; set; }

        /// <summary>
        ///  Gets or sets NeweggItemNumber.
        /// </summary>
        [DataMember(Name = "NeweggItemNumber")]
        public string NeweggItemNumber { get; set; }

        /// <summary>
        ///  Gets or sets PromotionText.
        /// </summary>
        [DataMember(Name = "PromotionText")]
        public string PromotionText { get; set; }

        /// <summary>
        /// Gets or sets RebateText.
        /// </summary>
        [DataMember(Name = "RebateText")]
        public string RebateText { get; set; }

        /// <summary>
        /// Gets or sets SavingText.
        /// </summary>
        [DataMember(Name = "SavingText")]
        public string SavingText { get; set; }

        /// <summary>
        /// Gets or sets SellerInfo.
        /// </summary>
        [DataMember(Name = "SellerInfo")]
        public VSellerInfo SellerInfo { get; set; }

        /// <summary>
        /// Gets or sets ShippingText.
        /// </summary>
        [DataMember(Name = "ShippingText")]
        public string ShippingText { get; set; }

        /// <summary>
        /// Gets or sets StoreNavigation.
        /// </summary>
        [DataMember(Name = "StoreNavigation")]
        public NavigationItemList StoreNavigation { get; set; }
    }
}
