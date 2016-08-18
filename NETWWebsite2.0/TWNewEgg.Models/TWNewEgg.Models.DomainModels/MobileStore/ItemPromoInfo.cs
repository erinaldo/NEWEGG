using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.MobileStore
{
    /// <summary>
    /// 手機版 - 單一商品的促銷活動資訊(促銷折數、促銷價...)
    /// </summary>
    public class ItemPromoInfo
    {
        /// <summary>
        /// 促銷折數
        /// </summary>
        public decimal PromoPercent { get; set; }

        /// <summary>
        /// 促銷價
        /// </summary>
        public decimal PromoPrice { get; set; }

        /// <summary>
        /// 是否為"美蛋閃購區"檔期中的商品(icon:美蛋同步)
        /// </summary>
        public Boolean IsFlashUS { get; set; }

        /// <summary>
        /// 是否為"獨家販售"的商品(icon:獨家)
        /// </summary>
        public Boolean IsExclusiveSale { get; set; }
    }
}
