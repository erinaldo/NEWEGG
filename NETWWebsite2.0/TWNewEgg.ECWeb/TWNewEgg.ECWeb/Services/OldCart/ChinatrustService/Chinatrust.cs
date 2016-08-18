using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using CTCB.Crypto;
using CTCB.PayPortal;
using CTCB.POS;
using TWNewEgg.Website.ECWeb.Models;

namespace TWNewEgg.Website.ECWeb.Service
{
    /// <summary>
    /// 中國信託內定的交易型態
    /// </summary>
    public enum Chinatrust_txType { Credic_Normal = 0, Credic_Installments = 1, Credic_BonusNormal = 2, Credic_BonusInstallments = 4, WebATM = 9 };
    public class Chinatrust
    {
        #region 宣告全域常數
        /* ------- Chinatrust Common Arguments --------- */
        //新蛋要顯示的商店名稱
        private string CONST_Chinatrust_StoreName = ConfigurationManager.AppSettings.Get("CONST_Chinatrust_StoreName");

        /* ------- Chinatrust WebATM Arguments ------- */
        //merID
        private string CONST_Chinatrust_WebAtm_merID = ConfigurationManager.AppSettings.Get("CONST_Chinatrust_WebAtm_merID");
        private string CONST_Chinatrust_Credic_Normal_merID = ConfigurationManager.AppSettings.Get("CONST_Chinatrust_Credic_Normal_merID");
        private string CONST_Chinatrust_Credic_Intallments_merID = ConfigurationManager.AppSettings.Get("CONST_Chinatrust_Credic_Intallments_merID");
        //MerchantID 銀行所授與的特店代號，純數字，固定13 碼
        private string CONST_Chinatrust_WebAtm_MerchantID = ConfigurationManager.AppSettings.Get("CONST_Chinatrust_WebAtm_MerchantID");
        private string CONST_Chinatrust_Credic_Normal_MerchantID = ConfigurationManager.AppSettings.Get("CONST_Chinatrust_Credic_Normal_MerchantID");
        private string CONST_Chinatrust_Credic_Intallments_MerchantID = ConfigurationManager.AppSettings.Get("CONST_Chinatrust_Credic_Intallments_MerchantID");
        //TerminalID 銀行所授與的終端機代號，純數字，固定 8 碼。
        private string CONST_Chinatrust_WebAtm_TerminalID = ConfigurationManager.AppSettings.Get("CONST_Chinatrust_WebAtm_TerminalID");
        private string CONST_Chinatrust_Credic_Normal_TerminalID = ConfigurationManager.AppSettings.Get("CONST_Chinatrust_Credic_Normal_TerminalID");
        private string CONST_Chinatrust_Credic_Intallments_TerminalID = ConfigurationManager.AppSettings.Get("CONST_Chinatrust_Credic_Intallments_TerminalID");
        //新蛋在URL 帳務管理後台登錄的24bytes 壓碼字串。
        private string CONST_Chinatrust_WebAtm_Key = ConfigurationManager.AppSettings.Get("CONST_Chinatrust_WebAtm_Key");
        private string CONST_Chinatrust_Credic_Normal_Key = ConfigurationManager.AppSettings.Get("CONST_Chinatrust_Credic_Normal_Key");
        private string CONST_Chinatrust_Credic_Intallments_Key = ConfigurationManager.AppSettings.Get("CONST_Chinatrust_Credic_Intallments_Key");

        //新蛋的銀行帳號, 最多16位數
        private string CONST_Chinatrust_WebAtm_VirtualAcct = ConfigurationManager.AppSettings.Get("CONST_Chinatrust_WebAtm_VirtualAcct");
        //private const string CONST_Chinatrust_WebAtm_Acct = "";

        private string m_strWebAtmUrl = ConfigurationManager.AppSettings.Get("CONST_WebAtmUrl");
        private string m_strCredicUrl = ConfigurationManager.AppSettings.Get("CONST_CredicUrl");
        //private CTCB.Crypto.Encrypt m_oChinastrustEncrypt = null;

