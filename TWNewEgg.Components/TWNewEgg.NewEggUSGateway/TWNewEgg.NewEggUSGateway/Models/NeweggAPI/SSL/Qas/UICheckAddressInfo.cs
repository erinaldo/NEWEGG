using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UICheckAddresInfo class.
    /// </summary>
    [DataContract]
    public class UICheckAddresInfo
    {
        /// <summary>
        /// Gets or sets ShippingAddress.
        /// </summary>
        [DataMember(Name = "ShippingAddress")]
        public UIQASAddressInputInfo ShippingAddress { get; set; }
        
        /// <summary>
        /// Gets or sets BillingAddress.
        /// </summary>
        [DataMember(Name = "BillingAddress")]
        public UIQASAddressInputInfo BillingAddress { get; set; }
        
        /// <summary>
        /// Gets or sets ShippingCheckAction.
        /// </summary>
        [DataMember(Name = "ShippingCheckAction")]
        public UIQASCheckActionType ShippingCheckAction { get; set; }
        
        /// <summary>
        /// Gets or sets BillingCheckAction.
        /// </summary>
        [DataMember(Name = "BillingCheckAction")]
        public UIQASCheckActionType BillingCheckAction { get; set; }

        /// <summary>
        /// Gets or sets BillingCheckAction.
        /// </summary>
        [DataMember(Name = "ShippingQASVerifyLevelTypeValue")]
        public int? ShippingQASVerifyLevelTypeValue { get; set; }

        /// <summary>
        /// Gets or sets BillingCheckAction.
        /// </summary>
        [DataMember(Name = "BillingQASVerifyLevelTypeValue")]
        public int? BillingQASVerifyLevelTypeValue { get; set; }
    }
}
