using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIShoppingCartUnitInfo class.
    /// </summary>
    [DataContract]
    public class UIShoppingCartUnitInfo
    {
        /// <summary>
        /// Gets or sets MainItemNumber.
        /// </summary>
        [DataMember(Name = "MainItemNumber")]
        public string MainItemNumber { get; set; }

        /// <summary>
        /// Gets or sets RelatedItemNumber.
        /// </summary>
        [DataMember(Name = "RelatedItemNumber")]
        public string RelatedItemNumber { get; set; }

        /// <summary>
        /// Gets or sets Quantity.
        /// </summary>
        [DataMember(Name = "Quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets RelatedQuantity.
        /// </summary>
        [DataMember(Name = "RelatedQuantity")]
        public int RelatedQuantity { get; set; }

        /// <summary>
        /// Gets or sets ItemType.
        /// </summary>
        [DataMember(Name = "ItemType")]
        public int ItemType { get; set; }

        /// <summary>
        /// Gets or sets FinalPrice.
        /// </summary>
        [DataMember(Name = "FinalPrice")]
        public string FinalPrice { get; set; }

        /// <summary>
        /// Gets or sets ImageURL.
        /// </summary>
        [DataMember(Name = "ImageURL")]
        public string ImageURL { get; set; }

        /// <summary>
        /// Gets or sets ItemNumber.
        /// </summary>
        [DataMember(Name = "ItemNumber")]
        public string ItemNumber { get; set; }

        /// <summary>
        /// Gets or sets PreSelectItemNumber.
        /// </summary>
        [DataMember(Name = "PreSelectItemNumber")]
        public string PreSelectItemNumber { get; set; }

        /// <summary>
        /// Gets or sets SnetList.
        /// </summary>
        [DataMember(Name = "SnetList")]
        public List<UIShoppingCartSNETInfo> SnetList { get; set; }

        /// <summary>
        /// Gets or sets ItemMapPriceMarkType.
        /// </summary>
        [DataMember(Name = "ItemMapPriceMarkType")]
        public UIMapPriceMarkType ItemMapPriceMarkType { get; set; }
    }
}
