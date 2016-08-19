using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Web;
using System.Data;
using System.Web.Mvc;
using System.IO;
using System.Net;
using System.Drawing;
using TWNewEgg.Website.ECWeb.Models;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWBACKENDDB.Models;
using System.Data.SqlClient;
using System.Globalization;

namespace TWNewEgg.Website.ECWeb.Service
{
    public enum HitrustTradeType { WebATM = 0, CredicOnce = 1, CredicInstallments = 3 }
    public class HitrustService
    {
        private string m_strMerConfigName_CredicOnce = "";
        private string m_strMerConfigName_Installments = "";
        private string m_strSerConfigName = "";
        private string m_strUpdateUrl = "";
        private string m_strReturnURL = "";

        /// <summary>
        /// 建構函式
        /// </summary>
        public HitrustService()
        {
            string dtformat = "yyyy/MM/dd HH:mm:ss";
            string dt1 = System.Configuration.ConfigurationManager.AppSettings["Hitrust_NewLifeBegin"].ToString();
            int result;
            DateTime dtHitrust_NewLifeBegin;
            //StringToDateTime Sucess;
            if (DateTime.TryParseExact(dt1, dtformat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dtHitrust_NewLifeBegin))
            {
                DateTime dtNow = DateTime.Now;
                result = DateTime.Compare(dtNow, dtHitrust_NewLifeBegin);
                /* dt1 < dt2 ，rlt < 0
                   dt1 > dt2 ，rlt > 0 
                */
                if (result == -1)
                {
                    //設定所有初始值
                    //D:\Payment\hitrust\HiServer.conf"
                    this.m_strSerConfigName = System.Configuration.ConfigurationManager.AppSettings["Hitrust_SerConfigPath"].TrimEnd('\\') + "\\" + "HiServer.conf";
                    //D:\Payment\hitrust\61138\61138.conf
                    this.m_strMerConfigName_CredicOnce = System.Configuration.ConfigurationManager.AppSettings["Hitrust_MerConfigPath_CredicOnce"].TrimEnd('\\') + "\\" + System.Configuration.ConfigurationManager.AppSettings["Hitrust_MerId_CredicOnce"] + ".conf";
                    //D:\Payment\hitrust\61138\61138.conf
                    this.m_strMerConfigName_Installments = System.Configuration.ConfigurationManager.AppSettings["Hitrust_MerConfigPath_Installments"].TrimEnd('\\') + "\\" + System.Configuration.ConfigurationManager.AppSettings["Hitrust_MerId_Installments"] + ".conf";
                }
                if (result >= 0)
                {
                    //D:\\Work\\Web\\hitrust\HiServer.conf"
                    this.m_strSerConfigName = System.Configuration.ConfigurationManager.AppSettings["Hitrust_SerConfigPath_NewLife"].TrimEnd('\\') + "\\" + "HiServer.conf";
                    //D:\Payment\hitrust\61720\61720.conf
                    this.m_strMerConfigName_CredicOnce = System.Configuration.ConfigurationManager.AppSettings["Hitrust_MerConfigPath_CredicOnce_NewLife"].TrimEnd('\\') + "\\" + System.Configuration.ConfigurationManager.AppSettings["Hitrust_MerId_CredicOnce_NewLife"] + ".conf";
                    //D:\Payment\hitrust\61720\61720.conf
                    this.m_strMerConfigName_Installments = System.Configuration.ConfigurationManager.AppSettings["Hitrust_MerConfigPath_Installments_NewLife"].TrimEnd('\\') + "\\" + System.Configuration.ConfigurationManager.AppSettings["Hitrust_MerId_Installments_NewLife"] + ".conf";
                }
                //https://202.133.245.183/Paytype/Hitrust_ReturnMessage
                this.m_strUpdateUrl = System.Configuration.ConfigurationManager.AppSettings["ChinatrustFeedback"].TrimEnd('/') + System.Configuration.ConfigurationManager.AppSettings["Hitrust_UpdateURLPage"];
            }
            else
            {
                //如果AppSetting時間輸入錯誤，走61138.conf
                //D:\Payment\hitrust\HiServer.conf"
                this.m_strSerConfigName = System.Configuration.ConfigurationManager.AppSettings["Hitrust_SerConfigPath"].TrimEnd('\\') + "\\" + "HiServer.conf";
                //D:\Payment\hitrust\61138\61138.conf
                this.m_strMerConfigName_CredicOnce = System.Configuration.ConfigurationManager.AppSettings["Hitrust_MerConfigPath_CredicOnce"].TrimEnd('\\') + "\\" + System.Configuration.ConfigurationManager.AppSettings["Hitrust_MerId_CredicOnce"] + ".conf";
                //D:\Payment\hitrust\61138\61138.conf
                this.m_strMerConfigName_Installments = System.Configuration.ConfigurationManager.AppSettings["Hitrust_MerConfigPath_Installments"].TrimEnd('\\') + "\\" + System.Configuration.ConfigurationManager.AppSettings["Hitrust_MerId_Installments"] + ".conf";
                this.m_strUpdateUrl = System.Configuration.ConfigurationManager.AppSettings["ChinatrustFeedback"].TrimEnd('/') + System.Configuration.ConfigurationManager.AppSettings["Hitrust_UpdateURLPage"];
            }

        }

