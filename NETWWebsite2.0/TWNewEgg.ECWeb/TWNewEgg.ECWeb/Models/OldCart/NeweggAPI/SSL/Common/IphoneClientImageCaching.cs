using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the IphoneClientImageCaching class.
    /// </summary>
    [DataContract]
    public class IphoneClientImageCaching
    {
        /// <summary>
        /// Gets or sets a value indicating whether Enable.
        /// </summary>
        [DataMember(Name = "Enable")]
        public bool Enable { get; set; }

        /// <summary>
        /// Gets or sets MaxPixelCount.
        /// </summary>
        [DataMember(Name = "MaxPixelCount")]
        public int MaxPixelCount { get; set; }

        /// <summary>
        /// Gets or sets MaxPixelCountExpression.
        /// </summary>
        [DataMember(Name = "MaxPixelCountExpression")]
        public string MaxPixelCountExpression { get; set; }
    }
}
