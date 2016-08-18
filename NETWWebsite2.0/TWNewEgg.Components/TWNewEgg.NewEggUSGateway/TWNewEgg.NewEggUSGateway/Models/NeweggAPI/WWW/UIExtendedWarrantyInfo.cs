using System;
using System.Runtime.Serialization;
using Newegg.Website.DomainModel.Product.ExtendedWarranty;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIExtendedWarrantyInfo class.
    /// </summary>
    [DataContract]
    public class UIExtendedWarrantyInfo
    {
        /// <summary>
        /// Gets or sets ItemNumber.
        /// </summary>
        [DataMember(Name = "ItemNumber")]
        public string ItemNumber { get; set; }

        /// <summary>
        /// Gets or sets ItemPrice.
        /// </summary>
        [DataMember(Name = "ItemPrice")]
        public decimal ItemPrice { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        [DataMember(Name = "Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets Years.
        /// </summary>
        [DataMember(Name = "Years")]
        public int Years { get; set; }

        /// <summary>
        /// Gets or sets ComboId.
        /// </summary>
        [DataMember(Name = "ComboId")]
        public int ComboId { get; set; }

        /// <summary>
        /// Gets or sets PreSelectedMark.
        /// </summary>
        [DataMember(Name = "PreSelectedMark")]
        public string PreSelectedMark { get; set; }

        /// <summary>
        /// Gets or sets TotalDiscountAmount.
        /// </summary>
        [DataMember(Name = "TotalDiscountAmount")]
        public decimal TotalDiscountAmount { get; set; }

        /// <summary>
        /// Gets or sets SpecialPrice.
        /// </summary>
        [DataMember(Name = "SpecialPrice")]
        public decimal SpecialPrice { get; set; }

        /// <summary>
        /// Gets or sets InternalType.
        /// </summary>
        [DataMember(Name = "InternalType")]
        public int InternalType { get; set; }

        /// <summary>
        /// Gets or sets ExtendedWarrantyType.
        /// </summary>
        [DataMember(Name = "ExtendedWarrantyType")]
        public ExtendedWarrantyType ExtendedWarrantyType { get; set; }

        /// <summary>
        /// Gets or sets SpecialType.
        /// </summary>
        [DataMember(Name = "SpecialType")]
        public string SpecialType { get; set; }

        /// <summary>
        /// Gets or sets GroupID.
        /// </summary>
        [DataMember(Name = "GroupID")]
        public int GroupID { get; set; }

        /// <summary>
        /// Gets or sets GroupName.
        /// </summary>
        [DataMember(Name = "GroupName")]
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets GroupDescription.
        /// </summary>
        [DataMember(Name = "GroupDescription")]
        public string GroupDescription { get; set; }

        /// <summary>
        /// Gets or sets Summary.
        /// </summary>
        [DataMember(Name = "Summary")]
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets FinalPrice.
        /// </summary>
        [DataMember(Name = "FinalPrice")]
        public decimal FinalPrice { get; set; }

        /// <summary>
        /// Gets or sets OriginalPrice.
        /// </summary>
        [DataMember(Name = "OriginalPrice")]
        public decimal OriginalPrice { get; set; }
    }
}
