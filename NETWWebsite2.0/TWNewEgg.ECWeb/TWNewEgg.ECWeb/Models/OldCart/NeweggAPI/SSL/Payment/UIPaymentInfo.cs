using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Global = Newegg.Mobile.MvcApplication.Globalization.Models.Payment;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIPaymentInfo class.
    /// </summary>
    [DataContract]
    public class UIPaymentInfo
    {
        /// <summary>
        /// Gets or sets CountryCode.
        /// </summary>
        [DataMember(Name = "CountryCode")]
        public string CountryCode { get; set; }
        
        /// <summary>
        /// Gets or sets PaytermsCode.
        /// </summary>
        [DataMember(Name = "PaytermsCode")]
        public string PaytermsCode { get; set; }
        
        /// <summary>
        /// Gets or sets Payterms.
        /// </summary>
        [DataMember(Name = "Payterms")]
        [Display(ResourceType = typeof(Global.CreditCardModel), Name = "SaveAs")]
        public string Payterms { get; set; }
		
        /// <summary>
        /// Gets or sets PaytermsType.
        /// </summary>
        [DataMember(Name = "PaytermsType")]
        public string PaytermsType { get; set; }
    }
}