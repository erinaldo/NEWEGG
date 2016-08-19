using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.API.Models
{
    public class SummarySPResult
    {   
        /// <summary>
        /// 商品價格
        /// </summary>
        public decimal ProductPrice { get; set; }

        /// <summary>
        /// 運費
        /// </summary>
        public decimal ShippingFee { get; set; }

        /// <summary>
        /// 退款
        /// </summary>
        public decimal Refund { get; set; }

        /// <summary>
        /// 每月的收費付款詳細項目
        /// </summary>
        public decimal ChargeNPayDetailsPerMonth { get; set; }

        /// <summary>
        /// 傭金費用
        /// </summary>
        public decimal CommissionFee { get; set; }

        /// <summary>
        /// 交易費用
        /// </summary>
        public decimal TransactionFee { get; set; }

        /// <summary>
        /// 倉儲費用
        /// </summary>
        public decimal StorageFee { get; set; }

        /// <summary>
        /// 總金額
        /// </summary>
        public decimal TotalAmount { get; set; }
    }
}
