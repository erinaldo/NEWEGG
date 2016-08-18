using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using log4net;
using TWNewEgg.NewEggUSGateway.Models;
using TWNewEgg.NewEggUSGateway.Interface;
namespace TWNewEgg.NewEggUSGateway
{
    public class NeweggUSAAPI : INeweggUSAAPI
    {       //非管制商品 帳號
        private string _loginNameNonControlled = "po_netw@newegg.com";
        private string _passwordNonControlled = "Newegg61022!";
        public int _customerNumberNonControlled = 0;
        public string _authTokenNonControlled = "";
        //管制商品 帳號
        private string _loginNameControlled = "ponetw2@newegg.com";
        private string _passwordControlled = "newegg61705!";
        public int _customerNumberControlled = 0;
        public string _authTokenControlled = "";
        string auth = "";
        string Env = "";
        string NeweggUSAServiceUri;

        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        public NeweggUSAAPI()
        {


            this.Env = System.Configuration.ConfigurationManager.AppSettings["Environment"];
            this.NeweggUSAServiceUri = System.Configuration.ConfigurationManager.AppSettings[Env + "_NeweggAPICreateSOService"];
            this.auth = System.Configuration.ConfigurationManager.AppSettings[Env + "_NeweggAPIAppKey"]
              + "&" + System.Configuration.ConfigurationManager.AppSettings[Env + "_NeweggAPIToken"];

        }

        public ActionResponse<string> SendToNeweggUSA(SendToNeweggUSAApiModel SendToNeweggUSAApiModel)
        {
            ActionResponse<string> ActionResponse = new ActionResponse<string>();
            ActionResponse.Msg += "Send Start...ItemNumber:" + SendToNeweggUSAApiModel.itemNumber;
            logger.Info("Send Start...ItemNumber:" + SendToNeweggUSAApiModel.itemNumber);

            //依據美蛋的資料規則，運費若為0.01 則變更為0
            if (SendToNeweggUSAApiModel.shippingCharge <= 0.01m)
            {
                SendToNeweggUSAApiModel.shippingCharge = 0m;
            }

            Publish sendOrderInfo = new Publish();

            sendOrderInfo.schemaLocation = "http://soa.newegg.com/SOA/USA/InfrastructureService/V10/EcommercePubSubServiceNEG.OrderManagement.CreateSOForEDI.V10.xsd";
            sendOrderInfo.Subject = "CreateSOForEDI";
            sendOrderInfo.FromService = "http://soa.newegg.com/SOA/USA/InfrastructureService/V10/Ecommerce/PubSubService";
            sendOrderInfo.ToService = "http://soa.newegg.com/SOA/USA/OrderManagement/V10/SSL31/CreateSOForEDI";

            Message sendMessage = new Message();

            HeaderInfo sendHeaderInfo = new HeaderInfo();
            sendHeaderInfo.Namespace = "http://soa.newegg.com/OrderManagement";
            sendHeaderInfo.Version = "V10";
            sendHeaderInfo.Action = "CreateSOForEDI";
            sendHeaderInfo.Sender = "EDIPortal";
            sendHeaderInfo.CompanyID = 1003;
            sendHeaderInfo.Tag = "CreateSOForEDI";
            sendHeaderInfo.Language = "en-us";
            sendHeaderInfo.CountryCode = "USA";
            sendHeaderInfo.Description = SendToNeweggUSAApiModel.commissionNote;
            sendHeaderInfo.OriginalSender = "";
            sendHeaderInfo.CallbackAddress = "";
            sendHeaderInfo.From = "";
            sendHeaderInfo.To = "";
            sendHeaderInfo.FromSystem = "";
            sendHeaderInfo.ToSystem = "";
            sendHeaderInfo.GlobalBusinessType = "Listing";
            sendHeaderInfo.TransactionCode = "UD-002-0-001";

            sendMessage.Header = sendHeaderInfo;

            BodyInfo sendBodyInfo = new BodyInfo();

            OrderInfoBase orderInfoBase;

            Newegg.Mobile.MvcApplication.Models.UIProductItemDetailAllInfo productDetail = GetProductDetail(SendToNeweggUSAApiModel.itemNumber);
            TWNewEgg.NewEggService.Models.ProductDetail Detail = GetProductDetailV2(SendToNeweggUSAApiModel.itemNumber);
            if (Detail != null && Detail.Basic != null && Detail.Basic.SellerInfo != null && string.IsNullOrEmpty(Detail.Basic.SellerInfo.SellerId) == false && SendToNeweggUSAApiModel.itemNumber.IndexOf("9SI") >= 0)
            {

                if (productDetail.IsShipByNewegg == true)
                {

                    orderInfoBase = new Models.OrderInfo9SI();
                    Models.OrderInfo9SI tempOrderInfo9SI = (Models.OrderInfo9SI)orderInfoBase;
                    tempOrderInfo9SI.SOType = 99998;

                }
                else
                {
                    orderInfoBase = new Models.OrderInfo9SI();
                    Models.OrderInfo9SI tempOrderInfo9SI = (Models.OrderInfo9SI)orderInfoBase;
                    tempOrderInfo9SI.SOType = 99999;

                }
            }
            else
            {
                //淡水
                orderInfoBase = new Models.OrderInfo();
                Models.OrderInfo tempOrderInfo = (Models.OrderInfo)orderInfoBase;
                tempOrderInfo.IsInternational = 1;
            }

            //is newegg flash ??
            orderInfoBase.IsNF = SendToNeweggUSAApiModel.isNeweggFlash;

            orderInfoBase.EDIID = Guid.NewGuid().ToString();
            if (SendToNeweggUSAApiModel.isControlled)
            {
                //管制商品 下單帳號 7535442
                orderInfoBase.CustomerNumber = 47519290;
            }
            else
            {
                //非管制商品 下單帳號 24672992
                orderInfoBase.CustomerNumber = 47519410;
            }
            orderInfoBase.SODate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            orderInfoBase.SOAmount = 0;
            orderInfoBase.ShipViaCode = SendToNeweggUSAApiModel.shipViaCode;
            orderInfoBase.CustomerOwnShippingAccount = SendToNeweggUSAApiModel.customerOwnShippingAccount;

            // 取得本機名稱
            string strHostName = System.Net.Dns.GetHostName();

            // 取得本機的IpHostEntry類別實體
            System.Net.IPHostEntry iphostentry = System.Net.Dns.GetHostEntry(strHostName);

            string ip = "";
            // 取得所有 IP 位址
            foreach (System.Net.IPAddress ipaddress in iphostentry.AddressList)
            {
                // 只取得IP V4的Address
                if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ip = ipaddress.ToString();
                    break;
                }
            }