        /*
        oType.InvokeMember("MerConfigName", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { "E:/HiTRUSTasp/HiTrustConf/" + Request.Params["merid"] + ".conf" }); //商家config檔路徑
                oType.InvokeMember("SerConfigName", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { "E:/HiTRUSTasp/HiTrustConf/HiServer.conf" });//Server conf路徑
         */

        /// <summary>
        /// 商家一次付清的Conf路徑, EX: E:/HiTRUSTasp/HiTrustConf/61138.conf" 
        /// </summary>
        public string MerConfigName_CredicOnce { get { return this.m_strMerConfigName_CredicOnce; } }
        /// <summary>
        /// 商家分期付款的Conf路徑, EX: E:/HiTRUSTasp/HiTrustConf/61138.conf" 
        /// </summary>
        public string MerConfigName_Installments { get { return this.m_strMerConfigName_Installments; } }
        /// <summary>
        /// Server Conf 路徑, EX: E:/HiTRUSTasp/HiTrustConf/HiServer.conf
        /// </summary>
        public string SerConfigName { get { return this.m_strSerConfigName; } }
        /*
        /// <summary>
        /// 幣別(此函式只能從設定檔讀取, 程式內函式不可設定)
        /// </summary>
        public string currency { get { return ""; } }
         */
        /// <summary>
        /// 自動請款, 1:自動請款; 0:一般交易
        /// </summary>
        public string depositflag { get { return "1"; } }
        /// <summary>
        /// 啟動查詢, 1: 詳細資料, 0:一般資料
        /// (啟動詳細資料時, Trust Pay Server會將交易詳細資料以POST的方式送至MerUpdateURL)
        /// </summary>
        public string queryflag { get { return "1"; } }
        /// <summary>
        /// 指定接續網址
        /// </summary>
        public string returnURL { get { return this.m_strReturnURL; } }
        /// <summary>
        /// 交易結果網址 (啟動queryflag=1時, 交易資料會以POST方式傳至merupdateURL
        /// </summary>
        public string merupdateURL { get { return this.m_strSerConfigName; } }
        /// <summary>
        /// 加密結果回傳網址
        /// </summary>
        public string updateURL { get { return this.m_strUpdateUrl; } }
        /*
        /// <summary>
        /// 加密結果回傳網址(僅可由設定檔讀取, 程式無法設定)
        /// </summary>
        public string updateURL { get; set; }
         */

