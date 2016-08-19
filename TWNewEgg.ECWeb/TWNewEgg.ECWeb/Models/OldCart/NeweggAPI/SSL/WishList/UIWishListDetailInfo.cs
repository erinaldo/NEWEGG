using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIWishListDetailInfo class.
    /// </summary>
    [DataContract]
    public class UIWishListDetailInfo
    {
        /// <summary>
        /// Gets or sets CustomerNumber.
        /// </summary>
        [DataMember(Name = "CustomerNumber")]
        public int CustomerNumber { get; set; }

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

        /// <summary>
        /// Gets or sets TotalItemQty.
        /// </summary>
        [DataMember(Name = "TotalItemQty")]
        public int TotalItemQty { get; set; }

        /// <summary>
        /// Gets or sets TotalItemPrice.
        /// </summary>
        [DataMember(Name = "TotalItemPrice")]
        public string TotalItemPrice { get; set; }

        /// <summary>
        /// Gets or sets PageInfo.
        /// </summary>
        [DataMember(Name = "PageInfo")]
        public UIPageInfo PageInfo { get; set; }

        /// <summary>
        /// Gets or sets WishListItemList.
        /// </summary>
        [DataMember(Name = "WishListItemList")]
        public List<UIWishListProductInfo> WishListItemList { get; set; }
    }
}
