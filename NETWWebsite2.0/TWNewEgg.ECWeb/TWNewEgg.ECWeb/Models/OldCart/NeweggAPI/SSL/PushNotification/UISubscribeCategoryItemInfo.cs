using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UISubscribeCategoryItemInfo class.
    /// </summary>
    [DataContract]
    public class UISubscribeCategoryItemInfo
    {
        /// <summary>
        /// Gets or sets SubscribeID.
        /// </summary>
        [DataMember(Name = "SubscribeID")]
        public int SubscribeID { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        [DataMember(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        [DataMember(Name = "Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsDefault.
        /// </summary>
        [DataMember(Name = "IsDefault")]
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsRegister.
        /// </summary>
        [DataMember(Name = "IsRegister")]
        public bool IsRegister { get; set; }
    }
}
