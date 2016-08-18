using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using TWNewEgg.API.Models;
using AutoMapper;
using System.Web;
using System.Transactions;
using System.Data;


namespace TWNewEgg.API.Service
{
    public class PropertyService
    {
        private static ILog log = LogManager.GetLogger(typeof(PropertyService));
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        TWNewEgg.API.Service.ExceptionService exceptionService = new ExceptionService();
        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.sketchPropertyExamine>> PropertyServiceList(TWNewEgg.API.Models.ItemSketchSearchCondition itemSearch)
        {
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.sketchPropertyExamine>> result = new Models.ActionResponse<List<Models.sketchPropertyExamine>>();
            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            TWNewEgg.DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            if (this.CheckSellerExist(itemSearch.SellerID))
            {
                var itemQueryable = this.ItemSearchResult(itemSearch).AsQueryable();
                List<TWNewEgg.API.Models.sketchPropertyExamine> itemInfoList = new List<sketchPropertyExamine>();
                // 紀錄符合條件總筆數(實際回傳筆數只有PageSize筆)
                int totalCount = itemQueryable.Count();
                itemInfoList = itemQueryable.ToList();
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

                        string serverPath = HttpContext.Current.Server.MapPath("~//pic/pic/item/" + pidf4 + "/" + pidl4 + "_1_60.jpg");
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

                                        //// 賣場連結
                                        //itemInfo.Item.ItemURL = webSite + "/item/itemdetail?item_id=" + itemInfo.Item.ItemID;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error("預覽 Msg: " + ex.Message + ", Stacktrace: " + ex.StackTrace);
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
                        itemInfo.ItemDisplayPrice.GrossMargin = decimal.Round(((itemInfo.Item.PriceCash.Value - (itemInfo.Product.Cost.Value * itemInfo.CurrencyAverageExchange)) / itemInfo.Item.PriceCash.Value) * 100m, 0);
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
                            itemInfo.ItemCategory.SubCategoryID_1_Layer2_Name = db.Category.Where(x => x.ID == itemInfo.ItemCategory.SubCategoryID_1_Layer2).Select(x => x.Description).FirstOrDefault();
                            itemInfo.ItemCategory.SubCategoryID_1_Layer1 = dbFront.Category.Where(x => x.ID == itemInfo.ItemCategory.SubCategoryID_1_Layer2).Select(x => x.ParentID).FirstOrDefault();
                            itemInfo.ItemCategory.SubCategoryID_1_Layer1_Name = db.Category.Where(x => x.ID == itemInfo.ItemCategory.SubCategoryID_1_Layer1).Select(x => x.Description).FirstOrDefault();

                            if (categoryIDCell.Count == 2)
                            {
                                itemInfo.ItemCategory.SubCategoryID_2_Layer2 = categoryIDCell[1];
                                itemInfo.ItemCategory.SubCategoryID_2_Layer2_Name = db.Category.Where(x => x.ID == itemInfo.ItemCategory.SubCategoryID_2_Layer2).Select(x => x.Description).FirstOrDefault();
                                itemInfo.ItemCategory.SubCategoryID_2_Layer1 = dbFront.Category.Where(x => x.ID == itemInfo.ItemCategory.SubCategoryID_2_Layer2).Select(x => x.ParentID).FirstOrDefault();
                                itemInfo.ItemCategory.SubCategoryID_2_Layer1_Name = db.Category.Where(x => x.ID == itemInfo.ItemCategory.SubCategoryID_2_Layer1).Select(x => x.Description).FirstOrDefault();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Msg = "發生 exception，取得跨分類資訊失敗。";
                        logger.Info("取得跨分類資訊失敗(exception)：(ItemSketchID = " + itemInfo.Item.ID + ") " + ex.ToString());
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
        public IQueryable<TWNewEgg.API.Models.sketchPropertyExamine> ItemSearchResult(TWNewEgg.API.Models.ItemSketchSearchCondition itemSearch)
        {
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            IQueryable<TWNewEgg.API.Models.sketchPropertyExamine> itemList = this.QueryItemList(itemSearch.SellerID).AsQueryable();
            //var itemList = this.QueryItemList(itemSearch.SellerID).AsQueryable();
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
                case ItemSketchKeyWordSearchTarget.GroupId:
                    if (string.IsNullOrEmpty(itemSearch.KeyWord) == false)
                    {
                        int groupid = Convert.ToInt32(itemSearch.KeyWord);
                        itemList = itemList.Where(x => x.group_id == groupid).AsQueryable();
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

            // 商品加價購
            if (itemSearch.ShowOrder.HasValue)
            {
                itemList = itemList.Where(x => x.Item.ShowOrder == itemSearch.ShowOrder).AsQueryable();
            }

            // 商品審核狀態
            if (itemSearch.Status.HasValue)
            {
                itemList = itemList.Where(x => x.Item.status == itemSearch.Status).AsQueryable();
            }
            // 製造商
            if (itemSearch.ManufactureID > 0)
            {
                //12198
                itemList = itemList.Where(x => x.Product.ManufactureID == itemSearch.ManufactureID).AsQueryable();
                //itemList = itemList.Where(x => x.Product.ManufactureID == 12198).AsQueryable();
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
            return itemList;
        }
        public IQueryable<TWNewEgg.API.Models.sketchPropertyExamine> QueryItemList(int sellerID)
        {
            TWNewEgg.DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            var itemList = (from productTemp in db.ProductTemp
                            join itemTemp in db.ItemTemp on productTemp.ID equals itemTemp.ProduttempID
                            join itemStockTemp in db.ItemStocktemp on productTemp.ID equals itemStockTemp.producttempID
                            join seller in db.Seller on itemTemp.SellerID equals seller.ID
                            join currency in db.Currency on seller.CountryID equals currency.CountryID into currency_Temp
                            from currency in currency_Temp.DefaultIfEmpty()
                            join manufacture in db.Manufacture on itemTemp.ManufactureID equals manufacture.ID into ManufactureID_Temp
                            from manufacture in ManufactureID_Temp.DefaultIfEmpty()
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
                            join igdp in db.ItemGroupDetailProperty on itemTemp.ID equals igdp.ItemTempID
                            join itemsketchid in db.ItemSketch on igdp.GroupID equals itemsketchid.ItemTempGroupID into tmp
                            from ds in tmp.DefaultIfEmpty()
                            select new TWNewEgg.API.Models.sketchPropertyExamine
                            {
                                ID = ds.ItemTempGroupID.HasValue ? ds.ID : 0,
                                //ID = itskid.ID,
                                Status = 0,
                                CountryID = seller.CountryID ?? 1,
                                CurrencyAverageExchange = 1,
                                group_id = igdp.GroupID,
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
                                    // 賣場可售數量
                                    CanSaleLimitQty = (itemTemp.Qty - itemTemp.QtyReg) <= 0 ? 0 : (itemTemp.Qty - itemTemp.QtyReg),
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
                                    //Description = "",
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
                                    GrossMargin = (itemTemp.PriceCash > 0 && productTemp.Cost != null) ? Decimal.Round((itemTemp.PriceCash - productTemp.Cost.Value) / itemTemp.PriceCash * 100m, 0) : 0
                                },
                                CreateAndUpdate = new CreateAndUpdateIfno
                                {
                                    CreateDate = itemTemp.CreateDate,
                                    UpdateDate = itemTemp.UpdateDate.HasValue ? itemTemp.UpdateDate.Value : itemTemp.CreateDate
                                }
                            }).Where(x => x.Item.SellerID == sellerID && x.Item.ItemStatus != 99).Distinct().AsQueryable();
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
                    logger.Info("查詢 Category 失敗 (exctption)：" + ex.ToString());
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = "Parent ID 錯誤。";
                logger.Info("查詢 Category 失敗：Parent ID 錯誤。");
            }

            if (result.Body.Count() == 0)
            {
                result.IsSuccess = false;
                result.Msg = "查無資料。";
                logger.Info("查詢 Category 失敗：查無資料。");
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
        private bool CheckSellerExist(int sellerID)
        {
            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            bool sellerExist = spdb.Seller_BasicInfo.Where(x => x.SellerID == sellerID).Any();
            return sellerExist;
        }


        #region 待審 detail 修改 可以修改 狀態未通過的商品
        public ActionResponse<string> propertyDetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
            int itemtempid = itemSketch.Item.ID;
            //從 ItemGroupDetailProperty 利用 ItemTempID 把對應的 groupid 取出來
            var Group_groupid = dbFront.ItemGroupDetailProperty.Where(p => p.ItemTempID == itemtempid).Select(p => p.GroupID).FirstOrDefault();
            if (Group_groupid == null || Group_groupid == 0)
            {
                result.IsSuccess = false;
                result.Msg = "修改資料群組編號錯誤";
                return result;
            }
            //移除沒有 itemtempid 即顏色
            var itemTempGroup = dbFront.ItemGroupDetailProperty.Where(p => p.GroupID == Group_groupid && p.GroupValueID != p.ValueID && p.ItemTempID != null).Select(p => p.ItemTempID).ToList();
            //利用剛剛 query 出來的 itemtempid 把在 ItemTemp 對應的資料拉回來
            var itemTempList = dbFront.ItemTemp.Where(p => itemTempGroup.Contains(p.ID)).ToList();
            //暫存從 itemTempList 找回對應的 productTempId
            List<int> productTempIdInt = itemTempList.Select(p => p.ProduttempID.GetValueOrDefault()).ToList();
            var productTempList = dbFront.ProductTemp.Where(p => productTempIdInt.Contains(p.ID)).ToList();
            List<TWNewEgg.API.Models.ItemTempJoinProductTemp> itemTempJoinProductTemp = (from i in itemTempList
                                                                                         join p in productTempList
                                                                                         on i.ProduttempID equals p.ID
                                                                                         where i.ItemStatus != 99
                                                                                         select new TWNewEgg.API.Models.ItemTempJoinProductTemp
                                                                                         {
                                                                                             itemtemp_id = i.ID,
                                                                                             item_id = i.ItemID,
                                                                                             productTemp_id = p.ID,
                                                                                             product_id = p.ProductID
                                                                                         }).ToList();
            // Transaction Mark，加入會導致 資料庫 lock
            //using (TransactionScope scope = new TransactionScope())
            //{
                //開始對整個群組下的資料進行修改
                var editResult = this.propertyDetailEditView(itemSketch, itemTempJoinProductTemp);
                //修改成功
                if (editResult.IsSuccess == true)
                {
                    //scope.Complete();
                    //有圖片才做處理
                    if (itemSketch.Product.PicPatch_Edit.Count > 0)
                    {
                        //正式的 Item、Product圖片，要傳入 Item ID， Product ID
                        ImageService imgsService = new ImageService();
                        //有 itemid productid 才未正式賣場的圖做處理
                        if (itemSketch.Item.ItemID.HasValue == true && itemSketch.Product.ProductID.HasValue == true)
                        {
                            ActionResponse<bool> savePicResult_Item = imgsService.ImageProcessItemAndProduct(itemSketch.Product.PicPatch_Edit, "pic\\item", "pic\\pic\\item", itemSketch.Item.ItemID.Value, itemSketch.Product.ProductID.Value, "pic\\pic\\product", true);
                            if (savePicResult_Item.IsSuccess == false)
                            {
                                logger.Info("儲存正式賣場, 正式商品圖片成功。");
                            }
                            else
                            {
                                logger.Info("儲存正式賣場, 正式商品圖片失敗。");
                            }
                            ActionResponse<bool> savePicResult_ItemTemp = imgsService.ImageProcess(itemSketch.Product.PicPatch_Edit, "pic\\itemtemp", "pic\\pic\\itemtemp", itemSketch.Item.ID);
                            if (savePicResult_ItemTemp.IsSuccess)
                            {
                                logger.Info("儲存 Temp 賣場圖片成功。");
                            }
                            else
                            {
                                logger.Info("儲存 Temp 賣場圖片失敗。");
                            }
                            //    ActionResponse<bool> savePicResult_Item = imgsService.ImageProcess(itemSketch.Product.PicPatch_Edit, "pic\\item", "pic\\pic\\item", itemSketch.Item.ItemID.Value, true);
                            //    if (savePicResult_Item.IsSuccess)
                            //    {
                            //        logger.Info("儲存正式賣場圖片成功。");
                            //    }
                            //    else
                            //    {
                            //        logger.Info("儲存正式賣場圖片失敗。");
                            //    }
                            //    ActionResponse<bool> savePicResult_Product = imgsService.ImageProcess(itemSketch.Product.PicPatch_Edit, "pic\\product", "pic\\pic\\product", itemSketch.Product.ProductID.Value, true);
                            //    if (savePicResult_Product.IsSuccess)
                            //    {
                            //        logger.Info("儲存正式商品圖片成功。");
                            //    }
                            //    else
                            //    {
                            //        logger.Info("儲存正式商品圖片失敗。");
                            //    }
                        }
                        else
                        {
                            //沒有 itemid productid 只對 temp 賣場圖作處理
                            ActionResponse<bool> savePicResult_ItemTemp = imgsService.ImageProcess(itemSketch.Product.PicPatch_Edit, "pic\\itemtemp", "pic\\pic\\itemtemp", itemSketch.Item.ID);
                            if (savePicResult_ItemTemp.IsSuccess)
                            {
                                logger.Info("儲存 Temp 賣場圖片成功。");
                            }
                            else
                            {
                                logger.Info("儲存 Temp 賣場圖片失敗。");
                            }
                        }
                    }
                    else
                    {
                        //沒有圖片，所以不做任何處理
                        logger.Info("itemtemp_id: " + itemSketch.Item.ID + ": detail 修改沒有任何片傳入");
                    }
                    result.IsSuccess = true;
                    result.Msg = editResult.Msg;
                    return result;
                }
                else
                {
                    //修改失敗
                    //scope.Dispose();
                    result.IsSuccess = false;
                    result.Msg = editResult.Msg;
                    return result;
                }
            //}

        }
        public ActionResponse<string> propertyDetailEditView(TWNewEgg.API.Models.ItemSketch itemSketch, List<TWNewEgg.API.Models.ItemTempJoinProductTemp> _itemTempJoinProductTemp)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            #region 初始化回傳的 MODEL
            result.Body = string.Empty;
            result.IsSuccess = true;
            result.Msg = string.Empty;
            #endregion 
            ActionResponse<string> getAccountType = GetAccountType(itemSketch.Item.SellerID);
            ActionResponse<int> getDelvType = new ActionResponse<int>();
            // 取得 Account Type 成功，才取得 DelvType
            if (getAccountType.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = getAccountType.Msg;
                return result;
            }
            // 當遞送方式為供應商時，才將值更新成 Account Type
            if (itemSketch.Item.ShipType == "S")
            {
                itemSketch.Item.ShipType = getAccountType.Body;
            }
            getDelvType = GetDelvType(getAccountType.Body, itemSketch.Item.ShipType);
            if (getDelvType.IsSuccess == true)
            {
                // 更新時間
                DateTime updateTime = DateTime.Now;
                #region PicStart、PicEnd
                if (itemSketch.Product.PicPatch_Edit != null)
                {
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
                }
                else
                {
                    itemSketch.Product.PicStart = 0;
                    itemSketch.Product.PicEnd = 0;
                }
                #endregion PicStart、PicEnd
                // 更新正式表
                ActionResponse<string> updateOfficial_DetailEdit = UpdateOfficial_DetailEdit(itemSketch, getDelvType.Body, updateTime, _itemTempJoinProductTemp);
                if (updateOfficial_DetailEdit.IsSuccess == true)
                {
                    //正式表更新成功之後就接著更新 TEMP 表
                    ActionResponse<string> updateTemp_DetailEdit = UpdateTemp_DetailEdit(itemSketch, getDelvType.Body, updateTime, _itemTempJoinProductTemp);
                    if (updateTemp_DetailEdit.IsSuccess == true)
                    {
                        result.IsSuccess = true;
                        result.Msg = updateTemp_DetailEdit.Msg;
                        return result;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(updateTemp_DetailEdit.Msg) == true)
                        {
                            result.Msg = "資料修改錯誤";
                            result.IsSuccess = false;
                            return result;
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Msg = updateTemp_DetailEdit.Msg;
                            return result;
                        }
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = updateOfficial_DetailEdit.Msg;
                    return result; 
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = getDelvType.Msg;
                return result;
            }
        }
        #endregion


        #region 20150814 待審區修改
        public ActionResponse<string> PropertyOpenViewEdit(TWNewEgg.API.Models.ItemSketch itemSketch, List<TWNewEgg.API.Models.ItemTempJoinProductTemp> _itemTempJoinProductTemp)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            result.Body = string.Empty;
            result.IsSuccess = true;
            result.Msg = string.Empty;

            ActionResponse<string> getAccountType = GetAccountType(itemSketch.Item.SellerID);
            ActionResponse<int> getDelvType = new ActionResponse<int>();
            // 取得 Account Type 成功，才取得 DelvType
            if (getAccountType.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = getAccountType.Msg;
                return result;
            }
            // 當遞送方式為供應商時，才將值更新成 Account Type
            if (itemSketch.Item.ShipType == "S")
            {
                itemSketch.Item.ShipType = getAccountType.Body;
            }
            getDelvType = GetDelvType(getAccountType.Body, itemSketch.Item.ShipType);
            if (getDelvType.IsSuccess == true)
            {
                // 更新時間
                DateTime updateTime = DateTime.Now;

                #region PicStart、PicEnd
                if (itemSketch.Product.PicPatch_Edit != null)
                {
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
                    ActionResponse<string> updateOfficial_DetailEdit = UpdateOfficial_DetailEdit(itemSketch, getDelvType.Body, updateTime, _itemTempJoinProductTemp);
                    if (updateOfficial_DetailEdit.IsSuccess == true)
                    {
                        //正式表更新成功之後就接著更新 TEMP 表
                        ActionResponse<string> updateTemp_DetailEdit = UpdateTemp_DetailEdit(itemSketch, getDelvType.Body, updateTime, _itemTempJoinProductTemp);
                        if (updateTemp_DetailEdit.IsSuccess == false)
                        {
                            result.IsSuccess = false;
                            if (string.IsNullOrEmpty(updateTemp_DetailEdit.Msg) == true)
                            {
                                result.Msg = "資料修改錯誤";
                            }
                            else
                            {
                                result.Msg = updateTemp_DetailEdit.Msg;
                            }
                        }
                        else
                        {
                            result.IsSuccess = true;
                            result.Msg = updateTemp_DetailEdit.Msg;
                        }
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Msg = "更新資料失敗";
                    }
                    if (result.IsSuccess == true)
                    {
                        scope.Complete();
                        result.Msg = "儲存成功。";
                        // 若有傳入圖片，才儲存圖片
                        if (itemSketch.Product.PicPatch_Edit.Count > 0)
                        {
                            ImageService imgsService = new ImageService();
                            //正式的 Item、Product圖片，要傳入 Item ID， Product ID
                            ActionResponse<bool> savePicResult_Item = imgsService.ImageProcessItemAndProduct(itemSketch.Product.PicPatch_Edit, "pic\\item", "pic\\pic\\item", itemSketch.Item.ItemID.Value, itemSketch.Product.ProductID.Value, "pic\\pic\\product", true);
                            if (savePicResult_Item.IsSuccess == false)
                            {
                                logger.Info("儲存正式賣場, 正式商品圖片成功。");
                            }
                            else
                            {
                                logger.Info("儲存正式賣場, 正式商品圖片失敗。");
                            }
                            ActionResponse<bool> savePicResult_ItemTemp = imgsService.ImageProcess(itemSketch.Product.PicPatch_Edit, "pic\\itemtemp", "pic\\pic\\itemtemp", itemSketch.Item.ID);
                            if (savePicResult_ItemTemp.IsSuccess)
                            {
                                logger.Info("儲存 Temp 賣場圖片成功。");
                            }
                            else
                            {
                                logger.Info("儲存 Temp 賣場圖片失敗。");
                            }
                            //ActionResponse<bool> savePicResult_Item = imgsService.ImageProcess(itemSketch.Product.PicPatch_Edit, "pic\\item", "pic\\pic\\item", itemSketch.Item.ItemID.Value, true);
                            //if (savePicResult_Item.IsSuccess)
                            //{
                            //    logger.Info("儲存正式賣場圖片成功。");
                            //}
                            //else
                            //{
                            //    logger.Info("儲存正式賣場圖片失敗。");
                            //}

                            //ActionResponse<bool> savePicResult_Product = imgsService.ImageProcess(itemSketch.Product.PicPatch_Edit, "pic\\product", "pic\\pic\\product", itemSketch.Product.ProductID.Value, true);
                            //if (savePicResult_Product.IsSuccess)
                            //{
                            //    logger.Info("儲存正式商品圖片成功。");
                            //}
                            //else
                            //{
                            //    logger.Info("儲存正式商品圖片失敗。");
                            //}
                        }
                    }
                    else
                    {
                        scope.Dispose();
                    }

                }
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = getDelvType.Msg;
            }
            return result;
        }
        #endregion 20150814
        #region Temp 表
        public ActionResponse<string> UpdateTemp_DetailEdit(Models.ItemSketch itemSketch, int delvType, DateTime updateDate, List<TWNewEgg.API.Models.ItemTempJoinProductTemp> _itemTempJoinProductTemp)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            result.IsSuccess = true;
            #region Temp 待審狀態變更
            // Temp 待審狀態變更
            ActionResponse<string> updateTempStatus_DetailEdit = UpdateTempStatus_DetailEdit(itemSketch, updateDate, _itemTempJoinProductTemp);
            if (updateTempStatus_DetailEdit.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg += updateTempStatus_DetailEdit.Msg;
                return result;
            }
            #endregion
            #region 更新商品Temp
            ActionResponse<string> updateProductTemp_DetailEdit = UpdateProductTemp_DetailEdit(itemSketch, delvType, updateDate, _itemTempJoinProductTemp);
            if (updateProductTemp_DetailEdit.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg += updateProductTemp_DetailEdit.Msg;
                return result;
            }
            #endregion
            #region 更新賣場Temp
            // 更新賣場Temp
            ActionResponse<string> updateItemTemp_DetailEdit = UpdateItemTemp_DetailEdit(itemSketch, delvType, updateDate, _itemTempJoinProductTemp);
            if (updateItemTemp_DetailEdit.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg += updateItemTemp_DetailEdit.Msg;
                return result;
            }
            #endregion
            #region 更新庫存Temp
            // 更新庫存Temp
            ActionResponse<string> updateItemStockTemp_DetailEdit = UpdateItemStockTemp_DetailEdit(itemSketch, updateDate, _itemTempJoinProductTemp);
            if (updateItemStockTemp_DetailEdit.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg += updateItemStockTemp_DetailEdit.Msg;
                return result;
            }
            #endregion
            #region 更新跨分類Temp
            //更新跨分類Temp
            ActionResponse<bool> updateItemCategoryTemp_DetailEdit = UpdateItemCategoryTemp_DetailEdit(itemSketch, updateDate, _itemTempJoinProductTemp);
            if (updateItemCategoryTemp_DetailEdit.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg += updateItemCategoryTemp_DetailEdit.Msg;
                return result;
            }
            #endregion
            #region 更新商品屬性Temp
            ActionResponse<string> updateProductProteryTemp_DetailEdit = UpdateCategoryProteryTemp_DetailEdit(itemSketch, _itemTempJoinProductTemp);
            if (updateProductProteryTemp_DetailEdit.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg += updateItemTemp_DetailEdit.Msg;
                return result;
            }
            #endregion
            return result;
        }

        public ActionResponse<string> UpdateCategoryProteryTemp_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, List<TWNewEgg.API.Models.ItemTempJoinProductTemp> _itemTempJoinProductTemp)
        {

            ActionResponse<string> result = new ActionResponse<string>();
            List<int> productTempid = _itemTempJoinProductTemp.Where(p => p.productTemp_id != 0 && p.productTemp_id != null).Select(p => p.productTemp_id).ToList();
            if (productTempid.Count == 0)
            {
                result.IsSuccess = false;
                result.Msg = "修改資料錯誤";
                logger.Info("更新商品規格品待審區屬性: 沒有對應的 producttemp_id");
                return result;
            }
            TWNewEgg.DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
            int? GroupID = dbFront.ItemGroupDetailProperty.Where(x => x.ItemTempID == itemSketch.Item.ID).Select(x => x.GroupID).FirstOrDefault();
            var itemList = (from ss in dbFront.ItemGroupDetailProperty
                            where ss.GroupID == GroupID
                            select new { ItemID = ss.ItemID, ItemTempID = ss.ItemTempID, MasterPropertyID = ss.MasterPropertyID, PropertyID = ss.PropertyID, GroupValueID = ss.GroupValueID, ValueId = ss.ValueID, InputValue = ss.InputValue, ValueName = ss.ValueName }).ToList();
            // 使用 TransactionScope 保持資料交易完整性
            using (TransactionScope scope = new TransactionScope())
            {
                foreach (var proTempid in productTempid)
                {
                    API.Service.ProductPorpertyTempService productPorpertyTempService = new API.Service.ProductPorpertyTempService();
                    AutoMapper.Mapper.CreateMap<TWNewEgg.API.Models.SaveProductProperty, TWNewEgg.BackendService.Models.SaveProductProperty>();
                    List<SaveProductProperty> saveProductProperty = AutoMapper.Mapper.Map<List<SaveProductProperty>>(itemSketch.SaveProductPropertyList);
                    List<SaveProductProperty> saveProperty = new List<SaveProductProperty>();
                    int? ItemTempID = dbFront.ItemTemp.Where(x => x.ProduttempID == proTempid).Select(x => x.ID).FirstOrDefault();
                    var Group_Product_Data = itemList.Where(x => x.ItemTempID == ItemTempID).FirstOrDefault();
                    saveProperty = saveProductProperty;
                    for (int i = 0; i < saveProperty.Count(); i++)
                    {
                        if (saveProperty[i].PropertyID == Group_Product_Data.MasterPropertyID)
                        {
                            saveProperty[i].ValueID = Group_Product_Data.GroupValueID;
                        }
                        else if (saveProperty[i].PropertyID == Group_Product_Data.PropertyID)
                        {
                            saveProperty[i].ValueID = Group_Product_Data.ValueId;
                        }
                    }
                    ActionResponse<string> saveProductPropertyClick = productPorpertyTempService.SaveProductPropertyTempClick(saveProperty, proTempid, itemSketch.CreateAndUpdate.UpdateUser);
                    if (saveProductPropertyClick.IsSuccess == false)
                    {
                        scope.Dispose();
                        result.IsSuccess = false;
                        logger.Info(string.Format("Temp 規格品商品待審區屬性修改資料失敗; ProductID = {0}.", itemSketch.Product.ID));
                        break;
                    }
                    result.IsSuccess = true;
                }
                if (result.IsSuccess == true)
                {
                    scope.Complete();
                    result.Msg = "Temp 規格品商品待審區屬性修改資料成功。";
                }
            }
            return result;
        }
        #region 更新跨分類Temp
        public ActionResponse<bool> UpdateItemCategoryTemp_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, DateTime updateDate, List<TWNewEgg.API.Models.ItemTempJoinProductTemp> _itemTempJoinProductTemp)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;
            List<int> itemTempid_list = new List<int>();
            itemTempid_list = _itemTempJoinProductTemp.Where(p => p.itemtemp_id != 0 && p.itemtemp_id != null).Select(p => p.itemtemp_id).ToList();
            
