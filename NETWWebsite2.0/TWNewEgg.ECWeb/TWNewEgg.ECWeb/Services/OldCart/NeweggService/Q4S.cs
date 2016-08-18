using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace TWNewEgg.Website.ECWeb.Service
{
    public class Q4S : IProductInfoProvider
    {
        //正式 http://apis.newegg.org/Q4SService/Q4sService.svc/QueryRawInventoryByItems?bizunit=USA
        //GQC http://10.1.24.151:89/Q4sService.svc/QueryRawInventoryByItems?bizunit=USA
        //測試 http://10.16.50.86:89/Q4sService.svc/QueryRawInventoryByItems?bizunit=USA
        public Models.Q4S.QueryRawInventoryResponse rawInventoryResponse;
        public string serviceUri;
        private string appkey;
        private string token;
        public Q4S()
        {
            //API路徑、Key、Toke設定
            string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
            this.serviceUri = System.Configuration.ConfigurationManager.AppSettings[environment + "_NeweggAPIQ4S"];
            this.appkey = "";
            this.token = "";
            try
            {
                this.appkey = System.Configuration.ConfigurationManager.AppSettings[environment + "_NeweggAPIAppKey"];
                this.token = System.Configuration.ConfigurationManager.AppSettings[environment + "_NeweggAPIToken"];
            }
            catch { }
        }

        public Dictionary<string, bool> GetStatus(List<string> sellerProductIdList)
        {
            //Q4S API doesn't has price infomation
            //get price infomation from [Pricing]
            Service.Pricing pricing = new Pricing();
            return pricing.GetStatus(sellerProductIdList);
        }

        public Dictionary<string, decimal> GetPrice(List<string> sellerProductIdList)
        {
            //Q4S API doesn't has price infomation
            //get price infomation from [Pricing]
            Service.Pricing pricing = new Pricing();
            return pricing.GetPrice(sellerProductIdList);
        }

        public Dictionary<string, decimal> GetPriceWithShippingCharge(List<string> sellerProductIdList)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, int> GetStock(List<string> sellerProductIdList, string warehouseNumber)
        {
            return GetWarehouseQuantity(sellerProductIdList.ToArray(), warehouseNumber);
        }

        /// <summary>
        /// 回傳指定倉庫內 ItemNumber 對應的庫存數量
        /// Dictionary (Key : string ItemNumber) (Value : int qty)
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
            Models.Q4S.QueryRawInventoryResponse res = Send<Models.Q4S.QueryRawInventoryResponse>(req);
            return res;
        }

        private T Send<T>(Models.Q4S.QueryRawInventoryRequest req)
        {
            return Send<T>((object)req);
        }

        private T Send<T>(object requestBody)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            string requestJSON = jss.Serialize(requestBody);
            System.Diagnostics.Debug.Print("Q4S Send() " + this.serviceUri);
            System.Net.WebRequest req = System.Net.WebRequest.Create(this.serviceUri);
            req.Timeout = 5000;
            req.Method = "POST";
            req.ContentType = "application/json";
            req.Headers.Add("X-QueryDB", "NEWSQL");
            req.Headers.Add("X-Sender", "NeweggWebsite");
            req.Headers.Add("Authorization", this.appkey + "&" + this.token);
            using (System.IO.Stream output = req.GetRequestStream())
            {
                byte[] outputData = System.Text.Encoding.ASCII.GetBytes(requestJSON);
                output.Write(outputData, 0, outputData.Length);
                output.Flush();
                output.Close();
            }
            List<byte> data = new List<byte>();
            using (System.IO.Stream stream = req.GetResponse().GetResponseStream())
            {
                byte[] buffer = new byte[512];
                int count = 0;
                while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    byte[] currentData = new byte[count];
                    for (int i = 0; i < count; i++)
                    {
                        currentData[i] = buffer[i];
                    }
                    data.AddRange(currentData);
                }
                stream.Close();
            }
            string temp = System.Text.Encoding.ASCII.GetString(data.ToArray());
            T tt = jss.Deserialize<T>(temp);
            return tt;
        }
    }
}