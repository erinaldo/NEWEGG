using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIShoppingCartDataInfo class.
    /// </summary>
    [DataContract]
    public class UIShoppingCartDataInfo
    {
        /// <summary>
        /// Gets or sets a value indicating whether IsItemChanged.
        /// </summary>
        [DataMember(Name = "IsItemChanged")]
        public bool IsItemChanged { get; set; }

        /// <summary>
        /// Gets or sets ZipCode.
        /// </summary>
        [DataMember(Name = "ZipCode")]
        public string ZipCode { get; set; }

        /// <summary>
        /// Gets or sets CustomerNumber.
        /// </summary>
        [DataMember(Name = "CustomerNumber")]
        public int CustomerNumber { get; set; }

        /// <summary>
        /// Gets or sets LoginName.
        /// </summary>
        [DataMember(Name = "LoginName")]
        public string LoginName { get; set; }

        /// <summary>
        /// Gets or sets GiftCodeList.
        /// </summary>
        [DataMember(Name = "GiftCodeList")]
        public List<string> GiftCodeList { get; set; }

        /// <summary>
        /// Gets or sets SecurityCodeList.
        /// </summary>
        [DataMember(Name = "SecurityCodeList")]
        public List<string> SecurityCodeList { get; set; }

        /// <summary>
        /// Gets or sets PromotionCodeList.
        /// </summary>
        [DataMember(Name = "PromotionCodeList")]
        public List<string> PromotionCodeList { get; set; }

        /// <summary>
        /// Gets or sets itemList.
        /// </summary>
        [DataMember(Name = "ItemList")]
        public List<UIShoppingCartUnitInfo> ItemList { get; set; }

        /// <summary>
        /// Gets or sets OrderDefaultShippingMethodInfoList.
        /// </summary>
        [DataMember(Name = "OrderDefaultShippingMethodInfoList")]
        public List<UIOrderDefaultShippingMethodInfo> OrderDefaultShippingMethodInfoList { get; set; }
    }
}
