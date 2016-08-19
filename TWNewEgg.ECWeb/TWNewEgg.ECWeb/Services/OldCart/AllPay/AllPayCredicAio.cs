using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Xml;
using TWNewEgg.Website.ECWeb;
using Allpay.AES;

namespace TWNewEgg.Website.ECWeb.Service
{
    public class AllPayCredicAio
    {
        private string m_strNewStartDate = ConfigurationManager.AppSettings.Get("CONST_NewAllPay_StartDate");
        public string MerchantID
        {
            get
            {
                if (DateTime.Now.CompareTo(Convert.ToDateTime(m_strNewStartDate)) >= 0)
                    return ConfigurationManager.AppSettings.Get("CONST_NewAllPay_MerchantId");
                else
                    return ConfigurationManager.AppSettings.Get("CONST_AllPay_MerchantId");
            }
        }
        public string HashKey
        {
            get
            {
                if (DateTime.Now.CompareTo(Convert.ToDateTime(m_strNewStartDate)) >= 0)
                    return ConfigurationManager.AppSettings.Get("CONST_NewAllPay_Normal_HashKey");
                else
                    return ConfigurationManager.AppSettings.Get("CONST_AllPay_Normal_HashKey");
            }
        }
        public string HashIV
        {
            get
            {
                if (DateTime.Now.CompareTo(Convert.ToDateTime(m_strNewStartDate)) >= 0)
                    return ConfigurationManager.AppSettings.Get("CONST_NewAllPay_Normal_HashIV");
                else
                    return ConfigurationManager.AppSettings.Get("CONST_AllPay_Normal_HashIV");
            }
        }

