using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIShoppingOrderInfo class.
    /// </summary>
    [DataContract]
    public class UIShoppingOrderInfo
    {
        /// <summary>
        /// Gets or sets ShippingMethodItemList.
        /// </summary>
        [DataMember(Name = "ShippingMethodItemList")]
        public List<UIShippingMethodItemInfo> ShippingMethodItemList { get; set; }

        /// <summary>
        /// Gets or sets ShippingMethodList.
        /// </summary>
        [DataMember(Name = "ShippingMethodList")]
        public List<UIShippingMethodInfo> ShippingMethodList { get; set; }

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
        /// Gets or sets a value indicating whether IsSameDayOrder.
        /// </summary>
        [DataMember(Name = "IsSameDayOrder")]
        public bool IsSameDayOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsMaunalMethod.
        /// </summary>
        [DataMember(Name = "IsMaunalMethod")]
        public bool IsMaunalMethod { get; set; }

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
