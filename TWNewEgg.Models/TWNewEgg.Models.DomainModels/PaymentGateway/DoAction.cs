using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.PaymentGateway
{
    public class DoAction
    { 
        /// <summary>
        /// 針對訂單做處理的動作。(C：關帳、R：退刷、E：取消、N：放棄)
        /// </summary>
        public enum ActionType{
            C = 0, 
            R = 1, 
            E = 2, 
            N =3  
        }

        /// <summary>
        /// 廠商的交易編號。
        /// </summary>
        public string MerchantTradeNo {get; set;}
        /// <summary>
        /// AllPay的交易編號
        /// </summary>
        public string TradeNo {get; set;}
        /// <summary>
        /// 針對訂單做處理的動作。(C：關帳、R：退刷、E：取消、N：放棄)
        /// </summary>
        public ActionType sActionType {get; set;} 
        /// <summary>
        /// 訂單總金額
        /// </summary>
        public Decimal TotalAmount  {get; set;} 
        /// <summary>
        /// AllPay提供的廠商編號
        /// </summary>
        public string MerchantID  {get; set;} 
        /// <summary>
        /// 交易狀態
        /// </summary>
        public int RtnCode  {get; set;} 
        /// <summary>
        /// 商品名稱
        /// </summary>
        public string RtnMsg  {get; set;} 


    }
}
