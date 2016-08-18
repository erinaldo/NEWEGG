using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIQueryRMATrackingNumberItemInfo class.
    /// </summary>
    [DataContract]
    public class UIQueryRMATrackingNumberItemInfo
    {
        /// <summary>
        /// Gets or sets LabelSONumber.
        /// </summary>
        [DataMember(Name = "LabelSONumber")]
        public int LabelSONumber { get; set; }

        /// <summary>
        /// Gets or sets TrackingNumber.
        /// </summary>
        [DataMember(Name = "TrackingNumber")]
        public string TrackingNumber { get; set; }

        /// <summary>
        /// Gets or sets TrackingNumberStatus.
        /// </summary>
        [DataMember(Name = "TrackingNumberStatus")]
        public string TrackingNumberStatus { get; set; }

        /// <summary>
        /// Gets or sets IssueDate.
        /// </summary>
        [DataMember(Name = "IssueDate")]
        public string IssueDate { get; set; }

        /// <summary>
        /// Gets or sets UPSShipment.
        /// </summary>
        [DataMember(Name = "UPSShipment")]
        public string UPSShipment { get; set; }

        /// <summary>
        /// Gets or sets ReprintLabel.
        /// </summary>
        [DataMember(Name = "ReprintLabel")]
        public string ReprintLabel { get; set; }

        /// <summary>
        /// Gets or sets RMAItemList.
        /// </summary>
        [DataMember(Name = "RMAItemList")]
        public List<UIQueryRMAItemInfo> RMAItemList { get; set; }
    }
}
