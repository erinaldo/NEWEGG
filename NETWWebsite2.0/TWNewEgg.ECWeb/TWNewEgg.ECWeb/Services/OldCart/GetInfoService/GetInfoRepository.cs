using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using TWNewEgg.Website.ECWeb.Models;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;
using TWNewEgg.DB.TWBACKENDDB.Models;
using TWNewEgg.ItemService.Models;
using TWNewEgg.DB;


namespace TWNewEgg.Website.ECWeb.Service
{
    public class GetInfoRepository : IGetInfo, IDisposable
    {
        private TWSqlDBContext db = new TWSqlDBContext();
        private AesCookies AesEnc = new AesCookies();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                db = null;
            }
        }

        public Country GetCountry(int countryID)
        {
            return db.Country.Where(x => x.ID == countryID).FirstOrDefault(); //Get Country's all data from DB.
        }
        public List<Seller> GetSeller()
        {
            return db.Seller.Distinct().ToList(); //Get all seller's Data from DB.
        }

        public string UrlDecode(string encodestring)
        {
            return HttpUtility.UrlDecode(encodestring);
        }
        public string[] Decoder(string fromCookie, bool uriDecode)
        {
            string[] splitString = new string[] { "_" };
            if (uriDecode)
                fromCookie = HttpUtility.UrlDecode(fromCookie);
            fromCookie = AesEnc.AESdecrypt(fromCookie);
            string[] plainText = fromCookie.Split(splitString, StringSplitOptions.RemoveEmptyEntries);
            //for (int i = 0; i < plainText.Length; i++)
            //{
            //    plainText[i] = AesEnc.AESdecrypt(plainText[i]);
            //}
            //string plainText = AesEnc.AESdecrypt(fromCookie); //Decode data that from client.
            return plainText;
        }
        public int CheckAccount(int accID) //check accid has login or not.
        {
            int? checkLogin = db.Account.Where(x => x.ID == accID && x.LoginStatus == 1).Select(y => y.LoginStatus).FirstOrDefault();
            if (checkLogin != null)
            {
                return checkLogin.Value;
            }
            else
            {
                return 0;
            }
        }
        public List<CookieCart> findShippingCart(string shippingCart)
        {
            //postData = "[{\"buyItemID\":\"58\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[]},{\"buyItemID\":\"65\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[]},{\"buyItemID\":\"66\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[{\"buyItemlistID\":\"62\",\"buyingNumber\":\"2\"}]},{\"buyItemID\":\"70\",\"item_AttrID\":\"72\",\"buyingNumber\":\"1\",\"buyItemLists\":[{\"buyItemlistID\":\"74\",\"buyingNumber\":\"2\"},{\"buyItemlistID\":\"75\",\"buyingNumber\":\"6\"}]},{\"buyItemID\":\"71\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[{\"buyItemlistID\":\"76\",\"buyingNumber\":\"6\"}]}]";
            //postData = "[{\"buyItemID\":\"58\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[]},{\"buyItemID\":\"65\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[]},{\"buyItemID\":\"70\",\"item_AttrID\":\"72\",\"buyingNumber\":\"1\",\"buyItemLists\":[{\"buyItemlistID\":\"74\",\"buyingNumber\":\"2\"},{\"buyItemlistID\":\"75\",\"buyingNumber\":\"6\"}]},{\"buyItemID\":\"71\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[{\"buyItemlistID\":\"75\",\"buyingNumber\":\"6\"}]}]";
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<CookieCart> shippingCarts = new List<CookieCart>();
            string newshippingCart = shippingCart.Replace("null", "");
            try
            {
                shippingCarts = serializer.Deserialize<List<CookieCart>>(newshippingCart);
                return shippingCarts;
            }
            catch (Exception e)
            {
                shippingCarts.Add(new CookieCart { itemID = new List<int>() { 0 }, itemlistID = new List<int>() { 0 } });
                return shippingCarts;
            }

        }
        public Dictionary<string, List<GetItemTaxDetail>> GetProductCostByItemID(List<BuyingItems> postData, string isOverSea)
        {
            Dictionary<string, List<GetItemTaxDetail>> productCosts = new Dictionary<string, List<GetItemTaxDetail>>();
            var productsCost = GetProductCostFromDB(postData);
            if (isOverSea == "True")
            {
                List<int> itemIDs = productsCost.Select(x => x.item_id).Distinct().ToList();
                foreach (var aItemID in itemIDs)
                {
                    productCosts.Add(aItemID.ToString(), productsCost.Where(x => x.item_id == aItemID).ToList());
                }
            }
            else
            {
                List<int> itemIDs = productsCost.Select(x => x.item_id).Distinct().ToList();
                var allItems = db.Item.Where(x => itemIDs.Contains(x.ID)).Distinct().ToList();
                var allSellerIDs = allItems.Select(x => x.SellerID).Distinct().ToList();
                foreach (var aSellerID in allSellerIDs)
                {
                    productCosts.Add(aSellerID.ToString(), productsCost.Where(x => allItems.Where(y => y.SellerID == aSellerID).Select(y => y.ID).ToList().Contains(x.item_id)).ToList());
                }

            }

            return productCosts;
        }
        private List<GetItemTaxDetail> GetProductCostFromDB(List<BuyingItems> postData)
        {
            List<GetItemTaxDetail> productsCost = new List<GetItemTaxDetail>();
            List<int> itemIDs = postData.Where(x => x.item_AttrID == null).Select(x => x.buyItemID).ToList();
            List<int> itemAttrIDs = postData.Where(x => x.item_AttrID != null).Select(x => x.item_AttrID.Value).ToList();
            var dbItemID = db.Item.Where(x => itemIDs.Contains(x.ID)).Join(db.Product.DefaultIfEmpty(), x => x.ProductID, y => y.ID, (x, y) => new { items = x, products = y }).ToList();
            var dbItemAttrID = db.ItemList.Where(x => itemAttrIDs.Contains(x.ID)).Join(db.Product.DefaultIfEmpty(), x => x.ItemlistProductID, y => y.ID, (x, y) => new { itemLists = x, products = y }).ToList();
            foreach (var aPostData in postData)
            {

                if (aPostData.item_AttrID != null) //if this item had arrtibute item find temp have or have not itemlist
                {

                }
                else
                {
                    var aDBItemID = dbItemID.Where(x => x.items.ID == aPostData.buyItemID).FirstOrDefault();
                    if (aDBItemID != null)
                    {
                        GetItemTaxDetail newProductCost = new GetItemTaxDetail();
                        newProductCost.item_id = aPostData.buyItemID;
                        newProductCost.itemlist_id = 0;
                        if (aDBItemID.items.DelvType == 0 || aDBItemID.items.DelvType == 2 || aDBItemID.items.DelvType == 4 || aDBItemID.items.DelvType == 6 || aDBItemID.items.DelvType == 7 || aDBItemID.items.DelvType == 8 || aDBItemID.items.DelvType == 9)
                        {
                            newProductCost.pricetaxdetail = "---";
                        }
                        else
                        {
                            newProductCost.pricetaxdetail = UsageCurrency(aDBItemID.items.SellerID) + (aDBItemID.products.Cost ?? 0).ToString();
                        }
                        productsCost.Add(newProductCost);
                    }
                }
                if (aPostData.buyItemLists.Count > 0)
                {
                    List<int> itemListIDs = aPostData.buyItemLists.Where(x => x.item_AttrID == null).Select(x => x.buyItemlistID).ToList();
                    List<int> itemListAttrIDs = aPostData.buyItemLists.Where(x => x.item_AttrID != null).Select(x => x.buyItemlistID).ToList();
                    var dbItemListID = db.ItemList.Where(x => itemListIDs.Contains(x.ID)).Join(db.Product.DefaultIfEmpty(), x => x.ItemlistProductID, y => y.ID, (x, y) => new { itemLists = x, products = y }).ToList();
                    var dbItemListAttrID = db.ItemList.Where(x => itemAttrIDs.Contains(x.ID)).Join(db.Product.DefaultIfEmpty(), x => x.ItemlistProductID, y => y.ID, (x, y) => new { itemLists = x, products = y }).ToList();
                    foreach (var buyingItemList in aPostData.buyItemLists) //find all itemlist in BuyingCarts
                    {
                        if (buyingItemList.item_AttrID != null)
                        {

                        }
                        else
                        {
                            var aDBItemListID = dbItemListID.Where(x => x.itemLists.ID == buyingItemList.buyItemlistID).FirstOrDefault();
                            if (aDBItemListID != null)
                            {
                                GetItemTaxDetail newProductCost = new GetItemTaxDetail();
                                newProductCost.item_id = aPostData.buyItemID;
                                newProductCost.itemlist_id = buyingItemList.buyItemlistID;
                                if (aDBItemListID.products.DelvType == 0 || aDBItemListID.products.DelvType == 4 || aDBItemListID.products.DelvType == 6)
                                {
                                    newProductCost.pricetaxdetail = "---";
                                }
                                else
                                {
                                    newProductCost.pricetaxdetail = UsageCurrency(aDBItemListID.products.SellerID) + (aDBItemListID.products.Cost ?? 0).ToString();
                                }
                                productsCost.Add(newProductCost);
                            }
                        }
                    }
                }
            }


            return productsCost;
        }
        private string UsageCurrency(int sellerID)
        {
            string currency = "";
            int countryID = db.Seller.Where(x => x.ID == sellerID).FirstOrDefault().CountryID ?? 0;
            currency = db.Country.Where(x => x.ID == countryID).FirstOrDefault().UsageCurrency ?? "";
            currency += " ";
            return currency;
        }
        /// <summary>
        /// receve string from user client and transformer to model object
        /// </summary>
        /// <param name="postData"></param>
        /// <returns></returns>
        public List<BuyingItems> GetBuyingCartItems(string postData)
        {
            //postData = "[{\"buyItemID\":\"58\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[]},{\"buyItemID\":\"65\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[]},{\"buyItemID\":\"66\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[{\"buyItemlistID\":\"62\",\"buyingNumber\":\"2\"}]},{\"buyItemID\":\"70\",\"item_AttrID\":\"72\",\"buyingNumber\":\"1\",\"buyItemLists\":[{\"buyItemlistID\":\"74\",\"buyingNumber\":\"2\"},{\"buyItemlistID\":\"75\",\"buyingNumber\":\"6\"}]},{\"buyItemID\":\"71\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[{\"buyItemlistID\":\"76\",\"buyingNumber\":\"6\"}]}]";
            //postData = "[{\"buyItemID\":\"58\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[]},{\"buyItemID\":\"65\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[]},{\"buyItemID\":\"70\",\"item_AttrID\":\"72\",\"buyingNumber\":\"1\",\"buyItemLists\":[{\"buyItemlistID\":\"74\",\"buyingNumber\":\"2\"},{\"buyItemlistID\":\"75\",\"buyingNumber\":\"6\"}]},{\"buyItemID\":\"71\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[{\"buyItemlistID\":\"75\",\"buyingNumber\":\"6\"}]}]";
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<BuyingItems> PostData = new List<BuyingItems>();
            string newPostData = postData.Replace("null", "");
            try
            {
                PostData = serializer.Deserialize<List<BuyingItems>>(newPostData); //string to model object

                return PostData;
            }
            catch (Exception e)
            {
                PostData.Add(new BuyingItems { buyItemID = 0, buyingNumber = 0 });
                return PostData;
            }

        }
        public string ModeltoJSON(List<BuyingItems> postData)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string newPostData = "";
            try
            {
                newPostData = serializer.Serialize(postData);
                return newPostData;
            }
            catch (Exception e)
            {
                return "";
            }

        }
    }
}