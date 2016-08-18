using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIRegisterSubscribeRequestInfo class.
    /// </summary>
    [DataContract]
    public class UIRegisterSubscribeRequestInfo
    {
        /// <summary>
        /// Gets or sets DeviceID.
        /// </summary>
        [DataMember(Name = "DeviceID")]
        public string DeviceID { get; set; }

        /// <summary>
        /// Gets or sets DeviceUri.
        /// </summary>
        [DataMember(Name = "DeviceUri")]
        public string DeviceUri { get; set; }

        /// <summary>
        /// Gets or sets OSVersion.
        /// </summary>
        [DataMember(Name = "OSVersion")]
        public string OSVersion { get; set; }

        /// <summary>
        /// Gets or sets SubscribeCategories.
        /// </summary>
        [DataMember(Name = "SubscribeCategories")]
        public string SubscribeCategories { get; set; }
    }
}
