using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UISellerItemPropertyInfo class.
    /// </summary>
    [DataContract]
    public class UISellerItemPropertyInfo
    {
        /// <summary>
        /// Gets or sets ParentItem.
        /// </summary>
        [DataMember(Name = "ParentItem")]
        public string ParentItem { get; set; }

        /// <summary>
        /// Gets or sets PropertyId.
        /// </summary>
        [DataMember(Name = "PropertyId")]
        public int PropertyId { get; set; }

        /// <summary>
        /// Gets or sets PropertyDescription.
        /// </summary>
        [DataMember(Name = "PropertyDescription")]
        public string PropertyDescription { get; set; }

        /// <summary>
        /// Gets or sets PropertyValue.
        /// </summary>
        [DataMember(Name = "PropertyValue")]
        public UISellerPropertyValueInfo PropertyValue { get; set; }
    }
}
