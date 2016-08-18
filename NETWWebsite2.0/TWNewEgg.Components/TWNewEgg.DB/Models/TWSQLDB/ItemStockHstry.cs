using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("itemstockhstry")]
    public class ItemStockHstry
    {
        [Key]
        public int ID { get; set; }
        [DisplayName("產品序號")]
        public int ProductID { get; set; }
        [DisplayName("舊庫存數量")]
        public int OldQty { get; set; }
        [DisplayName("新庫存數量")]
        public int NewQty { get; set; }
        [DisplayName("舊安全庫存量")]
        public int OldSafeQty { get; set; }
        [DisplayName("新安全庫存數量")]
        public int NewSafeQty { get; set; }
        [DisplayName("更新前訂單取消是否回補")]
        public int Oldfdbcklmt { get; set; }
        [DisplayName("更新後訂單取消是否回補")]
        public int Newfdbcklmt { get; set; }
        [DisplayName("更新者")]
        public string UpdateUser { get; set; }
        [DisplayName("此修改創建日期")]
        public System.DateTime CreateDate { get; set; }
    }
}