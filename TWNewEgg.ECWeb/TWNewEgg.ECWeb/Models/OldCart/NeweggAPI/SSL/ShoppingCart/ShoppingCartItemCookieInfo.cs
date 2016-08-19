using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// ShoppingCart Item Cookie Info.
    /// </summary>
    [DataContract]
    public class ShoppingCartItemCookieInfo
    {
        /// <summary>
        /// Gets or sets ItemNumber or ComboId.
        /// </summary>
        [DataMember(Name = "itn")]
        public string ItemNumber { get; set; }

        /// <summary>
        /// Gets or sets Quantity.
        /// </summary>
        [DataMember(Name = "qty")]
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets real itemnumber.
        /// </summary>
        [DataMember(Name = "ritn")]
        public string RealItemNumber { get; set; }

        /// <summary>
        /// Gets or sets Combo releted item number.
        /// </summary>
        [DataMember(Name = "rtn")]
        public string ReleatedItemNumber { get; set; }

        /// <summary>
        /// Gets or sets Combo releted item number.
        /// </summary>
        [DataMember(Name = "tp")]
        public int ItemType { get; set; }

        /// <summary>
        /// Gets or sets RelatedItem, eg: SNET, INSTALLNET, TECH.
        /// </summary>
        [DataMember(Name = "rit")]
        public List<ShoppingCartSNETItemCookieInfo> RelatedItem { get; set; }
    }
}
