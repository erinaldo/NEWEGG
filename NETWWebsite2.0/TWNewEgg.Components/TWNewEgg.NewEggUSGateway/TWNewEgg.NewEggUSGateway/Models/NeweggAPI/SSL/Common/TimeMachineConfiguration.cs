using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the TimeMachineConfiguration class.
    /// </summary>
    [DataContract]
    public class TimeMachineConfiguration
    {
        /// <summary>
        ///  Gets or sets a value indicating whether Enabled is true or false.
        /// </summary>
        [DataMember(Name = "Enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets Url.
        /// </summary>
        [DataMember(Name = "Url")]
        public string Url { get; set; }
    }
}
