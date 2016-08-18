using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UISellerRMADetailInfo class.
    /// </summary>
    [DataContract]
    public class UISellerRMADetailInfo : UISellerRMAInfo
    {
        /// <summary>
        /// Gets or sets ReturnAddress.
        /// </summary>
        [DataMember(Name = "ReturnAddress")]
        public string ReturnAddress { get; set; }
        
        /// <summary>
        /// Gets or sets RMAProcessedBy.
        /// </summary>
        [DataMember(Name = "RMAProcessedBy")]
        public string RMAProcessedBy { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether RMAGeneratedByNewegg.
        /// </summary>
        [DataMember(Name = "RMAGeneratedByNewegg")]
        public bool RMAGeneratedByNewegg { get; set; }
        
        /// <summary>
        /// Gets or sets seller.
        /// </summary>
        [DataMember(Name = "Seller")]
        public UISellerInfo Seller { get; set; }
    }
}