        public string WebAtm_merID { get { return CONST_Chinatrust_WebAtm_merID; } }
        public string CredicNormal_merID { get { return CONST_Chinatrust_Credic_Normal_merID; } }
        public string CredicIntallments_merID { get { return CONST_Chinatrust_Credic_Intallments_merID; } }

        public string WebAtmUrl { get { return this.m_strWebAtmUrl; } }
        public string CredicUrl { get { return this.m_strCredicUrl; } }
        #endregion

        /// <summary>
        /// execute Chinatrust WebATM
        /// </summary>
        /// <param name="arg_strLidm">訂單編號</param>
        /// <param name="arg_nPurchAmt">訂單總金額</param>
        /// <param name="arg_strAuthResURL">交易完成後要導向的網址</param>
        /// <param name="arg_strBillShortDesc">訂單摘要、商品名稱,最長50位元文數字</param>
        /// <returns>string type of URLResEnc 加密後的文件</returns>
        public string getURLResEncOfWebAtm(string arg_strOrderNumber, string arg_strAuthAmt, string arg_strAuthResURL, string arg_strBillShortDesc, DateTime arg_oOrderTime)
        {
            if (arg_strOrderNumber.Length > 16)
                return "Order Number > 16";
            try
            {
                Convert.ToInt32(arg_strAuthAmt);
            }
            catch
            {
                return "Order Amount is invalid.";
            }

            //進行編碼
            string strURLResEnc = "";
            CTCB.Crypto.Encrypt oEncrypt = new Encrypt();

            oEncrypt.MerchantID = CONST_Chinatrust_WebAtm_MerchantID;
            oEncrypt.TerminalID = CONST_Chinatrust_WebAtm_TerminalID;
            oEncrypt.OrderNo = arg_strOrderNumber;
            oEncrypt.AuthAmt = arg_strAuthAmt;
            oEncrypt.AuthResURL = arg_strAuthResURL;
            oEncrypt.TxType = Convert.ToString(Convert.ToInt32(Chinatrust_txType.WebATM));
            oEncrypt.WebATMAcct = getWebAtmAccount(14, arg_strOrderNumber, arg_oOrderTime, arg_strAuthAmt);
            oEncrypt.Key = CONST_Chinatrust_WebAtm_Key;
            oEncrypt.StoreName = CONST_Chinatrust_StoreName;
            oEncrypt.BillShortDesc = arg_strBillShortDesc;
            oEncrypt.Option = ""; //WebATM請帶空字串

            if (oEncrypt.LastError == 0)
                strURLResEnc = oEncrypt.EncodeData;
            else
                strURLResEnc = oEncrypt.LastError.ToString();


            oEncrypt.ClearData();
            oEncrypt = null;
            return strURLResEnc;
        }//end getWebAtmURLResEnc

