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
    /// 金流與物流對應總表
    /// </summary>
    [Table("paytypedeliver")]
    public class PayTypeDeliver
    {
        public PayTypeDeliver()
        {

        }

        /// <summary>
        /// 自動產生的編號
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// Seller Id
        /// </summary>
        [Key, Column(Order=0)]
        public int SellerId { get; set; }

        /// <summary>
        /// PayType
        /// </summary>
        [Key, Column(Order = 1)]
        public int PayType { get; set; }

        /// <summary>
        /// 可使用的物流Code列表
        /// </summary>
        public string DeliverCodeList { get; set; }

        /// <summary>
        /// 轉化為Json字串的物流Code列表
        /// </summary>
        public string JsonDeliverCodeList { get; set; }

        /// <summary>
        /// 創建人
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 創建時間
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改次數
        /// </summary>
        public int Updated { get; set; }

        /// <summary>
        /// 最後一次修改者
        /// </summary>
        public string UpdateUser { get; set; }

        /// <summary>
        /// 最後一次修改時間
        /// </summary>
        public Nullable<DateTime> UpdateDate { get; set; }
    }
}
