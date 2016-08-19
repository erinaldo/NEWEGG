using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Global = Newegg.Mobile.MvcApplication.Globalization.Models.Order;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [DataContract]
    public class UITrackingOrderInfo
    {
        /// <summary>
        /// Gets or sets TrackingOrderItemList.
        /// </summary>
        [DataMember]
        public List<UITrackingOrderItemInfo> TrackingOrderItemList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsRedirectToThirdPartyWebsite.
        /// </summary>
        [DataMember]
        public bool IsRedirectToThirdPartyWebsite { get; set; }

        /// <summary>
        /// Gets or sets TrackingResultURL.
        /// </summary>
        [DataMember]
        public string TrackingResultURL { get; set; }

        /// <summary>
        /// Gets or sets TrackingNumber.
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Global.TrackingOrderModel), Name = "Tracking")]
        public string TrackingNumber { get; set; }

        /// <summary>
        /// Gets or sets TrackingActivityList.
        /// </summary>
        [DataMember]
        public List<UITrackingActivityInfo> TrackingActivityList { get; set; }
    }
}
