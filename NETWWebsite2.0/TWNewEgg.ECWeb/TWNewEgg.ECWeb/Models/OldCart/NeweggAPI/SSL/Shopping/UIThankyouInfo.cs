using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIThankyouInfo class.
    /// </summary>
    [DataContract]
    public class UIThankyouInfo
    {
        /// <summary>
        /// Gets or sets CartNO.
        /// </summary>
        public string CartNO { get; set; }

        /// <summary>
        /// Gets or sets SubmittedDate.
        /// </summary>
        [DataMember(Name = "SubmittedDate")]
        public string SubmittedDate { get; set; }

        /// <summary>
        /// Gets or sets UICoreMetricsInfo.
        /// </summary>
        [DataMember(Name = "UICoreMetricsInfo")]
        public UICoreMetricsInfo UICoreMetricsInfo { get; set; }

        /// <summary>
        /// Gets or sets CustomerInfo.
        /// </summary>
        [DataMember(Name = "CustomerInfo")]
        public UICustomerInfo CustomerInfo { get; set; }

        /// <summary>
        /// Gets or sets ShipTo.
        /// </summary>
        [DataMember(Name = "ShipTo")]
        public List<string> ShipTo { get; set; }

        /// <summary>
        /// Gets or sets BillingTo.
        /// </summary>
        [DataMember(Name = "BillingTo")]
        public List<string> BillingTo { get; set; }

        /// <summary>
        /// Gets or sets OrderInfoList.
        /// </summary>
        [DataMember(Name = "OrderInfoList")]
        public List<UIOrderForThankyouInfo> OrderInfoList { get; set; }

        /// <summary>
        /// Gets or sets GrandTotal.
        /// </summary>
        [DataMember(Name = "GrandTotal")]
        public string GrandTotal { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsGrandTotalShow.
        /// </summary>
        [DataMember(Name = "IsGrandTotalShow")]
        public bool IsGrandTotalShow { get; set; }

        /// <summary>
        /// Gets or sets SeeListing.
        /// </summary>
        [DataMember(Name = "SeeListing")]
        public string SeeListing { get; set; }

        /// <summary>
        /// Gets or sets Notes.
        /// </summary>
        [DataMember(Name = "Notes")]
        public List<UINoteItemInfo> Notes { get; set; }
    }
}
