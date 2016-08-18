using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIPaypalEntity class.
    /// </summary>
    [DataContract]
    public class UIPaypalEntity
    {
        /// <summary>
        /// Gets or sets CancelUrl.
        /// </summary>
        [DataMember(Name = "CancelUrl")]
        public string CancelUrl { get; set; }
        
        /// <summary>
        /// Gets or sets CountryCode.
        /// </summary>
        [DataMember(Name = "CountryCode")]
        public string CountryCode { get; set; }
        
        /// <summary>
        /// Gets or sets CurrencyCode.
        /// </summary>
        [DataMember(Name = "CurrencyCode")]
        public string CurrencyCode { get; set; }
        
        /// <summary>
        /// Gets or sets CurrencyExchangeRate.
        /// </summary>
        [DataMember(Name = "CurrencyExchangeRate")]
        public decimal CurrencyExchangeRate { get; set; }
        
        /// <summary>
        /// Gets or sets CurrencySOAmount.
        /// </summary>
        [DataMember(Name = "CurrencySOAmount")]
        public decimal CurrencySOAmount { get; set; }

        /// <summary>
        /// Gets or sets NeweggLogoImage.
        /// </summary>
        [DataMember(Name = "NeweggLogoImage")]
        public string NeweggLogoImage { get; set; }

        /// <summary>
        /// Gets or sets PayerID.
        /// </summary>
        [DataMember(Name = "PayerID")]
        public string PayerID { get; set; }

        /// <summary>
        /// Gets or sets PaypalAddress.
        /// </summary>
        [DataMember(Name = "PaypalAddress")]
        public UICustomerShipping PaypalAddress { get; set; }

        /// <summary>
        /// Gets or sets PaypalEmail.
        /// </summary>
        [DataMember(Name = "PaypalEmail")]
        public string PaypalEmail { get; set; }

        /// <summary>
        /// Gets or sets RequestString.
        /// </summary>
        [DataMember(Name = "RequestString")]
        public string RequestString { get; set; }

        /// <summary>
        /// Gets or sets ReturnUrl.
        /// </summary>
        [DataMember(Name = "ReturnUrl")]
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Gets or sets SessionID.
        /// </summary>
        [DataMember(Name = "SessionID")]
        public string SessionID { get; set; }

        /// <summary>
        /// Gets or sets SOAmount.
        /// </summary>
        [DataMember(Name = "SOAmount")]
        public decimal SOAmount { get; set; }

        /// <summary>
        /// Gets or sets Status.
        /// </summary>
        [DataMember(Name = "Status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets Step.
        /// </summary>
        [DataMember(Name = "Step")]
        public int Step { get; set; }
        
        /// <summary>
        /// Gets or sets Token.
        /// </summary>
        [DataMember(Name = "Token")]
        public string Token { get; set; }
    }
}
