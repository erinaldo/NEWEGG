using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Configuration;
using TWNewEgg.Website.ECWeb;

namespace TWNewEgg.Website.ECWeb.Service
{
    public enum AllPayChoosePayment
    {
        Credit = 1,
        WebATM = 2,
        ATM = 3,
        CVS = 4,
        BARCODE = 5,
        Alipay = 6,
        Tenpay = 7,
        TopUpUsed = 8
    }

    public enum AllPaySubPayment
    {
        WebATM_Unknown = 0, //任意選擇
        WebATM_TAISHIN = 1, //WebATM_台新
        WebATM_ESUN = 2,    //WebATM_玉山
        WebATM_HUANAN = 3,    //WebATM_華南
        WebATM_BOT = 4,    //WebATM_台灣銀行
        WebATM_FUBON = 5,    //WebATM_台北富邦
        CHINATRUST = 6,    //WebATM_中國信託
        WebATM_FIRST = 7,    //WebATM_第一銀行
        WebATM_CHINATRUST = 8,  //WebATM_中國信託
        WebATM_CATHAY = 9, //WebATM_國泰
        WebATM_YUANTA = 10, //WebATM_元大
        ATM_TAISHIN = 11,    //ATM_台新
        ATM_ESUN = 12,    //ATM_玉山
        ATM_HUANAN = 13,    //ATM_華南
        ATM_BOT = 14,    //ATM_台灣銀行
        ATM_FUBON = 15,    //ATM_台北富邦
        ATM_CHINATRUST = 16,    //ATM_中國信託
        ATM_FIRST = 17,    //ATM_第一銀行
        WebATM_LAND = 18, //WebATM_土地銀行
        WebATM_MEGA = 19, //WebATM_兆豐
        CVS_CVS = 31,   //超商代碼繳款
        CVS_OK = 32,    //OK超商代碼繳款
        CVS_FAMILY = 33,    //全家超商代碼繳款
        CVS_HILIFE = 34,    //萊爾富超商代碼繳款
        CVS_IBON = 35,  //7-11 ibon代碼繳款
        BARCODE_BARCODE = 36,   //超商條碼繳款
        Alipay_Alipay = 41, //支付寶
        Tenpay_Tenpay = 42, //財付通
        CreditCard_GW = 43, //信用卡_MasterCard_JCB_VISA
        AE_AE = 44, //美國運通
        UnionPay_UnionPay = 45, //銀聯卡
        UCard_UCard = 46,   //聯合信用卡
        TopUpUsed_AllPay = 61,  //儲值/餘額消費_歐付寶
        topUpUsed_ESUN = 62 //儲值/餘額消費_玉山

    }

    public class AllPayAio
    {
        private string m_strNewStartDate = ConfigurationManager.AppSettings.Get("CONST_NewAllPay_StartDate");
        public string MerchantId
        {
            get
            {
                if (DateTime.Now.CompareTo(Convert.ToDateTime(m_strNewStartDate)) >= 0)
                    return ConfigurationManager.AppSettings.Get("CONST_NewAllPay_MerchantId");
                else
                    return ConfigurationManager.AppSettings.Get("CONST_AllPay_MerchantId");
            }
            //get { return ConfigurationManager.AppSettings.Get("CONST_AllPay_MerchantId"); }
        }

        public string HashKey
        {
            get
            {
                if (DateTime.Now.CompareTo(Convert.ToDateTime(m_strNewStartDate)) >= 0)
                    return ConfigurationManager.AppSettings.Get("CONST_NewAllPay_All_In_One_HashKey");
                else
                    return ConfigurationManager.AppSettings.Get("CONST_AllPay_All_In_One_HashKey");
            }
            //get { return ConfigurationManager.AppSettings.Get("CONST_AllPay_All_In_One_HashKey"); }
        }

        public string HashIV
        {
            get
            {
                if (DateTime.Now.CompareTo(Convert.ToDateTime(m_strNewStartDate)) >= 0)
                    return ConfigurationManager.AppSettings.Get("CONST_NewAllPay_All_In_One_HashIV");
                else
                    return ConfigurationManager.AppSettings.Get("CONST_AllPay_All_In_One_HashIV");
            }
            //get { return ConfigurationManager.AppSettings.Get("CONST_AllPay_All_In_One_HashIV"); }
        }

