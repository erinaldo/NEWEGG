using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIGiftCertificateRedeemInfo class.
    /// </summary>
    [DataContract]
    public class UIGiftCertificateRedeemInfo
    {
        /// <summary>
        /// Gets or sets ErrorCode.
        /// </summary>
        [DataMember(Name = "ErrorCode")]
        public int ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets GCRedeemAmount.
        /// </summary>
        [DataMember(Name = "GCRedeemAmount")]
        public string GCRedeemAmount { get; set; }

        /// <summary>
        /// Gets or sets GiftCode.
        /// </summary>
        [DataMember(Name = "GiftCode")]
        public string GiftCode { get; set; }

        /// <summary>
        /// Gets or sets LeftAmount.
        /// </summary>
        [DataMember(Name = "LeftAmount")]
        public string LeftAmount { get; set; }

        /// <summary>
        /// Gets or sets SecurityCode.
        /// </summary>
        [DataMember(Name = "SecurityCode")]
        public string SecurityCode { get; set; }
    }
}
