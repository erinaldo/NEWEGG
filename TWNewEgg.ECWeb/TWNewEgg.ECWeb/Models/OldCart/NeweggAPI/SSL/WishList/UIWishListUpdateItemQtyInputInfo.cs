using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIWishListUpdateItemQtyInputInfo class.
    /// </summary>
    [DataContract]
    public class UIWishListUpdateItemQtyInputInfo
    {
        /// <summary>
        /// Gets or sets ItemNumber.
        /// </summary>
        [DataMember(Name = "ItemNumber")]
        public string ItemNumber { get; set; }

        /// <summary>
        /// Gets or sets ItemQty.
        /// </summary>
        [DataMember(Name = "ItemQty")]
        public int ItemQty { get; set; }

        /// <summary>
        /// Gets or sets WishListNumber.
        /// </summary>
        [DataMember(Name = "WishListNumber")]
        public int WishListNumber { get; set; }
    }
}
