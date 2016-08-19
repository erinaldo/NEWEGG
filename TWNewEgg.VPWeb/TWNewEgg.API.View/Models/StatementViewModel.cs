using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TWNewEgg.API.View
{

    public class DetailForOrderReturnWarehouse: TWNewEgg.API.Models.basicDomain
    {
    }

    public class Detail
    {
        [DisplayName("對帳單年月份")]
        public string SettleMonth { get; set; }
        [DisplayName("對帳單編號")]
        public string SettlementID { get; set; }
        [DisplayName("結算日期")]
        public string SettleDate { get; set; }
        [DisplayName("結算日期區間(起)")]
        public string DateStart { get; set; }
        [DisplayName("結算日期區間(迄)")]
        public string DateEnd { get; set; }
        [DisplayName("付款方式")]
        public string PayType { get; set; }
        [DisplayName("匯款日期")]
        public string RemitDate { get; set; }
        [DisplayName("發票日期")]
        public string InvoDate { get; set; }
        [DisplayName("發票號碼")]
        public string InvoNumber { get; set; }

        /// <summary>
        /// 商家編號
        /// </summary>
        public int SellerID { get; set; }

        /// <summary>
        /// 是否開放使用者押發票
        /// </summary>
        public bool IsOpenInvoice { get; set; }
    }

    public class SubFooter
    {
        public SubFooter()
        {
            Count = 0;
            POPrice = 0;
            Potax = 0;
            Subtotal = 0;
        }
        [DisplayName("筆數")]
        public int Count { get; set; }
        [DisplayName("採購總額(未稅)")]
        public decimal POPrice { get; set; }
        [DisplayName("稅額")]
        public decimal Potax { get; set; }
        [DisplayName("小計")]
        public decimal Subtotal { get; set; }

    }

    public class MainFooter
    {

        public MainFooter()
        {
            TotalAmount = 0;
            TotalTax = 0;
            PaymentAmount = 0;
            InvoicePrice = 0;

            //依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160720
            TotalAmount2 = 0;
            //調整項的資料筆數
            TotalAmount2_Data_Records = 0;
        }

        [DisplayName("合計金額(未稅)")]
        public decimal TotalAmount { get; set; }

        [DisplayName("發票金額(未稅)")]
        public decimal InvoicePrice { get; set; }

        [DisplayName("營業稅")]
        public decimal TotalTax { get; set; }
        [DisplayName("本期應付總額(含稅)")]
        public decimal PaymentAmount { get; set; }
        [DisplayName("倉儲物流費用(含稅)")]
        public decimal SubWarehoursetotal { get; set; }

        //依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160720
        [DisplayName("調整項金額(含稅)")]
        public decimal TotalAmount2 { get; set; }
        //調整項的資料筆數
        public int TotalAmount2_Data_Records { get; set; }

    }
}