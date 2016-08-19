using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIAutoNotifyContent class.
    /// </summary>
    [DataContract]
    public class UIAutoNotifyContent
    {
        /// <summary>
        /// Gets or sets PageIndex.
        /// </summary>
        [DataMember(Name = "PageIndex")]
        public int PageIndex { get; set; }

        /// <summary>
        /// Gets or sets TotalPage.
        /// </summary>
        [DataMember(Name = "TotalPage")]
        public int TotalPage { get; set; }

        /// <summary>
        /// Gets or sets AutoNotifyCellInfoList.
        /// </summary>
        [DataMember(Name = "AutoNotifyCellInfoList")]
        public List<UIAutoNotifyCellInfo> AutoNotifyCellInfoList { get; set; }
    }
}
