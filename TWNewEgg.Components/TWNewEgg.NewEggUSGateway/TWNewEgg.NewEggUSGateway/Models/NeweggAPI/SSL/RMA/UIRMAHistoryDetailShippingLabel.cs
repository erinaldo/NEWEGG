using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIRMAHistoryDetailShippingLabel class.
    /// </summary>
    [DataContract]
    public class UIRMAHistoryDetailShippingLabel
    {
        /// <summary>
        /// Gets or sets a value indicating whether CanTracking.
        /// </summary>
        [DataMember(Name = "CanTracking")]
        public bool CanTracking { get; set; }

        /// <summary>
        /// Gets or sets TrackingNumber.
        /// </summary>
        [DataMember(Name = "TrackingNumber")]
        public string TrackingNumber { get; set; }

        /// <summary>
        /// Gets or sets Title.
        /// </summary>
        [DataMember(Name = "Title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets TrackingStatusDescription.
        /// </summary>
        [DataMember(Name = "TrackingStatusDescription")]
        public string TrackingStatusDescription { get; set; }

        /// <summary>
        /// Gets or sets InDate.
        /// </summary>
        [DataMember(Name = "InDate")]
        public string InDate { get; set; }

        /// <summary>
        /// Gets or sets itemList.
        /// </summary>
        [DataMember(Name = "ItemList")]
        public List<UIRMAHistoryDetailItemBase> ItemList { get; set; }
    }
}
