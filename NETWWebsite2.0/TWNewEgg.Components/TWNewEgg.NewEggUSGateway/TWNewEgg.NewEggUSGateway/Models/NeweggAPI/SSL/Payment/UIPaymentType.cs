using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIPaymentType class.
    /// </summary>
    [DataContract]
    public enum UIPaymentType
    {
        /// <summary>
        /// Credit card.
        /// </summary>
        [EnumMember]
        CreditCard
    }
}