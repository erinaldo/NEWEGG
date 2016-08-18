using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIServiceItemInfo class.
    /// </summary>
    [DataContract]
    public class UIServiceItemInfo
    {
        /// <summary>
        /// Gets or sets ItemNumber.
        /// </summary>
        [DataMember(Name = "ItemNumber")]
        public string ItemNumber { get; set; }
       
        /// <summary>
        /// Gets or sets a value indicating whether IsSelected.
        /// </summary>
        [DataMember(Name = "IsSelected")]
        public bool IsSelected { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        [DataMember(Name = "Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets UnitPrice.
        /// </summary>
        [DataMember(Name = "UnitPrice")]
        public string UnitPrice { get; set; }
    }
}
