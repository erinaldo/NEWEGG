using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIProductItem class.
    /// </summary>
    [DataContract]
    public class UIProductItem
    {
        /// <summary>
        /// Gets or sets ItemNumber.
        /// </summary>
        [DataMember(Name = "ItemNumber")]
        public string ItemNumber { get; set; }

        /// <summary>
        /// Gets or sets Quantity.
        /// </summary>
        [DataMember(Name = "Quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsNPAPromotionChecked.
        /// </summary>
        [DataMember(Name = "IsNPAPromotionChecked")]
        public bool IsNPAPromotionChecked { get; set; }
    }
}
