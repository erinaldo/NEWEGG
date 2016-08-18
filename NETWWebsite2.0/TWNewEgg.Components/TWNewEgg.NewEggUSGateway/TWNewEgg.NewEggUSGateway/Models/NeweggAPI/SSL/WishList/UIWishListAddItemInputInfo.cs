using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIWishListAddItemInputInfo class.
    /// </summary>
    [DataContract]
    public class UIWishListAddItemInputInfo : UIWishListBaseInputInfo
    {
        /// <summary>
        /// Gets or sets ItemNumberList.
        /// </summary>
        [DataMember(Name = "ItemNumberList")]
        public string ItemNumberList { get; set; }

        /// <summary>
        /// Gets or sets ItemQtyList.
        /// </summary>
        [DataMember(Name = "ItemQtyList")]
        public string ItemQtyList { get; set; }
    }
}
