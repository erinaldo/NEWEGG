using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIPromotionModuleInfo class.
    /// </summary>
    [DataContract]
    public class UIPromotionModuleInfo
    {
        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        [DataMember(Name = "Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets PromotionItems.
        /// </summary>
        [DataMember(Name = "PromotionItems")]
        public List<UIProductBasicInfo> PromotionItems { get; set; }
    }
}
