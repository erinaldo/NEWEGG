using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UINewRMAHistoryDetail class.
    /// </summary>
    [DataContract]
    public class UINewRMAHistoryDetail
    {
        /// <summary>
        /// Gets or sets RMANumber.
        /// </summary>
        [DataMember(Name = "RMANumber")]
        public int RMANumber { get; set; }

        /// <summary>
        /// Gets or sets RMADate.
        /// </summary>
        [DataMember(Name = "RMADate")]
        public string RMADate { get; set; }

        /// <summary>
        /// Gets or sets RMATotals.
        /// </summary>
        [DataMember(Name = "RMATotals")]
        public List<string> RMATotals { get; set; }

        /// <summary>
        /// Gets or sets ReturnInstructions.
        /// </summary>
        [DataMember(Name = "ReturnInstructions")]
        public string ReturnInstructions { get; set; }

        /// <summary>
        /// Gets or sets MemoList.
        /// </summary>
        [DataMember(Name = "MemoList")]
        public List<UIRMAHistoryDetailMemo> MemoList { get; set; }

        /// <summary>
        /// Gets or sets ShippingLabelInfoList.
        /// </summary>
        [DataMember(Name = "ShippingLabelInfoList")]
        public List<UIRMAHistoryDetailShippingLabel> ShippingLabelInfoList { get; set; }

        /// <summary>
        /// Gets or sets ItemDetailList.
        /// </summary>
        [DataMember(Name = "ItemDetailList")]
        public List<UIRMAHistoryDetailItem> ItemDetailList { get; set; }
    }
}
