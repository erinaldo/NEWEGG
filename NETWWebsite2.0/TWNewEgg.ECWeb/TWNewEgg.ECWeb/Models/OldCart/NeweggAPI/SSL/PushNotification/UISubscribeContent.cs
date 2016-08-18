using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UISubscribeContent class.
    /// </summary>
    [DataContract]
    public class UISubscribeContent
    {
        /// <summary>
        /// Gets or sets SubscribeItemInfoList.
        /// </summary>
        [DataMember(Name = "SubscribeItemInfoList")]
        public List<UISubscribeItemInfo> SubscribeItemInfoList { get; set; }

        /// <summary>
        /// Gets or sets PageInfo.
        /// </summary>
        [DataMember(Name = "PageInfo")]
        public UIPageInfo PageInfo { get; set; }
    }
}
