using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIProductPropertyInfo class.
    /// </summary>
    [DataContract]
    public class UIProductPropertyInfo
    {
        /// <summary>
        /// Gets or sets PropertyOptionGroups.
        /// </summary>
        [DataMember(Name = "PropertyOptionGroups")]
        public List<UIProductPropertyOptionGroupInfo> PropertyOptionGroups { get; set; }

        /// <summary>
        /// Gets or sets MapPropertyOptionGroups.
        /// </summary>
        [DataMember(Name = "MapPropertyOptionGroups")]
        public List<UIProductMapPropertyOptionInfo> MapPropertyOptionGroups { get; set; }

        /// <summary>
        /// Gets or sets SellerItemList.
        /// </summary>
        [DataMember(Name = "SellerItemList")]
        public List<UIProductListSellerItemInfo> SellerItemList { get; set; }

        /// <summary>
        /// Gets or sets AllSellerWarning.
        /// </summary>
        [DataMember(Name = "AllSellerWarning")]
        public string AllSellerWarning { get; set; }
    }
}