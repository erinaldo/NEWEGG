using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIOrderSummaryInfo class.
    /// </summary>
    [DataContract]
    public class UIOrderSummaryInfo
    {
        /// <summary>
        /// Gets or sets SOAmount.
        /// </summary>
        [DataMember(Name = "SOAmount")]
        public string SOAmount { get; set; }

        /// <summary>
        /// Gets or sets SODate.
        /// </summary>
        [DataMember(Name = "SODate")]
        public string SODate { get; set; }

        /// <summary>
        /// Gets or sets SONumber.
        /// </summary>
        [DataMember(Name = "SONumber")]
        public int SONumber { get; set; }

        /// <summary>
        /// Gets or sets PONumber.
        /// </summary>
        [DataMember(Name = "PONumber")]
        public string PONumber { get; set; } 

        /// <summary>
        /// Gets or sets InvoiceNumber.
        /// </summary>
        [DataMember(Name = "InvoiceNumber")]
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Gets or sets OrderDescriptions.
        /// </summary>
        [DataMember(Name = "OrderDescriptions")]
        public List<UIOrderItemInfo> OrderDescriptions { get; set; }

        /// <summary>
        /// Gets or sets SOStatus.
        /// </summary>
        [DataMember(Name = "SOStatus")]
        public string SOStatus { get; set; }

        /// <summary>
        /// Gets or sets SOStatusDescription.
        /// </summary>
        [DataMember(Name = "SOStatusDescription")]
        public string SOStatusDescription { get; set; }
    }
}