            orderInfoBase.IPAddress = ip;
            orderInfoBase.CompanyCode = 1003;
            orderInfoBase.TaxRate = 0;
            orderInfoBase.CustomerPONumber = SendToNeweggUSAApiModel.refPONumber;
            orderInfoBase.ReferenceSONumber = "";
            orderInfoBase.PayTermsCode = "011";
            orderInfoBase.SalesPostUser = "TWPM";
            orderInfoBase.SalesPostDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            orderInfoBase.AcctPostUser = "TWPM";
            orderInfoBase.AcctPostDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            orderInfoBase.SalesPerson = "TWPM";
            orderInfoBase.CreditCardCharged = 1;
            orderInfoBase.CreditCardVerifyMark = "G";
            orderInfoBase.SOMemo = "TWPM";
            orderInfoBase.SpecialComment = SendToNeweggUSAApiModel.specialComment;

            //SellerInfo
            SellerInfo sendSellerInfo = new SellerInfo();

            if (SendToNeweggUSAApiModel.itemNumber.IndexOf("9SI") >= 0)
            {
                //取得產品詳細資訊
                //if (Env == "PRD")
                //{
                    //正式PRD環境


                    if (Detail != null && productDetail != null && Detail.Basic!=null && Detail.Basic.SellerInfo != null)
                    {
                        if (Detail.Basic.SellerInfo.SellerId != null)
                        {
                            sendSellerInfo.SellerID = Detail.Basic.SellerInfo.SellerId;
                        }
                        else
                        {
                            ActionResponse.Msg += " ItemNumber:" + SendToNeweggUSAApiModel.itemNumber + " Seller資訊查詢失敗";
                            ActionResponse.IsSuccess = false;
                            return ActionResponse;
                            //throw new Exception(" ItemNumber:" + itemNumber + " Seller資訊查詢失敗");
                        }
                        if (Detail.Basic.SellerInfo.SellerName != null)
                        {
                            sendSellerInfo.SellerName = Detail.Basic.SellerInfo.SellerName;
                        }
                        else
                        {
                            ActionResponse.Msg += " ItemNumber:" + SendToNeweggUSAApiModel.itemNumber + " Seller資訊查詢失敗";
                            ActionResponse.IsSuccess = false;
                            return ActionResponse;
                        }
                    }
                    else
                    {
                        ActionResponse.Msg += " ItemNumber:" + SendToNeweggUSAApiModel.itemNumber + " Seller資訊查詢失敗";
                        ActionResponse.IsSuccess = false;
                        return ActionResponse;

                    }
                //}
                //else
                //{
                //    //"AWE5";
                //    //"Test_Sandbox_Taiwan_004";
                //    sendSellerInfo.SellerID = "AWE5";
                //    sendSellerInfo.SellerName = "Test_Sandbox_Taiwan_004";
                //}
            }
            else
            {
                //非 9SI 商品 SellerID、SellerName 給予空字串
                sendSellerInfo.SellerID = "";
                sendSellerInfo.SellerName = "";
            }

