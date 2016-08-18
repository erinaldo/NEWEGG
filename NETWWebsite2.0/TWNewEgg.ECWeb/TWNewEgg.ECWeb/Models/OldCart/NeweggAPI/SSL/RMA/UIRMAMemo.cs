using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIRMAMemo class.
    /// </summary>
    [DataContract]
    public class UIRMAMemo
    {
        /// <summary>
        /// Gets or sets RMANumber.
        /// </summary>
        [DataMember(Name = "RMANumber")]
        public int RMANumber { get; set; }

        /// <summary>
        /// Gets or sets EventTypeID.
        /// </summary>
        [DataMember(Name = "EventTypeID")]
        public int EventTypeID { get; set; }

        /// <summary>
        /// Gets or sets Subject.
        /// </summary>
        [DataMember(Name = "Subject")]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets Memo.
        /// </summary>
        [DataMember(Name = "Memo")]
        public string Memo { get; set; }
    }
}