        public string NormalHashKey
        {
            get
            {
                if (DateTime.Now.CompareTo(Convert.ToDateTime(m_strNewStartDate)) >= 0)
                    return ConfigurationManager.AppSettings.Get("CONST_NewAllPay_Normal_HashKey");
                else
                    return ConfigurationManager.AppSettings.Get("CONST_AllPay_Normal_HashKey");
            }
            //get { return ConfigurationManager.AppSettings.Get("CONST_AllPay_Normal_HashKey"); }
        }
        public string NormalHashIV
        {
            get
            {
                if (DateTime.Now.CompareTo(Convert.ToDateTime(m_strNewStartDate)) >= 0)
                    return ConfigurationManager.AppSettings.Get("CONST_NewAllPay_Normal_HashIV");
                else
                    return ConfigurationManager.AppSettings.Get("CONST_AllPay_Normal_HashIV");
            }
            //get { return ConfigurationManager.AppSettings.Get("CONST_AllPay_Normal_HashIV"); }
        }

        public string MerchantTradeNo { get; set; }
        public string MerchantTradeDate { get; set; }
        public string PaymentType { get { return "aio"; } }
        public int TotalAmount { get; set; }
        public string TradeDesc { get; set; }
        public string ItemName { get; set; }
        public string ReturnURL { get; set; }
        public AllPayChoosePayment ChoosePayment { get; set; }
        public string CheckMacValue { get; set; }
        public string ClientBackURL { get; set; }
        public string ItemURL { get; set; }
        public string Remark { get; set; }
        public AllPaySubPayment ChooseSubPayment { get; set; }
        public string OrderResultURL { get; set; }
        public int ExpireDate { get; set; }
        public string Desc_1 { get; set; }
        public string Desc_2 { get; set; }
        public string Desc_3 { get; set; }
        public string Desc_4 { get; set; }
        public string AlipayItemName { get; set; }
        public string AlipayItemCounts { get; set; }
        public string AlipayItemPrice { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string UserName { get; set; }
        public string ExpireTime { get; set; }
        public int CreditInstallment { get; set; }
        public int InstallmentAmount { get; set; }
        public char Redeem { get; set; }




        public string getCheckMacValue()
        {
            string strHashKey = "HashKey=" + this.HashKey;
            string strHashIv = "HashIV=" + this.HashIV;
            string strPostUrl = "";


            //AlipayItemCount, AlipayItemName, AlipayItemPrice
            if (this.ChoosePayment.Equals(AllPayChoosePayment.Alipay))
            {
                strPostUrl += "&AlipayItemCounts=" + this.AlipayItemCounts;
                strPostUrl += "&AlipayItemName=" + this.AlipayItemName;
                strPostUrl += "&AlipayItemPrice=" + this.AlipayItemPrice;
            }

            //ChoosePayMent
            strPostUrl += "&ChoosePayment=" + this.ChoosePayment.ToString();
            //ChooseSubPayment
            if (!this.ChooseSubPayment.Equals(AllPaySubPayment.WebATM_Unknown))
            {
                strPostUrl += "&ChooseSubPayment=" + this.ChooseSubPayment.ToString().Split('_')[1];
                //strPostUrl += "&ChooseSubPayment=TAISHIN";
            }
            else
                strPostUrl += "&ChooseSubPayment=";
            //ClientbackURL
            strPostUrl += "&ClientBackURL=" + this.ClientBackURL;
            //CreditInstallment
            if (this.ChoosePayment.Equals(AllPayChoosePayment.Credit))
            {
                strPostUrl += "&CreditInstallment=" + this.CreditInstallment.ToString();
            }
            //Desc_1 ~ Desc_4
            if (this.ChoosePayment.Equals(AllPayChoosePayment.CVS) || this.ChoosePayment.Equals(AllPayChoosePayment.BARCODE))
            {
                strPostUrl += "&Desc_1=" + this.Desc_1;
                strPostUrl += "&Desc_2=" + this.Desc_2;
                strPostUrl += "&Desc_3=" + this.Desc_3;
                strPostUrl += "&Desc_4=" + this.Desc_4;
            }
            //Email
            if (this.ChoosePayment.Equals(AllPayChoosePayment.Alipay))
            {
                strPostUrl += "&Email=" + this.Email;
            }
            //ExpireDate
            if (this.ChoosePayment.Equals(AllPayChoosePayment.ATM))
            {
                strPostUrl += "&ExpireDate=" + this.ExpireDate;
            }
            //ExpireTime
            if (this.ChoosePayment.Equals(AllPayChoosePayment.Tenpay))
            {
                strPostUrl += "&ExpireTime=" + this.ExpireTime;
            }
            //InstallmentAmount
            if (this.ChoosePayment.Equals(AllPayChoosePayment.Credit))
            {
                strPostUrl += "&InstallmentAmount=" + this.InstallmentAmount.ToString();
            }
            //ItemName
            strPostUrl += "&ItemName=" + this.ItemName;
            //ItemURL
            strPostUrl += "&ItemURL=" + this.ItemURL;
            //MerchantID
            strPostUrl += "&MerchantID=" + this.MerchantId;
            //MerchantTradeDate
            strPostUrl += "&MerchantTradeDate=" + this.MerchantTradeDate.ToString();
            //MerchantTradeNo
            strPostUrl += "&MerchantTradeNo=" + this.MerchantTradeNo;
            //OrderResultURL
            strPostUrl += "&OrderResultURL=" + this.OrderResultURL;
            //PaymentType
            strPostUrl += "&PaymentType=" + this.PaymentType;
            //PhoneNo
            if (this.ChoosePayment.Equals(AllPayChoosePayment.Alipay))
            {
                strPostUrl += "&PhoneNo=" + this.PhoneNo;
            }
            //Redeem
            if (this.ChoosePayment.Equals(AllPayChoosePayment.Credit) && this.CreditInstallment > 0)
            {
                strPostUrl += "&Redeem=" + this.Redeem.ToString();
            }
            //Remark
            strPostUrl += "&Remark=" + this.Remark;
            //ReturnURL
            strPostUrl += "&ReturnURL=" + this.ReturnURL;
            //TotalAmount
            strPostUrl += "&TotalAmount=" + this.TotalAmount;
            //TradeDesc
            strPostUrl += "&TradeDesc=" + this.TradeDesc;
            //UserName
            if (this.ChoosePayment.Equals(AllPayChoosePayment.Alipay))
            {
                strPostUrl += "&UserName=" + this.UserName;
            }

            //進行加密
            strPostUrl = strHashKey + strPostUrl + "&" + strHashIv;
            strPostUrl = HttpUtility.UrlEncode(strPostUrl);
            strPostUrl = strPostUrl.ToLower();
            strPostUrl = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(strPostUrl, "MD5");

            return strPostUrl;
        }//end getCheckMacValue

        public string getTradeCheckMacValue(string MerchantID, string MerchantTradeNo, string RtnCode, string RtnMsg, string TradeNo, string TradeAmt, string PaymentDate, string PaymentType, string PaymentTypeChargeFee, string TradeDate, string SimulatePaid)
        {
            string strCheckCode = "";

            strCheckCode += "&MerchantID=" + MerchantID;
            strCheckCode += "&MerchantTradeNo=" + MerchantTradeNo;
            strCheckCode += "&PaymentDate=" + PaymentDate;
            strCheckCode += "&PaymentType=" + PaymentType;
            strCheckCode += "&PaymentTypeChargeFee=" + PaymentTypeChargeFee;
            strCheckCode += "&RtnCode=" + RtnCode;
            strCheckCode += "&RtnMsg=" + RtnMsg;
            strCheckCode += "&SimulatePaid=" + SimulatePaid;
            strCheckCode += "&TradeAmt=" + TradeAmt;
            strCheckCode += "&TradeDate=" + TradeDate;
            strCheckCode += "&TradeNo=" + TradeNo;

            strCheckCode = "HashKey=" + this.HashKey + strCheckCode + "&HashIV=" + this.HashIV;

            strCheckCode = HttpUtility.UrlEncode(strCheckCode);
            strCheckCode = strCheckCode.ToLower();
            strCheckCode = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(strCheckCode, "MD5");

            return strCheckCode;
        }//end getTradeCheckMacValue()
    }

}//end namespace