        /// <summary>
        /// 廠商交易編號(不可重覆)
        /// </summary>
        public string MerchantTradeNo { get; set; }
        /// <summary>
        /// 廠商交易時間, 格式為 yyyy/MM/dd HH:mm:ss
        /// </summary>
        public string MerchantTradeDate { get; set; }
        /// <summary>
        /// 交易金額
        /// </summary>
        public int TotalAmount { get; set; }
        /// <summary>
        /// 交易描述, 請先利用CharSet格式進行UrlEncode
        /// </summary>
        public string TradeDesc { get; set; }
        /// <summary>
        /// 信用卡卡號
        /// 若要在AllPay顯示信用卡頁讓使用者輸入的話, 請放0。連同CardValidMM、CardValidYY及CardCVV2也都請放0
        /// </summary>
        public string CardNo { get; set; }
        /// <summary>
        /// 信用卡有效月份
        /// </summary>
        public string CardValidMM { get; set; }
        /// <summary>
        /// 信用卡有效年份
        /// </summary>
        public string CardValidYY { get; set; }
        /// <summary>
        /// 信用卡背後末3碼, 若不驗證末三碼, 請放空值
        /// </summary>
        public string CardCVV2 { get; set; }
        /// <summary>
        /// 是否為銀聯卡, Web Service版本都請帶0
        /// </summary>
        public int UnionPay { get { return 0; } }
        /// <summary>
        /// 分期期數, 若不分期請帶0
        /// </summary>
        public int Installment { get; set; }
        /// <summary>
        /// 是否使用3D驗證, 使用請帶1, 不使用請帶0
        /// </summary>
        public int ThreeD { get; set; }
        /// <summary>
        /// 中文編碼格式: utf-8或big5。本系統採用utf-8編碼格式
        /// </summary>
        public string CharSet { get { return "utf-8"; } }
        /// <summary>
        /// 英文交易時, 請帶e, 否則請放空值
        /// </summary>
        public string Enn { get; set; }
        /// <summary>
        /// 限制交易的銀行卡號, 只有符合您送來的指定銀行或卡片前6碼, 系統才會讓它交易, 不符合的都會被踢掉
        /// 銀行名稱, 或卡片前6碼, 只能擇一使用, 不能混用
        /// 銀行名稱需與各家銀行代碼查詢相同
        /// 多家銀行請以逗點分隔, 例: 123456, 555666
        /// 若無限制交易的銀行卡別, 請放空值
        /// </summary>
        public string BankOnly { get; set; }
        /// <summary>
        /// 請放空值, 目前Web Service是背景處理授權, 尚未開放紅利折抵
        /// 設為Y時, 會進入紅利折抵的交易流程(有申請紅利的商家, 紅利折抵參數才會有作用)
        /// </summary>
        public string Redeem { get; set; }
        /// <summary>
        /// 要做OTP認證的手機號碼
        /// 若要做OTP認證,則不可空白。訂單建立成功後,AllPay會傳送一組OTP認證碼給這個手機號碼
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 購買者是否同意加入AllPay會員, 同意加入請帶1, 不同意請帶0
        /// </summary>
        public string AddMember { get; set; }
        /// <summary>
        /// 消費者姓名, 若同意加入AllPay會員時, 則不可空白
        /// </summary>
        public string CName { get; set; }
        /// <summary>
        /// 消費者電子郵件, 可空白
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 備註, 可空白, 請先利用CharSet格式進行UrlEncode
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// output回傳: AllPay交易編號
        /// </summary>
        public string TradeNo { get; set; }
        /// <summary>
        /// output回傳: OTP驗證結果, 1:驗證成功, 其餘為失敗(不走OTP驗證的授權, 會回傳0)
        /// </summary>
        public int OtpResult { get; set; }
        /// <summary>
        /// output回傳: 交易狀態, 1:授權成功, 0:尚未授權, 其餘代碼皆為失敗, 失敗代碼請參考AllPay交易訊息代碼表
        /// </summary>
        public int RtnCode { get; set; }
        /// <summary>
        /// output回傳: OTP驗證訊息/交易訊息
        /// </summary>
        public string RtnMsg { get; set; }
        /// <summary>
        /// output回傳: 授權交易單號
        /// </summary>
        public int gwsr { get; set; }
        /// <summary>
        /// output回傳: 處理時間
        /// </summary>
        public string process_date { get; set; }
        /// <summary>
        /// output回傳: 授權碼
        /// </summary>
        public string auth_code { get; set; }
        /// <summary>
        /// output回傳: 金額
        /// </summary>
        public int amount { get; set; }
        /// <summary>
        /// output回傳: 分期期數
        /// </summary>
        public int stage { get; set; }
        /// <summary>
        /// output回傳: 頭期金額
        /// </summary>
        public int stast { get; set; }
        /// <summary>
        /// output回傳: 各期金額
        /// </summary>
        public int staed { get; set; }
        /// <summary>
        /// output回傳: 3D(VBV)回傳值(eci=5,6,2,1 代表該筆交易不可否認)
        /// </summary>
        public int eci { get; set; }
        /// <summary>
        /// output回傳: 卡片的末4碼
        /// </summary>
        public string card4no { get; set; }
        /// <summary>
        /// output回傳: 卡片的前6碼
        /// </summary>
        public string card6no { get; set; }
        /// <summary>
        /// output回傳: 紅利扣點
        /// </summary>
        public int red_dan { get; set; }
        /// <summary>
        /// output回傳: 紅利折抵金額
        /// </summary>
        public int red_de_amt { get; set; }
        /// <summary>
        /// output回傳: 實際扣款金額
        /// </summary>
        public int red_ok_amt { get; set; }
        /// <summary>
        /// ouput回傳: 紅利剩餘點數
        /// </summary>
        public int red_yet { get; set; }

