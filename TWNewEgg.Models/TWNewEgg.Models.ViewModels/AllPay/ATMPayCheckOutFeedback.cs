using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.AllPay
{
    public class ATMPayCheckOutFeedback
    {
        /* 使用 ATM 交易時，回傳的參數 */

        /// <summary>
        /// 廠商編號
        /// </summary>
        public string MerchantID { get; set; }
        /// <summary>
        /// 廠商交易編號
        /// </summary>
        public string MerchantTradeNo { get; set; }
        /// <summary>
        /// 會員選擇的付款方式
        /// </summary>
        public string PaymentType { get; set; }
        /// <summary>
        /// 交易狀態
        /// </summary>
        public string RtnCode { get; set; }
        /// <summary>
        /// 交易訊息
        /// </summary>
        public string RtnMsg { get; set; }
        /// <summary>
        /// 交易金額
        /// </summary>
        public string TradeAmt { get; set; }
        /// <summary>
        /// 訂單成立時間
        /// </summary>
        public string TradeDate { get; set; }
        /// <summary>
        /// AllPay的交易編號
        /// </summary>
        public string TradeNo { get; set; }
       /// <summary>
       /// 繳費銀行代碼 
       /// </summary>
        public string BankCode { get; set; }
        /// <summary>
        /// 繳費虛擬帳號
        /// </summary>
        public string VirtualAccount { get; set; }
        /// <summary>
        /// 繳費期限
        /// </summary>
        public string ExpireDate { get; set; }

    }
}
