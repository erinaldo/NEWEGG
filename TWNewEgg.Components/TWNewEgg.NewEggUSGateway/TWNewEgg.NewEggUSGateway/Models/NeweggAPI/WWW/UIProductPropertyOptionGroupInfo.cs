using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIProductPropertyOptionGroupInfo class.
    /// 产品属性选项组信息.
    /// </summary>
    [DataContract]
    public class UIProductPropertyOptionGroupInfo
    {
        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        [DataMember(Name = "Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets Properties.
        /// </summary>
        [DataMember(Name = "Properties")]
        public List<UIProductPropertyOptionInfo> Properties { get; set; }
    }
}
