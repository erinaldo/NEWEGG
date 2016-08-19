using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIQASAddressInputInfo class.
    /// </summary>
    [DataContract]
    public class UIQASAddressInputInfo : UIAddressInfo
    {
        /// <summary>
        /// Gets or sets RefinementText.
        /// </summary>
        [DataMember(Name = "RefinementText")]
        public string RefinementText { get; set; }

        /// <summary>
        /// Gets or sets InputMonikerKey.
        /// </summary>
        [DataMember(Name = "InputMonikerKey")]
        public string InputMonikerKey { get; set; }
    }
}
