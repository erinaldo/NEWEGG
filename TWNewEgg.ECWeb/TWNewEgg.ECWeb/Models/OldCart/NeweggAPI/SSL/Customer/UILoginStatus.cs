using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UILoginStatus class.
    /// </summary>
    [DataContract]
    public enum UILoginStatus
    {
        /// <summary>
        /// Gets or sets Failure.
        /// </summary>
        [EnumMember]
        Failure = 0,

        /// <summary>
        /// Gets or sets Success.
        /// </summary>
        [EnumMember]
        Success = 1
    }
}