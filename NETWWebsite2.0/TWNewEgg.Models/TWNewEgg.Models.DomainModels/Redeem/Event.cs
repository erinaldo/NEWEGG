using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Redeem
{
    public class Event
    {
        public Event()
        {
        }

        public enum GrantStatusOption
        {
            /// <summary>
            /// 未設定發送
            /// </summary>
            NotGrant = 0,
            /// <summary>
            /// 設定完成待發放
            /// </summary>
            WaitingForGranting = 1,
            /// <summary>
            /// 發送完成
            /// </summary>
            Granted = 2,
            /// <summary>
            /// 手動中斷發送
            /// </summary>
            BreakGrantedByUser = 3,
            /// <summary>
            /// 其他原因中斷
            /// </summary>
            BreakGrantedByOther = 4,
            /// <summary>
            /// 發送中
            /// </summary>
            Granting = 5
        }
        public enum FilterFileUsageOption
        {
            //是否使用附檔清, 0:不使用, 1:使用
            AllMember = 0,
            Use = 1,
            UseSQL = 2,
            WaitingForFile = 3

        }

        /// <summary>
        /// 活動id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 活動名稱
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 活動擁有者或管理人
        /// </summary>
        public string creator { get; set; }

        /// <summary>
        /// coupon券可使用的類別, 0:全類別, 以分號區隔, 如: " ;0; ", 全類別與其他類別組合為互斥, " ;1;2;3;4; "
        /// </summary>
        public string couponcategories { get; set; }

        /// <summary>
        /// coupon行銷代碼
        /// </summary>
        public string couponmarketnumber { get; set; }

        /// <summary>
        /// 此活動發送coupon的最大數量
        /// </summary>
        public int? couponmax { get; set; }

        /// <summary>
        /// 此活動目前發送的Coupon數量
        /// </summary>
        public int couponsum { get; set; }

        /// <summary>
        /// 單一活動中, 單一User最多可領取的張數, 預設為1, 即最多領一張, 0為不限
        /// </summary>
        public int couponreget { get; set; }

        /// <summary>
        /// 活動開始日
        /// </summary>
        public DateTime? datestart { get; set; }

        /// <summary>
        /// 活動結束日
        /// </summary>
        public DateTime? dateend { get; set; }

        /// <summary>
        /// 彈性使用區間(自active算起)
        /// </summary>
        public int couponactiveusagedays { get; set; }

        /// <summary>
        /// 是否顯示活動(不顯示不代表不執行), 0:不顯示, 1:顯示
        /// </summary>
        public int visible { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        public string note { get; set; }

        /// <summary>
        /// 系統發送coupon起始時間
        /// </summary>
        public DateTime? grantstart { get; set; }

        /// <summary>
        /// 系統發送coupon完成時間
        /// </summary>
        public DateTime? grantend { get; set; }

        /// <summary>
        /// 發送coupon狀態, 0:未設定發送, 1:設定完成待發放, 2:發放完成, 3:手動關閉, 4:other(Note需寫原因), 5:發送中
        /// </summary>
        public int grantstatus { get; set; }

        /// <summary>
        /// coupon的有效日期-開始日
        /// </summary>
        public DateTime? couponvalidstart { get; set; }

        /// <summary>
        /// coupon的有效日期-截止日
        /// </summary>
        public DateTime? couponvalidend { get; set; }

        /// <summary>
        /// coupon的價值/面額
        /// </summary>
        public decimal value { get; set; }

        /// <summary>
        /// coupon啟動方式
        /// </summary>
        public int couponactivetype { get; set; }

        /// <summary>
        /// 篩選條件，為一串SQL指令
        /// </summary>
        public string filter { get; set; }

        /// <summary>
        /// 是否使用附檔清, 0:不使用, 1:使用
        /// </summary>
        public int filterfileusage { get; set; }

        /// <summary>
        /// 附檔名單ID
        /// </summary>
        public int filterfileid { get; set; }

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
        /// 最後更改時間
        /// </summary>
        public DateTime? updatedate { get; set; }

        /// <summary>
        /// 最後更改人
        /// </summary>
        public string updateuser { get; set; }


        /// <summary>
        /// Coupon券使用金額的下限
        /// </summary>
        public decimal limit { get; set; }

        /// <summary>
        /// Coupon券適用範圍說明
        /// </summary>
        public string limitdescription { get; set; }

        /// <summary>
        /// Coupon券顯示給user看的描述
        /// </summary>
        public string coupondescription { get; set; }

        /// <summary>
        /// Event的描述
        /// </summary>
        public string eventdescription { get; set; }

        /// <summary>
        /// 限定可使用的Item, 以分號分隔
        /// </summary>
        public string items { get; set; }
    }
}
