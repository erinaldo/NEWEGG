using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIDiscountRowInfo class.
    /// </summary>
    public class UIDiscountRowInfo
    {
        /// <summary>
        /// Gets or sets ItemNumber.
        /// </summary>
        [DataMember(Name = "DiscountRowItemInfoList")]
        public List<UIDiscountRowItemInfo> DiscountRowItemInfoList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsMIRShow.
        /// </summary>
        [DataMember(Name = "IsMIRShow")]
        public bool IsMIRShow { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ComboMIRInfo.
        /// </summary>
        [DataMember(Name = "ComboMIRInfo")]
        public string ComboMIRInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsDiscountRowNormalShow.
        /// </summary>
        [DataMember(Name = "IsDiscountRowNormalShow")]
        public bool IsDiscountRowNormalShow { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsDiscountRowTwoColsShow.
        /// </summary>
        [DataMember(Name = "IsDiscountRowTwoColsShow")]
        public bool IsDiscountRowTwoColsShow { get; set; }
    }
}
