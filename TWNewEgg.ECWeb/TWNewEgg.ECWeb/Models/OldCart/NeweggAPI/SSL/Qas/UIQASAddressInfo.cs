using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIQASAddressInfo class.
    /// </summary>
    [DataContract]
    public class UIQASAddressInfo : UIAddressInfo
    {
        /// <summary>
        /// Gets or sets SubMonikerKey.
        /// </summary>
        [DataMember(Name = "SubMonikerKey")]
        public string SubMonikerKey { get; set; }

        /// <summary>
        /// Gets or sets PickListAddress.
        /// </summary>
        [DataMember(Name = "PickListAddress")]
        public string PickListAddress { get; set; }

        /// <summary>
        /// Gets or sets PickListZipCode.
        /// </summary>
        [DataMember(Name = "PickListZipCode")]
        public string PickListZipCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether MustRefine.
        /// </summary>
        [DataMember(Name = "MustRefine")]
        public bool MustRefine { get; set; }
    }
}
