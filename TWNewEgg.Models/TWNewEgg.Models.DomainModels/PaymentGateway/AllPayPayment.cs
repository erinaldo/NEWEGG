using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.Models.DomainModels.PaymentGateway;
namespace TWNewEgg.Models.DomainModels.PaymentGateway
{
    public class AllPayPayment
    {   
        /// <summary>
        /// 付款方式
        /// </summary>
        public int paymentType { get; set; }

        /// <summary>
        /// 訂單商品資料
        /// </summary>
        public List<AllPayItem> AllPayItem { get; set; }

        /* 基本參數 */
        /// <summary>
        /// 收到付款完成通知的伺服器端網址
        /// </summary>
        public string ReturnURL{ get; set; }
        
        /// <summary>
        /// 歐付寶返回按鈕導向的瀏覽器端網址
        /// </summary>
        public string ClientBackURL { get; set; }

        /// <summary>
        /// 收到付款完成通知的瀏覽器端網址
        /// </summary>
        public string OrderResultURL{ get; set; }

        /// <summary>
        /// 此筆訂單交易編號
        /// </summary>
        public string MerchantTradeNo { get; set; }

        /// <summary>
        /// 您此筆訂單的交易總金額
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 該筆訂單的描述
        /// </summary>
        public string TradeDesc { get; set; }



        /// <summary>
        /// 備註欄位
        /// </summary>
        public string Remark { get; set; }

        //Web ATM

        /// <summary>
        /// 允許繳費天數
        /// </summary>
        public int ExpireDate { get; set; }
        /// <summary>
        /// 伺服器回傳付款相關資訊
        /// </summary>
        public string PaymentInfoURL { get; set; }
        /// <summary>
        /// 收到虛擬帳號相關資訊的伺服器端網址
        /// </summary>
        public string ClientRedirectURL { get; set; }


        //Credit

        /// <summary>
        /// 刷卡分期數(預設不提供分期，預設0)
        /// </summary>
        public int CreditInstallment { get; set; }

        /// <summary>
        /// 刷卡分期的付款金額(預設不提供分期，預設0)
        /// </summary>
        public decimal InstallmentAmount { get; set; }

        /// <summary>
        /// 是否使用紅利折抵(是True; 否False)
        /// </summary>
        public bool Redeem { get; set; }

        /// <summary>
        /// 是否為聯營卡(是True; 否False)
        /// </summary>
        public bool UnionPay { get; set; }

    }
}
