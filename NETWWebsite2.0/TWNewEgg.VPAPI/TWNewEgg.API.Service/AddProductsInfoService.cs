using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.API.Models;
using TWNewEgg.DB.TWSELLERPORTALDB.Models;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net;
using LinqToExcel;
using Remotion;
using System.Text.RegularExpressions;
using System.Web;
using System.Transactions;
using log4net;
using log4net.Config;
using System.Threading;
using TWNewEgg.API.Models.Models;

namespace TWNewEgg.API.Service
{
    /// <summary>
    /// Add Products Info (APIService)
    /// <para>Website Page:Create Products(all step)</para>
    /// </summary>  
    public class AddProductsInfoService
    {
        #region 宣告變數

        /// <summary>
        /// 宣告存取建立 Product 後所產生的流水號 ID
        /// </summary>
        private static List<string> serialNumProductID = new List<string>();

        // 需告寫入前台資料的 Service
        private API.Service.TWService changefrontendService = new TWService();

        // 宣告 Log4net 寫入 Log 
        private static ILog log = LogManager.GetLogger(typeof(AddProductsInfoService));

        // 放入需要進行 IPP 總價化更新的 ItemID
        private List<int> updateIPPItemID = new List<int>();

        #endregion

        #region 傳入 Product 資料至 DB

        /// <summary>
        /// Post DB's field(Products Info) 
        /// </summary>
        /// <param name="productsInfo">傳入要建立的ProductInfoList</param>
        /// <returns>回傳bool代表成功或失敗</returns>
        public ActionResponse<bool> PostProductsInfoToDB(List<ProductsInfoResult> productsInfo)
        {
            DB.TWSQLDB.Models.Product frontendProduct = new DB.TWSQLDB.Models.Product();

            ActionResponse<bool> postProductInfoResult = new ActionResponse<bool>();
            serialNumProductID.Clear();
            string productID = string.Empty;
            // 將product寫入DB
            if (productsInfo != null)
            {
                foreach (var productInfo in productsInfo)
                {
                    // 將欄位資料填入，並回傳 DB Model
                    frontendProduct = TransToDBProductModel(productInfo);

                    // 建立 Product 並且回傳 ProductID
                    var productCreateResult = CreateProductInfo(frontendProduct);

                    switch (productCreateResult.SaveDBResult)
                    {
                        case Models.ResponseCode.Success:
                            {
                                productID = productCreateResult.ResultData;
                                // 建立完 Product 存取產生的流水號 ProductID
                                serialNumProductID.Add(productID);

                                postProductInfoResult.Body = true;
                                postProductInfoResult.Msg = "Product上傳成功！";
                                postProductInfoResult.Code = (int)ResponseCode.Success;
                                postProductInfoResult.IsSuccess = true;
                            }

                            break;
                        case Models.ResponseCode.Error:
                            {
                                postProductInfoResult.Body = false;
                                postProductInfoResult.Msg = productCreateResult.ResultData;
                                postProductInfoResult.Code = (int)ResponseCode.Error;
                                postProductInfoResult.IsSuccess = false;
                            }

                            break;
                    }
                }
            }
            else
            {
                postProductInfoResult.Body = false;
                postProductInfoResult.Msg = "上傳失敗：部分資料無內容(Product)。";
                postProductInfoResult.Code = (int)ResponseCode.Error;
                postProductInfoResult.IsSuccess = false;
            }

            return postProductInfoResult;
        }

        /// <summary>
        /// 建立前台 Product 資料
        /// </summary>
        /// <param name="frontendProduct">要建立的 product 資料，SP 傳入</param>
        /// <returns>回傳是否成功建立 Models.ResponseCode success or error </returns>
        private ItemCreationResult CreateProductInfo(DB.TWSQLDB.Models.Product frontendProduct)
        {
            ItemCreationResult postDBResult = new ItemCreationResult();

            DB.TWSqlDBContext frontenddb = new DB.TWSqlDBContext();
            try
            {
                frontenddb.Product.Add(frontendProduct);
                frontenddb.SaveChanges();

                postDBResult.SaveDBResult = Models.ResponseCode.Success;
                postDBResult.ResultData = frontendProduct.ID.ToString();
            }
            catch (Exception ex)
            {               
                postDBResult.SaveDBResult = Models.ResponseCode.Error;
                postDBResult.ResultData = ex.Message;

                string str = ex.Message;
                try
                {
                    str += ex.InnerException.Message;
                }
                catch
                {
                }

                try
                {
                    str += ex.InnerException.InnerException.Message;
                }
                catch
                {
                }

                try
                {
                    str += ex.InnerException.InnerException.InnerException.Message;
                }
                catch
                {
                }

                try
                {
                    str += ex.InnerException.InnerException.InnerException.InnerException.Message;
                }
                catch
                {
                }

                try
                {
                    str += ex.InnerException.InnerException.InnerException.InnerException.InnerException.Message;
                }
                catch
                {
                }

                log.Error(str);
            }

            return postDBResult;
        }

        /// <summary>
        /// API Model 轉換成 DB Model 
        /// </summary>
        /// <param name="productInfo">傳入要轉換的 API Model</param>
        /// <returns>回傳轉換完成的 DB Model</returns>
        private DB.TWSQLDB.Models.Product TransToDBProductModel(ProductsInfoResult productInfo)
        {
            DB.TWSQLDB.Models.Product transFrontendProduct = new DB.TWSQLDB.Models.Product();

            // 今日日期精準到毫秒
            DateTime dateTimeMillisecond = new DateTime(0001, 01, 01, 01, 01, 01, 01);
            string timeString = dateTimeMillisecond.ToString("yyyy-MM-dd hh:mm:ss fff");
            dateTimeMillisecond = DateTime.Now;

            #region Seller資訊
            transFrontendProduct.SellerID = productInfo.SellerID;
            transFrontendProduct.CreateUser = productInfo.CreateUser;
            // 不給值(因為有更新才給)
            // Product.UpdateUser = x.UpdateUser;
            #endregion

            #region BasicInfo
            transFrontendProduct.SellerProductID = productInfo.SellerProductID;
            transFrontendProduct.MenufacturePartNum = productInfo.MenufacturePartNum;
            transFrontendProduct.UPC = productInfo.UPC;
            transFrontendProduct.Status = productInfo.Status;
            transFrontendProduct.Name = productInfo.productName;
            transFrontendProduct.NameTW = productInfo.NameTW;
            transFrontendProduct.ManufactureID = productInfo.ManufacturerID;

            if (productInfo.Model == null)
            {
                productInfo.Model = "";
            }

            transFrontendProduct.Model = productInfo.Model;
            transFrontendProduct.BarCode = productInfo.BarCode;
            transFrontendProduct.Warranty = productInfo.Warranty;
            #endregion

            #region SpecificInfo
            transFrontendProduct.Length = productInfo.Length;
            transFrontendProduct.Width = productInfo.Width;
            transFrontendProduct.Height = productInfo.Height;
            transFrontendProduct.Weight = productInfo.Weight;
            transFrontendProduct.Description = productInfo.Description;
            transFrontendProduct.DescriptionTW = productInfo.DescriptionTW;
            transFrontendProduct.DelvType = productInfo.DelvType;
            transFrontendProduct.Note = productInfo.Note;
            transFrontendProduct.IsShipDanger = productInfo.IsShipDanger;
            //Product.Is18 = x.Is18;
            //Product.IsChokingDanger = x.IsChokingDanger;
            transFrontendProduct.IsMarket = productInfo.IsMarket;
            transFrontendProduct.PicStart = productInfo.PicStart;
            transFrontendProduct.PicEnd = productInfo.PicEnd;
            #endregion

            #region 固定值
            transFrontendProduct.SourceTable = "SellerPortal";
            transFrontendProduct.InvoiceType = 0;
            transFrontendProduct.SaleType = 0;
            transFrontendProduct.Updated = 0;

            //20140514欄位規則修改
            transFrontendProduct.Cost = productInfo.Cost;

            if (transFrontendProduct.TradeTax == null)
            {
                transFrontendProduct.TradeTax = 0;
            }

            //Product.CreateDate = DateTime.Today;
            transFrontendProduct.CreateDate = dateTimeMillisecond;

            //UpdateDate(更新日期)：不給值(因為有更新才給)
            //Product.UpdateDate = DateTime.Today;
            transFrontendProduct.Tax = 0;
            #endregion

            return transFrontendProduct;
        }

        #endregion

        #region 傳入 Product Detail 資料至 Spec 

        /// <summary>
        /// Post DB's field(Item Info) 
        /// </summary>
        /// <param name="detailInfo">要建立的DetailInfo</param>
        /// <returns>回傳 true or false 代表成功與失敗</returns>
        public ActionResponse<bool> PostDetailInfoToDB(List<DetailInfoResult> detailInfo)
        {
            DB.TWSQLDB.Models.Product frontendProduct = new DB.TWSQLDB.Models.Product();
            DB.TWSqlDBContext frontendSQLdb = new DB.TWSqlDBContext();
            DB.TWSellerPortalDBContext sellerPortaldb = new DB.TWSellerPortalDBContext();
            ActionResponse<bool> postDetailInfoResult = new ActionResponse<bool>();

            //寫入DB的時間設定
            //今日日期精準到毫秒
            DateTime dateTimeMillisecond = new DateTime(0001, 01, 01, 01, 01, 01, 01);
            string timeString = dateTimeMillisecond.ToString("yyyy-MM-dd hh:mm:ss fff");
            dateTimeMillisecond = DateTime.Now;

            Seller_ProductSpec productSpec = new Seller_ProductSpec();

            //將Model數個欄位資料寫入DB
            if (detailInfo != null)
            {
                //count = 1
                foreach (var x in detailInfo)
                {
                    try
                    {
                        #region 新版
                        if (true)
                        {
                            int saveList;
                            string sellerProductID = null;

                            if (x.ProductSpec_Model != null && x.ProductSpec_Model.Count != 0)
                            {
                                for (saveList = 0; saveList < x.ProductSpec_Model.Count / 6; saveList++)
                                {
                                    //寫入DB-Model
                                    #region 寫入DB-Model
                                    //使用者可能輸入不同的字串，將統一字串
                                    if (x.ProductSpec_Model[3 + (saveList * 6)].ToLower() == "model" || x.ProductSpec_Model[3 + (saveList * 6)] == "型號" || x.ProductSpec_Model[3 + (saveList * 6)] == "樣式")
                                    {
                                        productSpec.SellerProductID = x.ProductSpec_Model[0 + (saveList * 6)];
                                        sellerProductID = x.ProductSpec_Model[0 + (saveList * 6)];
                                        //製造商商品編號 Manufacturer Part # / ISBN
                                        productSpec.MenufacturePartNum = x.ProductSpec_Model[1 + (saveList * 6)];
                                        //商品條碼 UPC
                                        productSpec.UPC = x.ProductSpec_Model[2 + (saveList * 6)];
                                        //Model
                                        productSpec.SpecItem = 1;
                                        //Title
                                        productSpec.Title = x.ProductSpec_Model[4 + (saveList * 6)];
                                        //Description
                                        productSpec.Description = x.ProductSpec_Model[5 + (saveList * 6)];

                                        //更新日期與時間(首次創建不需填寫更新時間與更新人)
                                        //ProductSpec.UpdateDate = DateTimeMillisecond;
                                        //ProductSpec.UpdateUserID = x.UserID;
                                        productSpec.InDate = dateTimeMillisecond;
                                        productSpec.InUserID = x.UserID;

                                        //儲存寫入的DB
                                        sellerPortaldb.Seller_ProductSpec.Add(productSpec);
                                        sellerPortaldb.SaveChanges();
                                    }
                                    #endregion
                                }
                            }

                            if (x.ProductSpec_Spec != null && x.ProductSpec_Spec.Count != 0)
                            {
                                for (saveList = 0; saveList < x.ProductSpec_Spec.Count / 6; saveList++)
                                {
                                    //寫入DB-Spec
                                    #region 寫入DB-Spec
                                    //使用者可能輸入不同的字串，將統一字串
                                    if (x.ProductSpec_Spec[3 + (saveList * 6)] == "Spec" || x.ProductSpec_Spec[3 + (saveList * 6)] == "spec" ||
                                        x.ProductSpec_Spec[3 + (saveList * 6)] == "SPEC" || x.ProductSpec_Spec[3 + (saveList * 6)] == "SPEC." ||
                                        x.ProductSpec_Spec[3 + (saveList * 6)] == "Spec." || x.ProductSpec_Spec[3 + (saveList * 6)] == "spec." ||
                                        x.ProductSpec_Spec[3 + (saveList * 6)] == "specification" || x.ProductSpec_Spec[3 + (saveList * 6)] == "Specification" ||
                                        x.ProductSpec_Spec[3 + (saveList * 6)] == "SPECIFICATION" || x.ProductSpec_Spec[3 + (saveList * 6)] == "規格")
                                    {
                                        productSpec.SellerProductID = x.ProductSpec_Spec[0 + (saveList * 6)];
                                        sellerProductID = x.ProductSpec_Spec[0 + (saveList * 6)];
                                        //製造商商品編號 Manufacturer Part # / ISBN
                                        productSpec.MenufacturePartNum = x.ProductSpec_Spec[1 + (saveList * 6)];
                                        //商品條碼 UPC
                                        productSpec.UPC = x.ProductSpec_Spec[2 + (saveList * 6)];
                                        //Model
                                        productSpec.SpecItem = 2;
                                        //Title
                                        productSpec.Title = x.ProductSpec_Spec[4 + (saveList * 6)];
                                        //Description
                                        productSpec.Description = x.ProductSpec_Spec[5 + (saveList * 6)];

                                        //更新日期與時間(首次創建不需填寫更新時間與更新人)
                                        //ProductSpec.UpdateDate = DateTimeMillisecond;
                                        //ProductSpec.UpdateUserID = x.UserID;
                                        productSpec.InDate = dateTimeMillisecond;
                                        productSpec.InUserID = x.UserID;

                                        //儲存寫入的DB
                                        sellerPortaldb.Seller_ProductSpec.Add(productSpec);
                                        sellerPortaldb.SaveChanges();
                                    }
                                    #endregion
                                }
                            }

                            if (x.ProductSpec_Features != null && x.ProductSpec_Features.Count != 0)
                            {
                                for (saveList = 0; saveList < x.ProductSpec_Features.Count / 6; saveList++)
                                {
                                    //寫入DB-Features
                                    #region 寫入DB-Features
                                    //使用者可能輸入不同的字串，將統一字串
                                    if (x.ProductSpec_Features[3 + (saveList * 6)] == "Features" || x.ProductSpec_Features[3 + (saveList * 6)] == "features" ||
                                        x.ProductSpec_Features[3 + (saveList * 6)] == "Feature" || x.ProductSpec_Features[3 + (saveList * 6)] == "feature" ||
                                        x.ProductSpec_Features[3 + (saveList * 6)] == "FRATURES" || x.ProductSpec_Features[3 + (saveList * 6)] == "FRATURE" ||
                                        x.ProductSpec_Features[3 + (saveList * 6)] == "特點" || x.ProductSpec_Features[3 + (saveList * 6)] == "特色" ||
                                        x.ProductSpec_Features[3 + (saveList * 6)] == "特徵")
                                    {
                                        productSpec.SellerProductID = x.ProductSpec_Features[0 + (saveList * 6)];
                                        sellerProductID = x.ProductSpec_Features[0 + (saveList * 6)];
                                        //製造商商品編號 Manufacturer Part # / ISBN
                                        productSpec.MenufacturePartNum = x.ProductSpec_Features[1 + (saveList * 6)];
                                        //商品條碼 UPC
                                        productSpec.UPC = x.ProductSpec_Features[2 + (saveList * 6)];
                                        //Model
                                        productSpec.SpecItem = 3;
                                        //Title
                                        productSpec.Title = x.ProductSpec_Features[4 + (saveList * 6)];
                                        //Description
                                        productSpec.Description = x.ProductSpec_Features[5 + (saveList * 6)];

                                        //更新日期與時間(首次創建不需填寫更新時間與更新人)
                                        //ProductSpec.UpdateDate = DateTimeMillisecond;
                                        //ProductSpec.UpdateUserID = x.UserID;
                                        productSpec.InDate = dateTimeMillisecond;
                                        productSpec.InUserID = x.UserID;

                                        //儲存寫入的DB
                                        sellerPortaldb.Seller_ProductSpec.Add(productSpec);
                                        sellerPortaldb.SaveChanges();
                                    }
                                    #endregion
                                }
                            }

                            if (x.ProductSpec_Size != null && x.ProductSpec_Size.Count != 0)
                            {
                                for (saveList = 0; saveList < x.ProductSpec_Size.Count / 6; saveList++)
                                {
                                    //寫入DB-Size
                                    #region 寫入DB-Size
                                    //使用者可能輸入不同的字串，將統一字串
                                    if (x.ProductSpec_Size[3 + (saveList * 6)] == "Size" || x.ProductSpec_Size[3 + (saveList * 6)] == "size" ||
                                        x.ProductSpec_Size[3 + (saveList * 6)] == "SIZE" || x.ProductSpec_Size[3 + (saveList * 6)] == "尺寸" ||
                                        x.ProductSpec_Size[3 + (saveList * 6)] == "大小" || x.ProductSpec_Size[3 + (saveList * 6)] == "尺碼" ||
                                        x.ProductSpec_Size[3 + (saveList * 6)] == "號" || x.ProductSpec_Size[3 + (saveList * 6)] == "型")
                                    {
                                        productSpec.SellerProductID = x.ProductSpec_Size[0 + (saveList * 6)];
                                        sellerProductID = x.ProductSpec_Size[0 + (saveList * 6)];
                                        //製造商商品編號 Manufacturer Part # / ISBN
                                        productSpec.MenufacturePartNum = x.ProductSpec_Size[1 + (saveList * 6)];
                                        //商品條碼 UPC
                                        productSpec.UPC = x.ProductSpec_Size[2 + (saveList * 6)];
                                        //Model
                                        productSpec.SpecItem = 4;
                                        //Title
                                        productSpec.Title = x.ProductSpec_Size[4 + (saveList * 6)];
                                        //Description
                                        productSpec.Description = x.ProductSpec_Size[5 + (saveList * 6)];

                                        //更新日期與時間(首次創建不需填寫更新時間與更新人)
                                        //ProductSpec.UpdateDate = DateTimeMillisecond;
                                        //ProductSpec.UpdateUserID = x.UserID;
                                        productSpec.InDate = dateTimeMillisecond;
                                        productSpec.InUserID = x.UserID;

                                        //儲存寫入的DB
                                        sellerPortaldb.Seller_ProductSpec.Add(productSpec);
                                        sellerPortaldb.SaveChanges();
                                    }
                                    #endregion
                                }
                            }

                            if (x.productSpecList != null)
                            {
                                foreach (var a in x.productSpecList.HtmlList)
                                {
                                    List<DB.TWSQLDB.Models.Product> sellerProductID_List = frontendSQLdb.Product.Where(z => z.SellerProductID == a.SellerProductID).ToList();
                                    foreach (var b in sellerProductID_List)
                                    {
                                        b.SPEC = a.Html;
                                        b.UpdateDate = dateTimeMillisecond;
                                        b.UpdateUser = x.UserID.ToString();
                                    }

                                    frontendSQLdb.SaveChanges();
                                }
                            }

                            postDetailInfoResult.Body = true;
                            postDetailInfoResult.Msg = "Detail上傳成功";
                            postDetailInfoResult.Code = (int)ResponseCode.Success;
                            postDetailInfoResult.IsSuccess = true;
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex.Message);
                        postDetailInfoResult.Body = false;
                        postDetailInfoResult.Msg = "Detail 上傳發生意外錯誤，請確認內容是否正確及內容過長";
                        postDetailInfoResult.Code = (int)ResponseCode.Error;
                        postDetailInfoResult.IsSuccess = false;

                        // 紀錄 exception 訊息至 Log 內
                        string str = ex.Message;
                        try
                        {
                            str += ex.InnerException.Message;
                        }
                        catch
                        {
                        }

                        try
                        {
                            str += ex.InnerException.InnerException.Message;
                        }
                        catch
                        {
                        }

                        try
                        {
                            str += ex.InnerException.InnerException.InnerException.Message;
                        }
                        catch
                        {
                        }

                        try
                        {
                            str += ex.InnerException.InnerException.InnerException.InnerException.Message;
                        }
                        catch
                        {
                        }

                        try
                        {
                            str += ex.InnerException.InnerException.InnerException.InnerException.InnerException.Message;
                        }
                        catch
                        {
                        }

                        log.Error(str);
                    }
                }
            }
            else
            {
                postDetailInfoResult.Body = false;
                postDetailInfoResult.Msg = "上傳失敗：部分資料無內容(Detail)。";
                postDetailInfoResult.Code = (int)ResponseCode.Error;
                postDetailInfoResult.IsSuccess = false;
            }

            return postDetailInfoResult;
        }

