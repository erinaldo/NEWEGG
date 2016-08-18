using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UISellerRMAInfo class.
    /// </summary>
    [DataContract]
    public class UISellerRMAInfo
    {
        /// <summary>
        /// Gets or sets RefundDays.
        /// </summary>
        [DataMember(Name = "RefundDays")]
        public int RefundDays { get; set; }
        
        /// <summary>
        /// Gets or sets ReplacementDays.
        /// </summary>
        [DataMember(Name = "ReplacementDays")]
        public int ReplacementDays { get; set; }
        
        /// <summary>
        /// Gets or sets RMAPolicy.
        /// </summary>
        [DataMember(Name = "RMAPolicy")]
        public string RMAPolicy { get; set; }
        
        /// <summary>
        /// Gets or sets RMAInstruction.
        /// </summary>
        [DataMember(Name = "RMAInstruction")]
        public string RMAInstruction { get; set; }
        
        /// <summary>
        /// Gets or sets RestockingFeeRate.
        /// </summary>
        [DataMember(Name = "RestockingFeeRate")]
        public decimal RestockingFeeRate { get; set; }
        
        /// <summary>
        /// Gets or sets ContactFirstName.
        /// </summary>
        [DataMember(Name = "ContactFirstName")]
        public string ContactFirstName { get; set; }
        
        /// <summary>
        /// Gets or sets ContactLastName.
        /// </summary>
        [DataMember(Name = "ContactLastName")]
        public string ContactLastName { get; set; }
        
        /// <summary>
        /// Gets or sets Address.
        /// </summary>
        [DataMember(Name = "Address")]
        public string Address { get; set; }
        
        /// <summary>
        /// Gets or sets Address2.
        /// </summary>
        [DataMember(Name = "Address2")]
        public string Address2 { get; set; }
        
        /// <summary>
        /// Gets or sets City.
        /// </summary>
        [DataMember(Name = "City")]
        public string City { get; set; }
        
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
        
        /// <summary>
        /// Gets or sets PhoneNumber.
        /// </summary>
        [DataMember(Name = "PhoneNumber")]
        public string PhoneNumber { get; set; }
        
        /// <summary>
        /// Gets or sets CompanyCode.
        /// </summary>
        [DataMember(Name = "CompanyCode")]
        public int CompanyCode { get; set; }
        
        /// <summary>
        /// Gets or sets LanguageCode.
        /// </summary>
        [DataMember(Name = "LanguageCode")]
        public string LanguageCode { get; set; }
        
        /// <summary>
        /// Gets or sets CountryCode.
        /// </summary>
        [DataMember(Name = "CountryCode")]
        public string CountryCode { get; set; }
    }
}
