using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.PromoActive
{
    public class SearchPromoModel
    {
        /// <summary>
        /// 行銷活動編號
        /// </summary>
        /// <value>0:不篩選</value>
        public int ID { get; set; }

        /// <summary>
        /// 公佈類別
        /// </summary>
        /// <value>0:全部</value>
        /// <value>1:行銷活動</value>
        /// <value>2:中獎名單</value>
        public int FuncType { get; set; }

        /// <summary>
        /// 關鍵字
        /// </summary>
        public string Search_KeyWord { get; set; }

        /// <summary>
        /// 活動開始日期
        /// </summary>
        public Nullable<System.DateTime> Search_StartDate { get; set; }

        /// <summary>
        /// 活動結束日期
        /// </summary>
        public Nullable<System.DateTime> Search_EndDate { get; set; }
    }

    /// <summary>
    /// 中獎名單頁搜尋絛件
    /// </summary>
    public class AwardListSearchConditionDM
    {
        /// <summary>
        /// 中獎名單頁排序方式
        /// </summary>
        /// <value>TWNewEgg.Models.DomainModels.PromoActive.AwardOrderByType</value>
        public int OrderBy { get; set; }

        /// <summary>
        /// 中獎名單頁指定顯示頁數
        /// </summary>
        public int PageIndex { get; set; }

        public AwardListSearchConditionDM()
        {
            OrderBy = (int)PromoActive.ActivityListOrderByTypeDM.最新活動; ;
            PageIndex = 1;
        }
    }
}
