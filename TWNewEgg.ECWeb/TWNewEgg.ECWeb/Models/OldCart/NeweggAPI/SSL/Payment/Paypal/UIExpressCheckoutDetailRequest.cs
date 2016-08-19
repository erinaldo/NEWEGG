using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIExpressCheckoutDetailRequest class.
    /// </summary>
    [DataContract]
    public class UIExpressCheckoutDetailRequest
    {
        /// <summary>
        /// Gets or sets CustomerNumber.
        /// </summary>
        [DataMember(Name = "CustomerNumber")]
        public int CustomerNumber { get; set; }

        /// <summary>
        /// Gets or sets SessionID.
        /// </summary>
        [DataMember(Name = "SessionID")]
        public string SessionID { get; set; }

        /// <summary>
        /// Gets or sets Token.
        /// </summary>
        [DataMember(Name = "Token")]
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets PayerId.
        /// </summary>
        [DataMember(Name = "PayerId")]
        public string PayerId { get; set; }
    }
}