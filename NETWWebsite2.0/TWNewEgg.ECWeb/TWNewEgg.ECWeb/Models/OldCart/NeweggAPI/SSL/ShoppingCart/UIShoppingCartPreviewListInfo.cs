using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIShoppingCartPreviewListInfo class.
    /// </summary>
    [DataContract]
    public class UIShoppingCartPreviewListInfo : UIShippingMethodItemInfo
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
        /// Gets or sets ItemType.
        /// </summary>
        [DataMember(Name = "ItemType")]
        public int ItemType { get; set; }
    }
}
