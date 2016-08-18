using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("PromotionGiftRecords")]
    public class PromotionGiftRecords
    {
        public enum UsedStatusOption
        {
            /// <summary>
            /// 未設定(預設)
            /// </summary>
            NotSet = 0,
            /// <summary>
            /// 匯款成功設為使用
            /// </summary>
            Used = 1,
            /// <summary>
            /// 匯款失敗設為不使用
            /// </summary>
            NotUsed = 2,
            /// <summary>
            /// 暫時設定使用狀態但未結帳
            /// 於結帳流程中使用, 結帳完成會設定Used
            /// </summary>
            TempUsed = 3,
            /// <summary>
            /// 使用後(Used)但取消訂單
            /// </summary>
            CancelUsed = 4
        }

        /// <summary>
        /// 滿額贈ID
        /// </summary>
        [Key, Column(Order = 0)]
        public int PromotionGiftBasicID { get; set; }

        /// <summary>
        /// 訂單子單編號
        /// </summary>
        [Key, Column(Order = 1)]
        public string SalesOrderItemCode { get; set; }

        /// <summary>
        /// 級距ID
        /// </summary>
        public int PromotionGiftIntervalID { get; set; }

        /// <summary>
        /// 級距的折扣金額
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// 使用狀態
        /// </summary>
        public int UsedStatus { get; set; }

        /// <summary>
        /// 訂單子單分攤金額
        /// </summary>
        public decimal ApportionedAmount { get; set; }

        /// <summary>
        /// 產生入會計
        /// </summary>
        public Nullable<int> SAIn { get; set; }

        /// <summary>
        /// 報廢入會計
        /// </summary>
        public Nullable<int> SAOut { get; set; }

        /// <summary>
        /// 建立日期
        /// </summary>
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 最後修改日期
        /// </summary>
        public Nullable<DateTime> UpdateDate { get; set; }

        /// <summary>
        /// 最後修改人
        /// </summary>
        public string UpdateUser { get; set; }
    }
}
