using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIProductSpecificationPairInfo class.
    /// </summary>
    [DataContract]
    public class UIProductSpecificationPairInfo
    {
        /// <summary>
        /// Gets or sets Key.
        /// </summary>
        [DataMember(Name = "Key")]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets Value.
        /// </summary>
        [DataMember(Name = "Value")]
        public string Value { get; set; }
    }
}
