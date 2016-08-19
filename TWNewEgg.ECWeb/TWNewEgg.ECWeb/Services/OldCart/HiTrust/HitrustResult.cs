using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Website.ECWeb.Service
{
    public class HitrustResult
    {
        public HitrustResult()
        {
        }

        /// <summary>
        /// 訂單號碼
        /// </summary>
        public string ordernumber { get; set; }
        /// <summary>
        /// 交易結果
        /// </summary>
        public string retcode { get; set; }
        /// <summary>
        /// /幣號
        /// </summary>
        public string currency { get; set; }
        /// <summary>
        /// 訂單日期(YYYY-MM-DD.HH.MM.SS.XXXXXX)
        /// </summary>
        public string orderdate { get; set; }
        /// <summary>
        /// 訂單狀態碼
        /// </summary>
        public string orderstatus { get; set; }
        /// <summary>
        /// 核准金額
        /// </summary>
        public string approveamount { get; set; }
        /// <summary>
        /// 銀行授權碼
        /// </summary>
        public string authCode { get; set; }
        /// <summary>
        /// 銀行調單編號
        /// </summary>
        public string authRRN { get; set; }
        /// <summary>
        /// 請款金額
        /// </summary>
        public string depositamount { get; set; }
        /// <summary>
        /// 請款批次號碼
        /// </summary>
        public string paybatchnumber { get; set; }
        /// <summary>
        /// 請款日期(YYYY-MM-DD.HH.MM.SS.XXXXXX)
        /// </summary>
        public string capDate { get; set; }
        /// <summary>
        /// 退款金額
        /// </summary>
        public string credamount { get; set; }
        /// <summary>
        /// 退款批次號碼
        /// </summary>
        public string credbatchnumber { get; set; }
        /// <summary>
        /// 退款調單編號
        /// </summary>
        public string credRRN { get; set; }
        /// <summary>
        /// 退款授權碼
        /// </summary>
        public string credCode { get; set; }
        /// <summary>
        /// 退款日期
        /// </summary>
        public string creddate { get; set; }
        /// <summary>
        /// 授權方式(SSL, MIA, SET)
        /// </summary>
        public string eci { get; set; }
        /// <summary>
        /// 分期期數
        /// </summary>
        public string E06 { get; set; }
        /// <summary>
        /// 首期金額
        /// </summary>
        public string E07 { get; set; }
        /// <summary>
        /// 每期金額
        /// </summary>
        public string E08 { get; set; }
        /// <summary>
        /// 手續費
        /// </summary>
        public string E09 { get; set; }
        /// <summary>
        /// 點點變現金銷帳編號
        /// </summary>
        public string redemordernum { get; set; }
        /// <summary>
        /// 本次折抵點數
        /// </summary>
        public string redem_discount_point { get; set; }
        /// <summary>
        /// 本次折抵金額
        /// </summary>
        public string redem_discount_amount { get; set; }
        /// <summary>
        /// 本次實付金額
        /// </summary>
        public string redem_purchase_amount { get; set; }
        /// <summary>
        /// 剩餘點數
        /// </summary>
        public string redem_balance_point { get; set; }

        public string acquirer { get; set; }
        public string cardtype { get; set; }
    }//end class
}//end namespace