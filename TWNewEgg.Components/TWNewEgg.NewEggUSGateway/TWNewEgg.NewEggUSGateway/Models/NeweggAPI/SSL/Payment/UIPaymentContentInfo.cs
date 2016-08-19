using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIPaymentContentInfo class.
    /// </summary>
    [DataContract]
    public class UIPaymentContentInfo
    {
        /// <summary>
        /// Initializes a new instance of the UIPaymentContentInfo class.
        /// </summary>
        public UIPaymentContentInfo()
        {
            this.Type = UIPaymentType.CreditCard;
        }

        /// <summary>
        /// Gets or sets Type.
        /// </summary>
        [DataMember(Name = "Type")]
        public UIPaymentType Type { get; set; }

        /// <summary>
        /// Gets or sets CreditCard.
        /// </summary>
        [DataMember(Name = "CreditCard")]
        public UICreditCardPaymentInfo CreditCard { get; set; }
    }
}