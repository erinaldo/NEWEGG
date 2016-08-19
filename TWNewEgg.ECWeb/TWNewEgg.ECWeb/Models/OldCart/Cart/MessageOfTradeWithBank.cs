using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.Website.ECWeb.Service;

namespace TWNewEgg.Website.ECWeb.Models
{
    /// <summary>
    /// 此物件為交易完成後，頁面顯示的訊息
    /// 可依據各銀行的回傳值自行添加欄位
    /// </summary>
    public class MessageOfTradeWithBank
    {
        //建構函式
        public MessageOfTradeWithBank()
        {
        }

        #region 共用欄位
        /* ------ 共用欄位 ------*/
        /// <summary>
        /// 交易型態
        /// </summary>
        public Chinatrust_txType txType { get; set; }

        /// <summary>
        /// 交易錯誤代碼
        /// </summary>
        public string ErrCode { get; set; }   //交易錯誤代碼

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrDesc { get; set; }    //錯誤訊息

        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNumber { get; set; } //訂單編號

        /// <summary>
        /// 交易授權碼
        /// </summary>
        public string AuthCode { get; set; }    //交易授權碼

        /// <summary>
        /// 卡號末4碼
        /// </summary>
        public string PayerLastPin4Code { get; set; }   //卡號末4碼

        /// <summary>
        /// 執行狀態
        /// </summary>
        public string Status { get; set; }  //執行狀態

        /// <summary>
        /// 導回網址
        /// </summary>
        public string ResURL { get; set; }  //導回網址

        /// <summary>
        /// 總金額
        /// </summary>
        public string AuthAmt { get; set; } // 總金額

        /// <summary>
        /// MerID
        /// </summary>
        public string MerID { get; set; }

        public string LastError { get; set; }
        #endregion

        #region WebAtm用的欄位
        /* ------ WebAtm用的欄位 ------ */
        /// <summary>
        /// 手續費
        /// </summary>
        public string Fee { get; set; } //手續費

        /// <summary>
        /// 匯款帳號
        /// </summary>
        public string WebAtmAcc { get; set; }   //匯款帳號

        /// <summary>
        /// //匯款帳號
        public string PayerBandId { get; set; } //付款方銀行代碼
        /// </summary>
        #endregion

        #region 信用卡用的欄位
        /* ------信用卡用的欄位 ------*/
        /// <summary>
        /// 授權之交易序號
        /// </summary>
        public string XId { get; set; } //授權之交易序號

        /// <summary>
        /// 折抵金額
        /// </summary>
        public string OffsetAmt { get; set; }//折抵金額

        /// <summary>
        /// 原始訂單金額
        /// </summary>
        public string OriginalAmt { get; set; }//原始訂單金額

        /// <summary>
        /// 此次紅利交易的賺取點數
        /// </summary>
        public string AwardedPoint { get; set; }//此次紅利交易的賺取點數

        /// <summary>
        /// 分期的期數
        /// </summary>
        public string NumberOfPay { get; set; } //分期的期數

        /// <summary>
        /// 紅利折抵的產品代碼
        /// </summary>
        public string ProdCode { get; set; }    //紅利折抵的產品代碼

        /// <summary>
        /// 紅利交易的點數餘額
        /// </summary>
        public string PointBalance { get; set; }    //紅利交易的點數餘額
        #endregion
    }//end class
}//end namespace