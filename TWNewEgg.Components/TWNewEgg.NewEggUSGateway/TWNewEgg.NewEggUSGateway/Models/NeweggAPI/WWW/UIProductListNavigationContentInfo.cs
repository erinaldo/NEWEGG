using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIProductListNavigationContentInfo class.
    /// </summary>
    [DataContract]
    public class UIProductListNavigationContentInfo
    {
        /// <summary>
        /// Gets or sets TitleItem.
        /// </summary>
        [DataMember(Name = "TitleItem")]
        public UIProductListNavigationItemInfo TitleItem { get; set; }

        /// <summary>
        /// Gets or sets NavigationItemList.
        /// </summary>
        [DataMember(Name = "NavigationItemList")]
        public List<UIProductListNavigationItemInfo> NavigationItemList { get; set; }
    }
}
