using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIExtendedWarrantyContent class.
    /// </summary>
    [DataContract]
    public class UIExtendedWarrantyContent
    {
        /// <summary>
        /// Gets or sets ItemNumber.
        /// </summary>
        [DataMember(Name = "ItemNumber")]
        public string ItemNumber { get; set; }

        /// <summary>
        /// Gets or sets BrandId.
        /// </summary>
        [DataMember(Name = "BrandId")]
        public int BrandId { get; set; }

        /// <summary>
        /// Gets or sets SubcategoryId.
        /// </summary>
        [DataMember(Name = "SubcategoryId")]
        public int SubcategoryId { get; set; }

        /// <summary>
        /// Gets or sets MessageType.
        /// </summary>
        [DataMember(Name = "MessageType")]
        public string MessageType { get; set; }

        /// <summary>
        /// Gets or sets MessageTitle.
        /// </summary>
        [DataMember(Name = "MessageTitle")]
        public string MessageTitle { get; set; }

        /// <summary>
        /// Gets or sets MessageDescription.
        /// </summary>
        [DataMember(Name = "MessageDescription")]
        public string MessageDescription { get; set; }

        /// <summary>
        /// Gets or sets GroupInfo.
        /// </summary>
        [DataMember(Name = "GroupInfo")]
        public List<UIExtendedWarrantyGroupInfo> GroupInfo { get; set; }
    }
}
