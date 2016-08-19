using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the ThirdPartyContext class.
    /// </summary>
    public class ThirdPartyContext
    {
        /// <summary>
        /// Gets or sets OrderDetailList.
        /// </summary>
        public List<UIOrderInfo> OrderDetailList { get; set; }

        /// <summary>
        /// Gets ThirdPartySubTotalPriceValue.
        /// </summary>
        public string ThirdPartySubTotalPriceValue
        {
            get
            {
                return string.Format("{0:F2}", this.OrderDetailList.Sum(o => Convert.ToDecimal(o.OrderDisplayFeeInfo.ItemTotal.TrimStart('$'))));
            }
        }
    }
}
