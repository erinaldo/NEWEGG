using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the CoremetricsInfo class.
    /// </summary>
    [DataContract]
    public class CoremetricsInfo
    {
        /// <summary>
        /// Gets or sets CategoryID.
        /// </summary>
        [DataMember(Name = "CategoryID")]
        public string CategoryID { get; set; }
        
        /// <summary>
        /// Gets or sets ProductName.
        /// </summary>
        [DataMember(Name = "ProductName")]
        public string ProductName { get; set; }
        
        /// <summary>
        /// Gets or sets PageID.
        /// </summary>
        [DataMember(Name = "PageID")]
        public string PageID { get; set; }
        
        /// <summary>
        /// Gets or sets Brand.
        /// </summary>
        [DataMember(Name = "Brand")]
        public string Brand { get; set; }
    }
}
