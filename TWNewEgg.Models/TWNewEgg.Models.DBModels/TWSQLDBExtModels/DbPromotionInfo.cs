using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.Models.DBModels.TWSQLDBExtModels
{
    public class DbPromotionInfo
    {
        public DbPromotionInfo()
        {
            this.isDiscountAmount = false;
            this.isDiscountPercent = false;
            this.isCoupon = false;
            this.isLottery = false;
            this.isGift = false;
            this.promotionGiftBasicList = new List<PromotionGiftBasic>();
            this.promotionGiftIntervalList = new List<PromotionGiftInterval>();
        }
        /// <summary>
        /// 是否有滿額折優惠(現折)
        /// </summary>
        public bool isDiscountAmount { get; set; }
        
        /// <summary>
        /// 是否有滿額折優惠(折扣百分比)
        /// </summary>
        public bool isDiscountPercent { get; set; }

        /// <summary>
        /// 是否有折價卷優惠
        /// </summary>
        public bool isCoupon { get; set; }

        /// <summary>
        /// 是否有抽獎優惠
        /// </summary>
        public bool isLottery { get; set; }

        /// <summary>
        /// 是否有贈獎優惠(獎品)
        /// </summary>
        public bool isGift { get; set; }

        /// <summary>
        /// 滿額折活動資訊清單
        /// </summary>
        public List<PromotionGiftBasic> promotionGiftBasicList { get; set; }

        /// <summary>
        /// 滿額折活動級距清單
        /// </summary>
        public List<PromotionGiftInterval> promotionGiftIntervalList { get; set; }
    }
}
