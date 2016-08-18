using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIExpressCheckoutDetailResponse class.
    /// </summary>
    [DataContract]
    public class UIExpressCheckoutDetailResponse
    {
        /// <summary>
        /// Gets or sets PaypalInfo.
        /// </summary>
        [DataMember(Name = "PaypalInfo")]
        public UIPaypalEntity PaypalInfo { get; set; }
        
        /// <summary>
        /// Gets or sets CheckoutErrorMessage.
        /// </summary>
        [DataMember(Name = "CheckoutErrorMessage")]
        public string CheckoutErrorMessage { get; set; }
    }
}