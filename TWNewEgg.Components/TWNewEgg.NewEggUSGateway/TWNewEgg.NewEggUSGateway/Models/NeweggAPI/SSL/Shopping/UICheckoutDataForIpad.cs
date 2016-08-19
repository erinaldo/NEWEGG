using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UICheckoutDataForIpad class.
    /// </summary>
    [DataContract]
    public class UICheckoutDataForIpad
    {
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
        /// Gets or sets SessionID.
        /// </summary>
        [DataMember(Name = "SessionID")]
        public string SessionID { get; set; }

        /// <summary>
        /// Gets or sets CVV2Code.
        /// </summary>
        [DataMember(Name = "CVV2Code")]
        public string CVV2Code { get; set; }

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

        /// <summary>
        /// Gets or sets a value indicating whether IsSelectedRush.
        /// </summary>
        [DataMember(Name = "IsSelectedRush")]
        public bool IsSelectedRush { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsNPAPromotionChecked.
        /// </summary>
        [DataMember(Name = "IsNPAPromotionChecked")]
        public bool IsNPAPromotionChecked { get; set; }

        /// <summary>
        /// Gets or sets PaytermsCode.
        /// </summary>
        [DataMember(Name = "PaytermsCode")]
        public string PaytermsCode { get; set; }
    }
}
