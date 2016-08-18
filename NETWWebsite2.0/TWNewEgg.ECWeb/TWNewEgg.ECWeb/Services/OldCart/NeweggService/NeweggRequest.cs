using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using TWNewEgg.GetConfigData.Service;

namespace TWNewEgg.Website.ECWeb.Service
{
    public class NeweggRequest
    {
        TWNewEgg.GetConfigData.Models.WebSiteListWebSiteData WebSiteData = new TWNewEgg.GetConfigData.Models.WebSiteListWebSiteData(0);
        //非管制商品 帳號
        private string _loginNameNonControlled = "po_netw@newegg.com";
        private string _passwordNonControlled = "Newegg61022!";
        public int _customerNumberNonControlled = 0;
        public string _authTokenNonControlled = "";
        //管制商品 帳號
        private string _loginNameControlled = "ponetw2@newegg.com";
        private string _passwordControlled = "newegg61705!";
        public int _customerNumberControlled = 0;
        public string _authTokenControlled = "";

        string environment = "";
        string auth = "";

        public NeweggRequest()
        {
            environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
            auth = System.Configuration.ConfigurationManager.AppSettings[environment + "_NeweggAPIAppKey"]
                + "&" + System.Configuration.ConfigurationManager.AppSettings[environment + "_NeweggAPIToken"];
        }

        public T Get<T>(string url)
        {
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
            req.UserAgent = "Newegg Iphone App";
            if (url.IndexOf("https") >= 0)
            {
                req.Headers.Add("Authorization", auth);
                req.Headers.Add("X-AuthToken", _authTokenNonControlled);
            }
            req.Method = "GET";
            req.Timeout = 10000;
            System.IO.Stream s = req.GetResponse().GetResponseStream();
            System.IO.StreamReader sr = new System.IO.StreamReader(s);
            string str = sr.ReadToEnd();
            sr.Close();
            s.Close();
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            T data = js.Deserialize<T>(str);
            return data;
        }

        public T Post<T>(string url, object body)
        {
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            string serialStr = js.Serialize(body);
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
            req.UserAgent = "Newegg Iphone App";
            if (url.IndexOf("https") >= 0)
            {
                req.Headers.Add("Authorization", auth);
                if (serialStr.IndexOf(this._loginNameNonControlled) >= 0 || serialStr.IndexOf(this._customerNumberNonControlled.ToString()) >= 0)
                {
                    //非管制商品 帳號
                    req.Headers.Add("X-AuthToken", _authTokenNonControlled);
                }
                else
                {
                    //管制商品 帳號
                    req.Headers.Add("X-AuthToken", _authTokenControlled);
                }
            }
            req.Method = "POST";
            req.Timeout = 10000;
            System.IO.Stream streamOut = req.GetRequestStream();
            System.IO.StreamWriter sw = new System.IO.StreamWriter(streamOut);
            sw.Write(serialStr);
            sw.Flush();
            sw.Close();
            streamOut.Close();
            System.IO.Stream streamIn = req.GetResponse().GetResponseStream();
            System.IO.StreamReader sr = new System.IO.StreamReader(streamIn);
            string str = sr.ReadToEnd();
            sr.Close();
            streamIn.Close();
            T data = js.Deserialize<T>(str);
            return data;
        }

