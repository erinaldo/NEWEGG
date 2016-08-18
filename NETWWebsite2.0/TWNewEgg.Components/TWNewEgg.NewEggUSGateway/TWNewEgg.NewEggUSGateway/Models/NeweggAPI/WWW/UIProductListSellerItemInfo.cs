using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIProductListSellerItemInfo class.
    /// </summary>
    [DataContract]
    public class UIProductListSellerItemInfo : UIProductListItemInfo
    {
        /// <summary>
        /// Gets or sets ShippingDescription.
        /// </summary>
        [DataMember(Name = "ShippingDescription")]
        public string ShippingDescription { get; set; }

        /// <summary>
        /// Gets or sets VolumeDiscountInfo.
        /// </summary>
        [DataMember(Name = "VolumeDiscountInfo")]
        public List<UIVolumeDiscountInfo> VolumeDiscountInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsFeaturedMerchants.
        /// </summary>
        [DataMember(Name = "IsFeaturedMerchants")]
        public bool IsFeaturedMerchants { get; set; }

        /// <summary>
        /// Gets or sets SellerLogoUrl.
        /// </summary>
        [DataMember(Name = "SellerLogoUrl")]
        public string SellerLogoUrl { get; set; }
    }
}
