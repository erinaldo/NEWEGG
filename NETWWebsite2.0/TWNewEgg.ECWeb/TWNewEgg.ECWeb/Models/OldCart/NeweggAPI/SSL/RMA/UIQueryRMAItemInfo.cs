using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIQueryRMAItemInfo class.
    /// </summary>
    [DataContract]
    public class UIQueryRMAItemInfo
    {
        /// <summary>
        /// Gets or sets ItemDescription.
        /// </summary>
        [DataMember(Name = "ItemDescription")]
        public string ItemDescription { get; set; }

        /// <summary>
        /// Gets or sets PropertiesValuesDescription.
        /// </summary>
        [DataMember(Name = "PropertiesValuesDescription")]
        public string PropertiesValuesDescription { get; set; }

        /// <summary>
        /// Gets or sets RMAQuantity.
        /// </summary>
        [DataMember(Name = "RMAQuantity")]
        public int RMAQuantity { get; set; }

        /// <summary>
        /// Gets or sets RMAExtendPrice.
        /// </summary>
        [DataMember(Name = "RMAExtendPrice")]
        public string RMAExtendPrice { get; set; }

        /// <summary>
        /// Gets or sets SellerInfo.
        /// </summary>
        [DataMember(Name = "SellerInfo")]
        public UISellerInfo SellerInfo { get; set; }

        /// <summary>
        /// Gets or sets RMATypeString.
        /// </summary>
        [DataMember(Name = "RMATypeString")]
        public string RMATypeString { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsRMAConfirm.
        /// </summary>
        [DataMember(Name = "IsRMAConfirm")]
        public bool IsRMAConfirm { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsSeller.
        /// </summary>
        [DataMember(Name = "IsSeller")]
        public bool IsSeller { get; set; }

        /// <summary>
        /// Gets or sets RMAReason.
        /// </summary>
        [DataMember(Name = "RMAReason")]
        public string RMAReason { get; set; }
    }
}
