using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIRMAHistoryDetailMemo class.
    /// </summary>
    [DataContract]
    public class UIRMAHistoryDetailMemo
    {
        /// <summary>
        /// Gets or sets Content.
        /// </summary>
        [DataMember(Name = "Content")]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets Subject.
        /// </summary>
        [DataMember(Name = "Subject")]
        public string Subject { get; set; }
    }
}
