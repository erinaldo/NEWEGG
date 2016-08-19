using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("itemlisttemp")]
    public class ItemListTemp
    {
        public enum status
        {
            未上架 = 1,
            已上架 = 0
        };

        public ItemListTemp()
        {
            DateTime defaultDate = DateTime.Parse("1990/01/01");
            CreateDate = DateTime.Now;
            UpdateDate = defaultDate;
        }

        [Key]
        public int ID { get; set; }
        public int ItemID { get; set; }
        public int ItemlisttempgroupID { get; set; }
        public int ItemlisttempProductID { get; set; }
        public int ItemlisttempID { get; set; }
        public Nullable<int> ProdcutCostID { get; set; }
        public string Name { get; set; }
        public string Sdesc { get; set; }
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public decimal ServicePrice { get; set; }
        public decimal Pricehpinst1 { get; set; }
        public decimal Pricehpinst2 { get; set; }
        public decimal Priceship { get; set; }
        public int Qty { get; set; }
        public int SafeQty { get; set; }
        public int QtyReg { get; set; }
        public int QtyLimit { get; set; }
        public string Photo { get; set; }
        public int ItemlisttempOrder { get; set; }
        public string Note { get; set; }
        public int Status { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public int Updated { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}