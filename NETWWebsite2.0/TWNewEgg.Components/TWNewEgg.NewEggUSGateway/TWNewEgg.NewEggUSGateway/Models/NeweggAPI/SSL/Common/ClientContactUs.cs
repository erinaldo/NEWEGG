using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the ClientContactUs class.
    /// </summary>
    [DataContract]
    public class ClientContactUs
    {
        /// <summary>
        /// Gets or sets Company.
        /// </summary>
        [DataMember(Name = "Company")]
        public string Company { get; set; }

        /// <summary>
        /// Gets or sets Address.
        /// </summary>
        [DataMember(Name = "Address")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets State.
        /// </summary>
        [DataMember(Name = "State")]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets City.
        /// </summary>
        [DataMember(Name = "City")]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets ZipCode.
        /// </summary>
        [DataMember(Name = "ZipCode")]
        public string ZipCode { get; set; }

        /// <summary>
        /// Gets or sets TollFree.
        /// </summary>
        [DataMember(Name = "TollFree")]
        public string TollFree { get; set; }

        /// <summary>
        /// Gets or sets Fax.
        /// </summary>
        [DataMember(Name = "Fax")]
        public string Fax { get; set; }

        /// <summary>
        /// Gets or sets PhoneHours.
        /// </summary>
        [DataMember(Name = "PhoneHours")]
        public string PhoneHours { get; set; }
    }
}
