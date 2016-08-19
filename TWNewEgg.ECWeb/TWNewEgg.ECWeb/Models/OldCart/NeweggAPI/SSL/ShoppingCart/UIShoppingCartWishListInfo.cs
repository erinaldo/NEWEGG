using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIWishListInfo class.
    /// </summary>
    [DataContract]
    public class UIWishListInfo
    {
        /// <summary>
        /// Gets or sets WishListNumber.
        /// </summary>
        [DataMember(Name = "WishListNumber")]
        public int WishListNumber { get; set; }

        /// <summary>
        /// Gets or sets WishListTitle.
        /// </summary>
        [DataMember(Name = "WishListTitle")]
        public string WishListTitle { get; set; }
    }
}
