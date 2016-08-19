using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIProhibitedInfo class.
    /// </summary>
    [DataContract]
    public class UIProhibitedInfo
    {
        /// <summary>
        /// Gets or sets WarningContent.
        /// </summary>
        [DataMember(Name = "WarningContent")]
        public string WarningContent { get; set; }

        /// <summary>
        /// Gets or sets HelpTitle.
        /// </summary>
        [DataMember(Name = "HelpTitle")]
        public string HelpTitle { get; set; }

        /// <summary>
        /// Gets or sets HelpContent.
        /// </summary>
        [DataMember(Name = "HelpContent")]
        public string HelpContent { get; set; }
    }
}
