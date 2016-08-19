using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UISellerPropertyValueInfo class.
    /// </summary>
    [DataContract]
    public class UISellerPropertyValueInfo
    {
        /// <summary>
        /// Gets or sets ValueId.
        /// </summary>
        [DataMember(Name = "ValueId")]
        public int ValueId { get; set; }

        /// <summary>
        /// Gets or sets ValueDescription.
        /// </summary>
        [DataMember(Name = "ValueDescription")]
        public string ValueDescription { get; set; }
    }
}
