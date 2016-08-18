using System;
using System.Runtime.Serialization;
using Newegg.Mobile.Web.Authentication;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIShoppingLoginResultInfo class.
    /// </summary>
    [DataContract]
    public class UIShoppingLoginResultInfo : UILoginResultInfo, ICustomer
    {
        /// <summary>
        /// Gets or sets a value indicating whether HasShippingAddress.
        /// </summary>
        [DataMember(Name = "HasShippingAddress")]
        public bool HasShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether HasBillingAddress.
        /// </summary>
        [DataMember(Name = "HasBillingAddress")]
        public bool HasBillingAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether HasCreditCard.
        /// </summary>
        [DataMember(Name = "HasCreditCard")]
        public bool HasCreditCard { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsNPAAvailable.
        /// </summary>
        [DataMember(Name = "IsNPAAvailable")]
        public bool IsNPAAvailable { get; set; }

        /// <summary>
        /// Gets or sets customer name.
        /// </summary>
        public string Name
        {
            get { return this.Customer.Name; }
            set { this.Customer.Name = value; }
        }

        /// <summary>
        /// Gets or sets customer number.
        /// </summary>
        public int Number
        {
            get { return this.Customer.CustomerNumber; }
            set { this.Customer.CustomerNumber = value; }
        }

        /// <summary>
        /// Gets or sets login name.
        /// </summary>
        public string LoginName
        {
            get { return this.Customer.LoginName; }
            set { this.Customer.LoginName = value; }
        }

        /// <summary>
        /// Gets or sets zipcode.
        /// </summary>
        public string ZipCode
        {
            get { return this.Customer.ZipCode; }
            set { this.Customer.ZipCode = value; }
        }

        /// <summary>
        /// Gets or sets AuthToken.
        /// </summary>
        public string AuthToken
        {
            get { return this.Customer.AuthToken; }
            set { this.Customer.AuthToken = value; }
        }
    }
}
