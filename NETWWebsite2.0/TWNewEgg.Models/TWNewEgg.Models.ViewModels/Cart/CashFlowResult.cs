using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Cart
{
    /// <summary>
    /// 金流交易結果的統一性訊息物件
    /// </summary>
    public class CashFlowResult
    {
        public enum TradeMethodOption
        {
            Unknown = 0,
            CredicCard = 1,
            WebATM = 2,
            貨到付款 = 3,
            儲值支付 = 4,
            超商付款 = 5,
            電匯 = 6,
            ATM = 7
        }

        public CashFlowResult()
        {
            //設定預設值
            this.TradeResult = false;
            this.TradeMethod = (int)CashFlowResult.TradeMethodOption.Unknown;
        }

        /// <summary>
        /// 金流交易結果, true:成功, false:失敗
        /// </summary>
        public bool TradeResult { get; set; }

        /// <summary>
        /// 金流交易模式, refer CashFlowResult.TradeMethodOption
        /// </summary>
        public int TradeMethod { get; set; }

        /// <summary>
        /// 金流服務商
        /// </summary>
        public string Paytype { get; set; }

        /// <summary>
        /// 金流服務商回應訊息
        /// </summary>
        public string PaytypeReturnMsg { get; set; }

        /// <summary>
        /// 金流服務商回應代碼
        /// </summary>
        public string PaytypeReturnCode { get; set; }

        /// <summary>
        /// 金流服務商的交易代碼
        /// </summary>
        public string PaytypeAuthCode { get; set; }

        /// <summary>
        /// NewEgg自訂的回應訊息
        /// </summary>
        public string SystemMessage { get; set; }
    }
}
