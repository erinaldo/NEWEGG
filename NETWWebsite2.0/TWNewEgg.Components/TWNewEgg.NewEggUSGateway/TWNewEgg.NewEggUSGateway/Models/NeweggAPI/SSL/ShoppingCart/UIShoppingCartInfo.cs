using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIShoppingCartInfo class.
    /// </summary>
    [DataContract]
    public class UIShoppingCartInfo
    {
        /// <summary>
        /// Gets or sets ShoppingCartList.
        /// </summary>
        [DataMember(Name = "ShoppingCartList")]
        public List<UIShoppingCartListInfo> ShoppingCartList { get; set; }

        /// <summary>
        /// Gets or sets ShoppingCartPreviewList.
        /// </summary>
        [DataMember(Name = "ShoppingCartPreviewList")]
        public List<UIShoppingCartPreviewListInfo> ShoppingCartPreviewList { get; set; }

        /// <summary>
        /// Gets or sets OrderList.
        /// </summary>
        [DataMember(Name = "OrderList")]
        public List<UIShoppingOrderInfo> OrderList { get; set; }

        /// <summary>
        /// Gets or sets ShoppingCartUnitInfoList.
        /// </summary>
        [DataMember(Name = "ShoppingCartUnitInfoList")]
        public List<UIShoppingCartUnitInfo> ShoppingCartUnitInfoList { get; set; }

        /// <summary>
        /// Gets or sets TotalItemQty.
        /// </summary>
        [DataMember(Name = "TotalItemQty")]
        public int TotalItemQty { get; set; }

        /// <summary>
        /// Gets or sets SubTotal.
        /// </summary>
        [DataMember(Name = "SubTotal")]
        public string SubTotal { get; set; }

        /// <summary>
        /// Gets or sets GrandTotalAmount.
        /// </summary>
        [DataMember(Name = "GrandTotalAmount")]
        public string GrandTotalAmount { get; set; }

        /// <summary>
        /// Gets or sets TaxAmount.
        /// </summary>
        [DataMember(Name = "TaxAmount")]
        public string TaxAmount { get; set; }

        /// <summary>
        /// Gets or sets ShippingCharge.
        /// </summary>
        [DataMember(Name = "ShippingCharge")]
        public string ShippingCharge { get; set; }

        /// <summary>
        /// Gets or sets PromotionAmount.
        /// </summary>
        [DataMember(Name = "PromotionAmount")]
        public string PromotionAmount { get; set; }

        /// <summary>
        /// Gets or sets GiftCertificateRedeemAmount.
        /// </summary>
        [DataMember(Name = "GiftCertificateRedeemAmount")]
        public string GiftCertificateRedeemAmount { get; set; }

        /// <summary>
        /// Gets or sets ZipCode.
        /// </summary>
        [DataMember(Name = "ZipCode")]
        public string ZipCode { get; set; }

        /// <summary>
        /// Gets or sets ZipCodeRelateMessage.
        /// </summary>
        [DataMember(Name = "ZipCodeRelateMessage")]
        public string ZipCodeRelateMessage { get; set; }

        /// <summary>
        /// Gets or sets PromotionItemMessage.
        /// </summary>
        [DataMember(Name = "PromotionItemMessage")]
        public string PromotionItemMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsNeedShowShippingLayout.
        /// </summary>
        [DataMember(Name = "IsNeedShowShippingLayout")]
        public bool IsNeedShowShippingLayout { get; set; }

        /// <summary>
        /// Gets or sets ShoppingCartPromotionCodeInfoList.
        /// </summary>
        [DataMember(Name = "ShoppingCartPromotionCodeInfoList")]
        public List<UIShoppingCartPromotionCodeInfo> ShoppingCartPromotionCodeInfoList { get; set; }

        /// <summary>
        /// Gets or sets PCodeRelateMessage.
        /// </summary>
        [DataMember(Name = "PCodeRelateMessage")]
        public string PCodeRelateMessage { get; set; }

        /// <summary>
        /// Gets or sets GiftCertificateRedeemInfoList.
        /// </summary>
        [DataMember(Name = "GiftCertificateRedeemInfoList")]
        public List<UIGiftCertificateRedeemInfo> GiftCertificateRedeemInfoList { get; set; }

        /// <summary>
        /// Gets or sets WishList.
        /// </summary>
        [DataMember(Name = "WishList")]
        public List<UIWishListInfo> WishList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsShowPromotionCode.
        /// </summary>
        [DataMember(Name = "IsShowPromotionCode")]
        public bool IsShowPromotionCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsContainSecureCheckoutMAP.
        /// </summary>
        [DataMember(Name = "IsContainSecureCheckoutMAP")]
        public bool IsContainSecureCheckoutMAP { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether DisableCheckout.
        /// </summary>
        [DataMember(Name = "DisableCheckout")]
        public bool DisableCheckout { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsAddedToWishList.
        /// </summary>
        [DataMember(Name = "IsAddedToWishList")]
        public bool IsAddedToWishList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsAddedToWishListSuccess.
        /// </summary>
        [DataMember(Name = "IsAddedToWishListSuccess")]
        public bool IsAddedToWishListSuccess { get; set; }

        /// <summary>
        /// Gets or sets AddedToWishListMessage.
        /// </summary>
        [DataMember(Name = "AddedToWishListMessage")]
        public string AddedToWishListMessage { get; set; }

        /// <summary>
        /// Gets or sets CoreMetricsInfo.
        /// </summary>
        [DataMember(Name = "CoreMetricsInfo")]
        public UICoreMetricsInfo CoreMetricsInfo { get; set; }
    }
}
