using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the AddressNavigatuib class.
    /// </summary>
    [DataContract]
    public class AddressNavigatuib
    {
        /// <summary>
        /// Gets or sets AddressID.
        /// </summary>
        [DataMember(Name = "AddressID")]
        public int AddressID { get; set; }

        /// <summary>
        /// Gets or sets HeaderTitle.
        /// </summary>
        [DataMember(Name = "HeaderTitle")]
        public string HeaderTitle { get; set; }

        /// <summary>
        /// Gets or sets BackUrl.
        /// </summary>
        [DataMember(Name = "BackUrl")]
        public string BackUrl { get; set; }

        /// <summary>
        /// Gets or sets BackText.
        /// </summary>
        [DataMember(Name = "BackText")]
        public string BackText { get; set; }

        /// <summary>
        /// Gets or sets CancelUrl.
        /// </summary>
        [DataMember(Name = "CancelUrl")]
        public string CancelUrl { get; set; }
    }
}
