using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIProductListNavigationItemInfo class.
    /// </summary>
    [DataContract]
    public class UIProductListNavigationItemInfo
    {
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
        /// Gets or sets StoreDepaId.
        /// </summary>
        [DataMember(Name = "StoreDepaId")]
        public int StoreDepaId { get; set; }

        /// <summary>
        /// Gets or sets StoreType.
        /// </summary>
        [DataMember(Name = "StoreType")]
        public int StoreType { get; set; }

        /// <summary>
        /// Gets or sets CategoryId.
        /// </summary>
        [DataMember(Name = "CategoryId")]
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets SubCategoryId.
        /// </summary>
        [DataMember(Name = "SubCategoryId")]
        public int SubCategoryId { get; set; }

        /// <summary>
        /// Gets or sets BrandId.
        /// </summary>
        [DataMember(Name = "BrandId")]
        public int BrandId { get; set; }

        /// <summary>
        /// Gets or sets NValue.
        /// </summary>
        [DataMember(Name = "NValue")]
        public string NValue { get; set; }

        /// <summary>
        /// Gets or sets ElementValue.
        /// </summary>
        [DataMember(Name = "ElementValue")]
        public string ElementValue { get; set; }
    }
}