            if (itemTempid_list.Count == 0)
            {
                result.IsSuccess = false;
                result.Msg = "修改資料錯誤";
                logger.Info("沒有對應的 itemTempid");
                return result;
            }
            var json_itemTemp_id = Newtonsoft.Json.JsonConvert.SerializeObject(itemTempid_list);
            logger.Info("更新跨分類Temp, json_itemTemp_id: " + json_itemTemp_id);
            // 刪除此賣場的跨分類資訊
            foreach (var itemtempid in itemTempid_list)
            {
                ActionResponse<bool> deleteItemCagegoryTemp = DeleteItemCagegoryTemp(itemtempid);
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
                            ActionResponse<DB.TWSQLDB.Models.ItemCategorytemp> makeItemCategory_DB = MakeItemCategoryTemp_DB(itemtempid, itemSketch.Item.ItemID, categoryID, itemSketch.CreateAndUpdate.UpdateUser.ToString(), updateDate);

                            if (makeItemCategory_DB.IsSuccess)
                            {
                                dbFront.ItemCategorytemp.Add(makeItemCategory_DB.Body);
                            }
                            else
                            {
                                // 組合跨分類 Model 失敗
                                result.IsSuccess = false;
                                break;
                            }
                        }
                        // 組合跨分類 Model 成功，才進行 SaveChanges
                        if (result.IsSuccess)
                        {
                            try
                            {
                                dbFront.SaveChanges();
                                result.IsSuccess = true;
                            }
                            catch (Exception ex)
                            {
                                result.IsSuccess = false;
                                logger.Info(string.Format("Temp 跨分類修改資料失敗(expection); ItemTempID = {0}; CategoryID_1 = {1}; CategoryID_2 = {2}; ErrorMessage = {3}; StackTrace = {4}.",
                                    itemSketch.Item.ID,
                                    itemSketch.ItemCategory.SubCategoryID_1_Layer2.HasValue ? itemSketch.ItemCategory.SubCategoryID_1_Layer2.Value.ToString() : "null",
                                    itemSketch.ItemCategory.SubCategoryID_2_Layer2.HasValue ? itemSketch.ItemCategory.SubCategoryID_2_Layer2.Value.ToString() : "null",
                                    GetExceptionMessage(ex),
                                    ex.StackTrace));
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    // 刪除跨分類失敗
                    result.IsSuccess = false;
                    break;
                }
            }
            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);
            return result;
        }
        #endregion
        private ActionResponse<DB.TWSQLDB.Models.ItemCategorytemp> MakeItemCategoryTemp_DB(int itemTempID,int? itemID, int categoryID, string createUser, DateTime createDate)
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
                logger.Info(string.Format("組合 ItemCategoryTemp 失敗(expection); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }
        #region 刪除此賣場的跨分類資訊(Temp)
        public ActionResponse<bool> DeleteItemCagegoryTemp(int itemTempID)
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
                    logger.Info(string.Format("刪除 Temp 跨分類失敗(expection); ItemTempID = {0}; ErrorMessage = {1}; StackTrace = {2}.", itemTempID, GetExceptionMessage(ex), ex.StackTrace));
                }

                // 記錄刪除訊息
                if (result.IsSuccess)
                {
                    foreach (DB.TWSQLDB.Models.ItemCategorytemp itemCategorytemp in deleteList)
                    {
                        logger.Info(string.Format("刪除 Temp 跨分類成功，刪除資訊：ItemTempID = {0}, CategoryID = {1}, CreateUser = {2}, CreateDate = {3}, UpdateUser = {4}, UpdateDate = {5}.",
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
                logger.Info(string.Format("查無 Temp 跨分類項目; ItemTempID = {0}.", itemTempID));
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }
        #endregion
        #region 更新庫存Temp
        public ActionResponse<string> UpdateItemStockTemp_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, DateTime updateDate, List<TWNewEgg.API.Models.ItemTempJoinProductTemp> _itemTempJoinProductTemp)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.DB.TWSqlDBContext dbfront = new DB.TWSqlDBContext();
            result.IsSuccess = true;
            result.Msg = string.Empty;
            List<int> productTempidList = new List<int>();
            productTempidList = _itemTempJoinProductTemp.Where(p => p.productTemp_id != 0 && p.productTemp_id != null).Select(p => p.productTemp_id).ToList();
            if (productTempidList.Count == 0)
            {
                result.IsSuccess = false;
                result.Msg = "修改資料錯誤";
                logger.Info("沒有對應的 productTempid");
                return result;
            }
            var json_product_id = Newtonsoft.Json.JsonConvert.SerializeObject(productTempidList);
            List<DB.TWSQLDB.Models.ItemStocktemp> _listStockTemp = new List<DB.TWSQLDB.Models.ItemStocktemp>();
            _listStockTemp = dbfront.ItemStocktemp.Where(p => productTempidList.Contains(p.producttempID)).ToList();
            logger.Info("更新庫存Temp, json_product_id: " + json_product_id);
            int selectProductTempid = _itemTempJoinProductTemp.Where(p=>p.productTemp_id ==itemSketch.Product.ID ).Select(p=>p.productTemp_id).FirstOrDefault();
            foreach (var productTemp_id in _listStockTemp)
            {
                ActionResponse<ItemStock_DetialEdit> makeItemStock_DetailEdit = new ActionResponse<ItemStock_DetialEdit>();
                if (productTemp_id.producttempID == selectProductTempid)
                {
                    makeItemStock_DetailEdit = MakeItemStock_DetailEdit(itemSketch, updateDate, true, "Temp", productTemp_id.producttempID);
                }
                else
                {
                    makeItemStock_DetailEdit = MakeItemStock_DetailEdit(itemSketch, updateDate, false, "Temp", productTemp_id.producttempID);
                }
                if (makeItemStock_DetailEdit.IsSuccess)
                {
                    AutoMapper.Mapper.CreateMap<ItemStock_DetialEdit, DB.TWSQLDB.Models.ItemStocktemp>();
                    AutoMapper.Mapper.Map(makeItemStock_DetailEdit.Body, productTemp_id);
                }
            }
            try
            {
                dbfront.SaveChanges();
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                logger.Info(string.Format("Temp 庫存修改資料失敗(expection); ProductTempID = {0}; ErrorMessage = {1}; StackTrace = {2}.", itemSketch.Product.ID, GetExceptionMessage(ex), ex.StackTrace));
                
            }
            return result;
        }
        #endregion
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
                logger.Info(string.Format("取得庫存失敗(expection); ProductTempID = {0}; ErrorMessage = {1}; StackTrace = {2}.", productTempID, GetExceptionMessage(ex), ex.StackTrace));
            }

            if (result.Body == null)
            {
                result.IsSuccess = false;
                result.Msg = "查無庫存資訊。(T)";
                logger.Info(string.Format("查無庫存; ProductTempID = {0}.", productTempID));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }
        #region 更新賣場Temp
        public ActionResponse<string> UpdateItemTemp_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, int delvType, DateTime updateDate, List<TWNewEgg.API.Models.ItemTempJoinProductTemp> _itemTempJoinProductTemp)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            result.IsSuccess = true;
            result.Msg = string.Empty;
            TWNewEgg.DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
            // 暫存毛利率，log 時使用
            decimal? grossMargin = null;
            List<int> itemtempidList = new List<int>();
            //檢查 itemtemp id 資料是否存在資料庫裡
            itemtempidList = _itemTempJoinProductTemp.Where(p => p.itemtemp_id != 0 && p.itemtemp_id != null).Select(p => p.itemtemp_id).ToList();
            if (itemtempidList.Count == 0)
            {
                result.IsSuccess = false;
                result.Msg = "修改資料錯誤";
                logger.Info("沒有 itemtempidList ");
                return result;
            }
            var json_itemtemp_id = Newtonsoft.Json.JsonConvert.SerializeObject(itemtempidList);
            logger.Info("更新賣場Temp json_itemtemp_id: " + json_itemtemp_id);
            //把對應的 itemtempid 的資料從 DB 裡面撈出來
            List<TWNewEgg.DB.TWSQLDB.Models.ItemTemp> listItemTemp = dbFront.ItemTemp.Where(p => itemtempidList.Contains(p.ID)).ToList();
            if (listItemTemp == null)
            {
                result.IsSuccess = false;
                result.Msg = "修改資料錯誤";
                logger.Info("沒有對應的  ItemTemp 資料");
            }
            foreach (TWNewEgg.DB.TWSQLDB.Models.ItemTemp itemtempSingal in listItemTemp)
            {
                ActionResponse<Item_DetialEdit> makeItem_DetailEdit = new ActionResponse<Item_DetialEdit>();
                //畫面上開啟 Detail 的資料
                if (itemtempSingal.ID == itemSketch.Item.ID)
                {
                    makeItem_DetailEdit = MakeItem_DetailEdit(itemSketch, delvType, updateDate, null, itemtempSingal, "Temp", false);
                }
                else
                {
                    //同步要修改的資料
                    makeItem_DetailEdit = MakeItem_DetailEdit(itemSketch, delvType, updateDate, null, itemtempSingal, "Temp", true);
                }

                if (makeItem_DetailEdit.IsSuccess == true)
                {
                    AutoMapper.Mapper.CreateMap<Item_DetialEdit, DB.TWSQLDB.Models.ItemTemp>()
                            .ForMember(x => x.ItemTempDesc, x => x.MapFrom(src => src.ItemDesc))
                            .ForMember(x => x.DelvData, x => x.MapFrom(src => src.DelvDate));
                    AutoMapper.Mapper.Map(makeItem_DetailEdit.Body, itemtempSingal);
                    dbFront.Entry(itemtempSingal).State = System.Data.EntityState.Modified;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = "資料錯誤, 修改失敗";
                }
            }
            if (result.IsSuccess == true)
            {
                dbFront.SaveChanges();
                result.IsSuccess = true;
                result.Msg = "修改成功";
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = "資料錯誤, 修改失敗";
            }
            return result;
        }
        #endregion
        #region 更新商品Temp
        public ActionResponse<string> UpdateProductTemp_DetailEdit(Models.ItemSketch itemSketch, int delvType, DateTime updateDate, List<TWNewEgg.API.Models.ItemTempJoinProductTemp> _itemTempJoinProductTemp)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            List<int> productTempList_id = new List<int>();
            TWNewEgg.DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();

            #region set result default value
            result.IsSuccess = true;
            result.Msg = "修改成功";
            #endregion
            //判斷是否有對應的 priductTempid 資料
            productTempList_id = _itemTempJoinProductTemp.Where(p => p.productTemp_id != 0 && p.productTemp_id != null).Select(p => p.productTemp_id).ToList();
            if (productTempList_id.Count == 0)
            {
                result.IsSuccess = false;
                result.Msg = "資料修改錯誤";
                logger.Info("沒有 productTemp id");
                return result;
            }

            var json_productTemp_id = Newtonsoft.Json.JsonConvert.SerializeObject(productTempList_id);
            logger.Info("更新商品Temp json_productTemp_id: " + json_productTemp_id);
            //利用 productTempList_id 抓取再 DB 對應的資料
            List<TWNewEgg.DB.TWSQLDB.Models.ProductTemp> listProductTemp = new List<DB.TWSQLDB.Models.ProductTemp>();
            listProductTemp = dbFront.ProductTemp.Where(p => productTempList_id.Contains(p.ID)).ToList();
            if (listProductTemp.Count == 0 || listProductTemp == null)
            {
                result.IsSuccess = false;
                result.Msg = "資料修改錯誤";
                logger.Info("沒有 productTemp 對應的資料");
                return result;
            }
            foreach (TWNewEgg.DB.TWSQLDB.Models.ProductTemp productTempSingle in listProductTemp)
            {
                ActionResponse<Product_DetialEdit> makeProduct_DetailEdit = new ActionResponse<Product_DetialEdit>();
                //主要開啟修改 detail 的資料
                if (productTempSingle.ID == itemSketch.Product.ID)
                {
                    makeProduct_DetailEdit = MakeProduct_DetailEdit(itemSketch, delvType, updateDate, null, productTempSingle, true, "Temp", itemSketch.Product.ID);
                }
                else
                {
                    //其他要同步修改資料
                    makeProduct_DetailEdit = MakeProduct_DetailEdit(itemSketch, delvType, updateDate, null, productTempSingle, false, "Temp", itemSketch.Product.ID);
                }
                if (makeProduct_DetailEdit.IsSuccess == true)
                {
                    AutoMapper.Mapper.CreateMap<Product_DetialEdit, DB.TWSQLDB.Models.ProductTemp>();
                    AutoMapper.Mapper.Map(makeProduct_DetailEdit.Body, productTempSingle);
                    dbFront.Entry(productTempSingle).State = System.Data.EntityState.Modified;
                    result.IsSuccess = true;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = "資料錯誤";
                    break;
                }

            }
            //判斷資料是否修改成功, 是再做 db.SaveChange()
            if (result.IsSuccess == true)
            {
                try
                {
                    dbFront.SaveChanges();
                    result.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    logger.Error("[Message] = " + ex.Message + " ;[StackTrace]: " + ex.StackTrace + " ;[InnerExceptionMessage]: " + exceptionService.InnerExceptionMessage(ex));
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = "修改資料失敗";

            }
            return result;
        }
        #endregion 更新商品Temp
        #region 待審狀態變更
        public ActionResponse<string> UpdateTempStatus_DetailEdit(Models.ItemSketch itemSketch, DateTime updateDate, List<TWNewEgg.API.Models.ItemTempJoinProductTemp> _itemTempJoinProductTemp)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            result.IsSuccess = true;
            result.Msg = string.Empty;
            foreach (var have_to_use_id in _itemTempJoinProductTemp)
            {
                if (have_to_use_id.itemtemp_id > 0 && have_to_use_id.productTemp_id > 0)
                {
                    ActionResponse<DB.TWSQLDB.Models.ItemTemp> getItemTemp = GetItemTemp(have_to_use_id.itemtemp_id);
                    ActionResponse<DB.TWSQLDB.Models.ProductTemp> getProductTemp = GetProductTemp(have_to_use_id.productTemp_id);
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
                                result.IsSuccess = true;
                            }
                            catch (Exception ex)
                            {
                                result.IsSuccess = false;
                                result.Msg = "資料處理錯誤";
                                logger.Info(string.Format("Temp 待審狀態變更失敗(expection); ItemTempID = {0}; ProductTempID = {1}; ErrorMessage = {2}; StackTrace = {3}.", have_to_use_id.itemtemp_id, have_to_use_id.productTemp_id, GetExceptionMessage(ex), ex.StackTrace));
                                break;
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
                        break;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = "修改資料錯誤";
                    logger.Info("沒有 itemtemp_id 或沒有 productTemp_id");
                    break;
                }
            }
            return result;
        }
        #endregion 待審狀態變更
        #endregion
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
                logger.Info(string.Format("取得賣場Temp失敗(expection); ItemTempID = {0}; ErrorMessage = {1}; StackTrace = {2}.", itemTempID, GetExceptionMessage(ex), ex.StackTrace));
            }

            if (result.Body == null)
            {
                result.IsSuccess = false;
                result.Msg = "查無賣場資訊。(T)";
                logger.Info(string.Format("查無賣場Temp; ItemTempID = {0}.", itemTempID));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }
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
                logger.Info(string.Format("取得商品Temp失敗(expection); ProductTempID = {0}; ErrorMessage = {1}; StackTrace = {2}.", productTempID, GetExceptionMessage(ex), ex.StackTrace));
            }

            if (result.Body == null)
            {
                result.IsSuccess = false;
                result.Msg = "查無商品資訊。(T)";
                logger.Info(string.Format("查無商品Temp; ProductTempID = {0}.", productTempID));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }
        #region 正式表
        public ActionResponse<string> UpdateOfficial_DetailEdit(Models.ItemSketch itemSketch, int delvType, DateTime updateDate, List<TWNewEgg.API.Models.ItemTempJoinProductTemp> _itemTempJoinProductTemp)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            result.IsSuccess = true;
            #region 更新商品
            // 更新商品
            ActionResponse<string> updateProduct = UpdateProduct_DetailEdit(itemSketch, delvType, updateDate, _itemTempJoinProductTemp);
            if (updateProduct.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = updateProduct.Msg;
                return result;
            }
            #endregion
            #region  更新賣場
            ActionResponse<string> updateItem = UpdateItem_DetailEdit(itemSketch, delvType, updateDate, _itemTempJoinProductTemp);
            if (updateItem.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = updateItem.Msg;
                return result;
            }
            #endregion
            #region 更新庫存
            // 更新庫存
            ActionResponse<string> updateItemStock = UpdateItemStock_DetailEdit(itemSketch, updateDate, _itemTempJoinProductTemp);
            if (updateItemStock.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = updateItemStock.Msg;
                return result;
            }
            #endregion
            #region 更新跨分類
            // 更新跨分類
            ActionResponse<string> updateItemCategory = UpdateItemCategory_DetailEdit(itemSketch, updateDate, _itemTempJoinProductTemp);
            if (updateItemCategory.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = updateItemCategory.Msg;
                return result;
            }
            #endregion
            #region 更新商品屬性
            //更新商品屬性
            ActionResponse<string> updateProductProtery = UpdateCategoryProtery_DetailEdit(itemSketch, _itemTempJoinProductTemp);
            if (updateProductProtery.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = updateItem.Msg;
                return result;
            }
            #endregion 更新商品屬性 

            return result;
        }

        #region 規格品更新正式區商品屬性
        public ActionResponse<string> UpdateCategoryProtery_DetailEdit(Models.ItemSketch itemSketch, List<TWNewEgg.API.Models.ItemTempJoinProductTemp> _itemTempJoinProductTemp)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.API.Service.CategoryPropertyService categoryPropertyService = new TWNewEgg.API.Service.CategoryPropertyService();
            List<int> productidList = new List<int>();
            List<SaveProductProperty> Propertyresult = new List<SaveProductProperty>();
            //把對應的 productid 取出來，如果是空的則不對正式資料做更新
            productidList = _itemTempJoinProductTemp.Where(p => p.product_id.GetValueOrDefault() != 0 && p.product_id.GetValueOrDefault() != null).Select(p => p.product_id.GetValueOrDefault()).ToList();
            if (productidList.Count != 0)
            {               
                TWNewEgg.DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
                int? GroupID = dbFront.ItemGroupDetailProperty.Where(x => x.ItemID == itemSketch.Item.ItemID).Select(x => x.GroupID).FirstOrDefault();

                var itemList = (from ss in dbFront.ItemGroupDetailProperty
                                where ss.GroupID == GroupID
                                select new { ItemID = ss.ItemID, MasterPropertyID = ss.MasterPropertyID, PropertyID = ss.PropertyID, GroupValueID = ss.GroupValueID, ValueId = ss.ValueID, InputValue = ss.InputValue, ValueName = ss.ValueName }).ToList();
                if (itemList.Count == 0)
                {
                    result.IsSuccess = false;
                    result.Msg = "修改資料錯誤";
                    logger.Info("更新規格品正式區商品屬性: 沒有對應的 product_id");
                    return result;
                }
                // 使用 TransactionScope 保持資料交易完整性
                using (TransactionScope scope = new TransactionScope())
                {
                    foreach (int product_id in productidList)
                    {
                        AutoMapper.Mapper.CreateMap<TWNewEgg.API.Models.SaveProductProperty, TWNewEgg.BackendService.Models.SaveProductProperty>();
                        List<SaveProductProperty> saveProductProperty = AutoMapper.Mapper.Map<List<SaveProductProperty>>(itemSketch.SaveProductPropertyList);
                        List<SaveProductProperty> saveProperty = new List<SaveProductProperty>();
                        int? item_ID = dbFront.ItemTemp.Where(x => x.ProductID == product_id).Select(x => x.ItemID).FirstOrDefault();
                        var Group_Product_Data = itemList.Where(x => x.ItemID == item_ID).FirstOrDefault();
                        saveProperty = saveProductProperty;
                        for (int i = 0; i < saveProperty.Count(); i++)
                        {
                            if (saveProperty[i].PropertyID == Group_Product_Data.MasterPropertyID)
                            {
                                saveProperty[i].ValueID = Group_Product_Data.GroupValueID;
                            }
                            else if (saveProperty[i].PropertyID == Group_Product_Data.PropertyID)
                            {
                                saveProperty[i].ValueID = Group_Product_Data.ValueId;
                            }
                            saveProperty[i].UpdateUser = itemSketch.CreateAndUpdate.UpdateUser.ToString();
                        }
                        //更新商品屬性
                        Models.ActionResponse<string> saveProductPropertyResult = categoryPropertyService.SaveProductPropertyClick(saveProperty, product_id);
                        if (saveProductPropertyResult.IsSuccess == false)
                        {
                            scope.Dispose();
                            result.IsSuccess = false;
                            logger.Info(string.Format("規格品正式商品屬性修改資料失敗。(ProductID = {0})", itemSketch.Item.ItemID.Value));
                            break;
                        }
                        result.IsSuccess = true;
                    }
                    if (result.IsSuccess == true)
                    {
                        scope.Complete();
                        result.Msg = "規格品正式商品屬性修改資料成功。";
                    }
                }
            }
            else
            {
                //沒有 product 的資料需要同步修改
                result.IsSuccess = true;
            }
            //Propertyresult = saveProperty;
            return result;
        }
        #endregion 更新商品屬性
        #region 更新商品屬性
        public ActionResponse<string> UpdateProductProtery_DetailEdit(Models.ItemSketch itemSketch, List<TWNewEgg.API.Models.ItemTempJoinProductTemp> _itemTempJoinProductTemp)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.BackendService.Service.CategoryPropertyService categoryPropertyService = new BackendService.Service.CategoryPropertyService();

            List<int> productidList = new List<int>();
            productidList = _itemTempJoinProductTemp.Where(p => p.product_id.GetValueOrDefault() != 0 && p.product_id.GetValueOrDefault() != null).Select(p => p.product_id.GetValueOrDefault()).ToList();
            if (productidList.Count == 0)
            {
                result.IsSuccess = false;
                result.Msg = "修改資料錯誤";
                logger.Info("更新商品屬性: 沒有對應的 product_id");
                return result;
            }
            foreach (int product_id in productidList)
            {
                AutoMapper.Mapper.CreateMap<TWNewEgg.API.Models.SaveProductProperty, TWNewEgg.BackendService.Models.SaveProductProperty>();
                List<TWNewEgg.BackendService.Models.SaveProductProperty> saveProductProperty = AutoMapper.Mapper.Map<List<TWNewEgg.BackendService.Models.SaveProductProperty>>(itemSketch.SaveProductPropertyList);
                TWNewEgg.BackendService.Models.PropertySaveModel saveProperty = new BackendService.Models.PropertySaveModel();
                saveProperty.CategoryID = itemSketch.ItemCategory.MainCategoryID_Layer2.Value;
                saveProperty.ProductID = product_id;
                //saveProperty.ProductID = itemSketch.Product.ProductID.Value;
                saveProperty.SaveProductProperty = saveProductProperty;
                // 更新商品屬性
                TWNewEgg.BackendService.Models.ActionResponse<string> saveProductPropertyResult = categoryPropertyService.SaveProductPropertyClick(saveProperty);
                if (saveProductPropertyResult.IsSuccess == false)
                {
                    result.IsSuccess = false;
                    logger.Info(string.Format("正式商品屬性修改資料失敗。(ProductID = {0})", itemSketch.Item.ItemID.Value));
                    break;
                }
                result.IsSuccess = true;
            }
            return result;
        }
        #endregion 更新商品屬性
        #region 更新跨分類
        public ActionResponse<string> UpdateItemCategory_DetailEdit(Models.ItemSketch itemSketch, DateTime updateDate, List<TWNewEgg.API.Models.ItemTempJoinProductTemp> _itemTempJoinProductTemp)
        {
            TWNewEgg.DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
            ActionResponse<string> result = new ActionResponse<string>();
            result.IsSuccess = true;
            result.Msg = string.Empty;
            
            List<int> itemidList = new List<int>();
            itemidList = _itemTempJoinProductTemp.Where(p => p.item_id.GetValueOrDefault() != null && p.item_id.GetValueOrDefault() != 0).Select(p => p.item_id.GetValueOrDefault()).ToList();
            if (itemidList.Count != 0)
            {
                foreach (int item_id in itemidList)
                {
                    // 刪除此賣場的跨分類資訊
                    ActionResponse<bool> deleteItemCagegory = DeleteItemCagegory(item_id);
                    if (deleteItemCagegory.IsSuccess == true)
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
                            foreach (int categoryID in categoryIDCell)
                            {
                                // 組合跨分類 Model
                                ActionResponse<DB.TWSQLDB.Models.ItemCategory> makeItemCategory_DB = MakeItemCategory_DB(item_id, categoryID, itemSketch.CreateAndUpdate.UpdateUser.ToString(), updateDate);
                                if (makeItemCategory_DB.IsSuccess)
                                {
                                    dbFront.ItemCategory.Add(makeItemCategory_DB.Body);
                                }
                                else
                                {
                                    // 組合跨分類 Model 失敗
                                    result.IsSuccess = false;
                                    break;
                                }
                            }
                            if (result.IsSuccess == true)
                            {
                                try
                                {
                                    dbFront.SaveChanges();
                                    result.IsSuccess = true;
                                }
                                catch (Exception ex)
                                {
                                    result.IsSuccess = false;
                                    logger.Info(string.Format("正式跨分類修改資料失敗(expection); ItemID = {0}; CategoryID_1 = {1}; CategoryID_2 = {2}; ErrorMessage = {3}; StackTrace = {4}.",
                                        itemSketch.Item.ItemID.Value,
                                        itemSketch.ItemCategory.SubCategoryID_1_Layer2.HasValue ? itemSketch.ItemCategory.SubCategoryID_1_Layer2.Value.ToString() : "null",
                                        itemSketch.ItemCategory.SubCategoryID_2_Layer2.HasValue ? itemSketch.ItemCategory.SubCategoryID_2_Layer2.Value.ToString() : "null",
                                        GetExceptionMessage(ex),
                                        ex.StackTrace));
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        // 刪除跨分類失敗
                        result.IsSuccess = false;
                        result.Msg = "修改資料處理錯誤";
                        logger.Info("更新跨分類: 刪除跨分類失敗");
                        break;
                    }
                }
            }
            else
            {
                result.IsSuccess = true;
            }
            return result;
        }
        #endregion 更新跨分類 
        public ActionResponse<DB.TWSQLDB.Models.ItemCategory> MakeItemCategory_DB(int itemID, int categoryID, string createUser, DateTime createDate)
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
            catch (Exception error)
            {
                result.IsSuccess = false;
                logger.Error("[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }
            return result;
        }
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
                    logger.Info(string.Format("刪除正式跨分類失敗(expection); ItemID = {0}; ErrorMessage = {1}; StackTrace = {2}.", itemID, GetExceptionMessage(ex), ex.StackTrace));
                }

                // 記錄刪除訊息
                if (result.IsSuccess)
                {
                    foreach (DB.TWSQLDB.Models.ItemCategory itemCategory in deleteList)
                    {
                        logger.Info(string.Format("刪除正式跨分類成功，刪除資訊：ItemID = {0}, CategoryID = {1}, CreateUser = {2}, CreateDate = {3}, UpdateUser = {4}, UpdateDate = {5}.",
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
                logger.Info(string.Format("查無正式跨分類項目; ItemID = {0}.", itemID));
            }

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }
        #region 更新庫存
        public ActionResponse<string> UpdateItemStock_DetailEdit(Models.ItemSketch itemSketch, DateTime updateDate, List<TWNewEgg.API.Models.ItemTempJoinProductTemp> _itemTempJoinProductTemp)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
            List<int> product_id_list = new List<int>();
            product_id_list = _itemTempJoinProductTemp.Where(p => p.product_id.GetValueOrDefault() != 0 && p.product_id.GetValueOrDefault() != null).Select(p => p.product_id.GetValueOrDefault()).ToList();
            if (product_id_list.Count != 0)
            {
                List<DB.TWSQLDB.Models.ItemStock> _listItemStock = new List<DB.TWSQLDB.Models.ItemStock>();
                _listItemStock = dbFront.ItemStock.Where(p => product_id_list.Contains(p.ProductID)).ToList();
                if (_listItemStock.Count == 0)
                {
                    var json_product_id = Newtonsoft.Json.JsonConvert.SerializeObject(product_id_list);
                    result.IsSuccess = false;
                    result.Msg = "修改資料錯誤";
                    logger.Info("更新庫存: 沒有 productid 對應下的 ItemStock 資料. json_product_id: " + json_product_id);
                    return result;
                }

                //把最主要從畫面上送的 itemtempid 抓出來
                int selectProductid = _itemTempJoinProductTemp.Where(p => p.itemtemp_id == itemSketch.Item.ID).Select(p => p.product_id.GetValueOrDefault()).FirstOrDefault();
                foreach (var item in _listItemStock)
                {
                    //從畫面上選取的 itemtempid 
                    if (item.ProductID == selectProductid)
                    {
                        ActionResponse<ItemStock_DetialEdit> makeItemTemp_DetailEditResult = MakeItemStock_DetailEdit(itemSketch, updateDate, true, "Front", item.ProductID);
                        if (makeItemTemp_DetailEditResult.IsSuccess)
                        {
                            AutoMapper.Mapper.CreateMap<ItemStock_DetialEdit, DB.TWSQLDB.Models.ItemStock>();
                            AutoMapper.Mapper.Map(makeItemTemp_DetailEditResult.Body, item);
                        }
                    }
                    else
                    {
                        //不同步可售數量
                        ActionResponse<ItemStock_DetialEdit> makeItemTemp_DetailEditResult = MakeItemStock_DetailEdit(itemSketch, updateDate, false, "Front", item.ProductID);
                        if (makeItemTemp_DetailEditResult.IsSuccess)
                        {
                            AutoMapper.Mapper.CreateMap<ItemStock_DetialEdit, DB.TWSQLDB.Models.ItemStock>();
                            AutoMapper.Mapper.Map(makeItemTemp_DetailEditResult.Body, item);
                        }
                    }
                }
                try
                {
                    dbFront.SaveChanges();
                    result.IsSuccess = true;
                }
                catch (Exception error)
                {
                    result.IsSuccess = false;
                    result.Msg = "修改資料錯誤";
                    logger.Error("[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
                }
            }
            else
            {
                result.IsSuccess = true;
            }
            return result;
        }
        #endregion 更新庫存
        private ActionResponse<ItemStock_DetialEdit> MakeItemStock_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, DateTime updateDate, bool Synchronize = false, string source = "", int productid = 0)
        {
            ActionResponse<ItemStock_DetialEdit> result = new ActionResponse<ItemStock_DetialEdit>();
            TWNewEgg.DB.TWSqlDBContext dbFron = new DB.TWSqlDBContext();
            result.Body = new ItemStock_DetialEdit();
            result.IsSuccess = true;
            result.Msg = string.Empty;
            if (productid == 0)
            {
                result.IsSuccess = false;
                result.Msg = "修改資料錯誤";
                logger.Info("productid = 0 沒有傳入 productid 所以無法更新修改資料");
                return result;
            }
            try
            {
                if (Synchronize == true)
                {
                    //更新主要選的itemtempid
                    //result.Body.Qty = (itemSketch.ItemStock.CanSaleQty + itemSketch.ItemStock.InventoryQtyReg).GetValueOrDefault();
                    //result.Body.SafeQty = itemSketch.ItemStock.InventorySafeQty.GetValueOrDefault();
                    //result.Body.UpdateDate = updateDate;
                    //result.Body.UpdateUser = itemSketch.CreateAndUpdate.UpdateUser.ToString();

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
                else
                {
                    int orginalQty = 0;
                    if (source == "Temp")
                    {
                        orginalQty = dbFron.ItemStocktemp.Where(p => p.producttempID == productid).Select(p => p.Qty).FirstOrDefault();
                    }
                    else
                    {
                        orginalQty = dbFron.ItemStock.Where(p => p.ProductID == productid).Select(p => p.Qty).FirstOrDefault();
                    }
                    
                    //更新選的 itemtempid 下同群組的資料，可售數量不同步
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
                    result.Body.Qty = orginalQty;
                }
                
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                logger.Info(string.Format("組合 ItemStock 詳細表 Model 失敗 (expection); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }
        #region 更新商品 UpdateProduct_DetailEdit
        public ActionResponse<string> UpdateProduct_DetailEdit(Models.ItemSketch itemSketch, int delvType, DateTime updateDate, List<TWNewEgg.API.Models.ItemTempJoinProductTemp> _itemTempJoinProductTemp)
        {
            TWNewEgg.DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
            ActionResponse<string> result = new ActionResponse<string>();
            List<DB.TWSQLDB.Models.Product> _listProduct = new List<DB.TWSQLDB.Models.Product>();
            List<int> list_int_product = new List<int>();

            //把對應的 productid 取出來，如果是空的則不對正式資料做更新
            list_int_product = _itemTempJoinProductTemp.Where(p => p.product_id.GetValueOrDefault() != 0 && p.product_id.GetValueOrDefault() != null).Select(p => p.product_id.GetValueOrDefault()).ToList();
            //有資料再作修改. 可能選到的商品還沒有 itemid, productid 所以可能不會去修改正是賣場的相關資料
            if (list_int_product.Count != 0)
            {
                // 讀取資料庫內要被更新的賣場資訊
                _listProduct = dbFront.Product.Where(p => list_int_product.Contains(p.ID)).ToList();
                
                //如果再資料庫沒有相關資料, 則錯誤
                if (_listProduct.Count == 0)
                {
                    var json_product_id = Newtonsoft.Json.JsonConvert.SerializeObject(list_int_product);
                    logger.Info("更新商品: productid 沒有對應的資料庫資料 productid: " + json_product_id);
                    result.IsSuccess = false;
                    result.Msg = "修改資料錯誤";
                    return result;
                }
                //開始對從 DB 撈出來對應的資料開始進行修改
                foreach (DB.TWSQLDB.Models.Product singleProduct in _listProduct)
                {
                    ActionResponse<Product_DetialEdit> makeProduct_DetailEdit = new ActionResponse<Product_DetialEdit>();
                    //主要 detail 修改的資料
                    if (singleProduct.ID == itemSketch.Product.ProductID)
                    {
                        makeProduct_DetailEdit = MakeProduct_DetailEdit(itemSketch, delvType, updateDate, singleProduct, null, true, "Front", itemSketch.Product.ProductID);
                    }
                    else
                    {
                        //其他群組要同步的相關資料
                        makeProduct_DetailEdit = MakeProduct_DetailEdit(itemSketch, delvType, updateDate, singleProduct, null, false, "Front", itemSketch.Product.ProductID);
                    }
                    if (makeProduct_DetailEdit.IsSuccess == true)
                    {
                        AutoMapper.Mapper.CreateMap<Product_DetialEdit, DB.TWSQLDB.Models.Product>();
                        AutoMapper.Mapper.Map(makeProduct_DetailEdit.Body, singleProduct);
                        dbFront.Entry(singleProduct).State = System.Data.EntityState.Modified;
                        result.IsSuccess = true;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Msg = "修改資料處理錯誤";
                    }
                }
                if (result.IsSuccess == true)
                {
                    try
                    {
                        dbFront.SaveChanges();
                        result.IsSuccess = true;
                    }
                    catch (Exception error)
                    {
                        result.IsSuccess = false;
                        result.Msg = "修改資料處理錯誤";
                        logger.Error("[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[InnerExceptionMessage]: " + exceptionService.InnerExceptionMessage(error));
                    }
                }
            }
            else
            {
                //沒有 product 的資料需要同步修改
                result.IsSuccess = true;
            }
            return result;
        }
        #endregion 更新商品 UpdateProduct_DetailEdit

        private ActionResponse<Product_DetialEdit> MakeProduct_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, int delvType, DateTime updateDate, TWNewEgg.DB.TWSQLDB.Models.Product productFrontDB = null, TWNewEgg.DB.TWSQLDB.Models.ProductTemp productTempFrontDB = null, bool Synchronize = false, string source = "", int? productId = 0)
        {
            ActionResponse<Product_DetialEdit> result = new ActionResponse<Product_DetialEdit>();
            #region set defaule value
            result.Body = new Product_DetialEdit();
            result.IsSuccess = true;
            result.Msg = string.Empty;
            #endregion

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

            try
            {
                //判斷要修改的資料是 Temp 還是正式區
                if (source == "Temp")
                {
                    #region Temp Area
                    //判斷是否有傳入 Model
                    if (productTempFrontDB != null)
                    {
                        #region 進行資料修改
                        //判斷資料是否是要同步修改
                        if (Synchronize == true)
                        {
                            #region 群組修改

                            AutoMapper.Mapper.Map(itemSketch, result.Body);
                            #endregion

                            result.Body.DelvType = delvType;
                        }
                        else
                        {
                            #region 不同步修改的欄位
                            //商家商品編號
                            string productSellerProductId = string.Empty;
                            //條碼
                            string productBarCode = string.Empty;
                            //商品型號
                            string productModel = string.Empty;
                            //UPC
                            string productUPC = string.Empty;
                            #endregion

                            #region 群組修改

                            AutoMapper.Mapper.Map(itemSketch, result.Body);

                            #endregion

                            #region 商家商品編號, 條碼, 商品型號, UPC 不同步修改 所以把原本的直放回去
                            result.Body.SellerProductID = productTempFrontDB.SellerProductID;
                            result.Body.BarCode = productTempFrontDB.BarCode;
                            result.Body.Model = productTempFrontDB.Model;
                            result.Body.UPC = productTempFrontDB.UPC;

                            result.Body.DelvType = delvType;
                            #endregion
                        }
                        #endregion
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Msg = "修改失敗, 資料錯誤";
                        logger.Error("productTempFrontDB 是空的 沒有傳入");
                    }
                    #endregion
                }
                else if (source == "Front")
                {
                    #region Front Area
                    //修改正式區
                    if (productFrontDB != null)
                    {
                        #region 進行資料修改
                        //修改所有選取的 detail 資料
                        if (Synchronize == true)
                        {
                            #region 群組修改
                            AutoMapper.Mapper.Map(itemSketch, result.Body);
                            #endregion

                            result.Body.DelvType = delvType;
                        }
                        else
                        {
                            #region 處理不同步修改欄位資料
                            //不同步修改商家商品編號, 條碼, 商品型號, UPC
                            //商家商品編號
                            string productSellerProductId = string.Empty;
                            //條碼
                            string productBarCode = string.Empty;
                            //商品型號
                            string productModel = string.Empty;
                            //UPC
                            string productUPC = string.Empty;
                            //把原本的值先暫存起來(不同步修改)
                            productSellerProductId = productFrontDB.SellerProductID;
                            productBarCode = productFrontDB.BarCode;
                            productModel = productFrontDB.Model;
                            productUPC = productFrontDB.UPC;
                            #endregion
                            #region 群組修改
                            AutoMapper.Mapper.Map(itemSketch, result.Body);
                            #endregion
                            #region 商家商品編號, 條碼, 商品型號, UPC 不同步修改 所以把原本的直放回去
                            result.Body.SellerProductID = productSellerProductId;
                            result.Body.BarCode = productBarCode;
                            result.Body.Model = productModel;
                            result.Body.UPC = productUPC;

                            result.Body.DelvType = delvType;
                            #endregion
                        }
                        #endregion
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Msg = "修改失敗, 資料錯誤";
                        logger.Error("productFrontDB 是空的 沒有傳入");
                    }
                    #endregion
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = "資料錯誤, 修改失敗";
                }
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "資料錯誤, 修改失敗";
                logger.Error("[Message]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[InnerExceptionMessage]: " + exceptionService.InnerExceptionMessage(error));
            }
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }
        #region 更新賣場 UpdateItem_DetailEdit
        public ActionResponse<string> UpdateItem_DetailEdit(Models.ItemSketch itemSketch, int delvType, DateTime updateDate, List<TWNewEgg.API.Models.ItemTempJoinProductTemp> _itemTempJoinProductTemp)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
            List<int> itemid = new List<int>();
            itemid = _itemTempJoinProductTemp.Where(p => p.item_id.GetValueOrDefault() != 0 && p.item_id.GetValueOrDefault() != null).Select(p => p.item_id.GetValueOrDefault()).ToList();
            if (itemid.Count != 0)
            {
                List<DB.TWSQLDB.Models.Item> _list_getItem = new List<DB.TWSQLDB.Models.Item>();
                // 讀取資料庫內要被更新的庫存資訊
                _list_getItem = dbFront.Item.Where(p => itemid.Contains(p.ID)).ToList();
                //DB 裡面沒有對應的 itemid 資料
                if (_list_getItem == null || _list_getItem.Count == 0)
                {
                    var json_item_id = Newtonsoft.Json.JsonConvert.SerializeObject(itemid);
                    result.IsSuccess = false;
                    result.Msg = "找不到對應的資料";
                    logger.Info("更新賣場: 找不到對應的資料. json_item_id: " + json_item_id);
                    return result;
                }
                foreach (DB.TWSQLDB.Models.Item itemSingle in _list_getItem)
                {
                    ActionResponse<Item_DetialEdit> makeItem_DetailEdit = new ActionResponse<Item_DetialEdit>();
                    //開啟修改 Detail 的資料
                    if (itemSingle.ID == itemSketch.Item.ItemID)
                    {
                        makeItem_DetailEdit = MakeItem_DetailEdit(itemSketch, delvType, updateDate, itemSingle, null, "Front", false);
                    }
                    else
                    {
                        //同步修改的資料
                        makeItem_DetailEdit = MakeItem_DetailEdit(itemSketch, delvType, updateDate, itemSingle, null, "Front", true);
                    }
                    if (makeItem_DetailEdit.IsSuccess == true)
                    {
                        AutoMapper.Mapper.CreateMap<Item_DetialEdit, DB.TWSQLDB.Models.Item>();
                        AutoMapper.Mapper.Map(makeItem_DetailEdit.Body, itemSingle);
                        dbFront.Entry(itemSingle).State = System.Data.EntityState.Modified;
                        result.IsSuccess = true;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Msg = "資料錯誤, 修改失敗";
                        break;
                    }
                }
                //資料修改成功
                if (result.IsSuccess == true)
                {
                    try
                    {
                        dbFront.SaveChanges();
                        result.IsSuccess = true;
                    }
                    catch (Exception error)
                    {
                        result.IsSuccess = false;
                        result.Msg = "修改失敗";
                        logger.Error("[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[InnerExceptionMessage]: " + exceptionService.InnerExceptionMessage(error));
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = "修改失敗";
                }
                return result;
            }
            else
            {
                //可能畫面點選修改的資料還沒有 productid itemid 但是要對其他同群組修改, 所以直接回傳 true
                result.IsSuccess = true;
                return result;
            }
        }
        #endregion 更新賣場 UpdateItem_DetailEdit
        private ActionResponse<Item_DetialEdit> MakeItem_DetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch, int delvType, DateTime updateDate, DB.TWSQLDB.Models.Item itemTable = null, DB.TWSQLDB.Models.ItemTemp itemtempTale = null, string from = "", bool Synchronize = false)
        {
            ActionResponse<Item_DetialEdit> result = new ActionResponse<Item_DetialEdit>();
            result.Body = new Item_DetialEdit();
            result.IsSuccess = true;
            result.Msg = string.Empty;

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

            #endregion 計算毛利率

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
                            .ForMember(x => x.DelvDate, x => x.MapFrom(src => src.Item.DelvDate))
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
                            .ForMember(x => x.Qty, x => x.MapFrom(src => (src.Item.CanSaleLimitQty ?? 0) + src.Item.ItemQtyReg.Value))
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
                            // 更新者 ID
                            .ForMember(x => x.UpdateUser, x => x.MapFrom(src => src.CreateAndUpdate.UpdateUser));

            try
            {

                if (from == "Temp")
                {
                    #region Temp Area
                    if (Synchronize == false)
                    {
                        #region automap to model
                        AutoMapper.Mapper.Map(itemSketch, result.Body);
                        #endregion

                        result.Body.DelvType = delvType;
                        result.Body.GrossMargin = grossMargin;
                    }
                    else
                    {
                        int qtyLimit = 0;
                        qtyLimit = itemtempTale.QtyLimit;
                        #region automap to model
                        AutoMapper.Mapper.Map(itemSketch, result.Body);
                        result.Body.QtyLimit = qtyLimit;
                        result.Body.DelvType = delvType;
                        result.Body.GrossMargin = grossMargin;
                        #endregion
                    }
                    #endregion
                }
                else if (from == "Front")
                {
                    #region Front Area
                    //主要的 detail 資料
                    if (Synchronize == false)
                    {
                        #region automap to model
                        AutoMapper.Mapper.Map(itemSketch, result.Body);
                        #endregion

                        result.Body.DelvType = delvType;
                        result.Body.GrossMargin = grossMargin;
                    }
                    else
                    {
                        //其他同群組的資料
                        int qtyLimit = 0;
                        qtyLimit = itemTable.QtyLimit;
                        #region automap to model
                        AutoMapper.Mapper.Map(itemSketch, result.Body);
                        result.Body.QtyLimit = qtyLimit;
                        result.Body.DelvType = delvType;
                        result.Body.GrossMargin = grossMargin;
                        #endregion
                    }
                    #endregion
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = "資料錯誤";
                }
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "資料錯誤, 修改失敗";
                logger.Info("[Message]: " + error.Message + " ;[StackTrace]:" + error.StackTrace + " ;[InnerExceptionMessage]: " + exceptionService.InnerExceptionMessage(error));
            }
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }
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
                logger.Info(string.Format("取得商品失敗(expection); ProductID = {0}; ErrorMessage = {1}; StackTrace = {2}.", productID, GetExceptionMessage(ex), ex.StackTrace));
            }

            if (result.Body == null)
            {
                result.IsSuccess = false;
                result.Msg = "查無商品資訊。";
                logger.Info(string.Format("查無商品; ProductID = {0}.", productID));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }
        #endregion

        #region 共用
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
                    logger.Info(string.Format("取得 AccountTypeCode 失敗(expection); SellerID = {0}; ErrorMessage = {1}; StackTrace = {2}.", sellerID, GetExceptionMessage(ex), ex.StackTrace));
                }
            }
            else
            {
                result.IsSuccess = false;

                if (sellerID.Value == 0)
                {
                    logger.Info("商家 ID 不可為 0，取得 Account Type 失敗。");
                }

                if (sellerID == null)
                {
                    logger.Info("未輸入商家 ID，取得 Account Type 失敗。");
                }
            }

            if (string.IsNullOrEmpty(result.Body))
            {
                result.IsSuccess = false;
                result.Msg = "取得商家類別失敗。";
                logger.Info(string.Format("查無 SellerID = {0} 資料，取得 Account Type 失敗。", sellerID));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }
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
                logger.Info(string.Format("DelvType 給值失敗：AccountType = {0}， ShipType = {1}。", accountType, shipType));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }
        #endregion 共用
    }
}
