using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("PromotionGiftWhiteList")]
    public class PromotionGiftWhiteList
    {
        public enum WhiteListStatus
        {
            Used = 1,
            NotUsed = 2,
            TempUsed = 3,
        }

        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// 滿額贈ID
        /// </summary>
        [Key, Column(Order=0)]
        public int PromotionGiftBasicID { get; set; }

        /// <summary>
        /// 白名單ItemID
        /// </summary>
        [Key, Column(Order = 1)]
        public int ItemID { get; set; }

        /// <summary>
        /// 白名單ItemID是否啟用的設定狀態
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 建立者
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 建立日期
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 最後修改者
        /// </summary>
        public string UpdateUser { get; set; }

        /// <summary>
        /// 最後修改時間
        /// </summary>
        public DateTime? UpdateDate { get; set; }
    }
}
