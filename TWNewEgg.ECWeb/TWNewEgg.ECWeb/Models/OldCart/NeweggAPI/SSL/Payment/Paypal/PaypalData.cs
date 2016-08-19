using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Input Paypal Data.
    /// </summary>
    public class PaypalData
    {
        /// <summary>
        /// Gets or sets sessionID.
        /// </summary>
        public string SessionID { get; set; }

        /// <summary>
        /// Gets or sets CancelUrl.
        /// </summary>
        public string CancelUrl { get; set; }

        /// <summary>
        /// Gets or sets ReturnUrl.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Gets or sets PaypalSource.
        /// </summary>
        public string PaypalSource { get; set; }
    }
}
