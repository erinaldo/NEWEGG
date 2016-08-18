using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using TWNewEgg.DB;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.SMSService.Models;

namespace TWNewEgg.SMSService.Service
{
    /// <summary>
    /// 中華電信SMS簡訊解決方案
    /// </summary>
    public class HiNetSolution
    {
        // 建立資料庫物件
        private TWSqlDBContext db = new TWSqlDBContext();

        /// <summary>
        /// 簡易傳送簡訊
        /// </summary>
        /// <param name="toNumber">對象電話號碼。此欄可以包含一個以上的門號，以ASCII的逗號 (,) 隔開，最多接受20個獨立門號。</param>
        /// <param name="messageContent">簡訊內容</param>
        /// <returns>中華電信SMS簡訊回傳資料模型LIST</returns>
        public List<HinetSMSSubmitReturnModel> EasySendSMSMessage(string toNumber, string messageContent)
        {
            if (String.IsNullOrEmpty(toNumber))
            {
                return SetReturnSubmitReturn("-99", "沒有手機號碼");
            }

            if (String.IsNullOrEmpty(messageContent))
            {
                return SetReturnSubmitReturn("-99", "沒有簡訊內容");
            }

            HinetSMSSubmitModel hssm = new HinetSMSSubmitModel();

            List<HinetSMSSubmitReturnModel> hssrm = new List<HinetSMSSubmitReturnModel>();
            List<HinetSMSSubmitReturnModel> hssrmCombin = new List<HinetSMSSubmitReturnModel>();

            // 判斷是否為全英文
            string pattern = @"^[A-Za-z0-9]+$";
            Regex regex = new Regex(pattern);
            bool isEnglish = regex.IsMatch(messageContent);

            // 單封簡訊最大長度
            int cutLength = 0;
            // 多封簡訊最大長度
            int sendLength = 0;
            // 中英文編碼
            string msgCode = string.Empty;

            // 中英文長度設定
            if (isEnglish == true)
            {
                // 純英文
                cutLength = 140;
                sendLength = 134;
            }
            else
            {
                // 不是純英文
                cutLength = 70;
                sendLength = 67;
            }

            // 先確認簡訊長度，因為長度不一有不同的處理方式，純英文超過140個字元後就將會切為第二封，不是純英文則為70個字元
            string[] messageArray = SplitString(messageContent, cutLength);

            if (messageArray.Length == 1)
            {
                // 單封簡訊
                hssm.account = System.Configuration.ConfigurationManager.AppSettings["HinetAccount"];
                hssm.password = System.Configuration.ConfigurationManager.AppSettings["HinetPassword"];
                hssm.from_addr_type = FromAddrTypeEnum.MobileNumber;
                hssm.from_addr = System.Configuration.ConfigurationManager.AppSettings["FromAddress"];
                hssm.to_addr_type = ToAddrTypeEnum.MobileNumber;
                hssm.to_addr = toNumber;
                hssm.msg_dlv_time = string.Empty;
                hssm.msg_expire_time = "0";
                hssm.msg_type = MsgTypeEnum.General;

                // 單封簡訊字碼設定
                if (isEnglish == true)
                {
                    msgCode = "0";
                }
                else
                {
                    msgCode = "8";
                }

                hssm.msg_dcs = msgCode;
                hssm.msg_pclid = "0";
                hssm.msg_udhi = "0";
                // 依照是否為英文轉成URL-Encoding
                if (isEnglish == true)
                {
                    hssm.msg = HttpUtility.UrlEncode(messageArray[0], Encoding.ASCII);
                }
                else
                {
                    hssm.msg = StringToUTF16(messageArray[0]);
                }

                hssm.dest_port = string.Empty;

                // 寫入資料庫
                SmsSubmit smsModel = new SmsSubmit();

                smsModel.ToNumber = toNumber;
                smsModel.MessageNo = 1;
                smsModel.MessageContent = hssm.msg;

                db.SmsSubmit.Add(smsModel);
                db.SaveChanges();

                // 傳送訊息
                hssrm = SendSMSMessage(hssm, true);

                // 回傳資訊
                hssrmCombin.AddRange(hssrm);
            }
            else
            {
                // 長簡訊，超過一封
                // 長簡訊為純英文134個字，不是純英文67個字
                string[] messageContentArray = SplitString(messageContent, sendLength);

                // 總共幾封
                string msgCount = messageContentArray.Length.ToString("00");

                // 長簡訊識別碼
                string msgCodeNo = DateTime.Now.Millisecond.ToString("000").Substring(1, 2);

                int i = 1;

                foreach (string msgString in messageContentArray)
                {
                    hssm.account = System.Configuration.ConfigurationManager.AppSettings["HinetAccount"];
                    hssm.password = System.Configuration.ConfigurationManager.AppSettings["HinetPassword"];
                    hssm.from_addr_type = FromAddrTypeEnum.MobileNumber;
                    hssm.from_addr = System.Configuration.ConfigurationManager.AppSettings["FromAddress"];
                    hssm.to_addr_type = ToAddrTypeEnum.MobileNumber;
                    hssm.to_addr = toNumber;
                    hssm.msg_dlv_time = string.Empty;
                    hssm.msg_expire_time = "0";
                    hssm.msg_type = MsgTypeEnum.General;

                    // 長簡訊字碼設定
                    if (isEnglish == true)
                    {
                        msgCode = "1";
                    }
                    else
                    {
                        msgCode = "8";
                    }

                    hssm.msg_dcs = msgCode;
                    hssm.msg_pclid = "0";
                    hssm.msg_udhi = "1";

                    // 依照是否為英文轉成URL-Encoding
                    if (isEnglish == true)
                    {
                        hssm.msg = "050003" + msgCodeNo + msgCount + i.ToString("00") + StringToASCII(msgString);
                    }
                    else
                    {
                        hssm.msg = "050003" + msgCodeNo + msgCount + i.ToString("00") + StringToUTF16(msgString);
                    }

                    hssm.dest_port = string.Empty;

                    // 寫入資料庫
                    SmsSubmit smsModel = new SmsSubmit();

                    smsModel.ToNumber = toNumber;
                    smsModel.MessageNo = i;
                    smsModel.MessageContent = hssm.msg;

                    db.SmsSubmit.Add(smsModel);
                    db.SaveChanges();

                    i = i + 1;

                    // 傳送訊息
                    hssrm = SendSMSMessage(hssm, true);

                    // 回傳資訊
                    hssrmCombin.AddRange(hssrm);
                }
            }

            return hssrmCombin;
        }