        /// <summary>
        /// 取得中國信託信用卡交易的密文
        /// </summary>
        /// <param name="arg_strOrderNumber">訂單編號</param>
        /// <param name="arg_strAuthAmt">訂單總金額</param>
        /// <param name="arg_strAuthResURL">交易完成後要導向的網址</param>
        /// <param name="arg_strBillShortDesc">訂單摘要、商品名稱,最長50位元文數字</param>
        /// <param name="arg_strAutoCap">是否自動請款（0:預設值, 不轉入自動請款; 1:自動轉入請款檔）</param>
        /// <param name="arg_strProdCode">紅利折抵的產品代碼, 可省略</param>
        /// <param name="arg_strNumberOfPay">分期數：分期交易與紅利折抵分期交易必須輸入此參數，其餘交易則可省略，不得小於２</param>
        /// <returns></returns>
        public string getURLResEncOfCredicCard(string arg_strOrderNumber, string arg_strAuthAmt, string arg_strAuthResURL, string arg_strBillShortDesc, string arg_strAutoCap, string arg_strProdCode, string arg_strNumberOfPay, Chinatrust_txType arg_txType)
        {
            string strURLResEnc = "";
            CTCB.Crypto.Encrypt oEncrypt = null;
            int nTemp = 0;

            //判斷所有值是否正確
            try
            {
                Convert.ToInt32(arg_strAuthAmt);
            }
            catch
            {
                return "Some arguments are invalid.";
            }

            #region notice of 信用卡交易邏輯
            /* ------ 信用卡交易的邏輯 ------
             * 分期交易（分期交易/紅利折抵分期交易）的付款期數不得小於2，其餘交易NumberOfPay=1
             *          if( (txType == 1 || txType == 4) && NumberOfPay < 2)
             *          {
             *              return "Error";
             *          }
             *  紅利折抵需輸入ProdCode，其於交易則可省略；測試環境ProdCode請放"00"，其他依銀行約定
             *          if( (txType == 2 || txType == 4) && ProdCode.Length <= 0)
             *          {
             *              return "Error";
             *          }
             *          else
             *          {
             *              ProdCode = "00";
             *          }
             */
            #endregion
            //檢查參數是否符合邏輯 - 分期交易
            if (arg_txType.Equals(Chinatrust_txType.Credic_Installments) || arg_txType.Equals(Chinatrust_txType.Credic_BonusInstallments))
            {
                if (arg_strNumberOfPay == null || arg_strNumberOfPay.Trim().Length <= 0)
                    return "Error: Number of Pay format is uncorrect";
                if (!int.TryParse(arg_strNumberOfPay, out nTemp))
                    return "Error: Number of Pay format is uncorrect";
                if (nTemp < 2)
                    return "Error: Number of Pay must greater than 2 for Installments";
            }
            else
                arg_strNumberOfPay = "";
            //檢查參數是否符合邏輯 - 紅利
            if (arg_txType.Equals(Chinatrust_txType.Credic_BonusInstallments) || arg_txType.Equals(Chinatrust_txType.Credic_BonusNormal))
            {
                if (arg_strProdCode == null || arg_strProdCode.Trim().Length <= 0)
                    return "Error: ProdCode";
            }
            else
                arg_strProdCode = "00"; //00為測試時使用


            oEncrypt = new Encrypt();

            if (arg_txType.Equals(Chinatrust_txType.Credic_Normal) || arg_txType.Equals(Chinatrust_txType.Credic_BonusNormal))
            {
                //一般信用卡
                oEncrypt.MerchantID = CONST_Chinatrust_Credic_Normal_MerchantID;
                oEncrypt.TerminalID = CONST_Chinatrust_Credic_Normal_TerminalID;
                oEncrypt.Key = CONST_Chinatrust_Credic_Normal_Key;

                oEncrypt.OrderNo = arg_strOrderNumber;
                oEncrypt.AuthAmt = arg_strAuthAmt;
                oEncrypt.AuthResURL = arg_strAuthResURL;
                oEncrypt.TxType = Convert.ToString(Convert.ToInt32(arg_txType));
                oEncrypt.StoreName = CONST_Chinatrust_StoreName;
                oEncrypt.MerchantName = CONST_Chinatrust_StoreName;
                oEncrypt.OrderDetail = arg_strBillShortDesc;
                oEncrypt.AutoCap = arg_strAutoCap;
                oEncrypt.ProdCode = arg_strProdCode;
                oEncrypt.NumberOfPay = arg_strNumberOfPay;
            }
            else if (arg_txType.Equals(Chinatrust_txType.Credic_Installments) || arg_txType.Equals(Chinatrust_txType.Credic_BonusInstallments))
            {
                //分期信用卡
                oEncrypt.MerchantID = CONST_Chinatrust_Credic_Intallments_MerchantID;
                oEncrypt.TerminalID = CONST_Chinatrust_Credic_Intallments_TerminalID;
                oEncrypt.Key = CONST_Chinatrust_Credic_Intallments_Key;

                oEncrypt.OrderNo = arg_strOrderNumber;
                oEncrypt.AuthAmt = arg_strAuthAmt;
                oEncrypt.AuthResURL = arg_strAuthResURL;
                oEncrypt.TxType = Convert.ToString(Convert.ToInt32(arg_txType));
                oEncrypt.StoreName = CONST_Chinatrust_StoreName;
                oEncrypt.MerchantName = CONST_Chinatrust_StoreName;
                oEncrypt.OrderDetail = arg_strBillShortDesc;
                oEncrypt.AutoCap = arg_strAutoCap;
                oEncrypt.ProdCode = arg_strProdCode;
                oEncrypt.NumberOfPay = arg_strNumberOfPay;
            }

            if (arg_txType.Equals(Chinatrust_txType.Credic_Normal))
                oEncrypt.Option = "1";
            else if (arg_txType.Equals(Chinatrust_txType.Credic_Installments))
                oEncrypt.Option = arg_strNumberOfPay;
            else if (arg_txType.Equals(Chinatrust_txType.Credic_BonusNormal))
                oEncrypt.Option = arg_strProdCode;
            else if (arg_txType.Equals(Chinatrust_txType.Credic_BonusInstallments))
                oEncrypt.Option = arg_strProdCode + arg_strNumberOfPay;

            if (oEncrypt.LastError == 0)
                strURLResEnc = oEncrypt.EncodeData;
            else
                strURLResEnc = oEncrypt.LastError.ToString();

            oEncrypt.ClearData();
            oEncrypt = null;

            return strURLResEnc;
        }//end getURLResEncOfCredicCard

