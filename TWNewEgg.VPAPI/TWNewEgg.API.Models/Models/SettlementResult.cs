using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class SettlementResult
    {
        [DisplayName("結帳日期")]
        public string SettlementDate { get; set; }

        [DisplayName("訂單總額")]
        public Nullable<decimal> OrderTotalAmount { get; set; }

        [DisplayName("退款總額")]
        public Nullable<decimal> RefundTotalAmount { get; set; }

        [DisplayName("結算總額")]
        public Nullable<decimal> SettlementTotalAmount { get; set; }

        [DisplayName("摘要(連結欄位)")]
        public string Summary { get; set; }

        [DisplayName("交易明細(連結欄位)")]
        public string Transactions { get; set; }
    }
}
