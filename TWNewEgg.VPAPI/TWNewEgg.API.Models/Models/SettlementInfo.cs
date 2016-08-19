using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.API.Models.Models;

namespace TWNewEgg.API.Models
{
    public class SettlementInfo 
    {
        //Ben exec SP Begin
        //舊的名稱SummarySPResult，新的名稱SettlementInfo
        //參考至VS2010 Newegg.UIH.WebHost
        //public class SettlementInfo : SettlementBasicInfo
        //[DisplayName("商品價格")]
        public decimal ProductPrice { get; set; }

        //[DisplayName("運費")]
        public decimal ShippingFee { get; set; }

        //[DisplayName("退款")]
        public decimal Refund { get; set; }
        
        //[DisplayName("每月的收費付款詳細項目")]
        public decimal ChargeNPayDetailsPerMonth { get; set; }

        //[DisplayName("傭金費用")]
        public decimal CommissionFee { get; set; }

        //[DisplayName("交易費用")]
        public decimal TransactionFee { get; set; }

        //[DisplayName("倉儲費用")]
        public decimal StorageFee { get; set; }

        //[DisplayName("總金額")]
        public decimal TotalAmount { get; set; }
        
    }
}
