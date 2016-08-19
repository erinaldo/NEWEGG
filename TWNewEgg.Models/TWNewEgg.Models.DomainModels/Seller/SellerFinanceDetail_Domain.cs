using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace TWNewEgg.Models.DomainModels.SellerFinance
{
    public class SellerFinanceDetail_Domain
    {
        public SellerFinanceDetail_Domain()
        {
            this.IsCheck = "N";
            this.CartDate = DateTime.UtcNow.AddHours(8);
            this.InUserID = "System";
        }
        public int SN { get; set; }
        public string SettlementID { get; set; }
        public string IsCheck { get; set; }
        public int SettleType { get; set; }
        public string OrderID { get; set; }
        public string OrderDetailID { get; set; }
        public Nullable<System.DateTime> CartDate { get; set; }
        public Nullable<System.DateTime> TrackDate { get; set; }
        public Nullable<System.DateTime> RMADate { get; set; }
        public string POID { get; set; }
        public int SellerID { get; set; }
        public string SellerProductID { get; set; }
        public string BaseCurrency { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public Nullable<int> Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UnitTax { get; set; }
        public decimal SumPrice { get; set; }
        public decimal SumTax { get; set; }
        public Nullable<decimal> Size { get; set; }
        public Nullable<decimal> ShipFee { get; set; }
        public Nullable<decimal> ShipTax { get; set; }
        public Nullable<decimal> LogisticAmount { get; set; }
        public Nullable<decimal> LogisticTax { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUserID { get; set; }
        public Nullable<System.DateTime> InDate { get; set; }
        public string InUserID { get; set; }
    }

}
