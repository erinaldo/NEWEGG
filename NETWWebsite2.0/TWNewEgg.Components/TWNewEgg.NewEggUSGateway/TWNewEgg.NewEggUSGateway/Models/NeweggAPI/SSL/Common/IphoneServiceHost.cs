using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the IphoneServiceHost class.
    /// </summary>
    [DataContract]
    public class IphoneServiceHost
    {
        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        [DataMember(Name = "Name")]
        public string Name { get; set; }
    }
}
