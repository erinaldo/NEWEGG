using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIWishListRenameNoteInputInfo class.
    /// </summary>
    [DataContract]
    public class UIWishListRenameNoteInputInfo : UIWishListBaseInputInfo
    {
        /// <summary>
        /// Gets or sets WishListTitleNote.
        /// </summary>
        [DataMember(Name = "WishListTitleNote")]
        public string WishListTitleNote { get; set; }
    }
}