        /// <summary>
        /// 解密中國信託交易後的加密文
        /// </summary>
        /// <param name="arg_strURLResEnc">中國信託交易完成後的加密文</param>
        /// <returns>中國信託的解密文</returns>
        public MessageOfTradeWithBank getResEnc(string arg_strURLResEnc, Chinatrust_txType arg_txType)
        {
            MessageOfTradeWithBank oMessage = new MessageOfTradeWithBank();
            if (arg_strURLResEnc == null || arg_strURLResEnc.Length <= 0)
            {
                oMessage.ErrDesc = "URLResEnc Length <= 0";
                return oMessage;
            }

            CTCB.Crypto.Decrypt oDecrypt = new Decrypt();
            oDecrypt.EncRes = arg_strURLResEnc;
            switch (arg_txType)
            {
                case Chinatrust_txType.WebATM:
                    oDecrypt.Key = CONST_Chinatrust_WebAtm_Key;
                    break;
                case Chinatrust_txType.Credic_Normal:
                    oDecrypt.Key = CONST_Chinatrust_Credic_Normal_Key;
                    break;
                case Chinatrust_txType.Credic_Installments:
                    oDecrypt.Key = CONST_Chinatrust_Credic_Intallments_Key;
                    break;
                case Chinatrust_txType.Credic_BonusNormal:
                    oDecrypt.Key = CONST_Chinatrust_Credic_Normal_Key;
                    break;
                case Chinatrust_txType.Credic_BonusInstallments:
                    oDecrypt.Key = CONST_Chinatrust_Credic_Intallments_Key;
                    break;
            }

            oMessage.LastError = oDecrypt.LastError.ToString();
            if (oDecrypt.LastError == 0)
            {
                //共同訊息
                oMessage.txType = arg_txType;
                oMessage.AuthCode = oDecrypt.AuthCode;
                oMessage.ErrCode = oDecrypt.ErrCode;
                oMessage.ErrDesc = oDecrypt.ErrDesc;
                oMessage.OrderNumber = oDecrypt.OrderNo;
                oMessage.MerID = oDecrypt.MerID;
                oMessage.PayerLastPin4Code = oDecrypt.Last4digitPAN;
                oMessage.ResURL = oDecrypt.AuthResURL;
                oMessage.Status = oDecrypt.Status;
                oMessage.PayerLastPin4Code = oDecrypt.Last4digitPAN;

                //WebAtm訊息
                oMessage.AuthAmt = oDecrypt.AuthAmt;
                oMessage.Fee = oDecrypt.FeeCharge;
                oMessage.PayerBandId = oDecrypt.PayerBankId;
                oMessage.WebAtmAcc = oDecrypt.WebATMAcct;

                //信用卡訊息
                oMessage.AwardedPoint = oDecrypt.AwardedPoint;
                oMessage.NumberOfPay = oDecrypt.NumberOfPay;
                oMessage.OffsetAmt = oDecrypt.OffsetAmt;
                oMessage.OriginalAmt = oDecrypt.OriginalAmt;
                oMessage.PointBalance = oDecrypt.PointBalance;
                oMessage.ProdCode = oDecrypt.ProdCode;
                oMessage.XId = oDecrypt.XID;

            }
            else
            {
                oMessage.ErrCode = oDecrypt.LastError.ToString();
                oMessage.ErrDesc = "交易失敗，請詳查Error Code";
            }
            return oMessage;
        }//end getResEncOfWebAtm