            orderInfoBase.SellerInfo = sendSellerInfo;

            //MKTOrderInfo
            sendBodyInfo.MKTOrderInfo = orderInfoBase;
            sendBodyInfo.MKTOrderInfo.CustomerBillingInfo = new CustomerInfo();
            //帳單地址
            sendBodyInfo.MKTOrderInfo.CustomerBillingInfo.Address1 = SendToNeweggUSAApiModel.customerBillingInfo.Address1;
            sendBodyInfo.MKTOrderInfo.CustomerBillingInfo.Address1 = SendToNeweggUSAApiModel.customerBillingInfo.Address1;
            sendBodyInfo.MKTOrderInfo.CustomerBillingInfo.Address2 = SendToNeweggUSAApiModel.customerBillingInfo.Address2;
            sendBodyInfo.MKTOrderInfo.CustomerBillingInfo.City = SendToNeweggUSAApiModel.customerBillingInfo.City;
            sendBodyInfo.MKTOrderInfo.CustomerBillingInfo.CompanyName = SendToNeweggUSAApiModel.customerBillingInfo.CompanyName;
            sendBodyInfo.MKTOrderInfo.CustomerBillingInfo.ContactWith = SendToNeweggUSAApiModel.customerBillingInfo.ContactWith;
            sendBodyInfo.MKTOrderInfo.CustomerBillingInfo.Country = SendToNeweggUSAApiModel.customerBillingInfo.Country;
            sendBodyInfo.MKTOrderInfo.CustomerBillingInfo.Fax = SendToNeweggUSAApiModel.customerBillingInfo.Fax;
            sendBodyInfo.MKTOrderInfo.CustomerBillingInfo.HomePhone = SendToNeweggUSAApiModel.customerBillingInfo.HomePhone;
            sendBodyInfo.MKTOrderInfo.CustomerBillingInfo.State = SendToNeweggUSAApiModel.customerBillingInfo.State;
            sendBodyInfo.MKTOrderInfo.CustomerBillingInfo.ZipCode = SendToNeweggUSAApiModel.customerBillingInfo.ZipCode;

            //商品配送地址
            sendBodyInfo.MKTOrderInfo.CustomerShippingInfo = new CustomerInfo();
            sendBodyInfo.MKTOrderInfo.CustomerShippingInfo.Address1 = SendToNeweggUSAApiModel.customerShippingInfo.Address1;
            sendBodyInfo.MKTOrderInfo.CustomerShippingInfo.Address1 = SendToNeweggUSAApiModel.customerShippingInfo.Address1;
            sendBodyInfo.MKTOrderInfo.CustomerShippingInfo.Address2 = SendToNeweggUSAApiModel.customerShippingInfo.Address2;
            sendBodyInfo.MKTOrderInfo.CustomerShippingInfo.City = SendToNeweggUSAApiModel.customerShippingInfo.City;
            sendBodyInfo.MKTOrderInfo.CustomerShippingInfo.CompanyName = SendToNeweggUSAApiModel.customerShippingInfo.CompanyName;
            sendBodyInfo.MKTOrderInfo.CustomerShippingInfo.ContactWith = SendToNeweggUSAApiModel.customerShippingInfo.ContactWith;
            sendBodyInfo.MKTOrderInfo.CustomerShippingInfo.Country = SendToNeweggUSAApiModel.customerShippingInfo.Country;
            sendBodyInfo.MKTOrderInfo.CustomerShippingInfo.Fax = SendToNeweggUSAApiModel.customerShippingInfo.Fax;
            sendBodyInfo.MKTOrderInfo.CustomerShippingInfo.HomePhone = SendToNeweggUSAApiModel.customerShippingInfo.HomePhone;
            sendBodyInfo.MKTOrderInfo.CustomerShippingInfo.State = SendToNeweggUSAApiModel.customerShippingInfo.State;
            sendBodyInfo.MKTOrderInfo.CustomerShippingInfo.ZipCode = SendToNeweggUSAApiModel.customerShippingInfo.ZipCode;
            sendMessage.Body = sendBodyInfo;