        /// <summary>
        /// 使用預設的帳號密碼登入
        /// </summary>
        /// <returns></returns>
        public void Login()
        {
            string reqURL = NeweggConfiguration.SSL + "/ShoppingLogin.egg";
            try
            {
                //非管制商品 帳號
                var parameterForAccount1 = new { LoginName = this._loginNameNonControlled, Password = this._passwordNonControlled };
                var resultForAccount1 = Post<Newegg.Mobile.MvcApplication.Models.UIRegisterResultInfo>(reqURL, parameterForAccount1);
                this._authTokenNonControlled = resultForAccount1.Body.Customer.AuthToken;
                this._customerNumberNonControlled = resultForAccount1.Body.Customer.CustomerNumber;
            }
            catch (Exception ex)
            {
                try
                {
                    //登入失敗 發信通知 Dev Team
                    string env = System.Configuration.ConfigurationManager.AppSettings["Environment"];

                    string mailBody = "Newegg Mobile API Login Failed<br><hr>";
                    mailBody += "<h3>Help...</h3><br>";
                    mailBody += "系統環境:" + env.ToUpper() + "<br>";
                    mailBody += "非管制商品帳號登入失敗..." + this._loginNameNonControlled.Replace("@", "(at)") + "<br><br>";
                    mailBody += "Exception:" + ex.Message;

                    MailMessage msg = new MailMessage();
                    msg.From = new MailAddress("service@newegg.com.tw", WebSiteData.SiteName, System.Text.Encoding.UTF8);
                    msg.To.Add("amos.c.chuang@newegg.com");
                    if (env.ToUpper() == "PRD" || env.ToUpper() == "GQC")
                    {
                        msg.To.Add("Bill.S.Wu@newegg.com");
                        msg.To.Add("Steven.S.Chen@newegg.com");
                        msg.To.Add("Lynn.Y.Yeh@newegg.com");
                        msg.To.Add("Penny.P.Lee@newegg.com");
                        msg.To.Add("Yellow.C.Huang@newegg.com");
                    }
                    msg.Subject = env.ToUpper() + "_非管制商品帳號登入失敗";
                    msg.SubjectEncoding = System.Text.Encoding.UTF8;
                    msg.Body = mailBody;
                    msg.BodyEncoding = System.Text.Encoding.UTF8;
                    msg.IsBodyHtml = true;
                    msg.Priority = MailPriority.Normal;
                    string ECWeb_SMTP = System.Configuration.ConfigurationManager.AppSettings["ECWeb_SMTP"];
                    SmtpClient MySmtp = new SmtpClient(ECWeb_SMTP, 25);
                    MySmtp.Send(msg);
                }
                catch { }
            }
            try
            {
                //管制商品 帳號
                var parameterForAccount2 = new { LoginName = this._loginNameControlled, Password = this._passwordControlled };
                var resultForAccount2 = Post<Newegg.Mobile.MvcApplication.Models.UIRegisterResultInfo>(reqURL, parameterForAccount2);
                this._authTokenControlled = resultForAccount2.Body.Customer.AuthToken;
                this._customerNumberControlled = resultForAccount2.Body.Customer.CustomerNumber;
            }
            catch (Exception ex)
            {
                try
                {
                    //登入失敗 發信通知 Dev Team
                    string env = System.Configuration.ConfigurationManager.AppSettings["Environment"];

                    string mailBody = "Newegg Mobile API Login Failed<br><hr>";
                    mailBody += "<h3>Help...</h3><br>";
                    mailBody += "系統環境:" + env.ToUpper() + "<br>";
                    mailBody += "管制商品帳號登入失敗..." + this._loginNameControlled.Replace("@", "(at)") + "<br><br>";
                    mailBody += "Exception:" + ex.Message;

                    MailMessage msg = new MailMessage();
                    msg.From = new MailAddress("service@newegg.com.tw", WebSiteData.SiteName, System.Text.Encoding.UTF8);
                    msg.To.Add("amos.c.chuang@newegg.com");
                    if (env.ToUpper() == "PRD" || env.ToUpper() == "GQC")
                    {
                        msg.To.Add("Bill.S.Wu@newegg.com");
                        msg.To.Add("Steven.S.Chen@newegg.com");
                        msg.To.Add("Lynn.Y.Yeh@newegg.com");
                        msg.To.Add("Penny.P.Lee@newegg.com");
                        msg.To.Add("Yellow.C.Huang@newegg.com");
                    }
                    msg.Subject = env.ToUpper() + "_管制商品帳號登入失敗";
                    msg.SubjectEncoding = System.Text.Encoding.UTF8;
                    msg.Body = mailBody;
                    msg.BodyEncoding = System.Text.Encoding.UTF8;
                    msg.IsBodyHtml = true;
                    msg.Priority = MailPriority.Normal;
                    string ECWeb_SMTP = System.Configuration.ConfigurationManager.AppSettings["ECWeb_SMTP"];
                    SmtpClient MySmtp = new SmtpClient(ECWeb_SMTP, 25);
                    MySmtp.Send(msg);
                }
                catch { }
            }
        }

