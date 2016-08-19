using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("PromotionGiftBasic")]
    public class PromotionGiftBasic
    {
        public enum UsedStatus
        {
            Used = 1,
            NotUsed = 2,
            TempUsed = 3,
        }

        public PromotionGiftBasic()
        {
            this.Priority = 0;
        }

        /// <summary>
        /// 滿額贈ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// 類別ID String
        /// </summary>
        public string Categories { get; set; }

        /// <summary>
        /// 活動名稱
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 起始時間
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 結束時間
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 活動優先順序
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 活動優惠名稱顯示
        /// </summary>
        public string ShowDesc { get; set; }

        /// <summary>
        /// 所參考的清單，白名單或黑名單
        /// </summary>
        public string ReferencesList { get; set; }

        /// <summary>
        /// CSS設定
        /// </summary>
        public string CSS { get; set; }

        /// <summary>
        /// 商品下方所要顯示的訊息
        /// </summary>
        public string HighLight { get; set; }

        /// <summary>
        /// 建立日期
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 建立者
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 最後一次修改日期
        /// </summary>
        public DateTime? UpdateDate { get; set; }

        /// <summary>
        /// 最後一次的修改者
        /// </summary>
        public string UpdateUser { get; set; }
    }
}
