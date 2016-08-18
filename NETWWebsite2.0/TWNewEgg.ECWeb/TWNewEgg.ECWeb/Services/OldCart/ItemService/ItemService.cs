using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWBACKENDDB.Models;
using TWNewEgg.ECWeb.Auth;
using TWNewEgg.Framework.ServiceApi;

namespace TWNewEgg.Website.ECWeb.Service
{
    public class ItemService
    {
        Newegg.Mobile.MvcApplication.Models.UIProductItemDetailAllInfo info;
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        // 系統更改商品狀態時(上下架、售價、庫存)，UpdateUser填入值
        string newEggItemStatusCheck = "System_NewEggItemStatusCheck";
        //ProductDetailResult productdetailinfo;
        public ItemService()
        {
            info = null;
            //productdetailinfo = null;
        }

        /// <summary>
        /// 檢查商品於美蛋賣場的狀態是否正常，同時更新資料庫Item的狀態。
        /// 賣場狀態正常回傳True，反之回傳False
        /// </summary>
        /// <param name="productID">商品編號</param>
        /// <returns></returns>
        public bool CheckItemStatusNeweggUSA(int productID)
        {
            string userEmail = NEUser.Email;
            bool result = false;
            logger.Info("[UserEmail] " + userEmail + " [CheckItemStatusNeweggUSA] start");
            string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
            string turnON = System.Configuration.ConfigurationManager.AppSettings[environment + "_CheckItemStatusNeweggUSA"];
            TWNewEgg.DB.TWSqlDBContext db_before = new DB.TWSqlDBContext();
            turnON = turnON.ToLower();
            if (turnON != "on")
            {
                return true;
            }
            try
            {
                Product product = db_before.Product.Where(x => x.ID == productID).FirstOrDefault();
                if (product == null)
                {
                    throw new Exception("ProductID: " + productID.ToString() + " Not Exist");
                }
                else
                {
                    Service.QueryPriceStock qps = new QueryPriceStock(new QueryPriceStockFactory(QueryPriceStockFactory.Provider.APIsNewegg));
                    List<string> temp = new List<string>();
                    temp.Add(product.SellerProductID);
                    result = qps.GetStatus(temp)[product.SellerProductID];
                    logger.Info("[UserEmail] " + userEmail + " [CheckItemStatusNeweggUSA] GetStatus [" + result + "]");
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
            logger.Info("[UserEmail] " + userEmail + " [CheckItemStatusNeweggUSA] end");
            return result;
        }

        /*public bool CheckItemStatusNeweggUSA(int productID)
        {
            string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
            string turnON = System.Configuration.ConfigurationManager.AppSettings[environment + "_CheckItemStatusNeweggUSA"];
            turnON = turnON.ToLower();
            if (turnON != "on")
            {
                return true;
            }
            log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            try
            {
                Models.DatabaseContext db = new Models.DatabaseContext();
                Product product = db.Product.Where(x => x.ID == productID).FirstOrDefault();
                if (product == null)
                {
                    throw new Exception("ProductID: " + productID.ToString() + " Not Exist");
                }
                else
                {
                    if (product.SourceTable.ToLower() != "productfromws")
                    {
                        //非美蛋商品，直接回傳True
                        return true;
                    }
                    if (this.info == null)
                    {
                        //嘗試取得商品資訊
                        Service.NeweggRequest nr = new NeweggRequest();
                        this.info = nr.GetProductDetail(product.SellerProductID);
                    }
                    if (info != null)
                    {
                        //賣場狀態正常
                        List<Item> items = db.Item.Where(x => x.ProductID == productID).ToList();
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (items[i].Status == (int)Item.status.未上架)
                            {
                                items[i].Status = (int)Item.status.已上架;
                                items[i].UpdateDate = DateTime.UtcNow.AddHours(8);
                                db.SaveChanges();
                                //刪除快取
                                Service.ItemService.refreshCache(items[i].ID, null, null, true, "12345qwertASDFGzxcvb");
                                logger.Info("ItemNumber: " + product.SellerProductID + " 美蛋賣場正常，開啟賣場【" + items[i].ID.ToString() + "】");
                            }
                        }
                        return true;
                    }
                    else
                    {
                        //因美蛋賣場狀態異常，關閉賣場
                        List<Item> items = db.Item.Where(x => x.ProductID == productID).ToList();
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (items[i].Status == (int)Item.status.已上架)
                            {
                                items[i].Status = (int)Item.status.未上架;
                                items[i].UpdateDate = DateTime.UtcNow.AddHours(8);
                                db.SaveChanges();
                                //刪除快取
                                Service.ItemService.refreshCache(items[i].ID, null, null, true, "12345qwertASDFGzxcvb");
                                logger.Info("ItemNumber: " + product.SellerProductID + " 美蛋賣場異常，關閉賣場【" + items[i].ID.ToString() + "】");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return false;
        }*/

        /// <summary>
        /// 檢查美蛋賣場線上銷售價格是否與資料庫相符，若售價有異動則會更新資料庫Product Cost以及重新計算Item Price。
        /// 線上銷售價格與資料庫相符回傳True，反之回傳False
        /// </summary>
        public bool CheckItemPriceNeweggUSA(int productID)
        {
            string userEmail = NEUser.Email;
            logger.Info("[UserEmail] " + userEmail + " [CheckItemPriceNeweggUSA] start");
            string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
            string turnON = System.Configuration.ConfigurationManager.AppSettings[environment + "_CheckItemPriceNeweggUSA"];
            turnON = turnON.ToLower();
            TWNewEgg.DB.TWSqlDBContext db_before = new DB.TWSqlDBContext();
            if (turnON != "on")
            {
                return true;
            }
            bool isPriceConsistentWithDB = true;
            try
            {
                Product product = db_before.Product.Where(x => x.ID == productID).FirstOrDefault();
                List<int> changePriceItemIDList = new List<int>();
                if (product == null)
                {
                    throw new Exception("ProductID: " + productID.ToString() + " Not Exist");
                }
                else
                {
                    if (product.SourceTable.ToLower() != "productfromws")
                    {
                        //非美蛋商品
                        throw new Exception("ProductID:" + product.ID + " Not From NeweggUSA");
                    }
                    if (product.SellerProductID == null || product.SellerProductID == "")
                    {
                        //商品編號異常
                        throw new Exception("ProductID:" + product.ID + " sellerproductid is NULL or Empty String");
                    }
                    if (this.info == null)
                    {
                        //嘗試取得商品資訊
                        Service.NeweggRequest nr = new NeweggRequest();
                        this.info = nr.GetProductDetail(product.SellerProductID);
                    }
                    if (info != null)
                    {
                        info.FinalPrice = info.FinalPrice.Replace("$", "");
                        info.FinalPrice = info.FinalPrice.Replace(",", "");
                        decimal price = 0;
                        decimal.TryParse(info.FinalPrice, out price);

                        //暫時先在這裡改變價格取得方式，下次程式碼重構時需將賣場狀態&查價動作
                        //統一為使用 NeweggRequest.GetPrice
                        //Price >0 即代表 賣場狀態OK 同時取得價格
                        try
                        {
                            Service.QueryPriceStockFactory factory = new Service.QueryPriceStockFactory(Service.QueryPriceStockFactory.Provider.APIsNewegg);
                            Service.IProductInfoProvider pricing = factory.CreateProvider();
                            price = pricing.GetPriceWithShippingCharge(new List<string> { product.SellerProductID })[product.SellerProductID];
                        }
                        catch { }

                        if (price <= 0)
                        {
                            string url = System.Configuration.ConfigurationManager.AppSettings["ECWeb"];
                            //售價異常，關閉賣場
                            //所有販賣這個商品的賣場
                            List<Item> items = db_before.Item.Where(x => x.ProductID == product.ID).ToList();
                            //所有使用這個商品做為屬性
                            List<ItemList> itemlists = db_before.ItemList.Where(x => x.ItemlistProductID == product.ID).ToList();

                            for (int i = 0; i < items.Count; i++)
                            {
                                if (items[i].Status == (int)Item.status.已上架)
                                {
                                    logger.Error("ItemID:" + items[i].ToString() + " ProductID:" + productID.ToString() + " 售價為0，關閉賣場");
                                    //關閉賣場
                                    items[i].Status = (int)Item.status.未上架;
                                    items[i].UpdateDate = DateTime.UtcNow.AddHours(8);
                                    items[i].UpdateUser = newEggItemStatusCheck;
                                    //刪除快取
                                    Service.ItemService.refreshCache(items[i].ID, null, null, true, "12345qwertASDFGzxcvb");
                                }
                            }

                            for (int i = 0; i < itemlists.Count; i++)
                            {
                                if (itemlists[i].Status == (int)Item.status.已上架)
                                {
                                    logger.Error("ItemListID:" + itemlists[i].ToString() + " ProductID:" + productID.ToString() + " 售價為0，關閉屬性");
                                    //關閉屬性
                                    itemlists[i].Status = (int)ItemList.status.未上架;
                                    itemlists[i].UpdateDate = DateTime.UtcNow.AddHours(8);
                                    itemlists[i].UpdateUser = newEggItemStatusCheck;
                                    //刪除快取
                                    Service.ItemService.refreshCache(items[i].ID, null, null, true, "12345qwertASDFGzxcvb");
                                }
                            }
                            //賣場關閉flag設定為False
                            isPriceConsistentWithDB = false;
                        }

                        if (price != product.Cost)
                        {
                            //售價異動flag設定為False
                            isPriceConsistentWithDB = false;
                            //售價異動，更新售價
                            product.Cost = price;
                            //所有販賣這個商品的賣場
                            List<Item> items = db_before.Item.Where(x => x.ProductID == product.ID).ToList();
                            changePriceItemIDList.AddRange(items.Select(x => x.ID).ToList());
                            //所有使用這個商品做為屬性的賣場
                            List<ItemList> itemlists = db_before.ItemList.Where(x => x.ItemlistProductID == product.ID).ToList();
                            //搜尋匯率
                            Seller seller = db_before.Seller.Where(x => x.ID == product.SellerID).FirstOrDefault();
                            if (seller != null)
                            {
                                string year = DateTime.Now.Year.ToString();
                                string month = DateTime.Now.Month.ToString();
                                Currency currency = db_before.Currency.Where(x => x.Type == seller.CurrencyType && x.Year == year && x.Month == month).FirstOrDefault();
                                if (currency != null)
                                {
                                    //使用當期匯率重新計算售價
                                    decimal nt = price * currency.AverageexchangeRate;
                                    nt = Math.Ceiling(nt);
                                    for (int i = 0; i < items.Count; i++)
                                    {
                                        //新售價
                                        items[i].PriceCash = nt;
                                        items[i].UpdateDate = DateTime.UtcNow.AddHours(8);
                                        items[i].UpdateUser = newEggItemStatusCheck;
                                        //刪除快取
                                        Service.ItemService.refreshCache(items[i].ID, null, null, true, "12345qwertASDFGzxcvb");
                                    }
                                    for (int i = 0; i < itemlists.Count; i++)
                                    {
                                        //新售價
                                        itemlists[i].Price = nt;
                                        itemlists[i].UpdateDate = DateTime.UtcNow.AddHours(8);
                                        itemlists[i].UpdateUser = newEggItemStatusCheck;
                                        //刪除快取
                                        Service.ItemService.refreshCache(itemlists[i].ItemID, null, null, true, "12345qwertASDFGzxcvb");
                                    }
                                }
                                else
                                {
                                    //找不到當期匯率
                                    logger.Fatal("Currency Not Exist :" + year + "/" + month);
                                }
                            }
                            else
                            {
                                //找不到 seller
                                logger.Fatal("Seller Not Exist => ProductID:" + product.ID.ToString() + " SellerID:" + product.SellerID.ToString());
                            }
                        }
                    }
                    else
                    {
                        //因美蛋賣場狀態異常，查詢不到售價
                        throw new Exception("因美蛋賣場狀態異常，查詢不到售價 ItemNumber: " + product.SellerProductID);
                    }
                }

                db_before.SaveChanges();
                if (changePriceItemIDList.Count > 0)
                {
                    // 因變更售價故執行總價化流程
                    var getDisplayItemResult = Processor.Request<string, string>("ItemDisplayPriceService", "SetItemDisplayPriceByIDs", changePriceItemIDList);
                    if (getDisplayItemResult.error != null)
                    {
                        throw new Exception(getDisplayItemResult.error);
                    }
                }
            }
            catch (Exception e)
            {
                //售價取得失敗則flag設定為False
                isPriceConsistentWithDB = false;
                logger.Error(e.Message);
            }
            logger.Info("[UserEmail] " + userEmail + " [CheckItemPriceNeweggUSA] end");
            return isPriceConsistentWithDB;
        }

        /// <summary>
        /// 檢查商品於借賣網賣場的狀態是否正常，同時更新資料庫Item的狀態。
        /// 賣場狀態正常回傳True，反之回傳False
        /// </summary>
        /// <param name="productID">商品編號</param>
        /// <returns></returns>
        //public bool CheckItemStatusJieMai(int productID)
        //{
        //    string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
        //    string JieMaiServiceUri = System.Configuration.ConfigurationManager.AppSettings[environment + "_JieMaiAPICreateSOService"];
        //    string JieMaiQueryServiceUri = System.Configuration.ConfigurationManager.AppSettings[environment + "_JieMaiAPIQueryService"];
        //    string JieMaiServiceKey = System.Configuration.ConfigurationManager.AppSettings[environment + "_JieMaiAPICreateSOServiceKey"];
        //    JieMai.Service.JieMaiClass JieMaiServiceAPI = new JieMai.Service.JieMaiClass(JieMaiServiceUri, JieMaiQueryServiceUri, JieMaiServiceKey);
        //    TWNewEgg.DB.TWSqlDBContext db_before = new DB.TWSqlDBContext();
        //    List<string> SKUs = new List<string>();
        //    JieMaiService JMService = new JieMaiService();
        //    string turnON = System.Configuration.ConfigurationManager.AppSettings[environment + "_CheckItemStatusNeweggUSA"];
        //    turnON = turnON.ToLower();
        //    if (turnON != "on")
        //    {
        //        return true;
        //    }
        //    log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        //    try
        //    {
        //        Product product = db_before.Product.Where(x => x.ID == productID).FirstOrDefault();
        //        if (product == null)
        //        {
        //            throw new Exception("ProductID: " + productID.ToString() + " Not Exist");
        //        }
        //        else
        //        {
        //            if (product.SourceTable.ToLower() != "productfromjiemai")
        //            {
        //                //非借賣網商品，直接回傳True
        //                return true;
        //            }
        //            //if (this.productdetailinfo == null)
        //            //{
        //            //    //嘗試取得商品資訊
        //            //    SKUs.Add(product.SellerProductID);
        //            //    this.productdetailinfo = JieMaiServiceAPI.GetProductDetail(SKUs);
        //            //}
        //            //if (productdetailinfo != null)
        //            if (!JMService.checkproductonline(productID)) // 檢查借賣網該商品是否上架中，回傳false表示商品有在架上，True為商品不在架上
        //            {
        //                //賣場狀態正常
        //                //List<Item> items = db.Item.Where(x => x.ProductID == productID).ToList();
        //                //for (int i = 0; i < items.Count; i++)
        //                //{
        //                //    if (items[i].Status == (int)Item.status.未上架)
        //                //    {
        //                //        items[i].Status = (int)Item.status.已上架;
        //                //        items[i].UpdateDate = DateTime.UtcNow.AddHours(8);
        //                //        db.SaveChanges();
        //                //        //刪除快取
        //                //        Service.ItemService.refreshCache(items[i].ID, null, null, true, "12345qwertASDFGzxcvb");
        //                //        logger.Info("ItemNumber: " + product.SellerProductID + " 借賣網賣場正常，開啟賣場【" + items[i].ID.ToString() + "】");
        //                //    }
        //                //}
        //                return true;
        //            }
        //            else
        //            {
        //                //因借賣網賣場狀態異常，關閉賣場
        //                List<Item> items = db_before.Item.Where(x => x.ProductID == product.ID).ToList();
        //                for (int i = 0; i < items.Count; i++)
        //                {
        //                    if (items[i].Status == (int)Item.status.已上架)
        //                    {
        //                        //關閉賣場
        //                        items[i].Status = (int)Item.status.未上架;
        //                        items[i].UpdateDate = DateTime.UtcNow.AddHours(8);
        //                        //刪除快取
        //                        Service.ItemService.refreshCache(items[i].ID, null, null, true, "12345qwertASDFGzxcvb");
        //                        logger.Info("ItemNumber: " + product.SellerProductID + " 借賣網賣場異常，關閉賣場【" + items[i].ID.ToString() + "】");
        //                    }
        //                }
        //            }
        //        }
        //        db_before.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Error(e.Message);
        //    }

        //    return false;
        //}

        /// <summary>
        /// 檢查借賣網賣場線上銷售價格是否與資料庫相符，若售價有異動則會更新資料庫Product Cost以及重新計算Item Price。
        /// 線上銷售價格與資料庫相符回傳True，反之回傳False
        /// </summary>
        //public bool CheckItemPriceJieMai(Product queryProduct, ProductDetailResult productdetail)
        //{
        //    TWNewEgg.DB.TWSqlDBContext db_before = new DB.TWSqlDBContext();
        //    log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        //    bool isPriceConsistentWithDB = true;
        //    Product product = db_before.Product.Where(x => x.ID == queryProduct.ID).FirstOrDefault();
        //    try
        //    {
        //        if (product.SourceTable.ToLower() != "productfromjiemai")
        //        {
        //            // 非借賣網商品
        //            throw new Exception("ProductID:" + product.ID + " Not From JieMai");
        //        }
        //        if (product.SellerProductID == null || product.SellerProductID == "")
        //        {
        //            // 商品編號異常
        //            throw new Exception("ProductID:" + product.ID + " sellerproductid is NULL or Empty String");
        //        }
        //        if (productdetail != null)
        //        {
        //            string JMPrice = productdetail.productDetail[0].price.ToString();
        //            JMPrice = JMPrice.Replace("$", "");
        //            JMPrice = JMPrice.Replace(",", "");
        //            decimal price = 0;
        //            decimal.TryParse(JMPrice, out price);
        //            if (price <= 0)
        //            {
        //                string url = System.Configuration.ConfigurationManager.AppSettings["ECWeb"];
        //                // 售價異常，關閉賣場
        //                // 所有販賣這個商品的賣場
        //                List<Item> items = db_before.Item.Where(x => x.ProductID == product.ID).ToList();
        //                // 所有使用這個商品做為屬性
        //                List<ItemList> itemlists = db_before.ItemList.Where(x => x.ItemlistProductID == product.ID).ToList();
        //                for (int i = 0; i < items.Count; i++)
        //                {
        //                    if (items[i].Status == (int)Item.status.已上架)
        //                    {
        //                        logger.Error("ItemID:" + items[i].ToString() + " ProductID:" + product.ID.ToString() + " 售價為0，關閉賣場");
        //                        // 關閉賣場
        //                        items[i].Status = (int)Item.status.未上架;
        //                        items[i].UpdateDate = DateTime.UtcNow.AddHours(8);
        //                        // 刪除快取
        //                        Service.ItemService.refreshCache(items[i].ID, null, null, true, "12345qwertASDFGzxcvb");
        //                    }
        //                }
        //                for (int i = 0; i < itemlists.Count; i++)
        //                {
        //                    if (itemlists[i].Status == (int)Item.status.已上架)
        //                    {
        //                        logger.Error("ItemListID:" + itemlists[i].ToString() + " ProductID:" + product.ID.ToString() + " 售價為0，關閉屬性");
        //                        // 關閉屬性
        //                        itemlists[i].Status = (int)ItemList.status.未上架;
        //                        itemlists[i].UpdateDate = DateTime.UtcNow.AddHours(8);
        //                        // 刪除快取
        //                        Service.ItemService.refreshCache(items[i].ID, null, null, true, "12345qwertASDFGzxcvb");
        //                    }
        //                }
        //                // 賣場關閉flag設定為False
        //                isPriceConsistentWithDB = false;
        //            }
        //            if (price != product.Cost)
        //            {
        //                // 售價異動flag設定為false(因目前借賣網商品價格不再受Product.Cost影響所以將旗標改成true)
        //                //isPriceConsistentWithDB = false;
        //                isPriceConsistentWithDB = true;
        //                // 售價異動，更新Product.Cost
        //                product.Cost = price;
        //                logger.Info("售價異動，更新Product.Cost : Product.ID [" + product.ID + "]" + " Product.Cost [" + product.Cost + "]");
        //                ////所有販賣這個商品的賣場 // 暫時拿掉，若借賣網付款資訊取得與Product.Cost有關或修改就得加回來
        //                //List<Item> items = db.Item.Where(x => x.ProductID == product.ID).ToList();
        //                ////所有使用這個商品做為屬性的賣場
        //                //List<itemList> itemlists = db.itemList.Where(x => x.ItemlistProductID == product.ID).ToList();
        //                ////搜尋匯率
        //                //seller seller = db.seller.Where(x => x.ID == product.SellerID).FirstOrDefault();
        //                //if (seller != null)
        //                //{
        //                //    string year = DateTime.Now.Year.ToString();
        //                //    string month = DateTime.Now.Month.ToString();
        //                //    Currency currency = db.Currency.Where(x => x.Type == seller.CurrencyType && x.Year == year && x.Month == month).FirstOrDefault();
        //                //    if (currency != null)
        //                //    {
        //                //        //使用當期匯率重新計算售價
        //                //        decimal nt = price * currency.AverageexchangeRate;
        //                //        nt = Math.Ceiling(nt);
        //                //        for (int i = 0; i < items.Count; i++)
        //                //        {
        //                //            //新售價
        //                //            items[i].PriceCash = nt;
        //                //            items[i].UpdateDate = DateTime.UtcNow.AddHours(8);
        //                //            //刪除快取
        //                //            Service.ItemService.refreshCache(items[i].ID, null, null, true, "12345qwertASDFGzxcvb");
        //                //        }
        //                //        for (int i = 0; i < itemlists.Count; i++)
        //                //        {
        //                //            //新售價
        //                //            itemlists[i].Price = nt;
        //                //            itemlists[i].UpdateDate = DateTime.UtcNow.AddHours(8);
        //                //            //刪除快取
        //                //            Service.ItemService.refreshCache(itemlists[i].ItemID, null, null, true, "12345qwertASDFGzxcvb");
        //                //        }
        //                //    }
        //                //    else
        //                //    {
        //                //        //找不到當期匯率
        //                //        logger.Fatal("Currency Not Exist :" + year + "/" + month);
        //                //    }
        //                //}
        //                //else
        //                //{
        //                //    //找不到 seller
        //                //    logger.Fatal("seller Not Exist => ProductID:" + product.ID.ToString() + " SellerID:" + product.SellerID.ToString());
        //                //}
        //            }
        //        }
        //        else
        //        {
        //            //因借賣網賣場狀態異常，查詢不到售價
        //            throw new Exception("因借賣網賣場狀態異常，查詢不到售價 ItemNumber: " + product.SellerProductID);
        //        }
        //        db_before.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Error(e.Message);
        //        throw new Exception(e.Message + "_" + e.StackTrace);
        //    }
        //    return isPriceConsistentWithDB;
        //}

        /// <summary>
        /// setup the cookie to store the ids of recently viewed items.
        /// </summary>
        /// <param name="item_id">the id of the latest viewed item</param>
        public static void setCookie(int item_id, HttpResponseBase Response, HttpRequestBase req)
        {
            List<string> recentID = getCookeiItemIDs(req);
            if (recentID.Contains(item_id.ToString()))
            {
                recentID.Remove(item_id.ToString());
            }
            recentID.Add(item_id.ToString());
            if (recentID.Count() > 5)
            {
                recentID.Reverse();
                recentID = recentID.Take(5).ToList();
                recentID.Reverse();
            }

            if (req.Cookies["recentItem"] != null)
            {
                req.Cookies["recentItem"].Value = recentID.Aggregate((i, j) => i + ',' + j);
            }
            if (Response.Cookies["recentItem"] != null)
            {
                Response.Cookies["recentItem"].Value = recentID.Aggregate((i, j) => i + ',' + j);
            }
        }

        /// <summary>
        /// get the ids of recently viewed items.
        /// </summary>
        /// <returns></returns>
        public static List<string> getCookeiItemIDs(HttpRequestBase Request)
        {
            if (Request.Cookies.AllKeys.Contains("recentItem"))
            {
                HttpCookie cookie = Request.Cookies["recentItem"];
                List<string> recentID = new List<string>((cookie.Value ?? "").Split(',').ToList());
                return recentID;
            }
            else
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// remove cache of specific item if call by internal ip
        /// </summary>
        /// <param name="item_id">cache to remove</param>
        /// <param name="req">HttpRequestBase to remove the cache</param>
        /// <param name="servBase">HttpServerUtilityBase where cache saved at</param>
        /// <returns>true if cache was successfully deleted, else return false</returns>
        public static bool refreshCache(int item_id, HttpRequestBase req, HttpServerUtilityBase servBase, bool first = false, string password = "")
        {
            log4net.ILog logger = log4net.LogManager.GetLogger("ItemService");
            string ip = "";
            if (req != null)
            {
                ip = req.UserHostAddress.Split('.')[0];
            }
            if (password == "12345qwertASDFGzxcvb" || ip == "172" || ip == "10" || ip == "::1")
            {
                if (first)
                {
                    WebClient client = new WebClient();
                    System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"ECWeb_[\d]+");
                    var ECServers = System.Configuration.ConfigurationManager.AppSettings.AllKeys.Where(s => regex.IsMatch(s)).ToList();
                    foreach (var host in ECServers)
                    {
                        //logger.Info("Refresh Cache Server=" + host + " URL=" + host + "/item/itemdetailr?item_id=" + item_id + "&first=false&password=12345qwertASDFGzxcvb");
                        try
                        {
                            client.DownloadString(new Uri(host + "/item/itemdetailr?item_id=" + item_id + "&first=false&password=12345qwertASDFGzxcvb"));
                        }
                        catch (Exception e)
                        {
                            //logger.Info(e.Message);
                        }
                    }
                }
                HttpRuntime.Cache.Remove(item_id.ToString());
                string path = CachePath(item_id, servBase);
                if (System.IO.File.Exists(path + "/" + item_id % 10000 + ".xml"))
                {
                    System.IO.File.Delete(path + "/" + item_id % 10000 + ".xml");
                }
                if (!System.IO.File.Exists(path + "/" + item_id % 10000 + ".xml"))
                {
                    return true;
                }
            }
            return false;
        }

        public List<int> FindProductIDsByCategoryID(int CategoryID, int Layer, int CountryID, int BrandID, int SellerID)
        {
            List<int> allProductIDs = new List<int>();
            TWNewEgg.DB.TWSqlDBContext twsqlDB = new DB.TWSqlDBContext();

            var categoryID = twsqlDB.Category.Where(x => x.ID == CategoryID && x.Layer == Layer).FirstOrDefault();
            if (categoryID == null)
            {
                return allProductIDs;
            }

            var sellerID = twsqlDB.Seller.Where(x => x.ID == SellerID).FirstOrDefault();

            var manufactureID = twsqlDB.Manufacture.Where(x => x.ID == BrandID).FirstOrDefault();

            var categoryItems = twsqlDB.Item.Where(x => x.CategoryID == CategoryID);

            if (sellerID != null)
            {
                categoryItems = categoryItems.Where(x => x.SellerID == sellerID.ID);
            }
            if (manufactureID != null)
            {
                categoryItems = categoryItems.Where(x => x.ManufactureID == manufactureID.ID);
            }

            categoryItems = categoryItems.Where(x => x.CategoryID == CategoryID && x.Status == 0 && x.ShowOrder >= 0);

            allProductIDs = categoryItems.Select(x => x.ProductID).Distinct().ToList();

            return allProductIDs;
        }

        /// <summary>
        /// get the path of cache of specific item
        /// </summary>
        /// <param name="item_id">id of the cache</param>
        /// <param name="servBase">HttpServerUtilityBase where cache saved at</param>
        /// <returns></returns>
        public static string CachePath(int item_id, HttpServerUtilityBase servBase)
        {
            return AppDomain.CurrentDomain.BaseDirectory + "itemcache/" + (item_id / 10000).ToString("0000");
        }

        public string GetItemImagePath(int id, int order, int size, string table)
        {

            string path = "";
            if (order != 0)
            {
                path = string.Format("/Pic/{0}/{1}/{2}_{3}_{4}.{5}", table, (id / 10000).ToString("0000"), (id % 10000).ToString("0000"), order, size, "jpg");
            }
            else
            {
                path = "";
            }
            return path;
        }
    }
}