        /// <summary>
        /// 傳送簡訊
        /// </summary>
        /// <param name="smsSubmit">中華電信SMS簡訊資料模型</param>
        /// <param name="isSSL">是否使用SSL</param>
        /// <returns>中華電信SMS簡訊回傳資料模型LIST</returns>
        private List<HinetSMSSubmitReturnModel> SendSMSMessage(HinetSMSSubmitModel smsSubmit, bool isSSL)
        {
            string result = string.Empty;

            string submitURL = string.Empty;

            // 判斷是否使用 SSL
            if (isSSL)
            {
                submitURL = smsSubmit.httpsurl;
            }
            else
            {
                submitURL = smsSubmit.httpurl;
            }

            #region 設定Paramater

            // 設定Paramater
            string param = string.Empty;

            // 登入帳號
            if (smsSubmit.account != string.Empty)
            {
                param = param + string.Format("account={0}", smsSubmit.account);
            }

            // 登入密碼
            if (smsSubmit.password != string.Empty)
            {
                param = param + string.Format("&password={0}", smsSubmit.password);
            }

            // 發送者號碼的種類
            param = param + string.Format("&from_addr_type={0}", (int)smsSubmit.from_addr_type);

            // SMS訊息的發送者的號碼
            if (smsSubmit.from_addr != string.Empty)
            {
                param = param + string.Format("&from_addr={0}", smsSubmit.from_addr);
            }

            // 發送對象號碼的種類
            param = param + string.Format("&to_addr_type={0}", (int)smsSubmit.to_addr_type);

            // SMS訊息的發送對象
            if (smsSubmit.to_addr != string.Empty)
            {
                param = param + string.Format("&to_addr={0}", smsSubmit.to_addr);
            }

            // SMS訊息的預約傳送時間
            if (smsSubmit.msg_dlv_time != string.Empty)
            {
                param = param + string.Format("&msg_dlv_time={0}", smsSubmit.msg_dlv_time);
            }

            // SMS訊息的失效時間
            if (smsSubmit.msg_expire_time != string.Empty)
            {
                param = param + string.Format("&msg_expire_time={0}", smsSubmit.msg_expire_time);
            }

            // SMS訊息的訊息種類
            param = param + string.Format("&msg_type={0}", (int)smsSubmit.msg_type);

            // SMS訊息的訊息編碼方式
            if (smsSubmit.msg_dcs != string.Empty)
            {
                param = param + string.Format("&msg_dcs={0}", smsSubmit.msg_dcs);
            }

            // 需設UDHI，此欄設為1，反之，此欄設為0
            if (smsSubmit.msg_udhi != string.Empty)
            {
                param = param + string.Format("&msg_udhi={0}", smsSubmit.msg_udhi);
            }

            if (smsSubmit.msg_type != MsgTypeEnum.General && smsSubmit.msg_type != MsgTypeEnum.Popup)
            {
                // SMS訊息的訊息GSM協定識別碼
                if (smsSubmit.msg_pclid != string.Empty)
                {
                    param = param + string.Format("&msg_pclid={0}", smsSubmit.msg_pclid);
                }

                // 當msg_type=2或3時, 訊息將送至手機上此參數所指定的port (msg_udhi將自動設為1)，以HEX字串表示
                if (smsSubmit.dest_port != string.Empty)
                {
                    param = param + string.Format("&dest_port={0}", smsSubmit.dest_port);
                }
            }

            // SMS訊息的訊息內容
            if (smsSubmit.msg != string.Empty)
            {
                param = param + string.Format("&msg={0}", smsSubmit.msg);
            }

            #endregion 設定Paramater

            //// 設定 HTTPS 連線時，不要理會憑證的有效性問題
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);

