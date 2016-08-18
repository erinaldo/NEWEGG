using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models.Shopping
{
    /// <summary>
    /// Initializes a new instance of the UIShoppingCartBuyItem class.
    /// </summary>
    [DataContract]
    public class UIShoppingCartBuyItem
    {
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
        /// Gets or sets Quantity.
        /// </summary>
        [DataMember(Name = "Quantity")]
        public int Quantity { get; set; }
    }
}
