using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TWNewEgg.Models.DomainModels.SellerFinance
{
    /// <summary>
    /// 對帳單主單
    /// </summary>
    public class MainStatement
    {
        public string BaseCurrency { get; set; }

        /// <summary>
        /// 結算日期(起)
        /// </summary>
        public DateTime DateStart { get; set; }

        /// <summary>
        /// 結算日期(迄)
        /// </summary>
        public DateTime DateEnd { get; set; }


        /// <summary>
        /// 匯款沖銷日期(起)
        /// </summary>
        public DateTime StartRemitDate { get; set; }

        /// <summary>
        /// 匯款沖銷日期(迄)
        /// </summary>
        public DateTime EndRemitDate { get; set; }
        /// <summary>
        /// 
        /// 建檔日期
        /// </summary>
        public DateTime? InDate { get; set; }

        /// <summary>
        /// 建檔人員
        /// </summary>
        public string InUserID { get; set; }

        /// <summary>
        /// 發票日期
        /// </summary>
        public DateTime? InvoDate { get; set; }

        /// <summary>
        /// 發票號碼
        /// </summary>
        public string InvoNumber { get; set; }

        /// <summary>
        /// 是否開放 Vendor Portal 查詢
        /// </summary>
        public string IsOpen { get; set; }

        /// <summary>
        /// 本期應付(應收)總額(含稅)
        /// </summary>
        public decimal PaymentAmount { get; set; }

        /// <summary>
        /// 採購總額(未稅)
        /// </summary>
        public decimal POPrice { get; set; }

        /// <summary>
        /// 採購稅額
        /// </summary>
        public decimal POTax { get; set; }

        /// <summary>
        /// 匯款日期
        /// </summary>
        public DateTime? RemitDate { get; set; }

        /// <summary>
        /// 退款總額(未稅)
        /// </summary>
        public decimal RMAPrice { get; set; }

        /// <summary>
        /// 退款稅額
        /// </summary>
        public decimal RMATax { get; set; }

        /// <summary>
        /// 商家編號
        /// </summary>
        public int SellerID { get; set; }

        /// <summary>
        /// 商家名稱
        /// </summary>
        public string SellerName { get; set; }

        public string SettleCurrency { get; set; }

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
        /// 修改日期
        /// </summary>
        public DateTime? UpdateDate { get; set; }

        /// <summary>
        /// 修改人員
        /// </summary>
        public string UpdateUserID { get; set; }

        /// <summary>
        /// 寄倉總額(未稅)
        /// </summary>
        public decimal WarehousePrice { get; set; }

        /// <summary>
        /// 寄倉稅額
        /// </summary>
        public decimal WarehouseTax { get; set; }

        /// <summary>
        /// 結帳狀態
        /// </summary>
        public string FinanStatus { get; set; }

        /// <summary>
        /// 帳戶名稱
        /// </summary>
        public string BeneficiaryName { get; set; }

        //依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160801
        public decimal TotalAmount2 { get; set; }
        

    }

    /// <summary>
    /// 對帳單主單查詢條件
    /// </summary>
    public class MainStatementSearchCondition
    {
        /// <summary>
        /// 結算日期(起)
        /// </summary>
        public DateTime? DateStart { get; set; }

        /// <summary>
        /// 結算日期(迄)
        /// </summary>
        public DateTime? DateEnd { get; set; }

        /// <summary>
        /// 匯款沖銷日期(起)
        /// </summary>
        public DateTime? StartRemitDate { get; set; }

        /// <summary>
        /// 匯款沖銷日期(迄)
        /// </summary>
        public DateTime? EndRemitDate { get; set; }
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

        public string FinanStatus { get; set; }

        public string IsOpen { get; set; }

        public MainStatementSearchCondition()
        {
            DateStart = null;
            DateEnd = null;
            PageInfo = null;
            SellerID = -1;
            SettlementID = string.Empty;
            WhosCall = WhosCall.VendorPortal;
            FinanStatus = null;
            IsOpen = null;
        }
    }

    public class PageInfo
    {
        /// <summary>
        /// Number of rows in one page
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Current page index
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// Total page count
        /// </summary>
        public int TotalPage { get; set; }
    }

    /// <summary>
    /// 資料需求方
    /// </summary>
    public enum WhosCall
    { 
        VendorPortal,
        IPP
    }

    /// <summary>
    /// 商家資訊
    /// </summary>
    public class SellerInfo
    {
        /// <summary>
        /// 商家 ID
        /// </summary>
        public int SellerID { get; set; }

        /// <summary>
        /// 商家名稱
        /// </summary>
        public string SellerName { get; set; }
    }
}
