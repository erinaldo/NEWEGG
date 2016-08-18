using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Item group type.
    /// </summary>
    [DataContract]
    public enum UIItemGroupType
    {
        /// <summary>
        /// Gets or sets Single.
        /// </summary>
        [EnumMember]
        Single = 1,

        /// <summary>
        /// Gets or sets AMD.
        /// </summary>
        [EnumMember]
        AMD = 2,

        /// <summary>
        /// Gets or sets Combo.
        /// </summary>
        [EnumMember]
        Combo = 3,

        /// <summary>
        /// Gets or sets SNET.
        /// </summary>
        [EnumMember]
        SNET = 4,

        /// <summary>
        /// Gets or sets CellPhone.
        /// </summary>
        [EnumMember]
        CellPhone = 5,

        /// <summary>
        /// Gets or sets WarrantyOnly.
        /// </summary>
        [EnumMember]
        WarrantyOnly = 6,

        /// <summary>
        /// Gets or sets DriveSaver.
        /// </summary>
        [EnumMember]
        DriveSaver = 7,
    }
}