            //訂購商品
            ItemInfo sendItem;

            if (SendToNeweggUSAApiModel.itemNumber.IndexOf("9SI") >= 0)
            {
                //WH08
                sendItem = new ItemInfo();
            }
            else
            {
                //淡水
                sendItem = new ItemInfoExt();
                ItemInfoExt tempItemInfoExt = (ItemInfoExt)sendItem;
                //單價
                tempItemInfoExt.UnitPrice = SendToNeweggUSAApiModel.price;
                //下單 倉庫07
                tempItemInfoExt.WarehouseNumber = SendToNeweggUSAApiModel.WarehouseNumber;
            }

            sendItem.ItemNumber = SendToNeweggUSAApiModel.itemNumber;
            sendItem.DefaultShippingCharge = SendToNeweggUSAApiModel.shippingCharge;
            sendItem.Quantity = SendToNeweggUSAApiModel.quantity;
            //計算總價，商品價格已內涵運費
            sendBodyInfo.MKTOrderInfo.SOAmount = (float)((decimal)SendToNeweggUSAApiModel.price * (decimal)SendToNeweggUSAApiModel.quantity);
            if (SendToNeweggUSAApiModel.price <= 0 || sendBodyInfo.MKTOrderInfo.SOAmount <= 0)
            {
                //throw new Exception("商品價錢異常");
            }

            //放入商品資訊陣列
            ItemInfo[] sendItems = new ItemInfo[1];
            sendItems[0] = sendItem;

            sendBodyInfo.MKTOrderInfo.ItemList = sendItems;

            NodeList nodeList = new NodeList();
            nodeList.Message = sendMessage;

            sendOrderInfo.Node = nodeList;


            string serializeStr = Serialize(sendOrderInfo);

