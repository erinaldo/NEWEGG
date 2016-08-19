using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TWNewEgg.API.Models
{
    public class SettlementSPResult
    {
        [DisplayName("結帳日期")]
        public string Settlement { get; set; }

        [DisplayName("結帳編號")]
        public string SettlementID { get; set; }

        [DisplayName("訂單總額")]
        public decimal OrdersTotal { get; set; }
        
        [DisplayName("退款總額")]
        public decimal RefundsTotal { get; set; }
        //public Nullable<decimal> RefundTotal { get; set; }

        [DisplayName("結算總額")]
        public decimal SettlementTotal { get; set; }

        [DisplayName("摘要(連結欄位)")]
        public string Summary { get; set; }

        [DisplayName("交易明細(連結欄位)")]
        public string Transactions { get; set; }


    }
}
