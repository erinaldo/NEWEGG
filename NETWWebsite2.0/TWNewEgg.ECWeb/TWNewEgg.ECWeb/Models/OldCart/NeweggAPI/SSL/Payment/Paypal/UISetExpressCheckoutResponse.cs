using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UISetExpressCheckoutResponse class.
    /// </summary>
    [DataContract]
    public class UISetExpressCheckoutResponse
    {
        /// <summary>
        /// Gets or sets Token.
        /// </summary>
        [DataMember(Name = "Token")]
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets CheckoutErrorMessage.
        /// </summary>
        [DataMember(Name = "CheckoutErrorMessage")]
        public string CheckoutErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets CustomerShipping.
        /// </summary>
        [DataMember(Name = "CustomerShipping")]
        public UICustomerShipping CustomerShipping { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsNeedCheckout.
        /// </summary>
        [DataMember(Name = "IsNeedCheckout")]
        public bool IsNeedCheckout { get; set; }
    }
}