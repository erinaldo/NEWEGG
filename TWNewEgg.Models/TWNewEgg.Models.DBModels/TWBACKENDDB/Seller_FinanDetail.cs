using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWBACKENDDB
{
    [Table("Seller_FinanDetail")]
    public class Seller_FinanDetail
    {
        public enum SettleType_Identify
        {
            訂單 = 1,
            退貨 = 2,
            寄倉 = 3
        }

        //public enum OrderTypeEnum
        //{
        //    LBO,
        //    LBR
        //}
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int SN { get; set; }
        public string IsCheck { get; set; }
        public string SettlementID { get; set; }
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
        public decimal ShipFee { get; set; }
        public decimal ShipTax { get; set; }
        public decimal LogisticAmount { get; set; }
        public decimal LogisticTax { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUserID { get; set; }
        public Nullable<System.DateTime> InDate { get; set; }
        public string InUserID { get; set; }
    }
}
