using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIAnchorInfo class.
    /// </summary>
    [DataContract]
    public class UIAnchorInfo
    {
        /// <summary>
        /// Gets or sets Title.
        /// </summary>
        [DataMember(Name = "Title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets Href.
        /// </summary>
        [DataMember(Name = "Href")]
        public string Href { get; set; }
    }
}
