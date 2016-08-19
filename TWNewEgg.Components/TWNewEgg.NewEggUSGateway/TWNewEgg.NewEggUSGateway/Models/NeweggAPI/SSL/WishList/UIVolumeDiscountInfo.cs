using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIVolumeDiscountInfo class.
    /// </summary>
    [DataContract]
    public class UIVolumeDiscountInfo
    {
        /// <summary>
        /// Gets or sets MinQuantity.
        /// </summary>
        [DataMember(Name = "MinQuantity")]
        public int MinQuantity { get; set; }

        /// <summary>
        /// Gets or sets MaxQuantity.
        /// </summary>
        [DataMember(Name = "MaxQuantity")]
        public int MaxQuantity { get; set; }

        /// <summary>
        /// Gets or sets KeyQuantity.
        /// </summary>
        [DataMember(Name = "KeyQuantity")]
        public string KeyQuantity { get; set; }

        /// <summary>
        /// Gets or sets UnitPrice.
        /// </summary>
        [DataMember(Name = "UnitPrice")]
        public decimal UnitPrice { get; set; }
    }
}
