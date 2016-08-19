using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIOrderDefaultShippingMethodInfo class.
    /// </summary>
    [DataContract]
    public class UIOrderDefaultShippingMethodInfo
    {
        /// <summary>
        /// Gets or sets OrderItemType.
        /// </summary>
        [DataMember(Name = "OrderItemType")]
        public int OrderItemType { get; set; }

        /// <summary>
        /// Gets or sets DefaultShippingMethod.
        /// </summary>
        [DataMember(Name = "DefaultShippingMethod")]
        public string DefaultShippingMethod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsSamDayOrder.
        /// </summary>
        [DataMember(Name = "IsSamDayOrder")]
        public bool IsSamDayOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsMaunalMehtod.
        /// </summary>
        [DataMember(Name = "IsMaunalMehtod")]
        public bool IsMaunalMehtod { get; set; }

        /// <summary>
        /// Gets or sets SellerID.
        /// </summary>
        [DataMember(Name = "SellerID")]
        public string SellerID { get; set; }

        /// <summary>
        /// Gets or sets OrderType.
        /// </summary>
        [DataMember(Name = "OrderType")]
        public int OrderType { get; set; }
    }
}
