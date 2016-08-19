using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UISetExpressCheckoutRequest class.
    /// </summary>
    [DataContract]
    public class UISetExpressCheckoutRequest
    {
        /// <summary>
        /// Gets or sets CustomerNumber.
        /// </summary>
        [DataMember(Name = "CustomerNumber")]
        public int CustomerNumber { get; set; }

        /// <summary>
        /// Gets or sets PaypalInfo.
        /// </summary>
        [DataMember(Name = "PaypalInfo")]
        public UIPaypalEntity PaypalInfo { get; set; }
    }
}