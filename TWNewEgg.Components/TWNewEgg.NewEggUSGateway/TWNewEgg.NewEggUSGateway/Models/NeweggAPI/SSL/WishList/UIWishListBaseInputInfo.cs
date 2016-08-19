using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIWishListBaseInputInfo class.
    /// </summary>
    [DataContract]
    public class UIWishListBaseInputInfo
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
    }
}
