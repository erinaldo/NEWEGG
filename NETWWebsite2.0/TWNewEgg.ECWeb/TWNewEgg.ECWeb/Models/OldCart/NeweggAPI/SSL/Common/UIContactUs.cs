using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Contact us model.
    /// </summary>
    public class UIContactUs
    {
        /// <summary>
        /// Gets or sets Company.
        /// </summary>
        [DataMember]
        public string Company { get; set; }

        /// <summary>
        /// Gets or sets Address.
        /// </summary>
        [DataMember]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets State.
        /// </summary>
        [DataMember]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets City.
        /// </summary>
        [DataMember]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets ZipCode.
        /// </summary>
        [DataMember]
        public string ZipCode { get; set; }

        /// <summary>
        /// Gets or sets TollFree.
        /// </summary>
        [DataMember]
        public string TollFree { get; set; }

        /// <summary>
        /// Gets or sets Fax.
        /// </summary>
        [DataMember]
        public string Fax { get; set; }

        /// <summary>
        /// Gets or sets PhoneHours.
        /// </summary>
        [DataMember]
        public string PhoneHours { get; set; }
    }
}
