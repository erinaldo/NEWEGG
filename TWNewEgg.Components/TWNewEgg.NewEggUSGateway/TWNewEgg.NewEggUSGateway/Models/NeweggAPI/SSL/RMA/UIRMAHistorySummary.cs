using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIRMAHistorySummary class.
    /// </summary>
    [DataContract]
    public class UIRMAHistorySummary : UIOrderSummaryInfo
    {
        /// <summary>
        /// Gets or sets a value indicating whether IsShowCanCreateButton.
        /// </summary>
        [DataMember(Name = "IsShowCanCreateButton")]
        public bool IsShowCanCreateButton { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsShowTrackShipmentButton.
        /// </summary>
        [DataMember(Name = "IsShowTrackShipmentButton")]
        public bool IsShowTrackShipmentButton { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsShowRePrintButton.
        /// </summary>
        [DataMember(Name = "IsShowRePrintButton")]
        public bool IsShowRePrintButton { get; set; }

        /// <summary>
        /// Gets or sets RMAType.
        /// </summary>
        [DataMember(Name = "RMAType")]
        public string RMAType { get; set; }
    }
}
