using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.API.Models;
using System.Data;
using System.Web;
using log4net;
using log4net.Config;
using System.Transactions;
using AutoMapper;
using TWNewEgg.DB.TWSELLERPORTALDB.Models;
using System.Net;
using System.IO;

namespace TWNewEgg.API.Service
{
    public class SellerItemToVendor
    {
        private DB.TWBackendDBContext backend = new DB.TWBackendDBContext();
        private DB.TWSqlDBContext frontend = new DB.TWSqlDBContext();
        private DB.TWSellerPortalDBContext sellerPortalDB = new DB.TWSellerPortalDBContext();

        public class imageModel
        {
            public List<string> PictureURL { get; set; }

            public string ProductID { get; set; }

            public string ItemID { get; set; }
        }

        // 圖片機網址
        private string images = System.Configuration.ConfigurationManager.AppSettings["Images"];
        // 賣場網址
        private string webSite = System.Configuration.ConfigurationManager.AppSettings["WebSite"];

        private int GetNewSellerID(int oldSellerID)
        {
            string oldSellerEmail = sellerPortalDB.Seller_BasicInfo.Where(x => x.SellerID == oldSellerID).Select(r => r.SellerEmail).FirstOrDefault();

            int newSellerID = sellerPortalDB.Seller_BasicInfo.Where(x => x.SellerEmail == oldSellerEmail).Where(y => y.AccountTypeCode == "V").Select(r => r.SellerID).FirstOrDefault();

            return newSellerID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldSellerID"></param>
        /// <returns></returns>
        public List<string> ItemUpdate(int oldSellerID)
        {
            DB.TWSqlDBContext db_before = new DB.TWSqlDBContext();
            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            List<string> result = new List<string>();
            int newItemCount = 0;
            // 1. 先由 oldSellerID 取得此 Seller 一共有哪些舊的 ItemID 組成 List<int>
            List<Item> oldItemList = frontend.Item.Where(x => x.SellerID == oldSellerID && x.Status != 99).ToList();
            List<int> oldItemIDList = oldItemList.Select(s => s.ID).ToList();
            List<int> oldProductIDList = oldItemList.Select(x => x.ProductID).ToList();
            List<Product> oldProductList = frontend.Product.Where(x => oldProductIDList.Contains(x.ID)).ToList();
            List<ItemStock> oldItemStockList = frontend.ItemStock.Where(x => oldProductIDList.Contains(x.ProductID)).ToList();
            
            result.Add("SellerID :"+ oldSellerID + ",Item 有"+ oldItemIDList.Count);
            int newSellerID = 0;
            newSellerID = this.GetNewSellerID(oldSellerID);

            if (newSellerID != 0)
            {
                // 2. for迴圈(List<int> ItemID)呼叫 SearchCreatedItem(int oldsellerID, int itemID) 組出兩個 List 傳給創建商品
                // P.S DelvType 修改為 Vendor and sellerID改為新的 sellerID
                foreach (int oldSellerItemID_index in oldItemIDList)
                {
                    // SP.productdetail.safeqty 存入 -> twsql.itemstock.safeqty 利用 productID 找資料
                    this.AutoInsertSafeQty(oldSellerItemID_index, oldSellerID, newSellerID);

                    // 從DB用 ItemID 找Item、ItemStock、Product
                    // 先 Binding 新的DB Model 
                    // 修改 delvType(7)、createuser、updateuser、CreateDate、DateStart、DateEnd、DateDel
                    Item oldItem = oldItemList.Where(x => x.ID == oldSellerItemID_index).FirstOrDefault();
                    Product oldProduct = oldProductList.Where(x => x.ID == oldItem.ProductID).FirstOrDefault();
                    ItemStock oldItemStock = oldItemStockList.Where(x => x.ProductID == oldProduct.ID).FirstOrDefault();


                    if (oldItem != null && oldProduct != null && oldItemStock != null)
                    {
                        //----------------------------------------------------
                        // 創建新的Product並取得新Product的ID
                        int newProductID = 0;
                        newProductID = this.CreateNewProduct(oldProduct, newSellerID);

                        if (newProductID != 0)
                        {
                            // 創建新的Item與新的ItemStock
                            string message = this.CreateItemandItemStock(oldItem, oldItemStock, newSellerID, newProductID);
                            //------------------------------------------------------

                            string[] messageArray = message.Split(new char[] { '[', ']' }).Where(s => !string.IsNullOrEmpty(s)).ToArray();


                            DB.TWSELLERPORTALDB.Models.Seller_ChangeToVendor itemCreatelog = new Seller_ChangeToVendor();

                            if (messageArray[0].ToLower() == "success")
                            {
                                newItemCount++;
                                itemCreatelog.NewItemID = Convert.ToInt32(messageArray[1]);
                                result.Add(" 新Item.ID[" + messageArray[1] + "] ItemStock.ID[" + messageArray[2] + "] 創建成功 ");
                                GetitemImage(oldSellerItemID_index, messageArray[1], newProductID.ToString());
                            }
                            else
                            {
                                itemCreatelog.Exception = message;
                                result.Add(message);
                            }

                            itemCreatelog.NewProductID = newProductID;
                            itemCreatelog.NewSellerID = newSellerID;
                            itemCreatelog.OldItemID = oldSellerItemID_index;
                            itemCreatelog.OldProductID = oldItem.ProductID;
                            itemCreatelog.OldSellerID = oldItem.SellerID;
                            itemCreatelog.InDate = DateTime.Now;

                            spdb.Seller_ChangeToVendor.Add(itemCreatelog);
                        }
                    }
                    else
                    {
                        DB.TWSELLERPORTALDB.Models.Seller_ChangeToVendor itemCreatelog = new Seller_ChangeToVendor();

                        itemCreatelog.NewSellerID = newSellerID;
                        itemCreatelog.OldItemID = oldSellerItemID_index;
                        itemCreatelog.OldProductID = oldProduct != null ? oldProduct.ID : 0;
                        itemCreatelog.OldSellerID = oldItem != null ? oldItem.SellerID : 0;

                        itemCreatelog.Exception += oldProduct == null ? "oldItemID[" + oldItem.ID + "] 無 oldProduct" : "";
                        itemCreatelog.Exception += oldItemStock == null ? "oldItemID[" + oldItem.ID + "] 無 oldItemStock" : "";
                        itemCreatelog.InDate = DateTime.Now;
                        result.Add(itemCreatelog.Exception);

                        spdb.Seller_ChangeToVendor.Add(itemCreatelog);
                    }

                }

                try
                {
                    spdb.SaveChanges();
                }
                catch (Exception e)
                {
                }

                result.Add("APIt成功轉換: " + newItemCount + "筆資料");

            }
            else
            {
                DB.TWSELLERPORTALDB.Models.Seller_ChangeToVendor itemCreatelog = new Seller_ChangeToVendor();
                itemCreatelog.OldSellerID = oldSellerID;
                itemCreatelog.Exception = "Old SellerID: " + oldSellerID + ", 無法找到新的SellerID";
                itemCreatelog.InDate = DateTime.Now;

                try
                {
                    spdb.Seller_ChangeToVendor.Add(itemCreatelog);
                    spdb.SaveChanges();
                }
                catch (Exception)
                {

                }

                result.Add("Old SellerID: " + oldSellerID + ", 無法找到新的SellerID");
            }
            
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldItem"></param>
        /// <param name="oldItemStock"></param>
        /// <param name="newSellerID"></param>
        /// <param name="newProduct"></param>
        private string CreateItemandItemStock(Item oldItem, ItemStock oldItemStock, int newSellerID, int newProduct)
        {
            DB.TWSQLDB.Models.Item newItem = new DB.TWSQLDB.Models.Item();
            DB.TWSQLDB.Models.ItemStock newItemStock = new DB.TWSQLDB.Models.ItemStock();
            // 複製至新Item
            Mapper.CreateMap<DB.TWSQLDB.Models.Item, DB.TWSQLDB.Models.Item>()
                        .ForMember(x => x.SellerID, y => y.Ignore())
                        .ForMember(x => x.UpdateUser, y => y.Ignore())
                        .ForMember(x => x.ID, y => y.Ignore())
                        .ForMember(x => x.DelvType, y => y.Ignore());

            newItem = Mapper.Map<DB.TWSQLDB.Models.Item>(oldItem);

            newItem.Status = 1;
            newItem.ShowOrder = -1;
            newItem.ProductID = newProduct;
            newItem.CreateDate = DateTime.Now;
            newItem.CreateUser = "sellerSystem";
            newItem.UpdateUser = "sellerSystem";
            newItem.UpdateDate = DateTime.Now;
            newItem.DelvType = (int)TWNewEgg.DB.TWSQLDB.Models.Item.tradestatus.B2C直配;
            newItem.SellerID = newSellerID;
            DateTime startDate = DateTime.Parse("2015-03-31");
            newItem.DateStart = startDate;
            newItem.DateEnd = startDate.AddYears(10);
            newItem.DateDel = startDate.AddYears(10).AddDays(1);

            // 複製至新ItemStock
            Mapper.CreateMap<DB.TWSQLDB.Models.ItemStock, DB.TWSQLDB.Models.ItemStock>()
                        .ForMember(x => x.UpdateUser, y => y.Ignore())
                        .ForMember(x => x.ID, y => y.Ignore());

            newItemStock = Mapper.Map<DB.TWSQLDB.Models.ItemStock>(oldItemStock);

            newItemStock.ProductID = newProduct;
            newItemStock.CreateDate = DateTime.Now;
            newItemStock.CreateUser = "sellerSystem";
            newItemStock.UpdateUser = "sellerSystem";
            newItemStock.UpdateDate = DateTime.Now;

            return CreateItemInfo(newItem, newItemStock);
        }

        /// <summary>
        /// 建立新Item與新ItemStock
        /// </summary>
        /// <param name="frontendItem">新Item info</param>
        /// <param name="frontendItemStock">新ItemStock info</param>
        /// <returns>返回結果</returns>
        private string CreateItemInfo(DB.TWSQLDB.Models.Item frontendItem, DB.TWSQLDB.Models.ItemStock frontendItemStock)
        {
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            
            string str = string.Empty;

            try
            {
                int itemstockID = 0;
                itemstockID = db.ItemStock.Where(z => z.ProductID == frontendItemStock.ProductID).Select(z => z.ID).FirstOrDefault();

                if (itemstockID == 0)
                {
                    db.ItemStock.Add(frontendItemStock);
                }

                db.Item.Add(frontendItem);

                db.SaveChanges();

                return "[Success][" + frontendItem.ID + "][" + frontendItemStock.ID + "]";
            }
            catch (Exception ex)
            {               
                str = "[創建失敗][ErrorMessge] " + ex.Message + " [ErrorStackTrace] " + ex.StackTrace;
            }

            return str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldProduct"></param>
        /// <param name="newSellerID"></param>
        /// <returns></returns>
        private int CreateNewProduct(DB.TWSQLDB.Models.Product oldProduct,int newSellerID)
        {
            DB.TWSQLDB.Models.Product newProduct = new Product();

            Mapper.CreateMap<DB.TWSQLDB.Models.Product, DB.TWSQLDB.Models.Product>()
                        .ForMember(x => x.SellerID, y => y.Ignore())
                        .ForMember(x => x.UpdateUser, y => y.Ignore())
                        .ForMember(x=>x.ID,y=>y.Ignore())
                        .ForMember(x=>x.DelvType,y=>y.Ignore());

            newProduct = Mapper.Map<DB.TWSQLDB.Models.Product>(oldProduct);

            newProduct.CreateDate = DateTime.Now;
            newProduct.CreateUser = "sellerSystem";
            newProduct.UpdateUser = "sellerSystem";
            newProduct.UpdateDate = DateTime.Now;
            //newProduct.UpdateDate = DateTime.Now;
            newProduct.DelvType = (int)TWNewEgg.DB.TWSQLDB.Models.Item.tradestatus.B2C直配;
            newProduct.SellerID = newSellerID;

            int newProductID = this.CreateProductInfo(newProduct, oldProduct.ID, newSellerID);

            return newProductID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frontendProduct"></param>
        /// <returns></returns>
        private int CreateProductInfo(DB.TWSQLDB.Models.Product frontendProduct,int oldProductID,int newSellerID)
        {
            int newproductID = 0;
            DB.TWSqlDBContext frontenddb = new DB.TWSqlDBContext();
            
            try
            {
                frontenddb.Product.Add(frontendProduct);
                frontenddb.SaveChanges();
              
                newproductID = frontendProduct.ID;
            }
            catch (Exception ex)
            {
                string str = ex.Message;
                DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
                DB.TWSELLERPORTALDB.Models.Seller_ChangeToVendor itemCreatelog = new Seller_ChangeToVendor();
                //itemCreatelog.OldSellerID = oldSellerID;
                itemCreatelog.OldProductID = oldProductID;
                itemCreatelog.NewSellerID = newSellerID;
                itemCreatelog.Exception = "[NewProduct創建失敗]" + ex.Message + ex.StackTrace;
                itemCreatelog.InDate = DateTime.Now;

                try
                {
                    spdb.Seller_ChangeToVendor.Add(itemCreatelog);
                    spdb.SaveChanges();
                }
                catch (Exception)
                {

                }
            }

            return newproductID;
        }

        public bool GetitemImage(int olditemID, string newItemID, string newProductID)
        {
            string imageDomain = System.Configuration.ConfigurationManager.AppSettings["Images"].ToString();
            string processresult = string.Empty;
            bool result = false;
            
            TWNewEgg.API.Service.AddProductsInfoForVendorService addForVendorService = new AddProductsInfoForVendorService();

            DB.TWSqlDBContext db_befoe = new DB.TWSqlDBContext();
            List<string> ItemImages640 = new List<string>();
            List<string> ItemImages300 = new List<string>();
            List<string> ItemImages125 = new List<string>();
            List<string> ItemImages60 = new List<string>();
            var PicEnd = db_befoe.Item.Where(x => x.ID == olditemID).Select(r => r.PicEnd).FirstOrDefault();

            // 商品圖
            string pid = olditemID.ToString("00000000");
            string pidf4 = pid.Substring(0, 4);
            string pidl4 = pid.Substring(4, 4);

            if (PicEnd != 0)
            {
                string imageURL = string.Empty;
                DB.TWSELLERPORTALDB.Models.Seller_ChangeToVendor log = new Seller_ChangeToVendor();

                for (int picIndex = 1; picIndex <= PicEnd; picIndex++)
                {
                    ItemImages640.Add(imageDomain + "/pic/item/" + pidf4 + "/" + pidl4 + "_" + picIndex + "_640.jpg");
                    ItemImages300.Add(imageDomain + "/pic/item/" + pidf4 + "/" + pidl4 + "_" + picIndex + "_300.jpg");
                    ItemImages125.Add(imageDomain + "/pic/item/" + pidf4 + "/" + pidl4 + "_" + picIndex + "_125.jpg");
                    ItemImages60.Add(imageDomain + "/pic/item/" + pidf4 + "/" + pidl4 + "_" + picIndex + "_60.jpg"); 
                }

                //result = addForVendorService.PostImageToDB(ItemImages, newProductID, newItemID).IsSuccess;

                imageModel image640 = new imageModel();
                image640.PictureURL = ItemImages640;
                image640.ItemID = newItemID;
                image640.ProductID = newProductID;

                imageModel image300 = new imageModel();
                image300.PictureURL = ItemImages300;
                image300.ItemID = newItemID;
                image300.ProductID = newProductID;

                imageModel image125 = new imageModel();
                image125.PictureURL = ItemImages125;
                image125.ItemID = newItemID;
                image125.ProductID = newProductID;

                imageModel image60 = new imageModel();
                image60.PictureURL = ItemImages60;
                image60.ItemID = newItemID;
                image60.ProductID = newProductID;



                Connector conn = new Connector();
                //result = conn.Post<ActionResponse<bool>>("http://172.22.55.134:71/SellerToVendor/Process640Images", null, null, image640).IsSuccess;

                

                // 處理 640 圖片的結果
                if (conn.Post<ActionResponse<bool>>("http://172.22.55.134:71/SellerToVendor/Process640Images", null, null, image640).IsSuccess)
                {
                    processresult = "640處理成功";
                    result = true;
                }
                else
                {
                    processresult = "640處理失敗";
                    result = false;
                }

                if (conn.Post<ActionResponse<bool>>("http://172.22.55.134:71/SellerToVendor/Process300Images", null, null, image300).IsSuccess)
                {
                    processresult += ", 300處理成功";
                    result = true;
                }
                else
                {
                    processresult += ", 300處理失敗";
                    result = false;
                }

                if (conn.Post<ActionResponse<bool>>("http://172.22.55.134:71/SellerToVendor/Process125Images", null, null, image125).IsSuccess)
                {
                    processresult += ", 125處理成功";
                    result = true;
                }
                else
                {
                    processresult += ", 125處理失敗";
                    result = false;
                }

                if (conn.Post<ActionResponse<bool>>("http://172.22.55.134:71/SellerToVendor/Process60Images", null, null, image60).IsSuccess)
                {
                    processresult += ", 60處理成功";
                    result = true;
                }
                else
                {
                    processresult += ", 60處理失敗";
                    result = false;
                }
                               
                DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();

                log.NewItemID = Convert.ToInt32(newItemID);
                log.OldItemID = olditemID;
                log.InDate = DateTime.Now;
                log.Exception += result == true ? "圖片轉換成功" : "圖片轉換失敗" + processresult;

                try
                {
                    spdb.Seller_ChangeToVendor.Add(log);
                    spdb.SaveChanges();
                }
                catch(Exception ex)
                {
                    
                }               
            }

            return result;
        }

        public ActionResponse<bool> PostImageToDB640(List<string> picturesURL, string productID, string itemID)
        {
            ActionResponse<bool> queryResult = new ActionResponse<bool>();
            System.Net.WebClient client = new System.Net.WebClient();
            try
            {
                if (picturesURL != null)
                {
                    //ID = ("75403");
                    //int Size = 0;
                    int picCount = 1;

                    List<string> tempImageURL = new List<string>();

                    // 第一步 儲存原始圖片到/pic/item及Product
                    foreach (var picURL in picturesURL)
                    {
                        // WebClient 加入 Header 讓Server 認為是瀏覽器訪問
                        client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                        TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

                        productID = productID.PadLeft(8, '0');
                        itemID = itemID.PadLeft(8, '0');
                        string productFilePath = AppDomain.CurrentDomain.BaseDirectory + "pic\\pic\\product\\{0}\\{1}_{2}_{3}.jpg";
                        string path = string.Format(productFilePath, productID.Substring(0, 4), productID.Substring(4, 4), picCount, "640");
                        string itemFilePath = AppDomain.CurrentDomain.BaseDirectory + "pic\\pic\\item\\{0}\\{1}_{2}_{3}.jpg";
                        string itempath = string.Format(itemFilePath, itemID.Substring(0, 4), itemID.Substring(4, 4), picCount, "640");

                        //略過代理驗證 
                        IWebProxy myProxy = System.Net.GlobalProxySelection.GetEmptyWebProxy();
                        System.Net.GlobalProxySelection.Select = myProxy;

                        //檢查資料夾路徑
                        if (Directory.Exists(path.Substring(0, path.LastIndexOf('\\'))) == false)
                       {
                            Directory.CreateDirectory(path.Substring(0, path.LastIndexOf('\\')));
                       }

                        if (Directory.Exists(itempath.Substring(0, itempath.LastIndexOf('\\'))) == false)
                        {
                            Directory.CreateDirectory(itempath.Substring(0, itempath.LastIndexOf('\\')));
                        }

                        //從x的位置下載圖片，若圖片網址回應404，跳過儲存
                        try
                        {
                            client.DownloadFile(picURL, AppDomain.CurrentDomain.BaseDirectory + "pic\\pic\\product\\" + productID.Substring(0, 4) + "\\" + productID.Substring(4, 4) + "_" + picCount + "_" + "640.jpg");

                            client.DownloadFile(picURL, AppDomain.CurrentDomain.BaseDirectory + "pic\\pic\\item\\" + itemID.Substring(0, 4) + "\\" + itemID.Substring(4, 4) + "_" + picCount + "_" + "640.jpg");
                        }
                        catch
                        {
                            continue;
                        }

                        //使用以下位置及檔名儲存圖片
                        //string srcproductImagePath = AppDomain.CurrentDomain.BaseDirectory + "pic\\product\\" + productID.Substring(0, 4) + "\\" + productID.Substring(4, 4) + "_" + picCount + "_" + "原始大小.jpg";

                        //string srcItemImagePath = AppDomain.CurrentDomain.BaseDirectory + "pic\\item\\" + itemID.Substring(0, 4) + "\\" + itemID.Substring(4, 4) + "_" + picCount + "_" + "原始大小.jpg";

                        //使用product的原始圖片做縮圖，丟往第二步
                        //tempImageURL.Add(srcproductImagePath);

                        //client.DownloadFile(x , @"D:\pic\");
                        picCount++;
                    }

                    //// 第二步 使用/pic/item和Product 的圖做縮圖存到 Pic/pic/item&Product Jack.W.Wu mod 0530
                    //int saveCount = 1;
                    //foreach (var url in tempImageURL)
                    //{
                    //    SaveThumbPicWidth(url, 640, productID, itemID, saveCount);
                    //    SaveThumbPicWidth(url, 300, productID, itemID, saveCount);
                    //    SaveThumbPicWidth(url, 125, productID, itemID, saveCount);
                    //    SaveThumbPicWidth(url, 60, productID, itemID, saveCount);
                    //    saveCount++;
                    //}

                    queryResult.Body = true;
                    queryResult.Msg = "成功存入圖片！";
                    queryResult.Code = (int)ResponseCode.Success;
                    queryResult.IsSuccess = true;
                }
                else
                {
                    //顯示DB寫入狀態(Detail Info)
                    queryResult.Body = false;
                    queryResult.Msg = "部分資料無內容(Item_圖片URL)。";
                    queryResult.Code = (int)ResponseCode.Success;
                    queryResult.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                //顯示DB寫入狀態(Detail Info)
                queryResult.Body = false;
                queryResult.Msg = "例外錯誤發生，" + ex.Message;
                queryResult.Code = (int)ResponseCode.Error;
                queryResult.IsSuccess = false;
            }

            return queryResult;
        }

        public ActionResponse<bool> PostImageToDB300(List<string> picturesURL, string productID, string itemID)
        {
            ActionResponse<bool> queryResult = new ActionResponse<bool>();
            System.Net.WebClient client = new System.Net.WebClient();
            try
            {
                if (picturesURL != null)
                {
                    //ID = ("75403");
                    //int Size = 0;
                    int picCount = 1;

                    List<string> tempImageURL = new List<string>();

                    // 第一步 儲存原始圖片到/pic/item及Product
                    foreach (var picURL in picturesURL)
                    {
                        // WebClient 加入 Header 讓Server 認為是瀏覽器訪問
                        client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                        TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

                        productID = productID.PadLeft(8, '0');
                        itemID = itemID.PadLeft(8, '0');
                        string productFilePath = AppDomain.CurrentDomain.BaseDirectory + "pic\\pic\\product\\{0}\\{1}_{2}_{3}.jpg";
                        string path = string.Format(productFilePath, productID.Substring(0, 4), productID.Substring(4, 4), picCount, "640");
                        string itemFilePath = AppDomain.CurrentDomain.BaseDirectory + "pic\\pic\\item\\{0}\\{1}_{2}_{3}.jpg";
                        string itempath = string.Format(itemFilePath, itemID.Substring(0, 4), itemID.Substring(4, 4), picCount, "640");

                        //略過代理驗證 
                        IWebProxy myProxy = System.Net.GlobalProxySelection.GetEmptyWebProxy();
                        System.Net.GlobalProxySelection.Select = myProxy;

                        //檢查資料夾路徑
                        if (Directory.Exists(path.Substring(0, path.LastIndexOf('\\'))) == false)
                        {
                            Directory.CreateDirectory(path.Substring(0, path.LastIndexOf('\\')));
                        }

                        if (Directory.Exists(itempath.Substring(0, itempath.LastIndexOf('\\'))) == false)
                        {
                            Directory.CreateDirectory(itempath.Substring(0, itempath.LastIndexOf('\\')));
                        }

                        //從x的位置下載圖片，若圖片網址回應404，跳過儲存
                        try
                        {
                            client.DownloadFile(picURL, AppDomain.CurrentDomain.BaseDirectory + "pic\\pic\\product\\" + productID.Substring(0, 4) + "\\" + productID.Substring(4, 4) + "_" + picCount + "_" + "300.jpg");

                            client.DownloadFile(picURL, AppDomain.CurrentDomain.BaseDirectory + "pic\\pic\\item\\" + itemID.Substring(0, 4) + "\\" + itemID.Substring(4, 4) + "_" + picCount + "_" + "300.jpg");
                        }
                        catch
                        {
                            continue;
                        }

                        //使用以下位置及檔名儲存圖片
                        //string srcproductImagePath = AppDomain.CurrentDomain.BaseDirectory + "pic\\product\\" + productID.Substring(0, 4) + "\\" + productID.Substring(4, 4) + "_" + picCount + "_" + "原始大小.jpg";

                        //string srcItemImagePath = AppDomain.CurrentDomain.BaseDirectory + "pic\\item\\" + itemID.Substring(0, 4) + "\\" + itemID.Substring(4, 4) + "_" + picCount + "_" + "原始大小.jpg";

                        //使用product的原始圖片做縮圖，丟往第二步
                        //tempImageURL.Add(srcproductImagePath);

                        //client.DownloadFile(x , @"D:\pic\");
                        picCount++;
                    }

                    //// 第二步 使用/pic/item和Product 的圖做縮圖存到 Pic/pic/item&Product Jack.W.Wu mod 0530
                    //int saveCount = 1;
                    //foreach (var url in tempImageURL)
                    //{
                    //    SaveThumbPicWidth(url, 640, productID, itemID, saveCount);
                    //    SaveThumbPicWidth(url, 300, productID, itemID, saveCount);
                    //    SaveThumbPicWidth(url, 125, productID, itemID, saveCount);
                    //    SaveThumbPicWidth(url, 60, productID, itemID, saveCount);
                    //    saveCount++;
                    //}

                    queryResult.Body = true;
                    queryResult.Msg = "成功存入圖片！";
                    queryResult.Code = (int)ResponseCode.Success;
                    queryResult.IsSuccess = true;
                }
                else
                {
                    //顯示DB寫入狀態(Detail Info)
                    queryResult.Body = false;
                    queryResult.Msg = "部分資料無內容(Item_圖片URL)。";
                    queryResult.Code = (int)ResponseCode.Success;
                    queryResult.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                //顯示DB寫入狀態(Detail Info)
                queryResult.Body = false;
                queryResult.Msg = "例外錯誤發生，" + ex.Message;
                queryResult.Code = (int)ResponseCode.Error;
                queryResult.IsSuccess = false;
            }

            return queryResult;
        }

        /// <summary>
        /// save 125 size 的圖片
        /// </summary>
        /// <param name="picturesURL"></param>
        /// <param name="productID"></param>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public ActionResponse<bool> PostImageToDB125(List<string> picturesURL, string productID, string itemID)
        {
            ActionResponse<bool> queryResult = new ActionResponse<bool>();
            System.Net.WebClient client = new System.Net.WebClient();
            try
            {
                if (picturesURL != null)
                {
                    //ID = ("75403");
                    //int Size = 0;
                    int picCount = 1;

                    List<string> tempImageURL = new List<string>();

                    // 第一步 儲存原始圖片到/pic/item及Product
                    foreach (var picURL in picturesURL)
                    {
                        // WebClient 加入 Header 讓Server 認為是瀏覽器訪問
                        client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                        TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

                        productID = productID.PadLeft(8, '0');
                        itemID = itemID.PadLeft(8, '0');
                        string productFilePath = AppDomain.CurrentDomain.BaseDirectory + "pic\\pic\\product\\{0}\\{1}_{2}_{3}.jpg";
                        string path = string.Format(productFilePath, productID.Substring(0, 4), productID.Substring(4, 4), picCount, "640");
                        string itemFilePath = AppDomain.CurrentDomain.BaseDirectory + "pic\\pic\\item\\{0}\\{1}_{2}_{3}.jpg";
                        string itempath = string.Format(itemFilePath, itemID.Substring(0, 4), itemID.Substring(4, 4), picCount, "640");

                        //略過代理驗證 
                        IWebProxy myProxy = System.Net.GlobalProxySelection.GetEmptyWebProxy();
                        System.Net.GlobalProxySelection.Select = myProxy;

                        //檢查資料夾路徑
                        if (Directory.Exists(path.Substring(0, path.LastIndexOf('\\'))) == false)
                        {
                            Directory.CreateDirectory(path.Substring(0, path.LastIndexOf('\\')));
                        }

                        if (Directory.Exists(itempath.Substring(0, itempath.LastIndexOf('\\'))) == false)
                        {
                            Directory.CreateDirectory(itempath.Substring(0, itempath.LastIndexOf('\\')));
                        }

                        //從x的位置下載圖片，若圖片網址回應404，跳過儲存
                        try
                        {
                            client.DownloadFile(picURL, AppDomain.CurrentDomain.BaseDirectory + "pic\\pic\\product\\" + productID.Substring(0, 4) + "\\" + productID.Substring(4, 4) + "_" + picCount + "_" + "125.jpg");

                            client.DownloadFile(picURL, AppDomain.CurrentDomain.BaseDirectory + "pic\\pic\\item\\" + itemID.Substring(0, 4) + "\\" + itemID.Substring(4, 4) + "_" + picCount + "_" + "125.jpg");
                        }
                        catch
                        {
                            continue;
                        }

                        //使用以下位置及檔名儲存圖片
                        //string srcproductImagePath = AppDomain.CurrentDomain.BaseDirectory + "pic\\product\\" + productID.Substring(0, 4) + "\\" + productID.Substring(4, 4) + "_" + picCount + "_" + "原始大小.jpg";

                        //string srcItemImagePath = AppDomain.CurrentDomain.BaseDirectory + "pic\\item\\" + itemID.Substring(0, 4) + "\\" + itemID.Substring(4, 4) + "_" + picCount + "_" + "原始大小.jpg";

                        //使用product的原始圖片做縮圖，丟往第二步
                        //tempImageURL.Add(srcproductImagePath);

                        //client.DownloadFile(x , @"D:\pic\");
                        picCount++;
                    }

                    //// 第二步 使用/pic/item和Product 的圖做縮圖存到 Pic/pic/item&Product Jack.W.Wu mod 0530
                    //int saveCount = 1;
                    //foreach (var url in tempImageURL)
                    //{
                    //    SaveThumbPicWidth(url, 640, productID, itemID, saveCount);
                    //    SaveThumbPicWidth(url, 300, productID, itemID, saveCount);
                    //    SaveThumbPicWidth(url, 125, productID, itemID, saveCount);
                    //    SaveThumbPicWidth(url, 60, productID, itemID, saveCount);
                    //    saveCount++;
                    //}

                    queryResult.Body = true;
                    queryResult.Msg = "成功存入圖片！";
                    queryResult.Code = (int)ResponseCode.Success;
                    queryResult.IsSuccess = true;
                }
                else
                {
                    //顯示DB寫入狀態(Detail Info)
                    queryResult.Body = false;
                    queryResult.Msg = "部分資料無內容(Item_圖片URL)。";
                    queryResult.Code = (int)ResponseCode.Success;
                    queryResult.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                //顯示DB寫入狀態(Detail Info)
                queryResult.Body = false;
                queryResult.Msg = "例外錯誤發生，" + ex.Message;
                queryResult.Code = (int)ResponseCode.Error;
                queryResult.IsSuccess = false;
            }

            return queryResult;
        }

        /// <summary>
        /// save 60 size 的images
        /// </summary>
        /// <param name="picturesURL"></param>
        /// <param name="productID"></param>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public ActionResponse<bool> PostImageToDB60(List<string> picturesURL, string productID, string itemID)
        {
            ActionResponse<bool> queryResult = new ActionResponse<bool>();
            System.Net.WebClient client = new System.Net.WebClient();
            try
            {
                if (picturesURL != null)
                {
                    //ID = ("75403");
                    //int Size = 0;
                    int picCount = 1;

                    List<string> tempImageURL = new List<string>();

                    // 第一步 儲存原始圖片到/pic/item及Product
                    foreach (var picURL in picturesURL)
                    {
                        // WebClient 加入 Header 讓Server 認為是瀏覽器訪問
                        client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                        TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

                        productID = productID.PadLeft(8, '0');
                        itemID = itemID.PadLeft(8, '0');
                        string productFilePath = AppDomain.CurrentDomain.BaseDirectory + "pic\\pic\\product\\{0}\\{1}_{2}_{3}.jpg";
                        string path = string.Format(productFilePath, productID.Substring(0, 4), productID.Substring(4, 4), picCount, "640");
                        string itemFilePath = AppDomain.CurrentDomain.BaseDirectory + "pic\\pic\\item\\{0}\\{1}_{2}_{3}.jpg";
                        string itempath = string.Format(itemFilePath, itemID.Substring(0, 4), itemID.Substring(4, 4), picCount, "640");

                        //略過代理驗證 
                        IWebProxy myProxy = System.Net.GlobalProxySelection.GetEmptyWebProxy();
                        System.Net.GlobalProxySelection.Select = myProxy;

                        //檢查資料夾路徑
                        if (Directory.Exists(path.Substring(0, path.LastIndexOf('\\'))) == false)
                        {
                            Directory.CreateDirectory(path.Substring(0, path.LastIndexOf('\\')));
                        }

                        if (Directory.Exists(itempath.Substring(0, itempath.LastIndexOf('\\'))) == false)
                        {
                            Directory.CreateDirectory(itempath.Substring(0, itempath.LastIndexOf('\\')));
                        }

                        //從x的位置下載圖片，若圖片網址回應404，跳過儲存
                        try
                        {
                            client.DownloadFile(picURL, AppDomain.CurrentDomain.BaseDirectory + "pic\\pic\\product\\" + productID.Substring(0, 4) + "\\" + productID.Substring(4, 4) + "_" + picCount + "_" + "60.jpg");

                            client.DownloadFile(picURL, AppDomain.CurrentDomain.BaseDirectory + "pic\\pic\\item\\" + itemID.Substring(0, 4) + "\\" + itemID.Substring(4, 4) + "_" + picCount + "_" + "60.jpg");
                        }
                        catch
                        {
                            continue;
                        }

                        //使用以下位置及檔名儲存圖片
                        //string srcproductImagePath = AppDomain.CurrentDomain.BaseDirectory + "pic\\product\\" + productID.Substring(0, 4) + "\\" + productID.Substring(4, 4) + "_" + picCount + "_" + "原始大小.jpg";

                        //string srcItemImagePath = AppDomain.CurrentDomain.BaseDirectory + "pic\\item\\" + itemID.Substring(0, 4) + "\\" + itemID.Substring(4, 4) + "_" + picCount + "_" + "原始大小.jpg";

                        //使用product的原始圖片做縮圖，丟往第二步
                        //tempImageURL.Add(srcproductImagePath);

                        //client.DownloadFile(x , @"D:\pic\");
                        picCount++;
                    }

                    //// 第二步 使用/pic/item和Product 的圖做縮圖存到 Pic/pic/item&Product Jack.W.Wu mod 0530
                    //int saveCount = 1;
                    //foreach (var url in tempImageURL)
                    //{
                    //    SaveThumbPicWidth(url, 640, productID, itemID, saveCount);
                    //    SaveThumbPicWidth(url, 300, productID, itemID, saveCount);
                    //    SaveThumbPicWidth(url, 125, productID, itemID, saveCount);
                    //    SaveThumbPicWidth(url, 60, productID, itemID, saveCount);
                    //    saveCount++;
                    //}

                    queryResult.Body = true;
                    queryResult.Msg = "成功存入圖片！";
                    queryResult.Code = (int)ResponseCode.Success;
                    queryResult.IsSuccess = true;
                }
                else
                {
                    //顯示DB寫入狀態(Detail Info)
                    queryResult.Body = false;
                    queryResult.Msg = "部分資料無內容(Item_圖片URL)。";
                    queryResult.Code = (int)ResponseCode.Success;
                    queryResult.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                //顯示DB寫入狀態(Detail Info)
                queryResult.Body = false;
                queryResult.Msg = "例外錯誤發生，" + ex.Message;
                queryResult.Code = (int)ResponseCode.Error;
                queryResult.IsSuccess = false;
            }

            return queryResult;
        }



        /// <summary>
        /// SP.productdetail.safeqty 存入 -> twsql.itemstock.safeqty 利用 productID 找資料
        /// </summary>
        /// <param name="oldItemID">原ItemID</param>
        /// <param name="oldSellerID">原SellerID</param>
        /// <param name="newSellerID">新SellerID</param>
        private void AutoInsertSafeQty(int oldItemID, int oldSellerID, int newSellerID)
        {
            // 前台資料庫
            DB.TWSqlDBContext twDB = new DB.TWSqlDBContext();
            // sellerPortal資料庫
            DB.TWSellerPortalDBContext spDB = new DB.TWSellerPortalDBContext();
            Item searchItem = twDB.Item.Where(x => x.ID == oldItemID).FirstOrDefault();
            if (searchItem != null)
            {
                int productID = 0;
                productID = searchItem.ProductID;
                Seller_ProductDetail searchProductDetail = spDB.Seller_ProductDetail.Where(x => x.ProductID == productID).FirstOrDefault();
                ItemStock searchItemStock = twDB.ItemStock.Where(x => x.ProductID == productID).FirstOrDefault();

                if (searchProductDetail != null && searchItemStock != null && searchProductDetail.SafeQty > 0)
                {
                    searchItemStock.SafeQty = searchProductDetail.SafeQty;
                    try
                    {
                        twDB.SaveChanges();
                        try
                        {
                            // 成功轉存SafeQty
                            Seller_ChangeToVendor newChangeToVendor = new Seller_ChangeToVendor();
                            newChangeToVendor.OldSellerID = oldSellerID;
                            newChangeToVendor.NewSellerID = newSellerID;
                            newChangeToVendor.OldItemID = oldItemID;
                            newChangeToVendor.OldProductID = productID;
                            newChangeToVendor.Exception = "itemStock.SafeQty轉存成功";
                            newChangeToVendor.InDate = DateTime.Now;
                            spDB.Seller_ChangeToVendor.Add(newChangeToVendor);
                            spDB.SaveChanges();
                        }
                        catch { }
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            // ItemStock.SafeQty轉存失敗
                            Seller_ChangeToVendor newChangeToVendor = new Seller_ChangeToVendor();
                            newChangeToVendor.OldSellerID = oldSellerID;
                            newChangeToVendor.NewSellerID = newSellerID;
                            newChangeToVendor.OldItemID = oldItemID;
                            newChangeToVendor.OldProductID = productID;
                            newChangeToVendor.Exception = "itemStock.SafeQty轉存失敗 [ErrorMessage] " + e.Message + " [ErrorStackTrace] " + e.StackTrace;
                            newChangeToVendor.InDate = DateTime.Now;
                            spDB.Seller_ChangeToVendor.Add(newChangeToVendor);
                            spDB.SaveChanges();
                        }
                        catch { }
                    }
                }

                try
                {
                    // itemStock.SafeQty因故無法轉存
                    Seller_ChangeToVendor newChangeToVendor = new Seller_ChangeToVendor();
                    newChangeToVendor.OldSellerID = oldSellerID;
                    newChangeToVendor.NewSellerID = newSellerID;
                    newChangeToVendor.OldItemID = oldItemID;
                    newChangeToVendor.OldProductID = productID;
                    string errorMessage = string.Empty;
                    if (searchProductDetail == null) { errorMessage += "[searchProductDetail == null]"; }
                    if (searchItemStock == null) { errorMessage += "[searchItemStock == null]"; }
                    newChangeToVendor.Exception = "itemStock.SafeQty無法轉存" + errorMessage;
                    newChangeToVendor.InDate = DateTime.Now;
                    spDB.Seller_ChangeToVendor.Add(newChangeToVendor);
                    spDB.SaveChanges();
                }
                catch { }
            }
            else
            {
                try
                {
                    // 因查無此ItemID資訊故itemStock.SafeQty無法轉存
                    Seller_ChangeToVendor newChangeToVendor = new Seller_ChangeToVendor();
                    newChangeToVendor.OldSellerID = oldSellerID;
                    newChangeToVendor.NewSellerID = newSellerID;
                    newChangeToVendor.OldItemID = oldItemID;
                    newChangeToVendor.Exception = "因查無此ItemID資訊故itemStock.SafeQty無法轉存";
                    newChangeToVendor.InDate = DateTime.Now;
                    spDB.Seller_ChangeToVendor.Add(newChangeToVendor);
                    spDB.SaveChanges();
                }
                catch { }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tmpItemModel"></param>
        /// <returns>返回SPItemCreation模組</returns>
        private SPItemCreation TransferOldModel(List<APIItemModel> tmpItemModel, int newSellerID)
        {
            SPItemCreation newSellerItemModel = new SPItemCreation();
            DB.TWSqlDBContext db_before = new DB.TWSqlDBContext();
            foreach (var tmpItemModel_index in tmpItemModel)
            {
                ProductsInfoResult newSellerProduct = new ProductsInfoResult();
                ItemInfoResult newSellerItem = new ItemInfoResult();

                var db_Itemstock = db_before.ItemStock.Where(x => x.ProductID == tmpItemModel_index.API_Item_ProductID).FirstOrDefault();
                
                //newSellerProduct.BarCode = tmpItemModel_index.API_Product_BarCode;
                //newSellerProduct.Cost = tmpItemModel_index.API_Product_Cost.Value;
                //newSellerProduct.CreateDate = DateTime.Now;
                //newSellerProduct.CreateUser = "61753";
                //newSellerProduct.DelvType = 7;
                //newSellerProduct.Description = tmpItemModel_index.API_Product_Description;
                //newSellerProduct.DescriptionTW = tmpItemModel_index.API_Product_DescriptionTW;
                //newSellerProduct.Height = tmpItemModel_index.API_Product_Height.Value;
                ////newSellerProduct.InvoiceType = tmpItemModel_index
                //newSellerProduct.Is18 = tmpItemModel_index.API_Product_Is18;
                //newSellerProduct.IsChokingDanger = tmpItemModel_index.API_Product_IsChokingDanger;
                ////newSellerProduct.IsMarket = tmpItemModel_index
                //newSellerProduct.IsShipDanger = tmpItemModel_index.API_Product_IsShipDanger;
                //newSellerProduct.Length = tmpItemModel_index.API_Product_Length.Value;
                //newSellerProduct.ManufacturerID = tmpItemModel_index.API_Product_ManufacturerID;
                //newSellerProduct.MenufacturePartNum = tmpItemModel_index.API_Product_MenufacturePartNum;
                //newSellerProduct.Model = tmpItemModel_index.API_Product_Model;
                //newSellerProduct.NameTW = tmpItemModel_index.API_Product_NameTW;
                //newSellerProduct.Note = tmpItemModel_index.API_Product_Note;
                //newSellerProduct.PicEnd = tmpItemModel_index.API_Item_ItemImages.Count();

                //newSellerItem.ItemImages = tmpItemModel_index.API_Item_ItemImages;
                //newSellerItem.DelvType = (int)TWNewEgg.DB.TWSQLDB.Models.Item.tradestatus.B2C直配;
                //newSellerItem.DateDel = tmpItemModel_index.API_Item_DateDel;

                //newSellerProduct.SellerProductID =
                newSellerProduct.ManufacturerID = tmpItemModel_index.API_Product_ManufacturerID;
                newSellerProduct.MenufacturePartNum = tmpItemModel_index.API_Product_MenufacturePartNum;
                newSellerProduct.Cost = tmpItemModel_index.API_Product_Cost.Value;
                newSellerProduct.UPC = tmpItemModel_index.API_Product_UPC;
                newSellerProduct.productName = tmpItemModel_index.API_Product_productName;
                newSellerProduct.Description = tmpItemModel_index.API_Product_Description;
                newSellerProduct.DescriptionTW = tmpItemModel_index.API_Product_DescriptionTW;
                newSellerProduct.Note = tmpItemModel_index.API_Product_Note;
                newSellerProduct.Model = tmpItemModel_index.API_Product_Model;
                newSellerProduct.BarCode = tmpItemModel_index.API_Product_BarCode;
                newSellerProduct.Length = tmpItemModel_index.API_Product_Length.Value;
                newSellerProduct.Width = tmpItemModel_index.API_Product_Width.Value;
                newSellerProduct.Height = tmpItemModel_index.API_Product_Height.Value;
                newSellerProduct.Weight = tmpItemModel_index.API_Product_Weight.Value;
                newSellerProduct.Status = tmpItemModel_index.API_Product_Status;
                newSellerProduct.Warranty = tmpItemModel_index.API_Product_Warranty;
                newSellerProduct.IsShipDanger = tmpItemModel_index.API_Product_IsShipDanger;
                newSellerProduct.IsChokingDanger = tmpItemModel_index.API_Product_IsChokingDanger;
                newSellerProduct.SellerID = newSellerID;
                newSellerProduct.NameTW = tmpItemModel_index.API_Product_NameTW;
                newSellerProduct.DelvType = (int)TWNewEgg.DB.TWSQLDB.Models.Item.tradestatus.B2C直配;
                newSellerProduct.Is18 = tmpItemModel_index.API_Product_Is18;
                newSellerProduct.PicStart = tmpItemModel_index.API_Item_ItemImages != null ? 1 : 0;
                newSellerProduct.PicEnd = tmpItemModel_index.API_Item_ItemImages != null ? tmpItemModel_index.API_Item_ItemImages.Count() : 0;
                newSellerProduct.CreateDate = DateTime.Now;
                newSellerProduct.CreateUser = "61753";
                //newSellerProduct.UpdateDate = tmpItemModel_index
                //newSellerProduct.Updated = tmpItemModel_index.API_Product_Updated;
                //newSellerProduct.UpdateUser = tmpItemModel_index.API_Item_UpdateUser;
                //newSellerProduct.TradeTax = tmpItemModel_index
                //newSellerProduct.SPECLabel = tmpItemModel_index

                //newSellerProduct.Spec = tmpItemModel_index
                //newSellerProduct.SourceTable = tmpItemModel_index
                //newSellerProduct.InvoiceType = tmpItemModel_index
                //newSellerProduct.SaleType = tmpItemModel_index
                //newSellerProduct.Tax = tmpItemModel_index
                //newSellerProduct.IsMarket = tmpItemModel_index

                newSellerItem.CategoryID = tmpItemModel_index.API_Item_CategoryID;
                newSellerItem.DescTW = tmpItemModel_index.API_Item_DescTW;
                newSellerItem.Sdesc = tmpItemModel_index.API_Item_Sdesc;
                newSellerItem.Spechead = tmpItemModel_index.API_Item_Spechead;
                newSellerItem.Model = tmpItemModel_index.API_Item_Model;
                newSellerItem.ItemPackage = tmpItemModel_index.API_Item_ItemPackage;
                newSellerItem.PriceCash = tmpItemModel_index.API_Item_PriceCash;
                newSellerItem.MarketPrice = tmpItemModel_index.API_Item_MarketPrice;
                //newSellerItem.IsLimit = tmpItemModel_index
                
                // ItemStock
                newSellerItem.Inventory = db_Itemstock.Qty;
                newSellerItem.ItemStockQtyReg = db_before.ItemStock.Where(x => x.ProductID == tmpItemModel_index.API_Item_ProductID).Select(r => r.QtyReg).FirstOrDefault();
                newSellerItem.ItemStockFdbcklmt = db_before.ItemStock.Where(x => x.ProductID == tmpItemModel_index.API_Item_ProductID).Select(r => r.Fdbcklmt).FirstOrDefault();
                

                newSellerItem.ShipType = tmpItemModel_index.API_Item_ShipType;
                newSellerItem.DelvDate = tmpItemModel_index.API_Item_DelvDate;
                newSellerItem.DateStart = tmpItemModel_index.API_Item_DateStart;
                newSellerItem.DateEnd = tmpItemModel_index.API_Item_DateEnd;
                newSellerItem.DateDel = tmpItemModel_index.API_Item_DateDel;
                newSellerItem.ItemImages = tmpItemModel_index.API_Item_ItemImages;
                // Item 創建時存入
                newSellerItem.PropertyInfos = tmpItemModel_index.API_Product_Property;
                newSellerItem.CreateUser = "61753";
                newSellerItem.UpdateUser = tmpItemModel_index.API_Item_UpdateUser;
                newSellerItem.Name = tmpItemModel_index.API_Item_Name;
                newSellerItem.ItemDesc = tmpItemModel_index.API_Item_ItemDesc;
                newSellerItem.DelvType = (int)TWNewEgg.DB.TWSQLDB.Models.Item.tradestatus.B2C直配;
                //newSellerItem.ProductID = tmpItemModel_index.API_Item_ProductID;
                newSellerItem.PriceCard = tmpItemModel_index.API_Item_PriceCard;
                newSellerItem.ManufactureID = tmpItemModel_index.API_Item_ManufactureID;
                newSellerItem.SellerID = newSellerID;
                newSellerItem.Note = tmpItemModel_index.API_Item_Note;
                
                newSellerItem.PicStart = tmpItemModel_index.API_Item_ItemImages != null ? 1 : 0;
                newSellerItem.PicEnd = tmpItemModel_index.API_Item_ItemImages != null ? tmpItemModel_index.API_Item_ItemImages.Count : 0;                

                newSellerItem.PriceLocalship = tmpItemModel_index.API_Item_PriceLocalship;
                newSellerItem.QtyLimit = tmpItemModel_index.API_Item_QtyLimit;
                newSellerItem.Status = tmpItemModel_index.API_Item_Status;
                newSellerItem.CreateDate = DateTime.Now;
                newSellerItem.Qty = tmpItemModel_index.API_Item_Qty;
                newSellerItem.QtyReg = db_before.Item.Where(x => x.ID == tmpItemModel_index.API_Item_ID).Select(r => r.QtyReg).FirstOrDefault();
                
                newSellerItem.IsNew = tmpItemModel_index.API_Item_IsNew;
                newSellerItem.ItemStockSafeQty = tmpItemModel_index.API_Item_ItemStockSafeQty;
                // 不用填
                //newSellerItem.ItemtempID = tmpItemModel_index
                //newSellerItem.ShipFee = tmpItemModel_index

                // 固定值
                //newSellerItem.Taxfee = tmpItemModel_index
                //newSellerItem.SpecDetail = tmpItemModel_index
                //newSellerItem.SaleType = tmpItemModel_index
                //newSellerItem.PayType = tmpItemModel_index
                //newSellerItem.Layout = tmpItemModel_index
                //newSellerItem.Itemnumber = tmpItemModel_index
                //newSellerItem.Pricesgst = tmpItemModel_index
                //newSellerItem.ServicePrice = tmpItemModel_index
                //newSellerItem.PricehpType1 = tmpItemModel_index
                //newSellerItem.PricehpInst1 = tmpItemModel_index
                //newSellerItem.PricehpType2 = tmpItemModel_index
                //newSellerItem.PricehpInst2 = tmpItemModel_index
                //newSellerItem.Inst0Rate = tmpItemModel_index
                //newSellerItem.RedmfdbckRate = tmpItemModel_index
                //newSellerItem.Coupon = tmpItemModel_index
                //newSellerItem.PriceCoupon = tmpItemModel_index

                //newSellerItem.PriceGlobalship = tmpItemModel_index
                //newSellerItem.SafeQty = tmpItemModel_index
                //newSellerItem.LimitRule = tmpItemModel_index
                //newSellerItem.QtyReg = tmpItemModel_index
                //newSellerItem.PhotoName = tmpItemModel_index
                //newSellerItem.HtmlName = tmpItemModel_index
                //newSellerItem.ShowOrder = tmpItemModel_index
                //newSellerItem.Class = tmpItemModel_index
                //newSellerItem.StatusNote = tmpItemModel_index
                //newSellerItem.StatusDate = tmpItemModel_index
                //newSellerItem.Updated = tmpItemModel_index
                //newSellerItem.UpdateDate = tmpItemModel_index

                newSellerItemModel.ProductsInfo.Add(newSellerProduct);
                newSellerItemModel.ItemInfos.Add(newSellerItem);
            }

            return newSellerItemModel;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sellerID"></param>
        /// <param name="itemID"></param>
        /// <returns></returns>
        private APIItemModel QueryItemModel(int sellerID, int itemID)
        {
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            DateTime startDate = DateTime.Parse("2015-03-31");
            var itemList = (from i in db.Item
                            join p in db.Product on i.ProductID equals p.ID
                            join s in db.ItemStock on p.ID equals s.ProductID
                            join displayprice in db.ItemDisplayPrice on i.ID equals displayprice.ItemID into tmp
                            from ds in tmp.DefaultIfEmpty()
                            select new APIItemModel
                            {
                                API_Product_BarCode = p.BarCode,
                                //API_Product_CreateDate = p.CreateDate,
                                API_Product_CreateUser = p.CreateUser,
                                API_Product_DelvType = p.DelvType,
                                API_Product_Description = p.Description,
                                API_Product_DescriptionTW = p.DescriptionTW,
                                API_Product_Height = p.Height,
                                //API_Product_InvoiceType = p.InvoiceType,
                                //API_Product_IsMarket = p.IsMarket,
                                API_Product_IsShipDanger = p.IsShipDanger,
                                API_Product_Length = p.Length,
                                API_Product_ManufacturerID = p.ManufactureID,
                                API_Product_MenufacturePartNum = p.MenufacturePartNum,
                                API_Product_Model = p.Model,
                                API_Product_NameTW = p.NameTW,
                                API_Product_Note = p.Note,
                                //API_Product_PicEnd = p.PicEnd,
                                //API_Product_PicStart = p.PicStart,
                                API_Product_productName = p.Name,
                                //API_Product_SaleType = p.SaleType,
                                API_Product_SellerID = p.SellerID,
                                API_Product_SellerProductID = p.SellerProductID,
                                //API_Product_SourceTable = p.SourceTable,
                                //API_Product_Spec = p.SPEC,
                                API_Product_Status = p.Status,
                                //API_Product_Tax = p.Tax,
                                API_Product_UPC = p.UPC,
                                API_Product_Updated = p.Updated,
                                API_Product_UpdateDate = p.UpdateDate,
                                API_Product_UpdateUser = p.UpdateUser,
                                API_Product_Warranty = p.Warranty,
                                API_Product_Weight = p.Weight,
                                API_Product_Width = p.Width,
                                API_Item_Name = i.Name,
                                API_Item_DelvType = i.DelvType,
                                API_Item_ItemDesc = i.DescTW,
                                API_Item_ManufactureID = i.ManufactureID,
                                API_Item_Note = i.Note,
                                API_Item_PicEnd = i.PicEnd,
                                API_Item_PriceCard = i.PriceCard,
                                API_Item_PicStart = i.PicStart,
                                API_Item_ProductID = i.ProductID,
                                API_Item_SellerID = i.SellerID,
                                API_Item_CategoryID = i.CategoryID,
                                API_Item_DateStart = i.DateStart,
                                API_Item_DateEnd = i.DateEnd,                                
                                API_Item_DateDel = i.DateDel,
                                API_Item_DelvDate = i.DelvDate,
                                API_Item_DescTW = i.DescTW,
                                API_Item_Inventory = s.Qty - s.QtyReg,
                                //API_Item_IsLimit = i.LimitRule,
                                API_Item_ItemPackage = i.ItemPackage,
                                API_Item_MarketPrice = i.MarketPrice,
                                API_Item_Model = i.Model,
                                API_Item_PriceCash = i.PriceCash,
                                API_Item_Sdesc = i.Sdesc,
                                API_Item_ShipType = i.ShipType,
                                API_Item_Spechead = i.Spechead,
                                API_Item_PriceLocalship = i.PriceLocalship,
                                API_Item_Qty = i.Qty,
                                API_Item_QtyLimit = i.QtyLimit,
                                API_Item_UpdateUser = i.UpdateUser,
                                API_Product_IsChokingDanger = p.IsChokingDanger,
                                API_Product_Cost = p.Cost,
                                API_Product_Is18 = p.Is18,
                                API_Item_ID = i.ID,
                                API_Record_IndustryID = i.CategoryID,
                                API_Item_Status = i.Status,
                                API_Item_IsNew = i.IsNew,

                                API_Item_ItemStockSafeQty = s.SafeQty,
                                API_Itemdisplayprice_GrossMargin = ds.ItemProfitPercent.HasValue ? ds.ItemProfitPercent.Value * 100 : ds.ItemProfitPercent.Value,
                            }).Where(x => x.API_Item_ID == itemID && x.API_Item_Status != 99).Distinct().FirstOrDefault();
            itemList.API_Item_DateStart = startDate;
            itemList.API_Item_DateEnd = startDate.AddYears(10);
            itemList.API_Item_DateDel = startDate.AddYears(10).AddDays(1);
            return itemList;
        }

        private string CategoryID2Title(int categoryID)
        {
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            string title = db.Category.Where(x => x.ID == categoryID).Select(x => x.Description).FirstOrDefault();
            return title;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sellerID"></param>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public ActionResponse<APIItemModel> SearchCreatedItem(int sellerID, int itemID)
        {
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();

            ActionResponse<APIItemModel> result = new ActionResponse<APIItemModel>();

            APIItemModel item = new APIItemModel();

            item = QueryItemModel(sellerID, itemID);

            if (item != null)
            {
                // 商品圖
                string pid = item.API_Item_ID.ToString("00000000");
                string pidf4 = pid.Substring(0, 4);
                string pidl4 = pid.Substring(4, 4);
                item.API_Item_ItemImages = new List<string>();
                for (int picIndex = 1; picIndex <= item.API_Item_PicEnd; picIndex++)
                {
                    item.API_Item_ItemImages.Add(this.images + "/Item/" + pidf4 + "/" + pidl4 + "_" + picIndex + "_原始大小.jpg");
                }

                //商品屬性
                API.Service.CategoryPropertyService PropertyService = new Service.CategoryPropertyService();
                item.API_Product_Property = new List<SaveProductProperty>();
                item.API_Product_Property = PropertyService.GetProductProperty(item.API_Item_ProductID).Body;

                // 抓主類別編號 先填入CategoryID
                int tempCategoryID = item.API_Item_CategoryID;
                int tempSubCategoryID = db.Category.Where(x => x.ID == tempCategoryID).Select(x => x.ParentID).FirstOrDefault();
                item.API_Record_IndustryID = db.Category.Where(x => x.ID == tempSubCategoryID).Select(x => x.ParentID).FirstOrDefault();

                // 類別地圖：主類別 > 子類別 > 商品類別
                item.API_Record_CategoryNameMap = CategoryID2Title(item.API_Record_IndustryID) + " > " + CategoryID2Title(tempSubCategoryID) + " > " + CategoryID2Title(tempCategoryID);

                // 製造商ID轉名稱
                int tempManuID = item.API_Item_ManufactureID;
                item.API_Record_ManufacturerName = spdb.Seller_ManufactureInfo.Where(x => x.SN == tempManuID).Select(x => x.ManufactureName).FirstOrDefault();

                // 預設指定賣場開始日期為現在
                //item.API_Item_DateStart = DateTime.Now;

                // 判斷總價化是否有數值，若無則計算帶入初始值
                //if (!item.API_Itemdisplayprice_GrossMargin.HasValue)
                //    item.API_Itemdisplayprice_GrossMargin = decimal.Round(((item.API_Item_PriceCash - item.API_Product_Cost.Value) / item.API_Item_PriceCash) * 100, 2);

                result.Code = (int)ResponseCode.Success;
                result.IsSuccess = true;
                result.Msg = "搜尋成功";
                result.Body = item;
            }
            else
            {
                result.Code = (int)ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "查無商品";
            }

            return result;
        }
    }
}
