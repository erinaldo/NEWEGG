using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newegg.Mobile.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// ShippingMethodItem ViewData.
    /// </summary>
    [DataContract]
    public class ShippingMethodItemViewData
    {
        /// <summary>
        /// Initializes a new instance of the ShippingMethodItemViewData class.
        /// </summary>
        /// <param name="method">Method sss.</param>
        public ShippingMethodItemViewData(UIShippingMethodInfo method)
        {
            this.Code = method.Code;
            this.IsSamDayOrder = method.IsSamDayOrder;
            this.SellerID = method.SellerID;
        }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        [DataMember(Name = "sm")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsSamDayOrder.
        /// </summary>
        [DataMember(Name = "sd")]
        public bool IsSamDayOrder { get; set; }

        /// <summary>
        /// Gets or sets SellerID.
        /// </summary>
        [DataMember(Name = "si")]
        public string SellerID { get; set; }
    }
}
