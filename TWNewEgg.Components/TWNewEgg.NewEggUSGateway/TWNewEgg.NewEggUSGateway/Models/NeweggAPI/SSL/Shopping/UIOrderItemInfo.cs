using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIOrderItemInfo class.
    /// </summary>
    [DataContract]
    public class UIOrderItemInfo
    {
        /// <summary>
        /// Initializes a new instance of the UIOrderItemInfo class.
        /// </summary>
        public UIOrderItemInfo()
        {
            this.ItemNumber = string.Empty;
            this.ImageMiddlingUrl = string.Empty;
            this.ImageUrl = string.Empty;
            this.Note = string.Empty;
            this.ExtendPriceInfo = string.Empty;
            this.PropertyValues = string.Empty;
            this.NeweggItemNumber = string.Empty;
            this.ItemPriceAdj = string.Empty;
            this.UnitPriceInfo = string.Empty;
            this.TotalPriceInfo = string.Empty;
        }

        /// <summary>
        /// Gets or sets ItemNumber.
        /// </summary>
        [DataMember(Name = "ItemNumber")]
        public string ItemNumber { get; set; }

        /// <summary>
        /// Gets or sets NeweggItemNumber.
        /// </summary>
        [DataMember(Name = "NeweggItemNumber")]
        public string NeweggItemNumber { get; set; }

        /// <summary>
        /// Gets or sets Title.
        /// </summary>
        [DataMember(Name = "Title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets Note.
        /// </summary>
        [DataMember(Name = "Note")]
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets ExtendPrice.
        /// </summary>
        [DataMember(Name = "ExtendPrice")]
        public decimal ExtendPrice { get; set; }

        /// <summary>
        /// Gets or sets Quantity.
        /// </summary>
        [DataMember(Name = "Quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets ExtendPriceInfo.
        /// </summary>
        [DataMember(Name = "ExtendPriceInfo")]
        public string ExtendPriceInfo { get; set; }

        /// <summary>
        /// Gets or sets UnitPrice.
        /// </summary>
        [DataMember(Name = "UnitPrice")]
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Gets or sets UnitPriceInfo.
        /// </summary>
        [DataMember(Name = "UnitPriceInfo")]
        public string UnitPriceInfo { get; set; }

        /// <summary>
        /// Gets or sets TotalPriceInfo.
        /// </summary>
        [DataMember(Name = "TotalPriceInfo")]
        public string TotalPriceInfo { get; set; }

        /// <summary>
        /// Gets or sets ImageUrl.
        /// </summary>
        [DataMember(Name = "ImageUrl")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets ImageMiddlingUrl.
        /// </summary>
        [DataMember(Name = "ImageMiddlingUrl")]
        public string ImageMiddlingUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether EditEnabled.
        /// </summary>
        [DataMember(Name = "EditEnabled")]
        public bool EditEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsShowDetail.
        /// </summary>
        [DataMember(Name = "IsShowDetail")]
        public bool IsShowDetail { get; set; }

        /// <summary>
        /// Gets or sets ShippingPromotionInfo.
        /// </summary>
        [DataMember(Name = "ShippingPromotionInfo")]
        public UIShippingPromotionInfo ShippingPromotionInfo { get; set; }

        /// <summary>
        /// Gets or sets SellerInfo.
        /// </summary>
        [DataMember(Name = "SellerInfo")]
        public UISellerInfo SellerInfo { get; set; }

        /// <summary>
        /// Gets or sets ItemGroupType.
        /// </summary>
        [DataMember(Name = "ItemGroupType")]
        public UIItemGroupType ItemGroupType { get; set; }

        /// <summary>
        /// Gets or sets ComboId.
        /// </summary>
        [DataMember(Name = "ComboId")]
        public int ComboId { get; set; }

        /// <summary>
        /// Gets or sets PropertyValues.
        /// </summary>
        [DataMember(Name = "PropertyValues")]
        public string PropertyValues { get; set; }

        /// <summary>
        /// Gets or sets PreSelectComboId.
        /// </summary>
        [DataMember(Name = "PreSelectComboId")]
        public int PreSelectComboId { get; set; }

        /// <summary>
        /// Gets or sets OrderItemType.
        /// </summary>
        [DataMember(Name = "OrderItemType")]
        public UIOrderItemType OrderItemType { get; set; }

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

        /// <summary>
        /// Gets or sets ReturnPolicy.
        /// </summary>
        [DataMember(Name = "ReturnPolicy")]
        public UIWarningInfo ReturnPolicy { get; set; }

        /// <summary>
        /// Gets or sets ItemPriceAdj.
        /// </summary>
        [DataMember(Name = "ItemPriceAdj")]
        public string ItemPriceAdj { get; set; }

        /// <summary>
        /// Gets or sets SellerExtraInfo.
        /// </summary>
        [DataMember(Name = "SellerExtraInfo")]
        public UIOrderInvoicSellerExtraInfo SellerExtraInfo { get; set; }

        /// <summary>
        /// Gets or sets ComboMIRInfo.
        /// </summary>
        [DataMember(Name = "ComboMIRInfo")]
        public List<string> ComboMIRInfo { get; set; }

        /// <summary>
        /// Gets or sets SecurityCode.
        /// </summary>
        [DataMember(Name = "SecurityCode")]
        public List<string> SecurityCode { get; set; } ////Modify string[] to List<string>

        /// <summary>
        /// Gets or sets a value indicating whether CanWriteReview.
        /// </summary>
        [DataMember(Name = "CanWriteReview")]
        public bool CanWriteReview { get; set; }

        /// <summary>
        /// Gets or sets Notes.
        /// </summary>
        [DataMember(Name = "Notes")]
        public List<UINoteItemInfo> Notes { get; set; }

        /// <summary>
        /// Gets or sets GiftType.
        /// </summary>
        [DataMember(Name = "GiftType")]
        public UIGiftType GiftType { get; set; }
    }
}