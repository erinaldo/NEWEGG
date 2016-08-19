using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIReturnPolicyInfo class.
    /// </summary>
    [DataContract]
    public class UIReturnPolicyInfo
    {
        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        [DataMember(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets ID.
        /// </summary>
        [DataMember(Name = "ID")]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets HtmlContent.
        /// </summary>
        [DataMember(Name = "HtmlContent")]
        public string HtmlContent { get; set; }
    }
}
