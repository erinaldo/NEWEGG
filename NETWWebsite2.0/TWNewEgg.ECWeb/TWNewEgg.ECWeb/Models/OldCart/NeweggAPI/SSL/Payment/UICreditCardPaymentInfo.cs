using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Global = Newegg.Mobile.MvcApplication.Globalization.Models.Payment;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UICreditCardPaymentInfo class.
    /// </summary>
    [DataContract]
    public class UICreditCardPaymentInfo : UIPaymentInfo
    {
        /// <summary>
        /// Initializes a new instance of the UICreditCardPaymentInfo class.
        /// </summary>
        public UICreditCardPaymentInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the UICreditCardPaymentInfo class.
        /// </summary>
        /// <param name="paytermsCode">Pay terms code.</param>
        public UICreditCardPaymentInfo(string paytermsCode)
        {
            this.PaytermsCode = paytermsCode;
        }

        /// <summary>
        /// Gets or sets TransactionNumber.
        /// </summary>
        [DataMember(Name = "TransactionNumber")]
        public int TransactionNumber { get; set; }
        
        /// <summary>
        /// Gets or sets EncryptionCardNumber.
        /// </summary>
        [DataMember(Name = "EncryptionCardNumber")]
        public string EncryptionCardNumber { get; set; }
        
        /// <summary>
        /// Gets or sets EncryptionCVV2Code.
        /// </summary>
        [DataMember(Name = "EncryptionCVV2Code")]
        public string EncryptionCVV2Code { get; set; }
        
        /// <summary>
        /// Gets or sets IngianCardNumber.
        /// </summary>
        [DataMember(Name = "IngianCardNumber")]
        public string IngianCardNumber { get; set; }
        
        /// <summary>
        /// Gets or sets IngrianCVV2Code.
        /// </summary>
        [DataMember(Name = "IngrianCVV2Code")]
        public string IngrianCVV2Code { get; set; }
        
        /// <summary>
        /// Gets or sets HashCardNumber.
        /// </summary>
        [DataMember(Name = "HashCardNumber")]
        public string HashCardNumber { get; set; }
        
        /// <summary>
        /// Gets or sets CardNumber.
        /// </summary>
        [DataMember(Name = "CardNumber")]
        [Display(ResourceType = typeof(Global.CreditCardModel), Name = "Card")]
        public string CardNumber { get; set; }
        
        /// <summary>
        /// Gets or sets CVV2Code.
        /// </summary>
        [DataMember(Name = "CVV2Code")]
        public string CVV2Code { get; set; }
        
        /// <summary>
        /// Gets or sets ExpirationDate.
        /// </summary>
        [DataMember(Name = "ExpirationDate")]
        [Display(ResourceType = typeof(Global.CreditCardModel), Name = "ExpireDate")]
        public string ExpirationDate { get; set; }
        
        /// <summary>
        /// Gets or sets BankPhone.
        /// </summary>
        [DataMember(Name = "BankPhone")]
        public string BankPhone { get; set; }
        
        /// <summary>
        /// Gets or sets HolderName.
        /// </summary>
        [DataMember(Name = "HolderName")]
        [Required]
        [Display(ResourceType = typeof(Global.CreditCardModel), Name = "CardHolder")]
        public string HolderName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsDefault.
        /// </summary>
        [DataMember(Name = "IsDefault")]
        [Display(ResourceType = typeof(Global.CreditCardModel), Name = "SetDefault")]
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets MaintainType.
        /// </summary>
        [DataMember(Name = "MaintainType")]
        public string MaintainType { get; set; }

        /// <summary>
        /// Gets Expiration Month.
        /// </summary>
        [Required]
        public string ExpirationMonth
        {
            get
            {
                return this.ExpirationDate.Split('/')[0];
            }
        }

        /// <summary>
        /// Gets Expiration Year.
        /// </summary>
        [Required]
        public string ExpirationYear
        {
            get
            {
                return System.DateTime.Now.Year.ToString().Substring(0, 2) + this.ExpirationDate.Split('/')[1];
            }
        }
    }
}