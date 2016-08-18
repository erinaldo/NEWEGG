using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIQueryRMAContent class.
    /// </summary>
    [DataContract]
    public class UIQueryRMAContent
    {
        /// <summary>
        /// Gets or sets PageCount.
        /// </summary>
        [DataMember(Name = "PageCount")]
        public int PageCount { get; set; }

        /// <summary>
        /// Gets or sets Page.
        /// </summary>
        [DataMember(Name = "Page")]
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets RMAHistoryShowInfoList.
        /// </summary>
        [DataMember(Name = "RMAHistoryShowInfoList")]
        public List<UIRMAHistoryShowInfo> RMAHistoryShowInfoList { get; set; }
    }
}
