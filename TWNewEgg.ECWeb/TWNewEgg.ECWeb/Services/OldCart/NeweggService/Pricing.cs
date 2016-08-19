using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Website.ECWeb.Service
{
    public class Pricing : IProductInfoProvider
    {
        public Dictionary<string, bool> GetStatus(List<string> sellerProductIdList)
        {
            Dictionary<string, bool> result = new Dictionary<string, bool>();

            Models.PricingResponse info = new Models.PricingResponse();
            info.Items = new List<Models.PricingItemInfo>();
            int max = 15;
            int currentIndex = 0;
            while (currentIndex < sellerProductIdList.Count)
            {
                int count = 0;
                List<string> itemnumbers = new List<string>();
                while (count < max && currentIndex < sellerProductIdList.Count)
                {
                    itemnumbers.Add(sellerProductIdList[currentIndex]);
                    count++;
                    currentIndex++;
                }
                info.Items.AddRange(GetPricingInfo(itemnumbers).Items);
            }

            foreach (var itemnumber in sellerProductIdList)
            {
                try
                {
                    result.Add(itemnumber, false);
                    Models.PricingItemInfo data = info.Items.Where(x => x.ItemNumber == itemnumber).FirstOrDefault();
                    result[itemnumber] = data.Available;
                }
                catch { }
            }
            return result;
        }

        public Dictionary<string, int> GetStock(List<string> sellerProductIdList, string warehouseNumber)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();

            Models.PricingResponse info = new Models.PricingResponse();
            info.Items = new List<Models.PricingItemInfo>();
            int max = 15;
            int currentIndex = 0;
            while (currentIndex < sellerProductIdList.Count)
            {
                int count = 0;
                List<string> itemnumbers = new List<string>();
                while (count < max && currentIndex < sellerProductIdList.Count)
                {
                    itemnumbers.Add(sellerProductIdList[currentIndex]);
                    count++;
                    currentIndex++;
                }
                info.Items.AddRange(GetPricingInfo(itemnumbers).Items);
            }

            foreach (var itemnumber in sellerProductIdList)
            {
                try
                {
                    result.Add(itemnumber, 0);
                    Models.PricingItemInfo data = info.Items.Where(x => x.ItemNumber == itemnumber).FirstOrDefault();
                    int qty = 0;
                    try
                    {
                        qty = data.Warehouses.Where(x => x.WHNo == warehouseNumber).FirstOrDefault().Quantity;
                    }
                    catch { }
                    if (itemnumber.ToUpper().IndexOf("9SI") >= 0)
                    {
                        int vf = 0;
                        try
                        {
                            vf = data.VFInventory;
                        }
                        catch { }
                        qty += vf;
                    }

                    result[itemnumber] = qty;
                }
                catch { }
            }
            return result;
        }

        public Dictionary<string, decimal> GetPrice(List<string> sellerProductIdList)
        {
            Dictionary<string, decimal> result = new Dictionary<string, decimal>();
            try
            {
                int max = 15;
                int currentIndex = 0;
                while (currentIndex < sellerProductIdList.Count)
                {
                    int count = 0;
                    List<string> itemnumbers = new List<string>();
                    while (count < max && currentIndex < sellerProductIdList.Count)
                    {
                        itemnumbers.Add(sellerProductIdList[currentIndex]);
                        count++;
                        currentIndex++;
                    }

                    Models.PricingResponse response = GetPricingInfo(itemnumbers);
                    if (response != null)
                    {
                        foreach (var itemnumber in sellerProductIdList)
                        {
                            try
                            {
                                result.Add(itemnumber, 0);
                                var item = response.Items.Where(x => x.ItemNumber == itemnumber).FirstOrDefault();
                                if (item.Available)
                                {
                                    result[itemnumber] = item.FinalPrice;
                                }
                            }
                            catch { }
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
                //throw new Exception(e.Message);
            }
            return result;
        }

        public Dictionary<string, decimal> GetPriceWithShippingCharge(List<string> sellerProductIdList)
        {
            Dictionary<string, decimal> result = new Dictionary<string, decimal>();
            try
            {
                int max = 15;
                int currentIndex = 0;
                while (currentIndex < sellerProductIdList.Count)
                {
                    int count = 0;
                    List<string> itemnumbers = new List<string>();
                    while (count < max && currentIndex < sellerProductIdList.Count)
                    {
                        itemnumbers.Add(sellerProductIdList[currentIndex]);
                        count++;
                        currentIndex++;
                    }

                    Models.PricingResponse response = GetPricingInfo(itemnumbers);
                    if (response != null)
                    {
                        foreach (var itemnumber in sellerProductIdList)
                        {
                            try
                            {
                                result.Add(itemnumber, 0);
                                var item = response.Items.Where(x => x.ItemNumber == itemnumber).FirstOrDefault();
                                if (item.Available)
                                {
                                    result[itemnumber] = item.FinalPrice;
                                    //依據美蛋的資料規則，運費大於0.01才為有效運費
                                    if (item.ShippingCharge > 0.01m)
                                    {
                                        result[itemnumber] = result[itemnumber] + item.ShippingCharge;
                                    }
                                }
                            }
                            catch { }
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
                //throw new Exception(e.Message);
            }
            return result;
        }

        public Models.PricingResponse GetPricingInfo(List<string> itemNumbers)
        {
            Models.PricingResponse result = null;
            try
            {
                string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"].ToUpper();
                string _url = System.Configuration.ConfigurationManager.AppSettings[environment + "_PricingInfo"];
                string items = string.Join(",", itemNumbers);
                _url = _url + items;
                System.Net.HttpWebRequest req = System.Net.HttpWebRequest.CreateHttp(_url);
                req.Method = "GET";
                req.Timeout = 1000 * 180;
                req.Accept = "application/json;q=0.9,*/*;q=0.8";
                req.Headers.Add("Accept-Language", "en-US,en;q=0.8");
                req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.116 Safari/537.36";
                System.IO.Stream s = req.GetResponse().GetResponseStream();
                System.IO.StreamReader sr = new System.IO.StreamReader(s);
                string str = sr.ReadToEnd();
                sr.Close();
                s.Close();
                System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                result = jss.Deserialize<Models.PricingResponse>(str);
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }
    }
}