using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIWarrantyGroupInfo class.
    /// </summary>
    [DataContract]
    public class UIWarrantyGroupInfo
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
        /// Gets or sets WarrantyItemList.
        /// </summary>
        [DataMember(Name = "WarrantyItemList")]
        public List<UIWarrantyItemInfo> WarrantyItemList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsSelect.
        /// </summary>
        [DataMember(Name = "IsSelect")]
        public bool IsSelect { get; set; }
    }
}
