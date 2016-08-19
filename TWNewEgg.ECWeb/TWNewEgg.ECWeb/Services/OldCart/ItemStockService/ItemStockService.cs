using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWBACKENDDB.Models;
using TWNewEgg.DB;
using TWNewEgg.ECWeb.Auth;
//using JieMai.com.jiemai.query;

namespace TWNewEgg.Website.ECWeb.Service
{
    public class ItemStockService
    {
        /// <summary>
        /// 檢查美蛋07倉的商品庫存數量是否大於等於欲訂購數量，同時更新資料庫ItemStock的數量。
        /// </summary>
        /// <param name="productID">商品編號ProductID</param>
        /// <param name="soQTY">欲訂購數量</param>
        /// <returns>庫存數量大於等於欲訂購數量則回傳True</returns>
        public static bool CheckStockQtyNeweggUSA(int productID, int soQTY)
        {
            log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            string userEmail = NEUser.Email;
            logger.Info("[UserEmail] " + userEmail + " [CheckStockQtyNeweggUSA] start");
            string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
            string turnON = System.Configuration.ConfigurationManager.AppSettings[environment + "_CheckStockQtyNeweggUSA"];
            turnON = turnON.ToLower();
            if (turnON != "on")
            {
                return true;
            }
            try
            {
                TWSqlDBContext db = new TWSqlDBContext();
                Product product = db.Product.Where(x => x.ID == productID).FirstOrDefault();
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
                //嘗試取得商品資訊
                string itemNumber = product.SellerProductID;
                string warehouseNumber = "07";
                Q4S q4s = new Q4S();
                /*For Test*/
                /*itemNumber = "9SIAWE50292245";
                warehouseNumber = "9AD1";*/
                /**********/
                string config = "";
                try
                {
                    config = System.Configuration.ConfigurationManager.AppSettings[environment + "_QueryPriceStockProvider"];
                }
                catch { }
                int type = 0;
                int.TryParse(config, out type);
                Service.QueryPriceStock qps = new QueryPriceStock(new QueryPriceStockFactory((QueryPriceStockFactory.Provider)type));
                List<string> list = new List<string>();
                list.Add(itemNumber);
                Dictionary<string, int> result = qps.GetStock(list, warehouseNumber);
                //更新資料庫ItemStock
                UpdateItemStock(productID, result[itemNumber]);
                //判斷庫存數量是否大於等於訂購數量
                if (result[itemNumber] >= soQTY)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
            logger.Info("[UserEmail] " + userEmail + " [CheckStockQtyNeweggUSA] end");
            return false;
        }

        /// <summary>
        /// 檢查借賣網深圳倉的商品庫存數量是否大於等於欲訂購數量，同時更新資料庫ItemStock的數量。
        /// </summary>
        /// <param name="productID">商品編號ProductID</param>
        /// <param name="soQTY">欲訂購數量</param>
        /// <returns>庫存數量大於等於欲訂購數量則回傳True</returns>
        //public static bool CheckStockQtyJieMai(Product product, int soQTY, ProductDetailResult productdetail)
        //{
        //    int ItemStockQty = 0;
        //    try
        //    {
        //        if (product.SourceTable.ToLower() != "productfromjiemai")
        //        {
        //            //非借賣網商品
        //            throw new Exception("ProductID:" + product.ID + " Not From JieMai");
        //        }
        //        if (product.SellerProductID == null || product.SellerProductID == "")
        //        {
        //            //商品編號異常
        //            throw new Exception("ProductID:" + product.ID + " sellerproductid is NULL or Empty String");
        //        }
        //        int SellerProductIDCount = product.SellerProductID.Count();
        //        string subSellerProductID = product.SellerProductID.Substring(SellerProductIDCount - 4, 4);
        //        if (subSellerProductID.ToLower() != "szzw")
        //        {
        //            //非深圳倉商品
        //            throw new Exception("ProductID:" + product.ID + " Not From 深圳倉");
        //        }
        //        if (productdetail != null)
        //        {
        //            ItemStockQty = (int)productdetail.productDetail[0].stockNum;
        //        }
        //        else
        //        {
        //            //因借賣網賣場狀態異常，查詢不到商品資訊
        //            throw new Exception("因借賣網賣場狀態異常，查詢不到商品資訊 ItemNumber: " + product.SellerProductID);
        //        }
        //        //更新資料庫ItemStock
        //        UpdateItemStock(product.ID, ItemStockQty);
        //        //判斷庫存數量是否大於等於訂購數量
        //        if (ItemStockQty >= soQTY)
        //        {
        //            return true;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        //        logger.Error(e.Message);
        //    }
        //    return false;
        //}

        /// <summary>
        /// 更新資料庫ItemStock的庫存數量
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="qty"></param>
        private static void UpdateItemStock(int productID, int qty)
        {
            // 系統更改商品狀態時(上下架、售價、庫存)，UpdateUser填入值
            string newEggItemStatusCheck = "System_NewEggItemStatusCheck";
            //庫存緩衝數量 0
            qty = qty - 0;
            if (qty < 0)
            {
                qty = 0;
            }
            TWSqlDBContext db = new TWSqlDBContext();
            ItemStock stock = db.ItemStock.Where(x => x.ProductID == productID).FirstOrDefault();
            if (stock == null)
            {
                throw new Exception("ProductID: " + productID.ToString() + " ItemStock: Null");
            }
            else
            {
                bool canSync = true;//是否允許同步美蛋線上庫存
                int itemCount = db.Item.Where(x => x.DelvType == 0 && x.ProductID == stock.ProductID).Count();
                if (itemCount > 0)
                {
                    //存在切貨模式的賣場 就必須以新蛋倉庫存量為主 不允許同步美蛋線上庫存
                    canSync = false;
                }
                if (canSync)
                {
                    if (stock.Qty >= 32767)
                    {
                        //qty = 32767 是虛商品
                        //虛商品不做任何庫存數量的變動
                    }
                    else
                    {
                        qty = qty + stock.QtyReg;
                        if (qty >= 32767)
                        {
                            //為避免商品因庫存數量變動而變成虛商品
                            //當qty大於等於32767時，限制為 32766
                            qty = 32766;
                        }
                        if (qty >= stock.QtyReg)
                        {
                            //寫入
                            stock.Qty = qty;
                            stock.UpdateUser = newEggItemStatusCheck;
                            db.SaveChanges();
                        }
                    }
                }
            }
        }
    }
}