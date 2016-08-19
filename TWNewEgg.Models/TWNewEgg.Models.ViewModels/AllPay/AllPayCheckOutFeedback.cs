using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.AllPay
{
    public class AllPayCheckOutFeedback
    {

        /// <summary>
        /// 廠商編號
        /// </summary>
        public string MerchantID { get; set; }
        /// <summary>
        /// 廠商交易編號
        /// </summary>
        public string MerchantTradeNo { get; set; }
        /// <summary>
        /// PaymentDate
        /// </summary>
        public string PaymentDate { get; set; }
        /// <summary>
        /// 會員選擇的付款方式
        /// </summary>
        public string PaymentType { get; set; }
        /// <summary>
        /// 通路費
        /// </summary>
        public string PaymentTypeChargeFee { get; set; }
        /// <summary>
        /// 交易狀態
        /// </summary>
        public string RtnCode { get; set; }
        /// <summary>
        /// 交易訊息
        /// </summary>
        public string RtnMsg { get; set; }
        /// <summary>
        /// 是否為模擬付款1：模擬付款  0：非模擬付款
        /// </summary>
        public string SimulatePaid { get; set; }      
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

    }
}
