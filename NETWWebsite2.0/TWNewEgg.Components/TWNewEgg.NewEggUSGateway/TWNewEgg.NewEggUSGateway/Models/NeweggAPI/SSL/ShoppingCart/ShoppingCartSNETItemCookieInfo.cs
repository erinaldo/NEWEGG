using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Shopping cart SNET Item CookieInfo.
    /// </summary>
    [DataContract]
    public class ShoppingCartSNETItemCookieInfo
    {
        /// <summary>
        /// Gets or sets Main ItemNumber.
        /// </summary>
        [DataMember(Name = "it")]
        public string ItemNumber { get; set; }

        /// <summary>
        /// Gets or sets Related Snet Item or Ds Item.
        /// </summary>
        [DataMember(Name = "st")]
        public string SNETNumber { get; set; }
    }
}
