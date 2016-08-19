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
    /// coupon取消交易異動記錄
    /// </summary>
    [Table("couponcncl")]
    public class Couponcncl
    {
        public Couponcncl()
        {
        }

        /// <summary>
        /// 序號
        /// </summary>
        [Key]
        public int id { get; set; }
        /// <summary>
        /// 正反向(預設為正向,此功能目前尚未開放)
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 來自哪張取消單號, 非必填,可null
        /// </summary>
        public int frmid { get; set; }
        /// <summary>
        /// coupon id
        /// </summary>
        public int couponid { get; set; }
        /// <summary>
        /// coupon消費的購物單id
        /// </summary>
        public int processid { get; set; }
        /// <summary>
        /// coupon擁有者id
        /// </summary>
        public string accountid { get; set; }
        /// <summary>
        /// 頁目
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 執行結果
        /// </summary>
        public string execrslt { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 狀態修改時間
        /// </summary>
        public DateTime? statusdate { get; set; }
        /// <summary>
        /// 狀態修改人
        /// </summary>
        public string statususer { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string note { get; set; }
        public int updated { get; set; }
        /// <summary>
        /// 建檔日期
        /// </summary>
        public DateTime createdate { get; set; }
        /// <summary>
        /// 建檔人
        /// </summary>
        public string createuser { get; set; }
        /// <summary>
        /// 最後更改日期
        /// </summary>
        public DateTime? updatedate { get; set; }
        /// <summary>
        /// 最後更改人
        /// </summary>
        public string updateuser { get; set; }
    }//end class
}//end namespace