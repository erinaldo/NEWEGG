using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UICheckResultAddressInfo class.
    /// </summary>
    [DataContract]
    public class UICheckResultAddressInfo
    {
        /// <summary>
        /// Gets or sets ShippingAddress.
        /// </summary>
        [DataMember(Name = "ShippingAddress")]
        public UIQASCheckResultInfo ShippingAddress { get; set; }
        
        /// <summary>
        /// Gets or sets BillingAddress.
        /// </summary>
        [DataMember(Name = "BillingAddress")]
        public UIQASCheckResultInfo BillingAddress { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether GoToNextPage.
        /// </summary>
        [DataMember(Name = "GoToNextPage")]
        public bool GoToNextPage { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether IsBillAddressEqualsShippingAddress.
        /// </summary>
        [DataMember(Name = "IsBillAddressEqualsShippingAddress")]
        public bool IsBillAddressEqualsShippingAddress { get; set; }
    }
}
