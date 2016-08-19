using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("coupon")]
    public class Coupon
    {
        public enum CouponActiveTypeOption
        {
            /// <summary>
            /// 未生效
            /// </summary>
            NotActive = 0,
            /// <summary>
            /// 系統自動啟動生效
            /// </summary>
            SystemAutoActive = 1,
            /// <summary>
            /// User 兌碼啟動生效
            /// </summary>
            UserActive = 2
        }
        public enum ValidTypeOption
        {
            /// <summary>
            /// 等待系統啟動
            /// </summary>
            WaitingForSystemActive = 0,
            /// <summary>
            /// 系統啟動
            /// </summary>
            System = 1,
            /// <summary>
            /// 由User兌碼啟動
            /// </summary>
            ByUser = 2,
            /// <summary>
            /// 其他
            /// </summary>
            Other = 3,
            /// <summary>
            /// 不啟動
            /// </summary>
            NotActive = 4
        }
        public enum CouponUsedStatusOption : int
        {
            /// <summary>
            /// 未設定
            /// </summary>
            NotSet = 0,
            /// <summary>
            /// 已生效但未使用
            /// </summary>
            ActiveButNotUsed = 1,
            /// <summary>
            /// 已使用
            /// </summary>
            Used = 2,
            /// <summary>
            /// 未使用但已過期
            /// </summary>
            NotUsedButExpired = 3,
            /// <summary>
            /// 其他
            /// </summary>
            Other = 4,
            /// <summary>
            /// 暫時設定使用狀態但未結帳
            /// 於結帳流程中使用, 結帳完成會設定Used
            /// </summary>
            SetTempUsedForCheckout = 5,
            /// <summary>
            /// 使用後(Used)但取消訂單
            /// </summary>
            UsedButCancel = 6
        }
        public Coupon()
        {
        }

        /// <summary>
        /// 序號
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        /// <summary>
        /// coupon number, 共13碼, 第1碼: 英文, 2013年為A; 2-4碼:event id, 5-13: coupon編號
        /// </summary>
        [Key]
        public string number { get; set; }

        /// <summary>
        /// 對應的活動序號
        /// </summary>
        public int eventid { get; set; }
        /// <summary>
        /// 擁有者編號
        /// </summary>
        public string accountid { get; set; }
        /// <summary>
        /// 使用訂購單編號
        /// </summary>
        public string ordcode { get; set; }

        /// <summary>
        /// 項目
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 可使用類別, default ;0; 為通用, 其他類別以分號區隔, 此項需與event的categories相同
        /// </summary>
        public string categories { get; set; }
        /// <summary>
        /// 有效起始日
        /// </summary>
        public DateTime validstart { get; set; }
        /// <summary>
        /// 有效截止日
        /// </summary>
        public DateTime validend { get; set; }
        /// <summary>
        /// 使用時間
        /// </summary>
        public DateTime? usedate { get; set; }
        /// <summary>
        /// 是否顯示, default 0:不顯示, 1:顯示
        /// </summary>
        public int visible { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string note { get; set; }
        /// <summary>
        /// coupon使用狀態, default 0:未設定未生效, 1:生效可使用, 2: 已使用, 3:未使用已過期, 4:other(需在note寫明原因)
        /// </summary>
        public int usestatus { get; set; }
        /// <summary>
        /// coupon的價值/面額
        /// </summary>
        public decimal value { get; set; }
        /// <summary>
        /// 建檔時間
        /// </summary>
        public DateTime createdate { get; set; }
        /// <summary>
        /// 建檔人
        /// </summary>
        public string createuser { get; set; }
        public int updated { get; set; }
        /// <summary>
        /// 最後修改時間
        /// </summary>
        public DateTime? updatedate { get; set; }

        /// <summary>
        /// 最後修改人
        /// </summary>
        public string updateuser { get; set; }

        /// <summary>
        /// coupon 必須使用在何種售價以上的產品
        /// </summary>
        public decimal? limit { get; set; }

        /// <summary>
        /// coupon的實際的生效方式
        /// </summary>
        public int activetype { get; set; }

        /// <summary>
        /// 預設coupon的啟動方式
        /// </summary>
        public int validtype { get; set; }

        /// <summary>
        /// 產生入會計
        /// </summary>
        public int? SAIn { get; set; }

        /// <summary>
        /// 報廢入會計
        /// </summary>
        public int? SAOut { get; set; }

        /// <summary>
        /// 可使用的itemid, 以分號作間隔
        /// </summary>
        public string items { get; set; }

    }//end class
}