        /// <summary>
        /// 取得加密的文件
        /// </summary>
        /// <returns></returns>
        public string getEncryptInput()
        {
            Allpay.AES.AES oAes = null;

            string strXml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>";
            strXml += "<Root>";
            strXml += "<Data>";
            strXml += "<MerchantID>" + this.MerchantID + "</MerchantID>";
            strXml += "<MerchantTradeNo>" + this.MerchantTradeNo + "</MerchantTradeNo>";
            strXml += "<MerchantTradeDate>" + this.MerchantTradeDate + "</MerchantTradeDate>";
            strXml += "<TotalAmount>" + Convert.ToString(this.TotalAmount) + "</TotalAmount>";
            if (this.TradeDesc.Length > 200)
                this.TradeDesc = this.TradeDesc.Substring(0, 200);
            strXml += "<TradeDesc>" + HttpUtility.UrlEncode(this.TradeDesc) + "</TradeDesc>";
            strXml += "<CardNo>" + this.CardNo + "</CardNo>";
            strXml += "<CardValidMM>" + this.CardValidMM + "</CardValidMM>";
            strXml += "<CardValidYY>" + this.CardValidYY + "</CardValidYY>";
            strXml += "<CardCVV2>" + this.CardCVV2 + "</CardCVV2>";
            strXml += "<UnionPay>" + this.UnionPay.ToString() + "</UnionPay>";
            strXml += "<Installment>" + this.Installment.ToString() + "</Installment>";
            strXml += "<ThreeD>" + this.ThreeD + "</ThreeD>";
            strXml += "<CharSet>" + this.CharSet + "</CharSet>";
            strXml += "<Enn>" + this.Enn + "</Enn>";
            strXml += "<BankOnly>" + this.BankOnly + "</BankOnly>";
            strXml += "<Redeem>" + this.Redeem + "</Redeem>";
            strXml += "<PhoneNumber>" + this.PhoneNumber + "</PhoneNumber>";
            strXml += "<AddMember>" + this.AddMember + "</AddMember>";
            strXml += "<CName>" + this.CName + "</CName>";
            strXml += "<Email>" + this.Email + "</Email>";
            if (this.Remark.Length > 200)
                this.Remark = this.Remark.Substring(0, 200);
            strXml += "<Remark>" + HttpUtility.UrlEncode(this.Remark) + "</Remark>";
            strXml += "</Data>";
            strXml += "</Root>";

            //strXml = this.AESEncrypt(strXml, this.HashKey, this.HashIV);

            oAes = new AES();
            strXml = oAes.AES_EnCrypt(this.HashKey, this.HashIV, strXml);
            oAes = null;

            return strXml;
        }//end getInputXml