        public Chinatrust()
        {
        }

        /// <summary>
        /// 取得中國信託的虛擬帳號
        /// </summary>
        /// <param name="arg_nCharset">總帳號長度</param>
        /// <param name="arg_strOrderNumber">訂單編號</param>
        /// <param name="arg_oOrderTime">訂單成立時間</param>
        /// <returns>虛擬帳號</returns>
        private string getWebAtmAccount(int arg_nCharset, string arg_strOrderNumber, DateTime arg_oOrderTime, string arg_strAuthAmt)
        {
            /*
             * 虛擬帳號，共５碼
             * 訂單日期取月日部份，共４碼
             * arg_nCharset = 虛擬帳號碼數+訂單日期碼數+訂單取得碼數+驗證碼1碼
             * 故訂單需取得碼數 = arg_nCharset - 虛擬帳號5碼 - 訂單日期4碼 - 驗證碼1碼
             *                            = arg_nCharset - 10
             */

            string subOrderNumber = ""; //取得用於計算的OrderNumber部份
            string subOrderTime = "";   //取得用於計算OrderTime的部份
            string subPrice = "";   //取得用於計算Price的部份
            string strWeighted = "371"; //加權數
            string strCheckNumber = "";
            int i = 0;
            int sum = 0;
            int nA = 0;
            int nB = 0;

            subOrderNumber = arg_strOrderNumber.Substring(arg_strOrderNumber.Length - (arg_nCharset - 10)).PadLeft(arg_nCharset - 10, '0');
            subOrderTime = arg_oOrderTime.Month.ToString().PadLeft(2, '0') + arg_oOrderTime.Day.ToString().PadLeft(2, '0');
            subPrice = arg_strAuthAmt;
            if (subPrice.Length > 8)
                subPrice = subPrice.Substring(subPrice.Length - 8);
            else
                subPrice = subPrice.PadLeft(8, '0');

            /* ------ Step1: 取出虛擬帳號 + OrderNumber，以加權數算出Ａ ------ */
            //根據新蛋規則, OrderNumber = OrderTime + OrderNumber, 取與權數相乘之後的個位數, 再相加取個位數
            sum = 0;
            subOrderNumber = CONST_Chinatrust_WebAtm_VirtualAcct + subOrderTime + subOrderNumber;
            //與權數列相乘之後，取個位數相加
            for (i = 0; i < subOrderNumber.Length; i++)
                sum += Convert.ToInt16(subOrderNumber[i].ToString()) * Convert.ToInt16(strWeighted[i % strWeighted.Length].ToString()) % 10;
            nA = sum % 10; //最後再取個位數, 為A

            /* ------ Step2: 再以金額以371數列算出B ------ */
            sum = 0;
            for (i = 0; i < subPrice.Length; i++)
                sum += Convert.ToInt16(subPrice[i].ToString()) * Convert.ToInt16(strWeighted[i % strWeighted.Length].ToString()) % 10;
            nB = sum % 10;

            /* ------ Step3: C= A+B 取個位數，檢核碼 = 10-C ------ */
            strCheckNumber = Convert.ToString(10 - (nA + nB) % 10);

            /* ------ Step4: 組成帳號 ------ */
            subOrderNumber = subOrderNumber + strCheckNumber;


            return subOrderNumber;
        }//end getWebAtmAccount

    }
}