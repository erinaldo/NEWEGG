using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace TWNewEgg.Models.DomainModels.SellerFinance
{
    public class SellerFianceExcel
    {
        //結算年月份
        [DisplayName("結算年月分")]
        public string SettleMonth { get; set; }
        //廠商代碼
        [DisplayName("廠商代碼")]
        public int SellerID { get; set; }
        //廠商名稱
        [DisplayName("廠商名稱")]
        public string SellerName { get; set; }
        //帳戶名稱
        [DisplayName("帳戶名稱")]
        public string BeneficiaryName { get; set; }
        //結算年月日
        [DisplayName("結算年月日")]
        public string SettleYMD { get; set; }
        //對帳單編號
        [DisplayName("對帳單編號")]
        public string SettlementID { get; set; }
        //結算日期
        [DisplayName("結算日期")]
        public string SettleDate { get; set; }
        //開放狀態
        [DisplayName("開放狀態")]
        public string IsOpen { get; set; }
        //結算狀態
        [DisplayName("結算狀態")]
        public string FinanStatus { get; set; }
        //發票日期
        [DisplayName("發票日期")]
        //public DateTime? InvoDate { get; set; }
        public string InvoDate { get; set; }
        //發票號碼
        [DisplayName("發票號碼")]
        public string InvoNumber { get; set; }
        //匯款沖銷日期
        [DisplayName("匯款沖銷日期")]
        //public DateTime? RemitDate { get; set; }
        public string RemitDate { get; set; }

        //本期採購總額(未稅)decimal
        [DisplayName("本期採購總額(未稅)")]
        public int POPrice { get; set; }

        //採購稅額decimal
        [DisplayName("採購稅額")]
        public int POTax { get; set; }

        //本期採購總額(含稅)decimal
        [DisplayName("本期採購總額(含稅)")]
        public int SUMPOPrice { get; set; }

        //本期退貨總額(未稅)decimal
        [DisplayName("本期退貨總額(未稅)")]
        public int RMAPrice { get; set; }

        //退貨稅額decimal
        [DisplayName("退貨稅額")]
        public int RMATax { get; set; }

        //本期退貨總額(含稅)decimal
        [DisplayName("本期退貨總額(含稅)")]
        public int SUMRMAPrice { get; set; }

        //進貨總金額decimal
        [DisplayName("進貨總金額(含稅)")]
        public int POstockPrice { get; set; }

        //應收寄倉費用總額(未稅)decimal
        [DisplayName("應收寄倉費用總額(未稅)")]
        public int WarehousePrice { get; set; }

        //寄倉費用稅額decimal
        [DisplayName("寄倉費用稅額")]
        public int WarehouseTax { get; set; }

        //應收寄倉費用總額(含稅)decimal
        [DisplayName("應收寄倉費用總額(含稅)")]
        public int SUMWarehousePrice { get; set; }

        //本期應付(應收)總計金額(含稅)decimal
        [DisplayName("本期應付(應收)總計金額(含稅)")]
        public int SUMPrice { get; set; }

    }
}
