using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Order item type.
    /// </summary>
    [DataContract]
    public enum UIOrderItemType
    {
        /// <summary>
        /// Gets or sets Normal.
        /// </summary>
        [EnumMember]
        Normal = 1,

        /// <summary>
        /// Gets or sets Software.
        /// </summary>
        [EnumMember]
        Software = 2,

        /// <summary>
        /// Gets or sets DVD.
        /// </summary>
        [EnumMember]
        DVD = 3,

        /// <summary>
        /// Gets or sets AIT.
        /// </summary>
        [EnumMember]
        AIT = 4,

        /// <summary>
        /// Gets or sets CellPhone.
        /// </summary>
        [EnumMember]
        CellPhone = 5,

        /// <summary>
        /// Gets or sets Restricted.
        /// </summary>
        [EnumMember]
        Restricted = 6,

        /// <summary>
        /// Gets or sets InstallerNet.
        /// </summary>
        [EnumMember]
        InstallerNet = 7,

        /// <summary>
        /// Gets or sets SNET.
        /// </summary>
        [EnumMember]
        SNET = 8,

        /// <summary>
        /// Gets or sets TechSupport.
        /// </summary>
        [EnumMember]
        TechSupport = 9,
    }
}
