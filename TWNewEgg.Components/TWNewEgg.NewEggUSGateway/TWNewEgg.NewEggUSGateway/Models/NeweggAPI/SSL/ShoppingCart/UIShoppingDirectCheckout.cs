using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIShoppingDirectCheckout class.
    /// </summary>
    [DataContract]
    public class UIShoppingDirectCheckout
    {
        /// <summary>
        /// Gets or sets a value indicating whether IsDirectCheckout.
        /// </summary>
        [DataMember(Name = "IsDirectCheckout")]
        public bool IsDirectCheckout { get; set; }

        /// <summary>
        /// Gets or sets SessionID.
        /// </summary>
        [DataMember(Name = "SessionID")]
        public string SessionID { get; set; }
    }
}
