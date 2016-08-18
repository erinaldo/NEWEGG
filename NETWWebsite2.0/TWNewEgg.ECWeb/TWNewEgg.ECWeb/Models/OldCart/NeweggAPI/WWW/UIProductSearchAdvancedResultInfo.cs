using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIProductSearchAdvancedResultInfo class.
    /// </summary>
    [DataContract]
    public class UIProductSearchAdvancedResultInfo : UIProductSearchResult
    {
        /// <summary>
        /// Gets or sets NavigationContentList.
        /// </summary>
        [DataMember(Name = "NavigationContentList")]
        public List<UIProductListNavigationContentInfo> NavigationContentList { get; set; }

        /// <summary>
        /// Gets or sets RelatedLinkList.
        /// gets or sets related links
        /// this refer to "Is This what you are looking for?".
        /// </summary>
        [DataMember(Name = "RelatedLinkList")]
        public List<UIProductListNavigationContentInfo> RelatedLinkList { get; set; }

        /// <summary>
        /// Gets or sets CoremetricsInfo.
        /// </summary>
        [DataMember(Name = "CoremetricsInfo")]
        public CoremetricsInfo CoremetricsInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsAllComboBundle.
        /// </summary>
        [DataMember(Name = "IsAllComboBundle")]
        public bool IsAllComboBundle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether CanBeCompare.
        /// </summary>
        [DataMember(Name = "CanBeCompare")]
        public bool CanBeCompare { get; set; }

        /// <summary>
        /// Gets or sets MasterComboStoreId.
        /// </summary>
        [DataMember(Name = "MasterComboStoreId")]
        public int MasterComboStoreId { get; set; }

        /// <summary>
        /// Gets or sets SubCategoryId.
        /// </summary>
        [DataMember(Name = "SubCategoryId")]
        public int SubCategoryId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether HasDeactivatedItems.
        /// </summary>
        [DataMember(Name = "HasDeactivatedItems")]
        public bool HasDeactivatedItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether HasHasSimilarItems.
        /// </summary>
        [DataMember(Name = "HasHasSimilarItems")]
        public bool HasHasSimilarItems { get; set; }

        /// <summary>
        /// Gets or sets SearchProvider.
        /// </summary>
        [DataMember(Name = "SearchProvider")]
        public UISearchProvider SearchProvider { get; set; }

        /// <summary>
        /// Gets or sets SearchResultType.
        /// </summary>
        [DataMember(Name = "SearchResultType")]
        public UISearchResultType SearchResultType { get; set; }
    }
}
