using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIMoreTabStoreNavigationItemInfo class.
    /// </summary>
    [DataContract]
    public class UIMoreTabStoreNavigationItemInfo
    {
        /// <summary>
        /// Gets or sets StoreType.
        /// </summary>
        [DataMember(Name = "StoreType")]
        public StoreType StoreType { get; set; }

        /// <summary>
        /// Gets or sets StoreDepaId.
        /// </summary>
        [DataMember(Name = "StoreDepaId")]
        public int StoreDepaId { get; set; }

        /// <summary>
        /// Gets or sets NValue.
        /// </summary>
        [DataMember(Name = "NValue")]
        public string NValue { get; set; }

        /// <summary>
        /// Gets or sets ItemCount.
        /// </summary>
        [DataMember(Name = "ItemCount")]
        public int ItemCount { get; set; }

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

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        [DataMember(Name = "Description")]
        public string Description { get; set; }
    }
}
