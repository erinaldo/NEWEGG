using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("purchaseorderitem")]
    public class PurchaseOrderItem
    {
        public enum status
        {
            初始狀態 = 99,
            已成立 = 0,
            取消 = 1
        }
        public PurchaseOrderItem()
        {
            DateTime defaultDate = DateTime.Parse("1990/01/01");
            CreateDate = defaultDate;
        }
        [Key]
        public string Code { get; set; }
        /// <summary>
        /// Purchaseorder.Code
        /// </summary>
        public string PurchaseorderCode { get; set; }
        public string SalesOrderItemCode { get; set; }
        public string SellerOrderCode { get; set; }
        public Nullable<int> CurrencyID { get; set; }
        public int ItemID { get; set; }
        public int ItemlistID { get; set; }
        public int ProductID { get; set; }
        public int ProductlistID { get; set; }
        public Nullable<int> SellerID { get; set; }
        public string Name { get; set; }
        public Nullable<int> SourcecurrencyID { get; set; }
        public Nullable<decimal> LocalPriceinst { get; set; }
        public decimal Price { get; set; }
        public Nullable<decimal> Priceinst { get; set; }
        public int Qty { get; set; }
        public Nullable<decimal> PriceCoupon { get; set; }
        public string Coupons { get; set; }
        public Nullable<int> RedmtkOut { get; set; }
        public Nullable<int> RedmBLN { get; set; }
        public Nullable<int> RedmFDBCK { get; set; }
        public Nullable<int> Status { get; set; }
        public string StatusNote { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Attribs { get; set; }
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
        public Nullable<decimal> ImportCost { get; set; }
        public string CCCCode { get; set; }
        public Nullable<decimal> DutyRate { get; set; }
        public Nullable<decimal> ProductTax { get; set; }
        public Nullable<decimal> FullTaxValue { get; set; }
        public Nullable<decimal> ShippingFee { get; set; }
        public Nullable<int> WarehouseID { get; set; } //2013.12.20 add column by Bill
        public Nullable<decimal> SupplyShippingCharge { get; set; }
    }
}