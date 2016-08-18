using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIWishListCreateInputInfo class.
    /// </summary>
    [DataContract]
    public class UIWishListCreateInputInfo
    {
        /// <summary>
        /// Gets or sets CustomerNumber.
        /// </summary>
        [DataMember(Name = "CustomerNumber")]
        public int CustomerNumber { get; set; }

        /// <summary>
        /// Gets or sets WishListTitle.
        /// </summary>
        [DataMember(Name = "WishListTitle")]
        public string WishListTitle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsDefaultWishList.
        /// </summary>
        [DataMember(Name = "IsDefaultWishList")]
        public bool IsDefaultWishList { get; set; }

        /// <summary>
        /// Gets or sets WishListTitleNote.
        /// </summary>
        [DataMember(Name = "WishListTitleNote")]
        public string WishListTitleNote { get; set; }
    }
}
