using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Script.Serialization;
using log4net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;
namespace TWNewEgg.NeweggUSARequestServices.Services
{
    public class Q4S
    {
        //正式 http://apis.newegg.org/Q4SService/Q4sService.svc/QueryRawInventoryByItems?bizunit=USA
        //GQC http://10.1.24.151:89/Q4sService.svc/QueryRawInventoryByItems?bizunit=USA
        //測試 http://10.16.50.86:89/Q4sService.svc/QueryRawInventoryByItems?bizunit=USA
        public Models.Q4S.QueryRawInventoryResponse rawInventoryResponse;
        public string serviceUri;
        private string serviceAppKey;
        private string serviceToken;
        private ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        public Q4S()
        {
            //API路徑、Key、Toke設定
            string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
            this.serviceUri = System.Configuration.ConfigurationManager.AppSettings[environment + "_" + "NeweggAPIQ4S"];
            this.serviceAppKey = "";
            this.serviceToken = "";
            try
            {
                this.serviceAppKey = System.Configuration.ConfigurationManager.AppSettings[environment + "_" + "NeweggAPIAppKey"];
                this.serviceToken = System.Configuration.ConfigurationManager.AppSettings[environment + "_" + "NeweggAPIToken"];
            }
            catch { }
        }

        /// <summary>
        /// 回傳指定倉庫內 ItemNumber 對應的庫存數量
        /// Dictionary (Key : string ItemNumber) (Value : int Qty)
        /// </summary>
        /// <param name="WarehouseNumber"></param>
        /// <returns></returns>
        public Dictionary<string, int> GetWarehouseQuantity(string[] itemNumbers, string WarehouseNumber)
        {
            this.rawInventoryResponse = QueryRawInventory(itemNumbers);

            if (this.rawInventoryResponse == null)
            {
                throw new Exception("Error");
            }
            if (this.rawInventoryResponse.ItemInventoryList == null)
            {
                throw new Exception("ItemInventoryList == null");
            }

            Dictionary<string, int> result = new Dictionary<string, int>();
            for (int i = 0; i < itemNumbers.Length; i++)
            {
                try
                {
                    result.Add(itemNumbers[i], 0);
                }
                catch
                {
                    //catch if itemNumber repeat
                }
            }
            for (int i = 0; i < this.rawInventoryResponse.ItemInventoryList.Count; i++)
            {
                string itemNumber = "";
                try
                {
                    itemNumber = this.rawInventoryResponse.ItemInventoryList[i].ItemNumber.ToUpper();
                }
                catch { }
                int qty = 0;
                try
                {
                    if (itemNumber.IndexOf("9SI") >= 0)
                    {
                        try
                        {
                            qty = this.rawInventoryResponse.ItemInventoryList[i].WarehouseList.Where(x => x.WarehouseNumber == WarehouseNumber).Select(x => x.Quantity).First();
                        }
                        catch
                        {
                            qty = 0;
                        }
                        if (qty <= 0)
                        {
                            try
                            {
                                qty = this.rawInventoryResponse.ItemInventoryList[i].VF_Avail;
                            }
                            catch
                            {
                                qty = 0;
                            }
                        }
                    }
                    else
                    {
                        qty = this.rawInventoryResponse.ItemInventoryList[i].WarehouseList.Where(x => x.WarehouseNumber == WarehouseNumber).Select(x => x.Quantity).First();
                    }
                }
                catch { }
                result[itemNumber] = qty;
            }

            return result;
        }

        public Models.Q4S.QueryRawInventoryResponse QueryRawInventory(string[] itemNumbers)
        {
            Models.Q4S.QueryRawInventoryRequest req = new Models.Q4S.QueryRawInventoryRequest();
            req.Items = new List<Models.Q4S.Item>();
            for (int i = 0; i < itemNumbers.Length; i++)
            {
                Models.Q4S.Item item = new Models.Q4S.Item();
                item.ItemNumber = itemNumbers[i];
                req.Items.Add(item);
            }
             // Models.Q4S.QueryRawInventoryResponse res = GetStock(req);

          Models.Q4S.QueryRawInventoryResponse res = Send<Models.Q4S.QueryRawInventoryResponse>(req);


            return res;
        }
       
        private T Send<T>(Models.Q4S.QueryRawInventoryRequest req)
        {
            return Send<T>((object)req);
        }

        public T Send<T>(object requestBody)
        {
            string requestJSON = "";
     
            string res = "";
            try
            {

          
                    JavaScriptSerializer jss = new JavaScriptSerializer();
                    string requestJSONdetail = JsonConvert.SerializeObject(requestBody);
             
                    //HttpContent content = new 
                 
                    HttpClient httpClient = new HttpClient(new HttpClientHandler() { });

                    var request = new StringContent(requestJSONdetail, System.Text.Encoding.UTF8);
                        request.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                        httpClient.DefaultRequestHeaders.Add("X-QueryDB", "NEWSQL");
                        httpClient.DefaultRequestHeaders.Add("X-Sender", "NeweggWebsite");
                        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", this.serviceAppKey + "&" + this.serviceToken);


                        var response = httpClient.PostAsync(new Uri(this.serviceUri), request).Result;

                  

              
                  //var response =PostJsonAsync(this.serviceUri, requestJSONdetail).Result;
                   T tt = jss.Deserialize<T>(response.Content.ReadAsStringAsync().Result);
      
                    //var response = PostJsonAsync(this.serviceUri, requestJSON);
                 
       


               
                return tt;
            }
            catch (Exception e)
            {
                string ErrorMessage = e.ToString() + "\n\r\n\r\n\r" + (e.InnerException == null ? "" : "--------- InnerException: " + e.InnerException.ToString());
                logger.Error(ErrorMessage + "___JSON:___" + requestJSON);
                //throw new Exception(ErrorMessage);
                throw new Exception(ErrorMessage + "___JSON:___" + requestJSON);
            }
        }

        public class person
        {
            public string name { get; set; }
            public string surname { get; set; }
        } 
      

      

    }
}