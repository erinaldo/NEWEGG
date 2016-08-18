using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIRMAHistoryContent class.
    /// </summary>
    [DataContract]
    public class UIRMAHistoryContent
    {
        /// <summary>
        /// Gets or sets SeeListing.
        /// </summary>
        [DataMember(Name = "SeeListing")]
        public string SeeListing { get; set; }

        /// <summary>
        /// Gets or sets NoResultMessage.
        /// </summary>
        [DataMember(Name = "NoResultMessage")]
        public string NoResultMessage { get; set; }

        /// <summary>
        /// Gets or sets PageInfo.
        /// </summary>
        [DataMember(Name = "PageInfo")]
        public UIPageInfo PageInfo { get; set; }

        /// <summary>
        /// Gets or sets OrderSummaryList.
        /// </summary>
        [DataMember(Name = "OrderSummaryList")]
        public List<UIRMAHistorySummary> OrderSummaryList { get; set; }

        /// <summary>
        /// Gets or sets SearchTimeRegion.
        /// </summary>
        [DataMember(Name = "SearchTimeRegion")]
        public List<UISelectOrderOption> SearchTimeRegion { get; set; }
    }
}
