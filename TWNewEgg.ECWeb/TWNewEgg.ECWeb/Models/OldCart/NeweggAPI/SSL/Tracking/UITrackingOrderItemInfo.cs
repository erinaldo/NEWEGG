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
    public class UITrackingOrderItemInfo
    {
        /// <summary>
        /// Gets or sets DisplayTitle.
        /// </summary>
        [DataMember]
        public string DisplayTitle { get; set; }

        /// <summary>
        /// Gets or sets DisplayValue.
        /// </summary>
        [DataMember]
        public string DisplayValue { get; set; }
    }
}
