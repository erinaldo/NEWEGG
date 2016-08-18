using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIShoppingCartSNETInfo class.
    /// </summary>
    [DataContract]
    public class UIShoppingCartSNETInfo
    {
        /// <summary>
        /// Gets or sets ItemNumber.
        /// </summary>
        [DataMember(Name = "ItemNumber")]
        public string ItemNumber { get; set; }

        /// <summary>
        /// Gets or sets SnetNumber.
        /// </summary>
        [DataMember(Name = "SnetNumber")]
        public string SnetNumber { get; set; }

        /// <summary>
        /// Gets or sets Quantity.
        /// </summary>
        [DataMember(Name = "Quantity")]
        public int Quantity { get; set; }
    }
}
