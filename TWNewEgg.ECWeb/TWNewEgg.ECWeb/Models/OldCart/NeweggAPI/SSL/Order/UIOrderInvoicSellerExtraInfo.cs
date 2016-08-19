using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIOrderInvoicSellerExtraInfo class.
    /// </summary>
    [DataContract]
    public class UIOrderInvoicSellerExtraInfo
    {
        /// <summary>
        /// Gets or sets a value indicating whether IsShowRateSeller.
        /// </summary>
        [DataMember(Name = "IsShowRateSeller")]
        public bool IsShowRateSeller { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsShowRateEggs.
        /// </summary>
        [DataMember(Name = "IsShowRateEggs")]
        public bool IsShowRateEggs { get; set; }

        /// <summary>
        /// Gets or sets Rating.
        /// </summary>
        [DataMember(Name = "Rating")]
        public int Rating { get; set; }

        /// <summary>
        /// Gets or sets ReviewDate.
        /// </summary>
        [DataMember(Name = "ReviewDate")]
        public string ReviewDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsShowEmailSeller.
        /// </summary>
        [DataMember(Name = "IsShowEmailSeller")]
        public bool IsShowEmailSeller { get; set; }
    }
}
