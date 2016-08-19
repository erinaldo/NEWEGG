using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIPushDeviceType class.
    /// </summary>
    [DataContract]
    public enum UIPushDeviceType
    {
        /// <summary>
        /// Gets or sets ALL.
        /// </summary>
        [EnumMember]
        ALL,

        /// <summary>
        /// Gets or sets WPN.
        /// </summary>
        [EnumMember]
        WPN,

        /// <summary>
        /// Gets or sets IPN.
        /// </summary>
        [EnumMember]
        IPN,

        /// <summary>
        /// Gets or sets IPD.
        /// </summary>
        [EnumMember]
        IPD,

        /// <summary>
        /// Gets or sets ARD.
        /// </summary>
        [EnumMember]
        ARD
    }
}
