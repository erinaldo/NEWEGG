using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TWNewEgg.NeweggUSARequestServices.Models;
using TWNewEgg.NeweggUSARequestServices.Interface;
using log4net;

namespace TWNewEgg.NeweggUSARequestServices.Services
{
    public class NeweggRequest:INeweggRequest
    {
        public Action<string> StatusMsg;

        private string _loginName = "po_netw@newegg.com";
        private string _password = "newegg61705";
        private int _customerNumber = 0;
        private string environment = "";
        private string auth = "";
        private string authToken = "";
        private string owsUrl = "http://www.ows.newegg.com";
        private string getProductDetail = "/Products.egg/{itemNumber}/Detail?ispremier=false";
        private ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        public NeweggRequest()
        {
            auth = "2d84f3680d8db098ea6cdd704ad6bc4b";
        }

        public T Get<T>(string url)
        {
            T data = default(T);
            int retry = 2;
            while (retry > 0)
            {
                try
                {


                   // var proxy = new WebProxy("s1firewall", 8080);
                   // proxy.UseDefaultCredentials = true;


                    var reqclient = new System.Net.Http.HttpClient(new HttpClientHandler() { });
                    reqclient.Timeout = TimeSpan.FromSeconds(30000);

                    reqclient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");


                    reqclient.DefaultRequestHeaders.Add("User-Agent", "Newegg MobileWebSite");

                   
                    if (url.IndexOf("https") >= 0)
                    {
                        reqclient.DefaultRequestHeaders.Add("Authorization", auth);
                        reqclient.DefaultRequestHeaders.Add("X-AuthToken", authToken);
                    }
                    var resault = reqclient.GetByteArrayAsync(url).Result;
               
                    var tempData = System.Text.Encoding.ASCII.GetString(resault);
                    System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                     data = js.Deserialize<T>(tempData);
                }
                catch (Exception e)
                {
                    string ErrorMessage = e.ToString() + "\n\r\n\r\n\r" + (e.InnerException == null ? "" : "--------- InnerException: " + e.InnerException.ToString());
                    logger.Error(ErrorMessage + "____URL:____" + url);
                    StatusMsg(url + e.InnerException.StackTrace);


                }
                retry--;
            }

            return data;
        }

        public T Post<T>(string url, object body)
        {
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            System.Net.WebRequest req = System.Net.HttpWebRequest.Create(url);
            if (url.IndexOf("https") >= 0)
            {
                req.Headers.Add("Authorization", auth);
                req.Headers.Add("X-AuthToken", authToken);
            }
            req.Method = "POST";
            req.Timeout = 1000 * 10;
            System.IO.Stream streamOut = req.GetRequestStream();
            System.IO.StreamWriter sw = new System.IO.StreamWriter(streamOut);
            sw.Write(js.Serialize(body));
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

        public TWNewEgg.NeweggUSARequestServices.Models.ProductDetail GetProductDetail(string itemNumber)
        {
            TWNewEgg.NeweggUSARequestServices.Models.ProductDetail result = new TWNewEgg.NeweggUSARequestServices.Models.ProductDetail();
            //有 TimeOut 的版本
            //HostWWW/products.egg/{itemNumber}/ProductDetails
            string reqURL = owsUrl + getProductDetail;
            reqURL = reqURL.Replace("{itemNumber}", itemNumber);
            Models.ProductDetails productDetailData = Get<Models.ProductDetails>(reqURL);
            if (productDetailData != null && productDetailData.Basic != null)
            {
                if (productDetailData.Basic.imageGallery == null && productDetailData.Basic.ItemImages != null)
                {
                    productDetailData.Basic.imageGallery = productDetailData.Basic.ItemImages;
                }
                else if (productDetailData.Basic.imageGallery != null && productDetailData.Basic.ItemImages == null)
                {
                    productDetailData.Basic.ItemImages = productDetailData.Basic.imageGallery;
                }
                result = productDetailData.Basic;
            }
            return result;
        }

        public Dictionary<string, Models.Pricing.ItemInfo> GetPrice(List<string> itemNumbers)
        {
            Dictionary<string, Models.Pricing.ItemInfo> result = new Dictionary<string, Models.Pricing.ItemInfo>();
            string debugInfo = "";
            try
            {
                //http://www.newegg.com/Product/MappingPrice.aspx?Item=
                //http://www.newegg.com/Product/MappingPrice2012.aspx?Item=
                //http://apis.newegg.org/pricing/v1/item/listinginfo?ItemNumbers=
                string url = "http://apis.newegg.org/pricing/v1/item/listinginfo?format=json&ItemNumbers=";

                int max = 15;
                int currentIndex = 0;
                while (currentIndex < itemNumbers.Count)
                {
                    string items = "";
                    int count = 0;
                    while (count < max && currentIndex < itemNumbers.Count)
                    {
                        items += itemNumbers[currentIndex] + ",";
                        count++;
                        currentIndex++;
                    }
                    if (items[items.Length - 1] == ',')
                    {
                        //remove unnecessary comma
                        items = items.Substring(0, items.Length - 1);
                    }

                    debugInfo = url + items;
                    string str="";
                  //    Parallel.For(1,1, new ParallelOptions() { MaxDegreeOfParallelism = 1}, t =>
                  //{
                    //var proxy = new WebProxy("s1firewall", 8080);
                    //proxy.UseDefaultCredentials = true;
                    var req = new System.Net.Http.HttpClient(new HttpClientHandler() {});
                    //req.BaseAddress = new Uri(url + items);
                    req.Timeout = TimeSpan.FromSeconds(100);
                    req.DefaultRequestHeaders.Add("Accept", "application/json;q=0.9,*/*;q=0.8");
                    req.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (XXXXXXX NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.116 Safari/537.36");
                      
                    
                       //req.GetByteArrayAsync(url + items).ContinueWith(be =>
                       //{
                           var resault = req.GetByteArrayAsync(url + items).Result;
                           string tempData = System.Text.Encoding.ASCII.GetString(resault);
                     
                         
                       //}).Wait();
                   //});
                    //System.Net.HttpWebRequest req = System.Net.HttpWebRequest.CreateHttp(url + items);
                    //req.KeepAlive = false;
                    //req.Method = "GET";
                    //req.Timeout = 30000;
                    //req.Accept = "application/json;q=0.9,*/*;q=0.8";
                    //req.Headers.Add("Accept-Language", "en-US,en;q=0.8");
                    //req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.116 Safari/537.36";
               
                    System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                    Models.Pricing.Response response = jss.Deserialize<Models.Pricing.Response>(tempData);
                    if (response != null)
                    {
                        foreach (var itemnumber in itemNumbers)
                        {
                            if (!result.ContainsKey(itemnumber))
                            {
                                result.Add(itemnumber, null);
                            }
                            var item = response.Items.Where(x => x.ItemNumber == itemnumber).FirstOrDefault();
                            if (item != null)
                            {
                                result[itemnumber] = item;
                            }
                        }
                    }
                    else
                    {
                        result = null;
                    }
                }
            }
            catch (Exception e)
            {
                string ErrorMessage = e.ToString() + "\n\r\n\r\n\r" + (e.InnerException == null ? "" : "--------- InnerException: " + e.InnerException.ToString());
                logger.Error(ErrorMessage + "____URL:____" + debugInfo);
                //throw new Exception(ErrorMessage);
            }
            return result;
        }
    }
}
