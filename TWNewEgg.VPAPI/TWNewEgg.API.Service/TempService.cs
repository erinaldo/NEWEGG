using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using TWNewEgg.API.Models;
using TWNewEgg.API.Models.DomainModel;
using TWNewEgg.BackendService;
using TWNewEgg.DB.TWSQLDB.Models;
using AutoMapper;
using log4net;
using log4net.Config;
using System.Text.RegularExpressions;


namespace TWNewEgg.API.Service
{
    public class TempService
    {
        // 記錄訊息
        private static ILog log = LogManager.GetLogger(typeof(TempService));
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        TWNewEgg.DB.TWSqlDBContext db = new DB.TWSqlDBContext();

        private string images = System.Configuration.ConfigurationManager.AppSettings["Images"];

        private string website = System.Configuration.ConfigurationManager.AppSettings["TWSPHost"];

        #region 待審查詢

        /// <summary>
        /// 輸入查詢條件查詢ItemList
        /// </summary>
        /// <param name="itemSearch">itemSearch</param>
        /// <returns>ActionResponse</returns>
        public Models.ActionResponse<List<Models.ItemSketch>> ItemList(TWNewEgg.API.Models.ItemSketchSearchCondition itemSearch, bool boolDefault)
        {
            
            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            Models.ActionResponse<List<Models.ItemSketch>> result = new Models.ActionResponse<List<Models.ItemSketch>>();

            // 檢查輸入的Seller是否存在
            if (this.CheckSellerExist(itemSearch.SellerID))
            {
                // 搜尋條件搜尋結果
                var itemQueryable = this.ItemSearchResult(itemSearch, boolDefault);
                List<Models.ItemSketch> itemInfoList = new List<Models.ItemSketch>();

                // 紀錄符合條件總筆數(實際回傳筆數只有PageSize筆)
                int totalCount = itemQueryable.Count();

                // 若有分頁資訊則進行API分頁功能
                if (itemSearch.pageInfo != null)
                {
                    itemQueryable = itemQueryable.Skip(itemSearch.pageInfo.PageIndex * itemSearch.pageInfo.PageSize).Take(itemSearch.pageInfo.PageSize).AsQueryable();
                }
                else
                {
                    itemQueryable = itemQueryable.AsQueryable();
                }

                // 進行分頁語法產生後才實際從資料庫撈回資料
                itemInfoList = itemQueryable.OrderByDescending(x=>x.CreateAndUpdate.UpdateDate).ToList();

                string getYear = DateTime.Now.Year.ToString();
                string getMonth = DateTime.Now.Month.ToString();
                List<TWNewEgg.DB.TWSQLDB.Models.Currency> currencyList = db.Currency.Where(x => x.Year == getYear && x.Month == getMonth).ToList();

                foreach (var itemInfo in itemInfoList)
                {
                    // 商品成色
                    //int status = itemInfo.ProductStatusInt;
                    //switch (status)
                    //{
                    //    case 0:
                    //        itemInfo.ProductStatus = "未標";
                    //        break;
                    //    case 1:
                    //        itemInfo.ProductStatus = "新品";
                    //        break;
                    //    case 2:
                    //        itemInfo.ProductStatus = "拆封";
                    //        break;
                    //}

                    #region 轉換商品成色的值

                    // 將前台商品成色的值，轉換為 Seller Portal 商品成色的值
                    //switch (itemInfo.Item.IsNew)
                    //{
                    //    case "Y":
                    //        {
                    //            itemInfo.Condition = 1;
                    //            break;
                    //        }
                    //    case "N":
                    //        {
                    //            itemInfo.Condition = 2;
                    //            break;
                    //        }
                    //    default:
                    //        {
                    //            itemInfo.Condition = 0;
                    //            break;
                    //        }
                    //}

                    #endregion 轉換商品成色的值

                    #region 圖片超連結處理

                    string images = System.Configuration.ConfigurationManager.AppSettings["Images"];

                    string imagesServer = System.Configuration.ConfigurationManager.AppSettings["NeweggImages"];

                    int picID = 0;
                    if (itemInfo.Item.ItemID != 0 && itemInfo.Item.ItemID.HasValue)
                    {
                        picID = itemInfo.Item.ItemID.Value;

                        // 使用ItemID產生對應圖片網址                    
                        string pid = picID.ToString("00000000");
                        string pidf4 = pid.Substring(0, 4);
                        string pidl4 = pid.Substring(4, 4);
                        
                        string serverPath = HttpContext.Current.Server.MapPath("~/pic/pic/item/" + pidf4 + "/" + pidl4 + "_1_60.jpg");
                        itemInfo.Product.PicPatch_Edit = new List<string>();                        
                        if (System.IO.File.Exists(serverPath))
                        {                            
                            // 修改畫面所需要的七張圖片 URL
                            if (itemInfo.Product.PicStart != 0 && itemInfo.Product.PicEnd != 0)
                            {
                                itemInfo.Product.PicPath_Sketch = images + "/pic/item/" + pidf4 + "/" + pidl4 + "_1_60.jpg";

                                for (int picIndex = 1; picIndex <= itemInfo.Product.PicEnd; picIndex++)
                                {
                                    itemInfo.Product.PicPatch_Edit.Add(images + "/pic/item/" + pid.Substring(0, 4) + "/" + pid.Substring(4, 4) + "_" + picIndex + "_640.jpg");
                                }
                            }
                        }
                        else
                        {                            
                            // 修改畫面所需要的七張圖片 URL
                            if (itemInfo.Product.PicStart != 0 && itemInfo.Product.PicEnd != 0)
                            {
                                itemInfo.Product.PicPath_Sketch = imagesServer + "/pic/item/" + pidf4 + "/" + pidl4 + "_1_60.jpg";

                                for (int picIndex = 1; picIndex <= itemInfo.Product.PicEnd; picIndex++)
                                {
                                    itemInfo.Product.PicPatch_Edit.Add(imagesServer + "/pic/item/" + pid.Substring(0, 4) + "/" + pid.Substring(4, 4) + "_" + picIndex + "_640.jpg");
                                }
                            }
                        }
                    }
                    else
                    {
                        picID = itemInfo.Item.ID;

                        // 使用ItemID產生對應圖片網址                    
                        string pid = picID.ToString("00000000");
                        string pidf4 = pid.Substring(0, 4);
                        string pidl4 = pid.Substring(4, 4);

                        string serverPath = HttpContext.Current.Server.MapPath("~/pic/pic/itemtemp/" + pidf4 + "/" + pidl4 + "_1_60.jpg");
                        itemInfo.Product.PicPatch_Edit = new List<string>();
                        if (System.IO.File.Exists(serverPath))
                        {
                            itemInfo.Product.PicPatch_Edit = new List<string>();
                            // 修改畫面所需要的七張圖片 URL
                            if (itemInfo.Product.PicStart != 0 && itemInfo.Product.PicEnd != 0)
                            {
                                itemInfo.Product.PicPath_Sketch = images + "/pic/itemtemp/" + pidf4 + "/" + pidl4 + "_1_60.jpg";

                                for (int picIndex = 1; picIndex <= itemInfo.Product.PicEnd; picIndex++)
                                {
                                    itemInfo.Product.PicPatch_Edit.Add(images + "/pic/itemtemp/" + pid.Substring(0, 4) + "/" + pid.Substring(4, 4) + "_" + picIndex + "_640.jpg");
                                }
                            }
                        }
                        else
                        {
                            itemInfo.Product.PicPatch_Edit = new List<string>();
                            // 修改畫面所需要的七張圖片 URL
                            if (itemInfo.Product.PicStart != 0 && itemInfo.Product.PicEnd != 0)
                            {
                                itemInfo.Product.PicPath_Sketch = imagesServer + "/pic/itemtemp/" + pidf4 + "/" + pidl4 + "_1_60.jpg";

                                for (int picIndex = 1; picIndex <= itemInfo.Product.PicEnd; picIndex++)
                                {
                                    itemInfo.Product.PicPatch_Edit.Add(imagesServer + "/pic/itemtemp/" + pid.Substring(0, 4) + "/" + pid.Substring(4, 4) + "_" + picIndex + "_640.jpg");
                                }
                            }
                        }             
                    }

                    #endregion

                    #region 賣場預覽處理

                    string isOpenPreview = System.Configuration.ConfigurationManager.AppSettings["PreViewFlag"];
                    // 賣場網址
                    string webSite = System.Configuration.ConfigurationManager.AppSettings["WebSite"];

                    try
                    {
                        // 空值不顯示
                        if (string.IsNullOrWhiteSpace(isOpenPreview))
                        {
                            itemInfo.Item.ItemURL = webSite + "/item/itemdetail?item_id=" + itemInfo.Item.ItemID;
                        }
                        else
                        {
                            if (isOpenPreview.ToLower() == "off")
                            {
                                itemInfo.Item.ItemURL = webSite + "/item/itemdetail?item_id=" + itemInfo.Item.ItemID;
                            }
                            else
                            {
                                if (isOpenPreview.ToLower() == "on")
                                {
                                    if (itemInfo.Item.ItemID != 0)
                                    {
                                        string previewInfo = "temp_" + itemSearch.SellerID + "_" + itemInfo.Item.ID + "_" + DateTime.UtcNow.AddHours(8).ToString("yyyy/MM/dd HH:mm:ss");

                                        string aesPreviewInfo = AesEncryptor.AesEncrypt(previewInfo);

                                        itemInfo.Item.ItemURL = webSite + "/item/ItemPreview?itemID=" + HttpUtility.UrlEncode(aesPreviewInfo);

                                        // 賣場連結
                                        //itemInfo.Item.ItemURL = webSite + "/item/itemdetail?item_id=" + itemInfo.Item.ItemID;
                                    }
                                    else
                                    {
                                        string previewInfo = "temp_" + itemSearch.SellerID + "_" + itemInfo.Item.ID + "_" + DateTime.UtcNow.AddHours(8).ToString("yyyy/MM/dd HH:mm:ss");

                                        string aesPreviewInfo = AesEncryptor.AesEncrypt(previewInfo);

                                        itemInfo.Item.ItemURL = webSite + "/item/ItemPreview?itemID=" + HttpUtility.UrlEncode(aesPreviewInfo);
                                    }

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error("預覽 Msg: " + ex.Message + ", Stacktrace: " + ex.StackTrace);
                    }
                    #endregion


                    #region 取匯率

                    TWNewEgg.DB.TWSQLDB.Models.Currency searchCurrency = currencyList.Where(x => x.CountryID == itemInfo.CountryID).FirstOrDefault();
                    if (searchCurrency != null)
                    {
                        itemInfo.CurrencyAverageExchange = searchCurrency.AverageexchangeRate;
                    }

                    #endregion

                    #region 計算毛利率
                    if (itemInfo.Item.PriceCash != 0)
                    {
                        itemInfo.ItemDisplayPrice.GrossMargin = ((itemInfo.Item.PriceCash - (itemInfo.Product.Cost * itemInfo.CurrencyAverageExchange)) / itemInfo.Item.PriceCash) * 100;
                    }
                    else
                    {
                        itemInfo.ItemDisplayPrice.GrossMargin = 0;
                    }

                    #endregion 計算毛利率

                    // 轉成UTC格式給JSON才不會-8，但此處時間內容是本地時間
                    itemInfo.Item.DateCreate = itemInfo.Item.DateCreate.ToUniversalTime().AddHours(8);
                    itemInfo.Item.DateStart = itemInfo.Item.DateStart.ToUniversalTime().AddHours(8);
                    itemInfo.Item.DateEnd = itemInfo.Item.DateEnd.ToUniversalTime().AddHours(8);

                    #region 取得跨分類資訊

                    DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();

                    try
                    {
                        List<int> categoryIDCell = dbFront.ItemCategorytemp.Where(x => x.itemtempID == itemInfo.Item.ID && x.FromSystem == "1").Select(x => x.CategoryID).ToList();

                        if (categoryIDCell.Count > 0)
                        {
                            itemInfo.ItemCategory.SubCategoryID_1_Layer2 = categoryIDCell[0];
                            itemInfo.ItemCategory.SubCategoryID_1_Layer1 = dbFront.Category.Where(x => x.ID == itemInfo.ItemCategory.SubCategoryID_1_Layer2).Select(x => x.ParentID).FirstOrDefault();

                            if (categoryIDCell.Count == 2)
                            {
                                itemInfo.ItemCategory.SubCategoryID_2_Layer2 = categoryIDCell[1];
                                itemInfo.ItemCategory.SubCategoryID_2_Layer1 = dbFront.Category.Where(x => x.ID == itemInfo.ItemCategory.SubCategoryID_2_Layer2).Select(x => x.ParentID).FirstOrDefault();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Msg = "發生 exception，取得跨分類資訊失敗。";
                        log.Info("取得跨分類資訊失敗(exception)：(ItemSketchID = " + itemInfo.Item.ID + ") " + ex.ToString());
                    }

                    #endregion 取得跨分類
                }

                if (itemInfoList.Count == 0)
                {
                    result.Code = (int)ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "查無商品";
                }
                else
                {
                    result.Code = (int)ResponseCode.Success;
                    result.IsSuccess = true;
                    result.Msg = totalCount.ToString();
                    result.Body = itemInfoList;
                }
            }
            else
            {
                result.Code = (int)ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "查無此Seller";
            }
            
            return result;
        }

        /// <summary>
        /// 四種搜尋模式及進階查詢
        /// </summary>
        /// <param name="itemSearch">itemSearch</param>
        /// <returns>IQueryable</returns>
        private IQueryable<TWNewEgg.API.Models.ItemSketch> ItemSearchResult(TWNewEgg.API.Models.ItemSketchSearchCondition itemSearch, bool boolDefault)
        {
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            // 查詢特定seller 所有商品
            var itemList = this.QueryItemList(itemSearch.SellerID, boolDefault).AsQueryable();

            switch (itemSearch.KeyWordScarchTarget)
            {
                // 商家商品編號
                case ItemSketchKeyWordSearchTarget.SellerProductID:
                    if (itemSearch.KeyWord != string.Empty && itemSearch.KeyWord != null)
                    {
                        itemList = itemList.Where(x => x.Product.SellerProductID == itemSearch.KeyWord).AsQueryable();
                    }

                    break;
                // 廠商產品編號
                case ItemSketchKeyWordSearchTarget.MenufacturePartNum:
                    if (itemSearch.KeyWord != string.Empty && itemSearch.KeyWord != null)
                    {
                        itemList = itemList.Where(x => x.Product.MenufacturePartNum == itemSearch.KeyWord).AsQueryable();
                    }

                    break;
                // 新蛋商品編號
                case ItemSketchKeyWordSearchTarget.ItemID:
                    if (itemSearch.KeyWord != string.Empty && itemSearch.KeyWord != null)
                    {
                        int intKeyword = -1;
                        Int32.TryParse(itemSearch.KeyWord, out intKeyword);
                        itemList = itemList.Where(x => x.Item.ItemID == intKeyword).AsQueryable();
                    }

                    break;
                // 商品描述
                case ItemSketchKeyWordSearchTarget.ProductName:
                    if (itemSearch.KeyWord != string.Empty && itemSearch.KeyWord != null)
                    {
                        itemList = itemList.Where(x => x.Product.Name.Contains(itemSearch.KeyWord)).AsQueryable();
                    }

                    break;
                case ItemSketchKeyWordSearchTarget.ItemTempID:
                    if (itemSearch.KeyWord != string.Empty && itemSearch.KeyWord != null)
                    {
                        int intKeyword = -1;
                        Int32.TryParse(itemSearch.KeyWord, out intKeyword);
                        itemList = itemList.Where(x => x.Item.ID == intKeyword).AsQueryable();
                    }
                    break;
                // 綜合搜尋
                case ItemSketchKeyWordSearchTarget.All:
                    if (itemSearch.KeyWord != string.Empty && itemSearch.KeyWord != null)
                    {
                        int intKeyword = 0;
                        bool isInt = Int32.TryParse(itemSearch.KeyWord, out intKeyword);
                        if (isInt == false)
                        {
                            intKeyword = -1;
                        }

                        TWNewEgg.DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();

                        var manufacturerInfo = spdb.Seller_ManufactureInfo.ToList();
                        if (manufacturerInfo.Where(x => x.ManufactureName == itemSearch.KeyWord).Select(x => x.SN).Any())
                        {
                            int tempManufactureID = manufacturerInfo.Where(x => x.ManufactureName == itemSearch.KeyWord).Select(x => x.SN).FirstOrDefault();

                            itemList = itemList.Where(
                                x => x.Product.Name.Contains(itemSearch.KeyWord)
                                    || x.Product.MenufacturePartNum == itemSearch.KeyWord
                                    || x.Product.SellerProductID == itemSearch.KeyWord
                                    || x.Item.ItemID == intKeyword
                                    || x.Product.ManufactureID == intKeyword
                                    || x.Product.ManufactureID == tempManufactureID).AsQueryable();
                        }
                        else
                        {
                            itemList = itemList.Where(
                                x => x.Product.Name.Contains(itemSearch.KeyWord)
                                    || x.Product.MenufacturePartNum == itemSearch.KeyWord
                                    || x.Product.SellerProductID == itemSearch.KeyWord
                                    || x.Item.ItemID == intKeyword
                                    || x.Product.ManufactureID == intKeyword).AsQueryable();
                        }
                    }

                    break;
            }

            #region 進階搜尋

            // 商品加價購狀態
            if (itemSearch.ShowOrder.HasValue)
            {
                itemList = itemList.Where(x => x.Item.ShowOrder == itemSearch.ShowOrder).AsQueryable();
            }
            // 廢四機狀態
            if (itemSearch.IsRecover=="Y")
            {
                itemList = itemList.Where(x => x.Item.Discard4 == itemSearch.IsRecover).AsQueryable();
            }
            // 商品審核狀態
            if (itemSearch.Status.HasValue)
            {
                itemList = itemList.Where(x => x.Item.status == itemSearch.Status).AsQueryable();
            }
            // 製造商
            if (itemSearch.ManufactureID > 0)
            {
                itemList = itemList.Where(x => x.Product.ManufactureID == itemSearch.ManufactureID).AsQueryable();
            }

            // 商品狀態
            if (itemSearch.ItemStatus.HasValue)
            {
                itemList = itemList.Where(x => x.Item.ItemStatus == itemSearch.ItemStatus).AsQueryable();
            }

            //可售數量搜尋
            if (itemSearch.canSellQty != ItemSketchCanSellQty.All)
            {
                if (itemSearch.canSellQty == ItemSketchCanSellQty.EqualOrMoreThen100)
                {
                    itemList = itemList.Where(x => x.ItemStock.CanSaleQty >= 100).OrderBy(x => x.ItemStock.CanSaleQty).ThenBy(x => x.Item.ID).AsQueryable();
                }
                else
                {
                    int canSellQty = (int)itemSearch.canSellQty;
                    itemList = itemList.Where(x => x.ItemStock.CanSaleQty < canSellQty).OrderBy(x => x.ItemStock.CanSaleQty).ThenBy(x => x.Item.ID).AsQueryable();
                }
            }
            // 商品成色
            //if (itemSearch.ItemCondition.HasValue)
            //{
            //    itemList = itemList.Where(x => x.Item.IsNew == itemSearch.ItemCondition).AsQueryable();
            //}          

            #region 依分類篩選

            if (itemSearch.categoryID_Layer0 > 0 || itemSearch.categoryID_Layer1 > 0 || itemSearch.categoryID_Layer2 > 0)
            {
                // 第 0 到第 2 層分類都有選擇
                if (itemSearch.categoryID_Layer0 > 0 && itemSearch.categoryID_Layer1 > 0 && itemSearch.categoryID_Layer2 > 0)
                {
                    itemList = itemList.Where(x => x.ItemCategory.MainCategoryID_Layer2 == itemSearch.categoryID_Layer2).OrderBy(x => x.ItemCategory.MainCategoryID_Layer2).ThenBy(x => x.ID).AsQueryable();
                }
                else
                {
                    // 第 2 層分類 ID 清單
                    List<int> categoryIDCell_Layer2 = new List<int>();

                    // 只選擇第 0 到第 1 層分類
                    if (itemSearch.categoryID_Layer0 > 0 && itemSearch.categoryID_Layer1 > 0)
                    {
                        // 由第 1 層的分類 ID 查詢第 2 層的所有分類 ID
                        ActionResponse<List<int>> GetCategoryIDCellResult = GetChildCategoryIDCell(itemSearch.categoryID_Layer1);

                        if (GetCategoryIDCellResult.IsSuccess)
                        {
                            categoryIDCell_Layer2.AddRange(GetCategoryIDCellResult.Body);
                        }
                    }
                    else if (itemSearch.categoryID_Layer0 > 0)
                    {
                        // 只選擇第 0 層分類
                        // 由第 0 層的分類 ID 查詢第 1 層的所有分類 ID
                        ActionResponse<List<int>> categoryIDCell_Layer1 = GetChildCategoryIDCell(itemSearch.categoryID_Layer0);

                        if (categoryIDCell_Layer1.IsSuccess)
                        {
                            foreach (int categoryID_Layer1 in categoryIDCell_Layer1.Body)
                            {
                                // 由第 1 層的分類 ID 查詢第 2 層的所有分類 ID
                                ActionResponse<List<int>> GetCategoryIDCellResult = GetChildCategoryIDCell(categoryID_Layer1);

                                if (GetCategoryIDCellResult.IsSuccess)
                                {
                                    categoryIDCell_Layer2.AddRange(GetCategoryIDCellResult.Body);
                                }
                            }
                        }
                    }

                    // 將第 2 層分類 ID 清單，列入搜詢範圍
                    itemList = itemList.Where(x => categoryIDCell_Layer2.Contains(x.ItemCategory.MainCategoryID_Layer2 ?? 0)).OrderBy(x => x.ItemCategory.MainCategoryID_Layer2).ThenBy(x => x.ID).AsQueryable();
                }
            }

            #endregion 依分類篩選

            #region 依創建日期篩選

            if (itemSearch.createDate != ItemSketchCreateDate.All)
            {
                DateTime startDate, endDate;

                switch (itemSearch.createDate)
                {
                    default:
                    case ItemSketchCreateDate.Today:
                    case ItemSketchCreateDate.Last3Days:
                    case ItemSketchCreateDate.Last7Days:
                    case ItemSketchCreateDate.Last30Days:
                        {
                            startDate = DateTime.Today;
                            endDate = startDate.AddDays(1);

                            if (itemSearch.createDate != ItemSketchCreateDate.Today)
                            {
                                startDate = startDate.AddDays(-((int)itemSearch.createDate) + 1);
                            }
                            break;
                        }
                    case ItemSketchCreateDate.SpecifyDate:
                    case ItemSketchCreateDate.DateRange:
                        {
                            startDate = Convert.ToDateTime(itemSearch.startDate.Value.AddHours(8)).Date;
                            endDate = Convert.ToDateTime(itemSearch.endDate.Value.AddHours(8)).Date.AddDays(1);
                            break;
                        }
                }

                itemList = itemList.Where(x => x.CreateAndUpdate.CreateDate >= startDate && x.CreateAndUpdate.CreateDate < endDate).OrderByDescending(x => x.CreateAndUpdate.CreateDate).ThenBy(x => x.ID).AsQueryable();
            }

            #endregion 依創建日期篩選

            #region 依可賣數量篩選

            if (itemSearch.canSellQty != ItemSketchCanSellQty.All)
            {
                if (itemSearch.canSellQty == ItemSketchCanSellQty.EqualOrMoreThen100)
                {
                    itemList = itemList.Where(x => x.ItemStock.CanSaleQty >= 100).OrderBy(x => x.ItemStock.CanSaleQty).ThenBy(x => x.ID).ThenBy(x => x.ID).AsQueryable();
                }
                else
                {
                    int canSellQty = (int)itemSearch.canSellQty;
                    itemList = itemList.Where(x => x.ItemStock.CanSaleQty < canSellQty).OrderBy(x => x.ItemStock.CanSaleQty).ThenBy(x => x.ID).AsQueryable();
                }
            }

            #endregion 依可賣數量篩選

            #endregion 進階搜尋

            // 查詢結果排序後回傳

            #region 排序項目、遞增遞減設定

            #region 判斷搜尋條件有幾項

            // 搜尋項目數
            int searchConditionItmeCount = 0;

            if (string.IsNullOrEmpty(itemSearch.KeyWord) == false)
            {
                searchConditionItmeCount++;
            }

            if (searchConditionItmeCount > 0)
            {
                searchConditionItmeCount++;
            }

            if (itemSearch.canSellQty != ItemSketchCanSellQty.All)
            {
                searchConditionItmeCount++;
            }

            if (itemSearch.categoryID_Layer0 > 0 || itemSearch.categoryID_Layer1 > 0 || itemSearch.categoryID_Layer2 > 0)
            {
                searchConditionItmeCount++;
            }

            if (itemSearch.createDate != ItemSketchCreateDate.All)
            {
                searchConditionItmeCount++;
            }

            #endregion 判斷搜尋條件有幾項

            // 未設定任格搜索條件，或為多項目的複合搜尋時，將排序設為遞減的更新日期
            if (searchConditionItmeCount != 1)
            {
                itemList = itemList.OrderByDescending(x => x.CreateAndUpdate.UpdateDate).AsQueryable();
            }

            #endregion 排序項目、遞增遞減設定

            #region 分頁、顯示資料筆數設定

            //if (condition.PageIndex >= 0 && condition.PageSize > 0)
            //{
            //    sketchQueryable = sketchQueryable.Skip(condition.PageIndex.Value * condition.PageSize.Value).Take(condition.PageSize.Value).AsQueryable();
            //}

            #endregion 分頁、顯示資料筆數設定

            var result = itemList.ToList();

            return itemList;
        }

        #region 取得分類資訊

        /// <summary>
        /// 取得子分類 ID 清單 (0 = 取得第1層)
        /// </summary>
        /// <param name="parentID">父分類ID</param>
        /// <returns>子分類 ID 清單</returns>
        private Models.ActionResponse<List<int>> GetChildCategoryIDCell(int parentID)
        {
            DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();

            Models.ActionResponse<List<int>> result = new Models.ActionResponse<List<int>>();
            result.Body = new List<int>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            if (parentID > 0)
            {
                try
                {
                    result.Body = dbFront.Category.Where(x => x.ParentID == parentID).Select(x => x.ID).ToList();
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Msg = "發生 exctption，查詢 Category 失敗。";
                    log.Info("查詢 Category 失敗 (exctption)：" + ex.ToString());
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = "Parent ID 錯誤。";
                log.Info("查詢 Category 失敗：Parent ID 錯誤。");
            }

            if (result.Body.Count() == 0)
            {
                result.IsSuccess = false;
                result.Msg = "查無資料。";
                log.Info("查詢 Category 失敗：查無資料。");
            }

            if (result.IsSuccess)
            {
                result.Code = (int)ResponseCode.Success;
                result.Msg = "已成功查到 " + result.Body.Count.ToString() + " 筆資料。";
                //log.Info(result.Msg);
            }
            else
            {
                result.Code = (int)ResponseCode.Error;
            }

            return result;
        }

        #endregion 取得分類資訊

        /// <summary>
        /// 檢查Seller_BasicInfo是否有輸入的SellerID
        /// </summary>
        /// <param name="sellerID">sellerID</param>
        /// <returns>bool</returns>
        private bool CheckSellerExist(int sellerID)
        {
            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            bool sellerExist = spdb.Seller_BasicInfo.Where(x => x.SellerID == sellerID).Any();
            return sellerExist;
        }

        /// <summary>
        /// 3.丟入搜尋結果Model (item 及 product 綁定SellerID Inner Join)
        /// </summary>
        /// <param name="sellerID">sellerID</param>
        /// <returns>IQueryable</returns>
        private IQueryable<TWNewEgg.API.Models.ItemSketch> QueryItemList(int sellerID, bool boolDefault)
        {
            //boolDefault-> true:修改用; false: 查詢用
            if (boolDefault == true)
            {
                #region 修改用
                TWNewEgg.DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
                DB.TWSqlDBContext db = new DB.TWSqlDBContext();
                List<int> groupItemTempID = db.ItemGroupDetailProperty.Where(x => x.SellerID == sellerID).Select(r => r.ItemTempID.Value).ToList();
                var itemList = (from productTemp in db.ProductTemp
                                join itemTemp in db.ItemTemp on productTemp.ID equals itemTemp.ProduttempID
                                join itemStockTemp in db.ItemStocktemp on productTemp.ID equals itemStockTemp.producttempID
                                join seller in db.Seller on itemTemp.SellerID equals seller.ID
                                join currency in db.Currency on seller.CountryID equals currency.CountryID into currency_Temp
                                from currency in currency_Temp.DefaultIfEmpty()
                                join manufacture in db.Manufacture on itemTemp.ManufactureID equals manufacture.ID
                                join category in db.Category on itemTemp.CategoryID equals category.ID
                                // join subcatrgory
                                join sub in db.Category on category.ParentID equals sub.ID
                                //join displayprice in db.ItemDisplayPrice on i.ID equals displayprice.ItemID into tmp
                                //from ds in tmp.DefaultIfEmpty()
                                join category_Layer2 in db.Category on itemTemp.CategoryID equals category_Layer2.ID into category_Layer2_Temp
                                from category_Layer2 in category_Layer2_Temp.DefaultIfEmpty()
                                join category_Layer1 in db.Category on category_Layer2.ParentID equals category_Layer1.ID into category_Layer1_Temp
                                from category_Layer1 in category_Layer1_Temp.DefaultIfEmpty()
                                join category_Layer0 in db.Category on category_Layer1.ParentID equals category_Layer0.ID into category_Layer0_Temp
                                from category_Layer0 in category_Layer0_Temp.DefaultIfEmpty()
                                select new TWNewEgg.API.Models.ItemSketch
                                {
                                    ID = 0,
                                    Status = 0,
                                    CountryID = seller.CountryID ?? 1,
                                    CurrencyAverageExchange = 1,
                                    Item = new ItemSketch_Item
                                    {
                                        // ItemTempID
                                        ID = itemTemp.ID,
                                        // 商家名稱
                                        SellerName = seller.Name,
                                        // 遞送方式
                                        ShipType = string.IsNullOrEmpty(itemTemp.ShipType) ? "S" : itemTemp.ShipType,
                                        // 審核狀態
                                        status = itemTemp.Status,
                                        // 正式商品狀態
                                        //ItemStatus = itemTemp.ItemStatus.HasValue ? 1 : itemTemp.ItemStatus.Value,
                                        ItemStatus = itemTemp.ItemStatus.HasValue ? itemTemp.ItemStatus.Value : 1,
                                        // 新蛋賣場編號
                                        ItemID = itemTemp.ItemID,
                                        // 市場建議售價
                                        MarketPrice = itemTemp.MarketPrice,
                                        // 售價
                                        PriceCash = itemTemp.PriceCash,
                                        // 賣場限量數量
                                        ItemQty = itemTemp.Qty,
                                        // 賣場登記已售數量
                                        ItemQtyReg = itemTemp.QtyReg,
                                        // 賣場限量可售數量
                                        CanSaleLimitQty = (itemTemp.Qty - itemTemp.QtyReg) < 0 ? 0 : (itemTemp.Qty - itemTemp.QtyReg),
                                        // 商品成色
                                        IsNew = string.IsNullOrEmpty(itemTemp.IsNew) ? "Y" : itemTemp.IsNew,
                                        // 賣場開始日期
                                        DateStart = itemTemp.DateStart,
                                        // 賣場結束日期
                                        DateEnd = itemTemp.DateEnd,
                                        // 賣場建立日期
                                        DateCreate = itemTemp.CreateDate,
                                        // 商家 SellerID
                                        SellerID = itemTemp.SellerID,
                                        ItemPackage = string.IsNullOrEmpty(itemTemp.ItemPackage) ? "0" : itemTemp.ItemPackage,
                                        Note = itemTemp.Note,
                                        Sdesc = itemTemp.Sdesc,
                                        Spechead = itemTemp.Spechead,
                                        ShowOrder = itemTemp.Showorder,
                                        QtyLimit = itemTemp.QtyLimit,
                                        DelvDate = string.IsNullOrEmpty(itemTemp.DelvData) ? "1-7" : itemTemp.DelvData,
                                        Discard4 = itemTemp.Discard4,
                                    },
                                    Product = new ItemSketch_Product
                                    {
                                        // ProductTemp.ID
                                        ID = productTemp.ID,
                                        // 正式產品編號
                                        ProductID = productTemp.ProductID,
                                        // 商品名稱
                                        Name = itemTemp.Name,
                                        // 成本
                                        Cost = productTemp.Cost,
                                        // 商家商品編號
                                        SellerProductID = productTemp.SellerProductID,
                                        // 製造商ID
                                        ManufactureID = itemTemp.ManufactureID,
                                        // 製造商名稱
                                        ManufacturerName = manufacture.Name,
                                        // UPC
                                        UPC = productTemp.UPC,
                                        // 廠商產品編號
                                        MenufacturePartNum = productTemp.MenufacturePartNum,
                                        BarCode = productTemp.BarCode,
                                        Description = productTemp.DescriptionTW,

                                        Height = productTemp.Height ?? 1m,
                                        Length = productTemp.Length ?? 1m,
                                        Width = productTemp.Width ?? 1m,
                                        Weight = productTemp.Weight ?? 1m,
                                        Is18 = string.IsNullOrEmpty(productTemp.Is18) ? "N" : productTemp.Is18,
                                        IsChokingDanger = string.IsNullOrEmpty(productTemp.IsChokingDanger) ? "N" : productTemp.IsChokingDanger,
                                        IsShipDanger = string.IsNullOrEmpty(productTemp.IsShipDanger) ? "N" : productTemp.IsShipDanger,
                                        Model = productTemp.Model,
                                        PicEnd = productTemp.PicEnd,
                                        PicStart = productTemp.PicStart,
                                        Warranty = productTemp.Warranty,
                                    },
                                    ItemStock = new ItemSketch_ItemStock
                                    {
                                        // 安全庫存數
                                        InventorySafeQty = itemStockTemp.SafeQty,
                                        // 庫存
                                        InventoryQty = itemStockTemp.Qty,
                                        // 登記已售庫存數
                                        InventoryQtyReg = itemStockTemp.QtyReg,
                                        // 庫存可售數量
                                        CanSaleQty = itemStockTemp.Qty - itemStockTemp.QtyReg
                                    },
                                    ItemCategory = new ItemSketch_ItemCategory
                                    {
                                        MainCategoryID_Layer2 = itemTemp.CategoryID,
                                        MainCategoryName_Layer2 = category_Layer2.Description,
                                        MainCategoryID_Layer1 = category_Layer1.ID,
                                        MainCategoryName_Layer1 = category_Layer1.Description,
                                        MainCategoryID_Layer0 = category_Layer0.ID,
                                        MainCategoryName_Layer0 = category_Layer0.Description
                                    },
                                    ItemDisplayPrice = new ItemSketchListItemDisplayPrice
                                    {
                                        // 毛利率
                                        GrossMargin = itemTemp.PriceCash > 0 ? (itemTemp.PriceCash - productTemp.Cost) / itemTemp.PriceCash * 100 : 0
                                    },
                                    CreateAndUpdate = new CreateAndUpdateIfno
                                    {
                                        CreateDate = itemTemp.CreateDate,
                                        UpdateDate = itemTemp.UpdateDate.HasValue ? itemTemp.UpdateDate.Value : itemTemp.CreateDate
                                    }
                                }).Where(x => x.Item.SellerID == sellerID && x.Item.ItemStatus != 99 /*&& !groupItemTempID.Contains(x.Item.ID)*/).Distinct().AsParallel().WithDegreeOfParallelism(2).AsQueryable();
                itemList = itemList.Where(p => !groupItemTempID.Any(q => p.Item.ID == q)).AsQueryable();
                return itemList;
                #endregion 
            }
            else
            {
                #region 查詢用
                TWNewEgg.DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
                DB.TWSqlDBContext db = new DB.TWSqlDBContext();
                List<int> groupItemTempID = db.ItemGroupDetailProperty.Where(x => x.SellerID == sellerID).Select(r=>r.ItemTempID.Value).ToList();
                var itemList = (from productTemp in db.ProductTemp
                                join itemTemp in db.ItemTemp on productTemp.ID equals itemTemp.ProduttempID
                                join itemStockTemp in db.ItemStocktemp on productTemp.ID equals itemStockTemp.producttempID
                                join seller in db.Seller on itemTemp.SellerID equals seller.ID
                                join currency in db.Currency on seller.CountryID equals currency.CountryID into currency_Temp
                                from currency in currency_Temp.DefaultIfEmpty()
                                join manufacture in db.Manufacture on itemTemp.ManufactureID equals manufacture.ID
                                join category in db.Category on itemTemp.CategoryID equals category.ID
                                // join subcatrgory
                                join sub in db.Category on category.ParentID equals sub.ID
                                //join displayprice in db.ItemDisplayPrice on i.ID equals displayprice.ItemID into tmp
                                //from ds in tmp.DefaultIfEmpty()
                                join category_Layer2 in db.Category on itemTemp.CategoryID equals category_Layer2.ID into category_Layer2_Temp
                                from category_Layer2 in category_Layer2_Temp.DefaultIfEmpty()
                                join category_Layer1 in db.Category on category_Layer2.ParentID equals category_Layer1.ID into category_Layer1_Temp
                                from category_Layer1 in category_Layer1_Temp.DefaultIfEmpty()
                                join category_Layer0 in db.Category on category_Layer1.ParentID equals category_Layer0.ID into category_Layer0_Temp
                                from category_Layer0 in category_Layer0_Temp.DefaultIfEmpty()
                                select new TWNewEgg.API.Models.ItemSketch
                                {
                                    ID = 0,
                                    Status = 0,
                                    CountryID = seller.CountryID ?? 1,
                                    CurrencyAverageExchange = 1,
                                    Item = new ItemSketch_Item
                                    {
                                        // ItemTempID
                                        ID = itemTemp.ID,
                                        // 商家名稱
                                        SellerName = seller.Name,
                                        // 遞送方式
                                        ShipType = string.IsNullOrEmpty(itemTemp.ShipType) ? "S" : itemTemp.ShipType,
                                        // 審核狀態
                                        status = itemTemp.Status,
                                        // 正式商品狀態
                                        //ItemStatus = itemTemp.ItemStatus.HasValue ? 1 : itemTemp.ItemStatus.Value,
                                        ItemStatus = itemTemp.ItemStatus.HasValue ? itemTemp.ItemStatus.Value : 1,
                                        // 新蛋賣場編號
                                        ItemID = itemTemp.ItemID,
                                        // 市場建議售價
                                        MarketPrice = itemTemp.MarketPrice,
                                        // 售價
                                        PriceCash = itemTemp.PriceCash,
                                        // 賣場限量數量
                                        ItemQty = itemTemp.Qty,
                                        // 賣場登記已售數量
                                        ItemQtyReg = itemTemp.QtyReg,
                                        // 賣場限可售數量
                                        CanSaleLimitQty = (itemTemp.Qty - itemTemp.QtyReg) < 0 ? 0 : (itemTemp.Qty - itemTemp.QtyReg),
                                        // 商品成色
                                        IsNew = string.IsNullOrEmpty(itemTemp.IsNew) ? "Y" : itemTemp.IsNew,
                                        // 賣場開始日期
                                        DateStart = itemTemp.DateStart,
                                        // 賣場結束日期
                                        DateEnd = itemTemp.DateEnd,
                                        // 賣場建立日期
                                        DateCreate = itemTemp.CreateDate,
                                        // 商家 SellerID
                                        SellerID = itemTemp.SellerID,
                                        ItemPackage = string.IsNullOrEmpty(itemTemp.ItemPackage) ? "0" : itemTemp.ItemPackage,
                                        Note = itemTemp.Note,
                                        Sdesc = itemTemp.Sdesc,
                                        Spechead = itemTemp.Spechead,
                                        ShowOrder = itemTemp.Showorder,
                                        QtyLimit = itemTemp.QtyLimit,
                                        DelvDate = string.IsNullOrEmpty(itemTemp.DelvData) ? "1-7" : itemTemp.DelvData,
                                        Discard4 = itemTemp.Discard4,
                                    },
                                    Product = new ItemSketch_Product
                                    {
                                        // ProductTemp.ID
                                        ID = productTemp.ID,
                                        // 正式產品編號
                                        ProductID = productTemp.ProductID,
                                        // 商品名稱
                                        Name = itemTemp.Name,
                                        // 成本
                                        Cost = productTemp.Cost,
                                        // 商家商品編號
                                        SellerProductID = productTemp.SellerProductID,
                                        // 製造商ID
                                        ManufactureID = itemTemp.ManufactureID,
                                        // 製造商名稱
                                        ManufacturerName = manufacture.Name,
                                        // UPC
                                        UPC = productTemp.UPC,
                                        // 廠商產品編號
                                        MenufacturePartNum = productTemp.MenufacturePartNum,
                                        BarCode = productTemp.BarCode,
                                        //Description = productTemp.DescriptionTW,
                                        Description = "",
                                        Height = productTemp.Height ?? 1m,
                                        Length = productTemp.Length ?? 1m,
                                        Width = productTemp.Width ?? 1m,
                                        Weight = productTemp.Weight ?? 1m,
                                        Is18 = string.IsNullOrEmpty(productTemp.Is18) ? "N" : productTemp.Is18,
                                        IsChokingDanger = string.IsNullOrEmpty(productTemp.IsChokingDanger) ? "N" : productTemp.IsChokingDanger,
                                        IsShipDanger = string.IsNullOrEmpty(productTemp.IsShipDanger) ? "N" : productTemp.IsShipDanger,
                                        Model = productTemp.Model,
                                        PicEnd = productTemp.PicEnd,
                                        PicStart = productTemp.PicStart,
                                        Warranty = productTemp.Warranty,
                                    },
                                    ItemStock = new ItemSketch_ItemStock
                                    {
                                        // 安全庫存數
                                        InventorySafeQty = itemStockTemp.SafeQty,
                                        // 庫存
                                        InventoryQty = itemStockTemp.Qty,
                                        // 登記已售庫存數
                                        InventoryQtyReg = itemStockTemp.QtyReg,
                                        // 庫存可售數量
                                        CanSaleQty = itemStockTemp.Qty - itemStockTemp.QtyReg
                                    },
                                    ItemCategory = new ItemSketch_ItemCategory
                                    {
                                        MainCategoryID_Layer2 = itemTemp.CategoryID,
                                        MainCategoryName_Layer2 = category_Layer2.Description,
                                        MainCategoryID_Layer1 = category_Layer1.ID,
                                        MainCategoryName_Layer1 = category_Layer1.Description,
                                        MainCategoryID_Layer0 = category_Layer0.ID,
                                        MainCategoryName_Layer0 = category_Layer0.Description
                                    },
                                    ItemDisplayPrice = new ItemSketchListItemDisplayPrice
                                    {
                                        // 毛利率
                                        GrossMargin = itemTemp.PriceCash > 0 ? (itemTemp.PriceCash - productTemp.Cost) / itemTemp.PriceCash * 100 : 0
                                    },
                                    CreateAndUpdate = new CreateAndUpdateIfno
                                    {
                                        CreateDate = itemTemp.CreateDate,
                                        UpdateDate = itemTemp.UpdateDate.HasValue ? itemTemp.UpdateDate.Value : itemTemp.CreateDate
                                    }
                                }).Where(x => x.Item.SellerID == sellerID && x.Item.ItemStatus != 99 /*&& !groupItemTempID.Contains(x.Item.ID)*/).Distinct().AsParallel().WithDegreeOfParallelism(2).AsQueryable();
                itemList = itemList.Where(p => !groupItemTempID.Any(q => p.Item.ID == q)).AsQueryable();
                return itemList;
                #endregion
            }
            
        }

        #region QueryCategory
        /// <summary>
        /// 由layer和parentID找出符合條件的Category
        /// </summary>
        /// <param name="layer">layer</param>
        /// <param name="parentID">parentID</param>
        /// <returns>查詢結果</returns>
        public Models.ActionResponse<List<DB.TWSQLDB.Models.Category>> QueryCategory(int? layer, int? parentID)
        {
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            Models.ActionResponse<List<DB.TWSQLDB.Models.Category>> result = new Models.ActionResponse<List<DB.TWSQLDB.Models.Category>>();
            List<DB.TWSQLDB.Models.Category> categoryList = new List<DB.TWSQLDB.Models.Category>();
            if (layer == null && parentID == null)
            {
                categoryList = db.Category.ToList();
            }
            else
            {
                categoryList = db.Category.Where(x => x.Layer == layer && x.ParentID == parentID).ToList();
            }

            if (categoryList.Count() == 0)
            {
                result.Code = (int)ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "請確認Layer和Parent ID";
            }
            else
            {
                result.Code = (int)ResponseCode.Success;
                result.IsSuccess = true;
                result.Msg = categoryList.Count.ToString();
            }

            // 成功或失敗都要回傳body
            result.Body = categoryList;
            return result;
        }

        /// <summary>
        /// 查詢主類別數量給創建商品目錄使用
        /// </summary>
        /// <returns>Dictionary</returns>
        public Dictionary<string, int> CountCategory()
        {
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            Dictionary<string, int> result = new Dictionary<string, int>();
            List<DB.TWSQLDB.Models.Category> allCategory = db.Category.ToList();
            List<DB.TWSQLDB.Models.Category> industry = allCategory.Where(x => x.Layer == 0).ToList();
            List<DB.TWSQLDB.Models.Category> secondLayer = allCategory.Where(x => x.Layer == 1).ToList();
            List<DB.TWSQLDB.Models.Category> thirdLayer = allCategory.Where(x => x.Layer == 2).ToList();

            for (int i = 0; i < industry.Count; i++)
            {
                int industryID = industry[i].ID;
                List<int> secondLayers = secondLayer.Where(x => x.ParentID == industryID).Select(x => x.ID).ToList();
                int totalCount = 0;
                foreach (var item in secondLayers)
                {
                    totalCount += thirdLayer.Where(x => x.ParentID == item).Count();
                }

                result.Add(industryID.ToString(), totalCount);
                totalCount = 0;
            }

            return result;
        }

        #endregion

        #endregion

        #region 待審刪除

        public ActionResponse<List<string>> DeleteTemp(List<int> ItemTempID)
        {
            ActionResponse<List<string>> result = new ActionResponse<List<string>>();
            List<TWNewEgg.DB.TWSQLDB.Models.ItemTemp> itemtemp = new List<TWNewEgg.DB.TWSQLDB.Models.ItemTemp>();
            List<TWNewEgg.DB.TWSQLDB.Models.Item> item = new List<Item>();

            using (TransactionScope scope = new TransactionScope())
            {
                itemtemp = db.ItemTemp.Where(x => ItemTempID.Contains(x.ID) && x.ItemID != 0).ToList();

                if (itemtemp.Count != ItemTempID.Count)
                {
                    
                    result.IsSuccess = false;
                    result.Msg = "資料刪除失敗!";
                    result.Code = (int)ResponseCode.AccessError;
                    log.Error("ItemTemp 有資料尚未審核通過，無法刪除");
                    return result;
                }
                result.IsSuccess = true;
                foreach (var index in itemtemp)
                {
                    log.Info("Delete: ItemTempID = " + index.ID);

                    if (index.ItemID != 0 && index.ItemStatus != 0)
                    {
                        index.ItemStatus = 99;

                        log.Info("ItemTempID: " + index.ID + ", ItemID: " + index.ItemID);

                        var deleteItem = db.Item.Where(x => x.ID == index.ItemID).FirstOrDefault();
                        log.Info("deleteItem status: " + deleteItem.Status);
                        deleteItem.Status = 99;
                        log.Info("deleteItem status Set Success");
                    }
                    else
                    {
                        log.Error("刪除資料不可刪除，刪除失敗");
                        result.IsSuccess = false;
                        result.Msg = "資料刪除失敗";
                        result.Code = (int)ResponseCode.AccessError;
                        break;
                    }
                }

                if (result.IsSuccess == false)
                {
                    log.Info("刪除失敗，資料不可刪除");
                    return result;
                }

                try
                {
                    log.Info("刪除前");
                    db.SaveChanges();
                    scope.Complete();

                    result.IsSuccess = true;
                    result.Msg = "刪除成功";
                    result.Code = (int)ResponseCode.Success;
                    log.Info("刪除成功");
                    return result;
                }
                catch (Exception ex)
                {
                    log.Error("Exception: " + ex.Message +", StackTrace: " + ex.StackTrace);
                    result.IsSuccess = false;
                    result.Code = (int)ResponseCode.Error;
                    result.Msg = "資料刪除失敗，請聯絡供應商客服人員!";
                    result.Body = null;

                    return result;
                }
            }

            return result;
        }

        #endregion

        #region 待審編輯

        #region 詳細表編輯

        /// <summary>
        /// 詳細表編輯
        /// </summary>
        /// <param name="itemSketch">更新資訊</param>
        /// <returns>成功失敗訊息</returns>
        public ActionResponse<List<string>> TempDetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch)
        {
            ActionResponse<List<string>> result = new ActionResponse<List<string>>();
            result.Body = new List<string>();
            result.IsSuccess = true;
            result.Msg = string.Empty;
            try
            {
                var itemtempModel = db.ItemTemp.Where(x => x.ID == itemSketch.Item.ID).FirstOrDefault();

                // 根據審核狀態有不同方式修改           
                switch (itemtempModel.Status)
                {
                    default:
                    case 0:
                    case 1:
                        result = this.UpdateTempAndOffical(itemSketch);
                        break;
                    case 2:
                        // 舊品未通過 商品修改
                        if (itemSketch.Item.ItemID.HasValue && itemSketch.Product.ProductID.HasValue
                            && itemSketch.Item.ID != 0 && itemSketch.Product.ID != 0)
                        {
                            result = this.UpdateTempAndOffical(itemSketch);
                        }
                        else
                        {
                            // 未通過且ItemID and ProductID 無產生(新品未通過)
                            if (itemSketch.Item.ID != 0 && itemSketch.Product.ID != 0)
                            {
                                result = this.UpdateTemp_DetailEdit(itemSketch);
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                log.Error("Exception: " + ex.Message + ", StackTrace: " + ex.StackTrace);
                result.IsSuccess = false;
                result.Msg = "發生意外錯誤，請稍後再試";
                result.Body = null;
            }
            
                               
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        private ActionResponse<List<string>> UpdateTemp_DetailEdit(Models.ItemSketch itemSketch)
        {
            ActionResponse<List<string>> result = new ActionResponse<List<string>>();
            result.Body = new List<string>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            // 取得 Account Type
            ActionResponse<string> getAccountType = GetAccountType(itemSketch.Item.SellerID);
            ActionResponse<int> getDelvType = new ActionResponse<int>();

            // 取得 Account Type 成功，才取得 DelvType
            if (getAccountType.IsSuccess)
            {
                // 當遞送方式為供應商時，才將值更新成 Account Type
                if (itemSketch.Item.ShipType == "S")
                {
                    itemSketch.Item.ShipType = getAccountType.Body;
                }
               
                // 取得 DelvType
                getDelvType = GetDelvType(getAccountType.Body, itemSketch.Item.ShipType);
                
                // 取得 DelvType 成功，才開始更新資料
                if (getDelvType.IsSuccess)
                {
                    // 更新時間
                    DateTime updateTime = DateTime.Now;

                    #region PicStart、PicEnd

                    if (itemSketch.Product.PicPatch_Edit.Count > 0)
                    {
                        itemSketch.Product.PicStart = 1;
                        itemSketch.Product.PicEnd = itemSketch.Product.PicPatch_Edit.Count;
                    }
                    else
                    {
                        itemSketch.Product.PicStart = 0;
                        itemSketch.Product.PicEnd = 0;
                    }

                    #endregion PicStart、PicEnd

                    using (TransactionScope scope = new TransactionScope())
                    {                      
                        if (result.IsSuccess)
                        {
                            // 更新 Temp 表
                            ActionResponse<bool> updateTemp_DetailEdit = UpdateTemp_DetailEdit(itemSketch, getDelvType.Body, updateTime);

                            if (updateTemp_DetailEdit.IsSuccess == false)
                            {
                                result.IsSuccess = false;
                                log.Info("更新 Temp 表失敗。");
                            }
                        }

                        if (result.IsSuccess)
                        {
                            scope.Complete();

                            result.Msg = "儲存成功。";

                            // 若有傳入圖片，才儲存圖片
                            if (itemSketch.Product.PicPatch_Edit.Count > 0)
                            {
                                ImageService imgsService = new ImageService();
                               
                                #region 待審區圖片處理
                               
                                //待審的 ItemTemp、ProductTemp圖片，ItemTemp ID， ProductTemp ID
                                ActionResponse<bool> savePicResult_ItemTemp = imgsService.ImageProcess(itemSketch.Product.PicPatch_Edit, "pic\\itemtemp", "pic\\pic\\itemtemp", itemSketch.Item.ID);
                                if (savePicResult_ItemTemp.IsSuccess)
                                {
                                    log.Info("儲存 Temp 賣場圖片成功。");
                                }
                                else
                                {
                                    log.Info("儲存 Temp 賣場圖片失敗。");
                                }
                                                            
                                #endregion

                                // send Mail To who (包含 圖片 URL ItemID，ProductID，ItemTempID，ProductTempID)
                            }
                        }
                        else
                        {
                            scope.Dispose();
                            result.Msg = "儲存失敗。";
                            log.Info("更新詳細表編輯表失敗。");
                        }
                    }
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = getAccountType.Msg;
            }

            return result;
        }

        private ActionResponse<List<string>> UpdateTempAndOffical(Models.ItemSketch itemSketch)
        {
            ActionResponse<List<string>> result = new ActionResponse<List<string>>();
            result.Body = new List<string>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            // 取得 Account Type
            ActionResponse<string> getAccountType = GetAccountType(itemSketch.Item.SellerID);
            ActionResponse<int> getDelvType = new ActionResponse<int>();

            // 取得 Account Type 成功，才取得 DelvType
            if (getAccountType.IsSuccess)
            {
                // 當遞送方式為供應商時，才將值更新成 Account Type
                if (itemSketch.Item.ShipType == "S")
                {
                    itemSketch.Item.ShipType = getAccountType.Body;
                }
               
                // 取得 DelvType
                getDelvType = GetDelvType(getAccountType.Body, itemSketch.Item.ShipType);
                
                // 取得 DelvType 成功，才開始更新資料
                if (getDelvType.IsSuccess)
                {
                    // 更新時間
                    DateTime updateTime = DateTime.Now;

                    #region PicStart、PicEnd

                    if (itemSketch.Product.PicPatch_Edit.Count > 0)
                    {
                        itemSketch.Product.PicStart = 1;
                        itemSketch.Product.PicEnd = itemSketch.Product.PicPatch_Edit.Count;
                    }
                    else
                    {
                        itemSketch.Product.PicStart = 0;
                        itemSketch.Product.PicEnd = 0;
                    }

                    #endregion PicStart、PicEnd

                    using (TransactionScope scope = new TransactionScope())
                    {
                        // 更新正式表
                        ActionResponse<bool> updateOfficial_DetailEdit = UpdateOfficial_DetailEdit(itemSketch, getDelvType.Body, updateTime);

                        if (updateOfficial_DetailEdit.IsSuccess == false)
                        {
                            result.IsSuccess = false;
                            log.Info("更新正式表失敗。");
                        }

                        if (result.IsSuccess)
                        {
                            // 更新 Temp 表
                            ActionResponse<bool> updateTemp_DetailEdit = UpdateTemp_DetailEdit(itemSketch, getDelvType.Body, updateTime);

                            if (updateTemp_DetailEdit.IsSuccess == false)
                            {
                                result.IsSuccess = false;
                                log.Info("更新 Temp 表失敗。");
                            }
                        }

                        if (result.IsSuccess)
                        {
                            scope.Complete();

                            result.Msg = "商品資料儲存成功。";

                            // 若有傳入圖片，才儲存圖片
                            if (itemSketch.Product.PicPatch_Edit.Count > 0)
                            {
                                ImageService imgsService = new ImageService();

                                //正式的 Item、Product圖片，要傳入 Item ID， Product ID
                                ActionResponse<bool> savePicResult_Item = imgsService.ImageProcessItemAndProduct(itemSketch.Product.PicPatch_Edit, "pic\\item", "pic\\pic\\item", itemSketch.Item.ItemID.Value, itemSketch.Product.ProductID.Value, "pic\\pic\\product", true);

                                if (savePicResult_Item.IsSuccess)
                                {
                                    log.Info("儲存正式賣場圖片成功。");
                                }
                                else
                                {
                                    result.IsSuccess = false;
                                    result.Code = (int)ResponseCode.Error;
                                    result.Msg += "賣場圖片處理失敗，請刪除圖片重新上傳!";
                                    log.Info("儲存正式賣場圖片失敗。");
                                }
                               
                                //ActionResponse<bool> savePicResult_Item = imgsService.ImageProcess(itemSketch.Product.PicPatch_Edit, "pic\\item", "pic\\pic\\item", itemSketch.Item.ItemID.Value, true);
                                //if (savePicResult_Item.IsSuccess)
                                //{
                                //    log.Info("儲存正式賣場圖片成功。");
                                //}
                                //else
                                //{
                                //    result.IsSuccess = false;
                                //    result.Code = (int)ResponseCode.Error;
                                //    result.Msg += "賣場圖片處理失敗，請刪除圖片重新上傳!";
                                //    log.Info("儲存正式賣場圖片失敗。");
                                //}

                                //ActionResponse<bool> savePicResult_Product = imgsService.ImageProcess(itemSketch.Product.PicPatch_Edit, "pic\\product", "pic\\pic\\product", itemSketch.Product.ProductID.Value, true);
                                //if (savePicResult_Product.IsSuccess)
                                //{
                                //    log.Info("儲存正式商品圖片成功。");
                                //}
                                //else
                                //{
                                //    log.Info("儲存正式商品圖片失敗。");
                                //}

                                #region 待審區圖片處理

                                //*****************************************************************
                                //待審的 ItemTemp、ProductTemp圖片，ItemTemp ID， ProductTemp ID
                                ActionResponse<bool> savePicResult_ItemTemp = imgsService.ImageProcess(itemSketch.Product.PicPatch_Edit, "pic\\itemtemp", "pic\\pic\\itemtemp", itemSketch.Item.ID);
                                if (savePicResult_ItemTemp.IsSuccess)
                                {
                                    log.Info("儲存 Temp 賣場圖片成功。");
                                }
                                else
                                {
                                    result.IsSuccess = false;
                                    result.Code = (int)ResponseCode.Error;
                                    result.Msg += "賣場圖片處理失敗，請刪除圖片重新上傳!";
                                    log.Info("儲存 Temp 賣場圖片失敗。");
                                }

                                //ActionResponse<bool> savePicResult_ProductTemp = imgsService.ImageProcess(itemSketch.Product.PicPatch_Edit, "pic\\producttemp", "pic\\pic\\producttemp", itemSketch.Product.ID);
                                //if (savePicResult_ProductTemp.IsSuccess)
                                //{
                                //    log.Info("儲存 Temp 商品圖片成功。");
                                //}
                                //else
                                //{
                                //    log.Info("儲存 Temp 商品圖片失敗。");
                                //}
                                //*************************************************************

                                #endregion
                                // send Mail To who (包含 圖片 URL ItemID，ProductID，ItemTempID，ProductTempID)
                            }
                        }
                        else
                        {
                            scope.Dispose();
                            result.Msg = "儲存失敗。";
                            log.Info("更新詳細表編輯表失敗。");
                        }
                    }
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = getAccountType.Msg;
            }

            return result;
        }

        #region 更新正式表

        /// <summary>
        /// 更新正式表
        /// </summary>
        /// <param name="itemSketch">更新資訊</param>
        /// <param name="delvType">配送方式</param>
        /// <param name="updateDate">更新日期</param>
        /// <returns>成功失敗訊息</returns>
        private ActionResponse<bool> UpdateOfficial_DetailEdit(Models.ItemSketch itemSketch, int delvType, DateTime updateDate)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            // 更新商品
            ActionResponse<bool> updateProduct = UpdateProduct_DetailEdit(itemSketch, delvType, updateDate);

            if (updateProduct.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg += updateProduct.Msg;
            }

            if (result.IsSuccess)
            {
                // 更新賣場
                ActionResponse<bool> updateItem = UpdateItem_DetailEdit(itemSketch, delvType, updateDate);

                if (updateItem.IsSuccess == false)
                {
                    result.IsSuccess = false;
                    result.Msg += updateItem.Msg;
                }

                if (result.IsSuccess)
                {
                    // 更新庫存
                    ActionResponse<bool> updateItemStock = UpdateItemStock_DetailEdit(itemSketch, updateDate);

                    if (updateItemStock.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg += updateItemStock.Msg;
                    }

                    if (result.IsSuccess)
                    {
                        // 更新跨分類
                        ActionResponse<bool> updateItemCategory = UpdateItemCategory_DetailEdit(itemSketch, updateDate);

                        if (updateItemCategory.IsSuccess == false)
                        {
                            result.IsSuccess = false;
                            result.Msg += updateItemCategory.Msg;
                        }

                        if (result.IsSuccess)
                        {
                            // 更新商品屬性
                            ActionResponse<bool> updateProductProtery = UpdateProductProtery_DetailEdit(itemSketch);

                            if (updateProductProtery.IsSuccess == false)
                            {
                                result.IsSuccess = false;
                                result.Msg += updateProductProtery.Msg;
                            }
                        }
                    }
                }
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 更新商品
        /// </summary>
        /// <param name="itemSketch">更新資訊</param>
        /// <param name="delvType">配送方式</param>
        /// <param name="updateDate">更新日期</param>
        /// <returns>成功失敗訊息</returns>
        private ActionResponse<bool> UpdateProduct_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, int delvType, DateTime updateDate)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            if (itemSketch.Product.ProductID != null && itemSketch.Product.ProductID.Value > 0)
            {
                // 讀取資料庫內要被更新的賣場資訊
                ActionResponse<DB.TWSQLDB.Models.Product> getProduct = GetProduct(itemSketch.Product.ProductID.Value);

                if (getProduct.IsSuccess)
                {
                    // 組合要更新的資料
                    ActionResponse<Product_DetialEdit> makeProduct_DetailEdit = MakeProduct_DetailEdit(itemSketch, delvType, updateDate);

                    if (makeProduct_DetailEdit.IsSuccess)
                    {
                        AutoMapper.Mapper.CreateMap<Product_DetialEdit, DB.TWSQLDB.Models.Product>();
                        AutoMapper.Mapper.Map(makeProduct_DetailEdit.Body, getProduct.Body);
                    }

                    DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
                    dbFront.Entry(getProduct.Body).State = System.Data.EntityState.Modified;

                    try
                    {
                        dbFront.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        log.Info(string.Format("正式商品修改資料失敗(expection); ProductID = {0}; ErrorMessage = {1}; StackTrace = {2}", itemSketch.Product.ProductID.Value, GetExceptionMessage(ex), ex.StackTrace));
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = getProduct.Msg;
                }
            }
            else
            {
                result.IsSuccess = false;
                log.Info("商品 ID 不可為 null 或 小於等於 0。");
            }

            if (result.IsSuccess)
            {
                log.Info(string.Format("正式商品修改資料成功; ProductID = {0}.", itemSketch.Product.ProductID.Value));
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 更新賣場
        /// </summary>
        /// <param name="itemSketch">更新資訊</param>
        /// <param name="delvType">配送方式</param>
        /// <param name="updateDate">更新日期</param>
        /// <returns>成功失敗訊息</returns>
        private ActionResponse<bool> UpdateItem_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, int delvType, DateTime updateDate)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            if (itemSketch.Item.ItemID != null && itemSketch.Item.ItemID.Value > 0)
            {
                // 讀取資料庫內要被更新的賣場資訊
                ActionResponse<DB.TWSQLDB.Models.Item> getItem = GetItem(itemSketch.Item.ItemID.Value);

                if (getItem.IsSuccess)
                {
                    // 組合要更新的資料
                    ActionResponse<Item_DetialEdit> makeItem_DetailEdit = MakeItem_DetailEdit(itemSketch, delvType, updateDate);


                    if (makeItem_DetailEdit.IsSuccess)
                    {
                        AutoMapper.Mapper.CreateMap<Item_DetialEdit, DB.TWSQLDB.Models.Item>();
                        AutoMapper.Mapper.Map(makeItem_DetailEdit.Body, getItem.Body);
                    }


                    #region 不維護欄位，固定給預設值
                    getItem.Body.Qty = 0;
                    #endregion
                    
                    DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
                    dbFront.Entry(getItem.Body).State = System.Data.EntityState.Modified;

                    try
                    {
                        dbFront.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        log.Info(string.Format("正式賣場修改資料失敗(expection); ItemID = {0}; ErrorMessage = {1}; StackTrace = {2}.", itemSketch.Item.ItemID.Value, GetExceptionMessage(ex), ex.StackTrace));
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = getItem.Msg;
                }
            }
            else
            {
                result.IsSuccess = false;
                log.Info("賣場 ID 不可為 null 或 小於等於 0。");
            }

            if (result.IsSuccess)
            {
                log.Info(string.Format("正式賣場修改資料成功; ItemID = {0}.", itemSketch.Item.ItemID.Value));
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 更新庫存
        /// </summary>
        /// <param name="itemSketch">更新資訊</param>
        /// <param name="updateDate">更新日期</param>
        /// <returns>成功失敗訊息</returns>
        private ActionResponse<bool> UpdateItemStock_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, DateTime updateDate)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            if (itemSketch.Product.ProductID != null && itemSketch.Product.ProductID.Value > 0)
            {
                // 讀取資料庫內要被更新的庫存資訊
                ActionResponse<DB.TWSQLDB.Models.ItemStock> getItemStock = GetItemStock(itemSketch.Product.ProductID.Value);

                if (getItemStock.IsSuccess)
                {
                    // 組合要更新的資料
                    ActionResponse<ItemStock_DetialEdit> makeItemTemp_DetailEditResult = MakeItemStock_DetailEdit(itemSketch, updateDate);

                    if (makeItemTemp_DetailEditResult.IsSuccess)
                    {
                        AutoMapper.Mapper.CreateMap<ItemStock_DetialEdit, DB.TWSQLDB.Models.ItemStock>();
                        AutoMapper.Mapper.Map(makeItemTemp_DetailEditResult.Body, getItemStock.Body);
                    }

                    DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
                    dbFront.Entry(getItemStock.Body).State = System.Data.EntityState.Modified;

                    try
                    {
                        dbFront.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        log.Info(string.Format("正式庫存修改資料失敗(expection); ProductID = {0}; ErrorMessage = {1}; StackTrace = {2}.", itemSketch.Product.ProductID.Value, GetExceptionMessage(ex), ex.StackTrace));
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = getItemStock.Msg;
                }
            }
            else
            {
                result.IsSuccess = false;
                log.Info("商品 ID 不可為 null 或 小於等於 0。");
            }

            if (result.IsSuccess)
            {
                log.Info(string.Format("正式庫存修改資料成功; ProductID = {0}.", itemSketch.Product.ProductID.Value));
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 更新跨分類
        /// </summary>
        /// <param name="itemSketch">更新資訊</param>
        /// <param name="updateDate">更新日期</param>
        /// <returns>成功失敗訊息</returns>
        private ActionResponse<bool> UpdateItemCategory_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, DateTime updateDate)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            if (itemSketch.Item.ItemID.HasValue && itemSketch.Item.ItemID.Value > 0)
            {
                // 刪除此賣場的跨分類資訊
                ActionResponse<bool> deleteItemCagegory = DeleteItemCagegory(itemSketch.Item.ItemID.Value);

                if (deleteItemCagegory.IsSuccess)
                {
                    // 要新增的跨分類清單
                    List<int> categoryIDCell = new List<int>();

                    // 判斷第 1 個跨分類是否有值
                    if (itemSketch.ItemCategory.SubCategoryID_1_Layer2.HasValue && itemSketch.ItemCategory.SubCategoryID_1_Layer2.Value > 0)
                    {
                        categoryIDCell.Add(itemSketch.ItemCategory.SubCategoryID_1_Layer2.Value);
                    }

                    // 判斷第 2 個跨分類是否有值
                    if (itemSketch.ItemCategory.SubCategoryID_2_Layer2.HasValue && itemSketch.ItemCategory.SubCategoryID_2_Layer2.Value > 0)
                    {
                        categoryIDCell.Add(itemSketch.ItemCategory.SubCategoryID_2_Layer2.Value);
                    }

                    if (categoryIDCell.Count > 0)
                    {
                        DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();

                        foreach (int categoryID in categoryIDCell)
                        {
                            // 組合跨分類 Model
                            ActionResponse<DB.TWSQLDB.Models.ItemCategory> makeItemCategory_DB = MakeItemCategory_DB(itemSketch.Item.ItemID.Value, categoryID, itemSketch.CreateAndUpdate.UpdateUser.ToString(), updateDate);

                            if (makeItemCategory_DB.IsSuccess)
                            {
                                dbFront.ItemCategory.Add(makeItemCategory_DB.Body);
                            }
                            else
                            {
                                // 組合跨分類 Model 失敗
                                result.IsSuccess = false;
                            }
                        }

                        // 組合跨分類 Model 成功，才進行 SaveChanges
                        if (result.IsSuccess)
                        {
                            try
                            {
                                dbFront.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                result.IsSuccess = false;
                                log.Info(string.Format("正式跨分類修改資料失敗(expection); ItemID = {0}; CategoryID_1 = {1}; CategoryID_2 = {2}; ErrorMessage = {3}; StackTrace = {4}.",
                                    itemSketch.Item.ItemID.Value,
                                    itemSketch.ItemCategory.SubCategoryID_1_Layer2.HasValue ? itemSketch.ItemCategory.SubCategoryID_1_Layer2.Value.ToString() : "null",
                                    itemSketch.ItemCategory.SubCategoryID_2_Layer2.HasValue ? itemSketch.ItemCategory.SubCategoryID_2_Layer2.Value.ToString() : "null",
                                    GetExceptionMessage(ex),
                                    ex.StackTrace));
                            }
                        }
                    }
                }
                else
                {
                    // 刪除跨分類失敗
                    result.IsSuccess = false;
                }
            }
            else
            {
                result.IsSuccess = false;
                log.Info("賣場 ID 不可為 null 或 小於等於 0。");
            }

            if (result.IsSuccess)
            {
                log.Info(string.Format("正式跨分類修改資料成功; ItemID = {0}.", itemSketch.Item.ItemID.Value));
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 更新商品屬性
        /// </summary>
        /// <param name="itemSketch">更新資訊</param>
        /// <returns>成功失敗訊息</returns>
        private ActionResponse<bool> UpdateProductProtery_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            if (itemSketch.Product.ProductID.HasValue && itemSketch.Product.ProductID.Value > 0)
            {
                TWNewEgg.BackendService.Service.CategoryPropertyService categoryPropertyService = new BackendService.Service.CategoryPropertyService();

                // 轉換輸入 Model
                AutoMapper.Mapper.CreateMap<TWNewEgg.API.Models.SaveProductProperty, TWNewEgg.BackendService.Models.SaveProductProperty>();
                List<TWNewEgg.BackendService.Models.SaveProductProperty> saveProductProperty = AutoMapper.Mapper.Map<List<TWNewEgg.BackendService.Models.SaveProductProperty>>(itemSketch.SaveProductPropertyList);

                TWNewEgg.BackendService.Models.PropertySaveModel saveProperty = new BackendService.Models.PropertySaveModel();

                saveProperty.CategoryID = itemSketch.ItemCategory.MainCategoryID_Layer2.Value;
                saveProperty.ProductID = itemSketch.Product.ProductID.Value;
                saveProperty.SaveProductProperty = saveProductProperty;

                // 更新商品屬性
                TWNewEgg.BackendService.Models.ActionResponse<string> saveProductPropertyResult = categoryPropertyService.SaveProductPropertyClick(saveProperty);

                if (saveProductPropertyResult.IsSuccess == false)
                {
                    result.IsSuccess = false;
                    log.Info(string.Format("正式商品屬性修改資料失敗; ProductID = {0}; SaveProductPropertyClick_Message = {1}.", itemSketch.Product.ProductID.Value, saveProductPropertyResult.Msg));
                }
            }
            else
            {
                result.IsSuccess = false;
                log.Info("商品 ID 不可為 null 或 小於等於 0。");
            }

            if (result.IsSuccess)
            {
                log.Info(string.Format("正式商品屬性修改資料成功; ProductID = {0}.", itemSketch.Product.ProductID.Value));
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        #endregion 更新正式表

        #region 更新 temp 表

        /// <summary>
        /// 更新 temp 表
        /// </summary>
        /// <param name="itemSketch">更新資訊</param>
        /// <param name="delvType">配送方式</param>
        /// <param name="updateDate">更新日期</param>
        /// <returns>成功失敗訊息</returns>
        private ActionResponse<bool> UpdateTemp_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, int delvType, DateTime updateDate)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            // Temp 待審狀態變更
            ActionResponse<bool> updateTempStatus_DetailEdit = UpdateTempStatus_DetailEdit(itemSketch, updateDate);

            if (updateTempStatus_DetailEdit.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg += updateTempStatus_DetailEdit.Msg;
            }

            if (result.IsSuccess)
            {
                // 更新商品Temp
                ActionResponse<bool> updateProductTemp_DetailEdit = UpdateProductTemp_DetailEdit(itemSketch, delvType, updateDate);

                if (updateProductTemp_DetailEdit.IsSuccess == false)
                {
                    result.IsSuccess = false;
                    result.Msg += updateProductTemp_DetailEdit.Msg;
                }

                if (result.IsSuccess)
                {
                    // 更新賣場Temp
                    ActionResponse<bool> updateItemTemp_DetailEdit = UpdateItemTemp_DetailEdit(itemSketch, delvType, updateDate);

                    if (updateItemTemp_DetailEdit.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg += updateItemTemp_DetailEdit.Msg;
                    }

                    if (result.IsSuccess)
                    {
                        // 更新庫存Temp
                        ActionResponse<bool> updateItemStockTemp_DetailEdit = UpdateItemStockTemp_DetailEdit(itemSketch, updateDate);

                        if (updateItemStockTemp_DetailEdit.IsSuccess == false)
                        {
                            result.IsSuccess = false;
                            result.Msg += updateItemStockTemp_DetailEdit.Msg;
                        }

                        if (result.IsSuccess)
                        {
                            // 更新跨分類Temp
                            ActionResponse<bool> updateItemCategoryTemp_DetailEdit = UpdateItemCategoryTemp_DetailEdit(itemSketch, updateDate);

                            if (updateItemCategoryTemp_DetailEdit.IsSuccess == false)
                            {
                                result.IsSuccess = false;
                                result.Msg += updateItemCategoryTemp_DetailEdit.Msg;
                            }

                            if (result.IsSuccess)
                            {
                                // 更新商品屬性Temp
                                ActionResponse<bool> updateProductProteryTemp_DetailEdit = UpdateProductProteryTemp_DetailEdit(itemSketch);

                                if (updateProductProteryTemp_DetailEdit.IsSuccess == false)
                                {
                                    result.IsSuccess = false;
                                    result.Msg += updateProductProteryTemp_DetailEdit.Msg;
                                }
                            }
                        }
                    }
                }
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// Temp 待審狀態變更
        /// </summary>
        /// <param name="itemSketch">更新資訊</param>
        /// <param name="updateDate">更新日期</param>
        /// <returns>成功失敗訊息</returns>
        private ActionResponse<bool> UpdateTempStatus_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, DateTime updateDate)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;
            
            // 因為會在畫面進行 Name 的 URLEncode，因此需要將 Name Decode 回來
            itemSketch.Product.Name = HttpUtility.UrlDecode(itemSketch.Product.Name);
            
            if (itemSketch.Item.ID > 0 && itemSketch.Product.ID > 0)
            {
                ActionResponse<DB.TWSQLDB.Models.ItemTemp> getItemTemp = GetItemTemp(itemSketch.Item.ID);
                ActionResponse<DB.TWSQLDB.Models.ProductTemp> getProductTemp = GetProductTemp(itemSketch.Product.ID);

                if (getItemTemp.IsSuccess && getProductTemp.IsSuccess)
                {
                    DB.TWSQLDB.Models.ItemTemp itemTemp = getItemTemp.Body;
                    DB.TWSQLDB.Models.ProductTemp productTemp = getProductTemp.Body;

                    // 是否有修改值(用於判斷是否變更審核狀態)
                    bool isChangeValue = false;

                    // 修改商品名稱
                    if (itemTemp.Name != itemSketch.Product.Name)
                    {
                        isChangeValue = true;

                        //productTemp.Name = itemSketch.Product.Name;
                        //productTemp.NameTW = itemSketch.Product.Name;
                        itemTemp.Name = itemSketch.Product.Name;
                    }

                    // 修改售價
                    if ((itemSketch.Item.PriceCash.HasValue && itemTemp.PriceCash != itemSketch.Item.PriceCash.Value) || (itemSketch.Item.PriceCash.HasValue && itemTemp.PriceCard != itemSketch.Item.PriceCash.Value))
                    {
                        isChangeValue = true;

                        itemTemp.PriceCash = itemSketch.Item.PriceCash.Value;
                        itemTemp.PriceCard = itemSketch.Item.PriceCash.Value;
                    }

                    // 修改成本
                    if (itemSketch.Product.Cost.HasValue && productTemp.Cost != itemSketch.Product.Cost)
                    {
                        isChangeValue = true;

                        productTemp.Cost = itemSketch.Product.Cost;
                    }

                    // 未通過又是新品建立，直接給未審核

                    if (getItemTemp.Body.Status == 2 && !getItemTemp.Body.ItemID.HasValue && !getProductTemp.Body.ProductID.HasValue)
                    {
                        isChangeValue = true;
                    }

                    if (isChangeValue)
                    {
                        //變更狀態時計算毛利率
                        #region 計算毛利率

                        decimal? grossMargin = null;

                        if (itemSketch.Product.Cost == null || itemSketch.Item.PriceCash == null || itemSketch.Item.PriceCash.Value == 0)
                        {
                            grossMargin = 0;
                        }
                        else
                        {
                            grossMargin = ((itemSketch.Item.PriceCash.Value - itemSketch.Product.Cost.Value) / itemSketch.Item.PriceCash.Value) * 100m;
                            grossMargin = Decimal.Round(grossMargin.Value, 0);
                        }

                        itemTemp.GrossMargin = grossMargin;

                        #endregion 計算毛利率


                        // 變更審核狀態
                        itemTemp.Status = 1;
                        productTemp.Status = 1;

                        //更新送審日期、送審人
                        itemTemp.SubmitDate = updateDate;
                        itemTemp.SubmitMan = itemSketch.CreateAndUpdate.UpdateUser.ToString();

                        // 更新更新人、更新日期
                        itemTemp.UpdateUser = itemSketch.CreateAndUpdate.UpdateUser.ToString();
                        itemTemp.UpdateDate = updateDate;
                        productTemp.UpdateUser = itemSketch.CreateAndUpdate.UpdateUser.ToString();
                        productTemp.UpdateDate = updateDate;

                        DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
                        dbFront.Entry(itemTemp).State = EntityState.Modified;
                        dbFront.Entry(productTemp).State = EntityState.Modified;

                        try
                        {
                            dbFront.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            result.IsSuccess = false;
                            log.Info(string.Format("Temp 待審狀態變更失敗(expection); ItemTempID = {0}; ProductTempID = {1}; ErrorMessage = {2}; StackTrace = {3}.", itemSketch.Item.ID, itemSketch.Product.ID, GetExceptionMessage(ex), ex.StackTrace));
                        }
                    }
                }
                else
                {
                    result.IsSuccess = false;

                    if (getItemTemp.IsSuccess == false)
                    {
                        result.Msg += getItemTemp.Msg;
                    }

                    if (getProductTemp.IsSuccess == false)
                    {
                        result.Msg += getProductTemp.Msg;
                    }
                }
            }
            else
            {
                if (itemSketch.Item.ID <= 0)
                {
                    result.IsSuccess = false;
                    log.Info("ItemTempID 不可小於等於 0。");
                }

                if (itemSketch.Product.ID <= 0)
                {
                    result.IsSuccess = false;
                    log.Info("ProductTempID 不可小於等於 0。");
                }
            }

            if (result.IsSuccess)
            {
                log.Info(string.Format("Temp 待審狀態變更成功。"));
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 更新商品Temp
        /// </summary>
        /// <param name="itemSketch">更新資訊</param>
        /// <param name="delvType">配送方式</param>
        /// <param name="updateDate">更新日期</param>
        /// <returns>成功失敗訊息</returns>
        private ActionResponse<bool> UpdateProductTemp_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, int delvType, DateTime updateDate)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            if (itemSketch.Product.ID > 0)
            {
                // 讀取資料庫內要被更新的賣場資訊
                ActionResponse<DB.TWSQLDB.Models.ProductTemp> getProductTemp = GetProductTemp(itemSketch.Product.ID);

                if (getProductTemp.IsSuccess)
                {
                    // 組合要更新的資料
                    ActionResponse<Product_DetialEdit> makeProduct_DetailEdit = MakeProduct_DetailEdit(itemSketch, delvType, updateDate);

                    if (makeProduct_DetailEdit.IsSuccess)
                    {
                        AutoMapper.Mapper.CreateMap<Product_DetialEdit, DB.TWSQLDB.Models.ProductTemp>();
                        AutoMapper.Mapper.Map(makeProduct_DetailEdit.Body, getProductTemp.Body);
                    }

                    DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
                    dbFront.Entry(getProductTemp.Body).State = System.Data.EntityState.Modified;

                    try
                    {
                        dbFront.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        log.Info(string.Format("Temp 商品修改資料失敗(expection); ProductTempID = {0}; ErrorMessage = {1}; StackTrace = {2}", itemSketch.Product.ID, GetExceptionMessage(ex), ex.StackTrace));
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = getProductTemp.Msg;
                }
            }
            else
            {
                result.IsSuccess = false;
                log.Info("商品 TempID 不可於小於等於 0。");
            }

            if (result.IsSuccess)
            {
                log.Info(string.Format("Temp 商品修改資料成功; ProductTempID = {0}.", itemSketch.Product.ID));
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 更新賣場Temp
        /// </summary>
        /// <param name="itemSketch">更新資訊</param>
        /// <param name="delvType">配送方式</param>
        /// <param name="updateDate">更新日期</param>
        /// <returns>成功失敗訊息</returns>
        private ActionResponse<bool> UpdateItemTemp_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, int delvType, DateTime updateDate)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            if (itemSketch.Item.ID > 0)
            {
                // 讀取資料庫內要被更新的賣場資訊
                ActionResponse<DB.TWSQLDB.Models.ItemTemp> getItemTemp = GetItemTemp(itemSketch.Item.ID);

                if (getItemTemp.IsSuccess)
                {
                    // 組合要更新的資料
                    ActionResponse<Item_DetialEdit> makeItem_DetailEdit = MakeItem_DetailEdit(itemSketch, delvType, updateDate);

                    if (makeItem_DetailEdit.IsSuccess)
                    {
                        AutoMapper.Mapper.CreateMap<Item_DetialEdit, DB.TWSQLDB.Models.ItemTemp>()
                            .ForMember(x => x.ItemTempDesc, x => x.MapFrom(src => src.ItemDesc))
                            .ForMember(x => x.DelvData, x => x.MapFrom(src => src.DelvDate))
                            .ForMember(x => x.GrossMargin, x => x.Ignore());
                        AutoMapper.Mapper.Map(makeItem_DetailEdit.Body, getItemTemp.Body);
                    }

                    DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
                    dbFront.Entry(getItemTemp.Body).State = System.Data.EntityState.Modified;

                    #region 不維護欄位，給定預設值

                    getItemTemp.Body.Qty = 0;

                    #endregion

                    try
                    {
                        dbFront.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        log.Info(string.Format("Temp 賣場修改資料失敗(expection); ItemTempID = {0}; ErrorMessage = {1}; StackTrace = {2}.", itemSketch.Item.ID, GetExceptionMessage(ex), ex.StackTrace));
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = getItemTemp.Msg;
                }
            }
            else
            {
                result.IsSuccess = false;
                log.Info("賣場 TempID 不可小於等於 0。");
            }

            if (result.IsSuccess)
            {
                log.Info(string.Format("Temp 賣場修改資料成功; ItemTempID = {0}.", itemSketch.Item.ID));
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 更新庫存Temp
        /// </summary>
        /// <param name="itemSketch">更新資訊</param>
        /// <param name="updateDate">更新日期</param>
        /// <returns>成功失敗訊息</returns>
        private ActionResponse<bool> UpdateItemStockTemp_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, DateTime updateDate)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            if (itemSketch.Product.ID > 0)
            {
                // 讀取資料庫內要被更新的庫存資訊
                ActionResponse<DB.TWSQLDB.Models.ItemStocktemp> getItemStockTemp = GetItemStockTemp(itemSketch.Product.ID);

                if (getItemStockTemp.IsSuccess)
                {
                    // 組合要更新的資料
                    ActionResponse<ItemStock_DetialEdit> makeItemStock_DetailEdit = MakeItemStock_DetailEdit(itemSketch, updateDate);

                    if (makeItemStock_DetailEdit.IsSuccess)
                    {
                        AutoMapper.Mapper.CreateMap<ItemStock_DetialEdit, DB.TWSQLDB.Models.ItemStocktemp>();
                        AutoMapper.Mapper.Map(makeItemStock_DetailEdit.Body, getItemStockTemp.Body);
                    }

                    getItemStockTemp.Body.UpdateDate = DateTime.Now;

                    DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
                    dbFront.Entry(getItemStockTemp.Body).State = System.Data.EntityState.Modified;

                    try
                    {
                        dbFront.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        log.Info(string.Format("Temp 庫存修改資料失敗(expection); ProductTempID = {0}; ErrorMessage = {1}; StackTrace = {2}.", itemSketch.Product.ID, GetExceptionMessage(ex), ex.StackTrace));
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = getItemStockTemp.Msg;
                }
            }
            else
            {
                result.IsSuccess = false;
                log.Info("商品 TempID 不可小於等於 0。");
            }

            if (result.IsSuccess)
            {
                log.Info(string.Format("Temp 庫存修改資料成功; ProductTempID = {0}.", itemSketch.Product.ID));
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 更新跨分類Temp
        /// </summary>
        /// <param name="itemSketch">更新資訊</param>
        /// <param name="updateDate">更新日期</param>
        /// <returns>成功失敗訊息</returns>
        private ActionResponse<bool> UpdateItemCategoryTemp_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, DateTime updateDate)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            if (itemSketch.Item.ID > 0)
            {
                // 刪除此賣場的跨分類資訊
                ActionResponse<bool> deleteItemCagegoryTemp = DeleteItemCagegoryTemp(itemSketch.Item.ID);

                if (deleteItemCagegoryTemp.IsSuccess)
                {
                    // 要新增的跨分類清單
                    List<int> categoryIDCell = new List<int>();

                    // 判斷第 1 個跨分類是否有值
                    if (itemSketch.ItemCategory.SubCategoryID_1_Layer2.HasValue && itemSketch.ItemCategory.SubCategoryID_1_Layer2.Value > 0)
                    {
                        categoryIDCell.Add(itemSketch.ItemCategory.SubCategoryID_1_Layer2.Value);
                    }

                    // 判斷第 2 個跨分類是否有值
                    if (itemSketch.ItemCategory.SubCategoryID_2_Layer2.HasValue && itemSketch.ItemCategory.SubCategoryID_2_Layer2.Value > 0)
                    {
                        categoryIDCell.Add(itemSketch.ItemCategory.SubCategoryID_2_Layer2.Value);
                    }

                    if (categoryIDCell.Count > 0)
                    {
                        DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();

                        foreach (int categoryID in categoryIDCell)
                        {
                            // 組合跨分類 Model
                            ActionResponse<DB.TWSQLDB.Models.ItemCategorytemp> makeItemCategory_DB = MakeItemCategoryTemp_DB(itemSketch.Item.ID, itemSketch.Item.ItemID, categoryID, itemSketch.CreateAndUpdate.UpdateUser.ToString(), updateDate);

                            if (makeItemCategory_DB.IsSuccess)
                            {
                                dbFront.ItemCategorytemp.Add(makeItemCategory_DB.Body);
                            }
                            else
                            {
                                // 組合跨分類 Model 失敗
                                result.IsSuccess = false;
                            }
                        }

                        // 組合跨分類 Model 成功，才進行 SaveChanges
                        if (result.IsSuccess)
                        {
                            try
                            {
                                dbFront.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                result.IsSuccess = false;
                                log.Info(string.Format("Temp 跨分類修改資料失敗(expection); ItemTempID = {0}; CategoryID_1 = {1}; CategoryID_2 = {2}; ErrorMessage = {3}; StackTrace = {4}.",
                                    itemSketch.Item.ID,
                                    itemSketch.ItemCategory.SubCategoryID_1_Layer2.HasValue ? itemSketch.ItemCategory.SubCategoryID_1_Layer2.Value.ToString() : "null",
                                    itemSketch.ItemCategory.SubCategoryID_2_Layer2.HasValue ? itemSketch.ItemCategory.SubCategoryID_2_Layer2.Value.ToString() : "null",
                                    GetExceptionMessage(ex),
                                    ex.StackTrace));
                            }
                        }
                    }
                }
                else
                {
                    // 刪除跨分類失敗
                    result.IsSuccess = false;
                }
            }
            else
            {
                result.IsSuccess = false;
                log.Info("賣場 TempID 不可小於等於 0。");
            }

            if (result.IsSuccess)
            {
                log.Info(string.Format("Temp 跨分類修改資料成功; ItemTempID = {0}.", itemSketch.Item.ID));
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 更新商品屬性Temp
        /// </summary>
        /// <param name="itemSketch">更新資訊</param>
        /// <returns>成功失敗訊息</returns>
        private ActionResponse<bool> UpdateProductProteryTemp_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            if (itemSketch.Product.ID > 0)
            {
                ProductPorpertyTempService productPorpertyTempService = new ProductPorpertyTempService();
                ActionResponse<string> saveProductPropertyClick = productPorpertyTempService.SaveProductPropertyTempClick(itemSketch.SaveProductPropertyList, itemSketch.Product.ID, itemSketch.CreateAndUpdate.UpdateUser);
                if (saveProductPropertyClick.IsSuccess == false)
                {
                    result.IsSuccess = false;
                    log.Info(string.Format("Temp 商品屬性修改資料失敗; ProductID = {0}.", itemSketch.Product.ID));
                }
            }
            else
            {
                result.IsSuccess = false;
                log.Info("商品 TempID 不可小於等於 0。");
            }

            if (result.IsSuccess)
            {
                log.Info(string.Format("Temp 商品屬性修改資料成功; ProductID = {0}.", itemSketch.Product.ID));
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        #endregion 更新 temp 表

        #region 讀取資料庫資料

        /// <summary>
        /// 刪除跨分類
        /// </summary>
        /// <param name="itemID">賣場 ID</param>
        /// <returns>失敗成功訊息</returns>
        private ActionResponse<bool> DeleteItemCagegory(int itemID)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
            List<DB.TWSQLDB.Models.ItemCategory> deleteList = dbFront.ItemCategory.Where(x => x.ItemID == itemID && x.FromSystem == "1").ToList();

            if (deleteList.Capacity > 0)
            {
                foreach (DB.TWSQLDB.Models.ItemCategory itemCategory in deleteList)
                {
                    dbFront.ItemCategory.Remove(itemCategory);
                }

                try
                {
                    dbFront.SaveChanges();
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    log.Info(string.Format("刪除正式跨分類失敗(expection); ItemID = {0}; ErrorMessage = {1}; StackTrace = {2}.", itemID, GetExceptionMessage(ex), ex.StackTrace));
                }

                // 記錄刪除訊息
                if (result.IsSuccess)
                {
                    foreach (DB.TWSQLDB.Models.ItemCategory itemCategory in deleteList)
                    {
                        log.Info(string.Format("刪除正式跨分類成功，刪除資訊：ItemID = {0}, CategoryID = {1}, CreateUser = {2}, CreateDate = {3}, UpdateUser = {4}, UpdateDate = {5}.",
                            itemCategory.ItemID,
                            itemCategory.CategoryID,
                            itemCategory.CreateUser,
                            itemCategory.CreateDate,
                            itemCategory.UpdateUser,
                            itemCategory.UpdateDate));
                    }
                }
            }
            else
            {
                log.Info(string.Format("查無正式跨分類項目; ItemID = {0}.", itemID));
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 刪除跨分類Temp
        /// </summary>
        /// <param name="itemTempID">賣場 TempID</param>
        /// <returns>失敗成功訊息</returns>
        private ActionResponse<bool> DeleteItemCagegoryTemp(int itemTempID)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
            List<DB.TWSQLDB.Models.ItemCategorytemp> deleteList = dbFront.ItemCategorytemp.Where(x => x.itemtempID == itemTempID && x.FromSystem == "1").ToList();

            if (deleteList.Count > 0)
            {
                foreach (DB.TWSQLDB.Models.ItemCategorytemp itemCategory in deleteList)
                {
                    dbFront.ItemCategorytemp.Remove(itemCategory);
                }

                try
                {
                    dbFront.SaveChanges();
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    log.Info(string.Format("刪除 Temp 跨分類失敗(expection); ItemTempID = {0}; ErrorMessage = {1}; StackTrace = {2}.", itemTempID, GetExceptionMessage(ex), ex.StackTrace));
                }

                // 記錄刪除訊息
                if (result.IsSuccess)
                {
                    foreach (DB.TWSQLDB.Models.ItemCategorytemp itemCategorytemp in deleteList)
                    {
                        log.Info(string.Format("刪除 Temp 跨分類成功，刪除資訊：ItemTempID = {0}, CategoryID = {1}, CreateUser = {2}, CreateDate = {3}, UpdateUser = {4}, UpdateDate = {5}.",
                            itemCategorytemp.itemtempID,
                            itemCategorytemp.CategoryID,
                            itemCategorytemp.CreateUser,
                            itemCategorytemp.CreateDate,
                            itemCategorytemp.UpdateUser,
                            itemCategorytemp.UpdateDate));
                    }
                }
            }
            else
            {
                log.Info(string.Format("查無 Temp 跨分類項目; ItemTempID = {0}.", itemTempID));
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 取得 Account Type
        /// </summary>
        /// <param name="sellerID">商家 ID</param>
        /// <returns>Account Type</returns>
        private ActionResponse<string> GetAccountType(int? sellerID)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            result.Body = string.Empty;
            result.IsSuccess = true;
            result.Msg = string.Empty;

            DB.TWSellerPortalDBContext dbSellerPortal = new DB.TWSellerPortalDBContext();

            if (sellerID.HasValue && sellerID.Value != 0)
            {
                try
                {
                    result.Body = dbSellerPortal.Seller_BasicInfo.Where(x => x.SellerID == sellerID).Select(x => x.AccountTypeCode).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    log.Info(string.Format("取得 AccountTypeCode 失敗(expection); SellerID = {0}; ErrorMessage = {1}; StackTrace = {2}.", sellerID, GetExceptionMessage(ex), ex.StackTrace));
                }
            }
            else
            {
                result.IsSuccess = false;

                if (sellerID.Value == 0)
                {
                    log.Info("商家 ID 不可為 0，取得 Account Type 失敗。");
                }

                if (sellerID == null)
                {
                    log.Info("未輸入商家 ID，取得 Account Type 失敗。");
                }
            }

            if (string.IsNullOrEmpty(result.Body))
            {
                result.IsSuccess = false;
                result.Msg = "取得商家類別失敗。";
                log.Info(string.Format("查無 SellerID = {0} 資料，取得 Account Type 失敗。", sellerID));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 取得 DelvType
        /// </summary>
        /// <param name="accountType">Account Type</param>
        /// <param name="shipType">Ship Type</param>
        /// <returns>DelvType</returns>
        private ActionResponse<int> GetDelvType(string accountType, string shipType)
        {
            ActionResponse<int> result = new ActionResponse<int>();
            result.Body = -1;
            result.IsSuccess = true;
            result.Msg = string.Empty;

            if (accountType == "S")
            {
                // 由運送方決定DelvType
                switch (shipType)
                {
                    case "V":
                    case "S":
                        {
                            result.Body = 2;
                            break;
                        }
                    case "N":
                        {
                            result.Body = 8;
                            break;
                        }
                }
            }

            if (accountType == "V")
            {
                // 由運送方決定DelvType
                switch (shipType)
                {
                    case "V":
                    case "S":
                        {
                            result.Body = 7;
                            break;
                        }
                    case "N":
                        {
                            result.Body = 9;
                            break;
                        }
                }
            }

            if (result.Body == -1)
            {
                result.IsSuccess = false;
                result.Msg = "讀取運送方式失敗。";
                log.Info(string.Format("DelvType 給值失敗：AccountType = {0}， ShipType = {1}。", accountType, shipType));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 取得賣場
        /// </summary>
        /// <param name="itemID">賣場 ID</param>
        /// <returns>賣場</returns>
        private ActionResponse<DB.TWSQLDB.Models.Item> GetItem(int itemID)
        {
            ActionResponse<DB.TWSQLDB.Models.Item> result = new ActionResponse<DB.TWSQLDB.Models.Item>();
            result.Body = new DB.TWSQLDB.Models.Item();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();

            try
            {
                result.Body = dbFront.Item.Where(x => x.ID == itemID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = "取得賣場資訊失敗。";
                log.Info(string.Format("取得賣場失敗(expection); ItemID = {0}; ErrorMessage = {1}; StackTrace = {2}.", itemID, GetExceptionMessage(ex), ex.StackTrace));
            }

            if (result.Body == null)
            {
                result.IsSuccess = false;
                result.Msg = "查無賣場資訊。";
                log.Info(string.Format("查無賣場資訊; ItemID = {0}.", itemID));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 取得賣場Temp
        /// </summary>
        /// <param name="itemTempID">賣場 TempID</param>
        /// <returns>賣場Temp</returns>
        private ActionResponse<DB.TWSQLDB.Models.ItemTemp> GetItemTemp(int itemTempID)
        {
            ActionResponse<DB.TWSQLDB.Models.ItemTemp> result = new ActionResponse<DB.TWSQLDB.Models.ItemTemp>();
            result.Body = new DB.TWSQLDB.Models.ItemTemp();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();

            try
            {
                result.Body = dbFront.ItemTemp.Where(x => x.ID == itemTempID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = "取得賣場資訊失敗。(T)";
                log.Info(string.Format("取得賣場Temp失敗(expection); ItemTempID = {0}; ErrorMessage = {1}; StackTrace = {2}.", itemTempID, GetExceptionMessage(ex), ex.StackTrace));
            }

            if (result.Body == null)
            {
                result.IsSuccess = false;
                result.Msg = "查無賣場資訊。(T)";
                log.Info(string.Format("查無賣場Temp; ItemTempID = {0}.", itemTempID));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 取得庫存
        /// </summary>
        /// <param name="productID">商品 ID</param>
        /// <returns>庫存</returns>
        private ActionResponse<DB.TWSQLDB.Models.ItemStock> GetItemStock(int productID)
        {
            ActionResponse<DB.TWSQLDB.Models.ItemStock> result = new ActionResponse<DB.TWSQLDB.Models.ItemStock>();
            result.Body = new DB.TWSQLDB.Models.ItemStock();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();

            try
            {
                result.Body = dbFront.ItemStock.Where(x => x.ProductID == productID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = "取得庫存資訊失敗。";
                log.Info(string.Format("取得庫存失敗(expection); ProductID = {0}; ErrorMessage = {1}; StackTrace = {2}.", productID, GetExceptionMessage(ex), ex.StackTrace));
            }

            if (result.Body == null)
            {
                result.IsSuccess = false;
                result.Msg = "查無庫存資訊。";
                log.Info(string.Format("查無庫存; ProductID = {0}.", productID));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 取得庫存Temp
        /// </summary>
        /// <param name="productTempID">商品 TempID</param>
        /// <returns>庫存</returns>
        private ActionResponse<DB.TWSQLDB.Models.ItemStocktemp> GetItemStockTemp(int productTempID)
        {
            ActionResponse<DB.TWSQLDB.Models.ItemStocktemp> result = new ActionResponse<DB.TWSQLDB.Models.ItemStocktemp>();
            result.Body = new DB.TWSQLDB.Models.ItemStocktemp();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();

            try
            {
                result.Body = dbFront.ItemStocktemp.Where(x => x.producttempID == productTempID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = "取得庫存資訊失敗。(T)";
                log.Info(string.Format("取得庫存失敗(expection); ProductTempID = {0}; ErrorMessage = {1}; StackTrace = {2}.", productTempID, GetExceptionMessage(ex), ex.StackTrace));
            }

            if (result.Body == null)
            {
                result.IsSuccess = false;
                result.Msg = "查無庫存資訊。(T)";
                log.Info(string.Format("查無庫存; ProductTempID = {0}.", productTempID));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 取得商品
        /// </summary>
        /// <param name="productID">商品 ID</param>
        /// <returns>商品</returns>
        private ActionResponse<DB.TWSQLDB.Models.Product> GetProduct(int productID)
        {
            ActionResponse<DB.TWSQLDB.Models.Product> result = new ActionResponse<DB.TWSQLDB.Models.Product>();
            result.Body = new DB.TWSQLDB.Models.Product();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();

            try
            {
                result.Body = dbFront.Product.Where(x => x.ID == productID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = "取得商品資訊失敗。";
                log.Info(string.Format("取得商品失敗(expection); ProductID = {0}; ErrorMessage = {1}; StackTrace = {2}.", productID, GetExceptionMessage(ex), ex.StackTrace));
            }

            if (result.Body == null)
            {
                result.IsSuccess = false;
                result.Msg = "查無商品資訊。";
                log.Info(string.Format("查無商品; ProductID = {0}.", productID));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 取得商品Temp
        /// </summary>
        /// <param name="productTempID">商品 TempID</param>
        /// <returns>商品Temp</returns>
        private ActionResponse<DB.TWSQLDB.Models.ProductTemp> GetProductTemp(int productTempID)
        {
            ActionResponse<DB.TWSQLDB.Models.ProductTemp> result = new ActionResponse<DB.TWSQLDB.Models.ProductTemp>();
            result.Body = new DB.TWSQLDB.Models.ProductTemp();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();

            try
            {
                result.Body = dbFront.ProductTemp.Where(x => x.ID == productTempID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = "取得商品資訊失敗。(T)";
                log.Info(string.Format("取得商品Temp失敗(expection); ProductTempID = {0}; ErrorMessage = {1}; StackTrace = {2}.", productTempID, GetExceptionMessage(ex), ex.StackTrace));
            }

            if (result.Body == null)
            {
                result.IsSuccess = false;
                result.Msg = "查無商品資訊。(T)";
                log.Info(string.Format("查無商品Temp; ProductTempID = {0}.", productTempID));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        #endregion 讀取資料庫資料

        #region 組合 Model

        /// <summary>
        /// 組合正式 Product (詳細表)
        /// </summary>
        /// <param name="itemSketch">商品資訊</param>
        /// <param name="delvType">配送方式</param>
        /// <param name="updateDate">更新日期</param>
        /// <returns>正式 Product (詳細表)</returns>
        private ActionResponse<Product_DetialEdit> MakeProduct_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, int delvType, DateTime updateDate)
        {
            ActionResponse<Product_DetialEdit> result = new ActionResponse<Product_DetialEdit>();
            result.Body = new Product_DetialEdit();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            try
            {               
                AutoMapper.Mapper.CreateMap<TWNewEgg.API.Models.ItemSketch, Product_DetialEdit>()
                    // 條碼
                    .ForMember(x => x.BarCode, x => x.MapFrom(src => src.Product.BarCode))
                    // 配送方式
                    .ForMember(x => x.DelvType, x => x.Ignore())
                    // 商品中文說明
                    .ForMember(x => x.Description, x => x.MapFrom(src => src.Product.Description))
                    // 商品中文說明
                    .ForMember(x => x.DescriptionTW, x => x.MapFrom(src => src.Product.Description))
                    // 材積(公分)_高
                    .ForMember(x => x.Height, x => x.MapFrom(src => src.Product.Height))
                    // 是否為18歲商品
                    .ForMember(x => x.Is18, x => x.MapFrom(src => src.Product.Is18))
                    // 是否有窒息危險
                    .ForMember(x => x.IsChokingDanger, x => x.MapFrom(src => src.Product.IsChokingDanger))
                    // 是否有遞送危險
                    .ForMember(x => x.IsShipDanger, x => x.MapFrom(src => src.Product.IsShipDanger))
                    // 材積(公分)_長
                    .ForMember(x => x.Length, x => x.MapFrom(src => src.Product.Length))
                    // 製造商 ID
                    .ForMember(x => x.ManufactureID, x => x.MapFrom(src => src.Product.ManufactureID))
                    // 製造商商品編號
                    .ForMember(x => x.MenufacturePartNum, x => x.MapFrom(src => src.Product.MenufacturePartNum))
                    // 型號
                    .ForMember(x => x.Model, x => x.MapFrom(src => src.Product.Model))
                    // 注意事項
                    .ForMember(x => x.Note, x => x.MapFrom(src => src.Item.Note))
                    // 產品圖片最後一張
                    .ForMember(x => x.PicEnd, x => x.MapFrom(src => src.Product.PicEnd))
                    // 產品圖片第一張
                    .ForMember(x => x.PicStart, x => x.MapFrom(src => src.Product.PicStart))
                    // 商家商品編號
                    .ForMember(x => x.SellerProductID, x => x.MapFrom(src => src.Product.SellerProductID))
                    // UPC 編號
                    .ForMember(x => x.UPC, x => x.MapFrom(src => src.Product.UPC))
                    // 更新日期
                    .ForMember(x => x.UpdateDate, x => x.MapFrom(src => updateDate))
                    // 更新者 ID
                    .ForMember(x => x.UpdateUser, x => x.MapFrom(src => src.CreateAndUpdate.UpdateUser))
                    // 商品保固期(月)
                    .ForMember(x => x.Warranty, x => x.MapFrom(src => src.Product.Warranty))
                    // 重量(公斤)
                    .ForMember(x => x.Weight, x => x.MapFrom(src => src.Product.Weight))
                    // 材積(公分)_寬
                    .ForMember(x => x.Width, x => x.MapFrom(src => src.Product.Width));

                AutoMapper.Mapper.Map(itemSketch, result.Body);

                result.Body.DelvType = delvType;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                log.Info(string.Format("組合 Product 詳細表 Model 失敗 (expection); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 組合正式 Item (詳細表)
        /// </summary>
        /// <param name="itemSketch">要更新的商品資訊</param>
        /// <param name="delvType">配送方式</param>
        /// <param name="updateDate">更新日期</param>
        /// <returns>正式 Item (詳細表)</returns>
        private ActionResponse<Item_DetialEdit> MakeItem_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, int delvType, DateTime updateDate)
        {
            ActionResponse<Item_DetialEdit> result = new ActionResponse<Item_DetialEdit>();
            result.Body = new Item_DetialEdit();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            #region 計算毛利率
            // 搬移到審核狀態變更時進行計算 2015.07.25 by Jack
            //decimal? grossMargin = null;

            //if (itemSketch.Product.Cost == null || itemSketch.Item.PriceCash == null || itemSketch.Item.PriceCash.Value == 0)
            //{
            //    grossMargin = 0;
            //}
            //else
            //{
            //    grossMargin = ((itemSketch.Item.PriceCash.Value - itemSketch.Product.Cost.Value) / itemSketch.Item.PriceCash.Value) * 100m;
            //    grossMargin = Decimal.Round(grossMargin.Value, 2);
            //}

            #endregion 計算毛利率

            // 若原本 Qty 不等於 0 需要額外處理
            int itemQty = this.CountLimitCansaleQty(itemSketch);
                   
            try
            {
                AutoMapper.Mapper.CreateMap<TWNewEgg.API.Models.ItemSketch, Item_DetialEdit>()
                    // 商品主分類
                    .ForMember(x => x.CategoryID, x => x.MapFrom(src => src.ItemCategory.MainCategoryID_Layer2))
                    // 賣場開始日期
                    .ForMember(x => x.DateStart, x => x.MapFrom(src => src.Item.DateStart.AddHours(8).Date))
                    // 賣場結束日期 (將賣場開始日期的年份設為 2099 年)
                    .ForMember(x => x.DateEnd, x => x.MapFrom(src => src.Item.DateStart.AddHours(8).Date.AddYears(2099 - src.Item.DateStart.AddHours(8).Date.Year)))
                    // 賣場刪除日期 (賣場結束日期 +1 天)
                    .ForMember(x => x.DateDel, x => x.MapFrom(src => src.Item.DateStart.AddHours(8).Date.AddYears(2099 - src.Item.DateStart.AddHours(8).Date.Year).AddDays(1)))
                    // 到貨天數
                    .ForMember(x => x.DelvDate, x => x.MapFrom(src => string.IsNullOrWhiteSpace(src.Item.DelvDate) ? "1-7" : itemSketch.Item.DelvDate))
                    // 配送方式
                    .ForMember(x => x.DelvType, x => x.Ignore())
                    // 中文描述
                    .ForMember(x => x.DescTW, x => x.MapFrom(src => src.Product.Description))
                    // 中文描述
                    .ForMember(x => x.ItemDesc, x => x.MapFrom(src => src.Product.Description))
                    // 毛利率
                    .ForMember(x => x.GrossMargin, x => x.Ignore())
                    // 商品成色
                    .ForMember(x => x.IsNew, x => x.MapFrom(src => src.Item.IsNew))
                    // 商品包裝
                    .ForMember(x => x.ItemPackage, x => x.MapFrom(src => src.Item.ItemPackage))
                    // 製造商 ID
                    .ForMember(x => x.ManufactureID, x => x.MapFrom(src => src.Product.ManufactureID))
                    // 市場建議售價
                    .ForMember(x => x.MarketPrice, x => x.MapFrom(src => src.Item.MarketPrice ?? 0))
                    // 商品型號
                    .ForMember(x => x.Model, x => x.MapFrom(src => src.Product.Model ?? string.Empty))
                    // 注意事項
                    .ForMember(x => x.Note, x => x.MapFrom(src => src.Item.Note ?? string.Empty))
                    // 圖片結束
                    .ForMember(x => x.PicEnd, x => x.MapFrom(src => src.Product.PicEnd))
                    // 圖片開始
                    .ForMember(x => x.PicStart, x => x.MapFrom(src => src.Product.PicStart))
                    // 限量數量
                    .ForMember(x => x.Qty, x => x.MapFrom(src => itemQty))
                    // 限購數量
                    .ForMember(x => x.QtyLimit, x => x.MapFrom(src => src.Item.QtyLimit ?? 0))
                    // 商品特色標題
                    .ForMember(x => x.Sdesc, x => x.MapFrom(src => src.Item.Sdesc))
                    // 運送類型
                    .ForMember(x => x.ShipType, x => x.MapFrom(src => src.Item.ShipType))
                    // 商品簡要描述
                    .ForMember(x => x.Spechead, x => x.MapFrom(src => src.Item.Spechead))
                    // 更新日期
                    .ForMember(x => x.UpdateDate, x => x.MapFrom(src => updateDate))
                    .ForMember(x => x.Discard4, x => x.MapFrom(src => src.Item.Discard4))
                    // 更新者 ID
                    .ForMember(x => x.UpdateUser, x => x.MapFrom(src => src.CreateAndUpdate.UpdateUser));
          
                AutoMapper.Mapper.Map(itemSketch, result.Body);

                // AutoMapper 會無法work
                result.Body.DelvType = delvType;

                result.Body.Qty = 0;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                log.Info(string.Format("組合 Item 詳細表 Model 失敗 (expection); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        // 計算賣場限量數量
        private int CountLimitCansaleQty(Models.ItemSketch itemSketch)
        {
            if (itemSketch.Item.ItemQty == 0 && itemSketch.Item.CanSaleLimitQty == 0)
            {
                return 0;
            }
            else
            {
                return itemSketch.Item.CanSaleLimitQty.Value + itemSketch.Item.ItemQtyReg.Value;
            }
        }

        /// <summary>
        /// 組合正式 ItemStock (詳細表)
        /// </summary>
        /// <param name="itemSketch">要更新的商品資訊</param>
        /// <param name="updateDate">更新日期</param>
        /// <returns>正式 ItemStock (詳細表)</returns>
        private ActionResponse<ItemStock_DetialEdit> MakeItemStock_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, DateTime updateDate)
        {
            ActionResponse<ItemStock_DetialEdit> result = new ActionResponse<ItemStock_DetialEdit>();
            result.Body = new ItemStock_DetialEdit();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            try
            {
                AutoMapper.Mapper.CreateMap<TWNewEgg.API.Models.ItemSketch, ItemStock_DetialEdit>()
                    // 可售數量
                    .ForMember(x => x.Qty, x => x.MapFrom(src => src.ItemStock.CanSaleQty + src.ItemStock.InventoryQtyReg))
                    // 安全庫存
                    .ForMember(x => x.SafeQty, x => x.MapFrom(src => src.ItemStock.InventorySafeQty))
                    // 更新日期
                    .ForMember(x => x.UpdateDate, x => x.MapFrom(src => updateDate))
                    // 更新者 ID
                    .ForMember(x => x.UpdateUser, x => x.MapFrom(src => src.CreateAndUpdate.UpdateUser));

                AutoMapper.Mapper.Map(itemSketch, result.Body);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                log.Info(string.Format("組合 ItemStock 詳細表 Model 失敗 (expection); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 組合正式 ItemCategory
        /// </summary>
        /// <param name="itemID">賣場 ID</param>
        /// <param name="categoryID">跨分類 ID</param>
        /// <param name="createUser">建立者 ID</param>
        /// <param name="createDate">建立日期</param>
        /// <returns>正式 ItemCategory</returns>
        private ActionResponse<DB.TWSQLDB.Models.ItemCategory> MakeItemCategory_DB(int itemID, int categoryID, string createUser, DateTime createDate)
        {
            ActionResponse<DB.TWSQLDB.Models.ItemCategory> result = new ActionResponse<DB.TWSQLDB.Models.ItemCategory>();
            result.Body = new DB.TWSQLDB.Models.ItemCategory();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            try
            {
                result.Body.ItemID = itemID;
                result.Body.CategoryID = categoryID;
                result.Body.FromSystem = "1";

                result.Body.CreateDate = createDate;
                result.Body.CreateUser = createUser;
                result.Body.UpdateDate = createDate;
                result.Body.UpdateUser = createUser;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                log.Info(string.Format("組合 ItemCategory DB Model 失敗(expection); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 組合 ItemCategoryTemp
        /// </summary>
        /// <param name="itemTempID">賣場 TempID</param>
        /// <param name="categoryID">跨分類 ID</param>
        /// <param name="createUser">建立者 ID</param>
        /// <param name="createDate">建立日期</param>
        /// <returns>ItemCategoryTemp</returns>
        private ActionResponse<DB.TWSQLDB.Models.ItemCategorytemp> MakeItemCategoryTemp_DB(int itemTempID, int? itemID, int categoryID, string createUser, DateTime createDate)
        {
            ActionResponse<DB.TWSQLDB.Models.ItemCategorytemp> result = new ActionResponse<DB.TWSQLDB.Models.ItemCategorytemp>();
            result.Body = new DB.TWSQLDB.Models.ItemCategorytemp();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            try
            {
                result.Body.itemtempID = itemTempID;
                result.Body.CategoryID = categoryID;
                result.Body.FromSystem = "1";
                result.Body.ItemID = itemID.GetValueOrDefault();

                result.Body.CreateDate = createDate;
                result.Body.CreateUser = createUser;
                result.Body.UpdateDate = createDate;
                result.Body.UpdateUser = createUser;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                log.Info(string.Format("組合 ItemCategoryTemp 失敗(expection); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        #endregion 組合 Model

        #endregion 詳細表編輯

        #region 待審清單編輯

        public ActionResponse<List<string>> TempListEdit(List<TWNewEgg.API.Models.ItemSketch> listEdit)
        {
            ActionResponse<List<string>> result = new ActionResponse<List<string>>();

            // 檢查是否變更審核狀態
            foreach (TWNewEgg.API.Models.ItemSketch itemSketch in listEdit)
            {
                UpdateTempStatus_DetailEdit(itemSketch, DateTime.Now);
            }

            // trans to listEdit Model
            List<TempList> editModel = trantoListModel(listEdit);
            using (TransactionScope scope = new TransactionScope())
            {
                foreach (var index in editModel)
                {
                    // 若無正式賣場 ID and ProductID 則不修改
                    if (index.ItemID != 0 && index.ProductID != 0)
                    {
                        updateItemList(index);
                        updateItemStockList(index);
                        updateProductList(index);
                    }
                    
                    updateItemTempList(index);
                    updateProductTempList(index);
                    updateItemStockTempList(index);
                }

                try
                {
                    db.SaveChanges();
                    scope.Complete();
                    result.IsSuccess = true;
                    result.Msg = "資料儲存成功";
                    result.Code = (int)ResponseCode.Success;
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Msg = "待審清單編輯儲存失敗。";
                    //log.Info(string.Format("待審清單編輯儲存失敗(expection)：(ProductID = {0}) {1}。", index.ProductID, "[ErrorMessage] " + ex.Message + " [ErrorStackTrace] " + ex.StackTrace));
                    scope.Dispose();
                }

                scope.Dispose();
            }

            return result;
        }

        public List<TempList> trantoListModel(List<Models.ItemSketch> listEdit)
        {
            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();

            List<TempList> result = new List<TempList>();

            if (listEdit != null && listEdit.First().Item.SellerID.HasValue)
            {
                result = AutoMapper.Mapper.Map<List<TempList>>(listEdit);

                int getSellerID = listEdit.First().Item.SellerID ?? 0;

                string getAccountTypeCode = spdb.Seller_BasicInfo.Where(x => x.SellerID == getSellerID).Select(r => r.AccountTypeCode).FirstOrDefault();

                foreach (var index in result)
                {
                    switch (getAccountTypeCode)
                    {
                        case "S":
                            // 由運送方決定DelvType
                            switch (index.ShipType)
                            {
                                case "S":
                                    index.DelvType = 2;
                                    break;
                                case "N":
                                    index.DelvType = 8;
                                    break;
                            }
                            break;
                        case "V":
                            {
                                // 由運送方決定DelvType
                                switch (index.ShipType)
                                {
                                    case "S":
                                        index.DelvType = 7;
                                        index.ShipType = "V";
                                        break;
                                    case "N":
                                        index.DelvType = 9;
                                        break;
                                }
                            }

                            break;
                    }
                }
            }

            return result;
        }


        #region 更新正式表

        private void updateProductList(TempList edit)
        {
            DB.TWSQLDB.Models.Product updateProduct = db.Product.Where(x => x.ID == edit.ProductID).FirstOrDefault();
            //updateProduct.Cost = edit.Cost;
            updateProduct.DelvType = edit.DelvType;
            updateProduct.UpdateDate = DateTime.Now;
            updateProduct.UpdateUser = edit.UpdateUserID.ToString();
        }

        private void updateItemList(TempList edit)
        {
            DB.TWSQLDB.Models.Item updateItem = db.Item.Where(x => x.ID == edit.ItemID).FirstOrDefault();

            updateItem.ShipType = edit.ShipType;
            updateItem.Qty = 0;
            updateItem.DateStart = edit.DateSatrt;
            updateItem.MarketPrice = edit.MarketPrice;
            updateItem.UpdateDate = DateTime.Now;
            updateItem.UpdateUser = edit.UpdateUserID.ToString();
            updateItem.DelvType = edit.DelvType;
            //售價、Name 不更新
            //updateItem.PriceCash = edit.PriceCash;
            //updateItem.PriceCard = edit.PriceCash;
        }

        private void updateItemStockList(TempList edit)
        {
            DB.TWSQLDB.Models.ItemStock updateItemStock = db.ItemStock.Where(x => x.ProductID == edit.ProductID).FirstOrDefault();

            updateItemStock.SafeQty = edit.SafeQty;
            updateItemStock.Qty = edit.Qty;
            updateItemStock.UpdateDate = DateTime.Now;
            updateItemStock.UpdateUser = edit.UpdateUserID.ToString();
        }

        #endregion 更新正式表

        #region 更新 temp 表

        private void updateProductTempList(TempList edit)
        {
            DB.TWSQLDB.Models.ProductTemp updateProductTemp = db.ProductTemp.Where(x => x.ID == edit.ProductTempID).FirstOrDefault();
            updateProductTemp.Cost = edit.Cost;
            updateProductTemp.DelvType = edit.DelvType;
            updateProductTemp.UpdateDate = DateTime.Now;
            updateProductTemp.UpdateUser = edit.UpdateUserID.ToString();
        }

        private void updateItemTempList(TempList edit)
        {
            DB.TWSQLDB.Models.ItemTemp updateItemTemp = db.ItemTemp.Where(x => x.ID == edit.ItemTempID).FirstOrDefault();

            updateItemTemp.ShipType = edit.ShipType;
            updateItemTemp.Qty = 0;
            updateItemTemp.DateStart = edit.DateSatrt;
            updateItemTemp.MarketPrice = edit.MarketPrice;
            updateItemTemp.UpdateDate = DateTime.Now;
            updateItemTemp.UpdateUser = edit.UpdateUserID.ToString();
            updateItemTemp.DelvType = edit.DelvType;
            updateItemTemp.PriceCash = edit.PriceCash;
            updateItemTemp.PriceCard = edit.PriceCash;

            #region 計算毛利率
            // 搬移到審核狀態變更時一起計算 2015.07.25 by jack
            //if (edit.PriceCash == 0)
            //{
            //    updateItemTemp.GrossMargin = 0;
            //}
            //else
            //{
            //    updateItemTemp.GrossMargin = ((edit.PriceCash - edit.Cost) / edit.PriceCash) * 100m;
            //    updateItemTemp.GrossMargin = Decimal.Round(updateItemTemp.GrossMargin.Value, 2);
            //}

            #endregion 計算毛利率
        }

        private void updateItemStockTempList(TempList edit)
        {
            DB.TWSQLDB.Models.ItemStocktemp updateItemStocktemp = db.ItemStocktemp.Where(x => x.producttempID == edit.ProductTempID).FirstOrDefault();
            updateItemStocktemp.SafeQty = edit.SafeQty;
            updateItemStocktemp.Qty = edit.Qty;
            updateItemStocktemp.UpdateDate = DateTime.Now;
            updateItemStocktemp.UpdateUser = edit.UpdateUserID.ToString();
        }

        #endregion 更新 temp 表

        #endregion 待審清單編輯
        #region 批次待審編輯
        public ActionResponse<List<string>> BatchTempDetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch)
        {
            ActionResponse<List<string>> result = new ActionResponse<List<string>>();
            result.Body = new List<string>();
            result.IsSuccess = true;
            result.Msg = string.Empty;
            try
            {
                var itemtempModel = db.ItemTemp.Where(x => x.ID == itemSketch.Item.ID).FirstOrDefault();

                // 根據審核狀態有不同方式修改           
                switch (itemtempModel.Status)
                {
                    default:
                    case 0:
                    case 1:
                        result = this.BatchUpdateTempAndOffical(itemSketch);
                        break;
                    case 2:
                        // 舊品未通過 商品修改
                        if (itemSketch.Item.ItemID.HasValue && itemSketch.Product.ProductID.HasValue
                            && itemSketch.Item.ID != 0 && itemSketch.Product.ID != 0)
                        {
                            result = this.BatchUpdateTempAndOffical(itemSketch);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                log.Error("Exception: " + ex.Message + ", StackTrace: " + ex.StackTrace);
                result.IsSuccess = false;
                result.Msg = "發生意外錯誤，請稍後再試";
                result.Body = null;
            }


            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }
        public ActionResponse<List<string>> BatchTempDetailEdit(List<TWNewEgg.API.Models.ItemSketch> itemSketch, int UserID, int CurrentSellerID)
        {
            //加價購
            AdditionalPurchaseService AdditionalPurchaseService = new AdditionalPurchaseService();
            SellerUserService SellerUserService = new Service.SellerUserService();
            ItemSketchSearchCondition condition = new ItemSketchSearchCondition();
            ActionResponse<List<AdditionalPurchase>> AdditionalPurchaseList = new ActionResponse<List<AdditionalPurchase>>();

            //NullResult
            ActionResponse<List<string>> nullresult = new ActionResponse<List<string>>();
            nullresult.Body = new List<string>();
            nullresult.IsSuccess = true;
            nullresult.Msg = string.Empty;

            //Editresult
            ActionResponse<List<string>> Editresult = new ActionResponse<List<string>>();
            Editresult.Body = new List<string>();
            Editresult.IsSuccess = true;
            Editresult.Msg = string.Empty;

            //Message
            ActionResponse<List<string>> massage = new ActionResponse<List<string>>();
            massage.Body = new List<string>();
            massage.IsSuccess = true;
            massage.Msg = string.Empty;

            //DB
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            DB.TWSqlDBContext itemdb = new DB.TWSqlDBContext();
            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();

            //Log
            ILog log = LogManager.GetLogger(typeof(BatchItemUpdate));
            //日期
            DateTime dt = DateTime.UtcNow.AddHours(8);
            dt.GetDateTimeFormats('r')[0].ToString();
            Convert.ToDateTime(dt);
            //確認
            int checkCnt = 0;
            //修改
            int EditFailCount = 0;
            if (UserID <= 0 || CurrentSellerID <= 0)
            {
                nullresult.Msg = "錯誤的登入者身分";
                nullresult.Code = (int)ResponseCode.Error;
                nullresult.IsSuccess = false;
            }

            //查詢使用者身分
            var usertyperesult = SellerUserService.GetVenderOrSeller(UserID.ToString(), 0);
            //查詢vender or seller
            if (usertyperesult.IsSuccess == false)
            {
                nullresult.Msg = "查詢登入者身分為vender 或者 seller 時 發生錯誤 : " + usertyperesult.Msg;
                nullresult.Code = (int)ResponseCode.Error;
                nullresult.IsSuccess = false;
            }

            if (nullresult.IsSuccess == true)
            {
                foreach (API.Models.ItemSketch BatchEditItem in itemSketch)
                {
                    checkCnt++;
                    if (checkCnt >= 1)
                    {
                        //判斷model裡itemID是否為空值
                        if (string.IsNullOrWhiteSpace(BatchEditItem.Item.ItemID.ToString()))
                        {
                            //因為list.add是call by reference，所以需要重新new
                            ActionResponse<List<string>> errorInfo = new ActionResponse<List<string>>();
                            nullresult.Msg = "新蛋賣場編號：不可為空白！!!";
                            nullresult.Code = (int)ResponseCode.Error;
                            nullresult.IsSuccess = false;
                            break;
                        }
                        //判斷是否有此ItemID 的資料
                        else
                        {
                            //沒有ItmeID資料回傳
                            int intItemID = Convert.ToInt32(BatchEditItem.Item.ItemID.ToString());
                            DB.TWSQLDB.Models.Item ItemInfo = new DB.TWSQLDB.Models.Item();
                            ItemInfo = itemdb.Item.Where(x => x.ID == intItemID).SingleOrDefault();
                            if (ItemInfo == null)
                            {
                                ActionResponse<List<string>> errorInfo = new ActionResponse<List<string>>();
                                nullresult.Msg = "新蛋賣場編號：" + BatchEditItem.Item.ItemID.ToString() + "，沒有此筆資料!!!";
                                nullresult.Code = (int)ResponseCode.Error;
                                nullresult.IsSuccess = false;
                                break;
                            }
                            //Table存在，進行sellerID判斷與規格品判斷
                            else
                            {
                                if (CurrentSellerID != ItemInfo.SellerID)
                                {
                                    ActionResponse<List<string>> errorInfo = new ActionResponse<List<string>>();
                                    nullresult.Msg = "新蛋賣場編號：" + BatchEditItem.Item.ItemID.ToString() + " 請確認編號是否填寫正確!!!";
                                    nullresult.Code = (int)ResponseCode.Error;
                                    nullresult.IsSuccess = false;
                                    break;
                                }
                                else if (db.ItemGroupDetailProperty.Where(x => x.ItemID == intItemID).Count() != 0)
                                {
                                    ActionResponse<List<string>> errorInfo = new ActionResponse<List<string>>();
                                    nullresult.Msg = "新蛋賣場編號：" + BatchEditItem.Item.ItemID.ToString() + "為規格品，無法批次修改!!!";
                                    nullresult.Code = (int)ResponseCode.Error;
                                    nullresult.IsSuccess = false;
                                    break;
                                }
                                else if (usertyperesult.Body.Equals("V"))
                                {
                                    condition.KeyWord = BatchEditItem.Item.ItemID.ToString();
                                    condition.SellerID = ItemInfo.SellerID;
                                    condition.KeyWordScarchTarget = ItemSketchKeyWordSearchTarget.ItemID;

                                    AdditionalPurchaseList = AdditionalPurchaseService.AdditionalPurchaseItemSearchResult(condition, true);
                                    if (AdditionalPurchaseList.IsSuccess)
                                    {
                                        if (AdditionalPurchaseList.Body.Count != 0)
                                        {
                                            if (AdditionalPurchaseList.Body.FirstOrDefault().ShowOrder == (int)TWNewEgg.DB.TWSQLDB.Models.Item.ShowOrderStatus.AddtionalItemForCart)
                                            {
                                                logger.Error("itemid is " + BatchEditItem.Item.ItemID + ", showorder is " + AdditionalPurchaseList.Body.FirstOrDefault().ShowOrder + "為加價購商品");
                                                nullresult.Msg = "新蛋賣場編號 :" + condition.KeyWord + "為加價購，Vender不可修改";
                                                nullresult.Code = (int)ResponseCode.Error;
                                                nullresult.IsSuccess = false;
                                                break;
                                            }
                                            //nullresult.Msg = "新蛋賣場編號 :" + condition.KeyWord + "為加價購，Vender不可修改";
                                            //nullresult.Code = (int)ResponseCode.Error;
                                            //nullresult.IsSuccess = false;
                                            //break;
                                        }
                                    }
                                    else
                                    {
                                        nullresult.Msg = "查詢" + condition.KeyWord + "是否為加價購發生例外 : " + AdditionalPurchaseList.Msg;
                                        nullresult.Code = (int)ResponseCode.Error;
                                        nullresult.IsSuccess = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                }
            }
            //若沒有錯誤
            if (nullresult.IsSuccess == true)
            {
                int saveCnt = 0;
                int c = 0;
                try
                {
                    // 使用 TransactionScope 保持資料交易完整性
                    //using (TransactionScope scope = new TransactionScope())
                    //{
                        List<int> errornum = new List<int>();
                       
                        foreach (API.Models.ItemSketch BatchEditItem in itemSketch)
                        {
                            // 更新時間
                            DateTime updateTime = DateTime.Now;
                            int intItemID = Convert.ToInt32(BatchEditItem.Item.ItemID.ToString());
                            saveCnt++;
                            if (saveCnt > 0)
                            {
                                //取出當次table
                                DB.TWSQLDB.Models.Item ItemInfo = new DB.TWSQLDB.Models.Item();
                                DB.TWSQLDB.Models.Product ProInfo = new DB.TWSQLDB.Models.Product();
                                DB.TWSQLDB.Models.ItemStock StockInfo = new DB.TWSQLDB.Models.ItemStock();
                                DB.TWSQLDB.Models.ProductTemp ProductTempInfo = new DB.TWSQLDB.Models.ProductTemp();
                                DB.TWSQLDB.Models.ItemTemp ItemTempInfo = new DB.TWSQLDB.Models.ItemTemp();
                                DB.TWSQLDB.Models.ItemStocktemp StockTempInfo = new DB.TWSQLDB.Models.ItemStocktemp();
                                List<DB.TWSQLDB.Models.ItemCategory> ItemCategoryInfo = new List<DB.TWSQLDB.Models.ItemCategory>(); 
                                //ItemTable
                                ItemInfo = itemdb.Item.Where(x => x.ID == intItemID).SingleOrDefault();
                                ItemTempInfo = itemdb.ItemTemp.Where(x => x.ItemID == intItemID).SingleOrDefault();
                                int intProductID = db.Item.Where(x => x.ID == intItemID).Select(x => x.ProductID).FirstOrDefault();
                                //ProductTable
                                ProInfo = itemdb.Product.Where(x => x.ID == intProductID).SingleOrDefault();
                                //ItemStockTable
                                StockInfo = itemdb.ItemStock.Where(x => x.ProductID == intProductID).SingleOrDefault();
                                StockTempInfo = itemdb.ItemStocktemp.Where(x => x.ProductID == intProductID).SingleOrDefault();
                                //ProducttempTable
                                ProductTempInfo = itemdb.ProductTemp.Where(x => x.ProductID == intProductID).SingleOrDefault();
                                //ItemCategoryTable
                                ItemCategoryInfo = itemdb.ItemCategory.Where(x => x.ItemID == intItemID).ToList();
                                //將model填寫完整
                                if (ItemInfo != null && ItemTempInfo != null)
                                {
                                    //Excel未提供     
                                    //ID = ItmetempID
                                    BatchEditItem.Item.ID = string.IsNullOrWhiteSpace(ItemInfo.ItemtempID.ToString()) ? BatchEditItem.Item.ID : int.Parse(ItemInfo.ItemtempID.ToString());
                                    BatchEditItem.Item.DelvDate = ItemInfo.DelvDate;
                                    BatchEditItem.Item.SellerID = ItemInfo.SellerID;
                                    BatchEditItem.Item.ShipType = ItemTempInfo.ShipType;
                                    BatchEditItem.Item.status = ItemTempInfo.Status;

                                    BatchEditItem.Item.MarketPrice = BatchEditItem.Item.MarketPrice == null ? ItemTempInfo.MarketPrice : BatchEditItem.Item.MarketPrice;

                                    BatchEditItem.Item.Sdesc = string.IsNullOrEmpty(BatchEditItem.Item.Sdesc) ? ItemTempInfo.Sdesc : BatchEditItem.Item.Sdesc;

                                    BatchEditItem.Item.Spechead = string.IsNullOrEmpty(BatchEditItem.Item.Spechead) ? ItemTempInfo.Spechead : BatchEditItem.Item.Spechead;

                                    BatchEditItem.Item.PriceCash = BatchEditItem.Item.PriceCash == null ? ItemTempInfo.PriceCash : BatchEditItem.Item.PriceCash;                                  
                                }

                                if (StockTempInfo != null)
                                {
                                    BatchEditItem.ItemStock.InventorySafeQty = BatchEditItem.ItemStock.InventorySafeQty == null ? StockTempInfo.SafeQty : BatchEditItem.ItemStock.InventorySafeQty;

                                    BatchEditItem.ItemStock.InventoryQty = BatchEditItem.ItemStock.InventoryQty == null ? StockTempInfo.Qty - StockTempInfo.QtyReg : BatchEditItem.ItemStock.InventoryQty;

                                    //Excel未提供
                                    BatchEditItem.ItemStock.CanSaleQty = BatchEditItem.ItemStock.InventoryQty == null ? StockTempInfo.Qty - StockTempInfo.QtyReg : BatchEditItem.ItemStock.InventoryQty;
                                    BatchEditItem.ItemStock.InventoryQtyReg = StockTempInfo.QtyReg;
                                }
                                if (ProInfo != null && ProductTempInfo != null)
                                {
                                    //Excel未提供
                                    //ProdutId = > Product.ID
                                    BatchEditItem.Product.ProductID = ProInfo.ID;


                                    BatchEditItem.Product.Name = string.IsNullOrEmpty(BatchEditItem.Product.Name) ? ItemTempInfo.Name : BatchEditItem.Product.Name;

                                    BatchEditItem.Product.Description = string.IsNullOrEmpty(BatchEditItem.Product.Description) ? ItemTempInfo.DescTW : BatchEditItem.Product.Description;

                                    BatchEditItem.Product.Cost = BatchEditItem.Product.Cost == null ? ProductTempInfo.Cost : BatchEditItem.Product.Cost;

                                    BatchEditItem.Product.Warranty = BatchEditItem.Product.Warranty == null ? ProductTempInfo.Warranty : BatchEditItem.Product.Warranty;

                                    BatchEditItem.Product.SellerProductID = BatchEditItem.Product.SellerProductID == null ? ProductTempInfo.SellerProductID : BatchEditItem.Product.SellerProductID;
                                }
                                if (ProductTempInfo != null)
                                {
                                    BatchEditItem.Product.ID = ProductTempInfo.ID;
                                }
                                if (UserID.ToString() != null)
                                {
                                    BatchEditItem.CreateAndUpdate.UpdateUser = UserID;
                                }
                                if (updateTime.ToString() != null)
                                {
                                    BatchEditItem.CreateAndUpdate.UpdateDate = updateTime;
                                }

                            }
                            //成本去除小數點
                            if (BatchEditItem.Product.Cost != null)
                            {
                                BatchEditItem.Product.Cost = Math.Floor(BatchEditItem.Product.Cost.Value);
                            }
                            Editresult = BatchTempDetailEdit(BatchEditItem);
                            if (Editresult.IsSuccess == false)
                            {
                                EditFailCount++;
                                Editresult.IsSuccess = false;
                                log.Info(Editresult.Msg);
                                errornum.Add(saveCnt);
                                Editresult.IsSuccess = true;
                            }

                        }
                        // 成功修改 TransactionScope 結束
                        if (errornum.Count == 0)
                        {
                            //scope.Complete();
                            massage.Msg = "修改成功。";
                        }
                        //修改失敗
                        else
                        {
                            //scope.Dispose();
                            string errermess = "";

                            foreach (int err in errornum)
                            {
                                if (c == 0)
                                {
                                    errermess = errermess + err.ToString();
                                }
                                else
                                {
                                    errermess = errermess + "、" + err.ToString();
                                }
                                c++;
                            }
                            massage.Msg = "第" + errermess + "筆" + "修改失敗。";
                        }
                    //}
                    log.Info(string.Format("共 {0} 筆資料，已成功修改 {1}  筆資料，失敗 {2} 筆資料。", itemSketch.Count, itemSketch.Count - EditFailCount, EditFailCount));
                    // massage.Msg = (string.Format("共 {0} 筆資料，已成功修改 {1}  筆資料，失敗 {2} 筆資料。若有失敗請確認後重新上傳", itemSketch.Count, itemSketch.Count - EditFailCount, EditFailCount));
                    return massage;
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    massage.IsSuccess = false;
                    massage.Code = (int)ResponseCode.Error;
                    massage.Msg = "第" + saveCnt + "筆開始，例外發生，修改失敗。";
                    log.Info("批次修改 例外發生 : " + ex.Message + "StackTrace :" + ex.StackTrace);
                    massage.Body = null;
                    return massage;
                }
            }
            else
            {
                massage.Msg = nullresult.Msg.ToString();
                massage.Code = (int)ResponseCode.Error;
                massage.IsSuccess = false;
                massage.Body.Add(nullresult.ToString());
            }
            return massage;

        }
        private ActionResponse<List<string>> BatchUpdateTempAndOffical(Models.ItemSketch itemSketch)
        {
            ActionResponse<List<string>> result = new ActionResponse<List<string>>();
            result.Body = new List<string>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            // 取得 Account Type
            ActionResponse<string> getAccountType = GetAccountType(itemSketch.Item.SellerID);
            ActionResponse<int> getDelvType = new ActionResponse<int>();

            // 取得 Account Type 成功，才取得 DelvType
            if (getAccountType.IsSuccess)
            {

                // 取得 DelvType
                getDelvType = GetDelvType(getAccountType.Body, itemSketch.Item.ShipType);

                // 取得 DelvType 成功，才開始更新資料
                if (getDelvType.IsSuccess)
                {
                    // 更新時間
                    DateTime updateTime = DateTime.Now;

                    using (TransactionScope scope = new TransactionScope())
                    {
                        // 更新正式表
                        ActionResponse<bool> BatchupdateOfficial_DetailEdit = BatchUpdateOfficial_DetailEdit(itemSketch, getDelvType.Body, updateTime);

                        if (BatchupdateOfficial_DetailEdit.IsSuccess == false)
                        {
                            result.IsSuccess = false;
                            log.Info("批次更新正式表失敗。");
                        }

                        if (result.IsSuccess)
                        {
                            // 更新 Temp 表
                            ActionResponse<bool> BatchupdateTemp_DetailEdit = BatchUpdateTemp_DetailEdit(itemSketch, getDelvType.Body, updateTime);

                            if (BatchupdateTemp_DetailEdit.IsSuccess == false)
                            {
                                scope.Dispose();
                                result.IsSuccess = false;
                                log.Info("批次更新 Temp 表失敗。");
                            }
                            else
                            {
                                scope.Complete();
                                result.IsSuccess = true;
                                result.Msg = "批次修改成功。";
                                log.Info("批次更新詳細表編輯表成功。");
                            }
                        }

                        else
                        {
                            scope.Dispose();
                            result.Msg = "批次修改失敗。";
                            log.Info("批次更新詳細表編輯表失敗。");
                        }
                    }
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = getAccountType.Msg;
            }

            return result;
        }
        private ActionResponse<bool> BatchUpdateOfficial_DetailEdit(Models.ItemSketch itemSketch, int delvType, DateTime updateDate)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            // 更新商品
            ActionResponse<bool> updateProduct = BatchUpdateProduct_DetailEdit(itemSketch, delvType, updateDate);

            if (updateProduct.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg += updateProduct.Msg;
            }

            if (result.IsSuccess)
            {
                // 更新賣場
                ActionResponse<bool> updateItem = BatchUpdateItem_DetailEdit(itemSketch, delvType, updateDate);

                if (updateItem.IsSuccess == false)
                {
                    result.IsSuccess = false;
                    result.Msg += updateItem.Msg;
                }

                if (result.IsSuccess)
                {
                    // 更新庫存
                    ActionResponse<bool> updateItemStock = BatchUpdateItemStock_DetailEdit(itemSketch, updateDate);

                    if (updateItemStock.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg += updateItemStock.Msg;
                    }

                }
                if (result.IsSuccess)
                {
                    // 更新跨分類
                    ActionResponse<bool> updateItemCategory = UpdateItemCategory_DetailEdit(itemSketch, updateDate);

                    if (updateItemCategory.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg += updateItemCategory.Msg;
                    }
                }
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }
        /// <summary>
        /// 批次修改 更新商品
        /// </summary>
        /// <param name="itemSketch">更新資訊</param>
        /// <param name="updateDate">更新日期</param>
        /// <returns>成功失敗訊息</returns>
        private ActionResponse<bool> BatchUpdateProduct_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, int delvType, DateTime updateDate)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            if (itemSketch.Product.ProductID != null && itemSketch.Product.ProductID.Value > 0)
            {
                // 讀取資料庫內要被更新的賣場資訊
                ActionResponse<DB.TWSQLDB.Models.Product> getProduct = GetProduct(itemSketch.Product.ProductID.Value);

                if (getProduct.IsSuccess)
                {
                    // 組合要更新的資料
                    ActionResponse<BatchProduct_DetialEdit> makeProduct_DetailEdit = BatchMakeProduct_DetailEdit(itemSketch, delvType, updateDate);

                    if (makeProduct_DetailEdit.IsSuccess)
                    {
                        AutoMapper.Mapper.CreateMap<BatchProduct_DetialEdit, DB.TWSQLDB.Models.Product>();
                        AutoMapper.Mapper.Map(makeProduct_DetailEdit.Body, getProduct.Body);
                    }
                    DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
                    dbFront.Entry(getProduct.Body).State = EntityState.Unchanged;
                    dbFront.Entry(getProduct.Body).Property(x => x.Description).IsModified = true;
                    dbFront.Entry(getProduct.Body).Property(x => x.DescriptionTW).IsModified = true;
                    dbFront.Entry(getProduct.Body).Property(x => x.Warranty).IsModified = true;
                    dbFront.Entry(getProduct.Body).Property(x => x.UpdateUser).IsModified = true;
                    dbFront.Entry(getProduct.Body).Property(x => x.UpdateDate).IsModified = true;
                    dbFront.Entry(getProduct.Body).Property(x => x.SellerProductID).IsModified = true;

                    try
                    {
                        dbFront.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        log.Info(string.Format("正式商品批次修改資料失敗(expection); ProductID = {0}; ErrorMessage = {1}; StackTrace = {2}", itemSketch.Product.ProductID.Value, GetExceptionMessage(ex), ex.StackTrace));
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = getProduct.Msg;
                }
            }
            else
            {
                result.IsSuccess = false;
                log.Info("商品 ID 不可為 null 或 小於等於 0。");
            }

            if (result.IsSuccess)
            {
                log.Info(string.Format("正式商品批次修改資料成功; ProductID = {0}.", itemSketch.Product.ProductID.Value));
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }
        /// <summary>
        /// 批次修改 更新賣場
        /// </summary>
        /// <param name="itemSketch">更新資訊</param>
        /// <param name="updateDate">更新日期</param>
        /// <returns>成功失敗訊息</returns>
        private ActionResponse<bool> BatchUpdateItem_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, int delvType, DateTime updateDate)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            if (itemSketch.Item.ItemID != null && itemSketch.Item.ItemID.Value > 0)
            {
                // 讀取資料庫內要被更新的賣場資訊
                ActionResponse<DB.TWSQLDB.Models.Item> getItem = GetItem(itemSketch.Item.ItemID.Value);

                if (getItem.IsSuccess)
                {
                    // 組合要更新的資料
                    ActionResponse<BatchItem_DetialEdit> makeItem_DetailEdit = BatchMakeItem_DetailEdit(itemSketch, delvType, updateDate);


                    if (makeItem_DetailEdit.IsSuccess)
                    {
                        AutoMapper.Mapper.CreateMap<BatchItem_DetialEdit, DB.TWSQLDB.Models.Item>();
                        AutoMapper.Mapper.Map(makeItem_DetailEdit.Body, getItem.Body);
                    }

                    DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
                    dbFront.Entry(getItem.Body).State = EntityState.Unchanged;
                    //dbFront.Entry(getItem.Body).Property(x => x.Name).IsModified = true;
                    dbFront.Entry(getItem.Body).Property(x => x.ItemDesc).IsModified = true;
                    dbFront.Entry(getItem.Body).Property(x => x.DescTW).IsModified = true;
                    dbFront.Entry(getItem.Body).Property(x => x.Sdesc).IsModified = true;
                    dbFront.Entry(getItem.Body).Property(x => x.Spechead).IsModified = true;
                    //dbFront.Entry(getItem.Body).Property(x => x.PriceCard).IsModified = true;
                    //dbFront.Entry(getItem.Body).Property(x => x.PriceCash).IsModified = true;
                    //dbFront.Entry(getItem.Body).Property(x => x.Qty).IsModified = true;
                    //dbFront.Entry(getItem.Body).Property(x => x.SafeQty).IsModified = true;
                    dbFront.Entry(getItem.Body).Property(x => x.MarketPrice).IsModified = true;
                    dbFront.Entry(getItem.Body).Property(x => x.UpdateUser).IsModified = true;
                    dbFront.Entry(getItem.Body).Property(x => x.UpdateDate).IsModified = true;
                    try
                    {
                        dbFront.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        log.Info(string.Format("正式賣場批次修改資料失敗(expection); ItemID = {0}; ErrorMessage = {1}; StackTrace = {2}.", itemSketch.Item.ItemID.Value, GetExceptionMessage(ex), ex.StackTrace));
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = getItem.Msg;
                }
            }
            else
            {
                result.IsSuccess = false;
                log.Info("賣場 ID 不可為 null 或 小於等於 0。");
            }

            if (result.IsSuccess)
            {
                log.Info(string.Format("正式賣場批次修改資料成功; ItemID = {0}.", itemSketch.Item.ItemID.Value));
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 批次修改 更新庫存
        /// </summary>
        /// <param name="itemSketch">更新資訊</param>
        /// <param name="updateDate">更新日期</param>
        /// <returns>成功失敗訊息</returns>
        private ActionResponse<bool> BatchUpdateItemStock_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, DateTime updateDate)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            if (itemSketch.Product.ProductID != null && itemSketch.Product.ProductID.Value > 0)
            {
                // 讀取資料庫內要被更新的庫存資訊
                ActionResponse<DB.TWSQLDB.Models.ItemStock> getItemStock = GetItemStock(itemSketch.Product.ProductID.Value);

                if (getItemStock.IsSuccess)
                {
                    // 組合要更新的資料
                    ActionResponse<BatchItemStock_DetialEdit> makeItemTemp_DetailEditResult = BatchMakeItemStock_DetailEdit(itemSketch, updateDate);

                    if (makeItemTemp_DetailEditResult.IsSuccess)
                    {
                        AutoMapper.Mapper.CreateMap<BatchItemStock_DetialEdit, DB.TWSQLDB.Models.ItemStock>();
                        AutoMapper.Mapper.Map(makeItemTemp_DetailEditResult.Body, getItemStock.Body);
                    }

                    DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
                    dbFront.Entry(getItemStock.Body).State = EntityState.Unchanged;
                    dbFront.Entry(getItemStock.Body).Property(x => x.Qty).IsModified = true;
                    dbFront.Entry(getItemStock.Body).Property(x => x.SafeQty).IsModified = true;
                    dbFront.Entry(getItemStock.Body).Property(x => x.UpdateUser).IsModified = true;
                    dbFront.Entry(getItemStock.Body).Property(x => x.UpdateDate).IsModified = true;
                    try
                    {
                        dbFront.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        log.Info(string.Format("正式庫存批次修改資料失敗(expection); ProductID = {0}; ErrorMessage = {1}; StackTrace = {2}.", itemSketch.Product.ProductID.Value, GetExceptionMessage(ex), ex.StackTrace));
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = getItemStock.Msg;
                }
            }
            else
            {
                result.IsSuccess = false;
                log.Info("商品 ID 不可為 null 或 小於等於 0。");
            }

            if (result.IsSuccess)
            {
                log.Info(string.Format("正式庫存批次修改資料成功; ProductID = {0}.", itemSketch.Product.ProductID.Value));
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }
        /// <summary>
        /// 組合批次修改商品正式 Product (詳細表)
        /// </summary>
        /// <param name="updateDate">更新日期</param>
        /// <returns>正式  BatchEdit Product (詳細表)</returns>
        private ActionResponse<BatchProduct_DetialEdit> BatchMakeProduct_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, int delvType, DateTime updateDate)
        {
            ActionResponse<BatchProduct_DetialEdit> result = new ActionResponse<BatchProduct_DetialEdit>();
            result.Body = new BatchProduct_DetialEdit();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            try
            {
                AutoMapper.Mapper.CreateMap<TWNewEgg.API.Models.ItemSketch, BatchProduct_DetialEdit>()
                    //商品名稱
                    .ForMember(x => x.Name, x => x.MapFrom(src => src.Product.Name))
                    //商品名稱
                    .ForMember(x => x.NameTW, x => x.MapFrom(src => src.Product.Name))
                    //商品成本
                    .ForMember(x => x.Cost, x => x.MapFrom(src => src.Product.Cost))
                    // 商品中文說明
                    .ForMember(x => x.Description, x => x.MapFrom(src => src.Product.Description))
                    // 商品中文說明
                    .ForMember(x => x.DescriptionTW, x => x.MapFrom(src => src.Product.Description))
                    // 商品保固期(月)
                    .ForMember(x => x.Warranty, x => x.MapFrom(src => src.Product.Warranty))
                    // 商品中文說明
                    .ForMember(x => x.UpdateDate, x => x.MapFrom(src => src.CreateAndUpdate.UpdateDate))
                    // 商家商品編號
                    .ForMember(x => x.SellerProductID, x => x.MapFrom(src => src.Product.SellerProductID))
                    // 商品保固期(月)
                    .ForMember(x => x.UpdateUser, x => x.MapFrom(src => src.CreateAndUpdate.UpdateUser));

                AutoMapper.Mapper.Map(itemSketch, result.Body);

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                log.Info(string.Format("組合 BatchEdit Product 詳細表 Model 失敗 (expection); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }



        /// <summary>
        /// 組合批次修改 正式 ItemStock (詳細表)
        /// </summary>
        /// <param name="itemSketch">要更新的商品資訊</param>
        /// <param name="updateDate">更新日期</param>
        /// <returns>正式 ItemStock (詳細表)</returns>
        private ActionResponse<BatchItemStock_DetialEdit> BatchMakeItemStock_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, DateTime updateDate)
        {
            ActionResponse<BatchItemStock_DetialEdit> result = new ActionResponse<BatchItemStock_DetialEdit>();
            result.Body = new BatchItemStock_DetialEdit();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            try
            {
                AutoMapper.Mapper.CreateMap<TWNewEgg.API.Models.ItemSketch, BatchItemStock_DetialEdit>()
                    // 可售數量
                    .ForMember(x => x.Qty, x => x.MapFrom(src => src.ItemStock.CanSaleQty + src.ItemStock.InventoryQtyReg))
                    // 安全庫存
                    .ForMember(x => x.SafeQty, x => x.MapFrom(src => src.ItemStock.InventorySafeQty))
                    // 商品中文說明
                    .ForMember(x => x.UpdateDate, x => x.MapFrom(src => src.CreateAndUpdate.UpdateDate))
                    // 商品保固期(月)
                    .ForMember(x => x.UpdateUser, x => x.MapFrom(src => src.CreateAndUpdate.UpdateUser));


                AutoMapper.Mapper.Map(itemSketch, result.Body);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                log.Info(string.Format("組合 BatchEdit ItemStock 詳細表 Model 失敗 (expection); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }


        /// <summary>
        /// 組合批次修改 正式 Item (詳細表)
        /// </summary>
        /// <param name="updateDate">更新日期</param>
        /// <returns>正式 BatchEdit Item (詳細表)</returns>
        private ActionResponse<BatchItem_DetialEdit> BatchMakeItem_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, int delvType, DateTime updateDate)
        {
            ActionResponse<BatchItem_DetialEdit> result = new ActionResponse<BatchItem_DetialEdit>();
            result.Body = new BatchItem_DetialEdit();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            try
            {
                AutoMapper.Mapper.CreateMap<TWNewEgg.API.Models.ItemSketch, BatchItem_DetialEdit>()

                    //商品名稱
                    .ForMember(x => x.Name, x => x.MapFrom(src => src.Product.Name))
                    // 中文描述
                    .ForMember(x => x.DescTW, x => x.MapFrom(src => src.Product.Description))
                    // 中文描述
                    .ForMember(x => x.ItemDesc, x => x.MapFrom(src => src.Product.Description))
                    // 商品特色標題
                    .ForMember(x => x.Sdesc, x => x.MapFrom(src => src.Item.Sdesc))
                    // 商品簡要描述
                    .ForMember(x => x.Spechead, x => x.MapFrom(src => src.Item.Spechead))
                    // 商品賣價
                    .ForMember(x => x.PriceCard, x => x.MapFrom(src => src.Item.PriceCash))
                    // 商品賣價
                    .ForMember(x => x.PriceCash, x => x.MapFrom(src => src.Item.PriceCash))
                    // 市場建議售價
                    .ForMember(x => x.MarketPrice, x => x.MapFrom(src => src.Item.MarketPrice ?? 0))
                    // 庫存量
                    //.ForMember(x => x.Qty, x => x.MapFrom(src => src.ItemStock.InventoryQty))
                    //// 安全庫存量
                    //.ForMember(x => x.SafeQty, x => x.MapFrom(src => src.ItemStock.InventorySafeQty))
                    // 商品中文說明
                    .ForMember(x => x.UpdateDate, x => x.MapFrom(src => src.CreateAndUpdate.UpdateDate))
                    // 商品保固期(月)
                    .ForMember(x => x.UpdateUser, x => x.MapFrom(src => src.CreateAndUpdate.UpdateUser));

                AutoMapper.Mapper.Map(itemSketch, result.Body);

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                log.Info(string.Format("組合 BatchEdit Item 詳細表 Model 失敗 (expection); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }
        private ActionResponse<bool> BatchUpdateTemp_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, int delvType, DateTime updateDate)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            // Temp 待審狀態變更
            ActionResponse<bool> updateTempStatus_DetailEdit = BatchUpdateTempStatus_DetailEdit(itemSketch, updateDate);

            if (updateTempStatus_DetailEdit.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg += updateTempStatus_DetailEdit.Msg;
            }

            if (result.IsSuccess)
            {
                // 更新商品Temp
                ActionResponse<bool> updateProductTemp_DetailEdit = BatchUpdateProductTemp_DetailEdit(itemSketch, delvType, updateDate);

                if (updateProductTemp_DetailEdit.IsSuccess == false)
                {
                    result.IsSuccess = false;
                    result.Msg += updateProductTemp_DetailEdit.Msg;
                }

                if (result.IsSuccess)
                {
                    // 更新賣場Temp
                    ActionResponse<bool> updateItemTemp_DetailEdit = BatchUpdateItemTemp_DetailEdit(itemSketch, delvType, updateDate);

                    if (updateItemTemp_DetailEdit.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg += updateItemTemp_DetailEdit.Msg;
                    }

                    if (result.IsSuccess)
                    {
                        // 更新庫存Temp
                        ActionResponse<bool> updateItemStockTemp_DetailEdit = BatchUpdateItemStockTemp_DetailEdit(itemSketch, updateDate);

                        if (updateItemStockTemp_DetailEdit.IsSuccess == false)
                        {
                            result.IsSuccess = false;
                            result.Msg += updateItemStockTemp_DetailEdit.Msg;
                        }
                        if (result.IsSuccess)
                        {
                            // 更新跨分類Temp
                            ActionResponse<bool> updateItemCategoryTemp_DetailEdit = UpdateItemCategoryTemp_DetailEdit(itemSketch, updateDate);

                            if (updateItemCategoryTemp_DetailEdit.IsSuccess == false)
                            {
                                result.IsSuccess = false;
                                result.Msg += updateItemCategoryTemp_DetailEdit.Msg;
                            }
                        }
                    }
                }
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }
        /// <summary>
        /// 批次修改 Temp 待審狀態變更
        /// </summary>
        /// <param name="itemSketch">更新資訊</param>
        /// <param name="updateDate">更新日期</param>
        /// <returns>成功失敗訊息</returns>
        private ActionResponse<bool> BatchUpdateTempStatus_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, DateTime updateDate)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            if (itemSketch.Item.ID > 0 && itemSketch.Product.ID > 0)
            {
                ActionResponse<DB.TWSQLDB.Models.ItemTemp> getItemTemp = GetItemTemp(itemSketch.Item.ID);
                ActionResponse<DB.TWSQLDB.Models.ProductTemp> getProductTemp = GetProductTemp(itemSketch.Product.ID);

                if (getItemTemp.IsSuccess && getProductTemp.IsSuccess)
                {
                    DB.TWSQLDB.Models.ItemTemp itemTemp = getItemTemp.Body;
                    DB.TWSQLDB.Models.ProductTemp productTemp = getProductTemp.Body;

                    // 是否有修改值(用於判斷是否變更審核狀態)
                    bool isChangeValue = false;

                    // 修改商品名稱
                    if (itemTemp.Name != itemSketch.Product.Name)
                    {
                        log.Info(string.Format("Product.ID :" + itemSketch.Product.ID + "修改商品名稱為 " + itemSketch.Product.Name));
                        isChangeValue = true;
                    }

                    // 修改售價
                    if ((itemSketch.Item.PriceCash.HasValue && itemTemp.PriceCash != itemSketch.Item.PriceCash.Value) || (itemSketch.Item.PriceCash.HasValue && itemTemp.PriceCard != itemSketch.Item.PriceCash.Value))
                    {
                        log.Info(string.Format("Product.ID :" + itemSketch.Product.ID + "修改售價為 " + itemSketch.Item.PriceCash));
                        isChangeValue = true;
                    }

                    // 修改成本
                    if (itemSketch.Product.Cost.HasValue && productTemp.Cost != itemSketch.Product.Cost)
                    {
                        log.Info(string.Format("Product.ID :" + itemSketch.Product.ID + "修改成本為 " + itemSketch.Product.Cost));
                        isChangeValue = true;
                    }

                    // 未通過又是新品建立，直接給未審核

                    if (getItemTemp.Body.Status == 2 && !getItemTemp.Body.ItemID.HasValue && !getProductTemp.Body.ProductID.HasValue)
                    {
                        log.Info(string.Format("Product.ID :" + itemSketch.Product.ID + "未通過又是新品建立，直接給未審核 "));
                        isChangeValue = true;
                    }

                    if (isChangeValue)
                    {
                        //變更狀態時計算毛利率
                        #region 計算毛利率

                        decimal? grossMargin = null;

                        if (itemSketch.Product.Cost == null || itemSketch.Item.PriceCash == null || itemSketch.Item.PriceCash.Value == 0)
                        {
                            grossMargin = 0;
                        }
                        else
                        {
                            grossMargin = ((itemSketch.Item.PriceCash.Value - itemSketch.Product.Cost.Value) / itemSketch.Item.PriceCash.Value) * 100m;
                            grossMargin = Decimal.Round(grossMargin.Value, 0);
                        }

                        itemTemp.GrossMargin = grossMargin;

                        #endregion 計算毛利率

                        // 變更審核狀態
                        itemTemp.Status = 1;
                        productTemp.Status = 1;

                        log.Info(string.Format("Product.ID :" + itemSketch.Product.ID + "邊更審核狀態 : 未審核"));

                        //更新送審日期、送審人
                        itemTemp.SubmitDate = updateDate;
                        itemTemp.SubmitMan = itemSketch.CreateAndUpdate.UpdateUser.ToString();

                        log.Info(string.Format("Product.ID :" + "更新送審日期、送審人 " + updateDate + "、" + itemSketch.CreateAndUpdate.UpdateUser.ToString()));

                        DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();

                        dbFront.Entry(itemTemp).State = EntityState.Unchanged;
                        dbFront.Entry(itemTemp).Property(x => x.SubmitDate).IsModified = true;
                        dbFront.Entry(itemTemp).Property(x => x.SubmitMan).IsModified = true;
                        dbFront.Entry(itemTemp).Property(x => x.Status).IsModified = true;
                        dbFront.Entry(itemTemp).Property(x => x.GrossMargin).IsModified = true;

                        dbFront.Entry(productTemp).State = EntityState.Unchanged;
                        dbFront.Entry(productTemp).Property(x => x.Status).IsModified = true;

                        try
                        {
                            dbFront.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            result.IsSuccess = false;
                            log.Info(string.Format("Temp 待審狀態變更失敗(expection); ItemTempID = {0}; ProductTempID = {1}; ErrorMessage = {2}; StackTrace = {3}.", itemSketch.Item.ID, itemSketch.Product.ID, GetExceptionMessage(ex), ex.StackTrace));
                        }
                    }
                }
                else
                {
                    result.IsSuccess = false;

                    if (getItemTemp.IsSuccess == false)
                    {
                        result.Msg += getItemTemp.Msg;
                    }

                    if (getProductTemp.IsSuccess == false)
                    {
                        result.Msg += getProductTemp.Msg;
                    }
                }
            }
            else
            {
                if (itemSketch.Item.ID <= 0)
                {
                    result.IsSuccess = false;
                    log.Info("ItemTempID 不可小於等於 0。");
                }

                if (itemSketch.Product.ID <= 0)
                {
                    result.IsSuccess = false;
                    log.Info("ProductTempID 不可小於等於 0。");
                }
            }

            if (result.IsSuccess)
            {
                log.Info(string.Format("Temp 待審狀態變更成功。"));
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }
        /// <summary>
        /// 批次修改 更新商品Temp
        /// </summary>
        /// <param name="itemSketch">更新資訊</param>
        /// <param name="updateDate">更新日期</param>
        /// <returns>成功失敗訊息</returns>
        private ActionResponse<bool> BatchUpdateProductTemp_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, int delvType, DateTime updateDate)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            if (itemSketch.Product.ID > 0)
            {
                // 讀取資料庫內要被更新的賣場資訊
                ActionResponse<DB.TWSQLDB.Models.ProductTemp> getProductTemp = GetProductTemp(itemSketch.Product.ID);

                if (getProductTemp.IsSuccess)
                {
                    // 組合要更新的資料
                    ActionResponse<BatchProduct_DetialEdit> makeProduct_DetailEdit = BatchMakeProduct_DetailEdit(itemSketch, delvType, updateDate);

                    if (makeProduct_DetailEdit.IsSuccess)
                    {
                        AutoMapper.Mapper.CreateMap<BatchProduct_DetialEdit, DB.TWSQLDB.Models.ProductTemp>();
                        AutoMapper.Mapper.Map(makeProduct_DetailEdit.Body, getProductTemp.Body);
                    }

                    DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
                    dbFront.Entry(getProductTemp.Body).State = EntityState.Unchanged;
                    //dbFront.Entry(getProductTemp.Body).Property(x => x.Name).IsModified = true;
                    //dbFront.Entry(getProductTemp.Body).Property(x => x.NameTW).IsModified = true;
                    dbFront.Entry(getProductTemp.Body).Property(x => x.Description).IsModified = true;
                    dbFront.Entry(getProductTemp.Body).Property(x => x.DescriptionTW).IsModified = true;
                    dbFront.Entry(getProductTemp.Body).Property(x => x.Cost).IsModified = true;
                    dbFront.Entry(getProductTemp.Body).Property(x => x.Warranty).IsModified = true;
                    dbFront.Entry(getProductTemp.Body).Property(x => x.UpdateUser).IsModified = true;
                    dbFront.Entry(getProductTemp.Body).Property(x => x.UpdateDate).IsModified = true;
                    dbFront.Entry(getProductTemp.Body).Property(x => x.SellerProductID).IsModified = true;
                    try
                    {
                        dbFront.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        log.Info(string.Format("Temp 商品批次修改資料失敗(expection); ProductTempID = {0}; ErrorMessage = {1}; StackTrace = {2}", itemSketch.Product.ID, GetExceptionMessage(ex), ex.StackTrace));
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = getProductTemp.Msg;
                }
            }
            else
            {
                result.IsSuccess = false;
                log.Info("商品 TempID 不可於小於等於 0。");
            }

            if (result.IsSuccess)
            {
                log.Info(string.Format("Temp 商品修改資料成功; ProductTempID = {0}.", itemSketch.Product.ID));
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 批次修改 更新賣場Temp
        /// </summary>
        /// <param name="itemSketch">更新資訊</param>
        /// <param name="updateDate">更新日期</param>
        /// <returns>成功失敗訊息</returns>
        private ActionResponse<bool> BatchUpdateItemTemp_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, int delvType, DateTime updateDate)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            if (itemSketch.Item.ID > 0)
            {
                // 讀取資料庫內要被更新的賣場資訊
                ActionResponse<DB.TWSQLDB.Models.ItemTemp> getItemTemp = GetItemTemp(itemSketch.Item.ID);

                if (getItemTemp.IsSuccess)
                {
                    // 組合要更新的資料
                    ActionResponse<BatchItem_DetialEdit> makeItem_DetailEdit = BatchMakeItem_DetailEdit(itemSketch, delvType, updateDate);

                    if (makeItem_DetailEdit.IsSuccess)
                    {
                        AutoMapper.Mapper.CreateMap<BatchItem_DetialEdit, DB.TWSQLDB.Models.ItemTemp>()
                            .ForMember(x => x.ItemTempDesc, x => x.MapFrom(src => src.ItemDesc));
                        AutoMapper.Mapper.Map(makeItem_DetailEdit.Body, getItemTemp.Body);
                    }

                    DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
                    dbFront.Entry(getItemTemp.Body).State = EntityState.Unchanged;
                    dbFront.Entry(getItemTemp.Body).Property(x => x.Name).IsModified = true;
                    dbFront.Entry(getItemTemp.Body).Property(x => x.ItemTempDesc).IsModified = true;
                    dbFront.Entry(getItemTemp.Body).Property(x => x.DescTW).IsModified = true;
                    dbFront.Entry(getItemTemp.Body).Property(x => x.Sdesc).IsModified = true;
                    dbFront.Entry(getItemTemp.Body).Property(x => x.Spechead).IsModified = true;
                    dbFront.Entry(getItemTemp.Body).Property(x => x.PriceCard).IsModified = true;
                    dbFront.Entry(getItemTemp.Body).Property(x => x.PriceCash).IsModified = true;
                    //dbFront.Entry(getItemTemp.Body).Property(x => x.Qty).IsModified = true;
                    //dbFront.Entry(getItemTemp.Body).Property(x => x.SafeQty).IsModified = true;
                    dbFront.Entry(getItemTemp.Body).Property(x => x.MarketPrice).IsModified = true;
                    dbFront.Entry(getItemTemp.Body).Property(x => x.UpdateUser).IsModified = true;
                    dbFront.Entry(getItemTemp.Body).Property(x => x.UpdateDate).IsModified = true;
                    try
                    {
                        dbFront.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        log.Info(string.Format("Temp 賣場批次修改資料失敗(expection); ItemTempID = {0}; ErrorMessage = {1}; StackTrace = {2}.", itemSketch.Item.ID, GetExceptionMessage(ex), ex.StackTrace));
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = getItemTemp.Msg;
                }
            }
            else
            {
                result.IsSuccess = false;
                log.Info("賣場 TempID 不可小於等於 0。");
            }

            if (result.IsSuccess)
            {
                log.Info(string.Format("Temp 賣場修改資料成功; ItemTempID = {0}.", itemSketch.Item.ID));
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 批次修改 更新庫存Temp
        /// </summary>
        /// <param name="itemSketch">更新資訊</param>
        /// <param name="updateDate">更新日期</param>
        /// <returns>成功失敗訊息</returns>
        private ActionResponse<bool> BatchUpdateItemStockTemp_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, DateTime updateDate)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            if (itemSketch.Product.ID > 0)
            {
                // 讀取資料庫內要被更新的庫存資訊
                ActionResponse<DB.TWSQLDB.Models.ItemStocktemp> getItemStockTemp = GetItemStockTemp(itemSketch.Product.ID);

                if (getItemStockTemp.IsSuccess)
                {
                    // 組合要更新的資料
                    ActionResponse<BatchItemStock_DetialEdit> makeItemStock_DetailEdit = BatchMakeItemStock_DetailEdit(itemSketch, updateDate);

                    if (makeItemStock_DetailEdit.IsSuccess)
                    {
                        AutoMapper.Mapper.CreateMap<BatchItemStock_DetialEdit, DB.TWSQLDB.Models.ItemStocktemp>();
                        AutoMapper.Mapper.Map(makeItemStock_DetailEdit.Body, getItemStockTemp.Body);
                    }

                    DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
                    dbFront.Entry(getItemStockTemp.Body).State = EntityState.Unchanged;
                    dbFront.Entry(getItemStockTemp.Body).Property(x => x.Qty).IsModified = true;
                    dbFront.Entry(getItemStockTemp.Body).Property(x => x.SafeQty).IsModified = true;
                    dbFront.Entry(getItemStockTemp.Body).Property(x => x.UpdateUser).IsModified = true;
                    dbFront.Entry(getItemStockTemp.Body).Property(x => x.UpdateDate).IsModified = true;

                    try
                    {
                        dbFront.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        log.Info(string.Format("Temp 庫存批次修改資料失敗(expection); ProductTempID = {0}; ErrorMessage = {1}; StackTrace = {2}.", itemSketch.Product.ID, GetExceptionMessage(ex), ex.StackTrace));
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = getItemStockTemp.Msg;
                }
            }
            else
            {
                result.IsSuccess = false;
                log.Info("商品 TempID 不可小於等於 0。");
            }

            if (result.IsSuccess)
            {
                log.Info(string.Format("Temp 庫存批次修改資料成功; ProductTempID = {0}.", itemSketch.Product.ID));
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }
        #endregion
        #endregion 待審編輯

        #region Temp 待審編輯欄位檢查

        public ActionResponse<List<string>> check(TWNewEgg.API.Models.ItemSketch EditModel, bool checkAll = true)
        {
            ActionResponse<List<string>> result = new ActionResponse<List<string>>();

            List<string> checkresult = new List<string>();

            if (EditModel == null || EditModel.Item.ID == 0)
            {
                result.Finish(false, (int)ResponseCode.Error, "商品修改無傳入，修改失敗", null);

                return result;
            }
            else
            {                
                bool statusresult = this.checkstatus(EditModel);

                // 未審核狀態又無新蛋賣場、產品編號不供編輯
                if (statusresult == false)
                {
                    result.Finish(false, (int)ResponseCode.Error, "商品修改有賣場資料無法查詢，修改失敗", null);

                    return result;
                }
                string msg = checkdeatil(EditModel, checkAll);

                if (string.IsNullOrWhiteSpace(msg))
                {
                    result.Finish(true, (int)ResponseCode.Success, "", null);
                }
                else
                {

                    checkresult.Add("新蛋賣場編號: " + EditModel.Item.ItemID + ";" + msg);
                    result.Finish(false, (int)ResponseCode.AccessError, "", checkresult);
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EditModel"></param>
        /// <returns></returns>
        private bool checkstatus(Models.ItemSketch EditModel)
        {            
            var itemtempstatus = db.ItemTemp.Where(x => x.ID == EditModel.Item.ID).FirstOrDefault();

            bool statuscheckResult = true;

            if (itemtempstatus != null)
            {
                switch (itemtempstatus.Status)
                {
                    // 審核通過
                    case 0:
                    // 未審核
                    case 1:
                        if (EditModel.Item.ItemID == 0 || EditModel.Product.ProductID == 0
                    || EditModel.CreateAndUpdate.UpdateUser == 0 || EditModel.Item.SellerID == 0)
                        {
                            statuscheckResult = false;
                        }
                        break;
                    // 未通過
                    case 2:
                        statuscheckResult = true;
                        break;
                }
            }
           
            return statuscheckResult;
        }

        public ActionResponse<List<string>> check(List<TWNewEgg.API.Models.ItemSketch> UpdateListItemTemp, bool checkAll = false)
        {
            ActionResponse<List<string>> result = new ActionResponse<List<string>>();

            List<string> checkresult = new List<string>();

            if (UpdateListItemTemp == null)
            {
                result.Finish(false, (int)ResponseCode.Error, "商品修改無傳入，修改失敗", null);

                return result;
            }
            else
            {
                // 判斷傳入的資料是否有未審核狀態

                if (UpdateListItemTemp.Where(x => x.Item.status == 1 && (x.Item.ItemID == 0 || x.Product.ProductID == 0
                || x.CreateAndUpdate.UpdateUser == 0 || x.Item.SellerID == 0)).Any())
                {
                    result.Finish(false, (int)ResponseCode.Error, "商品修改有賣場資料無法查詢，修改失敗", null);

                    return result;
                }
                else
                {
                    #region 檢查每一筆資料欄位是否正確

                    foreach (var index in UpdateListItemTemp)
                    {
                        string msg = checkdeatil(index, checkAll);
                        if (!string.IsNullOrWhiteSpace(msg))
                        {
                            try
                            {
                                if (msg.IndexOf("賣場:") == -1)
                                {
                                    checkresult.Add("新蛋賣場編號: " + index.Item.ItemID + ";" + msg);
                                }
                                else
                                {
                                    checkresult.Add(msg.Replace("賣場:", "新蛋賣場編號: "));
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex.Message);
                                result.Finish(false, (int)ResponseCode.Error, "發生錯誤訊息，修改失敗", null);
                                return result;
                            }

                        }
                    }

                    if (checkresult.Any() || checkresult.Count > 0)
                    {
                        result.Finish(false, (int)ResponseCode.AccessError, "商品修改資料內容有誤，修改失敗", checkresult);
                    }
                    else
                    {
                        result.Finish(true, (int)ResponseCode.Success, "商品修改資料內容正確", null);
                    }

                    #endregion
                }
            }

            return result;
        }

        private string checkdeatil(TWNewEgg.API.Models.ItemSketch EditModel, bool checkAll)
        {
            string result = string.Empty;
            // 先檢查 8 個欄位
            try
            {
                #region SellerID，UpdateUserID

                if (!EditModel.Item.SellerID.HasValue)
                {
                    result += "無法查詢賣場供應商資料，修改失敗";
                }

                #endregion

                #region 審核狀態判斷 table 是否存在

                result += this.checkUpdateExist(EditModel);
              
                #endregion
                
                #region ShipType
                             
                // 檢查 Item.ShipType
                if (string.IsNullOrWhiteSpace(EditModel.Item.ShipType))
                {
                    result += "遞送方式不可為空";
                }
                else
                {
                    if (EditModel.Item.ShipType != "S" && EditModel.Item.ShipType != "V" && EditModel.Item.ShipType != "N")
                    {
                        result += "遞送方式型態有誤，無法判斷";
                    }
                    else
                    {

                    }
                }

                #endregion

                #region Product.Cost

                if (EditModel.Product.Cost <= 0)
                {
                    result += "成本必須大於 0";
                }
                else
                {
                    if (EditModel.Item.PriceCash < EditModel.Product.Cost)
                    {
                        result += "賣場:" + EditModel.Item.ItemID + "毛利率為負數，請重新設定售價或成本";
                    }
                }

                #endregion

                #region Item.DateStart

                if (new TimeSpan(EditModel.Item.DateStart.Ticks - EditModel.Item.DateEnd.Ticks).Days > 0)
                {
                    result += "上架日期必須早於下架日期";
                }

                #endregion

                #region Item.itemqty 賣場限量

                if (EditModel.Item.CanSaleLimitQty.Value > EditModel.ItemStock.CanSaleQty.Value)
                {
                    result += "賣場限量數量必須小於等於可售數量";
                }

                //if (EditModel.Item.CanSaleLimitQty.Value < 0)
                //{
                //    result += "賣場限量數量必須大於 0";
                //}

                #endregion

                #region ItemStock.SafeQty 安全庫存量

                if (EditModel.ItemStock.InventorySafeQty < 0)
                {
                    result += "安全庫存量數量必須大於 0";
                }

                if (EditModel.ItemStock.InventorySafeQty > EditModel.ItemStock.CanSaleQty.Value)
                {
                    result += "賣場安全庫存量不得大於可售數量";
                }

                #endregion

                // 檢查 Detail 畫面欄位
                if (checkAll == true)
                {
                    #region SellerProductID
                    #endregion

                    #region 製造商ID

                    if (!EditModel.Product.ManufactureID.HasValue || EditModel.Product.ManufactureID.Value == 0)
                    {
                        result += "請選擇製造商ID";
                    }

                    #endregion

                    #region 非必填 Product.Model

                    if (!string.IsNullOrWhiteSpace(EditModel.Product.Model))
                    {
                        if (EditModel.Product.Model.Length <= 30)
                        {
                            bool isNum = IsChiness(EditModel.Product.Model);

                            if (!isNum)
                            {
                                result += "商品型號，不可輸入中文";
                            }
                        }
                        else
                        {
                            result += "商品型號的欄位字數限制為30(中文2、英數1)，請檢查修改";
                        }
                    }

                    #endregion

                    #region 非必填 Product.Warranty

                    if (EditModel.Product.Warranty.HasValue)
                    {
                        if (EditModel.Product.Warranty.Value < 0)
                        {
                            result += "商品保固期(月)的欄位需大於0，請檢查修改";
                        }
                    }

                    #endregion

                    #region Product.BarCode

                    if (!string.IsNullOrWhiteSpace(EditModel.Product.BarCode))
                    {
                        if (EditModel.Product.BarCode.Length <= 50)
                        {
                            if (!this.IsNatural_Number(EditModel.Product.BarCode))
                            {
                                result += "只能允許輸入英文與數字。";
                            }
                        }
                        else
                        {
                            result += "條碼的欄位字數限制為50(中文2、英數1)，請檢查修改。";
                        }
                    }

                    #endregion

                    #region Product.Length、Width、Weight

                    if (EditModel.Product.Length.HasValue)
                    {
                        if (EditModel.Product.Length.Value <= 0)
                        {
                            result += "商品長度不得為 0";
                        }
                        else
                        {
                            if (EditModel.Product.Length > decimal.MaxValue)
                            {
                                result += "商品長度超過資料庫限制";
                            }
                        }
                    }
                    else
                    {
                        result += "商品長度不得為空";
                    }


                    if (EditModel.Product.Weight.HasValue)
                    {
                        if (EditModel.Product.Weight <= 0)
                        {
                            result += "商品重量不得為 0";
                        }
                        else
                        {
                            // 暫時無限制資料庫長度比較
                        }
                    }
                    else
                    {
                        result += "商品重量不得為空";
                    }



                    if (EditModel.Product.Width.HasValue)
                    {
                        if (EditModel.Product.Width <= 0)
                        {
                            result += "商品寬度不得為 0";
                        }
                        else
                        {
                            // 暫時無限制資料庫長度比較
                        }
                    }
                    else
                    {
                        result += "商品寬度不得為空";
                    }

                    #endregion

                    #region Product.UPC

                    if (!string.IsNullOrWhiteSpace(EditModel.Product.UPC))
                    {
                        if (EditModel.Product.UPC.Length <= 15)
                        {
                            if (!this.IsNatural_Number(EditModel.Product.UPC))
                            {
                                result += "不可輸入中文，只能輸入英文與數字。";
                            }
                        }
                        else
                        {
                            result += "商品 UPC 條件的欄位字數限制為15(中文2、英數1)，請檢查修改。";
                        }
                    }


                    #endregion

                    #region Product.到貨天數

                    if (string.IsNullOrWhiteSpace(EditModel.Item.DelvDate))
                    {
                        result += "到貨天數為必填，請檢查修改。";
                    }
                    else
                    {
                        if (EditModel.Item.DelvDate.Length > 40)
                        {
                            result += "到貨天數的欄位字數限制為50(中文2、英數1)，請檢查修改。";
                        }
                    }

                    #endregion

                    #region Item.MarketPrice

                    if (EditModel.Item.MarketPrice.HasValue)
                    {
                        if (EditModel.Item.MarketPrice.Value < 0)
                        {
                            result += "市場建議售價的欄位只能填寫大於0的數字，請檢查修改。";
                        }
                        else
                        {
                            // 無跟 DB 欄位長度限制比較
                        }
                    }

                    #endregion

                    #region Item.PriceCash

                    if (EditModel.Item.PriceCash.HasValue)
                    {
                        if (EditModel.Item.PriceCash.Value <= 0)
                        {
                            result += "售價的欄位只能填寫大於0的數字，請檢查修改。";
                        }
                        else
                        {
                            // 無跟 DB 欄位長度限制比較
                        }
                    }

                    #endregion

                    #region Item.LimitQty

                    if (EditModel.Item.QtyLimit.HasValue)
                    {
                        if (EditModel.Item.QtyLimit.Value < 0)
                        {
                            result += "限購數量欄位只能填寫大於0的數字，請檢查修改。";
                        }
                        else
                        {
                            // 無跟 DB 欄位長度限制比較
                        }
                    }

                    #endregion

                    #region Item.Qty

                    //if (EditModel.Item.CanSaleLimitQty.HasValue)
                    //{
                    //    if (EditModel.Item.CanSaleLimitQty.Value < 0)
                    //    {
                    //        result += "商品可售限量數量欄位只能填寫大於0的數字，請檢查修改。";
                    //    }
                    //    else
                    //    {
                    //        // 無跟 DB 欄位長度限制比較
                    //    }
                    //}

                    #endregion

                    #region Product.Name

                    if (string.IsNullOrWhiteSpace(EditModel.Product.Name))
                    {
                        result += "商品名稱不可為空";
                    }
                    else
                    {
                        if (EditModel.Product.Name.Length <= 2000)
                        {

                        }
                        else
                        {
                            result += "商品標題的欄位字數限制為2000(中文2、英數1)，請檢查修改。";
                        }
                    }

                    #endregion

                    #region 特色標題 Item.sdesc

                    if (string.IsNullOrWhiteSpace(EditModel.Item.Sdesc))
                    {
                        result += "商品特色標題的欄位為必填，請檢查修改。";
                    }
                    else
                    {
                        if (EditModel.Item.Sdesc.Length <= 500)
                        {

                        }
                        else
                        {
                            result += "商品特色標題的欄位字數限制為500(中文2、英數1)，請檢查修改。";
                        }
                    }

                    #endregion

                    #region 簡要描述 Item

                    if (string.IsNullOrWhiteSpace(EditModel.Item.Spechead))
                    {
                        result += "商品簡要描述的欄位為必填，請檢查修改。";
                    }
                    else
                    {
                        if (EditModel.Item.Spechead.Length <= 30)
                        {

                        }
                        else
                        {
                            result += "商品簡要描述的欄位字數限制為30(中文2、英數1)，請檢查修改。";
                        }
                    }

                    #endregion

                    #region 中文說明

                    if (string.IsNullOrWhiteSpace(EditModel.Product.Description))
                    {
                        result += "商品中文說明為必填，請檢查修改。";
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                log.Error("ItemTempID: " + EditModel.Item.ID);
                log.Error(result);
                log.Error("Exception: " + ex.Message + ", StackTrace: " + ex.StackTrace);

                result = "欄位檢查發生意外錯誤，請稍後再試";
            }

            return result;
        }

        private string checkUpdateExist(Models.ItemSketch EditModel)
        {
            string result = string.Empty;

            try
            {
                var itemtempModel = db.ItemTemp.Where(x => x.ID == EditModel.Item.ID && x.SellerID == EditModel.Item.SellerID).FirstOrDefault();

                if (itemtempModel != null)
                {
                    // 根據不同審核狀態進行判斷
                    switch (itemtempModel.Status)
                    {
                        default:
                        // 審核通過
                        case 0:
                        // 未審核
                        case 1:
                            {
                                #region ItemID，ItemTempID

                                if (EditModel.Item.ID == 0 || !EditModel.Item.ItemID.HasValue)
                                {
                                    result += "無法查詢Item資料，修改失敗";
                                }
                                else
                                {
                                    var checkItem = db.Item.Where(x => x.ID == EditModel.Item.ItemID.Value && x.SellerID == EditModel.Item.SellerID).Any();
                                    var checkItemTemp = db.ItemTemp.Where(x => x.ID == EditModel.Item.ID && x.SellerID == EditModel.Item.SellerID).Any();

                                    if (checkItem == false || checkItemTemp == false)
                                    {
                                        result += "無法查詢Item資料，修改失敗";
                                    }
                                }

                                #endregion

                                #region ProductID，ProductTempID

                                if (EditModel.Product.ID == 0 || !EditModel.Product.ProductID.HasValue)
                                {
                                    result += "無法查詢Product資料，修改失敗";
                                }
                                else
                                {
                                    var checkProduct = db.Product.Where(x => x.ID == EditModel.Product.ProductID.Value && x.SellerID == EditModel.Item.SellerID).Any();
                                    var checkProductTemp = db.ProductTemp.Where(x => x.ID == EditModel.Product.ID && x.SellerID == EditModel.Item.SellerID).Any();

                                    if (checkProduct == false || checkProductTemp == false)
                                    {
                                        result += "無法查詢Product資料，修改失敗";
                                    }
                                }

                                #endregion
                            }
                            break;
                        // 未通過
                        case 2:
                            {
                                if (EditModel.Item.ID != 0 || EditModel.Product.ID != 0)
                                {
                                    var checkProductTemp = db.ProductTemp.Where(x => x.ID == EditModel.Product.ID && x.SellerID == EditModel.Item.SellerID).Any();
                                    var checkItemTemp = db.ItemTemp.Where(x => x.ID == EditModel.Item.ID && x.SellerID == EditModel.Item.SellerID).Any();

                                    if (checkItemTemp == false || checkProductTemp == false)
                                    {
                                        result += "待審資料無法查詢並進行修改，修改失敗";
                                    }

                                    if (EditModel.Product.ProductID.HasValue && EditModel.Item.ItemID.HasValue)
                                    {
                                        var checkProduct = db.Product.Where(x => x.ID == EditModel.Product.ProductID.Value && x.SellerID == EditModel.Item.SellerID).Any();
                                        var checkItem = db.Item.Where(x => x.ID == EditModel.Item.ItemID.Value && x.SellerID == EditModel.Item.SellerID).Any();

                                        if (checkItem == false || checkProduct == false)
                                        {
                                            result += "待審資料無法查詢並進行修改，修改失敗";
                                        }
                                    }
                                }
                                else
                                {
                                    result += "未通過資料無法查詢並進行修改，修改失敗";
                                }
                            }
                            break;
                    }
                }
                else
                {
                    result += "無法查詢供應商審核資料，修改失敗";
                }
            }
            catch (Exception ex)
            {
                log.Error("Exception: " + ex.Message + ", StackTrace: " + ex.StackTrace);
                result += "審核資料查詢發生意外錯誤，請稍後再試";
            }

            return result;
        }

        #endregion

        #region Field Check Regular

        // 判斷是否英文與數字，以及 - _ (允許空格)
        public bool IsNatural_Number(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9-_\s]+$");

            return reg.IsMatch(str);
        }

        public bool IsChiness(string str)
        {
            Regex reg = new Regex(@"^[\x20-\x7E]+$");//Regex reg = new Regex(@"[\u4e00-\u9fa5]+\w*[\uFF00-\uFFFF]+");

            if (reg.IsMatch(str))
            {
                return true;
            }

            return false;
        }

        #endregion

        /// <summary>
        /// 設定 Reponse Code
        /// </summary>
        /// <param name="isSuccess">成功失敗資訊</param>
        /// <returns>Reponse Code</returns>
        private int SetResponseCode(bool isSuccess)
        {
            if (isSuccess)
            {
                return (int)ResponseCode.Success;
            }
            else
            {
                return (int)ResponseCode.Error;
            }
        }

        /// <summary>
        /// 取得 Exception 錯誤訊息
        /// </summary>
        /// <param name="ex">Exception 內容</param>
        /// <returns>Exception 錯誤訊息</returns>
        private string GetExceptionMessage(System.Exception ex)
        {
            string errorMessage = string.Empty;

            if (ex.Message.IndexOf("See the inner exception for details.") != -1)
            {
                errorMessage = ex.InnerException.Message;

                if (errorMessage.IndexOf("See the inner exception for details.") != -1)
                {
                    errorMessage = GetExceptionMessage(ex.InnerException);
                }
            }
            else
            {
                errorMessage = ex.Message;
            }

            return errorMessage;
        }

        #region 規格品排除

        public List<Models.ItemSketch> DistinctTempGroup(TWNewEgg.API.Models.ItemSketchSearchCondition condition, List<Models.ItemSketch> list)
        {
            TWNewEgg.DB.TWSqlDBContext db = new DB.TWSqlDBContext();

            // 找出此 Seller 的規格品待審 ID
            List<int> ItemTempGroup = db.ItemGroupDetailProperty.Where(x => x.SellerID == condition.SellerID && x.ItemTempID.HasValue).Select(r => r.ItemTempID.Value).ToList();

            var rrr = list.Where(x => !ItemTempGroup.Contains(x.Item.ID)).ToList();

            return rrr;
        }

        #endregion
    }
}
