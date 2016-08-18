using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    /// <summary>
    /// 折價券取消申請, 當status顯示為執行後, 所有的欄位皆不可再update, 若需取消須重新申請
    /// </summary>
    [Table("couponfrm")]
    public class Couponfrm
    {
        public Couponfrm()
        {
        }

        /// <summary>
        /// 編號
        /// </summary>
        [Key]
        public int id { get; set; }

        /// <summary>
        /// 申請人
        /// </summary>
        public string user { get; set; }

        /// <summary>
        /// 對應的活動id
        /// </summary>
        public int eventid { get; set; }

        /// <summary>
        /// 取消型態, default 0:設定中, 1:該活動發放coupon全部取消, 2:該活動發放coupon區段取消, 3:單張coupon取消, 4: 不執行
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 取消的coupon開始區段(當設定event id且type為2時, 此欄位必須有值, type非0非2時, 此欄為為null)
        /// </summary>
        public string startid { get; set; }
        /// <summary>
        /// 取消的coupon截止區段(當設定event id且type為2時, 此欄必須有值, type非0非2時, 此欄為為null)
        /// </summary>
        public string endid { get; set; }
        /// <summary>
        /// 取消的單張couponnumber,(type為3時, 表示單張取消, 此欄必須有值)
        /// </summary>
        public string couponnumber { get; set; }
        /// <summary>
        /// 預定執行時間
        /// </summary>
        public DateTime? dodate { get; set; }
        /// <summary>
        /// 實際執開始時間
        /// </summary>
        public DateTime? procstartdate { get; set; }
        /// <summary>
        /// 實際執行結束時間
        /// </summary>
        public DateTime? procenddate { get; set; }
        /// <summary>
        /// 預定影響筆數
        /// </summary>
        public int amount { get; set; }
        /// <summary>
        /// 執行成功筆數
        /// </summary>
        public int success { get; set; }
        /// <summary>
        /// 執行失敗筆數
        /// </summary>
        public int fail { get; set; }
        /// <summary>
        /// 執行狀態, default 0:未執行, 1:已執行完畢, 2:不執行, 3:執行中斷, 4:執行中
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string note { get; set; }
        /// <summary>
        /// 建檔時間
        /// </summary>
        public DateTime createdate { get; set; }
        /// <summary>
        /// 建檔人
        /// </summary>
        public string createuser { get; set; }
        /// <summary>
        /// 最後修改時間
        /// </summary>
        public DateTime? updatedate { get; set; }
        /// <summary>
        /// 最後修改人
        /// </summary>
        public string updateuser { get; set; }
        public int updated { get; set; }
    }//end class
}//end namespace