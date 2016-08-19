using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIEmailItemInfo class.
    /// </summary>
    [DataContract]
    public class UIEmailItemInfo
    {
        /// <summary>
        /// Gets or sets ItemLineDescription.
        /// </summary>
        [DataMember(Name = "ItemLineDescription")]
        public string ItemLineDescription { get; set; }

        /// <summary>
        /// Gets or sets UnitPrice.
        /// </summary>
        [DataMember(Name = "UnitPrice")]
        public float UnitPrice { get; set; }

        /// <summary>
        /// Gets or sets FinalPrice.
        /// </summary>
        [DataMember(Name = "FinalPrice")]
        public float FinalPrice { get; set; }

        /// <summary>
        /// Gets or sets ImageUrl.
        /// </summary>
        [DataMember(Name = "ImageUrl")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets SellerInfo.
        /// </summary>
        [DataMember(Name = "SellerInfo")]
        public UISellerInfo SellerInfo { get; set; }
    }
}
