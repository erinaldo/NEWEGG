using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIWishListDeleteInputInfo class.
    /// </summary>
    [DataContract]
    public class UIWishListDeleteInputInfo
    {
        /// <summary>
        /// Gets or sets CustomerNumber.
        /// </summary>
        [DataMember(Name = "CustomerNumber")]
        public int CustomerNumber { get; set; }

        /// <summary>
        /// Gets or sets WishListNumbers.
        /// </summary>
        [DataMember(Name = "WishListNumbers")]
        public string WishListNumbers { get; set; }
    }
}
