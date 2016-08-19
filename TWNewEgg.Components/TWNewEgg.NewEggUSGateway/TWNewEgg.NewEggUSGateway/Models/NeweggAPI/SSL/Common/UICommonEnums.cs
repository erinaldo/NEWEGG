using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIMapPriceMarkType class.
    /// </summary>
    [DataContract]
    public enum UIMapPriceMarkType
    {
        /// <summary>
        /// Gets or sets NOMAPPrice.
        /// </summary>
        [EnumMember]
        NOMAPPrice = 0,

        /// <summary>
        /// Gets or sets MAPPrice.
        /// </summary>
        [EnumMember]
        MAPPrice = 1,

        /// <summary>
        /// Gets or sets SecureCheckoutMAP.
        /// </summary>
        [EnumMember]
        SecureCheckoutMAP = 2,
    }

    /// <summary>
    /// Initializes a new instance of the UISalesType class.
    /// </summary>
    [DataContract]
    public enum UISalesType
    {
        /// <summary>
        /// Gets or sets salesOrder.
        /// </summary>
        [EnumMember]
        SalesOrder = 1,

        /// <summary>
        /// Gets or sets PreSalesOrder.
        /// </summary>
        [EnumMember]
        PreSalesOrder = 2,
    }
}
