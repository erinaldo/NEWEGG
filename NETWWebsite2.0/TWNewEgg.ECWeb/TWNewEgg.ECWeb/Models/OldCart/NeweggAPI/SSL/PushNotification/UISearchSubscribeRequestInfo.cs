using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UISearchSubscribeRequestInfo class.
    /// </summary>
    [DataContract]
    public class UISearchSubscribeRequestInfo
    {
        /// <summary>
        /// Gets or sets DeviceType.
        /// </summary>
        [DataMember(Name = "DeviceType")]
        public string DeviceType { get; set; }

        /// <summary>
        /// Gets or sets DeviceIDBeginValue.
        /// </summary>
        [DataMember(Name = "DeviceIDBeginValue")]
        public string DeviceIDBeginValue { get; set; }

        /// <summary>
        /// Gets or sets DeviceIDEndValue.
        /// </summary>
        [DataMember(Name = "DeviceIDEndValue")]
        public string DeviceIDEndValue { get; set; }

        /// <summary>
        /// Gets or sets CountryCode.
        /// </summary>
        [DataMember(Name = "CountryCode")]
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets CompanyCode.
        /// </summary>
        [DataMember(Name = "CompanyCode")]
        public int CompanyCode { get; set; }

        /// <summary>
        /// Gets or sets PageIndex.
        /// </summary>
        [DataMember(Name = "PageIndex")]
        public int PageIndex { get; set; }

        /// <summary>
        /// Gets or sets PageSize.
        /// </summary>
        [DataMember(Name = "PageSize")]
        public int PageSize { get; set; }
    }
}
