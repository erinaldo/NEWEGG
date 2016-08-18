using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TWNewEgg.API.Models;
using TWNewEgg.DB;
using log4net;
using log4net.Config;
using System.Threading;
using System.Web;

namespace TWNewEgg.API.Service
{
    /// <summary>
    /// 草稿
    /// </summary>
    public class ItemSketchService
    {
        // 記錄訊息
        private static ILog log = LogManager.GetLogger(typeof(ItemSketchService));
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        TWNewEgg.API.Service.ImageService imageService = new ImageService();
        TWNewEgg.BackendService.Interface.ICategoryService CategoryService = new TWNewEgg.BackendService.Service.CategoryService();

        
        #region 查詢草稿

        /// <summary>
        /// 取得草稿清單
        /// </summary>
        /// <param name="condition">草稿查詢條件</param>
        /// <returns>草稿清單</returns>
        public ActionResponse<List<ItemSketch>> GetItemSketchList(ItemSketchSearchCondition condition, bool IsTempCopy = false)
        {
            ActionResponse<List<ItemSketch>> result = new ActionResponse<List<ItemSketch>>();
            DB.TWSqlDBContext dbFront = new TWSqlDBContext();
            result.Body = new List<ItemSketch>();
            result.IsSuccess = true;
            result.Msg = string.Empty;
            
            // 查詢條件輸入檢查
            ActionResponse<bool> checkInputResult = CheckInput_ItemSketchSearchCondition(condition);

            if (checkInputResult.IsSuccess && checkInputResult.Body)
            {
                // 設定搜尋條件
                IQueryable<ItemSketch> sketchQueryable = SetQueryableInfo(condition, IsTempCopy);

                // 從資料庫取得資料
                try
                {
                    result.Body = sketchQueryable.OrderByDescending(x=>x.CreateAndUpdate.UpdateDate).ToList();
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    log.Info(string.Format("取得草稿清單失敗; ExceptionMessage = {0}; StackTrace = {1}", ex.Message, ex.StackTrace));
                }

                if (result.IsSuccess)
                {
                    if (result.Body.Count > 0)
                    {
                        string getYear = DateTime.Now.Year.ToString();
                        string getMonth = DateTime.Now.Month.ToString();
                        List<TWNewEgg.DB.TWSQLDB.Models.Currency> currencyList = dbFront.Currency.Where(x => x.Year == getYear && x.Month == getMonth).ToList();
                        // 更換顯示文字
                        foreach (ItemSketch itemSketch in result.Body)
                        {
                            #region 使用 ID 產生對應圖片網址、賣場連結

                            // 將 ID 補足 8 碼
                            string itemSketchID = itemSketch.ID.ToString("00000000");

                            // 圖片機網址
                            string images = System.Configuration.ConfigurationManager.AppSettings["Images"];

                            string serverPath = HttpContext.Current.Server.MapPath("~/pic/pic/ItemSketch/" + itemSketchID.Substring(0, 4) + "/" + itemSketchID.Substring(4, 4) + "_1_60.jpg");

                            if (System.IO.File.Exists(serverPath))
                            {
                            // 圖片位置 (圖片機/pic/itemsketch/草稿ID前4碼/草稿ID未4碼_1_60.jpg)
                            itemSketch.Product.PicPath_Sketch = images + "/pic/itemsketch/" + itemSketchID.Substring(0, 4) + "/" + itemSketchID.Substring(4, 4) + "_1_60.jpg";

                            itemSketch.Product.PicPatch_Edit = new List<string>();
                            // 修改畫面所需要的七張圖片 URL
                            if (itemSketch.Product.PicStart != 0 && itemSketch.Product.PicEnd != 0)
                            {
                                for (int picIndex = 1; picIndex <= itemSketch.Product.PicEnd; picIndex++)
                                {
                                    itemSketch.Product.PicPatch_Edit.Add(images + "/pic/itemsketch/" + itemSketchID.Substring(0, 4) + "/" + itemSketchID.Substring(4, 4) + "_" + picIndex + "_640.jpg");
                                }
                            }
                            }
                            else
                            {
                                string imagesServer = System.Configuration.ConfigurationManager.AppSettings["NeweggImages"];
                                //string imagesServer = "http://images.newegg.com.tw";

                                // 圖片位置 (圖片機/pic/itemsketch/草稿ID前4碼/草稿ID未4碼_1_60.jpg)
                                itemSketch.Product.PicPath_Sketch = imagesServer + "/pic/itemsketch/" + itemSketchID.Substring(0, 4) + "/" + itemSketchID.Substring(4, 4) + "_1_60.jpg";

                                itemSketch.Product.PicPatch_Edit = new List<string>();
                                // 修改畫面所需要的七張圖片 URL
                                if (itemSketch.Product.PicStart != 0 && itemSketch.Product.PicEnd != 0)
                                {
                                    for (int picIndex = 1; picIndex <= itemSketch.Product.PicEnd; picIndex++)
                                    {
                                        itemSketch.Product.PicPatch_Edit.Add(imagesServer + "/pic/itemsketch/" + itemSketchID.Substring(0, 4) + "/" + itemSketchID.Substring(4, 4) + "_" + picIndex + "_640.jpg");
                                    }
                                }
                            }

                            // 是否開啟預覽功能
                            string isOpenPreview = System.Configuration.ConfigurationManager.AppSettings["PreViewFlag"];
                            // 賣場網址
                            string webSite = System.Configuration.ConfigurationManager.AppSettings["WebSite"];

                            try
                            {
                                // 空值不顯示
                                if (string.IsNullOrWhiteSpace(isOpenPreview))
                                {
                                    itemSketch.Item.ItemURL = null;
                                }
                                else
                                {
                                    if (isOpenPreview.ToLower() == "off")
                                    {
                                        itemSketch.Item.ItemURL = null;
                                    }
                                    else
                                    {
                                        if (isOpenPreview.ToLower() == "on")
                                        {
                                            // 賣場連結
                                            string previewInfo = "sketch_" + condition.SellerID + "_" + itemSketch.ID + "_" + DateTime.UtcNow.AddHours(8).ToString("yyyy/MM/dd HH:mm:ss");

                                            string aesPreviewInfo = AesEncryptor.AesEncrypt(previewInfo);

                                            itemSketch.Item.ItemURL = webSite + "/item/ItemPreview?itemID=" + HttpUtility.UrlEncode(aesPreviewInfo);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error("預覽 Msg: " + ex.Message + ", Stacktrace: " + ex.StackTrace);
                            }

                            #endregion 使用 ID 產生對應圖片網址

                            #region 取匯率

                            TWNewEgg.DB.TWSQLDB.Models.Currency searchCurrency = currencyList.Where(x => x.CountryID == itemSketch.CountryID).FirstOrDefault();
                            if (searchCurrency != null)
                            {
                                itemSketch.CurrencyAverageExchange = searchCurrency.AverageexchangeRate;
                            }

                            #endregion

                            #region 計算毛利率

                            if ((itemSketch.Item.PriceCash.HasValue && itemSketch.Product.Cost.HasValue)
                                && (itemSketch.Item.PriceCash.Value > 0 && itemSketch.Product.Cost.Value > 0))
                            {
                                itemSketch.ItemDisplayPrice.GrossMargin = decimal.Round(((itemSketch.Item.PriceCash.Value - (itemSketch.Product.Cost.Value * itemSketch.CurrencyAverageExchange)) / itemSketch.Item.PriceCash.Value) * 100, 2);
                            }
                            else
                            {
                                itemSketch.ItemDisplayPrice.GrossMargin = 0;
                            }

                            #endregion 計算毛利率

                            #region 取得跨分類資訊

                            try
                            {
                                List<int> categoryIDCell = dbFront.ItemCategorySketch.Where(x => x.ItemSketchID == itemSketch.ID).Select(x => x.CategoryID).ToList();

                                if (categoryIDCell.Count > 0)
                                {
                                    itemSketch.ItemCategory.SubCategoryID_1_Layer2 = categoryIDCell[0];
                                    itemSketch.ItemCategory.SubCategoryID_1_Layer1 = dbFront.Category.Where(x => x.ID == itemSketch.ItemCategory.SubCategoryID_1_Layer2).Select(x => x.ParentID).FirstOrDefault();

                                    if (categoryIDCell.Count == 2)
                                    {
                                        itemSketch.ItemCategory.SubCategoryID_2_Layer2 = categoryIDCell[1];
                                        itemSketch.ItemCategory.SubCategoryID_2_Layer1 = dbFront.Category.Where(x => x.ID == itemSketch.ItemCategory.SubCategoryID_2_Layer2).Select(x => x.ParentID).FirstOrDefault();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                result.IsSuccess = false;
                                result.Msg = "發生 exception，取得跨分類資訊失敗。";
                                log.Info("取得跨分類資訊失敗(exception)：(ItemSketchID = " + itemSketch.ID + ") " + ex.ToString());
                            }

                            #endregion 取得跨分類

                            #region 運送類型

                            if (itemSketch.Item.ShipType == "V")
                            {
                                itemSketch.Item.ShipType = "S";
                            }

                            #endregion 運送類型

                            #region 調整開始銷售日期、結束銷售日期(過 json 到 view 時會減 8 小時，所以先在這裡加 8 小時)

                            itemSketch.Item.DateStart = itemSketch.Item.DateStart.Date.AddHours(8);
                            itemSketch.Item.DateEnd = itemSketch.Item.DateEnd.Date.AddHours(8);
                            itemSketch.CreateAndUpdate.CreateDate = itemSketch.CreateAndUpdate.CreateDate.AddHours(8);
                            itemSketch.CreateAndUpdate.UpdateDate = itemSketch.CreateAndUpdate.UpdateDate.AddHours(8);

                            #endregion 調整開始銷售日期、結束銷售日期(過 json 到 view 時會減 8 小時，所以先在這裡加 8 小時)
                        }
                    }
                    else
                    {
                        result.Msg = "查無資料。";
                    }
                }
            }
            else
            {
                result.IsSuccess = false;

                // 回傳檢查錯誤訊息
                result.Msg = checkInputResult.Msg;
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        #region 草稿查詢條件

        /// <summary>
        /// 設定搜尋條件
        /// </summary>
        /// <param name="condition">草稿查詢條件</param>
        /// <returns>搜尋條件</returns>
        private IQueryable<ItemSketch> SetQueryableInfo(ItemSketchSearchCondition condition, bool IsTempCopy = false)
        {
            IQueryable<ItemSketch> sketchQueryable = SetQueryableInfo_SetModel();

            if (IsTempCopy == false)
            {
                // 篩選刪除的資料
                sketchQueryable = sketchQueryable.Where(x => x.Status != 99).AsQueryable();
            }
            #region 篩選 SellerID

            // 但在有管理權限，且 SellerID 為 0 時不篩選，此時將顯示所有 SellerID 的資料
            if (!(condition.IsAdmin && condition.SellerID == 0))
            {
                sketchQueryable = sketchQueryable.Where(x => x.Item.SellerID == condition.SellerID).AsQueryable();
            }

            #endregion 篩選 SellerID

            #region 關鍵字搜尋

            if (!string.IsNullOrEmpty(condition.KeyWord))
            {
                switch (condition.KeyWordScarchTarget)
                {
                    
                    case ItemSketchKeyWordSearchTarget.SellerProductID:
                        {
                            sketchQueryable = sketchQueryable.Where(x => x.Product.SellerProductID.IndexOf(condition.KeyWord) != -1).OrderBy(x => x.Product.SellerProductID).ThenBy(x => x.ID).AsQueryable();
                            break;
                        }
                    case ItemSketchKeyWordSearchTarget.MenufacturePartNum:
                        {
                            sketchQueryable = sketchQueryable.Where(x => x.Product.MenufacturePartNum.IndexOf(condition.KeyWord) != -1).OrderBy(x => x.Product.MenufacturePartNum).ThenBy(x => x.ID).AsQueryable();
                            break;
                        }
                    case ItemSketchKeyWordSearchTarget.ProductName:
                        {
                            sketchQueryable = sketchQueryable.Where(x => x.Product.Name.IndexOf(condition.KeyWord) != -1).OrderBy(x => x.Product.Name).ThenBy(x => x.ID).AsQueryable();
                            break;
                        }
                    case ItemSketchKeyWordSearchTarget.ItemSketchID:
                        {
                            ActionResponse<List<int>> searchItemSketchIDResult = SearchItemSketchID(condition.KeyWord, IsTempCopy);
                            sketchQueryable = sketchQueryable.Where(x => searchItemSketchIDResult.Body.Contains(x.ID)).OrderBy(x => x.ID).AsQueryable();
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }

            #endregion 關鍵字搜尋

            #region 進階搜尋

            #region 依製造商篩選 (0 = 所有)

            if (condition.ManufactureID != 0)
            {
                sketchQueryable = sketchQueryable.Where(x => x.Product.ManufactureID == condition.ManufactureID).OrderBy(x => x.Product.ManufactureID).ThenBy(x => x.ID).AsQueryable();
            }

            #endregion 依製造商篩選 (0 = 所有)

            #region 依可賣數量篩選

            if (condition.canSellQty != ItemSketchCanSellQty.All)
            {
                if (condition.canSellQty == ItemSketchCanSellQty.EqualOrMoreThen100)
                {
                    sketchQueryable = sketchQueryable.Where(x => x.ItemStock.CanSaleQty >= 100).OrderBy(x => x.ItemStock.CanSaleQty).ThenBy(x => x.ID).ThenBy(x => x.ID).AsQueryable();
                }
                else
                {
                    int canSellQty = (int)condition.canSellQty;
                    sketchQueryable = sketchQueryable.Where(x => x.ItemStock.CanSaleQty < canSellQty).OrderBy(x => x.ItemStock.CanSaleQty).ThenBy(x => x.ID).AsQueryable();
                }
            }

            #endregion 依可賣數量篩選

            #region 依分類篩選

            if (condition.categoryID_Layer0 > 0 || condition.categoryID_Layer1 > 0 || condition.categoryID_Layer2 > 0)
            {
                // 第 0 到第 2 層分類都有選擇
                if (condition.categoryID_Layer0 > 0 && condition.categoryID_Layer1 > 0 && condition.categoryID_Layer2 > 0)
                {
                    sketchQueryable = sketchQueryable.Where(x => x.ItemCategory.MainCategoryID_Layer2 == condition.categoryID_Layer2).OrderBy(x => x.ItemCategory.MainCategoryID_Layer2).ThenBy(x => x.ID).AsQueryable();
                }
                else
                {
                    // 第 2 層分類 ID 清單
                    List<int> categoryIDCell_Layer2 = new List<int>();

                    // 只選擇第 0 到第 1 層分類
                    if (condition.categoryID_Layer0 > 0 && condition.categoryID_Layer1 > 0)
                    {
                        // 由第 1 層的分類 ID 查詢第 2 層的所有分類 ID
                        ActionResponse<List<int>> GetCategoryIDCellResult = GetChildCategoryIDCell(condition.categoryID_Layer1);

                        if (GetCategoryIDCellResult.IsSuccess)
                        {
                            categoryIDCell_Layer2.AddRange(GetCategoryIDCellResult.Body);
                        }
                    }
                    else if (condition.categoryID_Layer0 > 0)
                    {
                        // 只選擇第 0 層分類
                        // 由第 0 層的分類 ID 查詢第 1 層的所有分類 ID
                        ActionResponse<List<int>> categoryIDCell_Layer1 = GetChildCategoryIDCell(condition.categoryID_Layer0);

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
                    sketchQueryable = sketchQueryable.Where(x => categoryIDCell_Layer2.Contains(x.ItemCategory.MainCategoryID_Layer2 ?? 0)).OrderBy(x => x.ItemCategory.MainCategoryID_Layer2).ThenBy(x => x.ID).AsQueryable();
                }
            }

            #endregion 依分類篩選

            #region 依創建日期篩選

            if (condition.createDate != ItemSketchCreateDate.All)
            {
                DateTime startDate, endDate;

                switch (condition.createDate)
                {
                    default:
                    case ItemSketchCreateDate.Today:
                    case ItemSketchCreateDate.Last3Days:
                    case ItemSketchCreateDate.Last7Days:
                    case ItemSketchCreateDate.Last30Days:
                        {
                            startDate = DateTime.Today;
                            endDate = startDate.AddDays(1);

                            if (condition.createDate != ItemSketchCreateDate.Today)
                            {
                                startDate = startDate.AddDays(-((int)condition.createDate) + 1);
                            }
                            break;
                        }
                    case ItemSketchCreateDate.SpecifyDate:
                    case ItemSketchCreateDate.DateRange:
                        {
                            startDate = Convert.ToDateTime(condition.startDate.Value.AddHours(8)).Date;
                            endDate = Convert.ToDateTime(condition.endDate.Value.AddHours(8)).Date.AddDays(1);
                            break;
                        }
                }

                sketchQueryable = sketchQueryable.Where(x => x.CreateAndUpdate.CreateDate >= startDate && x.CreateAndUpdate.CreateDate < endDate).OrderByDescending(x => x.CreateAndUpdate.CreateDate).ThenBy(x => x.ID).AsQueryable();
            }

            #endregion 依創建日期篩選

            #endregion 進階搜尋

            #region 排序項目、遞增遞減設定

            #region 判斷搜尋條件有幾項

            // 搜尋項目數
            int searchConditionItmeCount = 0;

            if (string.IsNullOrEmpty(condition.KeyWord) == false)
            {
                searchConditionItmeCount++;
            }

            if (searchConditionItmeCount > 0)
            {
                searchConditionItmeCount++;
            }

            if (condition.canSellQty != ItemSketchCanSellQty.All)
            {
                searchConditionItmeCount++;
            }

            if (condition.categoryID_Layer0 > 0 || condition.categoryID_Layer1 > 0 || condition.categoryID_Layer2 > 0)
            {
                searchConditionItmeCount++;
            }

            if (condition.createDate != ItemSketchCreateDate.All)
            {
                searchConditionItmeCount++;
            }

            #endregion 判斷搜尋條件有幾項

            // 未設定任格搜索條件，或為多項目的複合搜尋時，將排序設為遞減的更新日期
            if (searchConditionItmeCount != 1)
            {
                sketchQueryable = sketchQueryable.OrderByDescending(x => x.CreateAndUpdate.UpdateDate).AsQueryable();
            }

            #endregion 排序項目、遞增遞減設定

            #region 分頁、顯示資料筆數設定

            //if (condition.PageIndex >= 0 && condition.PageSize > 0)
            //{
            //    sketchQueryable = sketchQueryable.Skip(condition.PageIndex.Value * condition.PageSize.Value).Take(condition.PageSize.Value).AsQueryable();
            //}

            #endregion 分頁、顯示資料筆數設定

            return sketchQueryable;
        }

        /// <summary>
        /// 設定查詢 DB 及 使用 Model
        /// </summary>
        /// <returns>搜尋條件</returns>
        private IQueryable<ItemSketch> SetQueryableInfo_SetModel()
        {
            DB.TWSqlDBContext dbFront = new TWSqlDBContext();

            IQueryable<ItemSketch> sketchQueryable = (from sketch in dbFront.ItemSketch
                                                      join seller in dbFront.Seller on sketch.SellerID equals seller.ID
                                                      join manufacturer in dbFront.Manufacture on sketch.ManufactureID equals manufacturer.ID into mnufacturer_Temp
                                                      from manufacturer in mnufacturer_Temp.DefaultIfEmpty()
                                                      join category_Layer2 in dbFront.Category on sketch.CategoryID equals category_Layer2.ID into category_Layer2_Temp
                                                      from category_Layer2 in category_Layer2_Temp.DefaultIfEmpty()
                                                      join category_Layer1 in dbFront.Category on category_Layer2.ParentID equals category_Layer1.ID into category_Layer1_Temp
                                                      from category_Layer1 in category_Layer1_Temp.DefaultIfEmpty()
                                                      join category_Layer0 in dbFront.Category on category_Layer1.ParentID equals category_Layer0.ID into category_Layer0_Temp
                                                      from category_Layer0 in category_Layer0_Temp.DefaultIfEmpty()
                                                      select new ItemSketch
                                                      {
                                                          ID = sketch.ID,
                                                          Status = sketch.Status.Value,
                                                          CountryID = seller.CountryID ?? 1,
                                                          CurrencyAverageExchange = 1,
                                                          GroupID = sketch.GroupID,
                                                          Item = new ItemSketch_Item
                                                          {
                                                              DateEnd = sketch.DateEnd.Value,
                                                              DateStart = sketch.DateStart.Value,
                                                              DelvDate = sketch.DelvDate,
                                                              IsNew = sketch.IsNew,
                                                              ItemPackage = sketch.ItemPackage,
                                                              ItemQty = 0,
                                                              ItemQtyReg = 0,
                                                              CanSaleLimitQty = sketch.ItemQty,
                                                              QtyLimit = sketch.QtyLimit.Value,
                                                              MarketPrice = sketch.MarketPrice,
                                                              PriceCash = sketch.PriceCash,
                                                              SellerID = sketch.SellerID,
                                                              SellerName = seller.Name,
                                                              ShipType = sketch.ShipType,
                                                              Spechead = sketch.Spechead,
                                                              Sdesc = sketch.Sdesc,
                                                              Note = sketch.Note,
                                                              ShowOrder = sketch.ShowOrder,
                                                              Discard4 = sketch.Discard4
                                                          },
                                                          Product = new ItemSketch_Product
                                                          {
                                                              BarCode = sketch.BarCode,
                                                              Cost = sketch.Cost,
                                                              Description = sketch.Description,
                                                              Height = sketch.Height.Value,
                                                              Is18 = sketch.Is18,
                                                              IsChokingDanger = sketch.IsChokingDanger,
                                                              IsShipDanger = sketch.IsShipDanger,
                                                              Length = sketch.Length.Value,
                                                              ManufactureID = sketch.ManufactureID,
                                                              ManufacturerName = manufacturer.Name,
                                                              MenufacturePartNum = sketch.MenufacturePartNum,
                                                              Model = sketch.Model,
                                                              Name = sketch.Name,
                                                              PicEnd = sketch.PicEnd,
                                                              PicStart = sketch.PicStart,
                                                              SellerProductID = sketch.SellerProductID,
                                                              UPC = sketch.UPC,
                                                              Warranty = sketch.Warranty.Value,
                                                              Weight = sketch.Weight.Value,
                                                              Width = sketch.Width.Value
                                                          },
                                                          ItemStock = new ItemSketch_ItemStock
                                                          {
                                                              InventoryQty = sketch.InventoryQty,
                                                              InventoryQtyReg = 0,
                                                              InventorySafeQty = sketch.InventorySafeQty,
                                                              CanSaleQty = sketch.InventoryQty
                                                          },
                                                          ItemCategory = new ItemSketch_ItemCategory
                                                          {
                                                              MainCategoryID_Layer2 = sketch.CategoryID,
                                                              MainCategoryName_Layer2 = category_Layer2.Description,
                                                              MainCategoryID_Layer1 = category_Layer1.ID,
                                                              MainCategoryName_Layer1 = category_Layer1.Description,
                                                              MainCategoryID_Layer0 = category_Layer0.ID,
                                                              MainCategoryName_Layer0 = category_Layer0.Description
                                                          },
                                                          CreateAndUpdate = new CreateAndUpdateIfno
                                                          {
                                                              CreateDate = sketch.CreateDate.Value,
                                                              UpdateDate = sketch.UpdateDate.Value
                                                          }
                                                      }).AsQueryable();

            return sketchQueryable;
        }

        /// <summary>
        /// 列出包函關鍵字內容的草稿 ID
        /// </summary>
        /// <param name="KeyWord">關鍵字</param>
        /// <returns>草稿 ID 清單</returns>
        private ActionResponse<List<int>> SearchItemSketchID(string KeyWord, bool isTempCopy = false)
        {
            ActionResponse<List<int>> result = new ActionResponse<List<int>>();
            result.Body = new List<int>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            DB.TWSqlDBContext dbFront = new TWSqlDBContext();

            // 讀取草稿內，非刪除的草稿 ID
            List<int> itemSketchIDCell = new List<int>();
            try
            {
                if (isTempCopy == false)
                {
                    itemSketchIDCell = dbFront.ItemSketch.Where(x => x.Status != 99).Select(x => x.ID).ToList();
                }
                else
                {
                    itemSketchIDCell = dbFront.ItemSketch.Select(p => p.ID).ToList();
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = "讀取草稿 ID 清單失敗。";
                log.Info(string.Format("讀取草稿 ID 清單失敗(expection); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            // 將包函 KeyWord 字樣的草稿 ID 列出
            if (result.IsSuccess && itemSketchIDCell.Count > 0)
            {
                foreach (int itemSketchID in itemSketchIDCell)
                {
                    if (itemSketchID.ToString().IndexOf(KeyWord) != -1)
                    {
                        result.Body.Add(itemSketchID);
                    }
                }
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        #endregion 草稿查詢條件

        #endregion 查詢草稿

        public ActionResponse<string> DeleteItemSketch(int SketchID)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            DB.TWSqlDBContext db_front = new TWSqlDBContext();
            DB.TWSQLDB.Models.ItemSketch _itemSketch = new DB.TWSQLDB.Models.ItemSketch();
            _itemSketch = db_front.ItemSketch.Where(p => p.ID == SketchID).FirstOrDefault();
            if (_itemSketch == null)
            {
                result.Body = null;
                result.Code = (int)ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "The data is not exists";
            }
            else
            {
                _itemSketch.Status = 99;
                try
                {
                    db_front.SaveChanges();
                    result.Body = null;
                    result.Code = (int)ResponseCode.Success;
                    result.IsSuccess = true;
                    result.Msg = "Delete Success";
                }
                catch (Exception error)
                {
                    result.Body = null;
                    result.Code = (int)ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = error.Message;
                }
            }
            return result;
        }

        #region new itemstockTemp model
        public ActionResponse<string> Itemstocktemp(DB.TWSQLDB.Models.ItemSketch _itemSketchtemp, int productId)
        {
            DB.TWSqlDBContext db_before = new TWSqlDBContext();
            ActionResponse<string> result = new ActionResponse<string>();
            DateTime dateTimeMillisecond = new DateTime(0001, 01, 01, 01, 01, 01, 01);
            string timeString = dateTimeMillisecond.ToString("yyyy-MM-dd hh:mm:ss fff");
            dateTimeMillisecond = DateTime.Now;

            DB.TWSQLDB.Models.ItemStocktemp _itemStocktemp = new DB.TWSQLDB.Models.ItemStocktemp();
            _itemStocktemp.producttempID = productId;
            _itemStocktemp.Qty = _itemSketchtemp.InventoryQty.GetValueOrDefault();
            _itemStocktemp.QtyReg = 0;
            _itemStocktemp.SafeQty = _itemSketchtemp.InventorySafeQty.GetValueOrDefault();
            _itemStocktemp.Fdbcklmt = 0;
            _itemStocktemp.CreateUser = _itemSketchtemp.CreateUser;
            _itemStocktemp.Updated = 0;
            _itemStocktemp.CreateDate = dateTimeMillisecond;
            _itemStocktemp.UpdateUser = _itemSketchtemp.UpdateUser;
            _itemStocktemp.UpdateDate = dateTimeMillisecond;
            db_before.ItemStocktemp.Add(_itemStocktemp);
            try
            {
                db_before.SaveChanges();
                result.IsSuccess = true;
                result.Code = (int)ResponseCode.Success;
                result.Msg = "success";
            }
            catch (Exception error)
            {
                logger.Error("/ItemSketch/Itemstocktemp error: " + error.Message + " [ErrorStackTrace] " + error.StackTrace + "; ItemStocktemp insert error");
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                result.Code = (int)ResponseCode.Error;
            }
            return result;
        }
        #endregion

        #region new itemtemp model and itemcategorytemp mode
        public ActionResponse<string> ItemtempAndItemCategorytemp(DB.TWSQLDB.Models.ItemSketch itemTemp, int productId, string Userid)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            DB.TWSqlDBContext db_before = new TWSqlDBContext();
            DateTime dateTimeMillisecond = new DateTime(0001, 01, 01, 01, 01, 01, 01);
            string timeString = dateTimeMillisecond.ToString("yyyy-MM-dd hh:mm:ss fff");
            dateTimeMillisecond = DateTime.Now;

            DB.TWSQLDB.Models.ItemTemp itemtempAdd = new DB.TWSQLDB.Models.ItemTemp();
            itemtempAdd.ProduttempID = productId;
            itemtempAdd.Name = itemTemp.Name;
            itemtempAdd.Sdesc = itemTemp.Sdesc;
            itemtempAdd.DescTW = itemTemp.Description;
            itemtempAdd.ItemTempDesc = itemTemp.Description;
            itemtempAdd.SpecDetail = "";//defalut value
            itemtempAdd.Spechead = itemTemp.Spechead;
            itemtempAdd.SaleType = 1;//defalut value
            itemtempAdd.PayType = 0;//defalut value
            itemtempAdd.Layout = 0;//defalut value
            itemtempAdd.DelvType = itemTemp.DelvType.GetValueOrDefault();
            itemtempAdd.DelvData = itemTemp.DelvDate ?? "1-7";
            itemtempAdd.ItemNumber = "";//defalut value
            itemtempAdd.CategoryID = itemTemp.CategoryID.GetValueOrDefault();
            if (itemTemp.Model == null)
            {
                itemtempAdd.Model = "";
            }
            else
            {
                itemtempAdd.Model = itemTemp.Model;
            }
            itemtempAdd.Discard4 = itemTemp.Discard4??"N";
            itemtempAdd.SellerID = itemTemp.SellerID.GetValueOrDefault();
            itemtempAdd.DateStart = itemTemp.DateStart.GetValueOrDefault();
            itemtempAdd.DateEnd = itemTemp.DateEnd.GetValueOrDefault();
            itemtempAdd.DateDel = itemTemp.DateDel.GetValueOrDefault();
            itemtempAdd.Pricesgst = 0;//defalut value
            itemtempAdd.PriceCard = itemTemp.PriceCard.GetValueOrDefault();
            itemtempAdd.PriceCash = itemTemp.PriceCash.GetValueOrDefault();
            itemtempAdd.ServicePrice = 0;
            itemtempAdd.PricehpType1 = 0;//defalut value
            itemtempAdd.Pricehpinst1 = 0;//defalut value
            itemtempAdd.PricehpType2 = 0;//defalut value
            itemtempAdd.Pricehpinst2 = 0;//defalut value
            itemtempAdd.Inst0Rate = 0;//defalut value
            itemtempAdd.RedmfdbckRate = 0;//defalut value
            itemtempAdd.Coupon = "0";//defalut value
            itemtempAdd.PriceCoupon = 0;//defalut value

            if (itemTemp.PriceLocalship == null)
            {
                itemtempAdd.PriceLocalship = 0;
            }
            else
            {
                itemtempAdd.PriceLocalship = (int)itemTemp.PriceLocalship;
            }
            itemtempAdd.PriceGlobalship = 0;//default value
            
            //itemtemp.PriceTrade 不使用
            itemtempAdd.Qty = itemTemp.ItemQty.GetValueOrDefault();
            itemtempAdd.SafeQty = 0;
            itemtempAdd.QtyLimit = itemTemp.QtyLimit.GetValueOrDefault();
            itemtempAdd.LimitRule = "";//defalut value
            itemtempAdd.QtyReg = 0;//defalut value
            itemtempAdd.PhotoName = "";//defalut value
            itemtempAdd.HtmlName = "";//defalut value
            itemtempAdd.Showorder = itemTemp.ShowOrder ?? default(int);

            itemtempAdd.Class = 1;//defalut value
            itemtempAdd.Status = 1;//defalut value
            itemtempAdd.ManufactureID = itemTemp.ManufactureID.GetValueOrDefault();
            itemtempAdd.StatusNote = "";//defalut value
            itemtempAdd.StatusDate = dateTimeMillisecond;
            if (string.IsNullOrEmpty(itemTemp.Note) == true)
            {
                itemtempAdd.Note = "";
            }
            else
            {
                itemtempAdd.Note = itemTemp.Note;
            }

            /*
            上下架狀態
            0：上架
            1：下架、未上架
            2：強制下架(無上架機會)
            3：售價異常(系統判斷下架)           
             */
            itemtempAdd.ItemStatus = 1;
            itemtempAdd.CreateDate = dateTimeMillisecond;
            itemtempAdd.CreateUser = itemTemp.CreateUser;
            itemtempAdd.Updated = 0;//defalut value
            itemtempAdd.UpdateUser = itemTemp.UpdateUser;
            //UpdateDate(更新日期)：不給值(因為有更新才給)
            //itemtemp.UpdateDate 
            itemtempAdd.PicStart = itemTemp.PicStart;
            itemtempAdd.PicEnd = itemTemp.PicEnd;
            itemtempAdd.MarketPrice = itemTemp.MarketPrice;
            itemtempAdd.WareHouseID = itemTemp.WarehouseID;
            itemtempAdd.ShipType = itemTemp.ShipType;
            itemtempAdd.Taxfee = 0;//defalut value
            itemtempAdd.ItemPackage = itemTemp.ItemPackage;
            itemtempAdd.IsNew = itemTemp.IsNew;
            itemtempAdd.GrossMargin = itemTemp.GrossMargin;
            //itemtemp.ApproveMan = item
            //itemtemp.ApproveDate = 
            itemtempAdd.SubmitMan = Userid;
            itemtempAdd.SubmitDate = dateTimeMillisecond;//不給值會出現datetime2無法轉換成datetime錯誤
            itemtempAdd.ApproveDate = null;
            db_before.ItemTemp.Add(itemtempAdd);
            try
            {
                //必須先savechenge 才會有itemtempAdd id
                db_before.SaveChanges();
                result.IsSuccess = true;
                result.Msg = "success";
                result.Code = (int)ResponseCode.Success;
            }
            catch (Exception error)
            {
                logger.Error("/ItemSketch/ItemtempAndItemCategorytemp error: " + error.Message + " [ErrorStackTrace] " + error.StackTrace + "; ItemTemp insert error");
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                result.Code = (int)ResponseCode.Error;
            }
            if (result.IsSuccess == true && result.Code == (int)ResponseCode.Success)
            {
                List<DB.TWSQLDB.Models.ItemCategorySketch> _itemCategorySketch = db_before.ItemCategorySketch.Where(p => p.ItemSketchID == itemTemp.ID).ToList();
                if (_itemCategorySketch.Count() != 0)
                {
                    foreach (var item in _itemCategorySketch)
                    {
                        DB.TWSQLDB.Models.ItemCategorytemp _itemcategorytemp = new DB.TWSQLDB.Models.ItemCategorytemp();
                        _itemcategorytemp.itemtempID = itemtempAdd.ID;
                        _itemcategorytemp.CategoryID = item.CategoryID;
                        _itemcategorytemp.FromSystem = "1";//0: PM; 1: sellerPortal
                        _itemcategorytemp.CreateUser = item.CreateUser;
                        _itemcategorytemp.CreateDate = item.CreateDate;
                        _itemcategorytemp.UpdateDate = item.UpdateDate;
                        _itemcategorytemp.UpdateUser = item.UpdateUser;
                        db_before.ItemCategorytemp.Add(_itemcategorytemp);
                    }
                }
                try
                {
                    db_before.SaveChanges();
                    result.IsSuccess = true;
                    result.Code = (int)ResponseCode.Success;
                    result.Msg = "Success";
                    result.Body = itemtempAdd.ID.ToString();
                }
                catch (Exception error)
                {
                    logger.Error("/ItemSketch/ItemtempAndItemCategorytemp error: " + error.Message + " [ErrorStackTrace] " + error.StackTrace + "; ItemCategorytemp insert error");
                    result.IsSuccess = false;
                    result.Msg = "資料錯誤";
                    result.Code = (int)ResponseCode.Error;
                }
            }
            return result;
        }
        #endregion

        #region new productTemp mode and new productpropertytem model
        public ActionResponse<string> ProductTempAndProductPropertyTemp(List<DB.TWSQLDB.Models.ItemSketch> itemSketchModel, string Userid/*, List<int> itemSketchID*/)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            DB.TWSqlDBContext db_before = new TWSqlDBContext();
            DateTime dateTimeMillisecond = new DateTime(0001, 01, 01, 01, 01, 01, 01);
            string timeString = dateTimeMillisecond.ToString("yyyy-MM-dd hh:mm:ss fff");
            dateTimeMillisecond = DateTime.Now;
            //List<DB.TWSQLDB.Models.ItemSketch> _listItemSketch = db_before.ItemSketch.Where(p => itemSketchID.Contains(p.ID)).ToList();
            //List<DB.TWSQLDB.Models.ProductPropertySketch> _listProductPropertySketch = db_before.ProductPropertySketch.Where(p => itemSketchID.Contains(p.ItemSketchID)).ToList();
            foreach (var item in itemSketchModel)
            {
                DB.TWSQLDB.Models.ProductTemp productTempAdd = new DB.TWSQLDB.Models.ProductTemp();
                productTempAdd.SellerProductID = string.IsNullOrWhiteSpace(item.SellerProductID) ? "1-7" : item.SellerProductID;

               
                productTempAdd.Name = item.Name;
                productTempAdd.NameTW = item.Name;                             
                productTempAdd.Description = item.Description;
                productTempAdd.DescriptionTW = item.Description;
                productTempAdd.SPEC = ""; //SPEC已不使用;
                productTempAdd.ManufactureID = item.ManufactureID.GetValueOrDefault();
                productTempAdd.Model = item.Model;
                productTempAdd.BarCode = item.BarCode;
                productTempAdd.SellerID = item.SellerID.GetValueOrDefault();
                productTempAdd.DelvType = item.DelvType;
                productTempAdd.PicStart = item.PicStart;
                productTempAdd.PicEnd = item.PicEnd;
                productTempAdd.Cost = item.Cost.GetValueOrDefault();
                productTempAdd.Status = 1;
                productTempAdd.InvoiceType = 0;//default value
                productTempAdd.SaleType = 0;//default value
                productTempAdd.Length = item.Length;
                productTempAdd.Width = item.Width;
                productTempAdd.Height = item.Height;
                productTempAdd.Weight = item.Weight;
                productTempAdd.CreateUser = item.CreateUser;
                productTempAdd.CreateDate = dateTimeMillisecond;
                productTempAdd.Updated = 0;//default value
                productTempAdd.UpdateDate = item.UpdateDate;
                productTempAdd.UpdateUser = item.UpdateUser;
                if (item.TradeTax == null)
                {
                    productTempAdd.TradeTax = 0;
                }
                else
                {
                    productTempAdd.TradeTax = item.TradeTax;
                }
                productTempAdd.Tax = 0;//default value
                productTempAdd.Warranty = item.Warranty;
                productTempAdd.UPC = item.UPC;
                productTempAdd.Note = item.Note;
                productTempAdd.IsMarket = "Y";
                productTempAdd.Is18 = item.Is18;
                productTempAdd.IsShipDanger = item.IsShipDanger;
                productTempAdd.IsChokingDanger = item.IsChokingDanger;
                productTempAdd.MenufacturePartNum = item.MenufacturePartNum;
                //productTempAdd.SPECLabel
                //productTempAdd.SupplyShippingCharge
                db_before.ProductTemp.Add(productTempAdd);
                try
                {
                    //必須先saveChange，這樣才會有 productTempAdd id
                    db_before.SaveChanges();
                    //result.IsSuccess = true;
                    //result.Code = (int)ResponseCode.Success;
                }
                catch (Exception error)
                {
                    logger.Error("/ItemSketch/ProductTempAndProductPropertyTemp error: " + error.Message + " [ErrorStackTrace] " + error.StackTrace);
                    result.IsSuccess = false;
                    result.Code = (int)ResponseCode.Error;
                    result.Msg = "資料錯誤";
                    break;//跳出迴圈
                }
                List<DB.TWSQLDB.Models.ProductPropertySketch> _productProprttySketch = db_before.ProductPropertySketch.Where(p => p.ItemSketchID == item.ID).ToList();
                //DB.TWSQLDB.Models.ProductPropertySketch _productProprttySketch = _listProductPropertySketch.Where(p => p.ItemSketchID == item.ID).FirstOrDefault();
                if (_productProprttySketch.Count() != 0)
                {
                    foreach (var productItem in _productProprttySketch)
                    {
                        DB.TWSQLDB.Models.ProductPropertytemp _productPropertytemp = new DB.TWSQLDB.Models.ProductPropertytemp();
                        _productPropertytemp.producttempID = productTempAdd.ID;
                        _productPropertytemp.ProductValueID = productItem.ProductValueID;
                        _productPropertytemp.UserInputValue = productItem.UserInputValue;
                        _productPropertytemp.UserInputValueTW = productItem.UserInputValueTW;
                        _productPropertytemp.Show = 0;//default value is 0, 0: show; 1: not show
                        _productPropertytemp.Label = 0;//default value is 0, 0: 不產生; 1: 產生標籤
                        _productPropertytemp.GroupID = productItem.GroupID.GetValueOrDefault();
                        _productPropertytemp.PropertyNameID = productItem.PropertyNameID.GetValueOrDefault();
                        _productPropertytemp.CreateUser = productItem.CreateUser;
                        _productPropertytemp.CreateDate = productItem.CreateDate;
                        _productPropertytemp.UpdateUser = productItem.UpdateUser;
                        _productPropertytemp.UpdateDate = productItem.UpdateDate;
                        db_before.ProductPropertytemp.Add(_productPropertytemp);
                    }
                }
                var r1 = ItemtempAndItemCategorytemp(item, productTempAdd.ID, Userid);
                var r2 = Itemstocktemp(item, productTempAdd.ID);
                if (r1.IsSuccess == true && r2.IsSuccess == true)
                {
                    //DB.TWSQLDB.Models.ItemSketch update = new DB.TWSQLDB.Models.ItemSketch();
                    //update = _listItemSketch.Where(p => p.ID == item.ID).FirstOrDefault();
                    //update.ProducttempID = productTempAdd.ID;
                    //update.itemtempID = Convert.ToInt32(r1.Body);
                    logger.Info("送審區域: itemsketchid = "  + item.ID + ", ProducttempID = " + productTempAdd.ID + ", itemtempID = " + r1.Body.ToString());
                    if (productTempAdd.ID <= 0 || string.IsNullOrEmpty(r1.Body) == true)
                    {
                        result.IsSuccess = false;
                        result.Code = (int)ResponseCode.Error;
                        result.Msg = "送審失敗";
                        logger.Info("送審區域: itemsketchid is :" + item.ID + "ProducttempID = " + productTempAdd.ID + ", itemtempID = " + r1.Body + "; 有一值為NULL");
                    }
                    else
                    {
                        item.ProducttempID = productTempAdd.ID;
                        item.itemtempID = Convert.ToInt32(r1.Body);
                        logger.Info("送審區域: ProducttempID, itemtempID 回填");
                        try
                        {
                            db_before.SaveChanges();//save ProductPropertytemp table and update the producttempid and itemtempid of the item
                            logger.Info("送審區域: ProducttempID, itemtempID 回填完成. SaveChanges 完成");
                            result.IsSuccess = true;
                            result.Msg = "success";
                            result.Code = (int)ResponseCode.Success;
                        }
                        catch (Exception error)
                        {
                            logger.Error("/ItemSketch/ProductTempAndProductPropertyTemp error: " + error.Message + " [ErrorStackTrace] " + error.StackTrace + "; ProductTemp insert error");
                            result.IsSuccess = false;
                            result.Msg = "資料錯誤";
                            result.Code = (int)ResponseCode.Error;
                            break;//跳出迴圈
                        }
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = "資料錯誤";
                    result.Code = (int)ResponseCode.Error;
                    break;
                }
            }
            logger.Info("送審結束");
            return result;
        }
        #endregion

        public ActionResponse<string> VerifyStetchModelCheck(List<DB.TWSQLDB.Models.ItemSketch> _listItemSketchCheck)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            string _strResult = string.Empty;
            result.IsSuccess = true;
            result.Msg = "success";
            foreach (var item in _listItemSketchCheck)
            {
                //商品類別
                if (item.CategoryID == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請選擇商品主分類";
                    break;
                }
                //製造商
                if (item.ManufactureID == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請選擇製造商";
                    break;
                }
                //材積(公分) 長
                if (item.Length == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選材積\"長\"";
                    break;
                }
                //材積(公分) 寬
                if (item.Width == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選材積\"寬\"";
                    break;
                }
                //材積(公分) 高
                if (item.Height == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選材積\"高\"";
                    break;
                }
                //重量(公斤)
                if (item.Weight == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選材積\"重量\"";
                    break;
                }
                //窒息危險性
                if (string.IsNullOrEmpty(item.IsChokingDanger) == true)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\"窒息危險性\"";
                    break;
                }
                //遞送危險物料
                if (string.IsNullOrEmpty(item.IsShipDanger) == true)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\"遞送危險物料\"";
                    break;
                }
                //遞送方式
                if (string.IsNullOrEmpty(item.ItemPackage) == true)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\"遞送方式\"";
                    break;
                }
                //售價
                if (item.PriceCash == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\"售價\"";
                    break;
                }
                //成本
                if (item.Cost == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\"成本\"";
                    break;
                }

                // 毛利率
                if ((item.PriceCash.HasValue && item.Cost.HasValue) && (item.Cost.Value > item.PriceCash))
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",\"毛利率\"為負數，請重新設定售價或成本";
                    break;
                }

                //可售數量
                if (item.InventoryQty - item.InventoryQtyReg <= 0)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",\"可售數量\"不可為0";
                    break;
                }
                //安全庫存
                if (item.InventorySafeQty <= 0)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",\"安全庫存\"不可為0";
                    break;
                }
                //商品名稱
                if (string.IsNullOrEmpty(item.Name) == true)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\"商品名稱\"";
                    break;
                }
                //商品簡要描述
                if (string.IsNullOrEmpty(item.Spechead) == true)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\" 商品簡要描述\"";
                    break;
                }
                //商品特色標題
                if (string.IsNullOrEmpty(item.Sdesc) == true)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\" 商品特色標題\"";
                    break;
                }
                //商品中文說明
                if (string.IsNullOrEmpty(item.Description) == true)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\" 商品中文說明\"";
                    break;
                }
                //賣場開賣時間
                if (item.DateStart == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\"賣場開賣日期錯誤\"";
                    break;
                }
                else
                {
                    DateTime timeTemp;
                    bool dateSuccess = DateTime.TryParse(item.DateStart.ToString(), out timeTemp);
                    if (dateSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = "產品編號為: " + item.ID + ",請填選\"賣場開賣日期錯誤\"";
                        break;
                    }
                }
                //賣場結束日期
                if (item.DateEnd == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",賣場結束日期錯誤";
                    break;
                }
                else
                {
                    DateTime timeTemp;
                    bool dateSuccess = DateTime.TryParse(item.DateEnd.ToString(), out timeTemp);
                    if (dateSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = "產品編號為: " + item.ID + ",賣場結束日期錯誤";
                        break;
                    }
                }
            }
            return result;
        }

        public ActionResponse<string> VerifyStetch(List<int> itemSketchID, string Userid)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            DB.TWSqlDBContext db_before = new TWSqlDBContext();
            DB.TWSellerPortalDBContext db_sellerPortal = new TWSellerPortalDBContext();
            List<DB.TWSQLDB.Models.ItemSketch> _listItemSketch = db_before.ItemSketch.Where(p => itemSketchID.Contains(p.ID)).ToList();
            List<DB.TWSQLDB.Models.ProductPropertySketch> _listProductPropertySketch = db_before.ProductPropertySketch.Where(p => itemSketchID.Contains(p.ItemSketchID)).ToList();
            List<DB.TWSQLDB.Models.ItemCategorySketch> _listItemCategorySketch = db_before.ItemCategorySketch.Where(p => itemSketchID.Contains(p.ItemSketchID)).ToList();
            ActionResponse<string> modelCheck = new ActionResponse<string>();
            
            foreach (var checkExist in _listItemSketch)
            {
                if (checkExist.Status == 99)
                {
                    result.IsSuccess = false;
                    result.Code = (int)ResponseCode.Error;
                    result.Msg = "產品編號: "  + checkExist.ID + "資料已送審，請更新畫面";
                    break;
                }
                result.IsSuccess = true;
            }
            if (result.IsSuccess == false)
            {
                return result;
            }
            

            modelCheck = VerifyStetchModelCheck(_listItemSketch);
            if (modelCheck.IsSuccess == true)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        var producttemp_productPropertytemp = ProductTempAndProductPropertyTemp(_listItemSketch, Userid/*, itemSketchID*/);
                        if (producttemp_productPropertytemp.Code == (int)ResponseCode.Success && producttemp_productPropertytemp.IsSuccess == true)
                        {
                            foreach (var item in _listItemSketch)
                            {
                                item.Status = 99;
                            }
                            db_before.SaveChanges();
                            scope.Complete();
                            result.IsSuccess = true;
                            result.Msg = "Success";
                            result.Code = (int)ResponseCode.Success;
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Msg = producttemp_productPropertytemp.Msg;
                            result.Code = (int)ResponseCode.Error;
                        }

                    }
                    catch (Exception error)
                    {
                        logger.Error("/ItemSketchService/VerifyStetch 發生例外 error: " + error.Message + " [ErrorStackTrace] " + error.StackTrace);
                        result.IsSuccess = false;
                        result.Msg = "發生例外";
                        result.Code = (int)ResponseCode.Error;
                    }
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = modelCheck.Msg;
                result.Code = (int)ResponseCode.Error;
            }
            return result;
        }
       
