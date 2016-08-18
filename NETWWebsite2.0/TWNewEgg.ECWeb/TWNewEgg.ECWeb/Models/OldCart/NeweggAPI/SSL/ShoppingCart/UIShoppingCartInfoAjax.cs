using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIShoppingCartInfoAjax class.
    /// </summary>
    [DataContract]
    public class UIShoppingCartInfoAjax
    {
        /// <summary>
        /// Gets or sets ShoppingCartList.
        /// </summary>
        [DataMember(Name = "ShoppingCartList")]
        public List<UIShoppingCartListInfo> ShoppingCartList { get; set; }

        /// <summary>
        /// Gets or sets SubTotal.
        /// </summary>
        [DataMember(Name = "SubTotal")]
        public string SubTotal { get; set; }

        /// <summary>
        /// Gets or sets GrandTotalAmount.
        /// </summary>
        [DataMember(Name = "GrandTotalAmount")]
        public string GrandTotalAmount { get; set; }

        /// <summary>
        /// Gets or sets ZipCode.
        /// </summary>
        [DataMember(Name = "ZipCode")]
        public string ZipCode { get; set; }

        /// <summary>
        /// Gets or sets ZipCodeRelateMessage.
        /// </summary>
        [DataMember(Name = "ZipCodeRelateMessage")]
        public string ZipCodeRelateMessage { get; set; }

        /// <summary>
        /// Gets or sets PromotionItemMessage.
        /// </summary>
        [DataMember(Name = "PromotionItemMessage")]
        public string PromotionItemMessage { get; set; }
    }
}
