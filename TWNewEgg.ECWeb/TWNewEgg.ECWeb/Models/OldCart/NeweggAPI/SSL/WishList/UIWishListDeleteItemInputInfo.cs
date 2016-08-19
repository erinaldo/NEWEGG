using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIWishListDeleteItemInputInfo class.
    /// </summary>
    [DataContract]
    public class UIWishListDeleteItemInputInfo
    {
        /// <summary>
        /// Gets or sets WishListNumber.
        /// </summary>
        [DataMember(Name = "WishListNumber")]
        public int WishListNumber { get; set; }

        /// <summary>
        /// Gets or sets ItemNumbers.
        /// </summary>
        [DataMember(Name = "ItemNumbers")]
        public string ItemNumbers { get; set; }
    }
}
