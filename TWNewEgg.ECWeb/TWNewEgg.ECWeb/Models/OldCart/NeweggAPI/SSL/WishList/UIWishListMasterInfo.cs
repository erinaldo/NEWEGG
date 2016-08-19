using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIWishListMasterInfo class.
    /// </summary>
    [DataContract]
    public class UIWishListMasterInfo
    {
        /// <summary>
        /// Gets or sets WishListNumber.
        /// </summary>
        [DataMember(Name = "WishListNumber")]
        public int WishListNumber { get; set; }

        /// <summary>
        /// Gets or sets Title.
        /// </summary>
        [DataMember(Name = "Title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets CustomerNumber.
        /// </summary>
        [DataMember(Name = "CustomerNumber")]
        public int CustomerNumber { get; set; }

        /// <summary>
        /// Gets or sets NoteDescription.
        /// </summary>
        [DataMember(Name = "NoteDescription")]
        public string NoteDescription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsDefault.
        /// </summary>
        [DataMember(Name = "IsDefault")]
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets ItemCount.
        /// </summary>
        [DataMember(Name = "ItemCount")]
        public int ItemCount { get; set; }
    }
}
