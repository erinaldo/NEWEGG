using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIWishListRenameTitleInputInfo class.
    /// </summary>
    [DataContract]
    public class UIWishListRenameTitleInputInfo : UIWishListBaseInputInfo
    {
        /// <summary>
        /// Gets or sets WishListRenameTitle.
        /// </summary>
        [DataMember(Name = "WishListRenameTitle")]
        public string WishListRenameTitle { get; set; }
    }
}
