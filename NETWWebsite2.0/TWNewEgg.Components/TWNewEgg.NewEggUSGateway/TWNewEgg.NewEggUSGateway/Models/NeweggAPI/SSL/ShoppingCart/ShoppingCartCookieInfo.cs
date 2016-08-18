using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// ShoppingCart Cookie Info.
    /// </summary>
    [DataContract]
    public class ShoppingCartCookieInfo
    {
        /// <summary>
        /// Gets or sets itemList.
        /// </summary>
        [DataMember(Name = "ls")]
        public List<ShoppingCartItemCookieInfo> ItemList { get; set; }

        /// <summary>
        /// Gets or sets total item quantity. 
        /// </summary>
        [DataMember(Name = "tiq")]
        public int TotalItemQty { get; set; }

        /// <summary>
        /// Gets or sets order default shipping method info.
        /// </summary>
        [DataMember(Name = "sls")]
        public List<ShoppingCartShippingMethodInfo> ShippingMethodList { get; set; }

        /// <summary>
        /// Gets or sets session id.
        /// </summary>
        [DataMember(Name = "sid")]
        public string SessionID { get; set; }
		
        /// <summary>
        /// Gets or sets promo codes. 
        /// </summary>
        [DataMember(Name = "pc")]
        public string PromoCodes { get; set; }

        /// <summary>
        /// Gets or sets Zip Code. 
        /// </summary>
        [DataMember(Name = "zp")]
        public string ZipCode { get; set; }

        /// <summary>
        /// Gets or sets Payer id. 
        /// </summary>
        [DataMember(Name = "pid")]
        public string PayerID { get; set; }

        /// <summary>
        /// Gets or sets Token. 
        /// </summary>
        [DataMember(Name = "tk")]
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets PaypalEmail. 
        /// </summary>
        [DataMember(Name = "ppe")]
        public string PaypalEmail { get; set; }

        /// <summary>
        /// Gets or sets PaypalEmail. 
        /// </summary>
        [DataMember(Name = "pac")]
        public string PaytermsCode { get; set; }
		
		/// <summary>
        /// Gets or sets a value indicating whether IsRushOrder. 
        /// </summary>
        [DataMember(Name = "rus")]
        public bool IsRushOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsNPAChecked. 
        /// </summary>
        [DataMember(Name = "npa")]
        public bool IsNPAChecked { get; set; }
    }
}
