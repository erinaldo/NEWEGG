using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIProductSearchResult class.
    /// </summary>
    [DataContract]
    public class UIProductSearchResult
    {
        /// <summary>
        /// Gets or sets ProductListItems.
        /// </summary>
        [DataMember(Name = "ProductListItems")]
        public List<UIProductListItemInfo> ProductListItems { get; set; }
		
        /// <summary>
        /// Gets or sets PaginationInfo.
        /// </summary>
        [DataMember(Name = "PaginationInfo")]
        public UIPageInfo PaginationInfo { get; set; }
    }
}