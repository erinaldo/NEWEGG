using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Product item type.
    /// </summary>
    [DataContract]
    public enum VProductItemType
    {
        /// <summary>
        /// Gets type of NormalItem.
        /// </summary>
        [EnumMember]
        NormalItem = 1,

        /// <summary>
        /// Gets type of ComboGroup.
        /// </summary>
        [EnumMember]
        ComboGroup = 2,

        /// <summary>
        /// Gets type of ComboCategory.
        /// </summary>
        [EnumMember]
        ComboCategory = 3,

        /// <summary>
        /// Gets type of ComboBundle.
        /// </summary>
        [EnumMember]
        ComboBundle = 4
    }
}
