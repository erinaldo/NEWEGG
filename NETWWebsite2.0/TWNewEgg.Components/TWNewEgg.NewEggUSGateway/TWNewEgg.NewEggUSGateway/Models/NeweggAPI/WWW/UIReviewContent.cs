using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIReviewContent class.
    /// </summary>
    [DataContract]
    public class UIReviewContent
    {
        /// <summary>
        /// Gets or sets Summary.
        /// </summary>
        [DataMember(Name = "Summary")]
        public UIReviewSummary Summary { get; set; }

        /// <summary>
        /// Gets or sets Reviews.
        /// </summary>
        [DataMember(Name = "Reviews")]
        public List<UIReviewInfo> Reviews { get; set; }

        /// <summary>
        /// Gets or sets PaginationInfo.
        /// </summary>
        [DataMember(Name = "PaginationInfo")]
        public UIPageInfo PaginationInfo { get; set; }

        /// <summary>
        /// Gets or sets ProductImageInfo.
        /// </summary>
        [DataMember(Name = "ProductImageInfo")]
        public UIImageInfo ProductImageInfo { get; set; }

        /// <summary>
        /// Gets or sets ProductReviewBarInfo.
        /// </summary>
        [DataMember(Name = "ProductReviewBarInfo")]
        public UIProductReviewBarInfo ProductReviewBarInfo { get; set; }
    }
}