        /// <summary>
        /// 將文件解密後, 並將值設定到各參數裡
        /// </summary>
        /// <param name="arg_strContent"></param>
        public void Decrypt(string arg_strContent)
        {
            if (arg_strContent == null || arg_strContent.Length <= 0)
                return;

            string strXmlContent = "";
            XmlDocument oXml = null;
            AES oAes = null;

            arg_strContent = arg_strContent.Replace(" ", "+");
            //strXmlContent = this.AESDecrypt(arg_strContent, this.HashKey, this.HashIV);
            oAes = new AES();
            strXmlContent = oAes.AES_DeCrypt(this.HashKey, this.HashIV, arg_strContent);
            oAes = null;

            try
            {
                oXml = new XmlDocument();
                oXml.LoadXml(strXmlContent);
            }
            catch (Exception ex)
            {
                this.MerchantTradeNo = "0000000000";
                this.TradeNo = "0000000000";
                this.RtnCode = 1111111111;
                this.RtnMsg = ex.Message + "\r\n content is " + strXmlContent;
                this.auth_code = "1111111";
                this.amount = 0;
                oXml = null;
                return;
            }

            if(oXml.GetElementsByTagName("MerchantTradeNo").Count > 0)
                this.MerchantTradeNo = oXml.GetElementsByTagName("MerchantTradeNo")[0].InnerText;
            if (oXml.GetElementsByTagName("TradeNo").Count > 0)
                this.TradeNo = oXml.GetElementsByTagName("TradeNo")[0].InnerText;
            if (oXml.GetElementsByTagName("OtpResult").Count > 0)
                this.OtpResult = Convert.ToInt32(oXml.GetElementsByTagName("OtpResult")[0].InnerText);
            if (oXml.GetElementsByTagName("RtnCode").Count > 0)
                this.RtnCode = Convert.ToInt32(oXml.GetElementsByTagName("RtnCode")[0].InnerText);
            if (oXml.GetElementsByTagName("RtnMsg").Count > 0)
                this.RtnMsg = HttpUtility.UrlDecode(oXml.GetElementsByTagName("RtnMsg")[0].InnerText);
            if (oXml.GetElementsByTagName("gwsr").Count > 0)
                this.gwsr = Convert.ToInt32(oXml.GetElementsByTagName("gwsr")[0].InnerText);
            if (oXml.GetElementsByTagName("process_date").Count > 0)
                this.process_date = oXml.GetElementsByTagName("process_date")[0].InnerText;
            if (oXml.GetElementsByTagName("auth_code").Count > 0)
                this.auth_code = oXml.GetElementsByTagName("auth_code")[0].InnerText;
            else
                this.auth_code = "";
            if (oXml.GetElementsByTagName("amount").Count > 0)
            {
                if (oXml.GetElementsByTagName("amount")[0].InnerText.Length > 0)
                    this.amount = Convert.ToInt32(oXml.GetElementsByTagName("amount")[0].InnerText);
                else
                    this.amount = 0;
            }
            else
                this.amount = 0;
            if (oXml.GetElementsByTagName("stage").Count > 0)
                this.stage = Convert.ToInt32(oXml.GetElementsByTagName("stage")[0].InnerText);
            else
                this.stage = 0;
            if (oXml.GetElementsByTagName("stast").Count > 0)
                this.stast = Convert.ToInt32(oXml.GetElementsByTagName("stast")[0].InnerText);
            else
                this.stast = 0;
            if (oXml.GetElementsByTagName("staed").Count > 0)
                this.staed = Convert.ToInt32(oXml.GetElementsByTagName("staed")[0].InnerText);
            else
                this.staed = 0;
            if (oXml.GetElementsByTagName("eci").Count > 0)
                this.eci = Convert.ToInt32(oXml.GetElementsByTagName("eci")[0].InnerText);
            else
                this.eci = 0;
            if (oXml.GetElementsByTagName("card4no").Count > 0)
                this.card4no = oXml.GetElementsByTagName("card4no")[0].InnerText;
            else
                this.card4no = "";
            if (oXml.GetElementsByTagName("card6no").Count > 0)
                this.card6no = oXml.GetElementsByTagName("card6no")[0].InnerText;
            else
                this.card6no = "";
            if (oXml.GetElementsByTagName("red_dan").Count > 0)
                this.red_dan = Convert.ToInt32(oXml.GetElementsByTagName("red_dan")[0].InnerText);
            else
                this.red_dan = 0;
            if (oXml.GetElementsByTagName("red_de_amt").Count > 0)
                this.red_de_amt = Convert.ToInt32(oXml.GetElementsByTagName("red_de_amt")[0].InnerText);
            else
                this.red_de_amt = 0;
            if (oXml.GetElementsByTagName("red_ok_amt").Count > 0)
                this.red_ok_amt = Convert.ToInt32(oXml.GetElementsByTagName("red_ok_amt")[0].InnerText);
            else
                this.red_ok_amt = 0;
            if (oXml.GetElementsByTagName("red_yet").Count > 0)
                this.red_yet = Convert.ToInt32(oXml.GetElementsByTagName("red_yet")[0].InnerText);
            else
                this.red_yet = 0;

            oXml = null;
        }//end Decrypt



