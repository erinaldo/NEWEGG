using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIWishListProductInfo class.
    /// </summary>
    [DataContract]
    public class UIWishListProductInfo
	{
        /// <summary>
        /// Gets or sets ItemNumber.
        /// </summary>
        [DataMember(Name = "ItemNumber")]
        public string ItemNumber { get; set; }

        /// <summary>
        /// Gets or sets qty.
        /// </summary>
        [DataMember(Name = "Qty")]
        public int Qty { get; set; }

        /// <summary>
        /// Gets or sets ImageUrl.
        /// </summary>
        [DataMember(Name = "ImageUrl")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets ItemTitle.
        /// </summary>
        [DataMember(Name = "ItemTitle")]
        public string ItemTitle { get; set; }

        /// <summary>
        /// Gets or sets ItemModel.
        /// </summary>
        [DataMember(Name = "ItemModel")]
        public string ItemModel { get; set; }

        /// <summary>
        /// Gets or sets UnitPrice.
        /// </summary>
        [DataMember(Name = "UnitPrice")]
        public string UnitPrice { get; set; }

        /// <summary>
        /// Gets or sets UnitFinalPrice.
        /// </summary>
        [DataMember(Name = "UnitFinalPrice")]
        public string UnitFinalPrice { get; set; }

        /// <summary>
        /// Gets or sets VolumeUnitPrice.
        /// </summary>
        [DataMember(Name = "VolumeUnitPrice")]
        public string VolumeUnitPrice { get; set; }

        /// <summary>
        /// Gets or sets DiscountPrice.
        /// </summary>
        [DataMember(Name = "DiscountPrice")]
        public string DiscountPrice { get; set; }

        /// <summary>
        /// Gets or sets InstantUnitPrice.
        /// </summary>
        [DataMember(Name = "InstantUnitPrice")]
        public string InstantUnitPrice { get; set; }

        /// <summary>
        /// Gets or sets FinalPrice.
        /// </summary>
        [DataMember(Name = "FinalPrice")]
        public string FinalPrice { get; set; }

        /// <summary>
        /// Gets or sets StockStatus.
        /// </summary>
        [DataMember(Name = "StockStatus")]
        public string StockStatus { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsOutOfStock.
        /// </summary>
        [DataMember(Name = "IsOutOfStock")]
        public bool IsOutOfStock { get; set; }

        /// <summary>
        /// Gets or sets SellerInfo.
        /// </summary>
        [DataMember(Name = "SellerInfo")]
        public UISellerInfo SellerInfo { get; set; }

        /// <summary>
        /// Gets or sets PropertyValues.
        /// </summary>
        [DataMember(Name = "PropertyValues")]
        public string PropertyValues { get; set; }
		
        /// <summary>
        /// Gets or sets VolumeDiscountInfo.
        /// </summary>
        [DataMember(Name = "VolumeDiscountInfo")]
        public List<UIVolumeDiscountInfo> VolumeDiscountInfo { get; set; }

        /// <summary>
        /// Gets or sets ItemMapPriceMarkType.
        /// </summary>
        [DataMember(Name = "ItemMapPriceMarkType")]
        public UIMapPriceMarkType ItemMapPriceMarkType { get; set; }

        /// <summary>
        /// Gets or sets MappingFinalPrice.
        /// </summary>
        [DataMember(Name = "MappingFinalPrice")]
        public string MappingFinalPrice { get; set; }

        /// <summary>
        /// Gets or sets ItemImage.
        /// </summary>
        [DataMember(Name = "ItemImage")]
        public UIImageInfo ItemImage { get; set; }
	}
}
