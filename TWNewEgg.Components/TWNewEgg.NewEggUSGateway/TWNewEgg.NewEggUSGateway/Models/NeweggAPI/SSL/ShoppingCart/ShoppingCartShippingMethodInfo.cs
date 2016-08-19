using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIOrderDefaultShippingMethodInfo class.
    /// </summary>
    [DataContract]
    public class ShoppingCartShippingMethodInfo
    {
        /// <summary>
        /// Gets or sets OrderItemType.
        /// </summary>
        [DataMember(Name = "oit")]
        public int OrderItemType { get; set; }

        /// <summary>
        /// Gets or sets DefaultShippingMethod.
        /// </summary>
        [DataMember(Name = "sm")]
        public string DefaultShippingMethod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsSamDayOrder.
        /// </summary>
        [DataMember(Name = "sd")]
        public bool IsSamDayOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsMaunalMehtod.
        /// </summary>
        [DataMember(Name = "mm")]
        public bool IsMaunalMehtod { get; set; }

        /// <summary>
        /// Gets or sets SellerID.
        /// </summary>
        [DataMember(Name = "si")]
        public string SellerID { get; set; }

        /// <summary>
        /// Gets or sets OrderType.
        /// </summary>
        [DataMember(Name = "ot")]
        public int OrderType { get; set; }

        /// <summary>
        /// Gets or sets OrderIndex.
        /// </summary>
        [DataMember(Name = "oid")]
        public int OrderIndex { get; set; }
    }
}
