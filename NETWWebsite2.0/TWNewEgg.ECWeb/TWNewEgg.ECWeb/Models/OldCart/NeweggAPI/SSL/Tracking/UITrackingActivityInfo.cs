using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Global = Newegg.Mobile.MvcApplication.Globalization.Models.Order;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Tracking Activity Info.
    /// </summary>
    public class UITrackingActivityInfo
    {
        /// <summary>
        /// Gets or sets Date.
        /// </summary>
        [Display(ResourceType = typeof(Global.TrackingOrderModel), Name = "ShipDate")]
        public string Date { get; set; }

        /// <summary>
        /// Gets or sets Activity.
        /// </summary>
        public string Activity { get; set; }

        /// <summary>
        /// Gets or sets Location.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets Details.
        /// </summary>
        public string Details { get; set; }
    }
}
