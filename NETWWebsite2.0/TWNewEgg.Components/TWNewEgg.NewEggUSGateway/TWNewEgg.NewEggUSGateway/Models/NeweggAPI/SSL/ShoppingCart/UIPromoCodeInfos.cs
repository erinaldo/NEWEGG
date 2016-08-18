using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Class named UIPromoCodeInfos.
    /// </summary>
    public class UIPromoCodeInfos
    {
        /// <summary>
        /// Initializes a new instance of the UIPromoCodeInfos class.
        /// </summary>
        /// <param name="promoCodeInfoList">Promo code info list.</param>
        /// <param name="relatedMessage">Related message.</param>
        public UIPromoCodeInfos(List<UIShoppingCartPromotionCodeInfo> promoCodeInfoList, string relatedMessage)
        {
            this.PromotionCodeInfoList = promoCodeInfoList;
            this.PCodeRelateMessage = relatedMessage;
        }

        /// <summary>
        /// Gets or sets ShoppingCartPromotionCodeInfoList.
        /// </summary>
        [DataMember(Name = "ShoppingCartPromotionCodeInfoList")]
        public List<UIShoppingCartPromotionCodeInfo> PromotionCodeInfoList { get; set; }

        /// <summary>
        /// Gets or sets PCodeRelateMessage.
        /// </summary>
        [DataMember(Name = "PCodeRelateMessage")]
        public string PCodeRelateMessage { get; set; }
    }
}
