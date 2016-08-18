using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("itemstock")]
    public class ItemStock
    {
        public ItemStock()
        {
            DateTime defaultDate = DateTime.Parse("1990/01/01");
            CreateDate = defaultDate;
        }
        [Key]
        public int ID { get; set; }
        /// <summary>
        /// 商品編號
        /// </summary>
        public int ProductID { get; set; }
        /// <summary>
        /// 庫存數量
        /// </summary>
        public int Qty { get; set; }
        /// <summary>
        /// 賣出數量 Default = 0
        /// </summary>
        public int QtyReg { get; set; }
        /// <summary>
        /// Default = 0
        /// </summary>
        public int SafeQty { get; set; }
        /// <summary>
        /// Default = 0
        /// </summary>
        public int Fdbcklmt { get; set; }
        /// <summary>
        /// 建立者代碼
        /// </summary>
        public string CreateUser { get; set; }
        /// <summary>
        /// 建立日期
        /// </summary>
        public System.DateTime CreateDate { get; set; }
        /// <summary>
        /// Default = 0
        /// </summary>
        public int Updated { get; set; }
        /// <summary>
        /// 更新者代碼
        /// </summary>
        public string UpdateUser { get; set; }
        /// <summary>
        /// 更新日期
        /// </summary>
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}