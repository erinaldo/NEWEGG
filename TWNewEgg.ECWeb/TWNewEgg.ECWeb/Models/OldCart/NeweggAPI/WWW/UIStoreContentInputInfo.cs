using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIStoreContentInputInfo class.
    /// </summary>
    [DataContract]
    public class UIStoreContentInputInfo
    {
        /// <summary>
        /// Initializes a new instance of the UIStoreContentInputInfo class.
        /// </summary>
        public UIStoreContentInputInfo()
        {
            // Result StoreDepaId.
            this.StoreDepaId = -1;

            // Result CategoryId.
            this.CategoryId = -1;
        }

        /// <summary>
        /// Gets or sets StoreType.
        /// </summary>
        [DataMember(Name = "StoreType")]
        public int StoreType { get; set; }

        /// <summary>
        /// Gets or sets StoreDepaId.
        /// </summary>
        [DataMember(Name = "StoreDepaId")]
        public int StoreDepaId { get; set; }

        /// <summary>
        /// Gets or sets CategoryId.
        /// </summary>
        [DataMember(Name = "CategoryId")]
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets PageNumber.
        /// </summary>
        [DataMember(Name = "PageNumber")]
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IncludeFeaturedItem.
        /// </summary>
        [DataMember(Name = "IncludeFeaturedItem")]
        public bool IncludeFeaturedItem { get; set; }
    }
}
