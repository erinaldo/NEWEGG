using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the CategoryType class.
    /// </summary>
    [DataContract]
    public enum CategoryType
    {
        /// <summary>
        /// Result Category.
        /// </summary>
        [EnumMember]
        Category = 0,

        /// <summary>
        /// Result SubCategory.
        /// </summary>
        [EnumMember]
        SubCategory = 1,
    }
}
