using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIQueryRMAInfo class.
    /// </summary>
    [DataContract]
    public class UIQueryRMAInfo
    {
        /// <summary>
        /// Gets or sets RMANumber.
        /// </summary>
        [DataMember(Name = "RMANumber")]
        public int RMANumber { get; set; }

        /// <summary>
        /// Gets or sets CustomerNumber.
        /// </summary>
        [DataMember(Name = "CustomerNumber")]
        public int CustomerNumber { get; set; }

        /// <summary>
        /// Gets or sets RMAExtendPrice.
        /// </summary>
        [DataMember(Name = "RMAExtendPrice")]
        public decimal RMAExtendPrice { get; set; }

        /// <summary>
        /// Gets or sets CurrencyRMAExtendPrice.
        /// </summary>
        [DataMember(Name = "CurrencyRMAExtendPrice")]
        public string CurrencyRMAExtendPrice { get; set; }

        /// <summary>
        /// Gets or sets RMADate.
        /// </summary>
        [DataMember(Name = "RMADate")]
        public DateTime RMADate { get; set; }

        /// <summary>
        /// Gets RMADateFormat.
        /// </summary>
        [DataMember(Name = "RMADateFormat")]
        public string RMADateFormat
        {
            get
            {
                return this.RMADate.ToString(@"MM\/dd\/yyyy");
            }
        }

        /// <summary>
        /// Gets RMADateLongFormat.
        /// </summary>
        [DataMember(Name = "RMADateLongFormat")]
        public string RMADateLongFormat
        {
            get
            {
                return this.RMADate.ToString(@"MM\/dd\/yyyy hh:mmtt").ToLower();
            }
        }

        /// <summary>
        /// Gets or sets RMAType.
        /// </summary>
        [DataMember(Name = "RMAType")]
        public string RMAType { get; set; }

        /// <summary>
        /// Gets or sets RMAStatus.
        /// </summary>
        [DataMember(Name = "RMAStatus")]
        public string RMAStatus { get; set; }

        /// <summary>
        /// Gets or sets TaxAmount.
        /// </summary>
        [DataMember(Name = "TaxAmount")]
        public decimal TaxAmount { get; set; }

        /// <summary>
        /// Gets or sets CurrencyTaxAmount.
        /// </summary>
        [DataMember(Name = "CurrencyTaxAmount")]
        public decimal CurrencyTaxAmount { get; set; }

        /// <summary>
        /// Gets or sets ContactWith.
        /// </summary>
        [DataMember(Name = "ContactWith")]
        public string ContactWith { get; set; }

        /// <summary>
        /// Gets or sets SellerInfo.
        /// </summary>
        [DataMember(Name = "SellerInfo")]
        public UISellerRMADetailInfo SellerInfo { get; set; }

        /// <summary>
        /// Gets or sets SOType.
        /// </summary>
        [DataMember(Name = "SOType")]
        public string SOType { get; set; }

        /// <summary>
        /// Gets or sets SONumber.
        /// </summary>
        [DataMember(Name = "SONumber")]
        public int SONumber { get; set; }

        /// <summary>
        /// Gets or sets RMATotals.
        /// </summary>
        [DataMember(Name = "RMATotals")]
        public List<string> RMATotals { get; set; }
    }
}