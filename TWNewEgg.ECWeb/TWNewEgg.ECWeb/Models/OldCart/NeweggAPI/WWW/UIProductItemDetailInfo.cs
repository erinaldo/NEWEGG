using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UI
    /// DetailInfo class.
    /// </summary>
    [DataContract]
    public class UIProductItemDetailInfo : UIProductListItemInfo
    {
        /// <summary>
        /// Gets or sets ReturnPolicyInfo.
        /// </summary>
        [DataMember(Name = "ReturnPolicyInfo")]
        public UIReturnPolicyInfo ReturnPolicyInfo { get; set; }

        /// <summary>
        /// Gets or sets MailInRebateInfo.
        /// </summary>
        [DataMember(Name = "MailInRebateInfo")]
        public List<string> MailInRebateInfo { get; set; } ////Change string[] to List<string> by Casper.

        /// <summary>
        /// Gets or sets imageGallery.
        /// </summary>
        [DataMember(Name = "imageGallery")]
        public List<UIImageInfo> ImageGallery { get; set; }

        /// <summary>
        /// Gets or sets ShippingInfo.
        /// </summary>
        [DataMember(Name = "ShippingInfo")]
        public UIShippingInfo ShippingInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsShowSoldOutText.
        /// </summary>
        [DataMember(Name = "IsShowSoldOutText)]")]
        public bool IsShowSoldOutText { get; set; }

        /// <summary>
        /// Gets or sets AddToCartButtonText.
        /// </summary>
        [DataMember(Name = "AddToCartButtonText")]
        public string AddToCartButtonText { get; set; }

        /// <summary>
        /// Gets or sets AddToCartButtonType.
        /// </summary>
        [DataMember(Name = "AddToCartButtonType")]
        public string AddToCartButtonType { get; set; }

        /// <summary>
        /// Gets or sets CoremetricsInfo.
        /// </summary>
        [DataMember(Name = "CoremetricsInfo")]
        public CoremetricsInfo CoremetricsInfo { get; set; }

        /// <summary>
        /// Gets or sets EmailFriendImageInfo.
        /// </summary>
        [DataMember(Name = "EmailFriendImageInfo")]
        public UIImageInfo EmailFriendImageInfo { get; set; }

        /// <summary>
        /// Gets or sets ShippingPromotionInfo.
        /// </summary>
        [DataMember(Name = "ShippingPromotionInfo")]
        public UIShippingPromotionInfo ShippingPromotionInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsActivated.
        /// </summary>
        [DataMember(Name = "IsActivated")]
        public bool IsActivated { get; set; }

        /// <summary>
        /// Gets or sets SellerItemPropertyList.
        /// </summary>
        [DataMember(Name = "SellerItemPropertyList")]
        public List<UISellerItemPropertyInfo> SellerItemPropertyList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsShowEnergyStarSection.
        /// 是否显示能源之星片段信息.
        /// </summary>
        [DataMember(Name = "IsShowEnergyStarSection")]
        public bool IsShowEnergyStarSection { get; set; }

        /// <summary>
        /// Gets or sets EnergyStarText.
        /// </summary>
        [DataMember(Name = "EnergyStarText")]
        public string EnergyStarText { get; set; }

        /// <summary>
        /// Gets or sets IronEggDescription.
        /// </summary>
        [DataMember(Name = "IronEggDescription")]
        public string IronEggDescription { get; set; }

        /// <summary>
        /// Gets or sets ComboCount.
        /// </summary>
        [DataMember(Name = "ComboCount")]
        public int ComboCount { get; set; }

        /// <summary>
        /// Gets or sets SubCategoryId.
        /// </summary>
        [DataMember(Name = "SubCategoryId")]
        public int SubCategoryId { get; set; }

        /// <summary>
        /// Gets or sets SubCategoryName.
        /// </summary>
        [DataMember(Name = "SubCategoryName")]
        public string SubCategoryName { get; set; }

        /// <summary>
        /// Gets or sets Warnings.
        /// </summary>
        [DataMember(Name = "Warnings")]
        public List<UIProductWarningInfo> Warnings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsShipByNewegg.
        /// </summary>
        [DataMember(Name = "IsShipByNewegg")]
        public bool IsShipByNewegg { get; set; }

        /// <summary>
        /// Gets or sets ShoppingInsight.
        /// </summary>
        [DataMember(Name = "ShoppingInsight")]
        public UIPromotionModuleInfo ShoppingInsight { get; set; }

        /// <summary>
        /// Gets or sets ProductProperties.
        /// </summary>
        [DataMember(Name = "ProductProperties")]
        public UIProductPropertyInfo ProductProperties { get; set; }

        /// <summary>
        /// Gets or sets EduHelpInfo.
        /// </summary>
        [DataMember(Name = "EduHelpInfo")]
        public UIHelpInfo EduHelpInfo { get; set; }
    }
}