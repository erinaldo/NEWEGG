using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UICheckoutDataInfo class.
    /// </summary>
    [DataContract]
    public class UICheckoutDataInfo
    {
        /// <summary>
        /// Gets or sets itemList.
        /// </summary>
        [DataMember(Name = "ItemList")]
        public List<CheckoutItemInfo> ItemList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Is Selected Rush.
        /// </summary>
        [DataMember(Name = "IsSelectedRush")]
        public bool IsSelectedRush { get; set; }

        /// <summary>
        /// Gets or sets CustomerNumber.
        /// </summary>
        [DataMember(Name = "CustomerNumber")]
        public int CustomerNumber { get; set; }
    }
}
