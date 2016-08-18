using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UITimeMachineViewModel class.
    /// </summary>
    [DataContract]
    public class UITimeMachineViewModel
    {
        /// <summary>
        /// Gets or sets CustomerNumber.
        /// </summary>
        public int CustomerNumber { get; set; }

        /// <summary>
        /// Gets or sets TokenID.
        /// </summary>
        public string TokenID { get; set; }

        /// <summary>
        /// Gets or sets Type.
        /// </summary>
        public string Type { get; set; }
    }
}
