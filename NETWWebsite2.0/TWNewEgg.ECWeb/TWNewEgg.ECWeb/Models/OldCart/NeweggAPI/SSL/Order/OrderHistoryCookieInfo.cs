using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models.SalesOrder
{
    /// <summary>
    /// Initializes a new instance of the OrderHistoryCookieInfo class.
    /// </summary>
    [DataContract]
    public class OrderHistoryCookieInfo
    {
        /// <summary>
        /// Gets or sets OrderDate.
        /// </summary>
        [DataMember(Name = "od")]
        public string OrderDate { get; set; }

        /// <summary>
        /// Gets or sets soNumber.
        /// </summary>
        [DataMember(Name = "son")]
        public int SONumber { get; set; }

        /// <summary>
        /// Gets or sets preSONumber.
        /// </summary>
        [DataMember(Name = "pson")]
        public int PreSONumber { get; set; }

        /// <summary>
        /// Gets or sets soStatus.
        /// </summary>
        [DataMember(Name = "ss")]
        public string SoStatus { get; set; }

        /// <summary>
        /// Gets or sets TrackingList.
        /// </summary>
        [DataMember(Name = "tl")]
        public string TrackingList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether isShowCancelButton.
        /// </summary>
        [DataMember(Name = "iscb")]
        public bool IsShowCancelButton { get; set; }

        /// <summary>
        /// Gets or sets securityCode.
        /// </summary>
        [DataMember(Name = "sc")]
        public string SecurityCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether isCorrelativeComboSO.
        /// </summary>
        [DataMember(Name = "iccso")]
        public bool IsCorrelativeComboSO { get; set; }

        /// <summary>
        /// Gets or sets invoiceNumber.
        /// </summary>
        [DataMember(Name = "ino")]
        public int InvoiceNumber { get; set; }

        /// <summary>
        /// Gets or sets ponumber.
        /// </summary>
        [DataMember(Name = "pon")]
        public string PONumber { get; set; }

        /// <summary>
        /// Gets or sets refundOrder.
        /// </summary>
        [DataMember(Name = "ro")]
        public string RefundOrder { get; set; }

        /// <summary>
        /// Gets or sets replaceOrder.
        /// </summary>
        [DataMember(Name = "rpo")]
        public string ReplaceOrder { get; set; }
    }
}
