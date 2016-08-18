using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIExtendedWarrantyGroupInfo class.
    /// </summary>
    [DataContract]
    public class UIExtendedWarrantyGroupInfo
    {
        /// <summary>
        /// Gets or sets GroupID.
        /// </summary>
        [DataMember(Name = "GroupID")]
        public int GroupID { get; set; }

        /// <summary>
        /// Gets or sets GroupName.
        /// </summary>
        [DataMember(Name = "GroupName")]
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets GroupDescription.
        /// </summary>
        [DataMember(Name = "GroupDescription")]
        public string GroupDescription { get; set; }

        /// <summary>
        /// Gets or sets Summary.
        /// </summary>
        [DataMember(Name = "Summary")]
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets ExtendedWarrantyInfoList.
        /// </summary>
        [DataMember(Name = "ExtendedWarrantyInfoList")]
        public List<UIExtendedWarrantyInfo> ExtendedWarrantyInfoList { get; set; }
    }
}
