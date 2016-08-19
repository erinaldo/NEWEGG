using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIAutoNotifyInfo class.
    /// </summary>
    [DataContract]
    public class UIAutoNotifyInfo
    {
        /// <summary>
        /// Gets or sets EmailAddress.
        /// </summary>
        [DataMember(Name = "EmailAddress")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets Notes.
        /// </summary>
        [DataMember(Name = "Notes")]
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets ItemNumber.
        /// </summary>
        [DataMember(Name = "ItemNumber")]
        public string ItemNumber { get; set; }

        /// <summary>
        /// Gets or sets ItemNumberList.
        /// </summary>
        [DataMember(Name = "ItemNumberList")]
        public string ItemNumberList { get; set; }
    }
}
