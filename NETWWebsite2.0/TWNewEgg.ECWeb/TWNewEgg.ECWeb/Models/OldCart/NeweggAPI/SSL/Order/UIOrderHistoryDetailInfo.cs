using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIOrderHistoryDetailInfo class.
    /// </summary>
    [DataContract]
    public class UIOrderHistoryDetailInfo : UIOrderInfo
    {
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
        /// Gets or sets ShipToLegend.
        /// </summary>
        [DataMember(Name = "ShipToLegend")]
        public string ShipToLegend { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsShowAddToCart.
        /// </summary>
        [DataMember(Name = "IsShowAddToCart")]
        public bool IsShowAddToCart { get; set; }

        /// <summary>
        /// Gets or sets ProcessInformation.
        /// </summary>
        [DataMember(Name = "ProcessInformation")]
        public string ProcessInformation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsShowProcessInfo.
        /// </summary>
        [DataMember(Name = "IsShowProcessInfo")]
        public bool IsShowProcessInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsShowEditCard.
        /// </summary>
        [DataMember(Name = "IsShowEditCard")]
        public bool IsShowEditCard { get; set; }

        /// <summary>
        /// Gets or sets GiftCardAmountNote.
        /// </summary>
        [DataMember(Name = "GiftCardAmountNote")]
        public string GiftCardAmountNote { get; set; }
    }
}
