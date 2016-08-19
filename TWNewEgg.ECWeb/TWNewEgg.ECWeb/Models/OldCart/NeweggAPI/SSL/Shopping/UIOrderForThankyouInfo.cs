using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIOrderForThankyouInfo class.
    /// </summary>
    [DataContract]
    public class UIOrderForThankyouInfo : UIOrderInfo
    {
        /// <summary>
        /// Gets or sets PromotionCodeInfos.
        /// </summary>
        [DataMember(Name = "PromotionCodeInfos")]
        public List<UIShoppingCartPromotionCodeInfo> PromotionCodeInfos { get; set; }

        /// <summary>
        /// Gets or sets DiscountRows.
        /// </summary>
        [DataMember(Name = "DiscountRows")]
        public List<UIDiscountRowItemInfo> DiscountRows { get; set; }
    }
}
