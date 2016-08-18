using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UISubscribeItemInfo class.
    /// </summary>
    [DataContract]
    public class UISubscribeItemInfo
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
        /// Gets or sets SubscribeID.
        /// </summary>
        [DataMember(Name = "SubscribeID")]
        public int SubscribeID { get; set; }

        /// <summary>
        /// Gets or sets SubscribeName.
        /// </summary>
        [DataMember(Name = "SubscribeName")]
        public string SubscribeName { get; set; }

        /// <summary>
        /// Gets or sets DeviceType.
        /// </summary>
        [DataMember(Name = "DeviceType")]
        public string DeviceType { get; set; }

        /// <summary>
        /// Gets or sets CompanyCode.
        /// </summary>
        [DataMember(Name = "CompanyCode")]
        public int CompanyCode { get; set; }

        /// <summary>
        /// Gets or sets CountryCode.
        /// </summary>
        [DataMember(Name = "CountryCode")]
        public string CountryCode { get; set; }
    }
}
