using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the StoreType enum.
    /// </summary>
    [DataContract]
    public enum StoreType
    {
        /// <summary>
        /// For internal use only. do not serialize.
        /// </summary>
        [EnumMember]
        CategoryStore = 1,

        /// <summary>
        /// Result SubCategoryStore.
        /// </summary>
        [EnumMember]
        SubCategoryStore = 2,

        /// <summary>
        /// Result BrandStore.
        /// </summary>
        [EnumMember]
        BrandStore = 3,

        /// <summary>
        /// Result TabStore.
        /// </summary>
        [EnumMember]
        TabStore = 4,

        /// <summary>
        /// Result DVDCategoryStore.
        /// </summary>
        [EnumMember]
        DVDCategoryStore = 5,

        /// <summary>
        /// Result BrandSubCategoryStore.
        /// </summary>
        [EnumMember]
        BrandSubCategoryStore = 6,

        /// <summary>
        /// Result ComboDealsStore.
        /// </summary>
        [EnumMember]
        ComboDealsStore = 7,

        /// <summary>
        /// Result NonStore.
        /// </summary>
        [EnumMember]
        NonStore = -1,

        /// <summary>
        /// Result InvalidStoreType.
        /// </summary>
        [EnumMember]
        InvalidStoreType = -2,

        /// <summary>
        /// Result RecertifiedStore.
        /// </summary>
        [EnumMember]
        RecertifiedStore = 8,

        /// <summary>
        /// Result ComboBundleStore.
        /// </summary>
        [EnumMember]
        ComboBundleStore = 9,

        /// <summary>
        /// Result SimplexityBrandStore.
        /// </summary>
        [EnumMember]
        SimplexityBrandStore = 10,

        /// <summary>
        /// Result Homepage2011.
        /// </summary>
        [EnumMember]
        Homepage2011 = 11,

        /// <summary>
        /// Result SellerStore.
        /// </summary>
        [EnumMember]
        SellerStore = 12,
    }
}
