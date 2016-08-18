using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIWarrantyItemInfo class.
    /// </summary>
    [DataContract]
    public class UIWarrantyItemInfo
    {
        /// <summary>
        /// Gets or sets ItemNumber.
        /// </summary>
        [DataMember(Name = "ItemNumber")]
        public string ItemNumber { get; set; }

        /// <summary>
        /// Gets or sets Year.
        /// </summary>
        [DataMember(Name = "Year")]
        public int Year { get; set; }

        /// <summary>
        /// Gets or sets OriginalPrice.
        /// </summary>
        [DataMember(Name = "OriginalPrice")]
        public string OriginalPrice { get; set; }

        /// <summary>
        /// Gets or sets UnitPrice.
        /// </summary>
        [DataMember(Name = "UnitPrice")]
        public string UnitPrice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether isSelect.
        /// </summary>
        [DataMember(Name = "isSelect")]
        public bool IsSelect { get; set; }
    }
}
