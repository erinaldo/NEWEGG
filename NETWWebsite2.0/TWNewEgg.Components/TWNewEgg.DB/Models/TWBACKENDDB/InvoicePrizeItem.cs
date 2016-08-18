using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("invoiceprizeitem")]
    public class InvoicePrizeItem
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        /// <summary>
        /// Table InvoicePrizeFooter's ID
        /// </summary>
        public int InvoicePrizeFooterID { get; set; }
        /// <summary>
        /// 廠商總公 司統一編號(TCompanyBan)
        /// </summary>
        public string TCompanyBan { get; set; }
        /// <summary>
        /// 發票號碼-字軌(InvoiceAxle)
        /// </summary>
        public string InvoiceAxle { get; set; }
        /// <summary>
        /// 發票號碼-號碼(InvoiceNumber)
        /// </summary>
        public string InvoiceNumber { get; set; }
        /// <summary>
        /// 消費者資訊 賣方-營業人名稱(Name)
        /// </summary>
        public string BuyerName { get; set; }
        /// <summary>
        /// 消費者資訊 賣方-營業人統一編號(BAN)
        /// </summary>
        public string BuyerBAN { get; set; }
        /// <summary>
        /// 消費者資訊 賣方-營業人地址(Address)
        /// </summary>
        public string BuyerAddress { get; set; }
        /// <summary>
        /// 消費者資訊 開立日期 發票日期(InvoiceDate)
        /// </summary>
        public string BuyerInvoiceDate { get; set; }
        /// <summary>
        /// 消費者資訊 開立日期 發票時間(InvoiceTime)
        /// </summary>
        public string BuyerInvoiceTime { get; set; }
        /// <summary>
        /// 消費者資訊 發票金額 總計(TotalAmount)
        /// </summary>
        public decimal TotalAmount { get; set; }
        /// <summary>
        /// 載具類別號碼(CarrierType)
        /// </summary>
        public string CarrierType { get; set; }
        /// <summary>
        /// 載具類別名稱(CarrierName)
        /// </summary>
        public string CarrierName { get; set; }
        /// <summary>
        /// 載具顯碼id(CarrierIdClear)
        /// </summary>
        public string CarrierIdClear { get; set; }
        /// <summary>
        /// 載具隱碼id(CarrierIdHide)
        /// </summary>
        public string CarrierIdHide { get; set; }
        /// <summary>
        /// 四位隨機碼(RandomNumber)
        /// </summary>
        public string RandomNumber { get; set; }
        /// <summary>
        /// 中獎獎別(PrizeType)
        /// </summary>
        public string PrizeType { get; set; }
        /// <summary>
        /// 中獎獎金(PrizeAmt)
        /// </summary>
        public decimal PrizeAmt { get; set; }
        /// <summary>
        /// 買受人-營利事業統一編號(BBAN)
        /// </summary>
        public string BBAN { get; set; }
        /// <summary>
        /// 大平台已匯款註記(DepositMK)
        /// </summary>
        public string DepositMK { get; set; }
        /// <summary>
        /// 資料類別(DataType)
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// 例外代碼
        /// </summary>
        public string ExceptionCode { get; set; }
        /// <summary>
        /// 列印格式
        /// </summary>
        public string PrintFormat { get; set; }
        /// <summary>
        /// 唯一識別碼
        /// </summary>
        public string UniqueCode { get; set; }
    }
}
