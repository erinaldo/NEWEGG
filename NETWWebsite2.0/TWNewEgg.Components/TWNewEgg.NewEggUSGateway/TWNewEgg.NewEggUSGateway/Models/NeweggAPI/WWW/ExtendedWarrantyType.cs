using System.Runtime.Serialization;

namespace Newegg.Website.DomainModel.Product.ExtendedWarranty
{
    /// <summary>
    /// Initializes a new instance of the ExtendedWarrantyType class.
    /// </summary>
    [DataContract]
    public enum ExtendedWarrantyType
    {
        /// <summary>
        /// Gets enum type of ItemExtendedWarranty.
        /// </summary>
        [EnumMember]
        ItemExtendedWarranty = 0,

        /// <summary>
        /// Gets enum type of CategoryExtendedWarranty.
        /// </summary>
        [EnumMember]
        CategoryExtendedWarranty = 1,
    }
}