using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Sort order type.
    /// </summary>
    [DataContract]
    public enum SortOrderType
    {
        /// <summary>
        /// Gets or sets ASC.
        /// </summary>
        [EnumMember]
        ASC,

        /// <summary>
        /// Gets or sets DESC.
        /// </summary>
        [EnumMember]
        DESC,
    }
}
