using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// ShoppingCart Item Cookie Info.
    /// </summary>
    [DataContract]
    public class HistoryCookieItemInfo
    {
        /// <summary>
        /// Gets or sets ItemNumber.
        /// </summary>
        [DataMember(Name = "itn")]
        public string ItemNumber { get; set; }

        /// <summary>
        /// Gets or sets item image URL.
        /// </summary>
        [DataMember(Name = "url")]
        public string ItemImageUrl { get; set; }

        /// <summary>
        /// Gets or sets item price.
        /// </summary>
        [DataMember(Name = "pri")]
        public string ItemPrice { get; set; }
    }
}
