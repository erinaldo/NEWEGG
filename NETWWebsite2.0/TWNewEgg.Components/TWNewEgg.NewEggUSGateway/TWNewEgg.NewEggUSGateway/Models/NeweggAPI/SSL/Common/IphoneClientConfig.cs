using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newegg.Mobile.MvcApplication.Models;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the IphoneClientConfig class.
    /// </summary>
    [DataContract]
    public class IphoneClientConfig
    {
        /// <summary>
        /// Gets or sets VersionControl.
        /// </summary>
        [DataMember(Name = "VersionControl")]
        public IphoneVersionControl VersionControl { get; set; }

        /// <summary>
        /// Gets or sets ImageCaching.
        /// </summary>
        [DataMember(Name = "ImageCaching")]
        public IphoneClientImageCaching ImageCaching { get; set; }

        /// <summary>
        /// Gets or sets Coremetrics.
        /// </summary>
        [DataMember(Name = "Cormetrics")]
        public IphoneCoremetrics Coremetrics { get; set; }

        /// <summary>
        /// Gets or sets BarcodeScan.
        /// </summary>
        [DataMember(Name = "BarcodeScan")]
        public BarcodeScan BarcodeScan { get; set; }

        /// <summary>
        /// Gets or sets ServiceHost.
        /// </summary>
        [DataMember(Name = "ServiceHost")]
        public IphoneServiceHost ServiceHost { get; set; }

        /// <summary>
        /// Gets or sets ContactUs.
        /// </summary>
        [DataMember(Name = "ContactUs")]
        public ClientContactUs ContactUs { get; set; }
    }
}
