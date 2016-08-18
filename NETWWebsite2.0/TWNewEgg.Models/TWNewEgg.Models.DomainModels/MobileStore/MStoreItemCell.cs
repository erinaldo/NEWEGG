using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Store;

namespace TWNewEgg.Models.DomainModels.MobileStore
{
    /// <summary>
    /// 手機版 - 單一商品的各個屬性
    /// </summary>
    public class MStoreItemCell : StoreItemCell
    {
        /// <summary>
        /// 商品的促銷資訊
        /// </summary>
        public ItemPromoInfo PromoInfo { get; set; }

        /// <summary>
        /// 是否為登入會員的追蹤商品
        /// </summary>
        public bool IsTrack { get; set; }

        /// <summary>
        /// 該商品是否已售完。true:已售完。
        /// </summary>
        public bool IsSoldOut { get; set; }

        /// <summary>
        /// 商品尚未折扣前的金額
        /// </summary>
        public decimal OriginalPrice { get; set; }
    }
}