        #endregion

        #region 傳入 Item、ItemStock 資料至 DB

        /// <summary>
        /// Post DB's field(Item Info) 
        /// </summary>
        /// <param name="itemInfos">要建立的 ItemInfos</param>
        /// <returns>回傳 actionResponse Model</returns>
        public ActionResponse<bool> PostItemInfoToDB(List<ItemInfoResult> itemInfos)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            API.Models.Connector conn = new Connector();

            //Seller_ProductDetail所需
            //DB.TWSELLERPORTALDB.Models.Seller_ProductDetail SellerProductDetail = new Seller_ProductDetail();
            //DB.TWSellerPortalDBContext SellerPortaldb = new DB.TWSellerPortalDBContext();

            //將Model數個欄位資料寫入DB
            if (itemInfos != null)
            {
                log.Info("Item 數量: " + itemInfos.Count);
                //2014.12.5 加入product property add by ice
                API.Service.CategoryPropertyService PropertyService = new Service.CategoryPropertyService();
                foreach (var itemInfo in itemInfos)
                {
                    int itemInfoindex = itemInfos.IndexOf(itemInfo);

                    // 取得 Product 流水號 ID
                    //int PID = Convert.ToInt32(serialNumProductID[itemInfoindex]);
                    //DB.TWSQLDB.Models.Product Product = db.Product.Where(z => z.ID == PID).FirstOrDefault();
                    var dbItemInfo = TransTODBItemModel(itemInfo, itemInfoindex);
                    var dbItemStock = TransTODBItemStockModel(itemInfo, itemInfoindex);
                
                    ItemCreationResult itemCreationResult = CreateItemInfo(dbItemInfo, dbItemStock);

                    //save property 2014.12.8 add by ice
                    var dbProductProperty = PropertyService.SaveProductPropertyClick(itemInfo.PropertyInfos, Convert.ToInt32(serialNumProductID[itemInfoindex]));

                    /*存圖*/
                    var postImageResult = PostImageToDB(itemInfo.ItemImages, serialNumProductID[itemInfoindex], itemCreationResult.ResultData);

                    switch (itemCreationResult.SaveDBResult)
                    {
                        case Models.ResponseCode.Success:
                            {
                                if (dbProductProperty.IsSuccess == true)
                                {
                                    result.Msg = "Item上傳成功！" + dbProductProperty.Msg;
                                    result.Code = (int)ResponseCode.Success;
                                    result.IsSuccess = true;
                                }
                                else if (dbProductProperty.IsSuccess == false)
                                {
                                    result.Msg = "Item 屬性上傳失敗。" + dbProductProperty.Msg;
                                    result.Code = (int)ResponseCode.Error;
                                    result.IsSuccess = false;
                                }

                                // 圖片不檢查是否成功儲存 2015.02.25 Jack.C
                                //else
                                //{
                                //    if (postImageResult.IsSuccess == false)
                                //    {
                                //        result.Msg = "Item 圖片上傳失敗。" + postImageResult.Msg;
                                //    }
                                //    else
                                //    {
                                //        result.Msg = "Item 屬性上傳失敗。" + dbProductProperty.Msg;
                                //    }
                                //    result.Code = (int)ResponseCode.Error;
                                //    result.IsSuccess = false;
                                //}
                            }

                            break;

                        case Models.ResponseCode.Error:
                            result.Msg = itemCreationResult.ResultData;
                            result.Code = (int)ResponseCode.Error;
                            result.IsSuccess = false;
                            break;
                    }
                }
            }
            else
            {
                result.Msg = "上傳失敗：部分資料無內容(Item)。";
                result.Code = (int)ResponseCode.Error;
                result.IsSuccess = false;
            }

            return result;
        }

        private DB.TWSQLDB.Models.ItemStock TransTODBItemStockModel(ItemInfoResult itemInfo, int itemInfoindex)
        {
            DB.TWSQLDB.Models.ItemStock frontendItemStock = new DB.TWSQLDB.Models.ItemStock();

            //今日日期(精準到毫秒)
            DateTime dateTimeMillisecond = new DateTime(0001, 01, 01, 01, 01, 01, 01);
            string timeString = dateTimeMillisecond.ToString("yyyy-MM-dd hh:mm:ss fff");
            dateTimeMillisecond = DateTime.Now;

            #region 庫存(直接存，表為Itemstock，非Item > 20140514修改)
            //(直接存)
            frontendItemStock.Qty = itemInfo.Inventory;
            frontendItemStock.ProductID = Convert.ToInt32(serialNumProductID[itemInfoindex]);
            frontendItemStock.CreateUser = itemInfo.CreateUser;
            frontendItemStock.SafeQty = itemInfo.ItemStockSafeQty;
            //不給值(因為有更新才給)
            //Itemstock.UpdateUser = x.UpdateUser;
            frontendItemStock.Updated = 0;
            //日期精準到毫秒
            //Itemstock.CreateDate = DateTime.Today;
            frontendItemStock.CreateDate = dateTimeMillisecond;
            //不給值(因為有更新才給)
            //Itemstock.UpdateDate = DateTime.Today;
            //int ItemstockID = 0;
            //ItemstockID = db.ItemStock.Where(z => z.ProductID == frontendItemStock.ProductID).Select(z => z.ID).FirstOrDefault();

            //if (ItemstockID == 0)
            //{
            //    db.ItemStock.Add(frontendItemStock);
            //}
            #endregion

            return frontendItemStock;
        }

        private DB.TWSQLDB.Models.Item TransTODBItemModel(ItemInfoResult itemInfo, int itemInfoindex)
        {
            //今日日期(精準到毫秒)
            DateTime dateTimeMillisecond = new DateTime(0001, 01, 01, 01, 01, 01, 01);
            string timeString = dateTimeMillisecond.ToString("yyyy-MM-dd hh:mm:ss fff");
            dateTimeMillisecond = DateTime.Now;

            DB.TWSQLDB.Models.Item frontendItem = new DB.TWSQLDB.Models.Item();

            #region Seller資訊
            frontendItem.CreateUser = itemInfo.CreateUser;
            // 不給值(因為有更新才給)
            // Item.UpdateUser = x.UpdateUser;
            frontendItem.SellerID = itemInfo.SellerID;
            #endregion

            #region ManageItems
            frontendItem.CategoryID = itemInfo.CategoryID;
            #endregion

            #region OtherInfo
            //add by Thisway(ice修改欄位02/24)
            frontendItem.Name = itemInfo.Name;

            frontendItem.DescTW = itemInfo.DescTW;
            frontendItem.Sdesc = itemInfo.Sdesc;
            frontendItem.Spechead = itemInfo.Spechead;

            if (itemInfo.Model == null)
            {
                itemInfo.Model = "";
            }

            frontendItem.Model = itemInfo.Model;
            frontendItem.DelvDate = itemInfo.DelvDate;
            frontendItem.PriceCash = itemInfo.PriceCash;
            frontendItem.MarketPrice = itemInfo.MarketPrice;
            frontendItem.ItemPackage = itemInfo.ItemPackage;
            frontendItem.ShipType = itemInfo.ShipType;

            // 2014.09.09  商品成色，紀錄 新品 或 福利品 add by jack
            frontendItem.IsNew = itemInfo.IsNew;
            /*
            switch (itemInfo.Condition)
            {
                default:
                case 0:
                    frontendItem.IsNew = "Y";
                    break;
                case 1:
                    frontendItem.IsNew = "N";
                    break;                 
            }
             */

            //限量數量(20140514新增)
            frontendItem.Qty = itemInfo.Qty;
            //限購數量(20140514新增)
            frontendItem.QtyLimit = itemInfo.QtyLimit;

            frontendItem.DateStart = itemInfo.DateStart;
            frontendItem.DateEnd = itemInfo.DateEnd;
            frontendItem.Name = itemInfo.Name;
            frontendItem.ItemDesc = itemInfo.ItemDesc;
            frontendItem.DelvType = itemInfo.DelvType;
            frontendItem.ProductID = Convert.ToInt32(serialNumProductID[itemInfoindex]);
            frontendItem.PriceCard = itemInfo.PriceCard;
            frontendItem.ManufactureID = itemInfo.ManufactureID;

            if (itemInfo.Note == null)
            {
                itemInfo.Note = "";
            }

            frontendItem.Note = itemInfo.Note;
            frontendItem.PicStart = itemInfo.PicStart;
            frontendItem.PicEnd = itemInfo.PicEnd;
            frontendItem.DateDel = itemInfo.DateDel;
            #endregion

            #region 固定值
            frontendItem.Taxfee = 0;
            frontendItem.SpecDetail = "";
            frontendItem.SaleType = 1;
            frontendItem.PayType = 0;
            frontendItem.Layout = 0;
            frontendItem.Itemnumber = "";
            frontendItem.Pricesgst = 0;
            frontendItem.ServicePrice = 0;
            frontendItem.PricehpType1 = 0;
            frontendItem.PricehpInst1 = 0;
            frontendItem.PricehpType2 = 0;
            frontendItem.PricehpInst2 = 0;
            frontendItem.Inst0Rate = 0;
            frontendItem.RedmfdbckRate = 0;
            frontendItem.Coupon = "0";
            frontendItem.PriceCoupon = 0;

            //商品單一上傳有運費(本地運費)、商品批次上傳沒有，在sellerportal加
            //Item.PriceLocalship = 0;
            //add by Jack.C 由 SP ship 填入此欄位
            frontendItem.PriceLocalship = itemInfo.PriceLocalship;

            frontendItem.PriceGlobalship = 0;
            // 供賣場限量檢查使用，預設值填 0
            frontendItem.SafeQty = 0;

            //限購數量，商品單一上傳有、批次上傳沒有，在sellerportal加 (不需要為0)
            ////Item.QtyLimit = 0;

            frontendItem.LimitRule = "";
            frontendItem.QtyReg = 0;
            frontendItem.PhotoName = "";
            frontendItem.HtmlName = "";
            frontendItem.ShowOrder = 0;
            //2014.06.23 更改為預設先開啟隱形賣場，供 User 能夠預覽賣場。
            //2014.06.30 修改回來為 0，因為需要與前台討論
            //frontendItem.ShowOrder = -1;
            frontendItem.Class = 1;
            frontendItem.Status = 1;
            //2014.06.23 更改為開啟賣場，供 User 能夠預覽賣場
            //2014.06.30 修改回來為 0，因為需要與前台討論
            //frontendItem.Status = 0;
            frontendItem.StatusNote = "";

            //日期精準到毫秒
            //Item.StatusDate = DateTime.Today;
            //Item.CreateDate = DateTime.Today;
            frontendItem.StatusDate = dateTimeMillisecond;
            frontendItem.CreateDate = dateTimeMillisecond;

            frontendItem.Updated = 0;
            //UpdateDate(更新日期)：不給值(因為有更新才給)
            //Item.UpdateDate = DateTime.Today;
            #endregion

            #region Seller_ProductDetail 資訊 (目前不用寫入)
            /*
                        SellerProductDetail.SellerID = itemInfo.SellerID;
                        SellerProductDetail.ProductID = itemInfo.ProductID;
                        SellerProductDetail.UPC = Product.UPC;
                        SellerProductDetail.SellerProductID = Product.SellerProductID;
                        SellerProductDetail.ManufactureID = itemInfo.ManufactureID;
                        SellerProductDetail.ManufacturePartNum = Product.MenufacturePartNum;
                        SellerProductDetail.Status = "N";
                        SellerProductDetail.Condition = itemInfo.Condition;
                        SellerProductDetail.ShipType = itemInfo.ShipType;
                        SellerProductDetail.Qty = itemInfo.Inventory;
                        SellerProductDetail.QtyReg = 0;
                        SellerProductDetail.SafeQty = 0;
                        SellerProductDetail.InUserID = Convert.ToInt32(itemInfo.CreateUser);
                        SellerProductDetail.InDate = DateTimeMillisecond;
                        SellerProductDetail.UpdateUserID = Convert.ToInt32(itemInfo.CreateUser);
                        SellerProductDetail.UpdateDate = DateTimeMillisecond;

                        SellerPortaldb.Seller_ProductDetail.Add(SellerProductDetail);
                        */
            #endregion

            return frontendItem;
        }

        private ItemCreationResult CreateItemInfo(DB.TWSQLDB.Models.Item frontendItem, DB.TWSQLDB.Models.ItemStock frontendItemStock)
        {
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            ItemCreationResult saveResult = new ItemCreationResult();

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

                // 將要更新總價化的 ID 存入
                updateIPPItemID.Add(frontendItem.ID);
                log.Info("ItemID: " + frontendItem.ID + ", Create User: " + frontendItem.CreateUser);
                saveResult.SaveDBResult = Models.ResponseCode.Success;
                saveResult.ResultData = frontendItem.ID.ToString();
            }
            catch (Exception ex)
            {              
                saveResult.SaveDBResult = Models.ResponseCode.Error;
                saveResult.ResultData = ex.ToString();
                string str = ex.Message;

                try
                {
                    str += ex.InnerException.Message;
                }
                catch
                {
                }

                try
                {
                    str += ex.InnerException.InnerException.Message;
                }
                catch
                {
                }

                try
                {
                    str += ex.InnerException.InnerException.InnerException.Message;
                }
                catch
                {
                }

                try
                {
                    str += ex.InnerException.InnerException.InnerException.InnerException.Message;
                }
                catch
                {
                }

                try
                {
                    str += ex.InnerException.InnerException.InnerException.InnerException.InnerException.Message;
                }
                catch
                {
                }

                log.Error(str);
            }

