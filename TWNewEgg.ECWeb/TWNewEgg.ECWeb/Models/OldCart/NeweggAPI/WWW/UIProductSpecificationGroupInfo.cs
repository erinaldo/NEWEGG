using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIProductSpecificationGroupInfo class.
    /// </summary>
    [DataContract]
    public class UIProductSpecificationGroupInfo
    {
        /// <summary>
        /// Gets or sets GroupName.
        /// </summary>
        [DataMember(Name = "GroupName")]
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets SpecificationPairList.
        /// </summary>
        [DataMember(Name = "SpecificationPairList")]
        public List<UIProductSpecificationPairInfo> SpecificationPairList { get; set; }
    }
}
