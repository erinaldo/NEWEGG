using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Answer
{
    public class SalesOrderItemInfo
    {
        public string Code { get; set; }
        public string SalesorderCode { get; set; }
        public int ItemID { get; set; }
        public int ItemlistID { get; set; }
        public int ProductID { get; set; }
        public int ProductlistID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public Nullable<decimal> Priceinst { get; set; }
        public int Qty { get; set; }
        public Nullable<decimal> Pricecoupon { get; set; }
        public string Coupons { get; set; }
        public Nullable<int> RedmtkOut { get; set; }
        public Nullable<int> RedmBLN { get; set; }
        public Nullable<int> Redmfdbck { get; set; }
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
    }
}
