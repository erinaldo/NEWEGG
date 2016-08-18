using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace TWNewEgg.Models.DomainModels.SellerFinance
{
    public class SellerFinanceDetail_Domain_Vendor_Portal // : TWNewEgg.DB.TWBACKENDDB.Models.Seller_FinanDetail
    {
        /// <summary>
        /// 發票狀態
        /// </summary>
        public string FinanStatus { get; set; }

        //TWNewEgg.DB.TWBACKENDDB.Models.Seller_FinanDetail
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
    public class MaterShowTop
    {
        [DisplayName("帳單年月份")]
        public string SettleMonth { get; set; }
        [DisplayName("帳單編號")]
        public string SettlementID { get; set; }
        [DisplayName("結算日期")]
        public DateTime SettleDate { get; set; }
        [DisplayName("結算起始日")]
        public DateTime DateStart { get; set; }
        [DisplayName("結算迄日")]
        public DateTime DateEnd { get; set; }
        [DisplayName("付款方式")]
        public string BillingCycle { get; set; }
        [DisplayName("匯款日期")]
        public Nullable<System.DateTime> RemitDate { get; set; }
        [DisplayName("發票日期")]
        public Nullable<System.DateTime> InvoDate { get; set; }
        [DisplayName("發票號碼")]
        public string InvoNumber { get; set; }

        [DisplayName("客戶訂單編號")]
        public string SoNumber { get; set; }
        [DisplayName("發票金額")]
        public decimal InvoPrice { get; set; }

        [DisplayName("供應商相關資訊")]
        public SellerInfo_IDNAMESAP sellerInfo_IDNAMESAP { get; set; }
        [DisplayName("供應商名稱")]
        public string sellerName { get; set; }

        [DisplayName("採購總額(未稅)")]
        public decimal POPrice { get; set; }
        [DisplayName("採購稅額")]
        public decimal POTax { get; set; }

        [DisplayName("本期退貨總額(未稅)")]
        public decimal RMAPrice { get; set; }
        [DisplayName("退貨稅額")]
        public decimal RMATax { get; set; }

        [DisplayName("本期寄倉費用總額(未稅)")]
        public decimal WarehousePrice { get; set; }
        [DisplayName("寄倉稅額")]
        public decimal WarehouseTax { get; set; }

        [DisplayName("合計總額(未稅)")]
        public decimal TotalAmount { get; set; }
        [DisplayName("營業稅")]
        public decimal TotalTax { get; set; }
        [DisplayName("本期應付(應收)總額(含稅)")]
        public decimal PaymentAmount { get; set; }
    }
    public class SellerInfo_IDNAMESAP
    {
        public int sellerid { get; set; }
        public string sellerName { get; set; }
        public string Sap { get; set; }
    }
    public class ReturnDetailAndNeedModel
    {
        public IEnumerable<SellerFinanceDetail_Domain_Vendor_Portal> sellerFinanceDetail_Domain_Vendor_Portal { get; set; }
        public MaterShowTop mastershowTop { get; set; }
    }
}
