using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("invoiceprizefooter")]
    public class InvoicePrizeFooter
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        /// <summary>
        /// 主檔代號 (Main) 
        /// </summary>
        public string Main { get; set; }
        /// <summary>
        /// 發票所屬年月(InvoiceYYYMM)
        /// </summary>
        public string InvoiceYYYMM { get; set; }
        /// <summary>
        /// 總記錄數(TotRecordCnt) 
        /// </summary>
        public int TotRecordCnt { get; set; }
        /// <summary>
        /// 總中獎獎金金額(TotPrizeAmt)
        /// </summary>
        public decimal TotPrizeAmt { get; set; }
        /// <summary>
        /// 領獎期間-起始日期(RecAwardBegin)
        /// </summary>
        public string RecAwardBegin { get; set; }
        /// <summary>
        /// 領獎期間-截止日期(RecAwardEnd)
        /// </summary>
        public string RecAwardEnd { get; set; }
    }
}
