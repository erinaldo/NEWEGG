using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the IphoneCoremetricsRequestServer class.
    /// </summary>
    [DataContract]
    public class IphoneCoremetricsRequestServer
    {
        /// <summary>
        /// Gets or sets HttpServer.
        /// </summary>
        [DataMember(Name = "HttpServer")]
        public string HttpServer { get; set; }

        /// <summary>
        /// Gets or sets HttpsServer.
        /// </summary>
        [DataMember(Name = "HttpsServer")]
        public string HttpsServer { get; set; }
    }
}
