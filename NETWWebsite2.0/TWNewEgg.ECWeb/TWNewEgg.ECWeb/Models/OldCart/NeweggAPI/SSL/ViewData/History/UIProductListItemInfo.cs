using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIProductListItemInfo class.
    /// </summary>
    [DataContract]
    public class UIProductListItemInfo
    {
        /// <summary>
        /// Gets or sets ItemNumber.
        /// </summary>
        [DataMember(Name = "ItemNumber")]
        public string ItemNumber { get; set; }

        /// <summary>
        /// Gets or sets Image.
        /// </summary>
        [DataMember(Name = "Image")]
        public UIImageInfo Image { get; set; }

        /// <summary>
        /// Gets or sets FinalPrice.
        /// </summary>
        [DataMember(Name = "FinalPrice")]
        public string FinalPrice { get; set; }

        [DataMember(Name = "Title")]
        public string Title { get; set; }
    }
}
