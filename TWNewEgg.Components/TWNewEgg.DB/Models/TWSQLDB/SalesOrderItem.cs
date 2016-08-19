using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.DB;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("salesorderitem")]
    public class SalesOrderItem : ExtModels.IOrderItems
    {   
        /// <summary>
        /// 訂購人編號
        /// </summary>
        [Key]
        public string Code { get; set; }
        /// <summary>
        /// 訂購主單編號
        /// </summary>
        public string SalesorderCode { get; set; }
        /// <summary>
        /// 上架商品編號
        /// </summary>
        public int ItemID { get; set; }
        /// <summary>
        /// 上架商品配件編號
        /// </summary>
        public int ItemlistID { get; set; }
        /// <summary>
        /// ERP 販售品配件編號
        /// </summary>
        public int ProductID { get; set; }
        /// <summary>
        /// ERP 販售品配件編號
        /// </summary>
        public int ProductlistID { get; set; }
        /// <summary>
        /// 商品名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 單價
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 利息
        /// </summary>
        public Nullable<decimal> Priceinst { get; set; }
        /// <summary>
        /// 數量
        /// </summary>
        public int Qty { get; set; }
        /// <summary>
        /// 折扣券抵扣額
        /// </summary>
        public Nullable<decimal> Pricecoupon { get; set; }
        public string Coupons { get; set; }
        /// <summary>
        /// 購物金餘額
        /// </summary>
        public Nullable<int> RedmtkOut { get; set; }
        /// <summary>
        /// 購物金折抵
        /// </summary>
        public Nullable<int> RedmBLN { get; set; }
        /// <summary>
        /// 購物金回饋
        /// </summary>
        public Nullable<int> Redmfdbck { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public Nullable<int> Status { get; set; }
        /// <summary>
        /// 狀態備註
        /// </summary>
        public string StatusNote { get; set; }
        //建檔日期
        public Nullable<System.DateTime> Date { get; set; }
        /// <summary>
        /// 屬性
        /// </summary>
        public string Attribs { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string Note { get; set; }
        public Nullable<int> WftkOut { get; set; }
        public Nullable<int> WfBLN { get; set; }
        public Nullable<int> AdjPrice { get; set; }
        public string ActID { get; set; }
        public Nullable<int> ActtkOut { get; set; }
        public Nullable<int> ProdcutCostID { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<decimal> DisplayPrice { get; set; }
        public Nullable<decimal> DiscountPrice { get; set; }   // 折扣金額
        public Nullable<decimal> ShippingExpense { get; set; }
        public Nullable<decimal> ServiceExpense { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public Nullable<int> WarehouseID { get; set; }  //2013.12.20 add column by Bill
        public Nullable<decimal> ItemPriceSum { get; set; }
        // 分期利息
        public decimal InstallmentFee { get; set; }
        public string IsNew { get; set; }
        // 滿額贈分攤金額
        public decimal ApportionedAmount { get; set; }
        public Nullable<decimal> SupplyShippingCharge { get; set; }
        public Nullable<int> itemCategory { get; set; } //2015.04.20 add by Bill 紀錄購買的Item Category

        public SalesOrderItem()
        {
            DateTime defaultDate = DateTime.Parse("1990/01/01");
            CreateDate = defaultDate;
            this.DisplayPrice = 0;
            this.ShippingExpense = 0;
            this.ServiceExpense = 0;
            this.Tax = 0;
            this.InstallmentFee = 0;
            this.ApportionedAmount = 0;
        }

        /// <summary>
        /// convert proc to salesorderitem for /myNewegg/recentOrder
        /// </summary>
        /// <param name="proc">data from backend db</param>
        public SalesOrderItem(TWNewEgg.DB.TWBACKENDDB.Models.Process proc)
        {
            this.Code = proc.ID;
            this.SalesorderCode = proc.CartID;
            this.ItemID = proc.StoreID ?? 0;
            this.ItemlistID = proc.ItemlistID ?? 0;
            this.ProductID = proc.ProductID ?? 0;
            this.ProductlistID = proc.ProductlistID ?? 0;
            this.Name = proc.Title;
            this.Price = proc.Price ?? 0;
            this.Priceinst = proc.Priceinst;
            this.Qty = proc.Qty ?? 0;
            this.Pricecoupon = proc.Pricecoupon;
            this.RedmtkOut = proc.RedmtkOut;
            this.RedmBLN = proc.RedmBLN;
            this.Redmfdbck = proc.Redmfdbck;
            this.Status = proc.Status ?? 0;
            this.StatusNote = proc.StatusNote;
            this.Date = proc.Date;
            this.Attribs = proc.Attribs;
            this.Note = proc.OrderNote;
            this.WftkOut = proc.WftkOut;
            this.WfBLN = proc.WfBLN;
            this.AdjPrice = proc.ADJPrice;
            this.ActID = proc.ACTID;
            this.ActtkOut = proc.ActtkOut;
            this.CreateUser = proc.PROCUser;
            this.CreateDate = proc.CreateDate ?? DateTime.MinValue;
            this.Updated = proc.Updated;
            this.UpdateDate = proc.UpdateDate;
            this.UpdateUser = proc.UpdateUser;
            this.WarehouseID = proc.WarehouseID; //2013.12.20 add column by Bill
            this.ApportionedAmount = proc.ApportionedAmount;
            this.SupplyShippingCharge = proc.SupplyShippingCharge;
            this.itemCategory = proc.itemCategory;
        }
    }
}
