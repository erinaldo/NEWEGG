using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Result status.
    /// </summary>
    [DataContract]
    public enum ResultStatus
    {
        /// <summary>
        /// Gets or sets Success.
        /// </summary>
        [EnumMember]
        Success,

        /// <summary>
        /// Gets or sets Fail.
        /// </summary>
        [EnumMember]
        Fail,

        /// <summary>
        /// Gets or sets Warning.
        /// </summary>
        [EnumMember]
        Warning,

        /// <summary>
        /// Gets or sets Exception.
        /// </summary>
        [EnumMember]
        Exception,
    }
}
