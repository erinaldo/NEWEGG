using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the NavigationItemList class.
    /// </summary>
    [DataContract]
    public class NavigationItemList
    {
        /// <summary>
        /// Gets or sets BrandId.
        /// </summary>
        [DataMember(Name = "BrandId")]
        public int BrandId { get; set; }

        /// <summary>
        /// Gets or sets CategoryId.
        /// </summary>
        [DataMember(Name = "CategoryId")]
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets CustomLink.
        /// </summary>
        [DataMember(Name = "CustomLink")]
        public string CustomLink { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        [DataMember(Name = "Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets ItemCount.
        /// </summary>
        [DataMember(Name = "ItemCount")]
        public int ItemCount { get; set; }

        /// <summary>
        /// Gets or sets Keyword.
        /// </summary>
        [DataMember(Name = "Keyword")]
        public string Keyword { get; set; }

        /// <summary>
        /// Gets or sets NodeId.
        /// </summary>
        [DataMember(Name = "NodeId")]
        public int NodeId { get; set; }

        /// <summary>
        /// Gets or sets NValue.
        /// </summary>
        [DataMember(Name = "NValue")]
        public string NValue { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether ShowSeeAllDeals is true or false.
        /// </summary>
        [DataMember(Name = "ShowSeeAllDeals")]
        public bool ShowSeeAllDeals { get; set; }

        /// <summary>
        /// Gets or sets StoreId.
        /// </summary>
        [DataMember(Name = "StoreId")]
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets StoreType.
        /// </summary>
        [DataMember(Name = "StoreType")]
        public StoreType StoreType { get; set; }

        /// <summary>
        /// Gets or sets SubCategoryId.
        /// </summary>
        [DataMember(Name = "SubCategoryId")]
        public int SubCategoryId { get; set; }
    }
}
