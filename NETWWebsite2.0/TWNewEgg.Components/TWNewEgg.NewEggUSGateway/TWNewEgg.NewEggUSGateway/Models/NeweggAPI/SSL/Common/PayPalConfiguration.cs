using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the PayPalConfiguration class.
    /// </summary>
    [DataContract]
    public class PayPalConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether Enable.
        /// </summary>
        [DataMember(Name = "Enable")]
        public bool Enable { get; set; }
    }
}
