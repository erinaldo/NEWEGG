using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Promotion
{
    public class PromotionDetail_View
    {
        public PromotionDetail_View()
        {
            this.PromotionGiftBasicID = 0;
            this.Priority = 0;
            this.PromotionGiftIntervalID = 0;
            this.ApportionedAmount = 0m;
        }

        /// <summary>
        /// 活動ID
        /// </summary>
        public int PromotionGiftBasicID { get; set; }

        /// <summary>
        /// 活動優先權，1為最高優先權其餘次之，若為0則優先權最低，當優先權為0不只一個時，則以CreateDate越早的優先權較高
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 活動名稱
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 購物車顯示活動優惠名稱
        /// </summary>
        public string ShowDesc { get; set; }

        /// <summary>
        /// 使用白名單或是黑名單等等
        /// </summary>
        public string ReferencesList { get; set; }

        /// <summary>
        /// 可使用滿額贈的ItemIDs
        /// </summary>
        public List<int> AcceptedItems { get; set; }
        
        /// <summary>
        /// 該類活動中特殊排除的ItemIDs
        /// </summary>
        public List<int> NotAcceptedItems { get; set; }

        /// <summary>
        /// 級距ID
        /// </summary>
        public int PromotionGiftIntervalID { get; set; }
        
        /// <summary>
        /// 級距的折扣金額
        /// </summary>
        public decimal ApportionedAmount { get; set; }

        /// <summary>
        /// PromotionGiftBasic的StartDate
        /// </summary>
        public DateTime PromotionGiftBasicStartDate { get; set; }

        /// <summary>
        /// CSS設定
        /// </summary>
        public string CSS { get; set; }

        /// <summary>
        /// 商品下方所要顯示的訊息
        /// </summary>
        public string HighLight { get; set; }
    }
}
