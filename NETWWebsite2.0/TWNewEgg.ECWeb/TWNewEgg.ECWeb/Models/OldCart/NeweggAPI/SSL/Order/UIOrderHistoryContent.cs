using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIOrderHistoryContent class.
    /// </summary>
    [DataContract]
    public class UIOrderHistoryContent
    {
        /// <summary>
        /// Gets or sets SeeListing.
        /// </summary>
        [DataMember(Name = "SeeListing")]
        public string SeeListing { get; set; }

        /// <summary>
        /// Gets or sets BeginDate.
        /// </summary>
        [DataMember(Name = "BeginDate")]
        public string BeginDate { get; set; }

        /// <summary>
        /// Gets or sets EndDate.
        /// </summary>
        [DataMember(Name = "EndDate")]
        public string EndDate { get; set; }

        /// <summary>
        /// Gets or sets NoResultMessage.
        /// </summary>
        [DataMember(Name = "NoResultMessage")]
        public string NoResultMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsDisplayShowAll.
        /// </summary>
        [DataMember(Name = "IsDisplayShowAll")]
        public bool IsDisplayShowAll { get; set; }

        /// <summary>
        /// Gets or sets PageCount.
        /// </summary>
        [DataMember(Name = "PageCount")]
        public int PageCount { get; set; }

        /// <summary>
        /// Gets or sets PageInfo.
        /// </summary>
        [DataMember(Name = "PageInfo")]
        public UIPageInfo PageInfo { get; set; }

        /// <summary>
        /// Gets or sets OrderSummaryList.
        /// </summary>
        [DataMember(Name = "OrderSummaryList")]
        public List<UIOrderHistorySummaryInfo> OrderSummaryList { get; set; }

        /// <summary>
        /// Gets or sets SearchTimeRegion.
        /// </summary>
        [DataMember(Name = "SearchTimeRegion")]
        public List<UISelectOrderOption> SearchTimeRegion { get; set; }
    }
}
