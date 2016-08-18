using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIServerInfo class.
    /// </summary>
    [DataContract]
    public class UIServerInfo
    {
        /// <summary>
        /// Gets or sets ServerName.
        /// </summary>
        [DataMember(Name = "ServerName")]
        public string ServerName { get; set; }
    }
}
