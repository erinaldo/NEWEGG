using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIVoteReviewInfo class.
    /// </summary>
    [DataContract]
    public class UIVoteReviewInfo
    {
        /// <summary>
        /// Gets or sets CustomerNumber.
        /// </summary>
        [DataMember(Name = "CustomerNumber")]
        public int CustomerNumber { get; set; }
        
        /// <summary>
        /// Gets or sets ReviewID.
        /// </summary>
        [DataMember(Name = "ReviewID")]
        public int ReviewID { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether IsAgree.
        /// </summary>
        [DataMember(Name = "IsAgree")]
        public bool IsAgree { get; set; }
    }
}