        /// <summary>
        /// 執行信用卡交易, 並且回傳交易結果
        /// </summary>
        /// <param name="arg_oSsl">Credit 的資訊</param>
        /// <param name="arg_TradeType">交易型態</param>
        /// <returns>交易結果</returns>
        public HitrustResult createCreditSslTransaction(HitrustSSL arg_oSsl, HitrustTradeType arg_TradeType)
        {
            if (arg_oSsl == null)
                return null;

            HitrustResult oResult = null;
            System.Type oType = System.Type.GetTypeFromProgID("HiB2CCom.eB2CCom.1");
            object tmp = System.Activator.CreateInstance(oType);
            log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

            try
            {
                //Setting your transaction informations
                //Merchant config path
                if (arg_TradeType.Equals(HitrustTradeType.CredicOnce))
                    oType.InvokeMember("MerConfigName", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { this.MerConfigName_CredicOnce });
                else if (arg_TradeType.Equals(HitrustTradeType.CredicInstallments))
                    oType.InvokeMember("MerConfigName", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { this.MerConfigName_Installments });
                //Server config path
                oType.InvokeMember("SerConfigName", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { this.SerConfigName });
                //Order Number
                oType.InvokeMember("ordernumber", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { arg_oSsl.ordernumber });
                //Amount
                oType.InvokeMember("amount", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { arg_oSsl.amount.ToString() });
                //Order Descript
                oType.InvokeMember("orderdesc", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { arg_oSsl.orderdesc });
                //自動請款 depositflag
                oType.InvokeMember("depositflag", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { this.depositflag });
                //啟動查詢(回傳詳細交易結果)
                oType.InvokeMember("queryflag", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { this.queryflag });
                //ticket no
                oType.InvokeMember("ticketno", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { "" });
                //With AuthSSL function, you could not to set returnURL.
                //oType.InvokeMember("returnURL", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { Request.Url.Scheme + "://" + Request.Url.Authority + "/" + Request.Url.Segments[1].Replace("/", "") + "/B2C_Return.aspx" }); 
                //With AuthSSL function, you could not to set merupdateURL.
                //oType.InvokeMember("merupdateURL", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { Request.Url.Scheme + "://" + Request.Url.Authority + "/" + Request.Url.Segments[1].Replace("/", "") + "/B2C_Update.aspx" }); 
                //加密結果回傳網址 updateURL
                //oType.InvokeMember("updateURL", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { this.updateURL });
                //Credit Card Number
                oType.InvokeMember("pan", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { arg_oSsl.pan });
                //Expiry Date
                oType.InvokeMember("expiry", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { arg_oSsl.expiry });
                //3-Digit Code
                oType.InvokeMember("E01", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { arg_oSsl.E01 });
                //若指定信用卡分期
                if (arg_TradeType.Equals(HitrustTradeType.CredicInstallments))
                {
                    //分期期數
                    arg_oSsl.E03 = Convert.ToInt32(arg_oSsl.E03).ToString().PadLeft(2, '0');
                    oType.InvokeMember("E03", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { arg_oSsl.E03 });
                    //紅利折抵: 不可與分期使用
                    oType.InvokeMember("E04", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { "0" });
                }

                //Call B2CAuthSSL
                oType.InvokeMember("B2CAuthSSL", System.Reflection.BindingFlags.InvokeMethod, null, tmp, null);

                //After transaction with B2CAuthSSL, you can get the result with below codes.
                //You can refer to out toolkit document with chapter Query.
                oResult = new HitrustResult();
                //訂單編號
                oResult.ordernumber = arg_oSsl.ordernumber;
                //交易結果代碼
                oResult.retcode = (string)oType.InvokeMember("retcode", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                oType.InvokeMember("B2CQuery", System.Reflection.BindingFlags.InvokeMethod, null, tmp, null);//呼叫查詢函式

                if (oResult.retcode.Equals("00"))
                {
                    //銀行授權碼
                    oResult.authCode = (string)oType.InvokeMember("authCode", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                    //銀行調單編號
                    oResult.authRRN = (string)oType.InvokeMember("authRRN", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                    //訂單狀態碼
                    oResult.orderstatus = (string)oType.InvokeMember("orderstatus", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                    //核准金額
                    oResult.approveamount = (string)oType.InvokeMember("approveamount", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                    //請款金額
                    oResult.depositamount = (string)oType.InvokeMember("depositamount", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                    //退款金額
                    oResult.credamount = (string)oType.InvokeMember("credamount", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                    //訂單日期(YYYY-MM-DD.HH.MM.SS.XXXXXX)
                    oResult.orderdate = (string)oType.InvokeMember("orderdate", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                    //請款日期(YYYY-MM-DD.HH.MM.SS.XXXXXX)
                    oResult.capDate = (string)oType.InvokeMember("capDate", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                    //幣別
                    oResult.currency = (string)oType.InvokeMember("currency", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                    //授權方式(SSL, MIA, SET)
                    oResult.eci = (string)oType.InvokeMember("eci", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                    //分期期數
                    oResult.E06 = (string)oType.InvokeMember("e06", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                    //首期金額
                    oResult.E07 = (string)oType.InvokeMember("e07", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                    //每期金額
                    oResult.E08 = (string)oType.InvokeMember("e08", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                    //手續費
                    oResult.E09 = (string)oType.InvokeMember("e09", System.Reflection.BindingFlags.GetProperty, null, tmp, null);

                    logger.Error("oResult.retcode=00");

                }
                else
                {
                    //因交易失敗, 可能導致有些訊息無法抓到, 故用try-catch包住, 能設定多少是多少
                    try
                    {
                        //銀行授權碼
                        oResult.authCode = (string)oType.InvokeMember("authCode", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                        //銀行調單編號
                        oResult.authRRN = (string)oType.InvokeMember("authRRN", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                        //訂單狀態碼
                        oResult.orderstatus = (string)oType.InvokeMember("orderstatus", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                        //核准金額
                        oResult.approveamount = (string)oType.InvokeMember("approveamount", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                        //請款金額
                        oResult.depositamount = (string)oType.InvokeMember("depositamount", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                        //退款金額
                        oResult.credamount = (string)oType.InvokeMember("credamount", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                        //訂單日期(YYYY-MM-DD.HH.MM.SS.XXXXXX)
                        oResult.orderdate = (string)oType.InvokeMember("orderdate", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                        //請款日期(YYYY-MM-DD.HH.MM.SS.XXXXXX)
                        oResult.capDate = (string)oType.InvokeMember("capDate", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                        //幣別
                        oResult.currency = (string)oType.InvokeMember("currency", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                        //授權方式(SSL, MIA, SET)
                        oResult.eci = (string)oType.InvokeMember("eci", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                        //分期期數
                        oResult.E06 = (string)oType.InvokeMember("e06", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                        //首期金額
                        oResult.E07 = (string)oType.InvokeMember("e07", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                        //每期金額
                        oResult.E08 = (string)oType.InvokeMember("e08", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                        //手續費
                        oResult.E09 = (string)oType.InvokeMember("e09", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                    }
                    catch
                    {
                    }

                    logger.Error("oResult.retcode=" + oResult.retcode);

                }
            }
            catch (Exception ex_Hitrust)
            {
                logger.Error("Hitrust_Error: " + ex_Hitrust.Message);
            }


            oType = null;
            return oResult;
        }// createCreditTransaction

        /// <summary>
        /// 用訂單編號查詢交易結果
        /// </summary>
        /// <param name="arg_strOrderNumber">訂單編號</param>
        /// <param name="arg_oTradeType">交易方式: 信用卡一次付清、信用卡分期</param>
        /// <returns></returns>
        public HitrustResult queryTrade(string arg_strOrderNumber, HitrustTradeType arg_oTradeType)
        {
            if (arg_strOrderNumber == null || arg_strOrderNumber.Length <= 0)
                return null;


            HitrustResult oResult = null;
            System.Type oType = System.Type.GetTypeFromProgID("HiB2CCom.eB2CCom.1");
            object tmp = System.Activator.CreateInstance(oType);



            //Setting your transaction informations
            //Merchant config path
            if (arg_oTradeType.Equals(HitrustTradeType.CredicOnce))
                oType.InvokeMember("MerConfigName", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { this.MerConfigName_CredicOnce });
            else if (arg_oTradeType.Equals(HitrustTradeType.CredicInstallments))
                oType.InvokeMember("MerConfigName", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { this.MerConfigName_Installments });
            //Server config path
            oType.InvokeMember("SerConfigName", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { this.SerConfigName });
            //Order Number
            oType.InvokeMember("ordernumber", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { arg_strOrderNumber });

            //Call B2CQuery
            oType.InvokeMember("B2CQuery", System.Reflection.BindingFlags.InvokeMethod, null, tmp, null);

            //After transaction with B2CAuthSSL, you can get the result with below codes.
            //You can refer to out toolkit document with chapter Query.
            oResult = new HitrustResult();
            //訂單編號
            oResult.ordernumber = arg_strOrderNumber;
            //交易結果代碼
            oResult.retcode = (string)oType.InvokeMember("retcode", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            //幣別
            oResult.currency = (string)oType.InvokeMember("currency", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            //訂單日期(YYYY-MM-DD.HH.MM.SS.XXXXXX)
            oResult.orderdate = (string)oType.InvokeMember("orderdate", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            //訂單狀態碼
            oResult.orderstatus = (string)oType.InvokeMember("orderstatus", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            //核准金額
            oResult.approveamount = (string)oType.InvokeMember("approvemount", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            //銀行授權碼
            oResult.authCode = (string)oType.InvokeMember("authCode", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            //銀行調單編號
            oResult.authRRN = (string)oType.InvokeMember("authRRN", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            //請款金額
            oResult.depositamount = (string)oType.InvokeMember("depositamount", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            //請款批次號碼
            oResult.paybatchnumber = (string)oType.InvokeMember("paybatchnumber", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            //請款日期(YYYY-MM-DD.HH.MM.SS.XXXXXX)
            oResult.capDate = (string)oType.InvokeMember("capDate", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            //退款金額
            oResult.credamount = (string)oType.InvokeMember("credamount", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            //退款批次號碼
            oResult.credbatchnumber = (string)oType.InvokeMember("credbatchnumber", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            //退款調單編號
            oResult.credRRN = (string)oType.InvokeMember("credRRN", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            //退款授權碼
            oResult.credCode = (string)oType.InvokeMember("credCode", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            //退款日期(YYYY-MM-DD.HH.MM.SS.XXXXXX)
            oResult.creddate = (string)oType.InvokeMember("creddate", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            //授權方式(SSL, MIA, SET)
            oResult.eci = (string)oType.InvokeMember("eci", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            //分期期數
            oResult.E06 = (string)oType.InvokeMember("E06", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            //首期金額
            oResult.E07 = (string)oType.InvokeMember("E07", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            //每期金額
            oResult.E08 = (string)oType.InvokeMember("E08", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            //手續費
            oResult.E09 = (string)oType.InvokeMember("E09", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            //點點變現金銷帳編號
            oResult.redemordernum = (string)oType.InvokeMember("redemordernum", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            //本次折抵點數
            oResult.redem_discount_point = (string)oType.InvokeMember("redem_discount_point", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            //本次折抵金額
            oResult.redem_discount_amount = (string)oType.InvokeMember("redem_discount_amount", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            //本次實付金額
            oResult.redem_purchase_amount = (string)oType.InvokeMember("redem_purchase_amount", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            //剩餘點數
            oResult.redem_balance_point = (string)oType.InvokeMember("redem_balance_point", System.Reflection.BindingFlags.GetProperty, null, tmp, null);

            //其他待確認的參數(文件上未提及)
            oResult.acquirer = (string)oType.InvokeMember("acquirer", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            oResult.cardtype = (string)oType.InvokeMember("cardtype", System.Reflection.BindingFlags.GetProperty, null, tmp, null);

            oType = null;
            return oResult;
        }//end queryTrade


    }//end class

}//end namespace