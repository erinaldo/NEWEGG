using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIEmailAFriendInputContentInfo class.
    /// </summary>
    [DataContract]
    public class UIEmailAFriendInputContentInfo
    {
        /// <summary>
        /// Gets or sets ItemNumber.
        /// </summary>
        [DataMember(Name = "ItemNumber")]
        public string ItemNumber { get; set; }

        /// <summary>
        /// Gets or sets itemList.
        /// </summary>
        [DataMember(Name = "ItemList")]
        public string ItemList { get; set; }

        /// <summary>
        /// Gets or sets CustomerNumber.
        /// </summary>
        [DataMember(Name = "CustomerNumber")]
        public int CustomerNumber { get; set; }

        /// <summary>
        /// Gets or sets CustomerEmailAddress.
        /// </summary>
        [DataMember(Name = "CustomerEmailAddress")]
        public string CustomerEmailAddress { get; set; }

        /// <summary>
        /// Gets or sets ReceiveEmailAddress.
        /// </summary>
        [DataMember(Name = "ReceiveEmailAddress")]
        public string ReceiveEmailAddress { get; set; }

        /// <summary>
        /// Gets or sets CustomerComment.
        /// </summary>
        [DataMember(Name = "CustomerComment")]
        public string CustomerComment { get; set; }

        /// <summary>
        /// Gets or sets ItemInfo.
        /// </summary>
        [DataMember(Name = "ItemInfo")]
        public UIEmailItemInfo ItemInfo { get; set; }
    }
}
