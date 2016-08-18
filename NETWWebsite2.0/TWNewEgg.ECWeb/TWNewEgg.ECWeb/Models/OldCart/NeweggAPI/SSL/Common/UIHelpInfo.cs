using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIHelpInfo class.
    /// </summary>
    [DataContract]
    public class UIHelpInfo
    {
        /// <summary>
        /// Gets or sets Summry.
        /// </summary>
        [DataMember(Name = "Summry")]
        public string Summry { get; set; }

        /// <summary>
        /// Gets or sets Content.
        /// </summary>
        [DataMember(Name = "Content")]
        public string Content { get; set; }
    }
}
