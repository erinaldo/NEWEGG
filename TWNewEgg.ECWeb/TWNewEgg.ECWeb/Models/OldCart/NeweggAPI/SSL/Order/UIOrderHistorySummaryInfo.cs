using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIOrderHistorySummaryInfo class.
    /// </summary>
    [DataContract]
    public class UIOrderHistorySummaryInfo : UIOrderSummaryInfo
    {
        /// <summary>
        /// Gets or sets PreSONumber.
        /// </summary>
        [DataMember(Name = "PreSONumber")]
        public int PreSONumber { get; set; }
        
        /// <summary>
        /// Gets or sets TrackingList.
        /// </summary>
        [DataMember(Name = "TrackingList")]
        public List<string> TrackingList { get; set; }

        /// <summary>
        /// Gets or sets RefundOrder.
        /// </summary>
        [DataMember(Name = "RefundOrder")]
        public int RefundOrder { get; set; }

        /// <summary>
        /// Gets or sets ReplaceOrder.
        /// </summary>
        [DataMember(Name = "ReplaceOrder")]
        public int ReplaceOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsCorrelativeComboSO.
        /// </summary>
        [DataMember(Name = "IsCorrelativeComboSO")]
        public bool IsCorrelativeComboSO { get; set; }

        /// <summary>
        /// Gets or sets CorrelativeComboMessage.
        /// </summary>
        [DataMember(Name = "CorrelativeComboMessage")]
        public string CorrelativeComboMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsShowCancelButton.
        /// </summary>
        [DataMember(Name = "IsShowCancelButton")]
        public bool IsShowCancelButton { get; set; }

        /// <summary>
        /// Gets or sets CancelButtonText.
        /// </summary>
        [DataMember(Name = "CancelButtonText")]
        public string CancelButtonText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsShowRefundButton.
        /// </summary>
        [DataMember(Name = "IsShowRefundButton")]
        public bool IsShowRefundButton { get; set; }

        /// <summary>
        /// Gets or sets RefundButtonText.
        /// </summary>
        [DataMember(Name = "RefundButtonText")]
        public string RefundButtonText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsShowReplaceButton.
        /// </summary>
        [DataMember(Name = "IsShowReplaceButton")]
        public bool IsShowReplaceButton { get; set; }

        /// <summary>
        /// Gets or sets ReplaceButtonText.
        /// </summary>
        [DataMember(Name = "ReplaceButtonText")]
        public string ReplaceButtonText { get; set; }
    }
}
