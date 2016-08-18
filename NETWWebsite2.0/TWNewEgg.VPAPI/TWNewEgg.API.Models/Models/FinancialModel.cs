using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    #region 主單

    /// <summary>
    /// 對帳單主單
    /// </summary>
    public class MainStatement
    {
        /// <summary>
        /// 結算日期(起)
        /// </summary>
        public DateTime DateStart { get; set; }

        /// <summary>
        /// 結算日期(迄)
        /// </summary>
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// 發票狀態
        /// </summary>
        public string FinanStatus { get; set; }

        /// <summary>
        /// 發票日期
        /// </summary>
        public DateTime? InvoDate { get; set; }

        /// <summary>
        /// 發票號碼
        /// </summary>
        public string InvoNumber { get; set; }

        /// <summary>
        /// 本期應付(應收)總額(含稅)
        /// </summary>
        public decimal PaymentAmount { get; set; }

        /// <summary>
        /// 匯款日期
        /// </summary>
        public DateTime? RemitDate { get; set; }

        /// <summary>
        /// 採購總額(未稅)
        /// </summary>
        public decimal POPrice { get; set; }

        /// <summary>
        /// 採購稅額
        /// </summary>
        public decimal POTax { get; set; }

        /// <summary>
        /// 採購總額
        /// </summary>
        public decimal POTotal { get; set; }

        /// <summary>
        /// 退款總額(未稅)
        /// </summary>
        public decimal RMAPrice { get; set; }

        /// <summary>
        /// 退款稅額
        /// </summary>
        public decimal RMATax { get; set; }

        /// <summary>
        /// 退貨總額
        /// </summary>
        public decimal RMATotal { get; set; }

        /// <summary>
        /// 商家
        /// </summary>
        /// <remarks>商家名稱(商家編號)</remarks>
        public string Seller { get; set; }

        /// <summary>
        /// 商家編號
        /// </summary>
        public int SellerID { get; set; }
        
        /// <summary>
        /// 結算日期
        /// </summary>
        public DateTime SettleDate { get; set; }

        /// <summary>
        /// 帳單編號
        /// </summary>
        public string SettlementID { get; set; }

        /// <summary>
        /// 帳單年月份
        /// </summary>
        public string SettleMonth { get; set; }

        /// <summary>
        /// 合計總額(未稅)
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 合計稅額
        /// </summary>
        public decimal TotalTax { get; set; }

        /// <summary>
        /// 寄倉總額(未稅)
        /// </summary>
        public decimal WarehousePrice { get; set; }

        /// <summary>
        /// 寄倉稅額
        /// </summary>
        public decimal WarehouseTax { get; set; }

        /// <summary>
        /// 寄倉收費總額
        /// </summary>
        public decimal WarehouseTotal { get; set; }

        //依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160801
        public decimal TotalAmount2 { get; set; }
        public decimal PurePrice { get; set; }
        public decimal TotalTax2 { get; set; }

    }

    /// <summary>
    /// 對帳單主單查詢條件
    /// </summary>
    public class MainStatementSearchCondition
    {
        /// <summary>
        /// 結算日期(起)
        /// </summary>
        public DateTime DateStart { get; set; }

        /// <summary>
        /// 結算日期(迄)
        /// </summary>
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// 是否具有管理員權限
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// 是否已開立發票
        /// </summary>
        public bool? IsInvoiced { get; set; }

        /// <summary>
        /// 分頁資訊
        /// </summary>
        public PageInfo PageInfo { get; set; }

        /// <summary>
        /// 商家編號
        /// </summary>
        public int? SellerID { get; set; }

        /// <summary>
        /// 帳單編號
        /// </summary>
        public string SettlementID { get; set; }

        /// <summary>
        /// 資料需求方
        /// </summary>
        public WhosCall WhosCall { get; set; }

        public MainStatementSearchCondition()
        {
            SellerID = -1;
            SettlementID = string.Empty;
            DateTime now = DateTime.Now;
            DateStart = now;
            DateEnd = now;
            WhosCall = WhosCall.VendorPortal;
            PageInfo = null;
            IsInvoiced = null;
            IsAdmin = false;
        }
    }

    public enum WhosCall
    {
        VendorPortal,
        IPP
    }

    #endregion 主單

    public class financialModel
    {
        public List<basicDomain> basicDomain { get; set; }
        public MasterTopData masterTopData { get; set; }
    }

    public class MasterTopData
    {
        [DisplayName("對帳單年月份")]
        public string SettleMonth { get; set; }
        [DisplayName("對帳單編號")]
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
        public DateTime? RemitDate { get; set; }
        [DisplayName("發票日期")]
        public DateTime? InvoDate { get; set; }
        [DisplayName("發票號碼")]
        public string InvoNumber { get; set; }

        [DisplayName("供應商名稱")]
        public string sellerName { get; set; }
        [DisplayName("供應商相關資訊")]
        public SellerInfo_IDNAMESAP sellerInfo_IDNAMESAP { get; set; }


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
    public class basicDomain: TWNewEgg.DB.TWBACKENDDB.Models.Seller_FinanDetail
    {
        //匯出 EXCEL 用
        [DisplayName("品項類別")]
        public string categoryDescription { get; set; }

        /// <summary>
        /// 單價(含稅)
        /// </summary>
        public decimal UnitPrice_Total { get; set; }

        /// <summary>
        /// 單筆總額(含稅)
        /// </summary>
        public decimal SumPrice_Total { get; set; }

        /// <summary>
        /// 運費單筆總額(含稅)
        /// </summary>
        public decimal ShipFee_Total { get; set; }

        /// <summary>
        /// 出貨處理費單筆總額(含稅)
        /// </summary>
        public decimal LogisticAmount_Total { get; set; }

        /// <summary>
        /// 寄倉總額(含稅)
        /// </summary>
        /// <remarks>運費單筆總額(含稅) + 出貨處理費單筆總額(含稅)</remarks>
        public decimal WhereHouse_Total { get; set; }

        /// <summary>
        /// 退貨單訂單編號
        /// </summary>
        public string RegoodOrderID { get; set; }
    }
    public class FinancialExportExcel : financialModel
    {
        
    }
    public class SellerInfo_IDNAMESAP
    {
        public int sellerid { get; set; }
        public string sellerName { get; set; }
        public string Sap { get; set; }
    }

    /// <summary>
    /// 主單分類資訊
    /// </summary>
    public class CartCategory
    {
        /// <summary>
        /// 第 0 層
        /// </summary>
        public CategoryLayer Layer0 { get; set; }

        /// <summary>
        /// 第 1 層
        /// </summary>
        public CategoryLayer Layer1 { get; set; }

        /// <summary>
        /// 第 2 層
        /// </summary>
        public CategoryLayer Layer2 { get; set; }
        
        /// <summary>
        /// 主單編號
        /// </summary>
        public string CartID { get; set; }

        /// <summary>
        /// 賣場編號
        /// </summary>
        public int ItemID { get; set; }

        public CartCategory()
        {
            Layer0 = new CategoryLayer();
            Layer1 = new CategoryLayer();
            Layer2 = new CategoryLayer();
        }
    }

    /// <summary>
    /// 各層分類資訊
    /// </summary>
    public class CategoryLayer
    {
        /// <summary>
        /// 分類編號
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 中文名稱
        /// </summary>
        public string Name_TW { get; set; }
    }
}
