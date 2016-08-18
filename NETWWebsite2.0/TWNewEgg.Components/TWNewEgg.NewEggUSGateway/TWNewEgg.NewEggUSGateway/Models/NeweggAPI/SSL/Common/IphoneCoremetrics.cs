using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newegg.Mobile.MvcApplication.Models;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the IphoneCoremetrics class.
    /// </summary>
    [DataContract]
    public class IphoneCoremetrics
    {
        /// <summary>
        /// Gets or sets ClientID.
        /// </summary>
        [DataMember(Name = "ClientID")]
        public string ClientID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Enable.
        /// </summary>
        [DataMember(Name = "Enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets Vn1.
        /// </summary>
        [DataMember(Name = "Vn1")]
        public string Vn1 { get; set; }

        /// <summary>
        /// Gets or sets Vn2.
        /// </summary>
        [DataMember(Name = "Vn2")]
        public string Vn2 { get; set; }

        /// <summary>
        /// Gets or sets RequestServer.
        /// </summary>
        [DataMember(Name = "RequestServer")]
        public IphoneCoremetricsRequestServer RequestServer { get; set; }
    }
}
