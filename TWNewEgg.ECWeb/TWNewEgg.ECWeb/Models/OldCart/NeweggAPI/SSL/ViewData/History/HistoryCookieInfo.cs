using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// ShoppingCart Cookie Info.
    /// </summary>
    [DataContract]
    public class HistoryCookieInfo
    {
        /// <summary>
        /// Gets or sets itemList.
        /// </summary>
        [DataMember(Name = "ls")]
        public List<HistoryCookieItemInfo> ItemList { get; set; }
    }
}
