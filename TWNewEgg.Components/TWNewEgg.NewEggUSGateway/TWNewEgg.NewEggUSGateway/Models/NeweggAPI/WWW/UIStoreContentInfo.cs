using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIStoreContentInfo class.
    /// </summary>
    [DataContract]
    public class UIStoreContentInfo
    {
        /// <summary>
        /// Gets or sets ProductItemList.
        /// </summary>
        [DataMember(Name = "ProductItemList")]
        public List<UIProductListItemInfo> ProductItemList { get; set; }

        /// <summary>
        /// Gets or sets PageInfo.
        /// </summary>
        [DataMember(Name = "PageInfo")]
        public UIPageInfo PageInfo { get; set; }

        /// <summary>
        /// Gets or sets PageNumberList.
        /// </summary>
        [DataMember(Name = "PageNumberList")]
        public List<int> PageNumberList { get; set; }

        /// <summary>
        /// Gets or sets CoremetricsInfo.
        /// </summary>
        [DataMember(Name = "CoremetricsInfo")]
        public CoremetricsInfo CoremetricsInfo { get; set; }
    }
}