        /// <summary>
        /// 有密码的AES加密 
        /// </summary>
        /// <param name="text">加密字符</param>
        /// <param name="password">加密的密码</param>
        /// <param name="iv">密钥</param>
        /// <returns></returns>
        private string AESEncrypt(string plainText, string key, string iv)
        {
            string ciperText = "";
            try
            {
                byte[] bPlainText = Encoding.UTF8.GetBytes(plainText);
                byte[] KEYData = Encoding.UTF8.GetBytes(key);
                byte[] IVData = Encoding.UTF8.GetBytes(iv);
                RijndaelManaged rijndael = new RijndaelManaged();
                rijndael.Padding = PaddingMode.PKCS7;
                rijndael.Mode = CipherMode.CBC;
                rijndael.KeySize = 128;
                rijndael.BlockSize = 128;
                ICryptoTransform transform = rijndael.CreateEncryptor(KEYData, IVData);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write);
                cs.Write(bPlainText, 0, bPlainText.Length);
                cs.FlushFinalBlock();

                ciperText = Convert.ToBase64String(ms.ToArray());
                ms.Close();
                cs.Close();
            }
            catch(Exception ex)
            { 
            }
            return ciperText;

            #region oldEncrypt
            /*

            RijndaelManaged rijndaelCipher = new RijndaelManaged();

            rijndaelCipher.Mode = CipherMode.CBC;

            rijndaelCipher.Padding = PaddingMode.PKCS7;

            rijndaelCipher.KeySize = 128;

            rijndaelCipher.BlockSize = 128;

            byte[] pwdBytes = System.Text.Encoding.ASCII.GetBytes(key);

            byte[] keyBytes = new byte[16];

            int len = pwdBytes.Length;

            if (len > keyBytes.Length) len = keyBytes.Length;

            System.Array.Copy(pwdBytes, keyBytes, len);

            rijndaelCipher.Key = keyBytes;


            byte[] ivBytes = System.Text.Encoding.ASCII.GetBytes(iv);
            rijndaelCipher.IV = ivBytes;

            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();

            byte[] plainText = Encoding.ASCII.GetBytes(text);

            byte[] cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);

            return Convert.ToBase64String(cipherBytes);
            */
            #endregion
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="text"></param>
        /// <param name="password"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        private string AESDecrypt(string cipherText, string key, string iv)
        {
            string plainText = "";
            try
            {
                byte[] KEYData = Encoding.UTF8.GetBytes(key);
                byte[] IVData = Encoding.UTF8.GetBytes(iv);
                RijndaelManaged rijndael = new RijndaelManaged();
                rijndael.Padding = PaddingMode.PKCS7;
                rijndael.Mode = CipherMode.CBC;
                rijndael.KeySize = 128;
                rijndael.BlockSize = 128;
                ICryptoTransform transform = rijndael.CreateDecryptor(KEYData, IVData);
                byte[] bCipherText = Convert.FromBase64String(cipherText);//这里要用这个函数来正确转换Base64字符串成Byte数组
                MemoryStream ms = new MemoryStream(bCipherText);
                CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Read);
                byte[] bPlainText = new byte[bCipherText.Length];
                cs.Read(bPlainText, 0, bPlainText.Length);
                plainText = Encoding.UTF8.GetString(bPlainText);
                //plainText = plainText.Trim('\0');
            }
            catch(Exception ex)
            { }
            return plainText;

            #region olderDecrypt
            /*
            RijndaelManaged rijndaelCipher = new RijndaelManaged();

            rijndaelCipher.Mode = CipherMode.CBC;

            rijndaelCipher.Padding = PaddingMode.PKCS7;

            rijndaelCipher.KeySize = 128;

            rijndaelCipher.BlockSize = 128;

            byte[] encryptedData = Convert.FromBase64String(text);

            byte[] pwdBytes = System.Text.Encoding.ASCII.GetBytes(key);

            byte[] keyBytes = new byte[16];

            int len = pwdBytes.Length;

            if (len > keyBytes.Length) len = keyBytes.Length;

            System.Array.Copy(pwdBytes, keyBytes, len);

            rijndaelCipher.Key = keyBytes;

            byte[] ivBytes = System.Text.Encoding.ASCII.GetBytes(iv);
            rijndaelCipher.IV = ivBytes;

            ICryptoTransform transform = rijndaelCipher.CreateDecryptor();

            byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);

            return Encoding.ASCII.GetString(plainText);
            */
            #endregion
        }//end AESDecrypt
    }//end Class
}//end namespace