        /// <summary>
        /// 建立草稿
        /// </summary>
        /// <param name="itemSketchCell">商品資訊</param>
        /// <param name="saveProductProperty">商品屬性</param>
        /// <returns>成功失敗訊息</returns>
        public ActionResponse<List<string>> CreateItemSketch(List<ItemSketch> itemSketchCell)
        {
            log.Info("建立草稿開始");

            ActionResponse<List<string>> result = new ActionResponse<List<string>>();
            result.Body = new List<string>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            // 檢查輸入資料
            ActionResponse<List<string>> checkInputResult = CheckInput_ItemSketch(itemSketchCell);

            if (checkInputResult.IsSuccess)
            {
                // 建立失敗計數
                int createFailCount = 0;

                // 取得 AccountType
                ActionResponse<string> getAccountType = GetAccountType(itemSketchCell[0].Item.SellerID.Value);

                if (getAccountType.IsSuccess)
                {
                    // 使用 TransactionScope 保持資料交易完整性
                    using (TransactionScope scope = new TransactionScope())
                    {
                        foreach (ItemSketch itemSketch in itemSketchCell)
                        {
                            //成本去除小數點
                            if (itemSketch.Product.Cost != null)
                            {
                                itemSketch.Product.Cost = Math.Floor(itemSketch.Product.Cost.Value);
                            }
                            ActionResponse<int> saveItemSketchResult = SaveItemSketch(ItemSketchEditType.Create, itemSketch, getAccountType.Body);

                            if (saveItemSketchResult.IsSuccess == false)
                            {
                                createFailCount++;
                                result.IsSuccess = false;
                                log.Info(saveItemSketchResult.Msg);
                            }
                            else
                            {
                                result.Body.Add( saveItemSketchResult.Body.ToString());
                            }
                            
                        }

                        // 成功建立製造商 TransactionScope 結束
                        if (result.IsSuccess)
                        {
                            scope.Complete();
                            result.Msg = "儲存成功。";
                        }
                        else
                        {
                            scope.Dispose();
                            result.Msg = "儲存失敗。";
                        }
                    }                   

                }

                log.Info(string.Format("共 {0} 筆草稿資料，已成功建立 {1}  筆資料，失敗 {2} 筆資料。", itemSketchCell.Count, itemSketchCell.Count - createFailCount, createFailCount));

                foreach (string resultMessage in result.Body)
                {
                    log.Info(resultMessage);
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = "輸入資料有誤，請檢查。";
                result.Body = checkInputResult.Body;

                foreach (string errorMessage in checkInputResult.Body)
                {
                    log.Info(errorMessage);
                }
            }

            result.Code = SetResponseCode(result.IsSuccess);

            log.Info("建立草稿結束");

            return result;
        }

        /// <summary>
        /// 編輯草稿
        /// </summary>
        /// <param name="itemSketchCell">商品資訊</param>
        /// <returns>成功、失敗訊息</returns>
        public ActionResponse<List<string>> EditItemSketch(ItemSketchEditType editType, List<ItemSketch> itemSketchCell)
        {
            log.Info("編輯草稿開始");           

            ActionResponse<List<string>> result = new ActionResponse<List<string>>();
            result.Body = new List<string>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            DB.TWSqlDBContext dbFront = new TWSqlDBContext();

            // 檢查輸入資料
            ActionResponse<List<string>> checkInputResult = CheckInput_ItemSketch(itemSketchCell);

            if (checkInputResult.IsSuccess)
            {
                // 建立失敗計數
                int createFailCount = 0;

                // 取得 AccountType
                ActionResponse<string> getAccountType = GetAccountType(itemSketchCell[0].Item.SellerID.Value);

                if (getAccountType.IsSuccess)
                {
                    // 使用 TransactionScope 保持資料交易完整性
                    using (TransactionScope scope = new TransactionScope())
                    {
                        foreach (ItemSketch itemSketch in itemSketchCell)
                        {                          
                            ActionResponse<int> saveItemSketchResult = SaveItemSketch(editType, itemSketch, getAccountType.Body);

                            if (saveItemSketchResult.IsSuccess == false)
                            {
                                createFailCount++;
                                result.IsSuccess = false;
                                log.Info(saveItemSketchResult.Msg);
                            }
                        }

                        // 成功建立製造商 TransactionScope 結束
                        if (result.IsSuccess)
                        {
                            scope.Complete();
                            result.Msg = "儲存成功。";
                        }
                        else
                        {
                            scope.Dispose();
                            result.Msg = "儲存失敗。";
                        }
                    }
                }

                log.Info(string.Format("共 {0} 筆草稿資料，已成功修改 {1}  筆資料，失敗 {2} 筆資料。", itemSketchCell.Count, itemSketchCell.Count - createFailCount, createFailCount));
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = "輸入資料有誤，請檢查。";
                result.Body = checkInputResult.Body;

                foreach (string errorMessage in checkInputResult.Body)
                {
                    log.Info(errorMessage);
                }
            }

            result.Code = SetResponseCode(result.IsSuccess);

            log.Info("編輯草稿結束");

            return result;
        }

        /// <summary>
        /// 儲存草稿
        /// </summary>
        /// <param name="editType">儲存草稿類型</param>
        /// <param name="itemSketch">商品資訊</param>
        /// <param name="accountType">Account Type</param>
        /// <returns>成功、失敗訊息</returns>
        public ActionResponse<int> SaveItemSketch(ItemSketchEditType editType, ItemSketch itemSketch, string accountType)
        {
            ActionResponse<int> result = new ActionResponse<int>();
            //result.Body = false;
            result.IsSuccess = true;
            result.Msg = string.Empty;
            //成本去除小數點
            if (itemSketch.Product.Cost != null)
            {
                itemSketch.Product.Cost = Math.Floor(itemSketch.Product.Cost.Value);
            }
            #region 填寫草稿 DB Model

            DB.TWSQLDB.Models.ItemSketch itemSketch_DB = new DB.TWSQLDB.Models.ItemSketch();

            // 讀取已存在的草稿內容
            if (editType == ItemSketchEditType.DetailEdit || editType == ItemSketchEditType.ListEdit)
            {
                DB.TWSqlDBContext dbFront = new TWSqlDBContext();

                try
                {
                    itemSketch_DB = dbFront.ItemSketch.Where(x => x.ID == itemSketch.ID).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    log.Info(string.Format("儲存草稿失敗(expecion); ItemSketchID = {0}; ErrorMessage = {1}; StackTrace = {2}.", itemSketch.ID, ex.Message, ex.StackTrace));
                    result.IsSuccess = false;
                }

                if (itemSketch_DB == null)
                {
                    result.IsSuccess = false;
                    log.Info("讀取資料庫失敗，無法修改草稿變更資料，儲存草稿失敗。");
                }
            }

            // 建立或更新草稿內容
            if (result.IsSuccess)
            {
                ActionResponse<DB.TWSQLDB.Models.ItemSketch> makeItemSketch_DB_Result = MakeItemSketch_DB(editType, itemSketch_DB, itemSketch, accountType);

                if (makeItemSketch_DB_Result.IsSuccess)
                {
                    itemSketch_DB = makeItemSketch_DB_Result.Body;
                }
                else
                {
                    // 組合草稿 DB Model 失敗
                    result.IsSuccess = false;
                }
            }

            #endregion 填寫草稿 DB Model

            #region 儲存草稿

            #region 不維護欄位給預設值

            itemSketch_DB.ItemQty = 0;

            #endregion
            
            if (result.IsSuccess)
            {
                DB.TWSqlDBContext _dbFront = new TWSqlDBContext();

                switch (editType)
                {
                    case ItemSketchEditType.Create:
                        {
                            _dbFront.ItemSketch.Add(itemSketch_DB);
                            break;
                        }
                    case ItemSketchEditType.DetailEdit:
                    case ItemSketchEditType.ListEdit:
                        {
                            _dbFront.Entry(itemSketch_DB).State = System.Data.EntityState.Modified;
                            break;
                        }
                    default:
                        {
                            result.IsSuccess = false;
                            log.Info("儲存草稿類型錯誤。");
                            break;
                        }
                }

                if (result.IsSuccess)
                {
                    try
                    {
                        _dbFront.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        log.Info(string.Format("儲存草稿失敗(exception); ErrorMessage = {0}; StackTrace = {1}.", ex.Message, ex.StackTrace));
                    }
                }
            }

            #endregion 儲存草稿

            if (result.IsSuccess && (editType == ItemSketchEditType.Create || editType == ItemSketchEditType.DetailEdit))
            {
                #region 儲存跨分類

                if (result.IsSuccess)
                {
                    // 先清除原有跨分類資訊，再新建
                    if (editType == ItemSketchEditType.DetailEdit)
                    {
                        // 取得目前跨分類資訊
                        ActionResponse<List<DB.TWSQLDB.Models.ItemCategorySketch>> getItemCategorySketch = GetItemCategorySketch(itemSketch_DB.ID);

                        if (getItemCategorySketch.IsSuccess)
                        {
                            // 刪除目前跨分類資訊
                            ActionResponse<bool> deleteItemCategorySketch = DeleteItemCategorySketch(getItemCategorySketch.Body);

                            if (deleteItemCategorySketch.IsSuccess == false)
                            {
                                result.IsSuccess = false;
                                result.Msg += deleteItemCategorySketch.Msg;
                            }
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Msg += getItemCategorySketch.Msg;
                        }
                    }

                    if (result.IsSuccess)
                    {
                        List<int> subCategoryIDCell = new List<int>();

                        if (itemSketch.ItemCategory.SubCategoryID_1_Layer2 != null && itemSketch.ItemCategory.SubCategoryID_1_Layer2.Value > 0)
                        {
                            subCategoryIDCell.Add(itemSketch.ItemCategory.SubCategoryID_1_Layer2.Value);
                        }

                        if (itemSketch.ItemCategory.SubCategoryID_2_Layer2 != null && itemSketch.ItemCategory.SubCategoryID_2_Layer2.Value > 0)
                        {
                            subCategoryIDCell.Add(itemSketch.ItemCategory.SubCategoryID_2_Layer2.Value);
                        }

                        if (subCategoryIDCell.Count > 0)
                        {
                            // 建立失敗計數
                            int createFailCount = 0;

                            foreach (int subCategoryID in subCategoryIDCell)
                            {
                                ActionResponse<bool> saveItemCategory = SaveItemCategorySketch(itemSketch_DB.ID, subCategoryID, itemSketch.CreateAndUpdate.UpdateUser.ToString());

                                if (saveItemCategory.IsSuccess == false)
                                {
                                    createFailCount++;
                                    result.IsSuccess = false;
                                }
                            }

                            if (createFailCount == 0)
                            {
                                result.Body = 0;
                            }

                            log.Info(string.Format("共有 {0} 筆跨分類資訊，已成功建立 {1} 筆，失敗 {2} 筆。", subCategoryIDCell.Count, subCategoryIDCell.Count - createFailCount, createFailCount));
                        }
                    }
                }

                #endregion 儲存跨分類
                
                #region 儲存圖片

                if (result.IsSuccess)
                {
                    
                    ActionResponse<bool> saveImageResult = imageService.ImageProcess(itemSketch.Product.PicPatch_Edit, "pic\\itemSketch", "pic\\pic\\itemSketch", itemSketch_DB.ID);

                    if (saveImageResult.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        log.Info(string.Format("儲存草稿圖片失敗; API Message = {0}", saveImageResult.Msg));
                    }
                }

                #endregion 圖片處理

                #region 儲存商品屬性

                if (result.IsSuccess)
                {
                    ProductPorpertySketchService productPorpertySketchService = new ProductPorpertySketchService();

                    ActionResponse<string> saveResult = productPorpertySketchService.SaveProductPropertyClick(itemSketch.SaveProductPropertyList, itemSketch_DB.ID, itemSketch.CreateAndUpdate.UpdateUser);

                    if (saveResult.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        log.Info(string.Format("儲存商品屬性失敗; Message = {0}.", saveResult.Msg));
                    }
                }

                #endregion 儲存商品屬性
            }

            result.Code = SetResponseCode(result.IsSuccess);

            // 儲存草稿成功，回傳草稿 ID
            if (result.IsSuccess)
            {
                itemSketch.ID = itemSketch_DB.ID;
                result.Body = itemSketch_DB.ID;
                log.Info(string.Format("儲存草稿成功; ItemSketchID = {0}.", itemSketch_DB.ID.ToString()));
            }

            return result;
        }

        #region 儲存跨分類資訊

        /// <summary>
        /// 儲存跨分類資訊
        /// </summary>
        /// <param name="itemSketchID">草稿 ID</param>
        /// <param name="categoryID">分類 ID</param>
        /// <param name="userID">建立或更新人 ID</param>
        /// <returns>成功、失敗訊息</returns>
        private ActionResponse<bool> SaveItemCategorySketch(int itemSketchID, int categoryID, string userID)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.Body = false;
            result.IsSuccess = true;
            result.Msg = string.Empty;

            ActionResponse<DB.TWSQLDB.Models.ItemCategorySketch> itemCategorySketch_DB_Result = MakeItemCategorySketch_DB(itemSketchID, categoryID, userID);

            if (itemCategorySketch_DB_Result.IsSuccess)
            {
                DB.TWSqlDBContext dbFront = new TWSqlDBContext();
                dbFront.ItemCategorySketch.Add(itemCategorySketch_DB_Result.Body);

                try
                {
                    dbFront.SaveChanges();
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    log.Info("儲存跨分類失敗(exception)：" + ex.ToString());
                    result.Msg = "發生 exception，儲存跨分類失敗。";
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = itemCategorySketch_DB_Result.Msg;
            }

            if (result.IsSuccess)
            {
                result.Body = true;
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 取得跨分類資訊
        /// </summary>
        /// <param name="itemSketchID">草稿 ID</param>
        /// <returns>跨分類資訊</returns>
        private ActionResponse<List<DB.TWSQLDB.Models.ItemCategorySketch>> GetItemCategorySketch(int itemSketchID)
        {
            ActionResponse<List<DB.TWSQLDB.Models.ItemCategorySketch>> result = new ActionResponse<List<DB.TWSQLDB.Models.ItemCategorySketch>>();
            result.Body = new List<DB.TWSQLDB.Models.ItemCategorySketch>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            DB.TWSqlDBContext dbFront = new TWSqlDBContext();

            try
            {
                result.Body = dbFront.ItemCategorySketch.Where(x => x.ItemSketchID == itemSketchID).ToList();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = "取得跨分類資訊失敗。";
                log.Info(string.Format("取得跨分類資訊失敗(expection); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 刪除跨分類資訊
        /// </summary>
        /// <param name="itemCategorySketchCell">跨分類資訊</param>
        /// <returns>成功、失敗訊息</returns>
        private ActionResponse<bool> DeleteItemCategorySketch(List<DB.TWSQLDB.Models.ItemCategorySketch> itemCategorySketchCell)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.Body = false;
            result.IsSuccess = true;
            result.Msg = string.Empty;

            DB.TWSqlDBContext dbFront = new TWSqlDBContext();

            try
            {
                foreach (DB.TWSQLDB.Models.ItemCategorySketch itemCategorySketch in itemCategorySketchCell)
                {
                    DB.TWSQLDB.Models.ItemCategorySketch deleteItem = dbFront.ItemCategorySketch.Where(x => x.ItemSketchID == itemCategorySketch.ItemSketchID && x.CategoryID == itemCategorySketch.CategoryID).FirstOrDefault();
                    dbFront.ItemCategorySketch.Remove(deleteItem);
                }

                dbFront.SaveChanges();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = "刪除跨分類資訊失敗。";
                log.Info(string.Format("刪除跨分類資訊失敗(expection); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            if (result.IsSuccess)
            {
                foreach (DB.TWSQLDB.Models.ItemCategorySketch itemCategorySketch in itemCategorySketchCell)
                {
                    log.Info(string.Format("刪除跨分類資訊：ItemSketchID = {0}, CategoryID = {1}, CreateUser = {2}, CreateDate = {3}, UpdateUser = {4}, UpdateDate = {5}",
                        itemCategorySketch.ItemSketchID, itemCategorySketch.CategoryID, itemCategorySketch.CreateUser, itemCategorySketch.CreateDate, itemCategorySketch.UpdateUser, itemCategorySketch.UpdateDate));
                }
                
                result.Body = true;
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        #endregion 儲存跨分類資訊

        #region 輸入檢查

        /// <summary>
        /// 檢查草稿查詢條件輸入資料
        /// </summary>
        /// <param name="condition">草稿查詢條件</param>
        /// <returns>檢查結果</returns>
        private ActionResponse<bool> CheckInput_ItemSketchSearchCondition(ItemSketchSearchCondition condition)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.Body = false;
            result.IsSuccess = true;
            result.Msg = string.Empty;

            #region 檢查 SellerID

            // 檢查 SellerID 是否存在 (但在有管理權限，且 SellerID 為 0 時不檢查，此時將顯示所有 SellerID 的資料)
            if (!(condition.IsAdmin && condition.SellerID == 0))
            {
                // 檢查資料庫內是否有該商家的資料
                ActionResponse<bool> checkSellerExistResult = CheckSellerExist(condition.SellerID);

                if (!(checkSellerExistResult.IsSuccess && checkSellerExistResult.Body))
                {
                    result.IsSuccess = false;

                    if (checkSellerExistResult.IsSuccess)
                    {
                        result.Msg += "查無此 SellerID 資料。";
                    }
                    else
                    {
                        result.Msg += "發生 exctption，查詢 SellerID 失敗。";
                    }
                }
            }

            #endregion 檢查 SellerID

            #region 檢查日期

            // 檢查創建日期如果是選指定日期或日期範圍時，是否有輸入迄始日期、迄止日期
            if (condition.createDate == ItemSketchCreateDate.SpecifyDate || condition.createDate == ItemSketchCreateDate.DateRange)
            {
                if (condition.startDate == null || condition.endDate == null)
                {
                    result.IsSuccess = false;

                    if (condition.startDate == null)
                    {
                        result.Msg += "未輸入迄始日期。";
                    }

                    if (condition.endDate == null)
                    {
                        result.Msg += "未輸入迄止日期。";
                    }
                }

                if (condition.startDate > condition.endDate)
                {
                    result.IsSuccess = false;
                    result.Msg += "迄始日期不可在迄止日期之後。";
                }   
            }

            #endregion 檢查日期

            #region 檢查分頁資訊

            //if (condition.PageIndex != null || condition.PageSize != null)
            //{
            //    if (condition.PageIndex == null)
            //    {
            //        result.IsSuccess = false;
            //        result.Msg += "未輸入分頁頁數。";
            //    }
            //    else if (condition.PageIndex < 0)
            //    {
            //        result.IsSuccess = false;
            //        result.Msg += "分頁頁數不可小於 0。";
            //    }

            //    if (condition.PageSize == null)
            //    {
            //        result.IsSuccess = false;
            //        result.Msg += "未輸入資料筆數。";
            //    }
            //    else if (condition.PageSize <= 0)
            //    {
            //        result.IsSuccess = false;
            //        result.Msg += "資料筆數不可小於等於 0。";
            //    }
            //}

            #endregion 檢查分頁資訊

            if (result.IsSuccess && result.Msg == string.Empty)
            {
                result.Body = true;
                result.Code = (int)ResponseCode.Success;
            }
            else
            {
                result.Code = (int)ResponseCode.Error;
            }

            return result;
        }

        /// <summary>
        /// 檢查商品資訊輸入資料
        /// </summary>
        /// <param name="itemSketchCell">商品資訊</param>
        /// <returns>檢查結果</returns>
        public ActionResponse<List<string>> CheckInput_ItemSketch(List<ItemSketch> itemSketchCell)
        {
            ActionResponse<List<string>> result = new ActionResponse<List<string>>();
            result.Body = new List<string>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            if (itemSketchCell.Count() > 0)
            {
                int itemSketchCellCount = 0;
                foreach (ItemSketch itemSketch in itemSketchCell)
                {
                    itemSketchCellCount++;
                    string errorMessage = string.Empty;

                    #region 因查詢 DB 用，或其它欄位給值判斷用列為必填項目

                    #region 檢查 UserID

                    ActionResponse<bool> checkUserIDResult_Create = CheckUserID(itemSketch.CreateAndUpdate.CreateUser);
                    ActionResponse<bool> checkUserIDResult_Update = CheckUserID(itemSketch.CreateAndUpdate.UpdateUser);

                    if (checkUserIDResult_Create.IsSuccess == false && checkUserIDResult_Update.IsSuccess == false)
                    {
                        errorMessage = "未輸入 CreateUser 及 UpdateUser 資訊。";
                    }
                    else
                    {
                        if (checkUserIDResult_Create.IsSuccess && checkUserIDResult_Update.IsSuccess)
                        {
                            if (itemSketch.CreateAndUpdate.CreateUser != itemSketch.CreateAndUpdate.UpdateUser)
                            {
                                errorMessage = "CreateUser 及 UpdateUser 值不一致。";
                            }
                        }
                        else
                        {
                            if (checkUserIDResult_Create.IsSuccess == false)
                            {
                                itemSketch.CreateAndUpdate.CreateUser = itemSketch.CreateAndUpdate.UpdateUser;
                            }

                            if (checkUserIDResult_Update.IsSuccess == false)
                            {
                                itemSketch.CreateAndUpdate.UpdateUser = itemSketch.CreateAndUpdate.CreateUser;
                            }
                        }
                    }

                    #endregion 檢查 UserID

                    #region 檢查 ShipType

                    ActionResponse<bool> checkShipTypeResult = CheckShipType(itemSketch.Item.ShipType);

                    if (checkShipTypeResult.IsSuccess == false)
                    {
                        errorMessage = checkShipTypeResult.Msg;
                    }

                    #endregion 檢查 ShipType

                    #region 檢查 Seller ID

                    ActionResponse<bool> checkSellerIDResult = CheckSellerID(itemSketch.Item.SellerID.Value);

                    if (checkSellerIDResult.IsSuccess == false)
                    {
                        errorMessage = checkSellerIDResult.Msg;
                    }

                    #endregion 檢查 Seller ID

                    #endregion 因查詢 DB 用，或其它欄位給值判斷用列為必填項目

                    #region 因畫面上呈現的選項沒有空值，而列為必填項目

                    #region 檢查 IsNew

                    ActionResponse<bool> checkIsNewResult = CheckIsNew(itemSketch.Item.IsNew);

                    if (checkIsNewResult.IsSuccess == false)
                    {
                        errorMessage = checkIsNewResult.Msg;
                    }

                    #endregion 檢查 IsNew

                    #region 檢查 Is18

                    ActionResponse<bool> checkIs18Result = CheckIs18(itemSketch.Product.Is18);

                    if (checkIs18Result.IsSuccess == false)
                    {
                        errorMessage = checkIs18Result.Msg;
                    }

                    #endregion 檢查 Is18

                    #region 檢查 IsChokingDanger

                    ActionResponse<bool> checkIsChokingDangerResult = CheckIsChokingDanger(itemSketch.Product.IsChokingDanger);

                    if (checkIsChokingDangerResult.IsSuccess == false)
                    {
                        errorMessage = checkIsChokingDangerResult.Msg;
                    }

                    #endregion 檢查 IsChokingDanger

                    #region 檢查 IsShipDanger

                    ActionResponse<bool> checkIsShipDangerResult = CheckIsShipDanger(itemSketch.Product.IsShipDanger);

                    if (checkIsShipDangerResult.IsSuccess == false)
                    {
                        errorMessage = checkIsShipDangerResult.Msg;
                    }

                    #endregion 檢查 IsShipDanger

                    #region 檢查 ItemPackage

                    ActionResponse<bool> checkItemPackageResult = CheckItemPackage(itemSketch.Item.ItemPackage);

                    if (checkItemPackageResult.IsSuccess == false)
                    {
                        errorMessage = checkItemPackageResult.Msg;
                    }

                    #endregion 檢查 ItemPackage

                    #endregion 因畫面上呈現的選項沒有空值，而列為必填項目

                    #region 輸入欄位最多可填字數檢查

                    #region 檢查 BarCode

                    ActionResponse<bool> checkBarCodeResult = CheckBarCode(itemSketch.Product.BarCode);

                    if (checkBarCodeResult.IsSuccess == false)
                    {
                        errorMessage = checkBarCodeResult.Msg;
                    }

                    #endregion 檢查 BarCode

                    #region 檢查 MenufacturePartNum

                    ActionResponse<bool> checkMenufacturePartNumResult = CheckMenufacturePartNum(itemSketch.Product.MenufacturePartNum);

                    if (checkMenufacturePartNumResult.IsSuccess == false)
                    {
                        errorMessage = checkMenufacturePartNumResult.Msg;
                    }

                    #endregion 檢查 MenufacturePartNum

                    #region 檢查 Model

                    ActionResponse<bool> checkModelResult = CheckModel(itemSketch.Product.Model);

                    if (checkModelResult.IsSuccess == false)
                    {
                        errorMessage = checkModelResult.Msg;
                    }

                    #endregion 檢查 Model

                    #region 檢查 UPC

                    ActionResponse<bool> checkUPCResult = CheckUPC(itemSketch.Product.UPC);

                    if (checkUPCResult.IsSuccess == false)
                    {
                        errorMessage = checkUPCResult.Msg;
                    }

                    #endregion 檢查 UPC

                    #region 檢查 DelvDate

                    ActionResponse<bool> checkDelvDateResult = CheckDelvDate(itemSketch.Item.DelvDate);

                    if (checkDelvDateResult.IsSuccess == false)
                    {
                        errorMessage = checkDelvDateResult.Msg;
                    }

                    #endregion 檢查 DelvDate

                    #endregion 輸入欄位最多可填字數檢查

                    #region 檢查毛利率

                    ActionResponse<bool> checkCheckGrossProfitResult = CheckGrossProfit(itemSketch.Product.Cost, itemSketch.Item.PriceCash);

                    if (checkCheckGrossProfitResult.IsSuccess == false)
                    {
                        errorMessage = checkCheckGrossProfitResult.Msg;
                    }

                    #endregion 檢查毛利率

                    #region 填寫檢查失敗錯誤訊息

                    if (string.IsNullOrEmpty(errorMessage) == false)
                    {
                        result.Body.Add(errorMessage);

                        #region 錯誤訊息抬頭、連接符號

                        if (string.IsNullOrEmpty(result.Msg))
                        {
                            if (itemSketch.ID > 0)
                            {
                                result.Msg += "檢查失敗(資料順序引數)：";
                            }
                            else
                            {
                                result.Msg += "檢查失敗草稿 ID：";
                            }
                        }
                        else
                        {
                            result.Msg += "、";
                        }

                        #endregion 錯誤訊息抬頭、連接符號

                        #region 錯誤資料索引值

                        if (itemSketch.ID > 0)
                        {
                            result.Msg += itemSketchCellCount.ToString();

                            // 填入 log 前，先加上資料的索引值
                            errorMessage = string.Format("(商品資訊第 {0} 筆資料) {1}", itemSketchCellCount, errorMessage);
                        }
                        else
                        {
                            result.Msg += itemSketch.ID.ToString();

                            // 填入 log 前，先加上資料的索引值
                            errorMessage = string.Format("(ItemSketch = {0}) {1}", itemSketch.ID, errorMessage);
                        }

                        #endregion 錯誤資料索引值

                        log.Info(string.Format("草稿儲存檢查失敗：{0}。", errorMessage));
                    }

                    #endregion 填寫檢查失敗錯誤訊息
                }

                if (result.Body.Count > 0 || string.IsNullOrEmpty(result.Msg) == false)
                {
                    result.IsSuccess = false;
                }
            }
            else
            {
                result.IsSuccess = false;
                log.Info("未輸入商品資訊。");
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 檢查條碼
        /// </summary>
        /// <param name="barCode">條碼</param>
        /// <returns>檢查結果</returns>
        private ActionResponse<bool> CheckBarCode(string barCode)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.Body = false;
            result.IsSuccess = false;
            result.Msg = string.Empty;

            if (!string.IsNullOrEmpty(barCode))
            {
                if (barCode.Length > 50)
                {
                    result.Msg += "條碼字數上限為 50 字。";
                }
            }

            if (result.Msg == string.Empty)
            {
                result.Body = true;
                result.IsSuccess = true;
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 檢查商品主分類 ID
        /// </summary>
        /// <param name="categoryID">商品主分類 ID</param>
        /// <returns>檢查結果</returns>
        private ActionResponse<bool> CheckCategoryID(int? categoryID)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.Body = false;
            result.IsSuccess = false;
            result.Msg = string.Empty;

            if (categoryID == null)
            {
                result.Msg += "商品主分類為必填。";
            }
            else if (categoryID <= 0)
            {
                result.Msg += "商品主分類 ID 不可小於等於 0。";
            }

            if (result.Msg == string.Empty)
            {
                result.Body = true;
                result.IsSuccess = true;
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }
        public ActionResponse<bool> CheckCategoryParentId(int main, List<int> checkcategoryid)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            int MainCategoryID_Layer2, SubCategoryID_1_Layer2, SubCategoryID_2_Layer2;
            MainCategoryID_Layer2 = CategoryService.LoadCategoryParentId(CategoryService.LoadCategoryParentId(main));
            int _comTemp1 = 0;
            if (checkcategoryid.Count == 1)
            {
  
                foreach (var item in checkcategoryid)
                {
                    try
                    {
                        _comTemp1 = CategoryService.LoadCategoryParentId(CategoryService.LoadCategoryParentId(item));
                    }
                    catch (Exception error)
                    {
                        result.IsSuccess = false;
                        result.Msg = "沒有這個類別";
                        break;
                    }
                    if (MainCategoryID_Layer2 != _comTemp1)
                    {
                        result.IsSuccess = false;
                        result.Msg = "商品類別 ID or 跨分類 1 or 跨分類 不在同一分類裡面";
                        break;
                    }
                }
            }
            else if (checkcategoryid.Count == 2)
            {
                int _temp1 = 0, _temp2 = 0;
                try
                {
                    _temp1 = CategoryService.LoadCategoryParentId(CategoryService.LoadCategoryParentId(checkcategoryid[0]));
                    _temp2 = CategoryService.LoadCategoryParentId(CategoryService.LoadCategoryParentId(checkcategoryid[1]));
                }
                catch (Exception error)
                {
                    result.IsSuccess = false;
                    result.Msg = "沒有這個類別";
                }
                if (result.IsSuccess == false)
                {
                    return result;
                }
                if (_temp1 != _temp2)
                {
                    result.IsSuccess = false;
                    result.Msg = "商品類別 ID or 跨分類 1 or 跨分類 不在同一分類裡面";
                }
                else
                {
                    if (_temp1 != MainCategoryID_Layer2)
                    {
                        result.IsSuccess = false;
                        result.Msg = "商品類別 ID or 跨分類 1 or 跨分類 不在同一分類裡面";
                    }
                }
            }
            return result;
            
        }
        /// <summary>
        /// 檢查跨分類 ID
        /// </summary>
        /// <param name="categoryID">跨分類 ID</param>
        /// <returns>檢查結果</returns>
        private ActionResponse<bool> CheckItemCategorySketchID(int? categoryID)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.Body = false;
            result.IsSuccess = false;
            result.Msg = string.Empty;

            if (categoryID != null)
            {
                if (categoryID <= 0)
                {
                    result.Msg += "跨分類 ID 不可小於等於 0。";
                }

                
            }

            if (result.Msg == string.Empty)
            {
                result.Body = true;
                result.IsSuccess = true;
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 檢查到貨天數
        /// </summary>
        /// <param name="delvDate">到貨天數</param>
        /// <returns>檢查結果</returns>
        private ActionResponse<bool> CheckDelvDate(string delvDate)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.Body = false;
            result.IsSuccess = false;
            result.Msg = string.Empty;

            if (!string.IsNullOrEmpty(delvDate))
            {
                if (delvDate.Length > 50)
                {
                    result.Msg += "到貨天數字數上限為 50 字。";
                }
            }

            if (result.Msg == string.Empty)
            {
                result.Body = true;
                result.IsSuccess = true;
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 檢查毛利率
        /// </summary>
        /// <param name="cost">成本</param>
        /// <param name="priceCash">售價</param>
        /// <returns>檢查結果</returns>
        private ActionResponse<bool> CheckGrossProfit(decimal? cost, decimal? priceCash)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.Body = false;
            result.IsSuccess = false;
            result.Msg = string.Empty;

            if (cost == null)
            {
                cost = 0;
            }

            if (priceCash == null)
            {
                priceCash = 0;
            }

            if (cost > priceCash)
            {
                result.Body = false;
                result.IsSuccess = false;
                result.Msg += "毛利率為負數，請重新設定售價或成本。";
            }
            else
            {
                result.Body = true;
                result.IsSuccess = true;
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 檢查是否為18歲商品
        /// </summary>
        /// <param name="is18">是否為18歲商品</param>
        /// <returns>檢查結果</returns>
        private ActionResponse<bool> CheckIs18(string is18)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.Body = false;
            result.IsSuccess = false;
            result.Msg = string.Empty;

            if (string.IsNullOrEmpty(is18))
            {
                result.Msg = "是否為18歲商品為必填。";
            }
            else
            {
                if (is18.Length != 1)
                {
                    result.Msg = "是否為18歲商品字數不可大於 1。";
                }
                else if (is18 != "Y" && is18 != "N")
                {
                    result.Msg = "是否為18歲商品輸入值錯誤。";
                }
            }

            if (result.Msg == string.Empty)
            {
                result.Body = true;
                result.IsSuccess = true;
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 檢查是否有窒息危險
        /// </summary>
        /// <param name="isChokingDanger">是否有窒息危險</param>
        /// <returns>檢查結果</returns>
        private ActionResponse<bool> CheckIsChokingDanger(string isChokingDanger)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.Body = false;
            result.IsSuccess = false;
            result.Msg = string.Empty;

            if (string.IsNullOrEmpty(isChokingDanger))
            {
                result.Msg = "是否有窒息危險為必填。";
            }
            else
            {
                if (isChokingDanger.Length != 1)
                {
                    result.Msg = "是否有窒息危險字數不可大於 1。";
                }
                else if (isChokingDanger != "Y" && isChokingDanger != "N")
                {
                    result.Msg = "是否有窒息危險輸入值錯誤。";
                }
            }

            if (result.Msg == string.Empty)
            {
                result.Body = true;
                result.IsSuccess = true;
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 檢查商品成色
        /// </summary>
        /// <param name="isNew">商品成色</param>
        /// <returns>檢查結果</returns>
        private ActionResponse<bool> CheckIsNew(string isNew)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.Body = false;
            result.IsSuccess = false;
            result.Msg = string.Empty;

            if (string.IsNullOrEmpty(isNew))
            {
                result.Msg = "商品成色為必填。";
            }
            else
            {
                if (isNew.Length != 1)
                {
                    result.Msg = "商品成色字數不可大於 1。";
                }
                else if (isNew != "Y" && isNew != "N")
                {
                    result.Msg = "商品成色輸入值錯誤。";
                }
            }

            if (result.Msg == string.Empty)
            {
                result.Body = true;
                result.IsSuccess = true;
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 檢查是否有遞送危險
        /// </summary>
        /// <param name="isShipDanger">是否有遞送危險</param>
        /// <returns>檢查結果</returns>
        private ActionResponse<bool> CheckIsShipDanger(string isShipDanger)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.Body = false;
            result.IsSuccess = false;
            result.Msg = string.Empty;

            if (string.IsNullOrEmpty(isShipDanger))
            {
                result.Msg = "是否有遞送危險為必填。";
            }
            else
            {
                if (isShipDanger.Length != 1)
                {
                    result.Msg = "是否有遞送危險字數不可大於 1。";
                }
                else if (isShipDanger != "Y" && isShipDanger != "N")
                {
                    result.Msg = "是否有遞送危險輸入值錯誤。";
                }
            }

            if (result.Msg == string.Empty)
            {
                result.Body = true;
                result.IsSuccess = true;
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 檢查商品包裝
        /// </summary>
        /// <param name="itemPackage">商品包裝</param>
        /// <returns>檢查結果</returns>
        private ActionResponse<bool> CheckItemPackage(string itemPackage)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.Body = false;
            result.IsSuccess = false;
            result.Msg = string.Empty;

            if (string.IsNullOrEmpty(itemPackage))
            {
                result.Msg = "商品包裝為必填。";
            }
            else
            {
                if (itemPackage.Length != 1)
                {
                    result.Msg = "商品包裝字數不可大於 1。";
                }
                else if (itemPackage != "0" && itemPackage != "1")
                {
                    result.Msg = "商品包裝輸入值錯誤。";
                }
            }

            if (result.Msg == string.Empty)
            {
                result.Body = true;
                result.IsSuccess = true;
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 檢查製造商 ID
        /// </summary>
        /// <param name="manufactureID">製造商 ID</param>
        /// <returns>檢查結果</returns>
        private ActionResponse<bool> CheckManufactureID(int? manufactureID)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.Body = false;
            result.IsSuccess = false;
            result.Msg = string.Empty;

            if (manufactureID == null)
            {
                result.Msg += "製造商為必填。";
            }
            else if (manufactureID <= 0)
            {
                result.Msg += "製造商 ID 不可小於等於 0。";
            }

            if (result.Msg == string.Empty)
            {
                result.Body = true;
                result.IsSuccess = true;
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 檢查商家商品編號
        /// </summary>
        /// <param name="menufacturePartNum">商家商品編號</param>
        /// <returns>檢查結果</returns>
        private ActionResponse<bool> CheckMenufacturePartNum(string menufacturePartNum)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.Body = false;
            result.IsSuccess = false;
            result.Msg = string.Empty;

            if (!string.IsNullOrEmpty(menufacturePartNum))
            {
                if (menufacturePartNum.Length > 150)
                {
                    result.Msg += "商家商品編號字數上限為 150 字。";
                }
            }

            if (result.Msg == string.Empty)
            {
                result.Body = true;
                result.IsSuccess = true;
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 檢查商品型號
        /// </summary>
        /// <param name="model">商品型號</param>
        /// <returns>檢查結果</returns>
        private ActionResponse<bool> CheckModel(string model)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.Body = false;
            result.IsSuccess = false;
            result.Msg = string.Empty;

            if (!string.IsNullOrEmpty(model))
            {
                if (model.Length > 30)
                {
                    result.Msg += "商品型號字數上限為 30 字。";
                }
            }

            if (result.Msg == string.Empty)
            {
                result.Body = true;
                result.IsSuccess = true;
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 檢查資料庫內是否有該商家的資料
        /// </summary>
        /// <param name="sellerID">商家 ID</param>
        /// <returns>查詢結果</returns>
        private ActionResponse<bool> CheckSellerExist(int sellerID)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.Body = false;
            result.IsSuccess = false;
            result.Msg = string.Empty;

            DB.TWSqlDBContext dbFront = new TWSqlDBContext();
            result.IsSuccess = true;

            if (sellerID > 0)
            {
                try
                {
                    result.Body = dbFront.ItemSketch.Where(x => x.SellerID == sellerID).Any();
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    log.Info(string.Format("查詢 SellerID 失敗exctption; ErrorMessage = {0}; StackTrace = {1}." + ex.Message, ex.StackTrace));
                }
            }
            else
            {
                result.IsSuccess = false;
                log.Info("商家 ID 不可以小於等於 0.");
            }

            if (result.Msg == string.Empty)
            {
                result.Body = true;
                result.IsSuccess = true;
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 檢查商家 ID
        /// </summary>
        /// <param name="sellerID">商家 ID</param>
        /// <returns>檢查結果</returns>
        private ActionResponse<bool> CheckSellerID(int? sellerID)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.Body = false;
            result.IsSuccess = false;
            result.Msg = string.Empty;

            DB.TWSqlDBContext dbFront = new TWSqlDBContext();
            result.IsSuccess = true;

            if (sellerID == null)
            {
                result.Msg += "商家 ID 為必填。";
            }
            else
            {
                if (sellerID < 0)
                {
                    result.Msg += "商家 ID 不可小於 0。";
                }
            }

            if (result.Msg == string.Empty)
            {
                result.Body = true;
                result.IsSuccess = true;
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        } 

        /// <summary>
        /// 檢查運送類型
        /// </summary>
        /// <param name="shipType">運送類型</param>
        /// <returns>檢查結果</returns>
        private ActionResponse<bool> CheckShipType(string shipType)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.Body = false;
            result.IsSuccess = false;
            result.Msg = string.Empty;

            DB.TWSqlDBContext dbFront = new TWSqlDBContext();
            result.IsSuccess = true;

            if (string.IsNullOrEmpty(shipType))
            {
                result.Msg = "運送類型為必填。";
            }
            else
            {
                if (shipType.Length != 1)
                {
                    result.Msg = "運送類型字數不可大於 1。";
                }
                else if (shipType != "S" && shipType != "N")
                {
                    result.Msg = "運送類型輸入值錯誤。";
                }
            }

            if (result.Msg == string.Empty)
            {
                result.Body = true;
                result.IsSuccess = true;
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 檢查 UPC
        /// </summary>
        /// <param name="upc">UPC</param>
        /// <returns>檢查結果</returns>
        private ActionResponse<bool> CheckUPC(string upc)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.Body = false;
            result.IsSuccess = false;
            result.Msg = string.Empty;

            if (!string.IsNullOrEmpty(upc))
            {
                if (upc.Length > 15)
                {
                    result.Msg += "UPC字數上限為 15 字。";
                }
            }

            if (result.Msg == string.Empty)
            {
                result.Body = true;
                result.IsSuccess = true;
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 檢查 User ID
        /// </summary>
        /// <param name="userID">User ID</param>
        /// <returns>檢查結果</returns>
        private ActionResponse<bool> CheckUserID(int userID)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.Body = false;
            result.IsSuccess = false;
            result.Msg = string.Empty;

            if (userID <= 0)
            {
                result.Msg += "UserID ID 不可小於等於 0。";
            }

            if (result.Msg == string.Empty)
            {
                result.Body = true;
                result.IsSuccess = true;
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 檢查商品保固期
        /// </summary>
        /// <param name="warranty">商品保固期</param>
        /// <returns>檢查結果</returns>
        private ActionResponse<bool> CheckWarranty(int warranty)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.Body = false;
            result.IsSuccess = false;
            result.Msg = string.Empty;

            if (warranty < 0)
            {
                result.Msg += "商品保固期輸入值不可小於 0。";
            }

            if (result.Msg == string.Empty)
            {
                result.Body = true;
                result.IsSuccess = true;
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        #endregion 輸入檢查

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

            if (result.IsSuccess && result.Body.Count() == 0)
            {
                result.IsSuccess = false;
                result.Msg = "查無資料。";
                log.Info("查詢 Category 失敗：查無資料。");
            }

            if (result.IsSuccess)
            {
                result.Code = (int)ResponseCode.Success;
                result.Msg = "已成功查到 " + result.Body.Count.ToString() + " 筆資料。";
                log.Info(result.Msg);
            }
            else
            {
                result.Code = (int)ResponseCode.Error;
            }

            return result;
        }

        #endregion 取得分類資訊

        #region 組 model

        /// <summary>
        /// 組合跨分類 Model
        /// </summary>
        /// <param name="itemSketchID">草稿 ID</param>
        /// <param name="categoryID">分類 ID</param>
        /// <param name="userID">新增或修改人 ID</param>
        /// <returns>跨分類 model</returns>
        private ActionResponse<DB.TWSQLDB.Models.ItemCategorySketch> MakeItemCategorySketch_DB(int itemSketchID, int categoryID, string userID)
        {
            ActionResponse<DB.TWSQLDB.Models.ItemCategorySketch> result = new ActionResponse<DB.TWSQLDB.Models.ItemCategorySketch>();
            result.Body = new DB.TWSQLDB.Models.ItemCategorySketch();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            try
            {
                result.Body.ItemSketchID = itemSketchID;
                result.Body.CategoryID = categoryID;
                result.Body.FromSystem = "1";

                result.Body.CreateUser = userID;
                result.Body.UpdateUser = userID;

                DateTime nowTime = DateTime.Now;
                result.Body.CreateDate = nowTime;
                result.Body.UpdateDate = nowTime;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                log.Info(string.Format("組合跨分類 Model 失敗(expection); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 組合草稿 DB Model
        /// </summary>
        /// <param name="itemSketch">商品資訊</param>
        /// <returns>草稿 DB Model</returns>
        private ActionResponse<DB.TWSQLDB.Models.ItemSketch> MakeItemSketch_DB(ItemSketchEditType editType, DB.TWSQLDB.Models.ItemSketch itemSketch_DB, ItemSketch itemSketch, string accountType)
        {
            ActionResponse<DB.TWSQLDB.Models.ItemSketch> result = new ActionResponse<DB.TWSQLDB.Models.ItemSketch>();

            switch (editType)
            {
                case ItemSketchEditType.Create:
                    {
                        result = MakeMakeItemSketch_DB_CreateData(itemSketch_DB, itemSketch, accountType);
                        break;
                    }
                case ItemSketchEditType.DetailEdit:
                    {
                        result = MakeMakeItemSketch_DB_DetailEditData(itemSketch_DB, itemSketch, accountType);
                        break;
                    }
                case ItemSketchEditType.ListEdit:
                    {
                        result = MakeMakeItemSketch_DB_ListEditData(itemSketch_DB, itemSketch, accountType);
                        break;
                    }
                default:
                    {
                        result.Finish(false, SetResponseCode(false), "組合類型錯誤，組合草稿失敗。", null);
                        break;
                    }
            }

            return result;
        }

        /// <summary>
        /// 組合建立草稿 DB Model
        /// </summary>
        /// <param name="itemSketch_DB">草稿 DB Model</param>
        /// <param name="itemSketch">商品資訊</param>
        /// <returns>草稿 DB Model</returns>
        private ActionResponse<DB.TWSQLDB.Models.ItemSketch> MakeMakeItemSketch_DB_CreateData(DB.TWSQLDB.Models.ItemSketch itemSketch_DB, ItemSketch itemSketch, string accountType)
        {
            ActionResponse<DB.TWSQLDB.Models.ItemSketch> result = new ActionResponse<DB.TWSQLDB.Models.ItemSketch>();
            result.Body = new DB.TWSQLDB.Models.ItemSketch();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            ActionResponse<DB.TWSQLDB.Models.ItemSketch> makeMakeItemSketch_DB_DetailEditData_Result = MakeMakeItemSketch_DB_DetailEditData(itemSketch_DB, itemSketch, accountType);

            if (makeMakeItemSketch_DB_DetailEditData_Result.IsSuccess)
            {
                try
                {
                    itemSketch_DB.SourceTable = "SellerPortal";
                    itemSketch_DB.Status = 0;

                    #region Item
                    
                        itemSketch_DB.ItemQtyReg = 0;
                        itemSketch_DB.ItemSafeQty = 0;
                        itemSketch_DB.PriceLocalship = 0;
                        itemSketch_DB.PriceGlobalship = 0;
                        itemSketch_DB.SellerID = itemSketch.Item.SellerID;
                        itemSketch_DB.Discard4 = itemSketch.Item.Discard4;
                        itemSketch_DB.ServicePrice = 0;
                        itemSketch_DB.WarehouseID = 0;
                        itemSketch_DB.ShowOrder = 0;
                        itemSketch_DB.SpecDetail = string.Empty;
                    
                    #endregion Item

                    #region Product
                                       
                    itemSketch_DB.IsMarket = "Y";
                    itemSketch_DB.Tax = 0;
                    itemSketch_DB.TradeTax = 0;
                
                    #endregion Product

                    #region ItemStock

                    itemSketch_DB.InventoryQtyReg = 0;
                   
                    #endregion ItemStock

                    #region CreateAndUpdate

                    itemSketch_DB.CreateDate = itemSketch_DB.UpdateDate;
                    itemSketch_DB.CreateUser = itemSketch.CreateAndUpdate.CreateUser.ToString();

                    #endregion CreateAndUpdate

                    result.Body = itemSketch_DB;
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Body = null;
                    log.Info(string.Format("組合草稿 DB Model (CreateData) 失敗(expection); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
                }
            }
            else
            {
                result = makeMakeItemSketch_DB_DetailEditData_Result;
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 組合明細編輯項目草稿 DB Model
        /// </summary>
        /// <param name="itemSketch_DB">草稿 DB Model</param>
        /// <param name="itemSketch">商品資訊</param>
        /// <returns>草稿 DB Model</returns>
        private ActionResponse<DB.TWSQLDB.Models.ItemSketch> MakeMakeItemSketch_DB_DetailEditData(DB.TWSQLDB.Models.ItemSketch itemSketch_DB, ItemSketch itemSketch, string accountType)
        {
            ActionResponse<DB.TWSQLDB.Models.ItemSketch> result = new ActionResponse<DB.TWSQLDB.Models.ItemSketch>();
            result.Body = new DB.TWSQLDB.Models.ItemSketch();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            ActionResponse<DB.TWSQLDB.Models.ItemSketch> makeMakeItemSketch_DB_ListEditData_Result = MakeMakeItemSketch_DB_ListEditData(itemSketch_DB, itemSketch, accountType);

            if (makeMakeItemSketch_DB_ListEditData_Result.IsSuccess)
            {
                try
                {
                    #region Item

                    itemSketch_DB.DelvDate = itemSketch.Item.DelvDate;
                    itemSketch_DB.IsNew = itemSketch.Item.IsNew;
                    itemSketch_DB.ItemPackage = itemSketch.Item.ItemPackage;
                    itemSketch_DB.Note = itemSketch.Item.Note;
                    itemSketch_DB.PriceCard = itemSketch.Item.PriceCash;
                    itemSketch_DB.QtyLimit = itemSketch.Item.QtyLimit;
                    itemSketch_DB.Sdesc = itemSketch.Item.Sdesc;
                    itemSketch_DB.Spechead = itemSketch.Item.Spechead;
                    itemSketch_DB.Discard4 = itemSketch.Item.Discard4;

                    #endregion Item

                    #region Product

                    itemSketch_DB.BarCode = itemSketch.Product.BarCode;
                    itemSketch_DB.Description = itemSketch.Product.Description;
                    itemSketch_DB.Is18 = itemSketch.Product.Is18;
                    itemSketch_DB.IsChokingDanger = itemSketch.Product.IsChokingDanger;
                    itemSketch_DB.IsShipDanger = itemSketch.Product.IsShipDanger;
                    itemSketch_DB.Height = itemSketch.Product.Height;
                    itemSketch_DB.Length = itemSketch.Product.Length;
                    itemSketch_DB.ManufactureID = itemSketch.Product.ManufactureID;
                    itemSketch_DB.Model = itemSketch.Product.Model;
                    itemSketch_DB.Name = itemSketch.Product.Name;
                    itemSketch_DB.SellerProductID = itemSketch.Product.SellerProductID;
                    itemSketch_DB.UPC = itemSketch.Product.UPC;
                    itemSketch_DB.Warranty = itemSketch.Product.Warranty;
                    itemSketch_DB.Weight = itemSketch.Product.Weight;
                    itemSketch_DB.Width = itemSketch.Product.Width;

                    #region DelvType

                    ActionResponse<int?> getDelvTypeResult = GetDelvType(accountType, itemSketch.Item.ShipType);

                    if (getDelvTypeResult.IsSuccess)
                    {
                        itemSketch_DB.DelvType = getDelvTypeResult.Body;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Msg += "DelvType 給值失敗。" + getDelvTypeResult.Msg;
                        itemSketch_DB.DelvType = null;
                    }

                    #endregion DelvType

                    #region 判斷圖片張數

                    if (itemSketch.Product.PicPatch_Edit.Count > 0)
                    {
                        itemSketch_DB.PicStart = 1;
                        itemSketch_DB.PicEnd = itemSketch.Product.PicPatch_Edit.Count;
                    }
                    else
                    {
                        itemSketch_DB.PicStart = 0;
                        itemSketch_DB.PicEnd = 0;
                    }

                    #endregion 判斷圖片張數

                    #endregion Product

                    #region ItemCategory

                    itemSketch_DB.CategoryID = itemSketch.ItemCategory.MainCategoryID_Layer2;

                    #endregion ItemCategory

                    #region ItemDisplayPrice

                    if ((itemSketch.Item.PriceCash != null && itemSketch.Product.Cost != null) && (itemSketch.Item.PriceCash > 0 && itemSketch.Product.Cost >= 0))
                    {
                        decimal grossMargin = ((itemSketch.Item.PriceCash.Value - itemSketch.Product.Cost.Value) / itemSketch.Item.PriceCash.Value) * 100;

                        itemSketch_DB.GrossMargin = System.Math.Round(grossMargin, 2);
                    }
                    else
                    {
                        itemSketch_DB.GrossMargin = null;
                    }

                    #endregion ItemDisplayPrice

                    result.Body = itemSketch_DB;
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Body = null;
                    log.Info(string.Format("組合草稿 DB Model (DetailEditData) 失敗(expection); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
                }
            }
            else
            {
                result = makeMakeItemSketch_DB_ListEditData_Result;
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 組合清單可編輯項目草稿 DB Model
        /// </summary>
        /// <param name="itemSketch_DB">草稿 DB Model</param>
        /// <param name="itemSketch">商品資訊</param>
        /// <returns>草稿 DB Model</returns>
        private ActionResponse<DB.TWSQLDB.Models.ItemSketch> MakeMakeItemSketch_DB_ListEditData(DB.TWSQLDB.Models.ItemSketch itemSketch_DB, ItemSketch itemSketch, string accountType)
        {
            ActionResponse<DB.TWSQLDB.Models.ItemSketch> result = new ActionResponse<DB.TWSQLDB.Models.ItemSketch>();
            result.Body = new DB.TWSQLDB.Models.ItemSketch();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            try
            {
                #region Item

                itemSketch_DB.DateStart = itemSketch.Item.DateStart.AddHours(8).Date;
                itemSketch_DB.DateEnd = itemSketch_DB.DateStart.Value.AddYears(2099 - itemSketch.Item.DateStart.Year);
                itemSketch_DB.DateDel = itemSketch_DB.DateEnd.Value.AddDays(1);
                itemSketch_DB.ItemQty = itemSketch.Item.CanSaleLimitQty;
                itemSketch_DB.MarketPrice = itemSketch.Item.MarketPrice;
                itemSketch_DB.PriceCash = itemSketch.Item.PriceCash;
                itemSketch_DB.Discard4 = itemSketch.Item.Discard4;


                #region ShipType

                if (itemSketch.Item.ShipType == "S")
                {
                    itemSketch_DB.ShipType = accountType;
                    itemSketch.Item.ShipType = accountType;
                }
                else
                {
                    itemSketch_DB.ShipType = itemSketch.Item.ShipType;
                }

                #endregion ShipType

                #endregion Item

                #region Product

                itemSketch_DB.Cost = itemSketch.Product.Cost;

                #endregion Product

                #region ItemStock

                itemSketch_DB.InventoryQty = itemSketch.ItemStock.CanSaleQty;
                itemSketch_DB.InventorySafeQty = itemSketch.ItemStock.InventorySafeQty;

                #endregion ItemStock

                #region ItemDisplayPrice

                if ((itemSketch.Item.PriceCash != null && itemSketch.Product.Cost != null) && (itemSketch.Item.PriceCash > 0 && itemSketch.Product.Cost >= 0))
                {
                    decimal grossMargin = ((itemSketch.Item.PriceCash.Value - itemSketch.Product.Cost.Value) / itemSketch.Item.PriceCash.Value) * 100;

                    itemSketch_DB.GrossMargin = System.Math.Round(grossMargin, 2);
                }
                else
                {
                    itemSketch_DB.GrossMargin = null;
                }

                #endregion ItemDisplayPrice

                #region CreateAndUpdate

                DateTime nowDateTime = DateTime.Now;

                itemSketch_DB.UpdateDate = nowDateTime;
                itemSketch_DB.UpdateUser = itemSketch.CreateAndUpdate.CreateUser.ToString();

                #endregion CreateAndUpdate

                result.Body = itemSketch_DB;
            } 
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Body = null;
                log.Info(string.Format("組合草稿 DB Model (ListEditData) 失敗(expection); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }


        /// <summary>
        /// 取得 Account Type
        /// </summary>
        /// <param name="sellerID">商家 ID</param>
        /// <returns>Account Type</returns>
        public ActionResponse<string> GetAccountType(int sellerID)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            result.Body = string.Empty;
            result.IsSuccess = true;
            result.Msg = string.Empty;

            DB.TWSellerPortalDBContext dbSellerPortal = new TWSellerPortalDBContext();

            try
            {
                result.Body = dbSellerPortal.Seller_BasicInfo.Where(x => x.SellerID == sellerID).Select(x => x.AccountTypeCode).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                log.Info(string.Format("取得 AccountType 失敗(expection); SellerID = {0}; ErrorMessage = {1}; StackTrace = {2}.", sellerID, GetExceptionMessage(ex), ex.StackTrace));
            }

            if (string.IsNullOrEmpty(result.Body) || (result.Body != "S" && result.Body != "V"))
            {
                result.IsSuccess = false;

                if (string.IsNullOrEmpty(result.Body))
                {
                    log.Info(string.Format("查無 AccountType 資訊。"));
                }

                if(result.Body != "S" && result.Body != "V")
                {
                    log.Info(string.Format("AccountType = {0} 為無效值。", result.Body));
                }
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
        private ActionResponse<int?> GetDelvType(string accountType, string shipType)
        {
            ActionResponse<int?> result = new ActionResponse<int?>();
            result.Body = null;
            result.IsSuccess = true;
            result.Msg = string.Empty;

            if (result.IsSuccess)
            {
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
            }

            if (result.Body == null)
            {
                result.IsSuccess = false;
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        #endregion 組 model
      
        /// <summary>
        /// 填寫 Response Code
        /// </summary>
        /// <param name="isSuccess">成功、失敗資訊</param>
        /// <returns>Response Code</returns>
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

        #region 草稿儲存與送審

        /// <summary>
        /// 草稿儲存與送審
        /// </summary>
        /// <param name="itemSketckList">送審資訊</param>
        /// <param name="userId">使用者ID</param>
        /// <param name="actionType">執行送審的類型</param>
        /// <returns>返回草稿儲存與送審結果</returns>
        public ActionResponse<string> SendItemSketchToPending(List<TWNewEgg.API.Models.ItemSketch> itemSketckList, string userId, int actionType)
        {
            ActionResponse<string> pendingResult = new ActionResponse<string>();
            string json_PicPatch_Edit = string.Empty;
            string json_PicPath_Sketch = string.Empty;
            API.Models.ActionResponse<List<string>> itemCreateResult = new ActionResponse<List<string>>();
            try
            {
                switch (actionType)
                {
                    // SketchAdd
                    case 2:
                        itemCreateResult = CreateItemSketch(itemSketckList);
                        break;
                    // SketchEdit
                    case 3:
                        itemCreateResult = EditItemSketch(ItemSketchEditType.DetailEdit, itemSketckList);
                        break;
                    // SketchCopy
                    case 4:
                        itemCreateResult = CreateItemSketch(itemSketckList);
                        break;
                    case 5:
                        itemCreateResult = CreateItemSketch(itemSketckList);
                        break;
                    default:
                        itemCreateResult.IsSuccess = false;
                        itemCreateResult.Msg = "ActionType資料傳入錯誤";
                        break;
                }
                json_PicPatch_Edit = Newtonsoft.Json.JsonConvert.SerializeObject(itemSketckList[0].Product.PicPatch_Edit);
                json_PicPath_Sketch = Newtonsoft.Json.JsonConvert.SerializeObject(itemSketckList[0].Product.PicPath_Sketch);
                log.Info("[actionType] " + actionType + " [ItemSketch.ID] " + itemSketckList[0].ID + " [Item.ID] " + itemSketckList[0].Item.ID + " [Item.ItemID] " + itemSketckList[0].Item.ItemID + " [Product.ID] " + itemSketckList[0].Product.ID + " [Product.ProductID] " + itemSketckList[0].Product.ProductID + " [Product.PicPath_Sketch] " + json_PicPath_Sketch + " [Product.PicPatch_Edit] " + json_PicPatch_Edit);
            }
            catch (Exception e)
            {
                pendingResult.IsSuccess = false;
                pendingResult.Msg = "發生Exception 送審資料儲存執行失敗";
                log.Info("發生Exception 送審資料儲存執行失敗 ErrorMessage [ " + e.Message + " ] ErrorStackTrace [ " + e.StackTrace + " ]");
            }

            List<int> itemSketchIDList = new List<int>();
            if (itemCreateResult.IsSuccess == true)
            {
                foreach (TWNewEgg.API.Models.ItemSketch subItemSketch in itemSketckList)
                {
                    itemSketchIDList.Add(subItemSketch.ID);
                }
                string json_itemSketchIDList = Newtonsoft.Json.JsonConvert.SerializeObject(itemSketchIDList);
                log.Info("[subItemSketch.ID] " + itemSketckList[0].ID + " [userId] " + userId + " [itemSketchIDList] " + json_itemSketchIDList);
                try
                {
                    // 送審
                    pendingResult = VerifyStetch(itemSketchIDList, userId);
                    if (pendingResult.IsSuccess == true)
                    {
                        TWNewEgg.BackendService.Interface.ICategoryAssociated CategoryAssociated = new TWNewEgg.BackendService.Service.CategoryAssociatedService();
                        TWNewEgg.API.Service.ImageService imgService = new Service.ImageService();
                        TWNewEgg.API.Service.PMInfoService pmService = new Service.PMInfoService();
                        log.Info("[送審成功進行圖片處理] [itemSketckList.FirstOrDefault().Product.PicPatch_Edit] " + json_PicPatch_Edit + " [itemSketchIDList.FirstOrDefault()] " + itemSketchIDList.FirstOrDefault());
                        // 送審成功進行圖片處理
                        imgService.ItemSketchDetailImgToTemp(itemSketckList[0].Product.PicPatch_Edit, itemSketchIDList[0]);

                        //Dictionary<int, int> checkResult = pmService.CheckGrossMargin();
                        // 改以送審的 ItemSketchID 進行館價判斷
                        Dictionary<int, int> checkResult = pmService.CheckGrossMargin(itemSketchIDList);

                        if (checkResult != null)
                        {
                            foreach (var index in checkResult)
                            {
                                // Call BackendService 寄送 Email
                                List<string> PmMails = CategoryAssociated.SendPMWithGrossMargin(index.Value);
                                pmService.SendPMGrossMarginMial(index.Key, PmMails);
                                //Thread.Sleep(1000);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    pendingResult.IsSuccess = false;
                    pendingResult.Msg = "發生Exception 資料送審執行失敗";
                    log.Info("發生Exception 資料送審執行失敗 ErrorMessage [ " + e.Message + " ] ErrorStackTrace [ " + e.StackTrace + " ]");
                }
            }
            else
            {
                pendingResult.IsSuccess = itemCreateResult.IsSuccess;
                pendingResult.Msg = itemCreateResult.Msg;
            }

            return pendingResult;
        }

        #endregion 草稿儲存與送審
        public List<ItemSketch> DistinctItemSketch(List<ItemSketch> itemSketchlist)
        {
            TWNewEgg.DB.TWSqlDBContext db = new TWSqlDBContext();
            List<int> groupItemSketch = db.ItemSketchGroup.Select(r => r.ItemSketchID).ToList();
            // 排除 Group草稿
            return itemSketchlist.Where(x => !groupItemSketch.Contains(x.ID)).ToList();
        }
    }
}
