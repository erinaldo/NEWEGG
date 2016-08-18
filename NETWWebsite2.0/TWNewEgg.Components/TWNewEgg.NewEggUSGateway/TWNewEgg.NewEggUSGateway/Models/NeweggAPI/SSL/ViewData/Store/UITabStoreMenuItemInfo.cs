using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UITabStoreMenuItemInfo class.
    /// </summary>
    [DataContract]
    public class UITabStoreMenuItemInfo
    {
        /// <summary>
        /// Gets or sets Title.
        /// </summary>
        [DataMember(Name = "Title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets StoreDepa.
        /// </summary>
        [DataMember(Name = "StoreDepa")]
        public string StoreDepa { get; set; }

        /// <summary>
        /// Gets or sets StoreID.
        /// </summary>
        [DataMember(Name = "StoreID")]
        public int StoreID { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether ShowSeeAllDeals.
        /// </summary>
        [DataMember(Name = "ShowSeeAllDeals")]
        public bool ShowSeeAllDeals { get; set; }

        /// <summary>
        /// Gets or sets categoryList.
        /// </summary>
        [DataMember(Name = "CategoryList")]
        public List<UICategoryNavigationItemInfo> CategoryList { get; set; }
    }
}