            // 建立WebRequest物件
            HttpWebRequest smsRequest = (HttpWebRequest)WebRequest.Create(submitURL + "?" + param);

            smsRequest.Method = "POST";
            // 連線中斷時間設為10秒
            smsRequest.Timeout = 10000;
            // 設定URL編碼字串是以x-www-form-urlencoded方式編碼的字串
            smsRequest.ContentType = "application/x-www-form-urlencoded";

            try
            {
                using (WebResponse response = smsRequest.GetResponse())
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    result = reader.ReadToEnd();
                    reader.Close();
                }


                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                return GetSendReturnMessage(result);
            }
            catch (Exception ex)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                return SetReturnSubmitReturn("-99", ex.Message);
            }
        }

        /// <summary>
        /// 簡訊回傳訊息
        /// </summary>
        /// <param name="returnHTML">回傳HTML字串</param>
        /// <returns>中華電信SMS簡訊回傳資料模型LIST</returns>
        private List<HinetSMSSubmitReturnModel> GetSendReturnMessage(string returnHTML)
        {
            List<HinetSMSSubmitReturnModel> hssrmList = new List<HinetSMSSubmitReturnModel>();

            string bodyString = string.Empty;

            // 讀取HTML字串
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(returnHTML);

            // 讀取<body>內的資訊
            HtmlNodeCollection htmlNodes = htmlDoc.DocumentNode.SelectNodes("/html/body");
            foreach (HtmlNode node in htmlNodes)
            {
                bodyString = node.InnerHtml.Replace("\n", "");
            }

            //// 拆解<body>內的字串，以『|』(pipeline)做分隔，{to_addr} | {return code} | {messageid | description}
            //// 如為多筆，則在每組訊息間加上<br>為區隔
            //// 先判斷是否有<br>確定是單筆或是多筆
            if (bodyString.IndexOf("<br>", StringComparison.OrdinalIgnoreCase) == 0)
            {
                string[] arrayMessage = bodyString.Split('|');

                HinetSMSSubmitReturnModel hssrm = new HinetSMSSubmitReturnModel();

                hssrm.to_addr = arrayMessage[0];
                hssrm.return_code = arrayMessage[1];
                hssrm.messageid = arrayMessage[2];
                hssrm.description = arrayMessage[3];

                // 寫入資料庫
                SmsSubmitReturn smsReturnModel = new SmsSubmitReturn();

                smsReturnModel.ToNumber = arrayMessage[0];
                smsReturnModel.ReturnCode = arrayMessage[1];
                smsReturnModel.MessageID = arrayMessage[2];
                smsReturnModel.ReturnDescription = arrayMessage[3];

                db.SmsSubmitReturn.Add(smsReturnModel);
                db.SaveChanges();
                hssrm.smsID = smsReturnModel.SmsID;
                hssrmList.Add(hssrm);
            }
            else
            {
                string[] arrayMessageSet = bodyString.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string stringMessage in arrayMessageSet)
                {
                    string[] arrayMessage = stringMessage.Split('|');

                    HinetSMSSubmitReturnModel hssrm = new HinetSMSSubmitReturnModel();

                    hssrm.to_addr = arrayMessage[0];
                    hssrm.return_code = arrayMessage[1];
                    hssrm.messageid = arrayMessage[2];
                    hssrm.description = arrayMessage[3];

                    // 寫入資料庫
                    SmsSubmitReturn smsReturnModel = new SmsSubmitReturn();

                    smsReturnModel.ToNumber = arrayMessage[0];
                    smsReturnModel.ReturnCode = arrayMessage[1];
                    smsReturnModel.MessageID = arrayMessage[2];
                    smsReturnModel.ReturnDescription = arrayMessage[3];

                    db.SmsSubmitReturn.Add(smsReturnModel);
                    db.SaveChanges();
                    hssrm.smsID = smsReturnModel.SmsID;
                    hssrmList.Add(hssrm);
                }
            }

            return hssrmList;
        }

        /// <summary>
        /// 簡易查詢狀態
        /// </summary>
        /// <param name="toNumber">對象電話號碼</param>
        /// <param name="messageid">是否使用SSL</param>
        /// <returns>中華電信SMS查詢回傳資料模型LIST</returns>
        public List<HinetSMSQueryReturnModel> EasyQuerySMSMessage(string toNumber, string messageid)
        {
            if (String.IsNullOrEmpty(toNumber))
            {
                return SetReturnQueryReturn("-99", "沒有手機號碼");
            }

            if (String.IsNullOrEmpty(messageid))
            {
                return SetReturnQueryReturn("-99", "沒有簡訊ID");
            }

            HinetSMSQueryModel hsqm = new HinetSMSQueryModel();

            List<HinetSMSQueryReturnModel> hssrmCombin = new List<HinetSMSQueryReturnModel>();

            hsqm.account = GetAppSetting("HinetAccount");
            hsqm.password = GetAppSetting("HinetPassword");
            hsqm.to_addr_type = ToAddrTypeEnum.MobileNumber;
            hsqm.to_addr = toNumber;
            hsqm.messageid = messageid;

            hssrmCombin = QuerySMSMessage(hsqm, true);

            return hssrmCombin;
        }

        /// <summary>
        /// 查詢狀態
        /// </summary>
        /// <param name="smsQuery">中華電信SMS查詢資料模型</param>
        /// <param name="isSSL">是否使用SSL</param>
        /// <returns>中華電信SMS查詢回傳資料模型LIST</returns>
        private List<HinetSMSQueryReturnModel> QuerySMSMessage(HinetSMSQueryModel smsQuery, bool isSSL)
        {
            string result = string.Empty;

            string submitURL = string.Empty;

            // 判斷是否使用 SSL
            if (isSSL)
            {
                submitURL = smsQuery.httpsurl;
            }
            else
            {
                submitURL = smsQuery.httpurl;
            }

            #region 設定Paramater

            // 設定Paramater
            string param = string.Empty;

            // 登入帳號
            if (smsQuery.account != string.Empty)
            {
                param = param + string.Format("account={0}", smsQuery.account);
            }

            // 登入密碼
            if (smsQuery.password != string.Empty)
            {
                param = param + string.Format("&password={0}", smsQuery.password);
            }

            // 發送對象號碼的種類
            param = param + string.Format("&to_addr_type={0}", (int)smsQuery.to_addr_type);

            // SMS訊息的發送對象
            if (smsQuery.to_addr != string.Empty)
            {
                param = param + string.Format("&to_addr={0}", smsQuery.to_addr);
            }

            // 由IMSP SMS Server在成功的SMS傳送請求時傳回的 message id
            if (smsQuery.messageid != string.Empty)
            {
                param = param + string.Format("&messageid={0}", smsQuery.messageid);
            }

            #endregion 設定Paramater

            //// 設定 HTTPS 連線時，不要理會憑證的有效性問題
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);

            // 建立WebRequest物件
            HttpWebRequest smsRequest = (HttpWebRequest)WebRequest.Create(submitURL + "?" + param);

            smsRequest.Method = "POST";
            // 連線中斷時間設為10秒
            smsRequest.Timeout = 10000;
            // 設定URL編碼字串是以x-www-form-urlencoded方式編碼的字串
            smsRequest.ContentType = "application/x-www-form-urlencoded";

            try
            {
                using (WebResponse response = smsRequest.GetResponse())
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    result = reader.ReadToEnd();
                    reader.Close();
                }

                return GetQueryReturnMessage(result);
            }
            catch (Exception ex)
            {
                return SetReturnQueryReturn("-99", ex.Message);
            }
        }

        /// <summary>
        /// 查詢回傳訊息
        /// </summary>
        /// <param name="returnHTML">回傳HTML字串</param>
        /// <returns>中華電信SMS查詢回傳資料模型LIST</returns>
        private List<HinetSMSQueryReturnModel> GetQueryReturnMessage(string returnHTML)
        {
            List<HinetSMSQueryReturnModel> hsqrmList = new List<HinetSMSQueryReturnModel>();

            string bodyString = string.Empty;

            // 讀取HTML字串
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(returnHTML);

            // 讀取<body>內的資訊
            HtmlNodeCollection htmlNodes = htmlDoc.DocumentNode.SelectNodes("/html/body");
            foreach (HtmlNode node in htmlNodes)
            {
                bodyString = node.InnerHtml.Replace("\n", "");
            }

            //// 拆解<body>內的字串，以『|』(pipeline)做分隔，{to_addr} | {return code} | {done_time | description}
            //// 如為多筆，則在每組訊息間加上<br>為區隔
            //// 先判斷是否有<br>確定是單筆或是多筆
            if (bodyString.IndexOf("<br>", StringComparison.OrdinalIgnoreCase) == 0)
            {
                string[] arrayMessage = bodyString.Split('|');

                HinetSMSQueryReturnModel hsqrm = new HinetSMSQueryReturnModel();

                hsqrm.to_addr = arrayMessage[0];
                hsqrm.return_code = arrayMessage[1];
                hsqrm.done_time = arrayMessage[2];
                hsqrm.description = arrayMessage[3];

                hsqrmList.Add(hsqrm);
            }
            else
            {
                string[] arrayMessageSet = bodyString.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string stringMessage in arrayMessageSet)
                {
                    string[] arrayMessage = stringMessage.Split('|');

                    HinetSMSQueryReturnModel hsqrm = new HinetSMSQueryReturnModel();

                    hsqrm.to_addr = arrayMessage[0];
                    hsqrm.return_code = arrayMessage[1];
                    hsqrm.done_time = arrayMessage[2];
                    hsqrm.description = arrayMessage[3];

                    hsqrmList.Add(hsqrm);
                }
            }

            return hsqrmList;
        }

        /// <summary>
        /// 設定 HTTPS 連線時，不要理會憑證的有效性問題
        /// </summary>
        /// <param name="sender">傳出物件</param>
        /// <param name="certificate">憑證驗證</param>
        /// <param name="chain">憑證鏈</param>
        /// <param name="sslPolicyErrors">SSL規則錯誤</param>
        /// <returns>直接回傳認證</returns>
        private static bool ValidateServerCertificate(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        /// <summary>
        /// 確定字串長度，純英文簡訊一次最多140個字
        /// 若超過140，則每封訊息則為134個字
        /// 非純英文簡訊一次最多70個字
        /// 若超過70，則每封訊息則為67個字
        /// </summary>
        /// <param name="smsmessage">傳送訊息全文</param>
        /// <param name="maxLength">訊息長度</param>
        /// <returns>回傳字串陣列</returns>
        private string[] SplitString(string smsmessage, int maxLength)
        {
            int stringLehgth = 0;

            string tempString = string.Empty;

            for (int i = 0; i < smsmessage.Length; i++)
            {
                stringLehgth = stringLehgth + 1;

                if (stringLehgth <= maxLength)
                {
                    tempString = tempString + smsmessage.Substring(i, 1);
                }
                else
                {
                    stringLehgth = 1;

                    tempString = tempString + "||" + smsmessage.Substring(i, 1);
                }
            }

            string[] arrayString = tempString.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);

            return arrayString;
        }

        /// <summary>
        /// 判斷字元長度
        /// </summary>
        /// <param name="word">字元</param>
        /// <returns>字元長度</returns>
        private int ByteLength(string word)
        {
            // 950為BIG5
            return Encoding.GetEncoding(950).GetBytes(word).Length;
        }

        /// <summary>
        /// 將字串轉成16進位ASCII
        /// </summary>
        /// <param name="sourceString">傳入字串</param>
        /// <returns>回傳16進位ASCII字串</returns>
        private string StringToASCII(string sourceString)
        {
            string returnString = string.Empty;

            byte[] byteArray = Encoding.ASCII.GetBytes(sourceString);

            foreach (byte bytes in byteArray)
            {
                returnString = returnString + Convert.ToString(bytes, 16).ToUpper();
            }

            return returnString;
        }

        /// <summary>
        /// 將字串轉成UTF16
        /// </summary>
        /// <param name="sourceString">傳入字串</param>
        /// <returns>回傳UTF16字串</returns>
        private string StringToUTF16(string sourceString)
        {
            int encodeValue;
            char[] charArray = sourceString.ToCharArray();
            StringBuilder uniText = new StringBuilder(sourceString.Length * 2);
            for (int i = 0; i < charArray.Length; i++)
            {
                char a = charArray[i];
                encodeValue = (a >> 8);
                string hexCode = encodeValue.ToString("X");

                if (hexCode.ToString().Length == 1)
                {
                    uniText.Append("0");
                }

                uniText.Append(hexCode);
                encodeValue = (a & 0xFF);
                hexCode = encodeValue.ToString("X");

                if (hexCode.ToString().Length == 1)
                {
                    uniText.Append("0");
                }

                uniText.Append(hexCode);
            }

            return (uniText.ToString());
        }

        /// <summary>
        /// 簡訊傳送回傳訊息
        /// </summary>
        /// <param name="returnCode">回傳碼</param>
        /// <returns>訊息內容</returns>
        private string GetHinetSMSSubmitMessage(string returnCode)
        {
            string returnDescription = string.Empty;

            switch (returnCode)
            {
                case "-1":
                    returnDescription = "系統或是資料庫故障";
                    break;

                case "0":
                    returnDescription = "訊息已成功接收";
                    break;

                case "2":
                    returnDescription = "訊息傳送失敗";
                    break;

                case "3":
                    returnDescription = "訊息預約時間超過48小時";
                    break;

                case "5":
                    returnDescription = "訊息從Big-5轉碼到UCS失敗";
                    break;

                case "11":
                    returnDescription = "參數錯誤";
                    break;

                case "12":
                    returnDescription = "訊息的失效時間數值錯誤";
                    break;

                case "13":
                    returnDescription = "SMS訊息的訊息種類不屬於合法的message type";
                    break;

                case "14":
                    returnDescription = "用戶具備改發訊號碼權限，請填發訊號碼";
                    break;

                case "15":
                    returnDescription = "發訊號碼格式錯誤";
                    break;

                case "16":
                    returnDescription = "系統無法執行msisdn<->subno，請稍候再試";
                    break;

                case "17":
                    returnDescription = "系統無法找出對應此subno之電話號碼，請查明subno是否正確";
                    break;

                case "18":
                    returnDescription = "請檢查受訊方號碼格式是否正確";
                    break;

                case "19":
                    returnDescription = "受訊號碼數目超過系統限制(目前為20)";
                    break;

                case "20":
                    returnDescription = "訊息長度不正確";
                    break;

                case "22":
                    returnDescription = "帳號或是密碼錯誤";
                    break;

                case "23":
                    returnDescription = "你的登入IP未在系統註冊";
                    break;

                case "24":
                    returnDescription = "帳號已停用";
                    break;

                case "31":
                    returnDescription = "企業預付帳號沒有金額，請儲值";
                    break;

                case "32":
                    returnDescription = "企業預付帳號尚未開通使用，請洽服務人員";
                    break;

                case "33":
                    returnDescription = "企業預付儲值金額已經逾期，已無法使用，請儲值";
                    break;

                case "34":
                    returnDescription = "企業預付儲值系統發生介接錯誤，請洽服務人員";
                    break;

                case "35":
                    returnDescription = "抱歉，企業預付系統扣款錯誤，請再試";
                    break;

                case "36":
                    returnDescription = "抱歉，企業預付扣款系統鎖住，暫時無法使用，請再試";
                    break;

                case "37":
                    returnDescription = "你的企業預付扣款帳號鎖住，暫時無法使用，請再試(可能你正以多條連線同時發訊所產生，請再重試)";
                    break;

                case "41":
                    returnDescription = "發訊內容含有系統不允許發送的字集，請修改訊息內容再發訊";
                    break;

                case "43":
                    returnDescription = "這個受訊號碼是空號(此錯誤碼只會發生在限發CHT的用戶發訊時產生)";
                    break;

                case "44":
                    returnDescription = "無法判斷號碼是否屬於中華電信門號。如果計費號碼是放心講客戶，則會因為無法判斷受訊號碼屬於網內/網外，無法決定費率，而停止服務";
                    break;

                case "45":
                    returnDescription = "放心講客戶餘額不足，無法發訊";
                    break;

                case "46":
                    returnDescription = "無法決定計費客戶屬性，而停止服務";
                    break;

                case "47":
                    returnDescription = "該特碼帳號無法提供預付式客戶使用";
                    break;

                case "48":
                    returnDescription = "受訊客戶要求拒收加值簡訊，請不要重送";
                    break;

                case "49":
                    returnDescription = "顯示於手機之發訊號碼格式不對";
                    break;

                case "50":
                    returnDescription = "放心講系統扣款錯誤，請再試";
                    break;

                case "51":
                    returnDescription = "預付客戶餘額不足，無法發訊";
                    break;

                case "52":
                    returnDescription = "抱歉，預付式系統扣款錯誤，請再試";
                    break;

                case "55":
                    returnDescription = "http (port 8008) 連線不允許使用 GET 方法，請改用 POST或改為https(port 4443) 連線";
                    break;

                default:
                    returnDescription = string.Empty;
                    break;
            }

            return returnDescription;
        }

        /// <summary>
        /// 查詢傳送回傳訊息
        /// </summary>
        /// <param name="returnCode">回傳碼</param>
        /// <returns>訊息內容</returns>
        private string GetHinetSMSQueryMessage(string returnCode)
        {
            string returnDescription = string.Empty;

            switch (returnCode)
            {
                case "-1":
                    returnDescription = "系統或是資料庫故障";
                    break;

                case "0":
                    returnDescription = "訊息已成功發送至接收端";
                    break;

                case "1":
                    returnDescription = "訊息傳送中";
                    break;

                case "2":
                    returnDescription = "系統無法找到您要找的訊息。請檢查你的toaddr和messageid是否正確";
                    break;

                case "3":
                    returnDescription = "訊息無法成功送達手機";
                    break;

                case "4":
                    returnDescription = "系統或是資料庫故障";
                    break;

                case "5":
                    returnDescription = "訊息狀態不明。此筆訊息已被刪除";
                    break;

                case "8":
                    returnDescription = "接收端SIM已滿，造成訊息傳送失敗";
                    break;

                case "9":
                    returnDescription = "錯誤的接收端號碼，可能是空號";
                    break;

                case "11":
                    returnDescription = "號碼格式錯誤";
                    break;

                case "12":
                    returnDescription = "收訊手機已設定拒收簡訊";
                    break;

                case "13":
                    returnDescription = "手機錯誤";
                    break;

                case "16":
                    returnDescription = "系統無法執行msisdn<->subno，請稍候再試";
                    break;

                case "17":
                    returnDescription = "系統無法找出對應此subno之電話號碼，請查明subno是否正確";
                    break;

                case "18":
                    returnDescription = "請檢查受訊方號碼格式是否正確";
                    break;

                case "21":
                    returnDescription = "請檢查Message id格式是否正確";
                    break;

                case "23":
                    returnDescription = "你的登入IP未在系統註冊";
                    break;

                case "24":
                    returnDescription = "帳號已停用";
                    break;

                case "31":
                    returnDescription = "訊息尚未傳送到SMSC";
                    break;

                case "32":
                    returnDescription = "訊息無法傳送到簡訊中心";
                    break;

                case "33":
                    returnDescription = "訊息無法傳送到簡訊中心(訊務繁忙)";
                    break;

                case "48":
                    returnDescription = "受訊客戶要求拒收加值簡訊，請不要再重送";
                    break;

                case "55":
                    returnDescription = "http (port 8008) 連線不允許使用 GET 方法，請改用 POST或改為https(port 4443) 連線。";
                    break;

                default:
                    returnDescription = string.Empty;
                    break;
            }

            return returnDescription;
        }

        /// <summary>
        /// 設定傳送回傳資訊
        /// </summary>
        /// <param name="return_code">回傳代碼</param>
        /// <param name="description">回傳說明</param>
        /// <returns>傳送回傳資訊模型List</returns>
        private List<HinetSMSSubmitReturnModel> SetReturnSubmitReturn(string return_code, string description)
        {
            HinetSMSSubmitReturnModel returnModel = new HinetSMSSubmitReturnModel();

            returnModel.return_code = return_code;
            returnModel.description = description;

            List<HinetSMSSubmitReturnModel> returnModellist = new List<HinetSMSSubmitReturnModel>();

            returnModellist.Add(returnModel);

            return returnModellist;
        }

        /// <summary>
        /// 設定查詢回傳資訊
        /// </summary>
        /// <param name="return_code">回傳代碼</param>
        /// <param name="description">回傳說明</param>
        /// <returns>查詢回傳資訊模型List</returns>
        private List<HinetSMSQueryReturnModel> SetReturnQueryReturn(string return_code, string description)
        {
            HinetSMSQueryReturnModel returnModel = new HinetSMSQueryReturnModel();

            returnModel.return_code = return_code;
            returnModel.description = description;

            List<HinetSMSQueryReturnModel> returnModellist = new List<HinetSMSQueryReturnModel>();

            returnModellist.Add(returnModel);

            return returnModellist;
        }

        /// <summary>
        /// 取得DLL的Setting檔
        /// </summary>
        /// <param name="key">Setting的Key</param>
        /// <returns>Value字串</returns>
        private string GetAppSetting(string key)
        {
            string result = string.Empty;
            var uri = new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase));
            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = Path.Combine(uri.LocalPath, "SMSService.config") };
            var assemblyConfig = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            if (assemblyConfig.HasFile)
            {
                AppSettingsSection section = (assemblyConfig.GetSection("appSettings") as AppSettingsSection);
                result = section.Settings[key].Value;
            }

            return result;
        }
    }
}