        /// <summary>
        /// 帳號登入
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Newegg.Mobile.MvcApplication.Models.UIRegisterResultInfo Login(string user, string password)
        {
            //Login
            string reqURL = NeweggConfiguration.SSL + "/ShoppingLogin.egg";
            var parameter = new { LoginName = user, Password = password };
            Newegg.Mobile.MvcApplication.Models.UIRegisterResultInfo result = Post<Newegg.Mobile.MvcApplication.Models.UIRegisterResultInfo>(reqURL, parameter);
            try
            {
                this._authTokenNonControlled = result.Body.Customer.AuthToken;
                this._customerNumberNonControlled = result.Body.Customer.CustomerNumber;
                this._loginNameNonControlled = user;
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 取得美蛋售價
        /// </summary>
        /// <param name="itemNumber"></param>
        /// <returns></returns>
        public decimal GetPrice(string itemNumber)
        {
            string html = "";
            decimal finalPrice = 0;
            try
            {
                //http://www.newegg.com/Product/MappingPrice.aspx?Item=
                //http://www.newegg.com/Product/MappingPrice2012.aspx?Item=
                string url = "http://www.newegg.com/Product/MappingPrice2012.aspx?Item=" + itemNumber;
                System.Net.HttpWebRequest req = System.Net.HttpWebRequest.CreateHttp(url);
                req.Method = "GET";
                req.Timeout = 1000 * 30;
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                req.Headers.Add("Accept-Language", "en-US,en;q=0.8");
                req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.116 Safari/537.36";

                System.IO.Stream s = req.GetResponse().GetResponseStream();
                System.IO.StreamReader sr = new System.IO.StreamReader(s);
                string str = sr.ReadToEnd();
                sr.Close();
                s.Close();

                html = str;

                str = str.Substring(str.IndexOf("<li class=\"price-current"));
                str = str.Substring(0, str.IndexOf("</li>") + 5);
                string i = str.Substring(str.IndexOf("<strong>") + 8);
                i = i.Substring(0, i.IndexOf("</strong>"));
                string f = str.Substring(str.IndexOf("<sup>") + 5);
                f = f.Substring(0, f.IndexOf("</sup>"));
                i = i.Replace(",", "");
                finalPrice = decimal.Parse(i + f);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + " __ " + html);
            }
            return finalPrice;
        }

        public Newegg.Mobile.MvcApplication.Models.UIProductItemDetailAllInfo GetProductDetail(string itemNumber)
        {
            int count = 0;
            while (true)
            {
                try
                {
                    string reqURL = NeweggConfiguration.WWW + "/products.egg/{itemNumber}/ProductDetails";
                    reqURL = reqURL.Replace("{itemNumber}", itemNumber);
                    Newegg.Mobile.MvcApplication.Models.UIProductItemDetailAllInfo result = Get<Newegg.Mobile.MvcApplication.Models.UIProductItemDetailAllInfo>(reqURL);
                    if (result == null)
                    {
                        throw new Exception();
                    }
                    else
                    {
                        return result;
                    }
                }
                catch
                {
                    count++;
                    if (count > 3)
                        break;
                }
            }
            return null;
        }

        public Newegg.Mobile.MvcApplication.Models.UIProductSearchAdvancedResultInfo AdvancedSearch(Newegg.Mobile.MvcApplication.Models.UIProductSearchAdvancedConditionInfo condition)
        {
            Newegg.Mobile.Service.Interface.IServiceRequest<Newegg.Mobile.MvcApplication.Models.UIProductSearchAdvancedResultInfo> req = Newegg.Mobile.Rest.Client.RestServiceRequestFactory.CreateRequest<Newegg.Mobile.MvcApplication.Models.UIProductSearchAdvancedResultInfo>("SearchAdvanced");
            req.RequestBody = condition;
            req.Send();
            return req.ResponseBody;
        }

        public Newegg.Mobile.MvcApplication.Models.UIReviewContent GetSimpleReviews(string itemNumber, int pageNumber)
        {
            Newegg.Mobile.Web.HttpValueCollection vc = new Newegg.Mobile.Web.HttpValueCollection();
            vc.Add("itemNumber", itemNumber);
            vc.Add("page", pageNumber.ToString());
            Newegg.Mobile.Service.Interface.IServiceRequest<Newegg.Mobile.MvcApplication.Models.UIReviewContent> req = Newegg.Mobile.Rest.Client.RestServiceRequestFactory.CreateRequest<Newegg.Mobile.MvcApplication.Models.UIReviewContent>("SimpleReviews", null, vc);
            req.Send();
            return req.ResponseBody;
        }

        /// <summary>
        /// 使用預設帳號，查詢訂單紀錄
        /// </summary>
        /// <param name="keyword">搜尋條件:關鍵字，或訂單編號</param>
        /// <param name="pageIndex"></param>
        /// <param name="option">時間範圍(過去30、90、120天)或者當前年分(2013)</param>
        /// <param name="isControlled">僅查詢管制或非管制商品下單帳號，預設為NULL兩個帳號查詢結果合併</param>
        /// <returns></returns>
        public Newegg.Mobile.MvcApplication.Models.Message<Newegg.Mobile.MvcApplication.Models.UIOrderHistoryContent> GetOrderHistory(string keyword, int pageIndex, Nullable<bool> isControlled = null, string option = "30")
        {
            //登入帳號
            if (this._customerNumberNonControlled <= 0 || this._customerNumberControlled <= 0)
                Login();
            //抓取訂單紀錄
            string reqURL = NeweggConfiguration.SSL + "/customers/order.egg/list";
            Newegg.Mobile.MvcApplication.Models.Message<Newegg.Mobile.MvcApplication.Models.UIOrderHistoryContent> result = null;
            if (isControlled == null || isControlled == false)
            {
                //查詢 非管制商品 下單帳號的訂單紀錄 +TWN
                try
                {
                    var parameterForAccount = new { LoginName = this._loginNameNonControlled, CustomerNumber = this._customerNumberNonControlled, Keyword = keyword, PageIndex = pageIndex, Option = option, BizUnit = "TWN" };
                    Newegg.Mobile.MvcApplication.Models.Message<Newegg.Mobile.MvcApplication.Models.UIOrderHistoryContent> temp = Post<Newegg.Mobile.MvcApplication.Models.Message<Newegg.Mobile.MvcApplication.Models.UIOrderHistoryContent>>(reqURL, parameterForAccount);
                    if (temp != null && temp.Code == "000")
                    {
                        if (temp.Body != null)
                        {
                            if (temp.Body.OrderSummaryList != null)
                            {
                                if (result != null && result.Body != null && result.Body.OrderSummaryList != null)
                                {
                                    result.Body.OrderSummaryList.AddRange(temp.Body.OrderSummaryList);
                                    if (temp.Body.PageInfo.PageCount > result.Body.PageInfo.PageCount)
                                        result.Body.PageInfo.PageCount = temp.Body.PageInfo.PageCount;
                                }
                                else
                                {
                                    result = temp;
                                }
                            }
                        }
                    }
                }
                catch { }
                //查詢 非管制商品 下單帳號的訂單紀錄
                try
                {
                    var parameterForAccount = new { LoginName = this._loginNameNonControlled, CustomerNumber = this._customerNumberNonControlled, Keyword = keyword, PageIndex = pageIndex, Option = option, BizUnit = "" };
                    Newegg.Mobile.MvcApplication.Models.Message<Newegg.Mobile.MvcApplication.Models.UIOrderHistoryContent> temp = Post<Newegg.Mobile.MvcApplication.Models.Message<Newegg.Mobile.MvcApplication.Models.UIOrderHistoryContent>>(reqURL, parameterForAccount);
                    if (temp != null && temp.Code == "000")
                    {
                        if (temp.Body != null)
                        {
                            if (temp.Body.OrderSummaryList != null)
                            {
                                if (result != null && result.Body != null && result.Body.OrderSummaryList != null)
                                {
                                    result.Body.OrderSummaryList.AddRange(temp.Body.OrderSummaryList);
                                    if (temp.Body.PageInfo.PageCount > result.Body.PageInfo.PageCount)
                                        result.Body.PageInfo.PageCount = temp.Body.PageInfo.PageCount;
                                }
                                else
                                {
                                    result = temp;
                                }
                            }
                        }
                    }
                }
                catch { }
            }
            if (isControlled == null || isControlled == true)
            {
                //查詢 管制商品 下單帳號的訂單紀錄 +TWN
                try
                {
                    var parameterForAccount = new { LoginName = this._loginNameControlled, CustomerNumber = this._customerNumberControlled, Keyword = keyword, PageIndex = pageIndex, Option = option, BizUnit = "TWN" };
                    Newegg.Mobile.MvcApplication.Models.Message<Newegg.Mobile.MvcApplication.Models.UIOrderHistoryContent> temp = Post<Newegg.Mobile.MvcApplication.Models.Message<Newegg.Mobile.MvcApplication.Models.UIOrderHistoryContent>>(reqURL, parameterForAccount);
                    if (temp != null && temp.Code == "000")
                    {
                        if (temp.Body != null)
                        {
                            if (temp.Body.OrderSummaryList != null)
                            {
                                if (result != null && result.Body != null && result.Body.OrderSummaryList != null)
                                {
                                    result.Body.OrderSummaryList.AddRange(temp.Body.OrderSummaryList);
                                    if (temp.Body.PageInfo.PageCount > result.Body.PageInfo.PageCount)
                                        result.Body.PageInfo.PageCount = temp.Body.PageInfo.PageCount;
                                }
                                else
                                {
                                    result = temp;
                                }
                            }
                        }
                    }
                }
                catch { }
                //查詢 管制商品 下單帳號的訂單紀錄
                try
                {
                    var parameterForAccount = new { LoginName = this._loginNameControlled, CustomerNumber = this._customerNumberControlled, Keyword = keyword, PageIndex = pageIndex, Option = option, BizUnit = "" };
                    Newegg.Mobile.MvcApplication.Models.Message<Newegg.Mobile.MvcApplication.Models.UIOrderHistoryContent> temp = Post<Newegg.Mobile.MvcApplication.Models.Message<Newegg.Mobile.MvcApplication.Models.UIOrderHistoryContent>>(reqURL, parameterForAccount);
                    if (temp != null && temp.Code == "000")
                    {
                        if (temp.Body != null)
                        {
                            if (temp.Body.OrderSummaryList != null)
                            {
                                if (result != null && result.Body != null && result.Body.OrderSummaryList != null)
                                {
                                    result.Body.OrderSummaryList.AddRange(temp.Body.OrderSummaryList);
                                    if (temp.Body.PageInfo.PageCount > result.Body.PageInfo.PageCount)
                                        result.Body.PageInfo.PageCount = temp.Body.PageInfo.PageCount;
                                }
                                else
                                {
                                    result = temp;
                                }
                            }
                        }
                    }
                }
                catch { }
            }
            return result;
        }

        /// <summary>
        /// 查詢訂單紀錄
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="customerNumber"></param>
        /// <param name="pageIndex"></param>
        /// <param name="isTWN"></param>
        /// <param name="option">時間範圍(過去30、90、120天)或者當前年分(2013)</param>
        /// <returns></returns>
        /*public Newegg.Mobile.MvcApplication.Models.Message<Newegg.Mobile.MvcApplication.Models.UIOrderHistoryContent> GetOrderHistory(string loginName, int customerNumber, int pageIndex, bool isTWN, string keyword = "", string option = "30")
        {
            string reqURL = NeweggConfiguration.SSL + "/customers/order.egg/list";
            Newegg.Mobile.MvcApplication.Models.Message<Newegg.Mobile.MvcApplication.Models.UIOrderHistoryContent> result = null;
            if (isTWN)
            {
                var parameter1 = new { LoginName = loginName, CustomerNumber = customerNumber, Keyword = keyword, PageIndex = pageIndex, Option = option, BizUnit = "TWN" };
                result = Post<Newegg.Mobile.MvcApplication.Models.Message<Newegg.Mobile.MvcApplication.Models.UIOrderHistoryContent>>(reqURL, parameter1);
            }
            else
            {
                var parameter2 = new { LoginName = loginName, CustomerNumber = customerNumber, Keyword = keyword, PageIndex = pageIndex, Option = option.ToString() };
                result = Post<Newegg.Mobile.MvcApplication.Models.Message<Newegg.Mobile.MvcApplication.Models.UIOrderHistoryContent>>(reqURL, parameter2);
            }
            return result;
        }*/

        public Newegg.Mobile.MvcApplication.Models.Message<Newegg.Mobile.MvcApplication.Models.UIOrderHistoryDetailInfo> GetOrderDetail(int customerNumber, string loginName, int soNumber, string date)
        {
            //GetOrderDetail
            Newegg.Mobile.MvcApplication.Models.UIOrderHistoryDetailParameter parameter = new Newegg.Mobile.MvcApplication.Models.UIOrderHistoryDetailParameter();
            parameter.CustomerNumber = customerNumber;
            parameter.LoginName = loginName;
            parameter.OrderDate = date;
            parameter.SONumber = soNumber;

            Newegg.Mobile.Service.Interface.IServiceRequest<Newegg.Mobile.MvcApplication.Models.Message<Newegg.Mobile.MvcApplication.Models.UIOrderHistoryDetailInfo>> req = Newegg.Mobile.Rest.Client.RestServiceRequestFactory.CreateRequest<Newegg.Mobile.MvcApplication.Models.Message<Newegg.Mobile.MvcApplication.Models.UIOrderHistoryDetailInfo>>("OrderDetail");
            req.RequestBody = parameter;
            req.Send();
            return req.ResponseBody;
        }

        /// <summary>
        /// 取消訂單
        /// </summary>
        /// <param name="soNumber">訂單號碼</param>
        /// <param name="preSoNumber"></param>
        /// <param name="reasonCode"></param>
        /// <param name="reasonDescription"></param>
        /// <returns></returns>
        public Newegg.Mobile.MvcApplication.Models.Message CancelOrder(int soNumber, int preSoNumber = 0, int reasonCode = 33, string reasonDescription = "No Longer Needed")
        {
            log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            logger.Info("CancelOrder SO: " + soNumber);
            Newegg.Mobile.MvcApplication.Models.Message result = null;
            //登入帳號
            if (this._customerNumberNonControlled <= 0 || this._customerNumberControlled <= 0)
                Login();
            //確認此筆訂單是用哪一個帳號下的
            bool isControlled = false;
            Newegg.Mobile.MvcApplication.Models.UIOrderHistorySummaryInfo orderInfo = GetOrderHistory(soNumber.ToString(), 1, isControlled).Body.OrderSummaryList.FirstOrDefault(x => x.SONumber == soNumber);
            if (orderInfo == null)
            {
                isControlled = true;
                orderInfo = GetOrderHistory(soNumber.ToString(), 1, isControlled).Body.OrderSummaryList.FirstOrDefault(x => x.SONumber == soNumber);
            }
            if (orderInfo != null)
            {
                string reqURL = NeweggConfiguration.SSL + "/customers/order.egg/cancelorder";
                if (isControlled)
                {
                    //以 管制商品帳號 取消訂單
                    logger.Info("以管制商品帳號取消訂單 SO:" + soNumber);
                    var parameter = new { PreSoNumber = preSoNumber, SoNumber = soNumber, CustomerNumber = this._customerNumberControlled, ReasonCode = reasonCode, ReasonDescription = reasonDescription };
                    result = Post<Newegg.Mobile.MvcApplication.Models.Message>(reqURL, parameter);
                }
                else
                {
                    //以 非管制商品帳號 取消訂單
                    logger.Info("以非管制商品帳號取消訂單 SO:" + soNumber);
                    var parameter = new { PreSoNumber = preSoNumber, SoNumber = soNumber, CustomerNumber = this._customerNumberNonControlled, ReasonCode = reasonCode, ReasonDescription = reasonDescription };
                    result = Post<Newegg.Mobile.MvcApplication.Models.Message>(reqURL, parameter);
                }
            }

            try
            {
                /*
                 *美蛋訂單_拋送取消_可能會有拋送失敗的狀況
                 *當發生失敗時寄信通知
                 */
                if (result == null || result.Code != "000")
                {
                    logger.Info("Cancel Order Fail SO: " + soNumber);

                    string env = System.Configuration.ConfigurationManager.AppSettings["Environment"];

                    System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                    string serialStr = js.Serialize(result);

                    string mailBody = "取消訂單失敗<hr>";
                    mailBody += "<h3>美蛋訂單編號:" + soNumber + "</h3><br><hr>系統資訊<hr>";
                    mailBody += serialStr;

                    MailMessage msg = new MailMessage();
                    msg.From = new MailAddress("service@newegg.com.tw", WebSiteData.SiteName, System.Text.Encoding.UTF8);
                    msg.To.Add("amos.c.chuang@newegg.com");
                    if (env.ToUpper() == "PRD" || env.ToUpper() == "GQC")
                    {
                        msg.To.Add("Gretchen.H.Yeh@newegg.com");
                        msg.To.Add("Grace.C.Hsiao@newegg.com");
                        msg.To.Add("Kevin.T.Yang@newegg.com");
                        msg.To.Add("steven.c.mao@newegg.com");
                        msg.To.Add("Amine.Y.Chang@newegg.com");
                        msg.To.Add("Dolcee.J.Chang@newegg.com");
                        msg.To.Add("Joyce.H.Hsiao@newegg.com");
                    }
                    msg.Subject = "美蛋訂單_拋送取消_失敗";
                    msg.SubjectEncoding = System.Text.Encoding.UTF8;
                    msg.Body = mailBody;
                    msg.BodyEncoding = System.Text.Encoding.UTF8;
                    msg.IsBodyHtml = true;
                    msg.Priority = MailPriority.Normal;
                    string ECWeb_SMTP = System.Configuration.ConfigurationManager.AppSettings["ECWeb_SMTP"];
                    SmtpClient MySmtp = new SmtpClient(ECWeb_SMTP, 25);
                    MySmtp.Send(msg);
                }
                else
                {
                    logger.Info("Cancel Order Success SO: " + soNumber);
                }
            }
            catch { }

            return result;
        }

        public string GetServerName()
        {
            //GetServerName
            string reqUrl = NeweggConfiguration.WWW + "/Server.egg";
            Dictionary<string, string> result = Get<Dictionary<string, string>>(reqUrl);
            return result["ServerName"];
        }

        private void Register()
        {
            //Register
            /*
             * regInfo.LoginName = "a95145684331180@gmail.com";
             * regInfo.Password = "asd95145684331180";
             * regInfo.LoginName = "asd0001@gmail.com";
             * regInfo.Password = "asd0001";
             * regInfo.LoginName = "asd0002@gmail.com";
             * regInfo.Password = "asd0002";
             * wh08@gmail.com
             * wh08wh08
             */
            Newegg.Mobile.Service.Interface.IServiceRequest<Newegg.Mobile.MvcApplication.Models.UIRegisterResultInfo> req = Newegg.Mobile.Rest.Client.RestServiceRequestFactory.CreateRequest<Newegg.Mobile.MvcApplication.Models.UIRegisterResultInfo>("Register");
            Newegg.Mobile.MvcApplication.Models.UICustomerRegisterInfo regInfo = new Newegg.Mobile.MvcApplication.Models.UICustomerRegisterInfo();
            regInfo.LoginName = "wh08@gmail.com";
            regInfo.Password = "wh08wh08";
            regInfo.AccountType = 0;
            regInfo.AllowNewsLetter = false;
            req.RequestBody = regInfo;
            req.Send();
            req = null;
        }

        private Newegg.Mobile.MvcApplication.Models.Message AddPaymentMethod(string customerNumber)
        {
            //AddPaymentMethod
            Newegg.Mobile.Web.HttpValueCollection vc = new Newegg.Mobile.Web.HttpValueCollection();
            vc.Add("customerNumber", customerNumber);
            Newegg.Mobile.Service.Interface.IServiceRequest<Newegg.Mobile.MvcApplication.Models.Message> addCreditCardReq = Newegg.Mobile.Rest.Client.RestServiceRequestFactory.CreateRequest<Newegg.Mobile.MvcApplication.Models.Message>("AddPaymentMethod", null, vc);
            Newegg.Mobile.MvcApplication.Models.UICreditCardPaymentInfo creditcard = new Newegg.Mobile.MvcApplication.Models.UICreditCardPaymentInfo();
            creditcard.BankPhone = "ugh";
            creditcard.CardNumber = "4111111111111111";
            creditcard.ExpirationDate = "10/14";
            creditcard.HolderName = "asd";
            creditcard.IsDefault = true;
            creditcard.Payterms = "untitled";
            creditcard.PaytermsCode = "001";
            creditcard.PaytermsType = "Visa";
            creditcard.TransactionNumber = 6540562;
            addCreditCardReq.RequestBody = creditcard;
            addCreditCardReq.Send();
            return addCreditCardReq.ResponseBody;
        }

        private Newegg.Mobile.MvcApplication.Models.UICheckoutData Checkout(int customerNumber, string itemNumber, int qty)
        {
            //Checkout
            Newegg.Mobile.MvcApplication.Models.UICheckoutData cdata = new Newegg.Mobile.MvcApplication.Models.UICheckoutData();
            cdata.CustomerNumber = customerNumber;
            List<Newegg.Mobile.MvcApplication.Models.UIProductItem> items = new List<Newegg.Mobile.MvcApplication.Models.UIProductItem>();
            Newegg.Mobile.MvcApplication.Models.UIProductItem item = new Newegg.Mobile.MvcApplication.Models.UIProductItem();
            item.ItemNumber = itemNumber;
            item.Quantity = qty;
            items.Add(item);
            cdata.ItemList = items;
            Newegg.Mobile.Service.Interface.IServiceRequest<Newegg.Mobile.MvcApplication.Models.Message<Newegg.Mobile.MvcApplication.Models.UICheckoutInfo>> req = Newegg.Mobile.Rest.Client.RestServiceRequestFactory.CreateRequest<Newegg.Mobile.MvcApplication.Models.Message<Newegg.Mobile.MvcApplication.Models.UICheckoutInfo>>("MobileEquipmentCheckout");
            req.RequestBody = cdata;
            req.Send();
            cdata.SessionID = req.ResponseBody.Body.SessionID;
            if (req.ResponseBody != null)
            {
                if (req.ResponseBody.Code != "000")
                {
                    string err = "拋送訂單失敗:[Newegg API Checkout Failure]";
                    try
                    {
                        err += " _ " + req.ResponseBody.Description;
                    }
                    catch { }
                    throw new Exception(err);
                }
            }
            return cdata;
        }

        private Newegg.Mobile.MvcApplication.Models.Message<Newegg.Mobile.MvcApplication.Models.UICheckoutResult> PlaceOrder(Newegg.Mobile.MvcApplication.Models.UICheckoutData checkoutData)
        {
            //PlaceOrder
            Newegg.Mobile.Service.Interface.IServiceRequest<Newegg.Mobile.MvcApplication.Models.Message<Newegg.Mobile.MvcApplication.Models.UICheckoutResult>> req = Newegg.Mobile.Rest.Client.RestServiceRequestFactory.CreateRequest<Newegg.Mobile.MvcApplication.Models.Message<Newegg.Mobile.MvcApplication.Models.UICheckoutResult>>("PlaceOrder");
            req.RequestBody = checkoutData;
            req.Send();
            return req.ResponseBody;
        }

        private Newegg.Mobile.MvcApplication.Models.Message AddAddress(int customerNumber, Newegg.Mobile.MvcApplication.Models.UIAddressInfo addr)
        {
            //AddShippingAddress
            Newegg.Mobile.Web.HttpValueCollection vc = new Newegg.Mobile.Web.HttpValueCollection();
            vc = new Newegg.Mobile.Web.HttpValueCollection();
            vc.Add("customerNumber", customerNumber.ToString());
            Newegg.Mobile.Service.Interface.IServiceRequest<Newegg.Mobile.MvcApplication.Models.Message> req = Newegg.Mobile.Rest.Client.RestServiceRequestFactory.CreateRequest<Newegg.Mobile.MvcApplication.Models.Message>("AddShippingAddress", null, vc);
            req.RequestBody = addr;
            req.Send();
            return req.ResponseBody;
        }

        private Newegg.Mobile.MvcApplication.Models.Message<List<Newegg.Mobile.MvcApplication.Models.UIAddressInfo>> GetAddressList(int customerNumber)
        {
            //GetAddressList
            Newegg.Mobile.Web.HttpValueCollection vc = new Newegg.Mobile.Web.HttpValueCollection();
            vc.Add("customerNumber", customerNumber.ToString());
            Newegg.Mobile.Service.Interface.IServiceRequest<Newegg.Mobile.MvcApplication.Models.Message<List<Newegg.Mobile.MvcApplication.Models.UIAddressInfo>>> req = Newegg.Mobile.Rest.Client.RestServiceRequestFactory.CreateRequest<Newegg.Mobile.MvcApplication.Models.Message<List<Newegg.Mobile.MvcApplication.Models.UIAddressInfo>>>("ShippingAddressList", null, vc);
            req.Send();
            return req.ResponseBody;
        }
    }
}