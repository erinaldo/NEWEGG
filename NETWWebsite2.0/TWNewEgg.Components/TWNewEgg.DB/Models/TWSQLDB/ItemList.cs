using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("itemlist")]
    public class ItemList
    {
        public enum status
        {
            未上架 = 1,
            已上架 = 0
        };

        public ItemList()
        {
            DateTime defaultDate = DateTime.Parse("1990/01/01");
            CreateDate = DateTime.Now;
            UpdateDate = DateTime.Now;
            ID = 0;
            ItemID = 0;
            ItemlistGroupID = 0;
            ItemlistProductID = 0;
            ItemlistID = 0;
            ProdcutcostID = 0;
            Name = "";
            Sdesc = "";
            Cost = 0;
            Price = 0;
            Pricehpinst1 = 0;
            Pricehpinst2 = 0;
            Priceship = 0;
            Qty = 0;
            SafeQty = 0;
            QtyReg = 0;
            QtyLimit = 0;
            Photo = "";
            ItemlistOrder = 0;
            Note = "";
            Status = 0;
            UpdateUser = "";
        }
        /// <summary>
        /// 配件編號
        /// </summary>
        [Key]
        public int ID { get; set; }
        public Nullable<int> ItemlisttempID { get; set; }
        /// <summary>
        /// 主件商品編號
        /// </summary>
        public int ItemID { get; set; }
        /// <summary>
        /// 配件分類編號
        /// </summary>
        public int ItemlistGroupID { get; set; }
        public int ItemlistProductID { get; set; }
        public int ItemlistID { get; set; }
        public Nullable<int> ProdcutcostID { get; set; }
        /// <summary>
        /// 配件名聲
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 簡短說明
        /// </summary>
        public string Sdesc { get; set; }
        /// <summary>
        /// 成本
        /// </summary>
        public decimal Cost { get; set; }
        /// <summary>
        /// 售價
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 服務費
        /// </summary>
        public decimal ServicePrice { get; set; }
        /// <summary>
        /// 分期利息 1
        /// </summary>
        public decimal Pricehpinst1 { get; set; }
        /// <summary>
        /// 分期利息 2
        /// </summary>
        public decimal Pricehpinst2 { get; set; }
        /// <summary>
        /// 運費
        /// </summary>
        public decimal Priceship { get; set; }
        /// <summary>
        /// 數量
        /// </summary>
        public int Qty { get; set; }
        /// <summary>
        /// 安全庫存數量
        /// </summary>
        public int SafeQty { get; set; }
        /// <summary>
        /// 登記數量
        /// </summary>
        public int QtyReg { get; set; }
        /// <summary>
        /// 限購數量
        /// </summary>
        public int QtyLimit { get; set; }
        /// <summary>
        /// 配件圖檔名稱
        /// </summary>
        public string Photo { get; set; }
        /// <summary>
        /// 配件項目排序
        /// </summary>
        public int ItemlistOrder { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 建立日期
        /// </summary>
        public System.DateTime CreateDate { get; set; }
        /// <summary>
        /// 建立使用者
        /// </summary>
        public string CreateUser { get; set; }
        public int Updated { get; set; }
        /// <summary>
        /// 更新日期
        /// </summary>
        public System.DateTime UpdateDate { get; set; }
        /// <summary>
        /// 更新者
        /// </summary>
        public string UpdateUser { get; set; }

    }
}