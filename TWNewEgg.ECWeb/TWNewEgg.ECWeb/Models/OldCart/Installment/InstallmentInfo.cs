using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Website.ECWeb.Models
{
    public class InstallmentInfo
    {
        public InstallmentInfo()
        {
            this.BankID = 0;
            this.BankCode = String.Empty;
            this.BankName = String.Empty;
            this.PayTypeID = 0;
            this.InsRate = 0m;
            this.InsRateName = String.Empty;
            this.InsRateShowName = String.Empty;
            this.PayType0rateNum = 0;
            this.CouponPrice = 0m;
            this.OriginalPriceSum = 0m;
            this.TotalInsRateFees = 0m;
            this.NewPriceSum = 0m;
            this.SaveSuccess = false;
        }

        // 分期銀行ID
        public int BankID { get; set; }

        // 分期銀行代碼
        public string BankCode { get; set; }

        // 分期銀行名稱
        public string BankName { get; set; }

        // 付款模式ID
        public int PayTypeID { get; set; }

        // 分期利率名稱
        public string InsRateName { get; set; }

        // 分期利率顯示名稱
        public string InsRateShowName { get; set; }

        // 分期利率
        public decimal InsRate { get; set; }

        // 分期期數設定，對應PayType Table 中的 PayType0rateNum
        public int PayType0rateNum { get; set; }

        // 折價金額
        public decimal CouponPrice { get; set; }

        // 滿額贈折扣總金額
        public decimal DiscountAmount { get; set; }

        // 未扣除折價金額且不含利息總金額
        public decimal OriginalPriceSum { get; set; }

        // 利息
        public decimal TotalInsRateFees { get; set; }

        // 加入扣除折價金額後再加入利息的總金額
        public decimal NewPriceSum { get; set; }

        // 是否成功儲存至DB
        public bool SaveSuccess { get; set; }
    }
}