            //紀錄序列化產生的XML
            logger.Info("Write XML Log ItemNumber:" + SendToNeweggUSAApiModel.itemNumber);
            string getNumber = sendBodyInfo.MKTOrderInfo.CustomerPONumber.Substring(6, sendBodyInfo.MKTOrderInfo.CustomerPONumber.Length - 7);
            int writeNumber = 0;
            int.TryParse(getNumber, out writeNumber);
            int writeSn = 0;
            writeSn = writeNumber / 10000;
            string cfolder = "USBO/" + writeSn.ToString();
            System.IO.Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("/log/placeOrderToUsByWs/" + cfolder));
            System.IO.StreamWriter sw = new System.IO.StreamWriter(System.Web.HttpContext.Current.Server.MapPath("/log/placeOrderToUsByWs/" + cfolder + "/placeOrderDetail_" + sendBodyInfo.MKTOrderInfo.CustomerPONumber + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xml"));
            sw.Write(serializeStr);
            sw.Close();

            //送出到美蛋 WebService
            logger.Info("Send XML ItemNumber:" + SendToNeweggUSAApiModel.itemNumber);
            ActionResponse.Msg += "Send XML ItemNumber:" + SendToNeweggUSAApiModel.itemNumber;
            string result = "";
            logger.Info("NeweggUSAServiceUri" + this.Env);
            logger.Info("NeweggUSAServiceUri" + this.NeweggUSAServiceUri);
            WebRequest req = HttpWebRequest.Create(this.NeweggUSAServiceUri);
            req.Timeout = 60 * 1000;
            req.Method = "POST";
            req.ContentType = "text/xml; charset=utf-8";
            req.Headers.Add("SOAPAction", "http://tempuri.org/IMessageProcessor/Process");
            string key = "";
            try
            {
                key = System.Configuration.ConfigurationManager.AppSettings[this.Env + "_NeweggAPICreateSOServiceKey"];
            }
            catch
            {

                ActionResponse.Msg += "無_NeweggAPICreateSOServiceKey ";
                ActionResponse.IsSuccess = false;
                return ActionResponse;
            }
            req.Headers.Add("Authorization", key);
            logger.Info("Header add Authorization: " + key);

            Stream w = req.GetRequestStream();
            StreamWriter swOut = new StreamWriter(w);
            swOut.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><soap:Body><Process xmlns=\"http://tempuri.org/\"><message>" + HttpContext.Current.Server.HtmlEncode(serializeStr) + "</message></Process></soap:Body></soap:Envelope>");
            swOut.Flush();
            Stream r = req.GetResponse().GetResponseStream();
            StreamReader srIn = new StreamReader(r);
            result = srIn.ReadToEnd();
            swOut.Close();
            srIn.Close();
            w.Close();
            r.Close();


            //紀錄回傳的XML
            logger.Info("Write Response XML Log ItemNumber:" + SendToNeweggUSAApiModel.itemNumber);
            ActionResponse.Msg += "Write Response XML Log ItemNumber:" + SendToNeweggUSAApiModel.itemNumber;
            System.IO.StreamWriter sw_output = new System.IO.StreamWriter(System.Web.HttpContext.Current.Server.MapPath("/log/placeOrderToUsByWs/" + cfolder + "/placeOrderDetail_" + sendBodyInfo.MKTOrderInfo.CustomerPONumber + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + "_output.xml"));
            sw_output.Write(result);
            sw_output.Close();

            if (result.IndexOf("&lt;SuccessFlag&gt;true&lt;/SuccessFlag&gt;") >= 0)
            {

                result = result.Substring(result.IndexOf("&lt;SONumber&gt;") + 16);
                result = result.Substring(0, result.IndexOf("&lt;"));
                logger.Info("Success ItemNumber:" + SendToNeweggUSAApiModel.itemNumber + " SellerOrderCode:" + result);
                ActionResponse.Body = result;
                ActionResponse.Msg += "Success ItemNumber:" + SendToNeweggUSAApiModel.itemNumber + " SellerOrderCode:" + result;
                ActionResponse.IsSuccess = true;
                return ActionResponse;
            }
            else
            {
                ActionResponse.Msg += "拋送訂單發生錯誤 ItemNumber:" + SendToNeweggUSAApiModel.itemNumber + " [回傳資訊] " + result;
                logger.Info("拋送訂單發生錯誤 ItemNumber:" + SendToNeweggUSAApiModel.itemNumber + " [回傳資訊] " + result);
                ActionResponse.IsSuccess = false;
                return ActionResponse;
            }
        }
        //舊的API
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



        public TWNewEgg.NewEggService.Models.ProductDetail GetProductDetailV2(string itemNumber)
        {
            int count = 0;
            while (true)
            {
                try
                {
                    string reqURL = NeweggConfiguration.WWW + "/products.egg/{itemNumber}/Detail";
                    reqURL = reqURL.Replace("{itemNumber}", itemNumber);
                    TWNewEgg.NewEggService.Models.ProductDetail result = Get<TWNewEgg.NewEggService.Models.ProductDetail>(reqURL);
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


        public T Get<T>(string url)
        {
            IWebProxy myProxy = GlobalProxySelection.GetEmptyWebProxy();
            GlobalProxySelection.Select = myProxy;
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);

            req.UserAgent = "MobileWebSite";
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
        public string Serialize(object o)
        {
            XmlSerializer ser = new XmlSerializer(o.GetType());
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            ser.Serialize(writer, o);
            writer.Close();
            string result = sb.ToString();
            result = result.Replace(":d1p1", "");
            result = result.Replace("d1p1:", "xsi:");
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("<MKTOrderInfo[\\w :=\\\"]+>");
            string temp = regex.Match(result).Value;
            if (temp.Length > 0)
            {
                result = result.Replace(temp, "<MKTOrderInfo>");
            }
            regex = new System.Text.RegularExpressions.Regex("<ItemInfo[\\w :=\\\"]+>");
            temp = regex.Match(result).Value;
            if (temp.Length > 0)
            {
                result = result.Replace(temp, "<ItemInfo>");
            }
            return result;
        }
    }
}
