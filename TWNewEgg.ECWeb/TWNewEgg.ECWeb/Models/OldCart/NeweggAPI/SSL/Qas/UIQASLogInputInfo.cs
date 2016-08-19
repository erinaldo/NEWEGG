using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIQASLogInputInfo class.
    /// </summary>
    [DataContract]
    public class UIQASLogInputInfo
    {
        /// <summary>
        /// Gets or sets SessinID.
        /// </summary>
        [DataMember(Name = "SessinID")]
        public string SessinID { get; set; }

        /// <summary>
        /// Gets or sets CustomerNumber.
        /// </summary>
        [DataMember(Name = "CustomerNumber")]
        public int CustomerNumber { get; set; }

        /// <summary>
        /// Gets or sets originalCustomerShippingInfo.
        /// </summary>
        [DataMember(Name = "originalCustomerShippingInfo")]
        public UICustomerShippingInfo OriginalCustomerShippingInfo { get; set; }

        /// <summary>
        /// Gets or sets originalCustomerBillingInfo.
        /// </summary>
        [DataMember(Name = "originalCustomerBillingInfo")]
        public UICustomerShippingInfo OriginalCustomerBillingInfo { get; set; }
    }
}
