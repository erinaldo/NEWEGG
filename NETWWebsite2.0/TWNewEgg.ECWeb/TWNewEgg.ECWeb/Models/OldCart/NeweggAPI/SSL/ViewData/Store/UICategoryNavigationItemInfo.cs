using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UICategoryNavigationItemInfo class.
    /// </summary>
    [DataContract]
    public class UICategoryNavigationItemInfo
    {
        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        [DataMember(Name = "Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets CategoryType.
        /// </summary>
        [DataMember(Name = "CategoryType")]
        public CategoryType CategoryType { get; set; }

        /// <summary>
        /// Gets or sets CategoryID.
        /// </summary>
        [DataMember(Name = "CategoryID")]
        public int CategoryID { get; set; }

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
        /// Gets or sets NodeId.
        /// </summary>
        [DataMember(Name = "NodeId")]
        public int NodeId { get; set; }
    }
}