            return saveResult;
        }

        #endregion
       
        #region 圖片處理

        /// <summary>
        /// Post DB's field(Product Picture) 
        /// </summary>
        /// <param name="picturesURL">Pictures URL</param>
        /// <param name="productID">Product ID</param>
        /// <param name="itemID">Item ID</param>
        /// <returns>回傳 True or False 代表成功或失敗</returns>
        public ActionResponse<bool> PostImageToDB(List<string> picturesURL, string productID, string itemID)
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
                        string productFilePath = AppDomain.CurrentDomain.BaseDirectory + "pic\\product\\{0}\\{1}_{2}_{3}.jpg";
                        string path = string.Format(productFilePath, productID.Substring(0, 4), productID.Substring(4, 4), picCount, "640");
                        string itemFilePath = AppDomain.CurrentDomain.BaseDirectory + "pic\\item\\{0}\\{1}_{2}_{3}.jpg";
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
                            client.DownloadFile(picURL, AppDomain.CurrentDomain.BaseDirectory + "pic\\product\\" + productID.Substring(0, 4) + "\\" + productID.Substring(4, 4) + "_" + picCount + "_" + "原始大小.jpg");

                            client.DownloadFile(picURL, AppDomain.CurrentDomain.BaseDirectory + "pic\\item\\" + itemID.Substring(0, 4) + "\\" + itemID.Substring(4, 4) + "_" + picCount + "_" + "原始大小.jpg");
                        }
                        catch
                        {
                            continue;
                        }

                        //使用以下位置及檔名儲存圖片
                        string srcproductImagePath = AppDomain.CurrentDomain.BaseDirectory + "pic\\product\\" + productID.Substring(0, 4) + "\\" + productID.Substring(4, 4) + "_" + picCount + "_" + "原始大小.jpg";

                        string srcItemImagePath = AppDomain.CurrentDomain.BaseDirectory + "pic\\item\\" + itemID.Substring(0, 4) + "\\" + itemID.Substring(4, 4) + "_" + picCount + "_" + "原始大小.jpg";

                        //使用product的原始圖片做縮圖，丟往第二步
                        tempImageURL.Add(srcproductImagePath);

                        //client.DownloadFile(x , @"D:\pic\");
                        picCount++;
                    }

                    // 第二步 使用/pic/item和Product 的圖做縮圖存到 Pic/pic/item&Product Jack.W.Wu mod 0530
                    int saveCount = 1;
                    foreach (var url in tempImageURL)
                    {
                        SaveThumbPicWidth(url, 640, productID, itemID, saveCount);
                        SaveThumbPicWidth(url, 300, productID, itemID, saveCount);
                        SaveThumbPicWidth(url, 125, productID, itemID, saveCount);
                        SaveThumbPicWidth(url, 60, productID, itemID, saveCount);
                        saveCount++;
                    }

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
                log.Error(ex.Message);
                //顯示DB寫入狀態(Detail Info)
                queryResult.Body = false;
                queryResult.Msg = "例外錯誤發生，" + ex.Message;
                queryResult.Code = (int)ResponseCode.Error;
                queryResult.IsSuccess = false;
            }
            
            return queryResult;
        }

        /// <summary>
        /// 儲存圖片並進行縮圖
        /// </summary>
        /// <param name="srcImagePath">圖片來源URL</param>
        /// <param name="widthMaxPix">最大寬度Pix</param>
        /// <param name="productID">ProductID</param>
        /// <param name="itemID">ItemID</param>
        /// <param name="picCount">紀錄第幾張圖</param>
        public static void SaveThumbPicWidth(string srcImagePath, int widthMaxPix, string productID, string itemID, int picCount)
        {
            using (FileStream fs = new FileStream(srcImagePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                //取得原始圖片
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(fs);

                //圖片寬高
                int imgWidth = bitmap.Width;
                int imgHeight = bitmap.Height;
                // 計算維持比例的縮圖大小
                int[] thumbnailScaleWidth = GetThumbPic_WidthAndHeight(bitmap, widthMaxPix);
                int afterImgWidth = thumbnailScaleWidth[0];
                int afterImgHeight = thumbnailScaleWidth[1];

                // 產生縮圖
                using (var bmp = new Bitmap(afterImgWidth, afterImgHeight))
                {
                    using (var gr = Graphics.FromImage(bmp))
                    {
                        gr.CompositingQuality = CompositingQuality.HighSpeed;
                        gr.SmoothingMode = SmoothingMode.HighSpeed;
                        gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        gr.DrawImage(bitmap, new Rectangle(0, 0, afterImgWidth, afterImgHeight), 0, 0, imgWidth, imgHeight, GraphicsUnit.Pixel);

                        string productImagePath = AppDomain.CurrentDomain.BaseDirectory + "pic\\pic\\product\\" + productID.Substring(0, 4) + "\\" + productID.Substring(4, 4) + "_" + picCount + "_" + widthMaxPix + ".jpg";
                        string itemImagePath = AppDomain.CurrentDomain.BaseDirectory + "pic\\pic\\item\\" + itemID.Substring(0, 4) + "\\" + itemID.Substring(4, 4) + "_" + picCount + "_" + widthMaxPix + ".jpg";

                        if (Directory.Exists(productImagePath.Substring(0, productImagePath.LastIndexOf('\\'))) == false || Directory.Exists(itemImagePath.Substring(0, itemImagePath.LastIndexOf('\\'))) == false)
                        {
                            Directory.CreateDirectory(productImagePath.Substring(0, productImagePath.LastIndexOf('\\')));
                            Directory.CreateDirectory(itemImagePath.Substring(0, itemImagePath.LastIndexOf('\\')));
                        }

                        bmp.Save(productImagePath);
                        bmp.Save(itemImagePath);
                    }
                }
            }
        }

        /// <summary>
        /// 執行圖片縮圖判斷
        /// </summary>
        /// <param name="image"> 要縮圖的圖片 </param>
        /// <param name="maxPx"> 縮圖的最大限制 </param>
        /// <returns>得到維持比例的縮圖大小比例</returns>
        public static int[] GetThumbPic_WidthAndHeight(System.Drawing.Image image, int maxPx)
        {
            int fixWidth = 0;
            int fixHeight = 0;

            //如果圖片的寬大於最大值或高大於最大值就往下執行 
            if (image.Width != maxPx || image.Height != maxPx)
            {
                //圖片的寬大於圖片的高 
                if (image.Width > image.Height)
                {
                    fixHeight = Convert.ToInt32((Convert.ToDouble(maxPx) / Convert.ToDouble(image.Width)) * Convert.ToDouble(image.Height));
                    //設定修改後的圖高 
                    fixWidth = maxPx;
                }
                else
                {
                    fixWidth = Convert.ToInt32((Convert.ToDouble(maxPx) / Convert.ToDouble(image.Height)) * Convert.ToDouble(image.Width));
                    //設定修改後的圖寬 
                    fixHeight = maxPx;
                }
            }
            else
            {
                //圖片沒有超過設定值，不執行縮圖 
                fixHeight = image.Height;

                fixWidth = image.Width;
            }

            int[] fixWidthAndfixHeight = { fixWidth, fixHeight };

            return fixWidthAndfixHeight;
        }

        #endregion

        #region 取得傭金費率

        /// <summary>
        /// 取得傭金費率
        /// </summary>
        /// <param name="sellerID"> SellerID </param>
        /// <param name="categoryID">Category </param>
        /// <returns> 回傳費率 </returns>
        public ActionResponse<decimal> GetCommision(int sellerID, int categoryID)
        {
            DB.TWSellerPortalDBContext db = new DB.TWSellerPortalDBContext();
            ActionResponse<decimal> result = new ActionResponse<decimal>();

            if (categoryID != 0)
            {
                result.Body = db.Seller_Charge.Where(x => x.SellerID == sellerID && x.CategoryID == categoryID).Select(x => x.Commission).FirstOrDefault();
                result.IsSuccess = true;
            }

            return result;
        }

        #endregion

        #region 建立商品

        /// <summary>
        /// 供 SP 單一創建商品使用，加入 RollBack 機制
        /// </summary>
        /// <param name="sellerportalCreationItemInfos">SP 整個創建商品的資訊</param>
        /// <returns> Msg 傳回商品創建成功或失敗，Body 放入哪個步驟成功或失敗</returns>
        public ActionResponse<Dictionary<string, bool>> SPItemCreation(SPItemCreation sellerportalCreationItemInfos)
        {
            // 參數宣告
            API.Models.ActionResponse<Dictionary<string, bool>> queryResult = new ActionResponse<Dictionary<string, bool>>();
            List<DB.TWSQLDB.Models.Product> listProduct = new List<DB.TWSQLDB.Models.Product>();
            Dictionary<string,bool> updateIPPPriceResult = new Dictionary<string,bool>();
            Dictionary<string, bool> updateProductXMLResult = new Dictionary<string, bool>();
            Dictionary<string, bool> creationResult = new Dictionary<string, bool>();
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();

            bool isAddProductInfoSuccess = false;
            bool isAddItemInfoSuccess = false;

            // Transcation 宣告，防止失敗資料寫入 DB
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    var addProductInfo = this.PostProductsInfoToDB(sellerportalCreationItemInfos.ProductsInfo);
                    isAddProductInfoSuccess = addProductInfo.IsSuccess;

                    var addItemInfo = this.PostItemInfoToDB(sellerportalCreationItemInfos.ItemInfos);
                    isAddItemInfoSuccess = addItemInfo.IsSuccess;

                    // product 與 Item 都建立成功才 complete
                    if (isAddProductInfoSuccess == true && isAddItemInfoSuccess == true)
                    {
                        scope.Complete(); //TransactionScope 結束

                        queryResult.IsSuccess = true;
                        queryResult.Code = (int)ResponseCode.Success;

                        //在 Msg 內，放入 ItemID 供後續瀏覽賣場需求使用(因為只有單一創建才有頁面可供點選瀏覽)
                        if (updateIPPItemID.Count == 1)
                        {
                            foreach (var itemID in updateIPPItemID)
                            {
                                queryResult.Msg = "商品 建立成功。" + "ItemID = " + itemID;
                            }
                        }
                    }
                    else
                    {
                        string resultMsg = "";

                        if (addProductInfo.Code == 1)
                        {
                            resultMsg += addProductInfo.Msg + ", ";
                        }

                        if (addItemInfo.Code == 1)
                        {
                            resultMsg += addItemInfo.Msg;
                        }

                        queryResult.IsSuccess = false;
                        queryResult.Msg = resultMsg;
                        queryResult.Code = (int)ResponseCode.Error;

                        return queryResult;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    queryResult.IsSuccess = false;
                    queryResult.Code = (int)ResponseCode.Error;
                    queryResult.Msg = "例外發生: " + ex.Message;
                    queryResult.Body = null;

                    return queryResult;
                }
            }

            // Item 與 Product 有成功創建，才進行其他 Service 呼叫
            if (queryResult.IsSuccess == true)
            {
                // 進行 IPP 總價化程式呼叫
                updateIPPPriceResult = UpdateIPPPrice();
                // product xml 更新呼叫
                updateProductXMLResult = UpdateXMLProperty();

                // IPP price update service result  
                if (updateIPPPriceResult.Count() != 0)
                {
                    queryResult.Code = (int)ResponseCode.IPPUpdateFailed;

                    foreach (var updatepriceResult in updateIPPPriceResult)
                    {
                        creationResult.Add(updatepriceResult.Key, updatepriceResult.Value);
                    }
                }

                // product xml update service result
                if (updateProductXMLResult.Count() != 0)
                {
                    queryResult.Code = (int)ResponseCode.IPPUpdateFailed;

                    foreach (var updateXMLResult in updateProductXMLResult)
                    {
                        creationResult.Add(updateXMLResult.Key, updateXMLResult.Value);
                    }
                }

                // 無論創建成功或失敗，將建立 Product 及 Item 創建的結果放入 body，只有例外發生 body = null
                creationResult.Add("Product", isAddProductInfoSuccess);
                creationResult.Add("Item", isAddItemInfoSuccess);

                log.Info("商品創建成功，共: " + sellerportalCreationItemInfos.ItemInfos.Count);              
            }

            queryResult.Body = creationResult;

            #region 創建商品，存入 SP productDetail (目前不需要將資料存入 SP ProductDetail 2014.06.13 Jack.C)
            // 目前創建商品，不需要將資料存入 SP ProductDetail 2014.06.13 Jack.C
            //foreach (var itemInfo in spCreateitemInfos.ItemInfos)
            //{
            //    int itemInfoindex = spCreateitemInfos.ItemInfos.IndexOf(itemInfo);
            //    // 取得 Product 流水號 ID
            //    int PID = Convert.ToInt32(serialNumProductID[itemInfoindex]);
            //    DB.TWSQLDB.Models.Product Product = db.Product.Where(z => z.ID == PID).FirstOrDefault();

            //    listProduct.Add(Product);
            //}

            //try
            //{
            //    PostProductDetailInfoToDB(spCreateitemInfos.ItemInfos, listProduct);
            //}
            //catch (Exception spdb)
            //{
            //    queryResult.Msg += " ProductDetail:" + spdb.ToString();
            //}           
            #endregion

            return queryResult;
        }

        /// <summary>
        /// 進行 Item IPP 總價化更新
        /// </summary>
        /// <returns>回傳一個 List 若數量為0代表總價化更新成功</returns>
        private Dictionary<string, bool> UpdateIPPPrice()
        {
            Dictionary<string, bool> updateIPPPriceResult = new Dictionary<string, bool>();

            List<int> updateIPPPriceFaildID = new List<int>();

            log.Info("Update Price Type:Add - call IPP API before"); //2014.7.8 add log by ice

            updateIPPPriceFaildID = this.changefrontendService.PriceAPI(updateIPPItemID).Body;

            // 創建成功，無例外發生及總價化成功
            if (updateIPPPriceFaildID.Count == 0)
            {              
                log.Info("總價化成功");
            }
            else if (updateIPPPriceFaildID.Count > 0)
            {
                log.Error("總價化失敗");
                
                // 總價化失敗              
                if (updateIPPPriceFaildID.Count > 1)
                {
                    foreach (var updateID in updateIPPPriceFaildID)
                    {
                        updateIPPPriceResult.Add("，價格計算錯誤，新蛋賣場編號: " + updateID, false);
                        log.Error("總價化失敗，新蛋賣場編號: " + updateID);
                    }
                }
                else
                {
                    updateIPPPriceResult.Add("價格計算錯誤，新蛋賣場編號: " + updateIPPPriceFaildID[0], false);
                    log.Error("總價化失敗，新蛋賣場編號: " + updateIPPPriceFaildID[0]);
                }
            }
            return updateIPPPriceResult;
        }

        /// <summary>
        /// 進行前台 XML 更新
        /// </summary>
        /// <returns> 回傳 string 紀錄有無錯誤訊息</returns>
        private Dictionary<string, bool> UpdateXMLProperty()
        {
            Dictionary<string, bool> updateXMLResult = new Dictionary<string, bool>();

            List<int> updateXML_Result = new List<int>();

            log.Info("Update Product XML:Add - call Product XML Update API before"); 

            updateXML_Result = this.changefrontendService.PropertyXMLAPI(serialNumProductID).Body;

            // 創建成功，無例外發生及總價化成功
            if (updateXML_Result.Count == 0)
            {
                log.Info("XML 更新成功");
            }
            else if (updateXML_Result.Count > 0)
            {
                log.Error("XML 更新失敗");

                // Product XML 更新失敗              
                if (updateXML_Result.Count > 1)
                {
                    foreach (var updateID in updateXML_Result)
                    {
                        updateXMLResult.Add("，商品屬性更新錯誤，新蛋商品編號: " + updateID, false);
                        log.Error("XML 更新失敗，新蛋商品編號: " + updateID);
                    }
                }
                else
                {
                    updateXMLResult.Add("商品屬性更新錯誤，新蛋商品編號: " + updateXML_Result[0], false);
                    log.Error("XML 更新失敗，新蛋商品編號: " + updateXML_Result[0]);
                }
            }

            return updateXMLResult;
        }

        #endregion

        #region Post ProductDetail

        /// <summary>
        /// Post Item and Product Info to SellerPortalProductDetail DB
        /// </summary>
        /// <param name="itemInfos">要建立的 ItemInfos</param>
        /// <param name="products">要建立的 ProductInfos</param>
        /// <returns>回傳 True or False 代表建立成功或失敗</returns>
        private bool PostProductDetailInfoToDB(List<ItemInfoResult> itemInfos, List<DB.TWSQLDB.Models.Product> products)
        {
            bool issaveProductDetailSuccess = false;

            DB.TWSELLERPORTALDB.Models.Seller_ProductDetail sellerProductDetail = new Seller_ProductDetail();
            DB.TWSellerPortalDBContext sellerPortaldb = new DB.TWSellerPortalDBContext();
            try
            {
                //今日日期(精準到毫秒)
                DateTime dateTimeMillisecond = new DateTime(0001, 01, 01, 01, 01, 01, 01);
                string timeString = dateTimeMillisecond.ToString("yyyy-MM-dd hh:mm:ss fff");
                dateTimeMillisecond = DateTime.Now;

                foreach (var itemInfo in itemInfos)
                {
                    int itemInfoindex = itemInfos.IndexOf(itemInfo);
                    // 取得 Product 流水號 ID
                    int pID = Convert.ToInt32(serialNumProductID[itemInfoindex]);
                    var product = products.Where(x => x.ID == pID).FirstOrDefault();

                    #region Seller_ProductDetail 資訊

                    sellerProductDetail.SellerID = itemInfo.SellerID;
                    sellerProductDetail.ProductID = itemInfo.ProductID;
                    sellerProductDetail.UPC = product.UPC;
                    sellerProductDetail.SellerProductID = product.SellerProductID;
                    sellerProductDetail.ManufactureID = itemInfo.ManufactureID;
                    sellerProductDetail.ManufacturePartNum = product.MenufacturePartNum;
                    sellerProductDetail.Status = "N";
                    //sellerProductDetail.Condition = itemInfo.Condition;
                    sellerProductDetail.ShipType = itemInfo.ShipType;
                    sellerProductDetail.Qty = itemInfo.Inventory;
                    sellerProductDetail.QtyReg = 0;
                    sellerProductDetail.SafeQty = 0;
                    sellerProductDetail.InUserID = Convert.ToInt32(itemInfo.CreateUser);
                    sellerProductDetail.InDate = dateTimeMillisecond;
                    sellerProductDetail.UpdateUserID = Convert.ToInt32(itemInfo.CreateUser);
                    sellerProductDetail.UpdateDate = dateTimeMillisecond;

                    sellerPortaldb.Seller_ProductDetail.Add(sellerProductDetail);
                    #endregion
                }

                sellerPortaldb.SaveChanges();

                issaveProductDetailSuccess = true;
            }
            catch (Exception)
            {
                issaveProductDetailSuccess = false;
            }

            return issaveProductDetailSuccess;
        }

        #endregion

        #region 批次上傳商品

        #region 批次上傳宣告變數

        //API Model 三個
        private List<ProductsInfoResult> batchUploadProductsInfo = new List<ProductsInfoResult>();
        private List<ItemInfoResult> batchUploadItemsInfo = new List<ItemInfoResult>();
        private List<DetailInfoResult> batchUploadDetailInfo = new List<DetailInfoResult>();
        private List<Manufacturer> manufacturerPost = new List<Manufacturer>();

        //html
        private List<ProductSpec> productSpecList = new List<ProductSpec>();
        private List<ProductSpec_Model> productSpec_ModelList = new List<ProductSpec_Model>();
        private List<ProductSpec_Spec> productSpec_SpecList = new List<ProductSpec_Spec>();
        private List<ProductSpec_Features> productSpec_FeaturesList = new List<ProductSpec_Features>();
        private List<ProductSpec_Size> productSpec_SizeList = new List<ProductSpec_Size>();

        // Datafeed 欄位檢查時，一併給值。並免重覆判斷與重複呼叫
        // 3. 製造商ID
        private int manufactureID;
        // 17. 商品成色
        //int productCondition;
        // 18. 商品包裝
        private string itemPackage;
        // 26. 運送類型
        private string shipby;
        // 32. 危險物品
        private string isDangerous;

        #endregion
        
        /// <summary>
        /// 開啟 Excel 表格
        /// </summary>
        /// <param name="bathCreateFileInfo">傳入批次創建商品資訊</param>
        /// <returns>回傳 Create Result</returns>
        public BathItemCreateInfoResult ExcelAnalyze(BathItemCreateInfo bathCreateFileInfo)
        {
            Connector conn = new Connector();
            string itemsResult, productsResult, detailResult;
           
            BathItemCreateInfoResult queryResult = new BathItemCreateInfoResult();
            string creationFile = bathCreateFileInfo.CreateFile;
            string extension = bathCreateFileInfo.Extension;
            string filePath = bathCreateFileInfo.FilePath;
            string fileName = bathCreateFileInfo.FileName;
            string accountTypeCode = bathCreateFileInfo.AccountTypeCode;
            int sellerID = bathCreateFileInfo.SellerID;

            string readExcelDataFeedResult = string.Empty;
            string readExcelDetailInfoResult = string.Empty;

            try
            {
                var excel = new ExcelQueryFactory(filePath + fileName + extension);

                if (extension.ToLower() == ".xls")
                {
                    var datafeed = excel.Worksheet<BathCreateItemInfoListDatafeed>("Datafeed");
                    var detailInfo = excel.Worksheet<BathCreateItemInfoListDetailInfo>("DetailInfo");
                    // 檢查 Excel 內，表格是否存在
                    if (datafeed != null)
                    {
                        int detailInfoCount = detailInfo.Skip(2).ToList().Count();

                        // 檢查Excel 表格內資料是否正確
                        readExcelDetailInfoResult = ReadExcelDetailInfo(detailInfo);

                        readExcelDataFeedResult = ReadExcelDatafeed(datafeed, accountTypeCode);

                        // detailInfo 可填可不填
                        if (detailInfoCount != 1)
                        {
                            readExcelDetailInfoResult = ReadExcelDetailInfo(detailInfo);
                        }
                        else
                        {
                            DetailInfoResult detail = new DetailInfoResult();
                            detail.ProductSpec_Features = takeProductSpec_Features;
                            detail.ProductSpec_Model = takeProductSpec_Model;
                            detail.ProductSpec_Size = takeProductSpec_Size;
                            detail.ProductSpec_Spec = takeProductSpec_Spec;
                            detail.UserID = Convert.ToInt32(bathCreateFileInfo.UserID);

                            batchUploadDetailInfo.Add(detail);
                        }

                        if (readExcelDataFeedResult == string.Empty)
                        {
                            //TransformationToAPIModel(datafeed, detailInfo, bathCreateFileInfo);

                            // 連結 API 前，在一次檢查 BatchUploadDetailInfo 串Html的長度，串此 Html 是在檢查錯誤、各別存入API Model 才串的
                            // 因此在傳入 API 之前才檢查 
                            foreach (var specList in batchUploadDetailInfo)
                            {
                                foreach (var htmlLength in specList.productSpecList.HtmlList)
                                {
                                    if (System.Text.Encoding.Default.GetBytes(htmlLength.Html).Length >= 4000)
                                    {
                                        queryResult.QueryResult = BathItemCreateInfoQueryResult.Failed;
                                        queryResult.ResultMessage = "上傳不成功：DetailInfo工作表，" + htmlLength.SellerProductID + "，DetailInfo 總內容不得超過4000字，請檢察修改。";
                                        return queryResult;
                                    }
                                }
                            }
                            
                            productsResult = conn.GetBatchUploadProductsInfo(string.Empty, string.Empty, batchUploadProductsInfo).Msg;
                            itemsResult = conn.GetBatchUploadItemsInfo(string.Empty, string.Empty, batchUploadItemsInfo).Msg;
                            detailResult = conn.GetBatchUploadDetailInfo(string.Empty, string.Empty, batchUploadDetailInfo).Msg;

                            queryResult.QueryResult = BathItemCreateInfoQueryResult.success;
                            queryResult.ResultMessage = productsResult + "\n" + itemsResult + "\n" + detailResult;
                        }
                        else
                        {
                            queryResult.QueryResult = BathItemCreateInfoQueryResult.Failed;
                            // 回傳錯誤兩個工作表錯誤的地方，先判斷 DetailInfo 是否為空則回傳 Datafeed 的錯誤地方
                            queryResult.ResultMessage = readExcelDetailInfoResult == string.Empty ? readExcelDataFeedResult : readExcelDetailInfoResult;
                        }
                    }
                    else
                    {
                        queryResult.QueryResult = BathItemCreateInfoQueryResult.Failed;
                        queryResult.ResultMessage = "上傳不成功：上傳檔案(excel)的工作表名稱有誤，需有Datafeed與DetailInfo工作表，請檢查修改，或重新下載檔案進行填寫創建商品資訊。";
                    }
                }
            }
            catch (Exception)
            {
                queryResult.QueryResult = BathItemCreateInfoQueryResult.Failed;
                queryResult.ResultMessage = "上傳不成功：上傳檔案(excel)的工作表名稱有誤，需有Datafeed與DetailInfo工作表，請檢查修改，或重新下載檔案進行填寫創建商品資訊。";
            }

            return queryResult;
        }

        /// <summary>
        /// 將 Excel 資料，各別存入Model 傳到 API 進行商品創建
        /// </summary>
        /// <param name="datafeed">excel datafeed 工作表</param>
        /// <param name="detailInfo">excel detailInfo 工作表</param>
        /// <param name="bathCreateFileInfo">批次創建商品 user、excel 資料</param>
        private void TransformationToAPIModel(LinqToExcel.Query.ExcelQueryable<BathCreateItemInfoListDatafeed> datafeed, LinqToExcel.Query.ExcelQueryable<BathCreateItemInfoListDetailInfo> detailInfo, BathItemCreateInfo bathCreateFileInfo)
        {
            List<BathCreateItemInfoListDetailInfo> detailInfoList = new List<BathCreateItemInfoListDetailInfo>();
            List<BathCreateItemInfoListDatafeed> datafeedInfo = new List<BathCreateItemInfoListDatafeed>();
            foreach (var detailItem in detailInfo)
            {
                // 避免 Excel 讀取的錯誤，將無資料的列篩選(這邊以SellerPartNumber當作依據)
                if (!string.IsNullOrEmpty(detailItem.SellerPartNumber))
                {
                    detailInfoList.Add(detailItem);
                }
            }

            foreach (var dataFeedItem in datafeed)
            {
                if (!string.IsNullOrEmpty(dataFeedItem.SellerPartNumber))
                {
                    datafeedInfo.Add(dataFeedItem);
                }
            }

            int detailCount = detailInfoList.Skip(2).Count();
            if (detailCount != 1)
            {
                BathDetailModelToAPIModel(detailInfoList, bathCreateFileInfo);
            }
           
            BatchDatafeedModelToAPIModel(datafeedInfo, bathCreateFileInfo);
        }

        private void BatchDatafeedModelToAPIModel(List<BathCreateItemInfoListDatafeed> datafeedInfo, BathItemCreateInfo bathCreateFileInfo)
        {
            ItemInfoResult item = new ItemInfoResult();
            ProductsInfoResult product = new ProductsInfoResult();
            int defauleSubCategory = 0;

            // 取得預設的 Subcategory
            string[] spiltcategoryString = datafeedInfo[1].SubCategoryID.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
            Int32.TryParse(spiltcategoryString[1], out defauleSubCategory);

            datafeedInfo = datafeedInfo.Skip(2).ToList();
            int changeIntOK;
            decimal changeDecimalOK;
            DateTime changeDateTimeOK;

            // 將 Excel 內容 一一分別存入Item 與 Product Model
            foreach (var data in datafeedInfo)
            {
                // 1. SubCategoryID
                if (string.IsNullOrEmpty(data.SubCategoryID))
                { 
                    item.CategoryID = defauleSubCategory;
                }
                // 2. 商家商品編號，DB：Product.SellerProductID→Excel：Seller Part #
                product.SellerProductID = data.SellerPartNumber.Trim();
                // 3. 製造商 ID
                product.ManufacturerID = manufactureID;
                item.ManufactureID = manufactureID;
                // 4. 製造商商品編號，DB：Product.MenufacturePartNum→Excel：Manufacturer Part # / ISBN
                product.MenufacturePartNum = data.ManufacturerPartNumber;
                // 5. 商品條碼，DB：Product.UPC→Excel：UPC
                product.UPC = data.UPC;
                // 6. 商品名稱，DB：Product.Name→Excel：Website Short Title
                product.productName = data.WebsiteShortTitle;
                product.NameTW = product.productName;
                item.Name = product.productName;
                // 7. 商品描述，DB：Product.Name→Excel：Product Description
                product.DescriptionTW = data.ProductDescription;
                product.Description = product.DescriptionTW;
                item.ItemDesc = product.DescriptionTW;
                item.DescTW = product.DescriptionTW;
                // 8. 商品注意事項，DB：Product.Note→Excel：Product Note
                product.Note = data.ProductNote;
                item.Note = product.Note;
                // 9. 商品簡要描述，DB：Item.Sdesc→Excel：Product Short Desc
                item.Sdesc = data.ShortDesc;
                // 10. 商品特色標題，DB：Item.Spechead→Excel：Product Feature Title
                item.Spechead = data.FeatureTitle;
                // 11. 商品型號，DB：Item.Model→Excel：Product Model
                if (string.IsNullOrEmpty(data.Model))
                {
                    item.Model = "";
                    product.Model = item.Model;
                }
                else
                {
                    item.Model = data.Model;
                    product.Model = data.Model;
                }
                // 12. 條碼，DB：Product.BarCode→Excel：Product Bar Code
                product.BarCode = data.BarCode;
                // 13. 長度，DB：Product.Length→Excel：Product Length
                decimal.TryParse(data.Length, out changeDecimalOK);
                product.Length = changeDecimalOK;
                
                // 14. 寬度，DB：Product.Width→Excel：Product Width
                decimal.TryParse(data.Width, out changeDecimalOK);
                product.Width = changeDecimalOK;
                
                // 15. 高度，DB：Product.Height→Excel：Product Height
                decimal.TryParse(data.Height, out changeDecimalOK);
                product.Height = changeDecimalOK;
                
                // 16. 重量，DB：Product.Weight→Excel：Product Weight
                decimal.TryParse(data.Weight, out changeDecimalOK);
                product.Weight = changeDecimalOK;
                
                // 17. 商品成色，DB：Product.Status→Excel：Product Condition
                // 目前沒用到，改放在 Seller_ProductDetail.Condition
                
                // 18. 商品包裝，DB：Item.ItemPackage→Excel：Product Package
                item.ItemPackage = itemPackage;
                
                // 19. 成本(seller非必填、vendor必填)，DB：Product.Cost→Excel：Cost
                if (!string.IsNullOrEmpty(data.ProductCost))
                {
                    decimal.TryParse(data.ProductCost, out changeDecimalOK);
                    product.Cost = changeDecimalOK;
                }
                
                // 20. 網路售價，DB：Item.PriceCash、Item.PriceCard→Excel：Selling Price (怪怪的)
                decimal.TryParse(data.SelleringPrice, out changeDecimalOK);
                item.PriceCash = changeDecimalOK;
                item.PriceCard = changeDecimalOK;

                // 21. 市場建議售價，DB：Item.MarketPrice→Excel：Market Price
                if (!string.IsNullOrEmpty(data.MarketPrice))
                {
                    decimal.TryParse(data.MarketPrice, out changeDecimalOK);
                    item.MarketPrice = changeDecimalOK;
                }
                              
                // 22. 保固期，DB：Product.Warranty→Excel：Warranty
                if (!string.IsNullOrEmpty(data.Warranty))
                {
                    int.TryParse(data.Warranty, out changeIntOK);
                    product.Warranty = changeIntOK;
                }

                // xxx 是否限量(已將excel 欄位刪除無使用)

                // 23. 限量數量，DB：Item.Qty→Excel：Limit Quantity
                if (string.IsNullOrEmpty(data.LimitQuantity))
                {
                    item.Qty = 0;
                }
                else
                {
                    int.TryParse(data.LimitQuantity, out changeIntOK);
                    item.Qty = changeIntOK;
                }

                // 24. 限購數量，DB：Item.QtyLimit→Excel：Quota Quantity
                if (string.IsNullOrEmpty(data.QuotaQuantity))
                {
                    item.QtyLimit = 0;
                }
                else
                {
                    int.TryParse(data.QuotaQuantity, out changeIntOK);
                    item.QtyLimit = changeIntOK;
                }

                // 25. 庫存，DB：Item.Qty || Itemstock.Qty→Excel：Inventory(非限量 || 限量){存入哪個DB會在API做}
                int.TryParse(data.Inventory, out changeIntOK);
                item.Inventory = changeIntOK;

                // 26. 運送類型，DB：Item.ShipType→Excel：Is Ship by Newegg (依據業務邏輯)
                // shipby 由 Datafeed 檢查時給值
                switch (shipby)
                { 
                    case "N":
                        if (bathCreateFileInfo.AccountTypeCode.ToLower() == "s")
                        {
                            item.ShipType = shipby;
                            product.DelvType = 8;
                            item.DelvType = 8;
                        }
                        else if (bathCreateFileInfo.AccountTypeCode.ToLower() == "v")
                        {
                            item.ShipType = shipby;
                            product.DelvType = 9;
                            item.DelvType = 9;
                        }

                        break;
                    case "S":
                        item.ShipType = shipby;
                        product.DelvType = 2;
                        item.DelvType = 2;
                        break;
                    case "V":
                        item.ShipType = shipby;
                        product.DelvType = 7;
                        item.DelvType = 7;
                        break;
                }

                // 27. 到貨天數，DB：Item.DelvDate→Excel：Delv Day
                item.DelvDate = data.DelvDay;

                // 28. 賣場開始日期，DB：Item.DateStart→Excel：Item Start Date
                DateTime.TryParse(data.StartDate, out changeDateTimeOK);
                changeDateTimeOK = DateTime.SpecifyKind(changeDateTimeOK, DateTimeKind.Utc);
                item.DateStart = changeDateTimeOK;

                // 29. 賣場結束日期，DB：Item.DateEnd→Excel：Item End Date
                DateTime.TryParse(data.EndDate, out changeDateTimeOK);
                changeDateTimeOK = DateTime.SpecifyKind(changeDateTimeOK, DateTimeKind.Utc);
                item.DateEnd = changeDateTimeOK;
                //*****DateDel(賣場刪除日期)
                //Item.DateDel > Item.DateEnd;
                //目前設定比DateDel比DateEnd晚一天
                item.DateDel = changeDateTimeOK.AddDays(1);

                // 30. 創建新商品，DB：XXX→Excel：Action
                // 31. 商品圖，DB：XXX→Excel：Item Images
                item.ItemImages = new List<string>();
                string[] strUrlList = Regex.Split(data.Images, ";", RegexOptions.IgnoreCase);

                foreach (var image in strUrlList)
                {
                    item.ItemImages.Add(image);
                }
                
                // 32. 危險物品，DB：Product.IsShipDanger→Excel：Is Dangerous
                product.IsShipDanger = isDangerous;
                // 33. 是否為市場售價，DB：Product.IsMarket→Excel：Is Market Place
                //char(1)、Allow null
                //目前預設為"Y"，需抓值注意！此為必填
                //*****Is Market Place(是否為商品價格)
                product.IsMarket = "Y";

                #region 與使用者輸入的某個欄位相同值(創建商品資料)

                item.SellerID = bathCreateFileInfo.SellerID;

                /*2014.4.24 修改item.PicStart &item.PicEnd 正確寫入資料庫 by thisway  */
                //*****PicStart(產品圖片第一張)
                //*****PicEnd(產品圖片最後一張)
                //Item.PicStart = Product.PicStart;
                if (strUrlList.Length != 0 && strUrlList != null)
                {
                    int picEndcnt = 0;

                    for (int strUrlListcnt = 0; strUrlListcnt <= strUrlList.Length - 1; strUrlListcnt++)
                    {
                        //去掉首尾空格
                        strUrlList[strUrlListcnt] = strUrlList[strUrlListcnt].Trim();

                        if (strUrlList[strUrlListcnt] != "" && strUrlList[strUrlListcnt] != null)
                        {
                            picEndcnt++;
                        }
                    }

                    if (picEndcnt >= 1)
                    {
                        product.PicStart = 1;
                        item.PicStart = 1;
                    }
                    else
                    {
                        product.PicStart = 0;
                        item.PicStart = 0;
                    }

                    product.PicEnd = picEndcnt;
                    item.PicEnd = picEndcnt;
                }
                #endregion

                #region API寫入，放入Product DB(創建商品時給固定的值)
                //*****SourceTable(來源表)
                //Product.SourceTable = "SellerPortal";

                //*****InvoiceType(發票)
                //Product.InvoiceType = 0;

                //*****SaleType(販售型態)
                //Product.SaleType = 0;

                //*****Updated(更新)
                //Product.Updated = 0;

                //*****CreateDate(建立日期)
                //精準到毫秒(今日日期)
                //Product.CreateDate = DateTimeMillisecond;

                //*****UpdateDate(更新日期)：不給值(因為有更新才給)
                //Product.UpdateDate = DateTime.Now;

                //*****Tax(稅)
                //Product.Tax = 0;
                //Item.Taxfee = 0;

                //*****SpecDetail(英文描述)
                //Item.SpecDetail = "";

                //*****SaleType(販售類型)
                //Item.SaleType = 1;

                //*****PayType(付款方式)
                //Item.PayType = 0;

                //*****Layout(安排)
                //Item.Layout = 0;
                //*****Itemnumber(來源賣場編號)
                //Item.Itemnumber = "";

                //*****Pricesgst(建議售價)
                //Item.Pricesgst = 0;

                //*****ServicePrice(服務費)
                //Item.ServicePrice = 0;

                //*****PricehpType1(分期期數)
                //Item.PricehpType1 = 0;

                //*****PricehpInst1(分期利息)
                //Item.PricehpInst1 = 0;

                //*****PricehpType2(分期期數二)
                //Item.PricehpType2 = 0;

                //*****PricehpInst2(分期利息二)
                //Item.PricehpInst2 = 0;

                //*****Inst0Rate(零利率分期期數)
                //Item.Inst0Rate = 0;

                //*****RedmfdbckRate(回饋比例)
                //Item.RedmfdbckRate = 0;

                //*****Coupon(折價券編號)
                //Item.Coupon = "0";

                //*****PriceCoupon(折價券金額)
                //Item.PriceCoupon = 0;

                //*****PriceLocalship(本地運費)
                //商品單一上傳有運費(本地運費)、商品批次上傳沒有，在sellerportal加
                item.PriceLocalship = 0;

                //*****PriceGlobalship(國際運費)
                //Item.PriceGlobalship = 0;

                //*****SafeQty(安全警告數量)
                //Item.SafeQty = 0;

                //*****QtyLimit(限購數量)
                //限購數量，商品單一上傳有、批次上傳沒有，在sellerportal加(不需要為0)
                //Item.QtyLimit = 0;

                //*****LimitRule(限購規則)
                //Item.LimitRule = "";

                //*****QtyReg(已登記數量)
                //Item.QtyReg = 0;

                //*****PhotoName(圖檔名稱)
                //Item.PhotoName = "";

                //*****HtmlName(網頁檔名)
                //Item.HtmlName = "";

                //*****ShowOrder(顯示順序)
                //Item.ShowOrder = 0;

                //*****Class(類型)
                //Item.Class = 1;

                //*****Status(上下架狀態)
                //Item.Status = 1;

                //*****StatusNote(狀態備註)
                //Item.StatusNote = "";

                //*****StatusDate(狀態最後更改時間)
                //Item.StatusDate = DateTime.Now;

                //*****CreateDate(建檔日期)
                //精準到毫秒
                //Item.CreateDate = DateTimeMillisecond;

                //*****Updated(更新)
                //Item.Updated = 0;

                //*****UpdateDate(更新日期)：不給值(因為有更新才給)
                //Item.UpdateDate = DateTime.Now;
                #endregion

                #region SellerPortal自動擷取，放入API
                //*****Product登入者的SellerID
                product.SellerID = bathCreateFileInfo.SellerID;
                //Product.SellerID = 2;

                //*****Product建立者CreateUser
                //Product.CreateUser = HttpContext.Request.QueryString["LoginUserName"];
                product.CreateUser = bathCreateFileInfo.UserID;

                //*****Product更新者UpdateUser(不給值(因為有更新才給))
                //Product.UpdateUser = HttpContext.Request.QueryString["LoginUserName"];

                //*****Item建立者CreateUser
                //Item.CreateUser = HttpContext.Request.QueryString["LoginUserName"];
                item.CreateUser = bathCreateFileInfo.UserID;

                //*****Item更新者UpdateUser(不給值(因為有更新才給))
                //Item.UpdateUser = HttpContext.Request.QueryString["LoginUserName"];
                #endregion

                //---------- 將傳值放入Model ----------
                //!!!!!!!!存入BatchUploadItemsInfo Model
                batchUploadItemsInfo.Add(item);

                //!!!!!!!!存入BatchUploadItemsInfo Model
                batchUploadProductsInfo.Add(product);
            }
        }

        //Detail的Model須包含4個List
        private List<string> takeProductSpec_Model = new List<string>();
        private List<string> takeProductSpec_Spec = new List<string>();
        private List<string> takeProductSpec_Features = new List<string>();
        private List<string> takeProductSpec_Size = new List<string>();       

        private DetailInfoResult detail = new DetailInfoResult();

        private void BathDetailModelToAPIModel(List<BathCreateItemInfoListDetailInfo> detailInfoList, BathItemCreateInfo bathCreateFileInfo)
        {
            detailInfoList = detailInfoList.Skip(2).ToList();

            // 將各 Type 存入 List
            foreach (var item in detailInfoList)
            {
                if (item.Type.ToLower() == "model" || item.Type.ToLower() == "樣式" || item.Type.ToLower() == "型號")
                {
                    takeProductSpec_Model.Add(item.SellerPartNumber);
                    takeProductSpec_Model.Add(item.ManufacturePartNumber);
                    takeProductSpec_Model.Add(item.Type);
                    takeProductSpec_Model.Add(item.UPC);
                    takeProductSpec_Model.Add(item.Title);
                    takeProductSpec_Model.Add(item.Description);
                }

                if (item.Type.ToLower() == "spec." || item.Type.ToLower() == "spec" || item.Type.ToLower() == "specification" || item.Type.ToLower() == "規格")
                {
                    takeProductSpec_Spec.Add(item.SellerPartNumber);
                    takeProductSpec_Spec.Add(item.ManufacturePartNumber);
                    takeProductSpec_Spec.Add(item.Type);
                    takeProductSpec_Spec.Add(item.UPC);
                    takeProductSpec_Spec.Add(item.Title);
                    takeProductSpec_Spec.Add(item.Description);
                }

                if (item.Type.ToLower() == "features" || item.Type.ToLower() == "特點" || item.Type.ToLower() == "特徵" || item.Type.ToLower() == "特色")
                {
                    takeProductSpec_Features.Add(item.SellerPartNumber);
                    takeProductSpec_Features.Add(item.ManufacturePartNumber);
                    takeProductSpec_Features.Add(item.Type);
                    takeProductSpec_Features.Add(item.UPC);
                    takeProductSpec_Features.Add(item.Title);
                    takeProductSpec_Features.Add(item.Description);
                }

                if (item.Type.ToLower() == "size" || item.Type.ToLower() == "大小" || item.Type.ToLower() == "尺寸" || item.Type.ToLower() == "尺碼" || item.Type.ToLower() == "號" || item.Type.ToLower() == "型")
                {
                    takeProductSpec_Size.Add(item.SellerPartNumber);
                    takeProductSpec_Size.Add(item.ManufacturePartNumber);
                    takeProductSpec_Size.Add(item.Type);
                    takeProductSpec_Size.Add(item.UPC);
                    takeProductSpec_Size.Add(item.Title);
                    takeProductSpec_Size.Add(item.Description);
                }
            }
                      
            detail.ProductSpec_Model = takeProductSpec_Model;
            detail.ProductSpec_Spec = takeProductSpec_Spec;
            detail.ProductSpec_Features = takeProductSpec_Features;
            detail.ProductSpec_Size = takeProductSpec_Size;
            detail.UserID = Convert.ToInt32(bathCreateFileInfo.UserID);
            
            batchUploadDetailInfo.Add(detail);

            ModelProcess();
            SpecProcess();
            FeaturesProcess();
            SizeProcess();
            ProductSpecEndProcess();
           
            if (productSpecList.Count > 0)
            {
                batchUploadDetailInfo[0].productSpecList = new ProductSpec();
                batchUploadDetailInfo[0].productSpecList.HtmlList = new List<HtmlSpec>();

                foreach (var psl in productSpecList)
                {
                    batchUploadDetailInfo[0].productSpecList.HtmlList.AddRange(psl.HtmlList);
                }
            }
        }
        
        /// <summary>
        /// 讀取 Excel DetailInfo 工作表
        /// </summary>
        /// <param name="detailInfo">要建立的 DetailInfos</param>
        /// <returns>回傳讀取及檢查的訊息， Empty 代表讀取成功及內容無錯誤</returns>
        private string ReadExcelDetailInfo(LinqToExcel.Query.ExcelQueryable<BathCreateItemInfoListDetailInfo> detailInfo)
        {
            List<BathCreateItemInfoListDetailInfo> createItemDetailInfoList = new List<BathCreateItemInfoListDetailInfo>();
            string readExcelDetailInfoResult = string.Empty;

            foreach (var excelDetailInfo in detailInfo)
            {
                // 避免 Excel 讀取的錯誤，將無資料的列篩選(這邊以SellerPartNumber當作依據)
                if (string.IsNullOrEmpty(excelDetailInfo.SellerPartNumber) == false)
                {
                    createItemDetailInfoList.Add(excelDetailInfo);
                }
            }

            createItemDetailInfoList = createItemDetailInfoList.Skip(2).ToList();
            
            if (createItemDetailInfoList.Count == 0)
            {
                readExcelDetailInfoResult = "上傳不成功：上傳檔案的DetailInfo工作表沒有填寫內容，請檢查修改。";
                return readExcelDetailInfoResult;
            }

            readExcelDetailInfoResult = CheckDetailInfoList(createItemDetailInfoList);

            return readExcelDetailInfoResult;
        }

        /// <summary>
        /// 檢查 Excel DetailInfo 工作表內容
        /// </summary>
        /// <param name="createItemDetailInfoList">要檢查的 DetailInfos</param>
        /// <returns>回傳檢查到的錯誤訊息， Empty 代表無錯誤</returns>
        private string CheckDetailInfoList(List<BathCreateItemInfoListDetailInfo> createItemDetailInfoList)
        {
            string checkDetailResult = string.Empty;

            foreach (var detailInfo in createItemDetailInfoList)
            {
                int detailInfoindex = createItemDetailInfoList.IndexOf(detailInfo);
                detailInfoindex = detailInfoindex + 1;

                // 1. Seller Part# 商家料號，Not Allow Null
                if (string.IsNullOrEmpty(detailInfo.SellerPartNumber))
                {
                    checkDetailResult = string.Format("【第" + detailInfoindex + "筆】" + "上傳失敗：上傳檔案的DetailInfo工作表的Seller Part #欄位為必填，請檢查修改。");
                    return checkDetailResult;
                }
                else
                {
                    if (System.Text.Encoding.Default.GetBytes(detailInfo.SellerPartNumber).Length >= 150)
                    {
                        checkDetailResult = string.Format("【第" + detailInfoindex + "筆】" + "上傳失敗：上傳檔案的DetailInfo工作表的Seller Part #欄位字數限制為150(中文2、英數1)，請檢查修改。");
                        return checkDetailResult;
                    }
                }
                // 2. Manufacturer Part # / ISBN，製造商商品編號，Allow Null
                if (!string.IsNullOrEmpty(detailInfo.ManufacturePartNumber))
                {
                    if (System.Text.Encoding.Default.GetBytes(detailInfo.ManufacturePartNumber).Length >= 15)
                    {
                        checkDetailResult = string.Format("【第" + detailInfoindex + "筆】" + "上傳失敗：上傳檔案的DetailInfo工作表的Manufacturer Part # / ISBN欄位字數限制為15(中文2、英數1)，請檢查修改。");
                        return checkDetailResult;
                    }
                }
                // 3. UPC 條碼，Allow Null
                if (!string.IsNullOrEmpty(detailInfo.UPC))
                {
                    if (System.Text.Encoding.Default.GetBytes(detailInfo.UPC).Length >= 15)
                    {
                        checkDetailResult = string.Format("【第" + detailInfoindex + "筆】" + "上傳失敗：上傳檔案的DetailInfo工作表的UPC欄位字數限制為15(中文2、英數1)，請檢查修改。");
                        return checkDetailResult;
                    }
                }
                // 4. Type 特色標題，Not Allow Null
                if (string.IsNullOrEmpty(detailInfo.Type))
                {
                    checkDetailResult = string.Format("【第" + detailInfoindex + "筆】" + "上傳失敗：上傳檔案的DetailInfo工作表的Type欄位為必填，請檢查修改。");
                    return checkDetailResult;
                }
                // 5. Title 標題，Not Allow Null
                if (string.IsNullOrEmpty(detailInfo.Title))
                {
                    checkDetailResult = string.Format("【第" + detailInfoindex + "筆】" + "上傳失敗：上傳檔案的DetailInfo工作表的Title欄位為必填，請檢查修改。");
                    return checkDetailResult;
                }
                else
                {
                    if (System.Text.Encoding.Default.GetBytes(detailInfo.Title).Length >= 30)
                    {
                        checkDetailResult = string.Format("【第" + detailInfoindex + "筆】" + "上傳失敗：上傳檔案的DetailInfo工作表的的Title欄位字數限制為30(中文2、英數1)，請檢查修改。");
                        return checkDetailResult;
                    }
                }
                // 6. 商品描述
                if (string.IsNullOrEmpty(detailInfo.Description))
                {
                    checkDetailResult = string.Format("【第" + detailInfoindex + "筆】" + "上傳失敗：上傳檔案的DetailInfo工作表的Description欄位為必填，請檢查修改。");
                    return checkDetailResult;
                }
                else
                {
                    if (System.Text.Encoding.Default.GetBytes(detailInfo.Description).Length >= 2000)
                    {
                        checkDetailResult = string.Format("【第" + detailInfoindex + "筆】" + "上傳失敗：上傳檔案的DetailInfo工作表的的Description欄位字數限制為2000(中文2、英數1)，請檢查修改。");
                        return checkDetailResult;
                    }
                }
            }

            return checkDetailResult;
        }

        /// <summary>
        /// 讀取Excel datafeed 工作表，並且檢查個欄位是否正確
        /// </summary>
        /// <param name="datafeed">excel data</param>
        /// <param name="accountType">Seller accounttype </param>
        /// <returns>回傳欄位錯誤訊息，Empty代表正確</returns>
        private string ReadExcelDatafeed(LinqToExcel.Query.ExcelQueryable<BathCreateItemInfoListDatafeed> datafeed, string accountType)
        {
            string checkDatafeedResultMessage = string.Empty;
            List<BathCreateItemInfoListDatafeed> crateItemInfoList = new List<BathCreateItemInfoListDatafeed>();
            BathItemCreateInfoResult bathCreateResult = new BathItemCreateInfoResult();

            // 將 Excel 資料放入 List 
            foreach (var item in datafeed)
            {
                // 先進行篩選，避免整列是空的資料放入處理
                if (!string.IsNullOrEmpty(item.SellerPartNumber))
                {
                    crateItemInfoList.Add(item);
                }
            }

            // 進行個欄位檢查
            checkDatafeedResultMessage = CheckDatafeed(crateItemInfoList, accountType);

            return checkDatafeedResultMessage;
        }

        /// <summary>
        /// 檢查 Datafeed Subcategory 及判斷是否有資料內容
        /// </summary>
        /// <param name="bathCreateInfoList">excel Datafeed 工作表內容</param>
        /// <param name="accountType">Seller accounttype</param>
        /// <returns>欄位的錯誤訊息</returns>
        public string CheckDatafeed(List<BathCreateItemInfoListDatafeed> bathCreateInfoList, string accountType)
        {
            string checkDatafeedResult = string.Empty;
            int defauleSubCategory = 0;

            // 取得預設的 Subcategory
            string[] spiltcategoryString = bathCreateInfoList[1].SubCategoryID.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

            // 1. 檢查SubcategoryID，設定若無取得 Default SubcategoryID 回傳錯誤
            if (Int32.TryParse(spiltcategoryString[1], out defauleSubCategory))
            {
                // 將頭兩個標題的列捨去
                bathCreateInfoList = bathCreateInfoList.Skip(2).ToList();
            }
            else
            {
                // 回傳 SubcategoryID 需要 Value 訊息
                checkDatafeedResult = "上傳不成功：上傳檔案的Datafeed工作表的 '欄位標題' 有誤，請檢查修改，建議重新下載檔案進行填寫創建商品資訊。";
                return checkDatafeedResult;
            }
            // Check 表格是否有資料
            if (bathCreateInfoList.Count == 0)
            {
                checkDatafeedResult = "上傳不成功：上傳檔案的Datafeed工作表的 '內容' 有誤，請檢查修改，建議重新下載檔案進行填寫創建商品資訊。";

                return checkDatafeedResult;
            }

            // 開始檢查工作表各必填欄位內容
            checkDatafeedResult = CheckRequiredfields(bathCreateInfoList, accountType);

            // 若沒 Erro 則會傳回 string.Empty
            return checkDatafeedResult;
        }

        /// <summary>
        /// 檢查Datafeed 各欄位資料是否有誤
        /// </summary>
        /// <param name="bathCreateInfoList">excel Datafeed 工作表內容</param>
        /// <param name="accountType">Seller accounttype</param>
        /// <returns>欄位的錯誤訊息</returns>
        private string CheckRequiredfields(List<BathCreateItemInfoListDatafeed> bathCreateInfoList, string accountType)
        {
            Connector conn = new Connector();
            decimal changeDecimalOK;
            int changeIntOK;

            string isErrorMessage = string.Empty;
            // 檢查各必填欄位是否沒填 or 無法轉換的格式錯誤
            foreach (var dataFeedItem in bathCreateInfoList)
            {   
                int dataFeedItemindex = bathCreateInfoList.IndexOf(dataFeedItem);
                dataFeedItemindex = dataFeedItemindex + 1;
                
                // 2. Seller Part #，DB：Product.SellerProductID→Excel：Seller Part # DB長度為 varchar(150)，Allow null
                if (!string.IsNullOrEmpty(dataFeedItem.SellerPartNumber))
                {   // 若不為 Null 判斷是否大於 150 字
                    if (System.Text.Encoding.Default.GetBytes(dataFeedItem.SellerPartNumber).Length >= 150)
                    {
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "筆】" + "上傳不成功 : 上傳檔案的Datafeed工作表的Seller Part #內容，輸入的內容長度過長，長度須小於150(中文2、英數1)，請檢查修改。");
                        
                        return isErrorMessage;
                    }
                }
                // 3. 製造商 URL，製造商，DB：Product.ManufactureID→Excel：Manufacturer，Not Allow Null
                if (string.IsNullOrEmpty(dataFeedItem.ManufacturerURL))
                {
                    isErrorMessage = string.Format("【第" + dataFeedItemindex + "筆】" + "上傳不成功 : 上傳檔案的Datafeed工作表 ManufactureURL 為必填不能為空值。");
                    
                    return isErrorMessage;
                }
                else
                {
                    SearchDataModel manufacturerSearch = new SearchDataModel();
                    manufacturerSearch.SearchType = SearchType.SearchofficialInfobyURL;
                    string manufacturerURL = dataFeedItem.ManufacturerURL.Trim();
                    manufacturerSearch.KeyWord = manufacturerURL;
                    // 利用 URL 找出 SN
                    List<Manufacturer> manufacturerInfo = conn.SearchManufacturerInfo(manufacturerSearch).Body;

                    if (manufacturerInfo != null)
                    {
                        if (manufacturerInfo[0].SN != null)
                        {
                            // 檢查 Manufacture ID 一併將 Value 給入
                            manufactureID = manufacturerInfo[0].SN;
                            // 要有審核通過才能建立
                            if (manufacturerInfo[0].ManufactureStatus != "Approve")
                            {
                                // 商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                                isErrorMessage = string.Format("【第" + dataFeedItemindex + "筆】" + "上傳不成功：上傳檔案的Datafeed工作表的Manufacturer URL 尚未審核通過，請檢查修改。");
                                
                                return isErrorMessage;
                            }
                        }
                    }
                    else
                    {
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "筆】" + "上傳不成功：上傳檔案的Datafeed工作表的Manufacturer URL 尚未建立或尚未審核通過，請檢查修改。");
                        
                        return isErrorMessage;
                    }
                }
                // 4. 製造商商品編號，DB：Product.MenufacturePartNum→Excel：Manufacturer Part # / ISBN DB字串長度為varchar(15)，ALlow Null
                if (!string.IsNullOrEmpty(dataFeedItem.ManufacturerPartNumber))
                {
                    isErrorMessage = System.Text.Encoding.Default.GetBytes(dataFeedItem.ManufacturerPartNumber).Length >= 15 ?
                    string.Format("【第" + dataFeedItemindex + "筆】" + "上傳不成功：上傳檔案的Datafeed工作表的Manufacturer Part # / ISBN內容，輸入的內容長度過長，長度須小於15(中文2、英數1)，請檢查修改。") : string.Empty;
                    
                    return isErrorMessage;
                }
                // 5. UPC，DB字串長度為varchar(15)，Allow Null
                if (!string.IsNullOrEmpty(dataFeedItem.UPC))
                {
                    // 判斷是否大於 DB 的長度
                    if (System.Text.Encoding.Default.GetBytes(dataFeedItem.UPC).Length >= 15)
                    {
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "筆】" + "上傳不成功：上傳檔案的Datafeed工作表的UPC內容，輸入的內容長度過長，長度須小於15(中文2、英數1)，請檢查修改。");
                        
                        return isErrorMessage;
                    }
                }
                // 6. 網站短標題 Website Short Title，DB字串長度為varchar(2000)，Not Allow Null
                if (string.IsNullOrEmpty(dataFeedItem.WebsiteShortTitle))
                {
                    isErrorMessage = string.Format("【第" + dataFeedItemindex + "筆】" + "上傳不成功 : 上傳檔案的Datafeed工作表 Website Short Title 為必填不能為空值，請檢查修改。");

                    return isErrorMessage;
                }
                else
                {
                    if (System.Text.Encoding.Default.GetBytes(dataFeedItem.WebsiteShortTitle).Length >= 2000)
                    {
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "筆】" + "上傳不成功 : 上傳檔案的Datafeed工作表 Website Short Title 為必填不能為空值或輸入長度過長，長度須小於200(中文2、英數1)，請檢查修改。");
                        return isErrorMessage;
                    }
                }
                // 7. 商品描述，DB：Product.Name→Excel：Product Description，varchar(Max)， Allow Null
                if (string.IsNullOrEmpty(dataFeedItem.ProductDescription))
                {
                    isErrorMessage = string.Format("【第" + dataFeedItemindex + "筆】" + "上傳不成功：上傳檔案的Datafeed工作表的Product Description內容，無編寫內容，請檢查修改。");
                    return isErrorMessage;
                }
                // 8. 商品注意事項，DB：Product.Note→Excel：Product Note varchar(500)，Allow Null
                if (!string.IsNullOrEmpty(dataFeedItem.ProductNote))
                {
                    if (System.Text.Encoding.Default.GetBytes(dataFeedItem.ProductNote).Length >= 500)
                    {
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "筆】" + "上傳不成功：上傳檔案的Datafeed工作表的Product Note內容，輸入的內容長度過長，長度須小於500(中文2、英數1)，請檢查修改。");
                        return isErrorMessage;
                    }
                }
                // 9. 商品簡要描述，DB：Item.Sdesc→Excel：Product Short Desc ，Not allow Null
                if (string.IsNullOrEmpty(dataFeedItem.ShortDesc))
                {
                    isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Product Short Desc，內容不允許為空，請檢查修改。");
                    return isErrorMessage;
                }
                else
                {
                    if (System.Text.Encoding.Default.GetBytes(dataFeedItem.ShortDesc).Length <= 500)
                    {
                        List<string> singleWord = new List<string>();

                        //直接去除所有空格
                        string delBlank = dataFeedItem.ShortDesc.Replace(" ", "");

                        //取出字串的各個單字
                        foreach (var single in delBlank)
                        {
                            singleWord.Add(single.ToString());
                        }

                        //此欄位只讀html格式(需前後加<li></li>，若沒加顯示error)
                        if (singleWord[0] == "<" &&
                            (singleWord[1] == "l" || singleWord[1] == "L") &&
                            (singleWord[2] == "i" || singleWord[2] == "I") &&
                            singleWord[3] == ">" &&
                            singleWord[singleWord.Count - 5] == "<" &&
                            singleWord[singleWord.Count - 4] == "/" &&
                            (singleWord[singleWord.Count - 3] == "l" || singleWord[singleWord.Count - 3] == "L") &&
                            (singleWord[singleWord.Count - 2] == "i" || singleWord[singleWord.Count - 2] == "I") &&
                            singleWord[singleWord.Count - 1] == ">")
                        {
                            // 代表格式 OK
                        }
                        else
                        {
                            isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Product Short Desc內容，只允許html格式(文字敘述前後需加入<li>xxx</li>)，請檢查修改。");
                            return isErrorMessage;
                        }
                    }
                    else
                    {
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Product Short Desc內容，輸入的內容長度過長，長度須小於500(中文2、英數1)，請檢查修改。");
                        return isErrorMessage;                   
                    }
                }              
                // 10. 商品特色標題，DB：Item.Spechead→Excel：Product Feature Title，Not Allow Null
                if (string.IsNullOrEmpty(dataFeedItem.FeatureTitle))
                {
                    isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Product Feature Title內容，不允許內容為空，請檢查修改。");
                    return isErrorMessage;
                }
                else
                {
                    if (System.Text.Encoding.Default.GetBytes(dataFeedItem.FeatureTitle).Length >= 30)
                    {
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Product Feature Title內容，輸入的內容長度過長，長度須小於30(中文2、英數1)，請檢查修改。");
                        return isErrorMessage;
                    }
                }              
                // 11. 商品型號，DB：Item.Model→Excel：Product Model，Allow Null(由 ThisWay 程式判斷)
                if (!string.IsNullOrEmpty(dataFeedItem.Model))
                {
                    if (System.Text.Encoding.Default.GetBytes(dataFeedItem.Model).Length >= 30)
                    {
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Product Model內容，輸入的內容長度過長，長度須小於30(中文2、英數1)，請檢查修改。");
                        return isErrorMessage;
                    }
                }
                
                // 12. 條碼，DB：Product.BarCode→Excel：Product Bar Code，Allow Null
                if (!string.IsNullOrEmpty(dataFeedItem.BarCode))
                {
                    if (System.Text.Encoding.Default.GetBytes(dataFeedItem.BarCode).Length >= 50)
                    {
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Product Bar Code內容，輸入的內容長度過長，長度須小於50(中文2、英數1)，請檢查修改。");
                        return isErrorMessage;
                    }
                }                
                // 13. 長度，DB：Product.Length→Excel：Product Length，Allow Null
                if (!string.IsNullOrEmpty(dataFeedItem.Length))
                {
                    if (decimal.TryParse(dataFeedItem.Length, out changeDecimalOK))
                    {
                        if (System.Text.Encoding.Default.GetBytes(dataFeedItem.Length).Length >= 10)
                        {
                            //商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                            isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Product Length內容，輸入的內容長度過長，長度須小於10，請檢查修改。");
                            return isErrorMessage;
                        }
                    }
                    else
                    {
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Product Length內容，系統無法讀取，請檢查修改。");
                        return isErrorMessage;
                    }
                }
                // 14. 寬度，DB：Product.Width→Excel：Product Width，Allow Null
                if (!string.IsNullOrEmpty(dataFeedItem.Width))
                {
                    if (decimal.TryParse(dataFeedItem.Width, out changeDecimalOK) == true)
                    {
                        // 檢查字串長度(byte)，DB字串長度為decimal(10, 2)
                        if (System.Text.Encoding.Default.GetBytes(dataFeedItem.Width).Length >= 10)
                        {
                            // 商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                            isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Product Width內容，輸入的內容長度過長，長度須小於10，請檢查修改。");
                            return isErrorMessage;
                        }
                    }
                    else
                    {
                        // 商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Product Width內容，系統無法讀取，請檢查修改。");
                        return isErrorMessage;
                    }
                }
                // 15. 高度，DB：Product.Height→Excel：Product Height，Allow Null
                if (!string.IsNullOrEmpty(dataFeedItem.Height))
                {
                    if (decimal.TryParse(dataFeedItem.Height, out changeDecimalOK) == true)
                    {
                        //檢查字串長度(byte)，DB字串長度為decimal(10, 2)
                        if (System.Text.Encoding.Default.GetBytes(dataFeedItem.Height).Length >= 10)
                        {
                            //商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                            isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Product Height內容，輸入的內容長度過長，長度須小於10，請檢查修改。");
                            return isErrorMessage;
                        }
                    }
                    else
                    {
                        //商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Product Height內容，系統無法讀取，請檢查修改。");
                        return isErrorMessage;
                    }
                }
                // 16. 重量，DB：Product.Weight→Excel：Product Weight，Allow Null
                if (!string.IsNullOrEmpty(dataFeedItem.Weight))
                {
                    if (decimal.TryParse(dataFeedItem.Weight, out changeDecimalOK) == true)
                    {
                        //檢查字串長度(byte)，DB字串長度為decimal(12, 4)
                        if (System.Text.Encoding.Default.GetBytes(dataFeedItem.Weight).Length >= 12)
                        {
                            //商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                            isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Product Weight內容，輸入的內容長度過長，長度須小於10，請檢查修改。");
                            return isErrorMessage;
                        }
                    }
                    else
                    {
                        //商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Product Weight內容，系統無法讀取，請檢查修改。");
                        return isErrorMessage;
                    }
                }
                // 17. 商品成色，DB：Product.Status→Excel：Product Condition，Allow Null
                #region 目前尚未檢查       
                /*-----第二階段再改，第一階段Mark-----*/
                //if (ItemsInfoListDatafeed[48 + (Sequence * 32)] == "New" || ItemsInfoListDatafeed[48 + (Sequence * 32)] == "NEW" ||
                //    ItemsInfoListDatafeed[48 + (Sequence * 32)] == "new" || ItemsInfoListDatafeed[48 + (Sequence * 32)] == "新的" ||
                //    ItemsInfoListDatafeed[48 + (Sequence * 32)] == "新品")
                //{
                //    //新品
                //    Product.Status = 1;
                //}
                //else if (ItemsInfoListDatafeed[48 + (Sequence * 32)] == "Refurbished" || ItemsInfoListDatafeed[48 + (Sequence * 32)] == "refurbished" ||
                //            ItemsInfoListDatafeed[48 + (Sequence * 32)] == "REFURBISHED" || ItemsInfoListDatafeed[48 + (Sequence * 32)] == "Refurbish" ||
                //            ItemsInfoListDatafeed[48 + (Sequence * 32)] == "refurbish" || ItemsInfoListDatafeed[48 + (Sequence * 32)] == "REFURBISH" ||
                //            ItemsInfoListDatafeed[48 + (Sequence * 32)] == "Refurbishment" || ItemsInfoListDatafeed[48 + (Sequence * 32)] == "refurbishment" ||
                //            ItemsInfoListDatafeed[48 + (Sequence * 32)] == "REFURBISHMENT" || ItemsInfoListDatafeed[48 + (Sequence * 32)] == "二手")
                //{
                //    //二手
                //    Product.Status = 2;
                //}
                //else
                //{
                //    //回傳的狀態
                //    result.Status = Newegg.Oversea.Silverlight.FileUploadHandler.ProcessStatus.Success;

                //    result.ResponseMsg = "【第" + Column + "行，第" + Row + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Product Condition內容，系統無法讀取，請檢查修改。";

                //    return result;
                //}
                #endregion
                // 18. 商品包裝，DB：Item.ItemPackage→Excel：Product Package，Allow Null
                if (!string.IsNullOrEmpty(dataFeedItem.Package))
                {
                    if (dataFeedItem.Package.ToLower() == "retail" || dataFeedItem.Package == "R" ||
                                    dataFeedItem.Package == "r")
                    {
                        //零售
                        itemPackage = "0";
                    }
                    else if (dataFeedItem.Package.ToLower() == "oem" || dataFeedItem.Package == "O" ||
                                dataFeedItem.Package == "o")
                    {
                        // OEM
                        itemPackage = "1";
                    }
                    else
                    {
                        //商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Product Package內容，系統無法讀取，請檢查修改。");
                        return isErrorMessage;
                    }
                }
    // -問題----// 19. 成本(seller非必填、vendor必填)，DB：Product.Cost→Excel：Cost(seller非必填、vendor必填)
                if (string.IsNullOrEmpty(dataFeedItem.ProductCost))
                {
                    if (accountType.ToLower() == "v")
                    {
                        //商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Cost內容，對於Vendor為必填，請檢查修改。");
                        return isErrorMessage;
                    }
                    //目前Seller可以不填Cost內容(不需要判斷)
                }
                else if (decimal.TryParse(dataFeedItem.ProductCost, out changeDecimalOK) == true)
                {
                    // 不為Null，判斷轉型，是否成功
                    //檢查字串長度(byte)，DB字串長度為decimal(10, 2)
                    if (System.Text.Encoding.Default.GetBytes(dataFeedItem.ProductCost).Length >= 10)
                    {
                        //商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Cost內容，輸入的內容長度過長，長度須小於10，請檢查修改。");
                        return isErrorMessage;
                    }
                }
                else
                {
                    //商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                    isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Cost內容須為數字，系統無法讀取，請檢查修改。");
                    return isErrorMessage;
                }
                
                #region *****成本(seller非必填、vendor必填)，DB：Product.Cost→Excel：Cost
                /*
                //*****成本(seller非必填、vendor必填)，DB：Product.Cost→Excel：Cost
                Column++;
                if (ItemsInfoListDatafeed[50 + (Sequence * 32)] == "" && AccountTypeCode == "V" || AccountTypeCode == "v")
                {
                    //商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                    isNull = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Cost內容，對於Vendor為必填，請檢查修改。");

                    //回傳的狀態
                    result.Status = Newegg.Oversea.Silverlight.FileUploadHandler.ProcessStatus.Failed;
                    return result;
                }
                //轉型，是否成功
                else if (decimal.TryParse(ItemsInfoListDatafeed[50 + (Sequence * 32)], out ChangeDecimalOK) == true)
                {
                    //檢查字串長度(byte)，DB字串長度為decimal(10, 2)
                    if (System.Text.Encoding.Default.GetBytes(ItemsInfoListDatafeed[50 + (Sequence * 32)]).Length <= 10)
                    {
                        Product.Cost = ChangeDecimalOK;
                    }
                    else
                    {
                        //商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                        isNull = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Cost內容，輸入的內容長度過長，長度須小於10，請檢查修改。");

                        //回傳的狀態
                        result.Status = Newegg.Oversea.Silverlight.FileUploadHandler.ProcessStatus.Failed;
                        return result;
                    }
                }
                else if (ItemsInfoListDatafeed[50 + (Sequence * 32)] == "" && AccountTypeCode == "s" || AccountTypeCode == "S")
                {
                    //目前Seller可以不填Cost內容
                }
                else
                {
                    //商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                    isNull = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Cost內容須為數字，系統無法讀取，請檢查修改。");

                    //回傳的狀態
                    result.Status = Newegg.Oversea.Silverlight.FileUploadHandler.ProcessStatus.Failed;
                    return result;
                }
                 * */
                #endregion

                #region *****網路售價，DB：Item.PriceCash、Item.PriceCard→Excel：Selling Price
                /*
                //*****網路售價，DB：Item.PriceCash→Excel：Selling Price
                Column++;
                //轉型，是否成功
                if (decimal.TryParse(ItemsInfoListDatafeed[51 + (Sequence * 32)], out ChangeDecimalOK) == true)
                {
                    //檢查字串長度(byte)，DB字串長度為decimal(10, 2)
                    if (System.Text.Encoding.Default.GetBytes(ItemsInfoListDatafeed[51 + (Sequence * 32)]).Length <= 10)
                    {
                        Item.PriceCash = ChangeDecimalOK;
                        Item.PriceCard = ChangeDecimalOK;

                        if (AccountTypeCode == "S" || AccountTypeCode == "s")
                        {
                            Item.PriceCash = ChangeDecimalOK;
                            Item.PriceCard = ChangeDecimalOK;
                        }
                        else if (AccountTypeCode == "V" || AccountTypeCode == "v")
                        {
                            Item.PriceCash = ChangeDecimalOK;
                            Item.PriceCard = ChangeDecimalOK;
                        }
                        else
                        {
                            //商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                            ResultCookie("上傳不成功：無法透過AccountTypeCode確認使用者為Seller或Vendor。");

                            //回傳的狀態
                            result.Status = Newegg.Oversea.Silverlight.FileUploadHandler.ProcessStatus.Failed;
                            return result;
                        }
                    }
                    else
                    {
                        //商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                        isNull = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Selling Price內容，輸入的內容長度過長，長度須小於10，請檢查修改。");

                        //回傳的狀態
                        result.Status = Newegg.Oversea.Silverlight.FileUploadHandler.ProcessStatus.Failed;
                        return result;
                    }
                }
                else
                {
                    //商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                    isNull = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Selling Price內容，系統無法讀取，請檢查修改。");

                    //回傳的狀態
                    result.Status = Newegg.Oversea.Silverlight.FileUploadHandler.ProcessStatus.Failed;
                    return result;
                }
                 * */
                #endregion
                // 20. 網路售價，DB：Item.PriceCash、Item.PriceCard→Excel：Selling Price，Not Allow Null
                if (string.IsNullOrEmpty(dataFeedItem.SelleringPrice))
                {
                    isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Selling Price內容，內容不得為空，請檢查修改。");
                    return isErrorMessage;
                }
                else
                {
                    //轉型，是否成功
                    if (decimal.TryParse(dataFeedItem.SelleringPrice, out changeDecimalOK) == true)
                    {
                        //檢查字串長度(byte)，DB字串長度為decimal(10, 2)
                        if (System.Text.Encoding.Default.GetBytes(dataFeedItem.SelleringPrice).Length >= 10)
                        {
                            // 長度大於 10
                            isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Selling Price內容，輸入的內容長度過長，長度須小於10，請檢查修改。");
                            return isErrorMessage;
                        }
                    }
                    else
                    {
                        //商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Selling Price內容，系統無法讀取，請檢查修改。");
                        return isErrorMessage;
                    }
                }
                // 21. 市場建議售價 DB：Item.MarketPrice→Excel：Market Price，Allow Null
                if (string.IsNullOrEmpty(dataFeedItem.MarketPrice))
                {    // 可不填
                    //isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Market Price內容，內容不得為空，請檢查修改。");
                    //return isErrorMessage;
                }
                else
                {
                    if (decimal.TryParse(dataFeedItem.MarketPrice, out changeDecimalOK) == true)
                    {
                        //檢查字串長度(byte)，DB字串長度為decimal(15, 4)
                        if (System.Text.Encoding.Default.GetBytes(dataFeedItem.MarketPrice).Length >= 15)
                        {
                            //商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                            isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Market Price內容，輸入的內容長度過長，長度須小於15，請檢查修改。");
                            return isErrorMessage;
                        }
                    }
                    else
                    {
                        //商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Market Price內容，系統無法讀取，請檢查修改。");
                        return isErrorMessage;
                    }
                }
                
                // 22. 保固期，DB：Product.Warranty→Excel：Warranty，Allow Null
                if (!string.IsNullOrEmpty(dataFeedItem.Warranty))
                {
                    if (int.TryParse(dataFeedItem.Warranty, out changeIntOK) == false)
                    {
                        //商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Warranty內容，系統無法讀取，請檢查修改。");
                        return isErrorMessage;
                    }
                }
                // 23. 限量數量，DB：Item.Qty→Excel：Limit Quantity 非必填
                if (!string.IsNullOrEmpty(dataFeedItem.LimitQuantity))
                {
                    if (int.TryParse(dataFeedItem.LimitQuantity, out changeIntOK) == false)
                    {
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Limit Quantity內容非數字，請檢查修改。");
                        return isErrorMessage;
                    }
                }
                // xx. 是否限量
                #region *****是否限量，DB：No，判斷庫存是否為限量→Excel：Is Limit(已將excel欄位刪除不使用)
                ////*****是否限量，DB：No，判斷庫存是否為限量→Excel：Is Limit
                //Column++;
                //if (ItemsInfoListDatafeed[54 + (Sequence * 32)] == "Yes" || ItemsInfoListDatafeed[54 + (Sequence * 32)] == "Y" ||
                //    ItemsInfoListDatafeed[54 + (Sequence * 32)] == "y" || ItemsInfoListDatafeed[54 + (Sequence * 32)] == "yes" ||
                //    ItemsInfoListDatafeed[54 + (Sequence * 32)] == "YES")
                //{
                //    Item.IsLimit = "Y";
                //}
                //else if (ItemsInfoListDatafeed[54 + (Sequence * 32)] == "No" || ItemsInfoListDatafeed[54 + (Sequence * 32)] == "N" ||
                //    ItemsInfoListDatafeed[54 + (Sequence * 32)] == "n" || ItemsInfoListDatafeed[54 + (Sequence * 32)] == "no" ||
                //    ItemsInfoListDatafeed[54 + (Sequence * 32)] == "NO")
                //{
                //    Item.IsLimit = "N";
                //}
                //else
                //{
                //    //商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                //    ResultCookie("【第" + Column + "行，第" + Row + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Is Limit內容，系統無法讀取，請檢查修改。");

                //    //回傳的狀態
                //    result.Status = Newegg.Oversea.Silverlight.FileUploadHandler.ProcessStatus.Failed;
                //    return result;
                //}
                #endregion
                
                // 24. 限購數量，DB：Item.QtyLimit→Excel：Quota Quantity，Allow Null
                if (!string.IsNullOrEmpty(dataFeedItem.QuotaQuantity))
                {
                    if (int.TryParse(dataFeedItem.QuotaQuantity, out changeIntOK) == false)
                    {
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Quota Quantity內容非數字，請檢查修改。");
                        return isErrorMessage;
                    }
                }
                // 25. 庫存，DB：Item.Qty || Itemstock.Qty→Excel：Inventory(非限量 || 限量){存入哪個DB會在API做}，Not Allow Null
                if (string.IsNullOrEmpty(dataFeedItem.Inventory))
                {
                    isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Inventory內容不得為空，請檢查修改。");
                    return isErrorMessage;
                }
                else
                {
                    if (int.TryParse(dataFeedItem.Inventory, out changeIntOK) == false)
                    {
                        //商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Inventory內容，系統無法讀取，請檢查修改。");
                        return isErrorMessage;
                    }
                }
               
                // 26. 運送類型，DB：Item.ShipType→Excel：Is Ship by Newegg
                if (string.IsNullOrEmpty(dataFeedItem.IsShipByNewegg))
                {
                    isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Is Ship by Newegg內容，無法為空值，請檢查修改。");
                    return isErrorMessage;
                }
                else
                {
                    // 判斷是否能夠解析內容
                    if (dataFeedItem.IsShipByNewegg.ToLower() == "newegg" || dataFeedItem.IsShipByNewegg.ToLower() == "n")
                    {
                        shipby = "N";
                        //內容包含上述代表解析 ok
                    }
                    else if (dataFeedItem.IsShipByNewegg.ToLower() == "seller/vendor" || dataFeedItem.IsShipByNewegg.ToLower() == "s/v"
                    || dataFeedItem.IsShipByNewegg.ToLower() == "vendor/seller" || dataFeedItem.IsShipByNewegg.ToLower() == "v/s")
                    {
                        if (accountType.ToLower() == "s")
                        {
                            shipby = "S";
                        }
                        else if (accountType.ToLower() == "v")
                        {
                            shipby = "V";
                        }
                    }
                    else
                    {
                        //商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Is Ship by Newegg內容，系統無法讀取，請檢查修改。");
                        return isErrorMessage;
                    }                
                }
                    
                // 27. 到貨天數，DB：Item.DelvDate→Excel：Delv Day，Not Allow Nul
                if (string.IsNullOrEmpty(dataFeedItem.DelvDay))
                {
                    isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Delv Day內容，內容不允許為空，請檢查修改。");
                    return isErrorMessage;
                }
                else
                {
                    if (System.Text.Encoding.Default.GetBytes(dataFeedItem.DelvDay).Length >= 50)
                    {
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Delv Day內容，輸入的內容長度過長，長度須小於50(中文2、英數1)，請檢查修改。");
                        return isErrorMessage;
                    }
                }
                // 28. 29. 賣場開始日期、賣場結束日期，DB：Item.DateStart→Excel：Item Start Date，Item.DateEnd→Excel：Item End Date，Not Allow Null
                DateTime todayDate = DateTime.Today;
                DateTime startDateTime;
                DateTime.TryParse(dataFeedItem.StartDate, out startDateTime);
                DateTime endDateTime;
                DateTime.TryParse(dataFeedItem.EndDate, out endDateTime);
                if (string.IsNullOrEmpty(dataFeedItem.StartDate) || string.IsNullOrEmpty(dataFeedItem.EndDate))
                {
                    isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Item Start Date或Item End Date內容，內容不允許為空，請檢查修改。");
                    return isErrorMessage;
                }
                else
                {
                    if (DateTime.TryParse(dataFeedItem.StartDate, out startDateTime) == false/* || DateTime.TryParse(dataFeedItem.EndDate, out endDateTime) == false*/)
                    {
                        //商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Item Start Date或Item End Date內容，系統無法讀取，請檢查修改。");
                        return isErrorMessage;
                    }
                    //如果 "今天日期" > "賣場開始日期" ，邏輯不正確error
                    if (DateTime.Compare(todayDate, startDateTime) > 0)
                    {
                        //商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Item Start Date內容有誤，賣場開始日期比今天日期早，邏輯不正確，請檢查修改。");
                        return isErrorMessage;
                    }
                    //如果 "今天日期" >= "賣場結束日期" ，邏輯不正確error
                    if (DateTime.Compare(todayDate, endDateTime) >= 0)
                    {
                        //商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Item End Date內容有誤， 賣場結束日期比今天日期早或相同，邏輯不正確，請檢查修改。");
                        return isErrorMessage;
                    }
                }       
                // 30. 創建新商品，DB：XXX→Excel：Action (目前不需要管)
                // 31. 商品圖，DB：XXX→Excel：Item Images 
                if (!string.IsNullOrEmpty(dataFeedItem.Images))
                {
                    string[] strUrlList = Regex.Split(dataFeedItem.Images, ";", RegexOptions.IgnoreCase);
                    if (strUrlList.Length == 0 || strUrlList == null)
                    {
                        isErrorMessage = string.Format("上傳不成功：上傳檔案的Datafeed工作表的Item Images內容，系統無法讀取，請檢查修改。");
                        return isErrorMessage;
                    }             
                }
                // 32. 危險物品，DB：Product.IsShipDanger→Excel：Is Dangerous
                if (!string.IsNullOrEmpty(dataFeedItem.IsDangerous))
                {
                    if (dataFeedItem.IsDangerous.ToLower() == "yes" || dataFeedItem.IsDangerous.ToLower() == "y")
                    {
                        isDangerous = "Y";
                        //Product.IsShipDanger = "Y";
                    }
                    else if (dataFeedItem.IsDangerous.ToLower() == "no" || dataFeedItem.IsDangerous.ToLower() == "n")
                    {
                        isDangerous = "N";
                        //Product.IsShipDanger = "N";
                    }
                    else
                    {
                        //商品批次建立上傳處理完成，以cookie紀錄訊息，於UI呈現Msg
                        isErrorMessage = string.Format("【第" + dataFeedItemindex + "列】" + "上傳不成功：上傳檔案的Datafeed工作表的Is Dangerous內容，系統無法讀取，請檢查修改。");
                        return isErrorMessage;
                    }
                }
                
                // 是否為市場售價，DB：Product.IsMarket→Excel：Is Market Place(不檢查)，預設是 Y
            }

            return isErrorMessage;
        }

        /// <summary>
        /// 處理 ProductDetail Model 欄位，組成 HTML 格式 
        /// </summary>
        public void ModelProcess()
        {
            //暫存到同一個List
            List<string> allProductDetailInfo = new List<string>();

            //相同商家商品編號的List分類
            List<string> sellerProductIDDetailInfo = new List<string>();

            int specNum = -1;
            int compareCnt = 0;

            //存入，且須6的倍數(因為每一條資訊有6個欄位，即使非必填位填也仍佔有一格空值)
            if (batchUploadDetailInfo[0].ProductSpec_Model.Count >= 6 && batchUploadDetailInfo[0].ProductSpec_Model.Count % 6 == 0)
            {
                for (int cnt = 0; cnt <= (batchUploadDetailInfo[0].ProductSpec_Model.Count / 6) - 1; cnt++)
                {
                    for (int num = 0; num <= (batchUploadDetailInfo[0].ProductSpec_Model.Count / 6) - 1; num++)
                    {
                        //如果第n個商家商品編號跟第n+1個商家商品編號一樣
                        if (batchUploadDetailInfo[0].ProductSpec_Model[cnt * 6] == batchUploadDetailInfo[0].ProductSpec_Model[num * 6])
                        {
                            //商家商品編號首次加入
                            if (compareCnt == 0 && num * 6 == 0 && cnt * 6 == 0)
                            {
                                specNum++;

                                //SellerProductID放入
                                ProductSpec_Model ps = new TWNewEgg.API.Models.Models.ProductSpec_Model();
                                ps.HtmlList_Model = new List<HtmlSpec_Model>();
                                HtmlSpec_Model htmlsp = new HtmlSpec_Model();

                                htmlsp.SellerProductID_Model = batchUploadDetailInfo[0].ProductSpec_Model[cnt * 6];
                                ps.HtmlList_Model.Add(htmlsp);
                                productSpec_ModelList.Add(ps);

                                productSpec_ModelList[specNum].HtmlList_Model[0].Html_Model = "<Group GroupName=\"Model\">";

                                productSpec_ModelList[specNum].HtmlList_Model[0].Html_Model = productSpec_ModelList[specNum].HtmlList_Model[0].Html_Model +
                                                                                                                "<Property Key=\"" + batchUploadDetailInfo[0].ProductSpec_Model[4 + (num * 6)] + "\"Value=\"" + 
                                                                                                                batchUploadDetailInfo[0].ProductSpec_Model[5 + (num * 6)] + "\"/>";
                            }
                            else if (compareCnt == cnt * 6)
                            {
                                //商家商品編號已有，增加html內容
                                productSpec_ModelList[specNum].HtmlList_Model[0].Html_Model = productSpec_ModelList[specNum].HtmlList_Model[0].Html_Model +
                                    "<Property Key=\"" + batchUploadDetailInfo[0].ProductSpec_Model[4 + (num * 6)] + "\"Value=\"" + batchUploadDetailInfo[0].ProductSpec_Model[5 + (num * 6)] + "\"/>";
                            }
                            else if (compareCnt != cnt * 6 && specNum >= 0)
                            {
                                //商家商品編號尚未加入再加
                                bool repeat = false;

                                //比對商家商品編號是否已加入存在
                                for (int compareSpecNum = 0; compareSpecNum <= specNum; compareSpecNum++)
                                {
                                    //如果存在，在存在的地方增加html內容
                                    if (batchUploadDetailInfo[0].ProductSpec_Model[cnt * 6] == productSpec_ModelList[compareSpecNum].HtmlList_Model[0].SellerProductID_Model)
                                    {
                                        repeat = true;

                                        //內容是否重複
                                        for (int compareNum = 0; compareNum <= num; compareNum++)
                                        {
                                            if (batchUploadDetailInfo[0].ProductSpec_Model[4 + (compareNum * 6)] == batchUploadDetailInfo[0].ProductSpec_Model[4 + (num * 6)])
                                            {
                                                repeat = false;
                                            }
                                        }

                                        if (repeat == true)
                                        {
                                            productSpec_ModelList[compareSpecNum].HtmlList_Model[0].Html_Model = productSpec_ModelList[compareSpecNum].HtmlList_Model[0].Html_Model +
                                                "<Property Key=\"" + batchUploadDetailInfo[0].ProductSpec_Model[4 + (num * 6)] + "\"Value=\"" + batchUploadDetailInfo[0].ProductSpec_Model[5 + (num * 6)] + "\"/>";
                                        }

                                        repeat = true;
                                    }
                                }

                                if (repeat == false)
                                {
                                    compareCnt = cnt * 6;

                                    specNum++;

                                    //SellerProductID放入
                                    ProductSpec_Model ps = new TWNewEgg.API.Models.Models.ProductSpec_Model();
                                    ps.HtmlList_Model = new List<HtmlSpec_Model>();
                                    HtmlSpec_Model htmlsp = new HtmlSpec_Model();

                                    htmlsp.SellerProductID_Model = batchUploadDetailInfo[0].ProductSpec_Model[cnt * 6];
                                    ps.HtmlList_Model.Add(htmlsp);
                                    productSpec_ModelList.Add(ps);

                                    productSpec_ModelList[specNum].HtmlList_Model[0].Html_Model = "<Group GroupName=\"Model\">";

                                    productSpec_ModelList[specNum].HtmlList_Model[0].Html_Model = productSpec_ModelList[specNum].HtmlList_Model[0].Html_Model +
                                                                                                                 "<Property Key=\"" + batchUploadDetailInfo[0].ProductSpec_Model[4 + (num * 6)] + "\"Value=\"" + batchUploadDetailInfo[0].ProductSpec_Model[5 + (num * 6)] + "\"/>";
                                }
                            }
                        }
                    }
                }
                //在每條不同的商家商品編號List加入結束html碼
                for (int endNum = 0; endNum <= specNum; endNum++)
                {
                    productSpec_ModelList[endNum].HtmlList_Model[0].Html_Model = productSpec_ModelList[endNum].HtmlList_Model[0].Html_Model + "</Group>";
                }
            }
        }

        /// <summary>
        /// 處理 ProductDetail Spec 欄位，組成 HTML 格式
        /// </summary>
        public void SpecProcess()
        {
            //暫存到同一個List
            List<string> allProductDetailInfo = new List<string>();

            //相同商家商品編號的List分類
            List<string> sellerProductIDDetailInfo = new List<string>();

            int specNum = -1;
            int compareCnt = 0;

            //存入，且須6的倍數(因為每一條資訊有6個欄位，即使非必填位填也仍佔有一格空值)
            if (batchUploadDetailInfo[0].ProductSpec_Spec.Count >= 6 && batchUploadDetailInfo[0].ProductSpec_Spec.Count % 6 == 0)
            {
                for (int cnt = 0; cnt <= (batchUploadDetailInfo[0].ProductSpec_Spec.Count / 6) - 1; cnt++)
                {
                    for (int num = 0; num <= (batchUploadDetailInfo[0].ProductSpec_Spec.Count / 6) - 1; num++)
                    {
                        //如果第n個商家商品編號跟第n+1個商家商品編號一樣
                        if (batchUploadDetailInfo[0].ProductSpec_Spec[cnt * 6] == batchUploadDetailInfo[0].ProductSpec_Spec[num * 6])
                        {
                            //商家商品編號首次加入
                            if (compareCnt == 0 && num * 6 == 0 && cnt * 6 == 0)
                            {
                                specNum++;

                                //SellerProductID放入
                                ProductSpec_Spec ps = new TWNewEgg.API.Models.Models.ProductSpec_Spec();
                                ps.HtmlList_Spec = new List<HtmlSpec_Spec>();
                                HtmlSpec_Spec htmlsp = new HtmlSpec_Spec();

                                htmlsp.SellerProductID_Spec = batchUploadDetailInfo[0].ProductSpec_Spec[cnt * 6];
                                ps.HtmlList_Spec.Add(htmlsp);
                                productSpec_SpecList.Add(ps);

                                productSpec_SpecList[specNum].HtmlList_Spec[0].Html_Spec = "<Group GroupName=\"SPEC\">";

                                productSpec_SpecList[specNum].HtmlList_Spec[0].Html_Spec = productSpec_SpecList[specNum].HtmlList_Spec[0].Html_Spec +
                                                                                                            "<Property Key=\"" + batchUploadDetailInfo[0].ProductSpec_Spec[4 + (num * 6)] + "\"Value=\"" + batchUploadDetailInfo[0].ProductSpec_Spec[5 + (num * 6)] + "\"/>";
                            }
                            else if (compareCnt == cnt * 6)
                            {
                                //商家商品編號已有，增加html內容
                                productSpec_SpecList[specNum].HtmlList_Spec[0].Html_Spec = productSpec_SpecList[specNum].HtmlList_Spec[0].Html_Spec +
                                    "<Property Key=\"" + batchUploadDetailInfo[0].ProductSpec_Spec[4 + (num * 6)] + "\"Value=\"" + batchUploadDetailInfo[0].ProductSpec_Spec[5 + (num * 6)] + "\"/>";
                            }
                            else if (compareCnt != cnt * 6 && specNum >= 0)
                            {
                                //商家商品編號尚未加入再加
                                bool repeat = false;

                                //比對商家商品編號是否已加入存在
                                for (int compareSpecNum = 0; compareSpecNum <= specNum; compareSpecNum++)
                                {
                                    //如果存在，在存在的地方增加html內容
                                    if (batchUploadDetailInfo[0].ProductSpec_Spec[cnt * 6] == productSpec_SpecList[compareSpecNum].HtmlList_Spec[0].SellerProductID_Spec)
                                    {
                                        repeat = true;

                                        //內容是否重複
                                        for (int compareNum = 0; compareNum <= num; compareNum++)
                                        {
                                            if (batchUploadDetailInfo[0].ProductSpec_Spec[4 + (compareNum * 6)] == batchUploadDetailInfo[0].ProductSpec_Spec[4 + (num * 6)])
                                            {
                                                repeat = false;
                                            }
                                        }

                                        if (repeat == true)
                                        {
                                            productSpec_SpecList[compareSpecNum].HtmlList_Spec[0].Html_Spec = productSpec_SpecList[compareSpecNum].HtmlList_Spec[0].Html_Spec +
                                                "<Property Key=\"" + batchUploadDetailInfo[0].ProductSpec_Spec[4 + (num * 6)] + "\"Value=\"" + batchUploadDetailInfo[0].ProductSpec_Spec[5 + (num * 6)] + "\"/>";
                                        }

                                        repeat = true;
                                    }
                                }

                                if (repeat == false)
                                {
                                    compareCnt = cnt * 6;

                                    specNum++;

                                    //SellerProductID放入
                                    ProductSpec_Spec ps = new TWNewEgg.API.Models.Models.ProductSpec_Spec();
                                    ps.HtmlList_Spec = new List<HtmlSpec_Spec>();
                                    HtmlSpec_Spec htmlsp = new HtmlSpec_Spec();

                                    htmlsp.SellerProductID_Spec = batchUploadDetailInfo[0].ProductSpec_Spec[cnt * 6];
                                    ps.HtmlList_Spec.Add(htmlsp);
                                    productSpec_SpecList.Add(ps);

                                    productSpec_SpecList[specNum].HtmlList_Spec[0].Html_Spec = "<Group GroupName=\"SPEC\">";

                                    productSpec_SpecList[specNum].HtmlList_Spec[0].Html_Spec = productSpec_SpecList[specNum].HtmlList_Spec[0].Html_Spec +
                                                                                                                "<Property Key=\"" + batchUploadDetailInfo[0].ProductSpec_Spec[4 + (num * 6)] + "\"Value=\"" + batchUploadDetailInfo[0].ProductSpec_Spec[5 + (num * 6)] + "\"/>";
                                }
                            }
                        }
                    }
                }
            }
            //在每條不同的商家商品編號List加入結束html碼
            for (int endNum = 0; endNum <= specNum; endNum++)
            {
                productSpec_SpecList[endNum].HtmlList_Spec[0].Html_Spec = productSpec_SpecList[endNum].HtmlList_Spec[0].Html_Spec + "</Group>";
            }
        }

        /// <summary>
        /// 處理 ProductDetail Feature 欄位，組成 HTML 格式
        /// </summary>
        public void FeaturesProcess()
        {
            //暫存到同一個List
            List<string> allProductDetailInfo = new List<string>();

            //相同商家商品編號的List分類
            List<string> sellerProductIDDetailInfo = new List<string>();

            int specNum = -1;
            int compareCnt = 0;

            //存入，且須6的倍數(因為每一條資訊有6個欄位，即使非必填位填也仍佔有一格空值)
            if (batchUploadDetailInfo[0].ProductSpec_Features.Count >= 6 && batchUploadDetailInfo[0].ProductSpec_Features.Count % 6 == 0)
            {
                for (int cnt = 0; cnt <= (batchUploadDetailInfo[0].ProductSpec_Features.Count / 6) - 1; cnt++)
                {
                    for (int num = 0; num <= (batchUploadDetailInfo[0].ProductSpec_Features.Count / 6) - 1; num++)
                    {
                        //如果第n個商家商品編號跟第n+1個商家商品編號一樣
                        if (batchUploadDetailInfo[0].ProductSpec_Features[cnt * 6] == batchUploadDetailInfo[0].ProductSpec_Features[num * 6])
                        {
                            //商家商品編號首次加入
                            if (compareCnt == 0 && num * 6 == 0 && cnt * 6 == 0)
                            {
                                specNum++;

                                //SellerProductID放入
                                ProductSpec_Features ps = new TWNewEgg.API.Models.Models.ProductSpec_Features();
                                ps.HtmlList_Features = new List<HtmlSpec_Features>();
                                HtmlSpec_Features htmlsp = new HtmlSpec_Features();

                                htmlsp.SellerProductID_Features = batchUploadDetailInfo[0].ProductSpec_Features[cnt * 6];
                                ps.HtmlList_Features.Add(htmlsp);
                                productSpec_FeaturesList.Add(ps);

                                productSpec_FeaturesList[specNum].HtmlList_Features[0].Html_Features = "<Group GroupName=\"Features\">";

                                productSpec_FeaturesList[specNum].HtmlList_Features[0].Html_Features = productSpec_FeaturesList[specNum].HtmlList_Features[0].Html_Features +
                                                                                                                "<Property Key=\"" + batchUploadDetailInfo[0].ProductSpec_Features[4 + (num * 6)] + "\"Value=\"" + batchUploadDetailInfo[0].ProductSpec_Features[5 + (num * 6)] + "\"/>";
                            }
                            else if (compareCnt == cnt * 6)
                            {
                                //商家商品編號已有，增加html內容
                                productSpec_FeaturesList[specNum].HtmlList_Features[0].Html_Features = productSpec_FeaturesList[specNum].HtmlList_Features[0].Html_Features +
                                    "<Property Key=\"" + batchUploadDetailInfo[0].ProductSpec_Features[4 + (num * 6)] + "\"Value=\"" + batchUploadDetailInfo[0].ProductSpec_Features[5 + (num * 6)] + "\"/>";
                            }
                            else if (compareCnt != cnt * 6 && specNum >= 0)
                            {
                                //商家商品編號尚未加入再加
                                bool repeat = false;

                                //比對商家商品編號是否已加入存在
                                for (int compareSpecNum = 0; compareSpecNum <= specNum; compareSpecNum++)
                                {
                                    //如果存在，在存在的地方增加html內容
                                    if (batchUploadDetailInfo[0].ProductSpec_Features[cnt * 6] == productSpec_FeaturesList[compareSpecNum].HtmlList_Features[0].SellerProductID_Features)
                                    {
                                        repeat = true;

                                        //內容是否重複
                                        for (int compareNum = 0; compareNum <= num; compareNum++)
                                        {
                                            if (batchUploadDetailInfo[0].ProductSpec_Features[4 + (compareNum * 6)] == batchUploadDetailInfo[0].ProductSpec_Features[4 + (num * 6)])
                                            {
                                                repeat = false;
                                            }
                                        }

                                        if (repeat == true)
                                        {
                                            productSpec_FeaturesList[compareSpecNum].HtmlList_Features[0].Html_Features = productSpec_FeaturesList[compareSpecNum].HtmlList_Features[0].Html_Features +
                                                "<Property Key=\"" + batchUploadDetailInfo[0].ProductSpec_Features[4 + (num * 6)] + "\"Value=\"" + batchUploadDetailInfo[0].ProductSpec_Features[5 + (num * 6)] + "\"/>";
                                        }

                                        repeat = true;
                                    }
                                }

                                if (repeat == false)
                                {
                                    compareCnt = cnt * 6;

                                    specNum++;

                                    //SellerProductID放入
                                    ProductSpec_Features ps = new TWNewEgg.API.Models.Models.ProductSpec_Features();
                                    ps.HtmlList_Features = new List<HtmlSpec_Features>();
                                    HtmlSpec_Features htmlsp = new HtmlSpec_Features();

                                    htmlsp.SellerProductID_Features = batchUploadDetailInfo[0].ProductSpec_Features[cnt * 6];
                                    ps.HtmlList_Features.Add(htmlsp);
                                    productSpec_FeaturesList.Add(ps);

                                    productSpec_FeaturesList[specNum].HtmlList_Features[0].Html_Features = "<Group GroupName=\"Features\">";

                                    productSpec_FeaturesList[specNum].HtmlList_Features[0].Html_Features = productSpec_FeaturesList[specNum].HtmlList_Features[0].Html_Features +
                                                                                                                 "<Property Key=\"" + batchUploadDetailInfo[0].ProductSpec_Features[4 + (num * 6)] + "\"Value=\"" + batchUploadDetailInfo[0].ProductSpec_Features[5 + (num * 6)] + "\"/>";
                                }
                            }
                        }
                    }
                }
                //在每條不同的商家商品編號List加入結束html碼
                for (int endNum = 0; endNum <= specNum; endNum++)
                {
                    productSpec_FeaturesList[endNum].HtmlList_Features[0].Html_Features = productSpec_FeaturesList[endNum].HtmlList_Features[0].Html_Features + "</Group>";
                }
            }
        }

        /// <summary>
        /// 處理 ProductDetail Size 欄位，組成 HTML 格式
        /// </summary>
        public void SizeProcess()
        {
            //暫存到同一個List
            List<string> allProductDetailInfo = new List<string>();

            //相同商家商品編號的List分類
            List<string> sellerProductIDDetailInfo = new List<string>();

            int specNum = -1;
            int compareCnt = 0;

            //存入，且須6的倍數(因為每一條資訊有6個欄位，即使非必填位填也仍佔有一格空值)
            if (batchUploadDetailInfo[0].ProductSpec_Size.Count >= 6 && batchUploadDetailInfo[0].ProductSpec_Size.Count % 6 == 0)
            {
                for (int cnt = 0; cnt <= (batchUploadDetailInfo[0].ProductSpec_Size.Count / 6) - 1; cnt++)
                {
                    for (int num = 0; num <= (batchUploadDetailInfo[0].ProductSpec_Size.Count / 6) - 1; num++)
                    {
                        //如果第n個商家商品編號跟第n+1個商家商品編號一樣
                        if (batchUploadDetailInfo[0].ProductSpec_Size[cnt * 6] == batchUploadDetailInfo[0].ProductSpec_Size[num * 6])
                        {
                            //商家商品編號首次加入
                            if (compareCnt == 0 && num * 6 == 0 && cnt * 6 == 0)
                            {
                                specNum++;

                                //SellerProductID放入
                                ProductSpec_Size ps = new TWNewEgg.API.Models.Models.ProductSpec_Size();
                                ps.HtmlList_Size = new List<HtmlSpec_Size>();
                                HtmlSpec_Size htmlsp = new HtmlSpec_Size();

                                htmlsp.SellerProductID_Size = batchUploadDetailInfo[0].ProductSpec_Size[cnt * 6];
                                ps.HtmlList_Size.Add(htmlsp);
                                productSpec_SizeList.Add(ps);

                                productSpec_SizeList[specNum].HtmlList_Size[0].Html_Size = "<Group GroupName=\"Size\">";

                                productSpec_SizeList[specNum].HtmlList_Size[0].Html_Size = productSpec_SizeList[specNum].HtmlList_Size[0].Html_Size +
                                                                                                                "<Property Key=\"" + batchUploadDetailInfo[0].ProductSpec_Size[4 + (num * 6)] + "\"Value=\"" + batchUploadDetailInfo[0].ProductSpec_Size[5 + (num * 6)] + "\"/>";
                            }                          
                            else if (compareCnt == cnt * 6)
                            {
                                //商家商品編號已有，增加html內容
                                productSpec_SizeList[specNum].HtmlList_Size[0].Html_Size = productSpec_SizeList[specNum].HtmlList_Size[0].Html_Size +
                                    "<Property Key=\"" + batchUploadDetailInfo[0].ProductSpec_Size[4 + (num * 6)] + "\"Value=\"" + batchUploadDetailInfo[0].ProductSpec_Size[5 + (num * 6)] + "\"/>";
                            }
                            else if (compareCnt != cnt * 6 && specNum >= 0)
                            {
                                //商家商品編號尚未加入再加
                                bool repeat = false;

                                //比對商家商品編號是否已加入存在
                                for (int compareSpecNum = 0; compareSpecNum <= specNum; compareSpecNum++)
                                {
                                    //如果存在，在存在的地方增加html內容
                                    if (batchUploadDetailInfo[0].ProductSpec_Size[cnt * 6] == productSpec_SizeList[compareSpecNum].HtmlList_Size[0].SellerProductID_Size)
                                    {
                                        repeat = true;

                                        //內容是否重複
                                        for (int compareNum = 0; compareNum <= num; compareNum++)
                                        {
                                            if (batchUploadDetailInfo[0].ProductSpec_Size[4 + (compareNum * 6)] == batchUploadDetailInfo[0].ProductSpec_Size[4 + (num * 6)])
                                            {
                                                repeat = false;
                                            }
                                        }

                                        if (repeat == true)
                                        {
                                            productSpec_SizeList[compareSpecNum].HtmlList_Size[0].Html_Size = productSpec_SizeList[compareSpecNum].HtmlList_Size[0].Html_Size +
                                                "<Property Key=\"" + batchUploadDetailInfo[0].ProductSpec_Size[4 + (num * 6)] + "\"Value=\"" + batchUploadDetailInfo[0].ProductSpec_Size[5 + (num * 6)] + "\"/>";
                                        }

                                        repeat = true;
                                    }
                                }

                                if (repeat == false)
                                {
                                    compareCnt = cnt * 6;

                                    specNum++;

                                    //SellerProductID放入
                                    ProductSpec_Size ps = new TWNewEgg.API.Models.Models.ProductSpec_Size();
                                    ps.HtmlList_Size = new List<HtmlSpec_Size>();
                                    HtmlSpec_Size htmlsp = new HtmlSpec_Size();

                                    htmlsp.SellerProductID_Size = batchUploadDetailInfo[0].ProductSpec_Size[cnt * 6];
                                    ps.HtmlList_Size.Add(htmlsp);
                                    productSpec_SizeList.Add(ps);

                                    productSpec_SizeList[specNum].HtmlList_Size[0].Html_Size = "<Group GroupName=\"Size\">";

                                    productSpec_SizeList[specNum].HtmlList_Size[0].Html_Size = productSpec_SizeList[specNum].HtmlList_Size[0].Html_Size +
                                                                                                                 "<Property Key=\"" + batchUploadDetailInfo[0].ProductSpec_Size[4 + (num * 6)] + "\"Value=\"" + batchUploadDetailInfo[0].ProductSpec_Size[5 + (num * 6)] + "\"/>";
                                }
                            }
                        }
                    }
                }
                //在每條不同的商家商品編號List加入結束html碼
                for (int endNum = 0; endNum <= specNum; endNum++)
                {
                    productSpec_SizeList[endNum].HtmlList_Size[0].Html_Size = productSpec_SizeList[endNum].HtmlList_Size[0].Html_Size + "</Group>";
                }
            }
        }

        /// <summary>
        /// 處理 ProductDetail SpecEndProcess 欄位，組成 HTML 格式
        /// </summary>
        public void ProductSpecEndProcess()
        {
            //暫存到同一個List
            List<string> allProductDetailInfo = new List<string>();

            //相同商家商品編號的List分類
            List<string> sellerProductIDDetailInfo = new List<string>();

            //如果分類好的model有值
            #region 如果分類好的model有值
            if (productSpec_ModelList.Count > 0)
            {
                //將ProductSpec加入model
                for (int cnt = 0; cnt <= productSpec_ModelList.Count - 1; cnt++)
                {
                    //SellerProductID放入
                    ProductSpec ps = new TWNewEgg.API.Models.Models.ProductSpec();
                    ps.HtmlList = new List<HtmlSpec>();
                    HtmlSpec htmlsp = new HtmlSpec();

                    htmlsp.SellerProductID = productSpec_ModelList[cnt].HtmlList_Model[0].SellerProductID_Model;
                    ps.HtmlList.Add(htmlsp);
                    productSpecList.Add(ps);

                    //html內容放入
                    productSpecList[cnt].HtmlList[0].Html = productSpec_ModelList[cnt].HtmlList_Model[0].Html_Model;
                }
            }
            #endregion

            //如果分類好的Spec有值
            #region 如果分類好的Spec有值
            if (productSpec_SpecList.Count > 0)
            {
                //已有相同
                bool repeat = false;

                //如果Model沒有值，則Spec直接存
                if (productSpec_ModelList.Count == 0)
                {
                    //將ProductSpec加入Spec
                    for (int cnt = 0; cnt <= productSpec_SpecList.Count - 1; cnt++)
                    {
                        //SellerProductID放入
                        ProductSpec ps = new TWNewEgg.API.Models.Models.ProductSpec();
                        ps.HtmlList = new List<HtmlSpec>();
                        HtmlSpec htmlsp = new HtmlSpec();

                        htmlsp.SellerProductID = productSpec_SpecList[cnt].HtmlList_Spec[0].SellerProductID_Spec;
                        ps.HtmlList.Add(htmlsp);
                        productSpecList.Add(ps);

                        //html內容放入
                        productSpecList[cnt].HtmlList[0].Html = productSpec_SpecList[cnt].HtmlList_Spec[0].Html_Spec;
                    }
                }

                //查看ProductSpec的SellerProductID與Spec的SellerProductID是否有不同的，增加list
                int productSpecListCount = productSpecList.Count - 1;
                for (int num = 0; num <= productSpecListCount; num++)
                {
                    for (int cnt = 0; cnt <= productSpec_SpecList.Count - 1; cnt++)
                    {
                        //相同
                        if (productSpecList[num].HtmlList[0].SellerProductID == productSpec_SpecList[cnt].HtmlList_Spec[0].SellerProductID_Spec)
                        {
                            //直接增加在相同的地方
                            productSpecList[num].HtmlList[0].Html = productSpecList[num].HtmlList[0].Html + productSpec_SpecList[cnt].HtmlList_Spec[0].Html_Spec;

                            repeat = true;
                        }

                        //不同
                        if (productSpecList[num].HtmlList[0].SellerProductID != productSpec_SpecList[cnt].HtmlList_Spec[0].SellerProductID_Spec)
                        {
                            repeat = false;

                            //再次比對所有的商家商品編號是否重覆
                            for (int numAgain = 0; numAgain <= productSpecList.Count - 1; numAgain++)
                            {
                                //有跟其他的商家商品編號相同
                                if (productSpecList[numAgain].HtmlList[0].SellerProductID == productSpec_SpecList[cnt].HtmlList_Spec[0].SellerProductID_Spec)
                                {
                                    //直接增加在相同的地方ProductSpecList[num].HtmlList[0].Html + ProductSpec_SpecList[cnt].HtmlList_Spec[0].Html_Spec
                                    //ProductSpecList[numAgain].HtmlList[0].Html = ProductSpecList[numAgain].HtmlList[0].Html + ProductSpec_SpecList[cnt].HtmlList_Spec[0].Html_Spec;
                                    repeat = true;
                                }
                            }

                            //再次比對後確認沒重覆
                            if (repeat == false)
                            {
                                //加入新的list
                                //SellerProductID放入
                                ProductSpec ps = new TWNewEgg.API.Models.Models.ProductSpec();
                                ps.HtmlList = new List<HtmlSpec>();
                                HtmlSpec htmlsp = new HtmlSpec();

                                htmlsp.SellerProductID = productSpec_SpecList[cnt].HtmlList_Spec[0].SellerProductID_Spec;
                                ps.HtmlList.Add(htmlsp);
                                productSpecList.Add(ps);

                                //html內容放入
                                productSpecList[productSpecList.Count - 1].HtmlList[0].Html = productSpec_SpecList[cnt].HtmlList_Spec[0].Html_Spec;
                            }
                        }
                    }
                }
            }
            #endregion

            //如果分類好的Features有值
            #region 如果分類好的Features有值
            if (productSpec_FeaturesList.Count > 0)
            {
                //已有相同
                bool repeat = false;

                //如果Model、Spec沒有值，則Features直接存
                if (productSpec_ModelList.Count == 0 && productSpec_SpecList.Count == 0)
                {
                    //將ProductSpec加入model
                    for (int cnt = 0; cnt <= productSpec_FeaturesList.Count - 1; cnt++)
                    {
                        //SellerProductID放入
                        ProductSpec ps = new TWNewEgg.API.Models.Models.ProductSpec();
                        ps.HtmlList = new List<HtmlSpec>();
                        HtmlSpec htmlsp = new HtmlSpec();

                        htmlsp.SellerProductID = productSpec_FeaturesList[cnt].HtmlList_Features[0].SellerProductID_Features;
                        ps.HtmlList.Add(htmlsp);
                        productSpecList.Add(ps);

                        //html內容放入
                        productSpecList[cnt].HtmlList[0].Html = productSpec_FeaturesList[cnt].HtmlList_Features[0].Html_Features;
                    }
                }

                //查看ProductSpec的SellerProductID與Spec的SellerProductID是否有不同的，增加list
                int productSpecListCount = productSpecList.Count - 1;
                for (int num = 0; num <= productSpecListCount; num++)
                {
                    for (int cnt = 0; cnt <= productSpec_FeaturesList.Count - 1; cnt++)
                    {
                        //相同
                        if (productSpecList[num].HtmlList[0].SellerProductID == productSpec_FeaturesList[cnt].HtmlList_Features[0].SellerProductID_Features)
                        {
                            //直接增加在相同的地方
                            productSpecList[num].HtmlList[0].Html = productSpecList[num].HtmlList[0].Html + productSpec_FeaturesList[cnt].HtmlList_Features[0].Html_Features;

                            repeat = true;
                        }

                        //不同
                        if (productSpecList[num].HtmlList[0].SellerProductID != productSpec_FeaturesList[cnt].HtmlList_Features[0].SellerProductID_Features)
                        {
                            repeat = false;

                            //再次比對所有的商家商品編號是否重覆
                            for (int numAgain = 0; numAgain <= productSpecList.Count - 1; numAgain++)
                            {
                                //有跟其他的商家商品編號相同
                                if (productSpecList[numAgain].HtmlList[0].SellerProductID == productSpec_FeaturesList[cnt].HtmlList_Features[0].SellerProductID_Features)
                                {
                                    //直接增加在相同的地方ProductSpecList[num].HtmlList[0].Html + ProductSpec_SpecList[cnt].HtmlList_Spec[0].Html_Spec
                                    //ProductSpecList[numAgain].HtmlList[0].Html = ProductSpecList[numAgain].HtmlList[0].Html + ProductSpec_SpecList[cnt].HtmlList_Spec[0].Html_Spec;
                                    repeat = true;
                                }
                            }

                            //再次比對後確認沒重覆
                            if (repeat == false)
                            {
                                //加入新的list
                                //SellerProductID放入
                                ProductSpec ps = new TWNewEgg.API.Models.Models.ProductSpec();
                                ps.HtmlList = new List<HtmlSpec>();
                                HtmlSpec htmlsp = new HtmlSpec();

                                htmlsp.SellerProductID = productSpec_FeaturesList[cnt].HtmlList_Features[0].SellerProductID_Features;
                                ps.HtmlList.Add(htmlsp);
                                productSpecList.Add(ps);

                                //html內容放入
                                productSpecList[productSpecList.Count - 1].HtmlList[0].Html = productSpec_FeaturesList[cnt].HtmlList_Features[0].Html_Features;
                            }
                        }
                    }
                }
            }
            #endregion

            //如果分類好的Size有值
            #region 如果分類好的Size有值
            if (productSpec_SizeList.Count > 0)
            {
                //已有相同
                bool repeat = false;

                //如果Model、Spec、Features沒有值，則Size直接存
                if (productSpec_ModelList.Count == 0 && productSpec_SpecList.Count == 0 && productSpec_FeaturesList.Count == 0)
                {
                    //將ProductSpec加入model
                    for (int cnt = 0; cnt <= productSpec_SizeList.Count - 1; cnt++)
                    {
                        //SellerProductID放入
                        ProductSpec ps = new TWNewEgg.API.Models.Models.ProductSpec();
                        ps.HtmlList = new List<HtmlSpec>();
                        HtmlSpec htmlsp = new HtmlSpec();

                        htmlsp.SellerProductID = productSpec_SizeList[cnt].HtmlList_Size[0].SellerProductID_Size;
                        ps.HtmlList.Add(htmlsp);
                        productSpecList.Add(ps);

                        //html內容放入
                        productSpecList[cnt].HtmlList[0].Html = productSpec_SizeList[cnt].HtmlList_Size[0].Html_Size;
                    }
                }

                //查看ProductSpec的SellerProductID與Spec的SellerProductID是否有不同的，增加list
                int productSpecListCount = productSpecList.Count - 1;
                for (int num = 0; num <= productSpecListCount; num++)
                {
                    for (int cnt = 0; cnt <= productSpec_SizeList.Count - 1; cnt++)
                    {
                        //相同
                        if (productSpecList[num].HtmlList[0].SellerProductID == productSpec_SizeList[cnt].HtmlList_Size[0].SellerProductID_Size)
                        {
                            //直接增加在相同的地方
                            productSpecList[num].HtmlList[0].Html = productSpecList[num].HtmlList[0].Html + productSpec_SizeList[cnt].HtmlList_Size[0].Html_Size;

                            repeat = true;
                        }

                        //不同
                        if (productSpecList[num].HtmlList[0].SellerProductID != productSpec_SizeList[cnt].HtmlList_Size[0].SellerProductID_Size)
                        {
                            repeat = false;

                            //再次比對所有的商家商品編號是否重覆
                            for (int numAgain = 0; numAgain <= productSpecList.Count - 1; numAgain++)
                            {
                                //有跟其他的商家商品編號相同
                                if (productSpecList[numAgain].HtmlList[0].SellerProductID == productSpec_SizeList[cnt].HtmlList_Size[0].SellerProductID_Size)
                                {
                                    //直接增加在相同的地方ProductSpecList[num].HtmlList[0].Html + ProductSpec_SpecList[cnt].HtmlList_Spec[0].Html_Spec
                                    //ProductSpecList[numAgain].HtmlList[0].Html = ProductSpecList[numAgain].HtmlList[0].Html + ProductSpec_SpecList[cnt].HtmlList_Spec[0].Html_Spec;
                                    repeat = true;
                                }
                            }

                            //再次比對後確認沒重覆
                            if (repeat == false)
                            {
                                //加入新的list
                                //SellerProductID放入
                                ProductSpec ps = new TWNewEgg.API.Models.Models.ProductSpec();
                                ps.HtmlList = new List<HtmlSpec>();
                                HtmlSpec htmlsp = new HtmlSpec();

                                htmlsp.SellerProductID = productSpec_SizeList[cnt].HtmlList_Size[0].SellerProductID_Size;
                                ps.HtmlList.Add(htmlsp);
                                productSpecList.Add(ps);

                                //html內容放入
                                productSpecList[productSpecList.Count - 1].HtmlList[0].Html = productSpec_SizeList[cnt].HtmlList_Size[0].Html_Size;
                            }
                        }
                    }
                }
            }
            #endregion

            //整合好後每一條都要加html開頭與結尾
            if (productSpecList.Count > 0)
            {
                //每一種商家商品編號的html前面加入開頭與結尾碼
                for (int cnt = 0; cnt <= productSpecList.Count - 1; cnt++)
                {
                    productSpecList[cnt].HtmlList[0].Html = "<LongDescription Name=\"Detailed Specifications\">" + productSpecList[cnt].HtmlList[0].Html + "</LongDescription>";
                }
            }
        }
        #endregion

        #region 批次修改商品

        /// <summary>
        /// 批次修改商品內容
        /// </summary>
        /// <param name="bathCreateFileInfo">傳入要批次修改的輸入資訊</param>
        /// <returns>回傳是否成功修改商品</returns>
        public ActionResponse<BathItemCreateInfoResult> BatchEditCreation(BathItemCreateInfo bathCreateFileInfo)
        {
            ActionResponse<BathItemCreateInfoResult> result = new ActionResponse<BathItemCreateInfoResult>();
            // 1. 讀取excel
            this.Readexcel(bathCreateFileInfo);
            // 2. 檢查excel欄位
            this.CheckeditExcel();
            // 3. 傳入 Model進行修改 (呼叫 中 Jack 程式)
            this.PostEditItemInfo();
            // 4. 回傳批次修改是否成功
            return result;
        }

        /// <summary>
        /// 讀取批次修改 Excel
        /// </summary>
        /// <param name="bathCreateFileInfo">傳入要批次修改的所需內容資訊</param>
        private void Readexcel(BathItemCreateInfo bathCreateFileInfo)
        {
            string itemsResult, productsResult, detailResult;

            BathItemCreateInfoResult queryResult = new BathItemCreateInfoResult();
            string creationFile = bathCreateFileInfo.CreateFile;
            string extension = bathCreateFileInfo.Extension;
            string filePath = bathCreateFileInfo.FilePath;
            string fileName = bathCreateFileInfo.FileName;
            string accountTypeCode = bathCreateFileInfo.AccountTypeCode;
            int sellerID = bathCreateFileInfo.SellerID;

            // 讀取 Excel 檔案
            var excel = new ExcelQueryFactory(filePath + fileName + extension);

            if (extension.ToLower() == ".xls")
            {
                var datafeed = excel.Worksheet();
                var detailInfo = excel.Worksheet();
            }
        }

        /// <summary>
        /// 傳入DB
        /// </summary>
        private void PostEditItemInfo()
        {
            APIItemModel editedItem = new APIItemModel();
            ItemService itemService = new ItemService();

            itemService.EditCreatedItem(editedItem);

            throw new NotImplementedException();
        }

        /// <summary>
        /// 檢查編輯的 excel
        /// </summary>
        private void CheckeditExcel()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
 
    /*---------- end by Ian and Thisway ----------*/
