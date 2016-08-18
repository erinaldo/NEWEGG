using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UICustomerShippingInfo class.
    /// </summary>
    [DataContract]
    public class UICustomerShippingInfo
    {
        /// <summary>
        /// Gets or sets Address1.
        /// </summary>
        [DataMember(Name = "Address1")]
        public string Address1 { get; set; }

        /// <summary>
        /// Gets or sets Address2.
        /// </summary>
        [DataMember(Name = "Address2")]
        public string Address2 { get; set; }

        /// <summary>
        /// Gets or sets AddressFlag.
        /// </summary>
        [DataMember(Name = "AddressFlag")]
        public string AddressFlag { get; set; }

        /// <summary>
        /// Gets or sets AddressLabel.
        /// </summary>
        [DataMember(Name = "AddressLabel")]
        public string AddressLabel { get; set; }

        /// <summary>
        /// Gets or sets AddressSource.
        /// </summary>
        [DataMember(Name = "AddressSource")]
        public string AddressSource { get; set; }

        /// <summary>
        /// Gets or sets AddressType.
        /// </summary>
        [DataMember(Name = "AddressType")]
        public UIAddressType AddressType { get; set; }

        /// <summary>
        /// Gets or sets AddressVerified.
        /// </summary>
        [DataMember(Name = "AddressVerified")]
        public UIQASAddressVerify AddressVerified { get; set; }

        /// <summary>
        /// Gets or sets City.
        /// </summary>
        [DataMember(Name = "City")]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets CompanyName.
        /// </summary>
        [DataMember(Name = "CompanyName")]
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets ContactWith.
        /// </summary>
        [DataMember(Name = "ContactWith")]
        public string ContactWith { get; set; }

        /// <summary>
        /// Gets or sets Country.
        /// </summary>
        [DataMember(Name = "Country")]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets County.
        /// </summary>
        [DataMember(Name = "County")]
        public int County { get; set; }

        /// <summary>
        /// Gets or sets DefaultSign.
        /// </summary>
        [DataMember(Name = "DefaultSign")]
        public string DefaultSign { get; set; }

        /// <summary>
        /// Gets or sets EditUser.
        /// </summary>
        [DataMember(Name = "EditUser")]
        public string EditUser { get; set; }

        /// <summary>
        /// Gets or sets Fax.
        /// </summary>
        [DataMember(Name = "Fax")]
        public string Fax { get; set; }

        /// <summary>
        /// Gets or sets InternalAddressType.
        /// </summary>
        [DataMember(Name = "InternalAddressType")]
        public int InternalAddressType { get; set; }

        /// <summary>
        /// Gets or sets InternalIsDisabled.
        /// </summary>
        [DataMember(Name = "InternalIsDisabled")]
        public int InternalIsDisabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsDefaultAddress.
        /// </summary>
        [DataMember(Name = "IsDefaultAddress")]
        public bool IsDefaultAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsDisabled.
        /// </summary>
        [DataMember(Name = "IsDisabled")]
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Gets or sets LanguageCode.
        /// </summary>
        [DataMember(Name = "LanguageCode")]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Gets or sets Number.
        /// </summary>
        [DataMember(Name = "Number")]
        public int Number { get; set; }

        /// <summary>
        /// Gets or sets OfficePhone.
        /// </summary>
        [DataMember(Name = "OfficePhone")]
        public string OfficePhone { get; set; }

        /// <summary>
        /// Gets or sets Phone.
        /// </summary>
        [DataMember(Name = "Phone")]
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets SAPTransactionNumber.
        /// </summary>
        [DataMember(Name = "SAPTransactionNumber")]
        public int SAPTransactionNumber { get; set; }

        /// <summary>
        /// Gets or sets ShippingContactWith.
        /// </summary>
        [DataMember(Name = "ShippingContactWith")]
        public string ShippingContactWith { get; set; }

        /// <summary>
        /// Gets or sets State.
        /// </summary>
        [DataMember(Name = "State")]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets ZipCode.
        /// </summary>
        [DataMember(Name = "ZipCode")]
        public string ZipCode { get; set; }
    }
}
