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
using System.Security.Cryptography;

namespace TWNewEgg.API.Service
{

    
    public class ExamineBatchService
    {
        private static ILog log = LogManager.GetLogger(typeof(ExamineBatchService));
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        TWNewEgg.API.Service.ImageService imageService = new ImageService();
        TWNewEgg.BackendService.Interface.ICategoryService CategoryService = new TWNewEgg.BackendService.Service.CategoryService();
        #region 批次送審 會經過草稿區
        ///// <summary>
        ///// 批次送審用的草稿建立
        ///// </summary>
        ///// <param name="itemSketchCreate"></param>
        ///// <returns></returns>
        //public TWNewEgg.API.Models.ActionResponse<string> ExamineBatchApiOne(TWNewEgg.API.Models.ItemSketch itemSketchCreate)
        //{

        //    TWNewEgg.API.Models.ActionResponse<string> result = new ActionResponse<string>();
        //    ActionResponse<string> checkResult = new ActionResponse<string>();
        //    checkResult = this.CheckInput_ItemSketch(itemSketchCreate);//檢查資料的完整性
        //    if (checkResult.IsSuccess == false)
        //    {
        //        result.IsSuccess = false;
        //        result.Msg = checkResult.Msg;
        //        return result;
        //    }
        //    else
        //    {
        //        ActionResponse<string> getAccountType = GetAccountType(itemSketchCreate.Item.SellerID.Value);
        //        if (getAccountType.IsSuccess == false)
        //        {
        //            result.IsSuccess = false;
        //            result.Msg = getAccountType.Msg;
        //            return result;
        //        }
        //        else
        //        {
        //            using (TransactionScope scope = new TransactionScope())
        //            {
        //                ActionResponse<string> saveItemSketchResult = SaveItemSketch(itemSketchCreate, getAccountType.Body);
        //                if (saveItemSketchResult.IsSuccess == true)
        //                {
        //                    result.IsSuccess = true;
        //                }
        //                else
        //                {
        //                    result.IsSuccess = false;
        //                    result.Msg = saveItemSketchResult.Msg;
        //                }
        //                if (result.IsSuccess == true)
        //                {
        //                    scope.Complete();
        //                    result.Body = saveItemSketchResult.Body;
        //                }
        //                else
        //                {
        //                    scope.Dispose();
        //                }
        //            }
        //            return result;
        //        }
        //    }
        //}



        //#region 批次審核多筆
        //public TWNewEgg.API.Models.ActionResponse<string> ExamineBatchApi(List<TWNewEgg.API.Models.ItemSketch> itemSketchCreate)
        //{
        //    TWNewEgg.API.Models.ActionResponse<string> result = new ActionResponse<string>();
        //    List<string> msgList = new List<string>();
        //    int dataCount = 1;
        //    List<string> itemSketchIdTemp = new List<string>();
        //    foreach (var item in itemSketchCreate)
        //    {
        //        ActionResponse<string> checkResult = new ActionResponse<string>();

        //        checkResult = this.CheckInput_ItemSketch(item);
        //        //檢查資料有錯誤的情形，並說明第幾筆資料
        //        if (checkResult.IsSuccess == false)
        //        {
        //            msgList.Add("第 " + dataCount + " 筆資料錯誤: " + checkResult.Msg);
        //        }
        //        else
        //        {
        //            ActionResponse<string> getAccountType = GetAccountType(item.Item.SellerID.Value);
        //            if (getAccountType.IsSuccess == false)
        //            {
        //                msgList.Add("第 " + dataCount + " 筆資料錯誤: " + getAccountType.Msg);
        //            }
        //            else
        //            {
        //                using (TransactionScope scope = new TransactionScope())
        //                {
        //                    ActionResponse<string> saveItemSketchResult = SaveItemSketch(item, getAccountType.Body);
        //                    if (saveItemSketchResult.IsSuccess == true)
        //                    {
        //                        //itemSketchIdTemp.Add(saveItemSketchResult.Body.ToString());
        //                        result.IsSuccess = true;
        //                    }
        //                    else
        //                    {
        //                        result.IsSuccess = false;
        //                        result.Msg = "第 " + dataCount + "筆資料錯誤: " + saveItemSketchResult.Msg;
        //                    }
        //                    if (result.IsSuccess == true)
        //                    {
        //                        scope.Complete();
        //                        //result.Body = itemSketchIdTemp;
        //                        result.Body = saveItemSketchResult.Body;
        //                    }
        //                    else
        //                    {
        //                        scope.Dispose();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return result;
        //}
        //#endregion
        ///*-----------------------------------------------------------------------*/
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="itemSketch"></param>
        ///// <param name="accountType"></param>
        ///// <returns></returns>
        //private ActionResponse<string> SaveItemSketch(ItemSketch itemSketch, string accountType)
        //{
        //    ActionResponse<string> result = new ActionResponse<string>();
        //    result.IsSuccess = true;
        //    result.Msg = string.Empty;
        //    DB.TWSQLDB.Models.ItemSketch itemSketch_DB = new DB.TWSQLDB.Models.ItemSketch();
        //    // 建立或更新草稿內容
        //    if (result.IsSuccess)
        //    {
        //        ActionResponse<DB.TWSQLDB.Models.ItemSketch> makeItemSketch_DB_Result = MakeItemSketch_DB(itemSketch_DB, itemSketch, accountType);
        //        if (makeItemSketch_DB_Result.IsSuccess)
        //        {
        //            itemSketch_DB = makeItemSketch_DB_Result.Body;
        //        }
        //        else
        //        {
        //            // 組合草稿 DB Model 失敗
        //            result.IsSuccess = false;
        //        }
        //    }
        //    #region 儲存草稿
        //    if (result.IsSuccess)
        //    {
        //        DB.TWSqlDBContext _dbFront = new TWSqlDBContext();
        //        _dbFront.ItemSketch.Add(itemSketch_DB);
        //        if (result.IsSuccess)
        //        {
        //            try
        //            {
        //                _dbFront.SaveChanges();
        //            }
        //            catch (Exception ex)
        //            {
        //                result.IsSuccess = false;
        //                result.Msg = "資料錯誤";
        //                log.Info(string.Format("ExamineBatchService/SavaItemSketch error: 儲存草稿失敗(exception); ErrorMessage = {0}; StackTrace = {1}.", ex.Message, ex.StackTrace));

        //            }
        //        }

        //    }
        //    #endregion
        //    if (result.IsSuccess == true)
        //    {
        //        #region 儲存跨分類
        //        if (result.IsSuccess)
        //        {
        //            if (result.IsSuccess)
        //            {
        //                List<int> subCategoryIDCell = new List<int>();

        //                if (itemSketch.ItemCategory.SubCategoryID_1_Layer2 != null && itemSketch.ItemCategory.SubCategoryID_1_Layer2.Value > 0)
        //                {
        //                    subCategoryIDCell.Add(itemSketch.ItemCategory.SubCategoryID_1_Layer2.Value);
        //                }

        //                if (itemSketch.ItemCategory.SubCategoryID_2_Layer2 != null && itemSketch.ItemCategory.SubCategoryID_2_Layer2.Value > 0)
        //                {
        //                    subCategoryIDCell.Add(itemSketch.ItemCategory.SubCategoryID_2_Layer2.Value);
        //                }

        //                if (subCategoryIDCell.Count > 0)
        //                {
        //                    // 建立失敗計數
        //                    int createFailCount = 0;

        //                    foreach (int subCategoryID in subCategoryIDCell)
        //                    {
        //                        ActionResponse<bool> saveItemCategory = SaveItemCategorySketch(itemSketch_DB.ID, subCategoryID, itemSketch.CreateAndUpdate.UpdateUser.ToString());

        //                        if (saveItemCategory.IsSuccess == false)
        //                        {
        //                            createFailCount++;
        //                            result.IsSuccess = false;
        //                            result.Msg = saveItemCategory.Msg;

        //                            log.Info("ExamineBatch/SaveItemSketch error: " + result.Msg);
        //                        }
        //                    }

        //                    if (createFailCount == 0)
        //                    {
        //                        //result.Body = true;
        //                    }

        //                    log.Info(string.Format("ExamineBatchService/SaveItemSketch: 共有 {0} 筆跨分類資訊，已成功建立 {1} 筆，失敗 {2} 筆。", subCategoryIDCell.Count, subCategoryIDCell.Count - createFailCount, createFailCount));
        //                }
        //            }
        //        }
        //        #endregion
        //        #region 儲存圖片

        //        if (result.IsSuccess)
        //        {

        //            ActionResponse<bool> saveImageResult = imageService.ImageProcess(itemSketch.Product.PicPatch_Edit, "pic\\itemSketch", "pic\\pic\\itemSketch", itemSketch_DB.ID);

        //            if (saveImageResult.IsSuccess == false)
        //            {
        //                result.IsSuccess = false;
        //                result.Msg = "儲存草稿圖片失敗";
        //                log.Info(string.Format("ExamineBatch/SaveItemSketch error: 儲存草稿圖片失敗; API Message = {0}", saveImageResult.Msg));
        //            }
        //        }

        //        #endregion 圖片處理
        //        #region 儲存商品屬性
        //        if (result.IsSuccess)
        //        {
        //            ProductPorpertySketchService productPorpertySketchService = new ProductPorpertySketchService();

        //            ActionResponse<string> saveResult = productPorpertySketchService.SaveProductPropertyClick(itemSketch.SaveProductPropertyList, itemSketch_DB.ID, itemSketch.CreateAndUpdate.UpdateUser);

        //            if (saveResult.IsSuccess == false)
        //            {
        //                result.IsSuccess = false;
        //                result.Msg = "商品屬性失敗";
        //                log.Info(string.Format("ExamineBatch/SaveItemSketch error: 儲存商品屬性失敗; Message = {0}.", saveResult.Msg));
        //            }
        //        }
        //        #endregion
        //    }
        //    result.Code = SetResponseCode(result.IsSuccess);
        //    if (result.IsSuccess)
        //    {
        //        //草稿建立成功之後，回傳 itemSketch_DB 以便之後送審用
        //        itemSketch.ID = itemSketch_DB.ID;
        //        result.Body = itemSketch.ID.ToString();
        //        log.Info(string.Format("儲存草稿成功; ItemSketchID = {0}.", itemSketch_DB.ID.ToString()));
        //    }
        //    return result;
        //}
        ///// <summary>
        ///// 儲存跨分類資訊
        ///// </summary>
        ///// <param name="itemSketchID">草稿 ID</param>
        ///// <param name="categoryID">分類 ID</param>
        ///// <param name="userID">建立或更新人 ID</param>
        ///// <returns>成功、失敗訊息</returns>
        //private ActionResponse<bool> SaveItemCategorySketch(int itemSketchID, int categoryID, string userID)
        //{
        //    ActionResponse<bool> result = new ActionResponse<bool>();
        //    result.Body = false;
        //    result.IsSuccess = true;
        //    result.Msg = string.Empty;

        //    ActionResponse<DB.TWSQLDB.Models.ItemCategorySketch> itemCategorySketch_DB_Result = MakeItemCategorySketch_DB(itemSketchID, categoryID, userID);

        //    if (itemCategorySketch_DB_Result.IsSuccess)
        //    {
        //        DB.TWSqlDBContext dbFront = new TWSqlDBContext();
        //        dbFront.ItemCategorySketch.Add(itemCategorySketch_DB_Result.Body);

        //        try
        //        {
        //            dbFront.SaveChanges();
        //        }
        //        catch (Exception ex)
        //        {
        //            result.IsSuccess = false;
        //            log.Info("ExamineBatchService/SaveItemCategorySketch error: 儲存跨分類失敗(exception)：" + ex.Message + "[StackTrace]" + ex.StackTrace);
        //            result.Msg = "發生 exception，儲存跨分類失敗。";
        //        }
        //    }
        //    else
        //    {
        //        result.IsSuccess = false;
        //        result.Msg = itemCategorySketch_DB_Result.Msg;
        //    }

        //    if (result.IsSuccess)
        //    {
        //        result.Body = true;
        //    }

        //    result.Code = SetResponseCode(result.IsSuccess);

        //    return result;
        //}
        ///*-----------------------------------------------------------------------*/
        ///// <summary>
        ///// 組合跨分類 Model
        ///// </summary>
        ///// <param name="itemSketchID">草稿 ID</param>
        ///// <param name="categoryID">分類 ID</param>
        ///// <param name="userID">新增或修改人 ID</param>
        ///// <returns>跨分類 model</returns>
        //private ActionResponse<DB.TWSQLDB.Models.ItemCategorySketch> MakeItemCategorySketch_DB(int itemSketchID, int categoryID, string userID)
        //{
        //    ActionResponse<DB.TWSQLDB.Models.ItemCategorySketch> result = new ActionResponse<DB.TWSQLDB.Models.ItemCategorySketch>();
        //    result.Body = new DB.TWSQLDB.Models.ItemCategorySketch();
        //    result.IsSuccess = true;
        //    result.Msg = string.Empty;

        //    try
        //    {
        //        result.Body.ItemSketchID = itemSketchID;
        //        result.Body.CategoryID = categoryID;
        //        result.Body.FromSystem = "1";

        //        result.Body.CreateUser = userID;
        //        result.Body.UpdateUser = userID;

        //        DateTime nowTime = DateTime.Now;
        //        result.Body.CreateDate = nowTime;
        //        result.Body.UpdateDate = nowTime;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.IsSuccess = false;
        //        result.Msg = "資料錯誤";
        //        log.Info(string.Format("ExamineBatchService/MakeItemCategorySketch_DB error: 組合跨分類 Model 失敗(expection); ErrorMessage = {0}; StackTrace = {1}.", ex.Message, ex.StackTrace));
        //    }

        //    result.Code = SetResponseCode(result.IsSuccess);

        //    return result;
        //}
        ///*-----------------------------------------------------------------------*/
        //private ActionResponse<DB.TWSQLDB.Models.ItemSketch> MakeItemSketch_DB(DB.TWSQLDB.Models.ItemSketch itemSketch_DB, ItemSketch itemSketch, string accountType)
        //{
        //    ActionResponse<DB.TWSQLDB.Models.ItemSketch> result = new ActionResponse<DB.TWSQLDB.Models.ItemSketch>();
        //    result = MakeMakeItemSketch_DB_CreateData(itemSketch_DB, itemSketch, accountType);
        //    return result;
        //}
        //private ActionResponse<DB.TWSQLDB.Models.ItemSketch> MakeMakeItemSketch_DB_CreateData(DB.TWSQLDB.Models.ItemSketch itemSketch_DB, ItemSketch itemSketch, string accountType)
        //{
        //    ActionResponse<DB.TWSQLDB.Models.ItemSketch> result = new ActionResponse<DB.TWSQLDB.Models.ItemSketch>();
        //    result.Body = new DB.TWSQLDB.Models.ItemSketch();
        //    result.IsSuccess = true;
        //    result.Msg = string.Empty;
        //    ActionResponse<DB.TWSQLDB.Models.ItemSketch> makeMakeItemSketch_DB_DetailEditData_Result = MakeMakeItemSketch_DB_DetailEditData(itemSketch_DB, itemSketch, accountType);
        //    if (makeMakeItemSketch_DB_DetailEditData_Result.IsSuccess)
        //    {
        //        try
        //        {
        //            itemSketch_DB.SourceTable = "SellerPortal";
        //            itemSketch_DB.Status = 0;

        //            #region Item

        //            itemSketch_DB.ItemQtyReg = 0;
        //            itemSketch_DB.ItemSafeQty = 0;
        //            itemSketch_DB.PriceLocalship = 0;
        //            itemSketch_DB.PriceGlobalship = 0;
        //            itemSketch_DB.SellerID = itemSketch.Item.SellerID;
        //            itemSketch_DB.ServicePrice = 0;
        //            itemSketch_DB.WarehouseID = 0;
        //            itemSketch_DB.ShowOrder = 0;
        //            itemSketch_DB.SpecDetail = string.Empty;

        //            #endregion Item

        //            #region Product

        //            itemSketch_DB.IsMarket = "Y";
        //            itemSketch_DB.Tax = 0;
        //            itemSketch_DB.TradeTax = 0;

        //            #endregion Product

        //            #region ItemStock

        //            itemSketch_DB.InventoryQtyReg = 0;

        //            #endregion ItemStock

        //            #region CreateAndUpdate

        //            itemSketch_DB.CreateDate = itemSketch_DB.UpdateDate;
        //            itemSketch_DB.CreateUser = itemSketch.CreateAndUpdate.CreateUser.ToString();

        //            #endregion CreateAndUpdate

        //            result.Body = itemSketch_DB;
        //        }
        //        catch (Exception ex)
        //        {
        //            result.IsSuccess = false;
        //            result.Body = null;
        //            result.Msg = "資料錯誤";
        //            log.Info(string.Format("/ExamineService/MakeMakeItemSketch_DB_CeateDate error: 組合草稿 DB Model (CreateData) 失敗(expection); ErrorMessage = {0}; StackTrace = {1}.", ex.Message, ex.StackTrace));
        //        }
        //    }
        //    else
        //    {
        //        result = makeMakeItemSketch_DB_DetailEditData_Result;
        //    }

        //    result.Code = SetResponseCode(result.IsSuccess);

        //    return result;

        //}
        //private ActionResponse<DB.TWSQLDB.Models.ItemSketch> MakeMakeItemSketch_DB_DetailEditData(DB.TWSQLDB.Models.ItemSketch itemSketch_DB, ItemSketch itemSketch, string accountType)
        //{
        //    ActionResponse<DB.TWSQLDB.Models.ItemSketch> result = new ActionResponse<DB.TWSQLDB.Models.ItemSketch>();
        //    result.Body = new DB.TWSQLDB.Models.ItemSketch();
        //    result.IsSuccess = true;
        //    result.Msg = string.Empty;

        //    ActionResponse<DB.TWSQLDB.Models.ItemSketch> makeMakeItemSketch_DB_ListEditData_Result = MakeMakeItemSketch_DB_ListEditData(itemSketch_DB, itemSketch, accountType);

        //    if (makeMakeItemSketch_DB_ListEditData_Result.IsSuccess)
        //    {
        //        try
        //        {
        //            #region Item

        //            itemSketch_DB.DateEnd = itemSketch.Item.DateStart.AddYears(2099 - itemSketch.Item.DateStart.Year);
        //            itemSketch_DB.DateDel = itemSketch_DB.DateEnd.Value.AddDays(1);
        //            itemSketch_DB.DelvDate = itemSketch.Item.DelvDate;
        //            itemSketch_DB.IsNew = itemSketch.Item.IsNew;
        //            itemSketch_DB.ItemPackage = itemSketch.Item.ItemPackage;
        //            itemSketch_DB.Note = itemSketch.Item.Note;
        //            itemSketch_DB.PriceCard = itemSketch.Item.PriceCash;
        //            itemSketch_DB.QtyLimit = itemSketch.Item.QtyLimit;
        //            itemSketch_DB.Sdesc = itemSketch.Item.Sdesc;
        //            itemSketch_DB.Spechead = itemSketch.Item.Spechead;

        //            #endregion Item

        //            #region Product

        //            itemSketch_DB.BarCode = itemSketch.Product.BarCode;
        //            itemSketch_DB.Description = itemSketch.Product.Description;
        //            itemSketch_DB.Is18 = itemSketch.Product.Is18;
        //            itemSketch_DB.IsChokingDanger = itemSketch.Product.IsChokingDanger;
        //            itemSketch_DB.IsShipDanger = itemSketch.Product.IsShipDanger;
        //            itemSketch_DB.Height = itemSketch.Product.Height;
        //            itemSketch_DB.Length = itemSketch.Product.Length;
        //            itemSketch_DB.ManufactureID = itemSketch.Product.ManufactureID;
        //            itemSketch_DB.Model = itemSketch.Product.Model;
        //            itemSketch_DB.Name = itemSketch.Product.Name;
        //            itemSketch_DB.SellerProductID = itemSketch.Product.SellerProductID;
        //            itemSketch_DB.UPC = itemSketch.Product.UPC;
        //            itemSketch_DB.Warranty = itemSketch.Product.Warranty;
        //            itemSketch_DB.Weight = itemSketch.Product.Weight;
        //            itemSketch_DB.Width = itemSketch.Product.Width;

        //            #region DelvType

        //            ActionResponse<int?> getDelvTypeResult = GetDelvType(accountType, itemSketch.Item.ShipType);

        //            if (getDelvTypeResult.IsSuccess)
        //            {
        //                itemSketch_DB.DelvType = getDelvTypeResult.Body;
        //            }
        //            else
        //            {
        //                result.IsSuccess = false;
        //                result.Msg += "DelvType 給值失敗。" + getDelvTypeResult.Msg;
        //                itemSketch_DB.DelvType = null;
        //            }

        //            #endregion DelvType

        //            #region 判斷圖片張數

        //            if (itemSketch.Product.PicPatch_Edit.Count > 0)
        //            {
        //                itemSketch_DB.PicStart = 1;
        //                itemSketch_DB.PicEnd = itemSketch.Product.PicPatch_Edit.Count;
        //            }
        //            else
        //            {
        //                itemSketch_DB.PicStart = 0;
        //                itemSketch_DB.PicEnd = 0;
        //            }

        //            #endregion 判斷圖片張數

        //            #endregion Product

        //            #region ItemCategory

        //            itemSketch_DB.CategoryID = itemSketch.ItemCategory.MainCategoryID_Layer2;

        //            #endregion ItemCategory

        //            #region ItemDisplayPrice

        //            if ((itemSketch.Item.PriceCash != null && itemSketch.Product.Cost != null) && (itemSketch.Item.PriceCash > 0 && itemSketch.Product.Cost >= 0))
        //            {
        //                decimal grossMargin = ((itemSketch.Item.PriceCash.Value - itemSketch.Product.Cost.Value) / itemSketch.Item.PriceCash.Value) * 100;

        //                itemSketch_DB.GrossMargin = System.Math.Round(grossMargin, 2);
        //            }
        //            else
        //            {
        //                itemSketch_DB.GrossMargin = null;
        //            }

        //            #endregion ItemDisplayPrice

        //            result.Body = itemSketch_DB;
        //        }
        //        catch (Exception ex)
        //        {
        //            result.IsSuccess = false;
        //            result.Body = null;
        //            result.Msg = "資料錯誤";
        //            log.Info(string.Format("/ExamineService/MakeMakeItemSketch error: 組合草稿 DB Model (DetailEditData) 失敗(expection); ErrorMessage = {0}; StackTrace = {1}.", ex.Message, ex.StackTrace));
        //        }
        //    }
        //    else
        //    {
        //        result = makeMakeItemSketch_DB_ListEditData_Result;
        //    }

        //    result.Code = SetResponseCode(result.IsSuccess);

        //    return result;
        //}
        //private ActionResponse<DB.TWSQLDB.Models.ItemSketch> MakeMakeItemSketch_DB_ListEditData(DB.TWSQLDB.Models.ItemSketch itemSketch_DB, ItemSketch itemSketch, string accountType)
        //{
        //    ActionResponse<DB.TWSQLDB.Models.ItemSketch> result = new ActionResponse<DB.TWSQLDB.Models.ItemSketch>();
        //    result.Body = new DB.TWSQLDB.Models.ItemSketch();
        //    result.IsSuccess = true;
        //    result.Msg = string.Empty;

        //    try
        //    {
        //        #region Item

        //        itemSketch_DB.DateStart = itemSketch.Item.DateStart.AddHours(8).Date;
        //        itemSketch_DB.ItemQty = itemSketch.Item.CanSaleLimitQty - itemSketch.Item.ItemQtyReg ?? 0;
        //        itemSketch_DB.MarketPrice = itemSketch.Item.MarketPrice;
        //        itemSketch_DB.PriceCash = itemSketch.Item.PriceCash;

        //        #region ShipType

        //        if (itemSketch.Item.ShipType == "S")
        //        {
        //            itemSketch_DB.ShipType = accountType;
        //            itemSketch.Item.ShipType = accountType;
        //        }
        //        else
        //        {
        //            itemSketch_DB.ShipType = itemSketch.Item.ShipType;
        //        }

        //        #endregion ShipType

        //        #endregion Item

        //        #region Product

        //        itemSketch_DB.Cost = itemSketch.Product.Cost;

        //        #endregion Product

        //        #region ItemStock

        //        itemSketch_DB.InventoryQty = itemSketch.ItemStock.CanSaleQty + itemSketch.ItemStock.InventoryQtyReg ?? 0;
        //        itemSketch_DB.InventorySafeQty = itemSketch.ItemStock.InventorySafeQty;

        //        #endregion ItemStock

        //        #region ItemDisplayPrice

        //        if ((itemSketch.Item.PriceCash != null && itemSketch.Product.Cost != null) && (itemSketch.Item.PriceCash > 0 && itemSketch.Product.Cost >= 0))
        //        {
        //            decimal grossMargin = ((itemSketch.Item.PriceCash.Value - itemSketch.Product.Cost.Value) / itemSketch.Item.PriceCash.Value) * 100;

        //            itemSketch_DB.GrossMargin = System.Math.Round(grossMargin, 2);
        //        }
        //        else
        //        {
        //            itemSketch_DB.GrossMargin = null;
        //        }

        //        #endregion ItemDisplayPrice

        //        #region CreateAndUpdate

        //        DateTime nowDateTime = DateTime.Now;

        //        itemSketch_DB.UpdateDate = nowDateTime;
        //        itemSketch_DB.UpdateUser = itemSketch.CreateAndUpdate.CreateUser.ToString();

        //        #endregion CreateAndUpdate

        //        result.Body = itemSketch_DB;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.IsSuccess = false;
        //        result.Body = null;
        //        result.Msg = "資料錯誤";
        //        log.Info(string.Format("/ExamineBatchService/MakeMakeItemSketch_DB_ListEditData error: 組合草稿 DB Model (ListEditData) 失敗(expection); ErrorMessage = {0}; StackTrace = {1}.", ex.Message, ex.StackTrace));
        //    }

        //    result.Code = SetResponseCode(result.IsSuccess);

        //    return result;
        //}





        ///// <summary>
        ///// 取得 DelvType
        ///// </summary>
        ///// <param name="accountType">Account Type</param>
        ///// <param name="shipType">Ship Type</param>
        ///// <returns>DelvType</returns>
        //private ActionResponse<int?> GetDelvType(string accountType, string shipType)
        //{
        //    ActionResponse<int?> result = new ActionResponse<int?>();
        //    result.Body = null;
        //    result.IsSuccess = true;
        //    result.Msg = string.Empty;

        //    if (result.IsSuccess)
        //    {
        //        if (accountType == "S")
        //        {
        //            // 由運送方決定DelvType
        //            switch (shipType)
        //            {
        //                case "V":
        //                case "S":
        //                    {
        //                        result.Body = 2;
        //                        break;
        //                    }
        //                case "N":
        //                    {
        //                        result.Body = 8;
        //                        break;
        //                    }
        //            }
        //        }

        //        if (accountType == "V")
        //        {
        //            // 由運送方決定DelvType
        //            switch (shipType)
        //            {
        //                case "V":
        //                case "S":
        //                    {
        //                        result.Body = 7;
        //                        break;
        //                    }
        //                case "N":
        //                    {
        //                        result.Body = 9;
        //                        break;
        //                    }
        //            }
        //        }
        //    }

        //    if (result.Body == null)
        //    {
        //        result.IsSuccess = false;
        //    }

        //    result.Code = SetResponseCode(result.IsSuccess);

        //    return result;
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sellerID"></param>
        ///// <returns></returns>
        //private ActionResponse<string> GetAccountType(int sellerID)
        //{
        //    ActionResponse<string> result = new ActionResponse<string>();
        //    result.Body = string.Empty;
        //    result.IsSuccess = true;
        //    result.Msg = string.Empty;

        //    DB.TWSellerPortalDBContext dbSellerPortal = new TWSellerPortalDBContext();

        //    try
        //    {
        //        result.Body = dbSellerPortal.Seller_BasicInfo.Where(x => x.SellerID == sellerID).Select(x => x.AccountTypeCode).FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Msg = "檢查賣家/供應商資料錯誤";
        //        result.IsSuccess = false;
        //        log.Info(string.Format("ExamineBatchService/GetAccountType error: 取得 AccountType 失敗(expection); SellerID = {0}; ErrorMessage = {1}; StackTrace = {2}.", sellerID, ex.Message, ex.StackTrace));
        //    }

        //    if (string.IsNullOrEmpty(result.Body) || (result.Body != "S" && result.Body != "V"))
        //    {
        //        result.IsSuccess = false;

        //        if (string.IsNullOrEmpty(result.Body))
        //        {
        //            log.Info(string.Format("ExamineBatchService/GetAccountType error: 查無 AccountType 資訊。"));
        //            result.Msg = "查無 賣家/供應商 資訊";

        //        }

        //        if (result.Body != "S" && result.Body != "V")
        //        {
        //            log.Info(string.Format("ExamineBatchService/GetAccountType error: AccountType = {0} 為無效值。", result.Body));
        //            result.Msg = "賣家/供應商 無效";
        //        }
        //    }

        //    result.Code = SetResponseCode(result.IsSuccess);

        //    return result;
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="itemSketchCell"></param>
        ///// <returns></returns>
        //public ActionResponse<string> CheckInput_ItemSketch(ItemSketch itemSketchCell)
        //{
        //    ActionResponse<string> result = new ActionResponse<string>();
        //    result.IsSuccess = true;
        //    result.Msg = string.Empty;
        //    string errorMessage = string.Empty;
        //    #region 因查詢 DB 用，或其它欄位給值判斷用列為必填項目
        //    #region 檢查 UserID
        //    ActionResponse<bool> checkUserIDResult_Create = CheckUserID(itemSketchCell.CreateAndUpdate.CreateUser);
        //    ActionResponse<bool> checkUserIDResult_Update = CheckUserID(itemSketchCell.CreateAndUpdate.UpdateUser);
        //    if (checkUserIDResult_Create.IsSuccess == false && checkUserIDResult_Update.IsSuccess == false)
        //    {
        //        errorMessage += "未輸入 CreateUser 及 UpdateUser 資訊; ";
        //    }
        //    else
        //    {
        //        if (checkUserIDResult_Create.IsSuccess && checkUserIDResult_Update.IsSuccess)
        //        {
        //            if (itemSketchCell.CreateAndUpdate.CreateUser != itemSketchCell.CreateAndUpdate.UpdateUser)
        //            {
        //                errorMessage += "CreateUser 及 UpdateUser 值不一致; ";
        //            }
        //        }
        //        else
        //        {
        //            if (checkUserIDResult_Create.IsSuccess == false)
        //            {
        //                itemSketchCell.CreateAndUpdate.CreateUser = itemSketchCell.CreateAndUpdate.UpdateUser;
        //            }

        //            if (checkUserIDResult_Update.IsSuccess == false)
        //            {
        //                itemSketchCell.CreateAndUpdate.UpdateUser = itemSketchCell.CreateAndUpdate.CreateUser;
        //            }
        //        }
        //    }
        //    #endregion
        //    #region 檢查 ShipType
        //    ActionResponse<bool> checkShipTypeResult = CheckShipType(itemSketchCell.Item.ShipType);

        //    if (checkShipTypeResult.IsSuccess == false)
        //    {
        //        errorMessage += checkShipTypeResult.Msg + "; ";
        //    }
        //    #endregion
        //    #region 檢查 Seller ID
        //    ActionResponse<bool> checkSellerIDResult = CheckSellerID(itemSketchCell.Item.SellerID.Value);

        //    if (checkSellerIDResult.IsSuccess == false)
        //    {
        //        errorMessage += checkSellerIDResult.Msg + "; ";
        //    }
        //    #endregion
        //    #endregion
        //    /*---------------------------------------------------------------*/
        //    #region 因畫面上呈現的選項沒有空值，而列為必填項目
        //    #region 檢查 IsNew

        //    ActionResponse<bool> checkIsNewResult = CheckIsNew(itemSketchCell.Item.IsNew);

        //    if (checkIsNewResult.IsSuccess == false)
        //    {
        //        errorMessage += checkIsNewResult.Msg + "; ";
        //    }

        //    #endregion 檢查 IsNew

        //    #region 檢查 Is18

        //    ActionResponse<bool> checkIs18Result = CheckIs18(itemSketchCell.Product.Is18);

        //    if (checkIs18Result.IsSuccess == false)
        //    {
        //        errorMessage += checkIs18Result.Msg + "; ";
        //    }

        //    #endregion 檢查 Is18

        //    #region 檢查 IsChokingDanger

        //    ActionResponse<bool> checkIsChokingDangerResult = CheckIsChokingDanger(itemSketchCell.Product.IsChokingDanger);

        //    if (checkIsChokingDangerResult.IsSuccess == false)
        //    {
        //        errorMessage += checkIsChokingDangerResult.Msg + "; ";
        //    }

        //    #endregion 檢查 IsChokingDanger

        //    #region 檢查 IsShipDanger

        //    ActionResponse<bool> checkIsShipDangerResult = CheckIsShipDanger(itemSketchCell.Product.IsShipDanger);

        //    if (checkIsShipDangerResult.IsSuccess == false)
        //    {
        //        errorMessage += checkIsShipDangerResult.Msg + "; ";
        //    }

        //    #endregion

        //    #region 檢查 ItemPackage

        //    ActionResponse<bool> checkItemPackageResult = CheckItemPackage(itemSketchCell.Item.ItemPackage);

        //    if (checkItemPackageResult.IsSuccess == false)
        //    {
        //        errorMessage += checkItemPackageResult.Msg + "; ";
        //    }

        //    #endregion
        //    #endregion
        //    /*---------------------------------------------------------------*/
        //    #region 輸入欄位最多可填字數檢查

        //    #region 檢查 BarCode

        //    ActionResponse<bool> checkBarCodeResult = CheckBarCode(itemSketchCell.Product.BarCode);

        //    if (checkBarCodeResult.IsSuccess == false)
        //    {
        //        errorMessage += checkBarCodeResult.Msg + "; ";
        //    }

        //    #endregion 檢查 BarCode

        //    #region 檢查 MenufacturePartNum

        //    ActionResponse<bool> checkMenufacturePartNumResult = CheckMenufacturePartNum(itemSketchCell.Product.MenufacturePartNum);

        //    if (checkMenufacturePartNumResult.IsSuccess == false)
        //    {
        //        errorMessage += checkMenufacturePartNumResult.Msg + "; ";
        //    }

        //    #endregion 檢查 MenufacturePartNum

        //    #region 檢查 Model

        //    ActionResponse<bool> checkModelResult = CheckModel(itemSketchCell.Product.Model);

        //    if (checkModelResult.IsSuccess == false)
        //    {
        //        errorMessage += checkModelResult.Msg + "; ";
        //    }

        //    #endregion 檢查 Model

        //    #region 檢查 UPC

        //    ActionResponse<bool> checkUPCResult = CheckUPC(itemSketchCell.Product.UPC);

        //    if (checkUPCResult.IsSuccess == false)
        //    {
        //        errorMessage += checkUPCResult.Msg + "; ";
        //    }

        //    #endregion 檢查 UPC

        //    #region 檢查 DelvDate

        //    ActionResponse<bool> checkDelvDateResult = CheckDelvDate(itemSketchCell.Item.DelvDate);

        //    if (checkDelvDateResult.IsSuccess == false)
        //    {
        //        errorMessage += checkDelvDateResult.Msg + "; ";
        //    }

        //    #endregion 檢查 DelvDate

        //    #endregion
        //    /*---------------------------------------------------------------*/
        //    #region 填寫檢查失敗錯誤訊息
        //    if (string.IsNullOrEmpty(errorMessage) == false)
        //    {
        //        logger.Info("批次送審建立草稿失敗: " + errorMessage);
        //        result.Msg = errorMessage;
        //        result.IsSuccess = false;
        //        return result;
        //    }
        //    #endregion
        //    result.IsSuccess = true;
        //    return result;
        //}
        ///// <summary>
        ///// 檢查商品資訊輸入資料
        ///// </summary>
        ///// <param name="itemSketchCell">商品資訊</param>
        ///// <returns>檢查結果</returns>
        //public ActionResponse<List<string>> CheckInput_ItemSketch(List<ItemSketch> itemSketchCell)
        //{
        //    ActionResponse<List<string>> result = new ActionResponse<List<string>>();
        //    result.Body = new List<string>();
        //    result.IsSuccess = true;
        //    result.Msg = string.Empty;
        //    int remeberItems = 1;

        //    if (itemSketchCell.Count() > 0)
        //    {
        //        int itemSketchCellCount = 0;
        //        foreach (ItemSketch itemSketch in itemSketchCell)
        //        {
        //            itemSketchCellCount++;
        //            string errorMessage = string.Empty;
        //            errorMessage = "第 " + remeberItems + " 筆資料: ";
        //            #region 因查詢 DB 用，或其它欄位給值判斷用列為必填項目

        //            #region 檢查 UserID

        //            ActionResponse<bool> checkUserIDResult_Create = CheckUserID(itemSketch.CreateAndUpdate.CreateUser);
        //            ActionResponse<bool> checkUserIDResult_Update = CheckUserID(itemSketch.CreateAndUpdate.UpdateUser);

        //            if (checkUserIDResult_Create.IsSuccess == false && checkUserIDResult_Update.IsSuccess == false)
        //            {
        //                errorMessage += "未輸入 CreateUser 及 UpdateUser 資訊; ";
        //            }
        //            else
        //            {
        //                if (checkUserIDResult_Create.IsSuccess && checkUserIDResult_Update.IsSuccess)
        //                {
        //                    if (itemSketch.CreateAndUpdate.CreateUser != itemSketch.CreateAndUpdate.UpdateUser)
        //                    {
        //                        errorMessage += "CreateUser 及 UpdateUser 值不一致; ";
        //                    }
        //                }
        //                else
        //                {
        //                    if (checkUserIDResult_Create.IsSuccess == false)
        //                    {
        //                        itemSketch.CreateAndUpdate.CreateUser = itemSketch.CreateAndUpdate.UpdateUser;
        //                    }

        //                    if (checkUserIDResult_Update.IsSuccess == false)
        //                    {
        //                        itemSketch.CreateAndUpdate.UpdateUser = itemSketch.CreateAndUpdate.CreateUser;
        //                    }
        //                }
        //            }

        //            #endregion

        //            #region 檢查 ShipType

        //            ActionResponse<bool> checkShipTypeResult = CheckShipType(itemSketch.Item.ShipType);

        //            if (checkShipTypeResult.IsSuccess == false)
        //            {
        //                errorMessage += checkShipTypeResult.Msg + "; ";
        //            }

        //            #endregion

        //            #region 檢查 Seller ID

        //            ActionResponse<bool> checkSellerIDResult = CheckSellerID(itemSketch.Item.SellerID.Value);

        //            if (checkSellerIDResult.IsSuccess == false)
        //            {
        //                errorMessage += checkSellerIDResult.Msg + "; ";
        //            }

        //            #endregion

        //            #endregion
        //            /*---------------------------------------------------------------*/
        //            #region 因畫面上呈現的選項沒有空值，而列為必填項目

        //            #region 檢查 IsNew

        //            ActionResponse<bool> checkIsNewResult = CheckIsNew(itemSketch.Item.IsNew);

        //            if (checkIsNewResult.IsSuccess == false)
        //            {
        //                errorMessage += checkIsNewResult.Msg + "; ";
        //            }

        //            #endregion 檢查 IsNew

        //            #region 檢查 Is18

        //            ActionResponse<bool> checkIs18Result = CheckIs18(itemSketch.Product.Is18);

        //            if (checkIs18Result.IsSuccess == false)
        //            {
        //                errorMessage += checkIs18Result.Msg + "; ";
        //            }

        //            #endregion 檢查 Is18

        //            #region 檢查 IsChokingDanger

        //            ActionResponse<bool> checkIsChokingDangerResult = CheckIsChokingDanger(itemSketch.Product.IsChokingDanger);

        //            if (checkIsChokingDangerResult.IsSuccess == false)
        //            {
        //                errorMessage += checkIsChokingDangerResult.Msg + "; ";
        //            }

        //            #endregion 檢查 IsChokingDanger

        //            #region 檢查 IsShipDanger

        //            ActionResponse<bool> checkIsShipDangerResult = CheckIsShipDanger(itemSketch.Product.IsShipDanger);

        //            if (checkIsShipDangerResult.IsSuccess == false)
        //            {
        //                errorMessage += checkIsShipDangerResult.Msg + "; ";
        //            }

        //            #endregion

        //            #region 檢查 ItemPackage

        //            ActionResponse<bool> checkItemPackageResult = CheckItemPackage(itemSketch.Item.ItemPackage);

        //            if (checkItemPackageResult.IsSuccess == false)
        //            {
        //                errorMessage += checkItemPackageResult.Msg + "; ";
        //            }

        //            #endregion

        //            #endregion
        //            /*---------------------------------------------------------------*/
        //            #region 輸入欄位最多可填字數檢查

        //            #region 檢查 BarCode

        //            ActionResponse<bool> checkBarCodeResult = CheckBarCode(itemSketch.Product.BarCode);

        //            if (checkBarCodeResult.IsSuccess == false)
        //            {
        //                errorMessage += checkBarCodeResult.Msg + "; ";
        //            }

        //            #endregion 檢查 BarCode

        //            #region 檢查 MenufacturePartNum

        //            ActionResponse<bool> checkMenufacturePartNumResult = CheckMenufacturePartNum(itemSketch.Product.MenufacturePartNum);

        //            if (checkMenufacturePartNumResult.IsSuccess == false)
        //            {
        //                errorMessage += checkMenufacturePartNumResult.Msg + "; ";
        //            }

        //            #endregion 檢查 MenufacturePartNum

        //            #region 檢查 Model

        //            ActionResponse<bool> checkModelResult = CheckModel(itemSketch.Product.Model);

        //            if (checkModelResult.IsSuccess == false)
        //            {
        //                errorMessage += checkModelResult.Msg + "; ";
        //            }

        //            #endregion 檢查 Model

        //            #region 檢查 UPC

        //            ActionResponse<bool> checkUPCResult = CheckUPC(itemSketch.Product.UPC);

        //            if (checkUPCResult.IsSuccess == false)
        //            {
        //                errorMessage += checkUPCResult.Msg + "; ";
        //            }

        //            #endregion 檢查 UPC

        //            #region 檢查 DelvDate

        //            ActionResponse<bool> checkDelvDateResult = CheckDelvDate(itemSketch.Item.DelvDate);

        //            if (checkDelvDateResult.IsSuccess == false)
        //            {
        //                errorMessage += checkDelvDateResult.Msg + "; ";
        //            }

        //            #endregion 檢查 DelvDate

        //            #endregion
        //            /*---------------------------------------------------------------*/
        //            #region 填寫檢查失敗錯誤訊息
        //            /*---------------------------------------------------------------*/
        //            if (string.IsNullOrEmpty(errorMessage) == false)
        //            {
        //                result.Body.Add("False: " + errorMessage);

        //                #region 錯誤訊息抬頭、連接符號

        //                //if (string.IsNullOrEmpty(result.Msg))
        //                //{
        //                //    if (itemSketch.ID > 0)
        //                //    {
        //                //        result.Msg += "檢查失敗(資料順序引數)：";
        //                //    }
        //                //    else
        //                //    {
        //                //        result.Msg += "檢查失敗草稿 ID：";
        //                //    }
        //                //}
        //                //else
        //                //{
        //                //    result.Msg += "、";
        //                //}

        //                #endregion

        //                #region 錯誤資料索引值

        //                //if (itemSketch.ID > 0)
        //                //{
        //                //    result.Msg += itemSketchCellCount.ToString();

        //                //    // 填入 log 前，先加上資料的索引值
        //                //    errorMessage = string.Format("(商品資訊第 {0} 筆資料) {1}", itemSketchCellCount, errorMessage);
        //                //}
        //                //else
        //                //{
        //                //    result.Msg += itemSketch.ID.ToString();

        //                //    // 填入 log 前，先加上資料的索引值
        //                //    errorMessage = string.Format("(ItemSketch = {0}) {1}", itemSketch.ID, errorMessage);
        //                //}

        //                #endregion

        //                log.Info(string.Format("草稿儲存檢查失敗：{0}。", errorMessage));
        //            }

        //            #endregion
        //            remeberItems++;
        //        }

        //        if (result.Body.Count > 0 || string.IsNullOrEmpty(result.Msg) == false)
        //        {
        //            result.IsSuccess = false;
        //        }
        //    }
        //    else
        //    {
        //        result.IsSuccess = false;
        //        log.Info("未輸入商品資訊。");
        //    }

        //    result.Code = SetResponseCode(result.IsSuccess);

        //    return result;
        //}
        //#region 送草稿區前的資料檢查
        ///// <summary>
        ///// 檢查 User ID
        ///// </summary>
        ///// <param name="userID">User ID</param>
        ///// <returns>檢查結果</returns>
        //private ActionResponse<bool> CheckUserID(int userID)
        //{
        //    ActionResponse<bool> result = new ActionResponse<bool>();
        //    result.Body = false;
        //    result.IsSuccess = false;
        //    result.Msg = string.Empty;

        //    if (userID <= 0)
        //    {
        //        result.Msg = "UserID ID 不可小於等於 0。";
        //        result.IsSuccess = false;
        //    }

        //    if (result.Msg == string.Empty)
        //    {
        //        result.Body = true;
        //        result.IsSuccess = true;
        //    }

        //    result.Code = SetResponseCode(result.IsSuccess);

        //    return result;
        //}
        ///// <summary>
        ///// 檢查運送類型
        ///// </summary>
        ///// <param name="shipType">運送類型</param>
        ///// <returns>檢查結果</returns>
        //private ActionResponse<bool> CheckShipType(string shipType)
        //{
        //    ActionResponse<bool> result = new ActionResponse<bool>();
        //    result.Body = false;
        //    result.IsSuccess = false;
        //    result.Msg = string.Empty;

        //    DB.TWSqlDBContext dbFront = new TWSqlDBContext();
        //    result.IsSuccess = true;

        //    if (string.IsNullOrEmpty(shipType))
        //    {
        //        result.Msg = "運送類型為必填。";
        //    }
        //    else
        //    {
        //        if (shipType.Length != 1)
        //        {
        //            result.Msg = "運送類型字數不可大於 1。";
        //        }
        //        else if (shipType != "S" && shipType != "N")
        //        {
        //            result.Msg = "運送類型輸入值錯誤。";
        //        }
        //    }

        //    if (result.Msg == string.Empty)
        //    {
        //        result.Body = true;
        //        result.IsSuccess = true;
        //    }

        //    result.Code = SetResponseCode(result.IsSuccess);

        //    return result;
        //}
        ///// <summary>
        ///// 檢查商家 ID
        ///// </summary>
        ///// <param name="sellerID">商家 ID</param>
        ///// <returns>檢查結果</returns>
        //private ActionResponse<bool> CheckSellerID(int? sellerID)
        //{
        //    ActionResponse<bool> result = new ActionResponse<bool>();
        //    result.Body = false;
        //    result.IsSuccess = false;
        //    result.Msg = string.Empty;

        //    DB.TWSqlDBContext dbFront = new TWSqlDBContext();
        //    result.IsSuccess = true;

        //    if (sellerID == null)
        //    {
        //        result.Msg = "商家 ID 為必填。";
        //    }
        //    else
        //    {
        //        if (sellerID < 0)
        //        {
        //            result.Msg = "商家 ID 不可小於 0。";
        //        }
        //    }

        //    if (result.Msg == string.Empty)
        //    {
        //        result.Body = true;
        //        result.IsSuccess = true;
        //    }

        //    result.Code = SetResponseCode(result.IsSuccess);

        //    return result;
        //}
        ///// <summary>
        ///// 檢查是否為18歲商品
        ///// </summary>
        ///// <param name="is18">是否為18歲商品</param>
        ///// <returns>檢查結果</returns>
        //private ActionResponse<bool> CheckIs18(string is18)
        //{
        //    ActionResponse<bool> result = new ActionResponse<bool>();
        //    result.Body = false;
        //    result.IsSuccess = false;
        //    result.Msg = string.Empty;

        //    if (string.IsNullOrEmpty(is18))
        //    {
        //        result.Msg = "是否為18歲商品為必填。";
        //    }
        //    else
        //    {
        //        if (is18.Length != 1)
        //        {
        //            result.Msg = "是否為18歲商品字數不可大於 1。";
        //        }
        //        else if (is18 != "Y" && is18 != "N")
        //        {
        //            result.Msg = "是否為18歲商品輸入值錯誤。";
        //        }
        //    }

        //    if (result.Msg == string.Empty)
        //    {
        //        result.Body = true;
        //        result.IsSuccess = true;
        //    }

        //    result.Code = SetResponseCode(result.IsSuccess);

        //    return result;
        //}
        ///// <summary>
        ///// 檢查是否有窒息危險
        ///// </summary>
        ///// <param name="isChokingDanger">是否有窒息危險</param>
        ///// <returns>檢查結果</returns>
        //private ActionResponse<bool> CheckIsChokingDanger(string isChokingDanger)
        //{
        //    ActionResponse<bool> result = new ActionResponse<bool>();
        //    result.Body = false;
        //    result.IsSuccess = false;
        //    result.Msg = string.Empty;

        //    if (string.IsNullOrEmpty(isChokingDanger))
        //    {
        //        result.Msg = "是否有窒息危險為必填。";
        //    }
        //    else
        //    {
        //        if (isChokingDanger.Length != 1)
        //        {
        //            result.Msg = "是否有窒息危險字數不可大於 1。";
        //        }
        //        else if (isChokingDanger != "Y" && isChokingDanger != "N")
        //        {
        //            result.Msg = "是否有窒息危險輸入值錯誤。";
        //        }
        //    }

        //    if (result.Msg == string.Empty)
        //    {
        //        result.Body = true;
        //        result.IsSuccess = true;
        //    }

        //    result.Code = SetResponseCode(result.IsSuccess);

        //    return result;
        //}
        ///// <summary>
        ///// 檢查是否有遞送危險
        ///// </summary>
        ///// <param name="isShipDanger">是否有遞送危險</param>
        ///// <returns>檢查結果</returns>
        //private ActionResponse<bool> CheckIsShipDanger(string isShipDanger)
        //{
        //    ActionResponse<bool> result = new ActionResponse<bool>();
        //    result.Body = false;
        //    result.IsSuccess = false;
        //    result.Msg = string.Empty;

        //    if (string.IsNullOrEmpty(isShipDanger))
        //    {
        //        result.Msg = "是否有遞送危險為必填。";
        //    }
        //    else
        //    {
        //        if (isShipDanger.Length != 1)
        //        {
        //            result.Msg = "是否有遞送危險字數不可大於 1。";
        //        }
        //        else if (isShipDanger != "Y" && isShipDanger != "N")
        //        {
        //            result.Msg = "是否有遞送危險輸入值錯誤。";
        //        }
        //    }

        //    if (result.Msg == string.Empty)
        //    {
        //        result.Body = true;
        //        result.IsSuccess = true;
        //    }

        //    result.Code = SetResponseCode(result.IsSuccess);

        //    return result;
        //}
        ///// <summary>
        ///// 檢查商品包裝
        ///// </summary>
        ///// <param name="itemPackage">商品包裝</param>
        ///// <returns>檢查結果</returns>
        //private ActionResponse<bool> CheckItemPackage(string itemPackage)
        //{
        //    ActionResponse<bool> result = new ActionResponse<bool>();
        //    result.Body = false;
        //    result.IsSuccess = false;
        //    result.Msg = string.Empty;

        //    if (string.IsNullOrEmpty(itemPackage))
        //    {
        //        result.Msg = "商品包裝為必填。";
        //    }
        //    else
        //    {
        //        if (itemPackage.Length != 1)
        //        {
        //            result.Msg = "商品包裝字數不可大於 1。";
        //        }
        //        else if (itemPackage != "0" && itemPackage != "1")
        //        {
        //            result.Msg = "商品包裝輸入值錯誤。";
        //        }
        //    }

        //    if (result.Msg == string.Empty)
        //    {
        //        result.Body = true;
        //        result.IsSuccess = true;
        //    }

        //    result.Code = SetResponseCode(result.IsSuccess);

        //    return result;
        //}
        ///// <summary>
        ///// 檢查條碼
        ///// </summary>
        ///// <param name="barCode">條碼</param>
        ///// <returns>檢查結果</returns>
        //private ActionResponse<bool> CheckBarCode(string barCode)
        //{
        //    ActionResponse<bool> result = new ActionResponse<bool>();
        //    result.Body = false;
        //    result.IsSuccess = false;
        //    result.Msg = string.Empty;

        //    if (!string.IsNullOrEmpty(barCode))
        //    {
        //        if (barCode.Length > 50)
        //        {
        //            result.Msg = "條碼字數上限為 50 字。";
        //        }
        //    }

        //    if (result.Msg == string.Empty)
        //    {
        //        result.Body = true;
        //        result.IsSuccess = true;
        //    }

        //    result.Code = SetResponseCode(result.IsSuccess);

        //    return result;
        //}
        ///// <summary>
        ///// 檢查商家商品編號
        ///// </summary>
        ///// <param name="menufacturePartNum">商家商品編號</param>
        ///// <returns>檢查結果</returns>
        //private ActionResponse<bool> CheckMenufacturePartNum(string menufacturePartNum)
        //{
        //    ActionResponse<bool> result = new ActionResponse<bool>();
        //    result.Body = false;
        //    result.IsSuccess = false;
        //    result.Msg = string.Empty;

        //    if (!string.IsNullOrEmpty(menufacturePartNum))
        //    {
        //        if (menufacturePartNum.Length > 150)
        //        {
        //            result.Msg = "商家商品編號字數上限為 150 字。";
        //        }
        //    }

        //    if (result.Msg == string.Empty)
        //    {
        //        result.Body = true;
        //        result.IsSuccess = true;
        //    }

        //    result.Code = SetResponseCode(result.IsSuccess);

        //    return result;
        //}
        ///// <summary>
        ///// 檢查商品型號
        ///// </summary>
        ///// <param name="model">商品型號</param>
        ///// <returns>檢查結果</returns>
        //private ActionResponse<bool> CheckModel(string model)
        //{
        //    ActionResponse<bool> result = new ActionResponse<bool>();
        //    result.Body = false;
        //    result.IsSuccess = false;
        //    result.Msg = string.Empty;

        //    if (!string.IsNullOrEmpty(model))
        //    {
        //        if (model.Length > 30)
        //        {
        //            result.Msg = "商品型號字數上限為 30 字。";
        //        }
        //    }

        //    if (result.Msg == string.Empty)
        //    {
        //        result.Body = true;
        //        result.IsSuccess = true;
        //    }

        //    result.Code = SetResponseCode(result.IsSuccess);

        //    return result;
        //}
        ///// <summary>
        ///// 檢查 UPC
        ///// </summary>
        ///// <param name="upc">UPC</param>
        ///// <returns>檢查結果</returns>
        //private ActionResponse<bool> CheckUPC(string upc)
        //{
        //    ActionResponse<bool> result = new ActionResponse<bool>();
        //    result.Body = false;
        //    result.IsSuccess = false;
        //    result.Msg = string.Empty;

        //    if (!string.IsNullOrEmpty(upc))
        //    {
        //        if (upc.Length > 15)
        //        {
        //            result.Msg = "UPC字數上限為 15 字。";
        //        }
        //    }

        //    if (result.Msg == string.Empty)
        //    {
        //        result.Body = true;
        //        result.IsSuccess = true;
        //    }

        //    result.Code = SetResponseCode(result.IsSuccess);

        //    return result;
        //}
        ///// <summary>
        ///// 檢查到貨天數
        ///// </summary>
        ///// <param name="delvDate">到貨天數</param>
        ///// <returns>檢查結果</returns>
        //private ActionResponse<bool> CheckDelvDate(string delvDate)
        //{
        //    ActionResponse<bool> result = new ActionResponse<bool>();
        //    result.Body = false;
        //    result.IsSuccess = false;
        //    result.Msg = string.Empty;

        //    if (!string.IsNullOrEmpty(delvDate))
        //    {
        //        if (delvDate.Length > 50)
        //        {
        //            result.Msg = "到貨天數字數上限為 50 字。";
        //        }
        //    }

        //    if (result.Msg == string.Empty)
        //    {
        //        result.Body = true;
        //        result.IsSuccess = true;
        //    }

        //    result.Code = SetResponseCode(result.IsSuccess);

        //    return result;
        //}
        ///// <summary>
        ///// 檢查商品成色
        ///// </summary>
        ///// <param name="isNew">商品成色</param>
        ///// <returns>檢查結果</returns>
        //private ActionResponse<bool> CheckIsNew(string isNew)
        //{
        //    ActionResponse<bool> result = new ActionResponse<bool>();
        //    result.Body = false;
        //    result.IsSuccess = false;
        //    result.Msg = string.Empty;

        //    if (string.IsNullOrEmpty(isNew))
        //    {
        //        result.Msg = "商品成色為必填。";
        //    }
        //    else
        //    {
        //        if (isNew.Length != 1)
        //        {
        //            result.Msg = "商品成色字數不可大於 1。";
        //        }
        //        else if (isNew != "Y" && isNew != "N")
        //        {
        //            result.Msg = "商品成色輸入值錯誤。";
        //        }
        //    }

        //    if (result.Msg == string.Empty)
        //    {
        //        result.Body = true;
        //        result.IsSuccess = true;
        //    }

        //    result.Code = SetResponseCode(result.IsSuccess);

        //    return result;
        //}
        //#endregion

        ///// <summary>
        ///// 填寫 Response Code
        ///// </summary>
        ///// <param name="isSuccess">成功、失敗資訊</param>
        ///// <returns>Response Code</returns>
        //private int SetResponseCode(bool isSuccess)
        //{
        //    if (isSuccess)
        //    {
        //        return (int)ResponseCode.Success;
        //    }
        //    else
        //    {
        //        return (int)ResponseCode.Error;
        //    }
        //}
        #endregion
        #region 批次送審 沒有經過草稿區建立草稿
        public ActionResponse<List<string>> ItemTempBatchService(List<ItemSketch> batchItemTempCreation, string userEmail, string passWord)
        {
            ActionResponse<List<string>> result = new ActionResponse<List<string>>();
            result.IsSuccess = true;
            TWNewEgg.DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            TWNewEgg.DB.TWSqlDBContext db_front = new DB.TWSqlDBContext();
            List<string> recordErrorMsg = new List<string>();
            #region 檢查上傳的 Email and password 在 DB 使否有對應的使用者
            //檢查是否有這個 user
            var checkVendorIsCorrect = checkUserExist(userEmail, passWord);
            if (checkVendorIsCorrect.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = checkVendorIsCorrect.Msg;
                recordErrorMsg.Add(checkVendorIsCorrect.Msg);
                result.Body = recordErrorMsg;
                return result;
            }
            #endregion
            int userid = checkVendorIsCorrect.Body.UserID;
            int sellerid = checkVendorIsCorrect.Body.SellerID.GetValueOrDefault();
            string AccountTypeCode = spdb.Seller_BasicInfo.Where(p => p.SellerID == sellerid).Select(p => p.AccountTypeCode).FirstOrDefault();
            Dictionary<int, string> dic_msg = new Dictionary<int, string>();//存放處理訊息
            Dictionary<int, TWNewEgg.API.Models.ItemSketch> dic_model = new Dictionary<int, ItemSketch>();//存放要送審的資料
            //List<string> returnMsg = new List<string>();
            #region 檢查傳過來的資料是否有錯誤
            int _intTempNumber = 1;
            foreach (var itemcheck in batchItemTempCreation)
            {
                //進行資料完整性的檢查
                var columnCheck = this.batchToExamine(itemcheck);
                //檢查資料有錯誤
                if (columnCheck.IsSuccess == false)
                {
                    //若資料檢查有錯誤，則把第幾筆資料跟錯誤訊息記錄起來
                    dic_msg.Add(_intTempNumber, columnCheck.Msg);
                }
                else
                {
                    //檢查資料 Y 跟 N 是不是小寫, 是的話 轉換成大寫的 Y 跟 N
                    itemcheck.Item.IsNew = this.ConvertYN(itemcheck.Item.IsNew);
                    itemcheck.Item.ShipType = this.ConvertYN(itemcheck.Item.ShipType);
                    itemcheck.Product.Is18 = this.ConvertYN(itemcheck.Product.Is18);
                    itemcheck.Product.IsChokingDanger = this.ConvertYN(itemcheck.Product.IsChokingDanger);
                    itemcheck.Product.IsShipDanger = this.ConvertYN(itemcheck.Product.IsShipDanger);
                    //若資料檢查沒有錯誤，則把第幾筆資料跟 model 記錄起來
                    dic_model.Add(_intTempNumber, itemcheck);
                }
                _intTempNumber++;
            }
            #endregion

            //要送審的 model 計數不是 0 
            if (dic_model.Count != 0)
            {
                #region 開始送審 v1.2
                //把資料無誤的 model 開始送審
                var productvertify_result = Productvertify(dic_model, sellerid, userid, AccountTypeCode);
                foreach (var dicItemMsg in productvertify_result.Body)
                {
                    dic_msg.Add(dicItemMsg.Key, dicItemMsg.Value);
                }
                result.Body = new List<string>();
                dic_msg = dic_msg.OrderBy(key => key.Key).ToDictionary(keyvalue => keyvalue.Key, keyValue => keyValue.Value);
                foreach (var item in dic_msg)
                {
                    string count = item.Key.ToString();
                    string value = item.Value;
                    result.Body.Add("第 " + count + " 筆資料: " + value);
                }
                return result;
                #endregion
                #region version 1.1
                //    if (productvertify_result.IsSuccess == true)
                //    {
                //        //送審成功且 returnMsg 沒有任何錯誤訊息存放，則通通成功
                //        if (returnMsg.Count == 0)
                //        {
                //            result.IsSuccess = true;
                //            result.Body = null;
                //            result.Msg = "送審成功";
                //            return result;
                //        }
                //        else
                //        {
                //            //送審之前的資料檢查 是有錯誤的，所以不算全部成功 所以 IsSuccess 給 false
                //            result.IsSuccess = false;
                //            result.Body = returnMsg;
                //            return result;
                //            #region temp
                //            /*送審成功，但是存放錯誤訊息的 List<string> 有存放錯誤訊息，
                //            表示送審之前的資料檢查 是有錯誤的，所以不算全部成功*/
                //            //if (productvertify_result.Body != null)
                //            //{
                //            //    foreach (var msg in productvertify_result.Body)
                //            //    {
                //            //        returnMsg.Add(msg);
                //            //    }
                //            //    result.IsSuccess = false;
                //            //    result.Body = returnMsg;
                //            //    //result.Body = productvertify_result.Body;
                //            //    return result;
                //            //}
                //            //else
                //            //{
                //            //    result.IsSuccess = false;
                //            //    result.Body = returnMsg;
                //            //    //result.Body = productvertify_result.Body;
                //            //    return result;
                //            //}
                //            #endregion
                //        }
                //        //result.IsSuccess = true;
                //        //result.Msg = productvertify_result.Msg;
                //        //return result;
                //    }
                //    else
                //    {
                //        //把送審回來的錯誤資訊寫入要回傳的 List<string>
                //        foreach (var msg in productvertify_result.Body)
                //        {
                //            //returnMsg.Add(msg);
                //        }
                //        result.IsSuccess = false;
                //        result.Body = returnMsg;
                //        //result.Body = productvertify_result.Body;
                //        return result;
                //    }

                #endregion
            }
            else
            {
                var orderbyDic = dic_msg.OrderBy(key => key.Key).ToDictionary(keyvalue => keyvalue.Key, keyValue => keyValue.Value);
                result.Body = new List<string>();
                foreach (var item in orderbyDic)
                {
                    result.Body.Add("第 " + item.Key + " 筆資料: " + item.Value);
                }
                return result;
            }

        }

        public ActionResponse<Dictionary<int, string>> Productvertify(Dictionary<int, TWNewEgg.API.Models.ItemSketch> _toVertify, int sellerid, int userid, string AccountTypeCode)
        {
            DB.TWSqlDBContext db_before = new DB.TWSqlDBContext();
            ActionResponse<List<string>> result = new ActionResponse<List<string>>();
            ActionResponse<Dictionary<int, string>> _result = new ActionResponse<Dictionary<int, string>>();
            Dictionary<int, string> dicResult = new Dictionary<int, string>();
            DateTime dateTimeMillisecond = new DateTime(0001, 01, 01, 01, 01, 01, 01);
            TWNewEgg.API.Service.ImageService imgService = new ImageService();
            string timeString = dateTimeMillisecond.ToString("yyyy-MM-dd hh:mm:ss fff");
            dateTimeMillisecond = DateTime.Now;
            //List<string> MsgList = new List<string>();
            logger.Info("批次送審開始: sellerid is: " + sellerid + "; userid is: " + userid);
            foreach (var dic_item in _toVertify)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    #region insert to ProductTemp table
                    DB.TWSQLDB.Models.ProductTemp productTempAdd = new DB.TWSQLDB.Models.ProductTemp();
                    productTempAdd.SellerProductID = dic_item.Value.Product.SellerProductID;
                    productTempAdd.Name = dic_item.Value.Product.Name;
                    productTempAdd.NameTW = dic_item.Value.Product.Name;
                    productTempAdd.Description = dic_item.Value.Product.Description;
                    productTempAdd.DescriptionTW = dic_item.Value.Product.Description;
                    productTempAdd.SPEC = ""; //SPEC已不使用;
                    productTempAdd.ManufactureID = dic_item.Value.Product.ManufactureID.GetValueOrDefault();
                    productTempAdd.Model = dic_item.Value.Product.Model;
                    productTempAdd.BarCode = dic_item.Value.Product.BarCode;
                    productTempAdd.SellerID = sellerid;
                    var delcType = this.GetDelvType(AccountTypeCode, dic_item.Value.Item.ShipType.ToUpper());
                    if (delcType.IsSuccess == false)
                    {
                        dicResult.Add(dic_item.Key, "找不到對應的 DelvType");
                        logger.Error("sellerid is: " + sellerid + "; userid is: " + userid + ". 第 " + dic_item.Key + " 筆資料錯誤： " + delcType.Msg + ". 找不到對應的 DelvType");
                        //var errorModelMsgType = this._combineBatchError(dic_item.Value, DB.TWSELLERPORTALDB.Models.BatchAuditError.ErrorType.送審錯誤, "找不到對應的 DelvType");
                        //_listBatchError.Add(errorModelMsgType);
                        //MsgList.Add("第 " + dic_item.Key + " 筆資料錯誤");
                        scope.Dispose();
                        continue;
                    }
                    else
                    {
                        productTempAdd.DelvType = delcType.Body;
                    }
                    if (dic_item.Value.Product.PicPatch_Edit.Count == 0)
                    {
                        productTempAdd.PicStart = 0;
                        productTempAdd.PicEnd = 0;
                    }
                    else
                    {
                        productTempAdd.PicStart = 1;
                        productTempAdd.PicEnd = dic_item.Value.Product.PicPatch_Edit.Count;
                    }
                    productTempAdd.Cost = dic_item.Value.Product.Cost;
                    productTempAdd.Status = 1;
                    productTempAdd.InvoiceType = 0;//default value
                    productTempAdd.SaleType = 0;//default value
                    productTempAdd.Length = dic_item.Value.Product.Length;
                    productTempAdd.Width = dic_item.Value.Product.Width;
                    productTempAdd.Height = dic_item.Value.Product.Height;
                    productTempAdd.Weight = dic_item.Value.Product.Weight;
                    productTempAdd.CreateUser = userid.ToString();
                    productTempAdd.CreateDate = dateTimeMillisecond;
                    productTempAdd.Updated = 0;//default value
                    productTempAdd.UpdateDate = dateTimeMillisecond;
                    productTempAdd.UpdateUser = userid.ToString();
                    productTempAdd.TradeTax = 0;
                    productTempAdd.Tax = 0;//default value
                    productTempAdd.Warranty = dic_item.Value.Product.Warranty;
                    productTempAdd.UPC = dic_item.Value.Product.UPC;
                    productTempAdd.Note = dic_item.Value.Item.Note;
                    productTempAdd.IsMarket = "Y";
                    productTempAdd.Is18 = dic_item.Value.Product.Is18;
                    productTempAdd.IsShipDanger = dic_item.Value.Product.IsShipDanger;
                    productTempAdd.IsChokingDanger = dic_item.Value.Product.IsChokingDanger;
                    productTempAdd.MenufacturePartNum = "";//製造商商品編號 (沒有輸入欄位) 暫時給空的
                    db_before.ProductTemp.Add(productTempAdd);
                    try
                    {
                        db_before.SaveChanges();
                    }
                    catch (Exception error)
                    {
                        dicResult.Add(dic_item.Key, "資料錯誤");
                        logger.Error("sellerid is: " + sellerid + "; userid is: " + userid + ". errorMsg: " + error.Message + " [ErrorStackTrace]: " + error.StackTrace);
                        //var errorModelMsgType = this._combineBatchError(dic_item.Value, DB.TWSELLERPORTALDB.Models.BatchAuditError.ErrorType.送審錯誤, "資料錯誤(productTempAdd)");
                        //_listBatchError.Add(errorModelMsgType);
                        //MsgList.Add("第 " + dic_item.Key + " 筆資料錯誤");
                        scope.Dispose();
                        continue;
                    }
                    #endregion
                    //寫入itemTemp
                    var r1 = ItemtempAndItemCategorytemp(dic_item.Value, productTempAdd.ID, userid, delcType.Body, sellerid);
                    //寫入itemStocktemp
                    var r2 = Itemstocktemp(dic_item.Value, productTempAdd.ID, userid);
                    List<int> subCategoryId = new List<int>();
                    if (dic_item.Value.ItemCategory.SubCategoryID_1_Layer2 != 0 && dic_item.Value.ItemCategory.SubCategoryID_1_Layer2 != null)
                    {
                        subCategoryId.Add(dic_item.Value.ItemCategory.SubCategoryID_1_Layer2.GetValueOrDefault());
                    }
                    if (dic_item.Value.ItemCategory.SubCategoryID_2_Layer2 != 0 && dic_item.Value.ItemCategory.SubCategoryID_2_Layer2 != null)
                    {
                        subCategoryId.Add(dic_item.Value.ItemCategory.SubCategoryID_2_Layer2.GetValueOrDefault());
                    }
                    ActionResponse<string> r3 = new ActionResponse<string>();
                    //有選填誇分類，subCategoryId.Count 不是 1 就是 2
                    if (subCategoryId.Count != 0)
                    {
                        r3 = ItemCategoryTemp(r1.Body, subCategoryId, userid);
                    }
                    else
                    {
                        r3.IsSuccess = true;
                    }
                    //所有insert to db 全對
                    if (r1.IsSuccess == true && r2.IsSuccess == true && r3.IsSuccess == true)
                    {
                        scope.Complete();
                        result.IsSuccess = true;
                        dicResult.Add(dic_item.Key, "送審成功");

                        //MsgList.Add("第 " + dic_item.Key + " 筆送審成功");
                        var imgProcessResult = imgService.ItemSketchBatchImgToTemp(dic_item.Value.Product.PicPatch_Edit, r1.Body);
                        if (imgProcessResult.IsSuccess == false)
                        {
                            logger.Info("sellerid is: " + sellerid + "; userid is: " + userid + " itemtempid = " + r1.Body + " . 第 " + dic_item.Key + " 筆圖片處理裡失敗");
                        }
                        else
                        {
                            logger.Info("第 " + dic_item.Key + " 圖片處理成功");
                        }
                    }
                    else//有部分資料錯誤
                    {
                        dicResult.Add(dic_item.Key, "資料錯誤");
                        scope.Dispose();
                        //var errorModelMsgType = this._combineBatchError(dic_item.Value, DB.TWSELLERPORTALDB.Models.BatchAuditError.ErrorType.送審錯誤, "資料錯誤");
                        //_listBatchError.Add(errorModelMsgType);
                        logger.Info("sellerid is: " + sellerid + "; userid is: " + userid + ". 批次送審時 r1 or r2 or r3有錯誤");
                        //MsgList.Add("第 " + dic_item.Key + " 筆資料錯誤");
                        result.IsSuccess = false;
                    }

                }
            }
            logger.Info("批次送審結束: sellerid is: " + sellerid + "; userid is: " + userid);
            _result.Body = dicResult;
            return _result;
        }
        public ActionResponse<int> ItemtempAndItemCategorytemp(ItemSketch _dicItemSketch, int productTempID, int userid, int? DelvType, int sellerid)
        {
            ActionResponse<int> result = new ActionResponse<int>();
            DB.TWSqlDBContext db_before = new TWSqlDBContext();
            DateTime dateTimeMillisecond = new DateTime(0001, 01, 01, 01, 01, 01, 01);
            string timeString = dateTimeMillisecond.ToString("yyyy-MM-dd hh:mm:ss fff");
            dateTimeMillisecond = DateTime.Now;

            DB.TWSQLDB.Models.ItemTemp itemtempAdd = new DB.TWSQLDB.Models.ItemTemp();
            itemtempAdd.ProduttempID = productTempID;
            itemtempAdd.Name = _dicItemSketch.Product.Name;
            itemtempAdd.Sdesc = _dicItemSketch.Item.Sdesc;
            itemtempAdd.DescTW = _dicItemSketch.Product.Description;
            itemtempAdd.SpecDetail = "";//defalut value
            itemtempAdd.Spechead = _dicItemSketch.Item.Spechead;
            itemtempAdd.SaleType = 1;//defalut value
            itemtempAdd.PayType = 0;//defalut value
            itemtempAdd.Layout = 0;//defalut value
            itemtempAdd.DelvType = DelvType.GetValueOrDefault();
            itemtempAdd.DelvData = _dicItemSketch.Item.DelvDate ?? "";
            itemtempAdd.ItemNumber = "";//defalut value
            itemtempAdd.CategoryID = _dicItemSketch.ItemCategory.MainCategoryID_Layer2.GetValueOrDefault();
            if (_dicItemSketch.Product.Model == null)
            {
                itemtempAdd.Model = "";
            }
            else
            {
                itemtempAdd.Model = _dicItemSketch.Product.Model;
            }
            itemtempAdd.SellerID = sellerid;
            itemtempAdd.DateStart = _dicItemSketch.Item.DateStart.AddHours(8);
            itemtempAdd.DateEnd = _dicItemSketch.Item.DateEnd;
            itemtempAdd.DateDel = _dicItemSketch.Item.DateEnd.AddDays(1);
            itemtempAdd.Pricesgst = 0;//defalut value
            itemtempAdd.PriceCard = _dicItemSketch.Item.PriceCash.GetValueOrDefault();
            itemtempAdd.PriceCash = _dicItemSketch.Item.PriceCash.GetValueOrDefault();
            itemtempAdd.ServicePrice = 0;
            itemtempAdd.PricehpType1 = 0;//defalut value
            itemtempAdd.Pricehpinst1 = 0;//defalut value
            itemtempAdd.PricehpType2 = 0;//defalut value
            itemtempAdd.Pricehpinst2 = 0;//defalut value
            itemtempAdd.Inst0Rate = 0;//defalut value
            itemtempAdd.RedmfdbckRate = 0;//defalut value
            itemtempAdd.Coupon = "0";//defalut value
            itemtempAdd.PriceCoupon = 0;//defalut value
            itemtempAdd.PriceLocalship = 0;
            itemtempAdd.PriceGlobalship = 0;//default value
            itemtempAdd.Qty = _dicItemSketch.Item.CanSaleLimitQty.GetValueOrDefault();
            itemtempAdd.SafeQty = 0;
            itemtempAdd.QtyLimit = _dicItemSketch.Item.QtyLimit.GetValueOrDefault();
            itemtempAdd.LimitRule = "";//defalut value
            itemtempAdd.QtyReg = 0;//defalut value
            itemtempAdd.PhotoName = "";//defalut value
            itemtempAdd.HtmlName = "";//defalut value
            itemtempAdd.Showorder = 0;//defalut value
            itemtempAdd.Class = 1;//defalut value
            itemtempAdd.Status = 1;//defalut value
            itemtempAdd.ManufactureID = _dicItemSketch.Product.ManufactureID.GetValueOrDefault();
            itemtempAdd.StatusNote = "";//defalut value
            itemtempAdd.StatusDate = dateTimeMillisecond;
            if (string.IsNullOrEmpty(_dicItemSketch.Item.Note) == true)
            {
                itemtempAdd.Note = "";
            }
            else
            {
                itemtempAdd.Note = _dicItemSketch.Item.Note;
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
            itemtempAdd.CreateUser = userid.ToString();
            itemtempAdd.Updated = 0;//defalut value
            itemtempAdd.UpdateUser = userid.ToString();
            //UpdateDate(更新日期)：不給值(因為有更新才給)
            //itemtemp.UpdateDate 
            if (_dicItemSketch.Product.PicPatch_Edit.Count == 0)
            {
                itemtempAdd.PicStart = 0;
                itemtempAdd.PicEnd = 0;
            }
            else
            {
                itemtempAdd.PicStart = 1;
                itemtempAdd.PicEnd = _dicItemSketch.Product.PicPatch_Edit.Count;
            }
            itemtempAdd.MarketPrice = _dicItemSketch.Item.MarketPrice;
            itemtempAdd.WareHouseID = 0;
            itemtempAdd.ShipType = _dicItemSketch.Item.ShipType;
            itemtempAdd.Taxfee = 0;//defalut value
            itemtempAdd.ItemPackage = _dicItemSketch.Item.ItemPackage;
            itemtempAdd.IsNew = _dicItemSketch.Item.IsNew;

            if ((_dicItemSketch.Item.PriceCash != null && _dicItemSketch.Product.Cost != null) && (_dicItemSketch.Item.PriceCash > 0 && _dicItemSketch.Product.Cost >= 0))
            {
                decimal grossMargin = ((_dicItemSketch.Item.PriceCash.Value - _dicItemSketch.Product.Cost.Value) / _dicItemSketch.Item.PriceCash.Value) * 100;
                itemtempAdd.GrossMargin = System.Math.Round(grossMargin, 2);
            }
            else
            {
                itemtempAdd.GrossMargin = 0;
            }
            itemtempAdd.SubmitMan = userid.ToString();
            itemtempAdd.SubmitDate = dateTimeMillisecond;//不給值會出現datetime2無法轉換成datetime錯誤
            itemtempAdd.ApproveDate = null;
            db_before.ItemTemp.Add(itemtempAdd);
            try
            {
                //必須先savechenge 才會有itemtempAdd id
                db_before.SaveChanges();
                result.Body = itemtempAdd.ID;
                result.IsSuccess = true;
                result.Msg = "success";
                result.Code = (int)ResponseCode.Success;
            }
            catch (Exception error)
            {
                logger.Error("ErrorMessage: " + error.Message + " [ErrorStackTrace] " + error.StackTrace + "; ItemTemp insert error");
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                result.Code = (int)ResponseCode.Error;
            }
            return result;
        }
        public ActionResponse<string> Itemstocktemp(ItemSketch _itemSketchtemp, int productTempId, int userid)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            DB.TWSqlDBContext db_before = new TWSqlDBContext();
            DateTime dateTimeMillisecond = new DateTime(0001, 01, 01, 01, 01, 01, 01);
            string timeString = dateTimeMillisecond.ToString("yyyy-MM-dd hh:mm:ss fff");
            dateTimeMillisecond = DateTime.Now;

            DB.TWSQLDB.Models.ItemStocktemp _itemStocktemp = new DB.TWSQLDB.Models.ItemStocktemp();
            _itemStocktemp.producttempID = productTempId;
            _itemStocktemp.Qty = _itemSketchtemp.ItemStock.CanSaleQty.GetValueOrDefault();
            _itemStocktemp.QtyReg = 0;
            _itemStocktemp.SafeQty = _itemSketchtemp.ItemStock.InventorySafeQty.GetValueOrDefault();
            _itemStocktemp.Fdbcklmt = 0;
            _itemStocktemp.CreateUser = userid.ToString();
            _itemStocktemp.Updated = 0;
            _itemStocktemp.CreateDate = dateTimeMillisecond;
            _itemStocktemp.UpdateUser = userid.ToString();
            _itemStocktemp.UpdateDate = dateTimeMillisecond;
            db_before.ItemStocktemp.Add(_itemStocktemp);
            //itemSketch_DB.InventorySafeQty = itemSketch.ItemStock.InventorySafeQty;
            try
            {
                db_before.SaveChanges();
                result.IsSuccess = true;
                result.Code = (int)ResponseCode.Success;
                result.Msg = "success";
            }
            catch (Exception error)
            {
                logger.Error("ErrorMsg:  " + error.Message + " [ErrorStackTrace] " + error.StackTrace + "; ItemStocktemp insert error");
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                result.Code = (int)ResponseCode.Error;
            }
            return result;
        }
        #region y, n 轉換成Y, N
        public string ConvertYN(string _strTemp)
        {
            string _strConvertResult = string.Empty;
            if (_strTemp == "n")
            {
                _strConvertResult = "N";
            }
            else if (_strTemp == "y")
            {
                _strConvertResult = "Y";
            }
            else
            {
                _strConvertResult = "N";
            }
            return _strConvertResult;
        }
        #endregion
        #region 檢查進來的資料完整性
        #region 檢查 user 是否合法
        public ActionResponse<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_User> checkUserExist(string userEmail, string passWord)
        {
            ActionResponse<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_User> result = new ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_User>();
            result.IsSuccess = true;
            TWNewEgg.DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            DB.TWSELLERPORTALDB.Models.Seller_User user = spdb.Seller_User.Where(x => x.UserEmail == userEmail
                                                                                                && (x.Status == "E" || x.Status == "R")).FirstOrDefault();
            if (user == null)
            {
                result.Msg = "Email 無效";
                result.IsSuccess = false;
                return result;
            }
            else
            {
                string sellerStatus = spdb.Seller_BasicInfo.Where(x => x.SellerID == user.SellerID).Select(r => r.SellerStatus).FirstOrDefault();
                if (sellerStatus == "C")
                {
                    result.Code = (int)UserLoginingResponseCode.Accountalreadystop;
                    result.IsSuccess = false;
                    result.Msg = "Email 已被停權";
                    return result;
                }
                string Pwd = TWNewEgg.API.Service.AesEncryptor.AesEncrypt(passWord + user.RanNum);
                SHA512 sha512 = new SHA512CryptoServiceProvider(); //建立一個SHA512
                byte[] source = Encoding.Default.GetBytes(Pwd); //將字串轉為Byte[]
                byte[] crypto = sha512.ComputeHash(source); //進行SHA512加密

                string PwdHashed = Convert.ToBase64String(crypto); //把加密後的字串從Byte[]轉為字串

                user = spdb.Seller_User.Where(x => x.UserEmail == userEmail
                                                             && x.Pwd == PwdHashed
                                                             && (x.Status == "E" || x.Status == "R")).FirstOrDefault();
                if (user == null)
                {
                    result.Msg = "Email 無效";
                    result.IsSuccess = false;
                    return result;
                }
                result.Body = user;
            }
            return result;
        }
        #endregion
        #region 檢查傳進來的 model 資料是否正確
        public ActionResponse<string> batchToExamine(TWNewEgg.API.Models.ItemSketch _itemSketchOne)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWSellerPortalDBContext spdb = new TWSellerPortalDBContext();
            ActionResponse<bool> scriptCheck = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = "Success";
            //logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            int intCheckTemp = 0;
            decimal decimalTemp = 0;
            DateTime dateTimeTemp = new DateTime();
            dateTimeTemp = DateTime.Now;
            string str_msg = string.Empty;
            #region 檢查 Json 傳送來的 MODEL 內容的資料
            try
            {
                #region CanSaleLimitQty
                if (int.TryParse(_itemSketchOne.Item.CanSaleLimitQty.ToString(), out intCheckTemp) == false)
                {
                    result.IsSuccess = false;
                    str_msg = str_msg + "CanSaleLimitQty 錯誤; ";
                }
                else
                {
                    if (_itemSketchOne.Item.CanSaleLimitQty <= 0)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + "CanSaleLimitQty 必須大於 0; ";
                    }
                }
                #endregion
                #region DateStart
                if (DateTime.TryParse(_itemSketchOne.Item.DateStart.ToString(), out dateTimeTemp) == false)
                {
                    result.IsSuccess = false;
                    str_msg = str_msg + "DateStart 錯誤; ";
                }
                else
                {
                    if (_itemSketchOne.Item.DateStart > _itemSketchOne.Item.DateEnd)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + "DateStart 錯誤; ";
                    }
                }
                #endregion
                #region DelvDate
                if (string.IsNullOrEmpty(_itemSketchOne.Item.DelvDate) == false)
                {
                    scriptCheck = this.scriptCheck(_itemSketchOne.Item.DelvDate);
                    if (scriptCheck.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg = "DelvDate 錯誤: " + scriptCheck.Msg + "; ";
                    }
                }
                #region 必填時
                //if (string.IsNullOrEmpty(_itemSketchOne.Item.DelvDate) == true)
                //{
                //    result.IsSuccess = false;
                //    str_msg = str_msg + "DelvDate 錯誤; ";
                //}
                //else
                //{
                //    scriptCheck = this.scriptCheck(_itemSketchOne.Item.DelvDate);
                //    if (scriptCheck.IsSuccess == false)
                //    {
                //        result.IsSuccess = false;
                //        str_msg = str_msg = "DelvDate 錯誤: " + scriptCheck.Msg + "; ";
                //    }
                //}
                #endregion
                #endregion
                #region IsNew
                if (string.IsNullOrEmpty(_itemSketchOne.Item.IsNew) == true)
                {
                    result.IsSuccess = false;
                    str_msg = str_msg + "IsNew 錯誤，請填寫 IsNew; ";
                }
                else
                {
                    if (_itemSketchOne.Item.IsNew.ToLower() != "y" && _itemSketchOne.Item.IsNew.ToLower() != "n")
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + "IsNew 錯誤，必須為 Y(Yes) or N(No); ";
                    }
                }
                #endregion
                #region ItemPackage
                if (string.IsNullOrEmpty(_itemSketchOne.Item.ItemPackage) == true)
                {
                    result.IsSuccess = false;
                    str_msg = str_msg + "ItemPackage 錯誤; ";
                }
                else
                {
                    if (_itemSketchOne.Item.ItemPackage != "1" && _itemSketchOne.Item.ItemPackage != "0")
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + "ItemPackage 錯誤，必須為 0:零售, 1:OEM; ";
                    }
                }
                #endregion
                #region ItemQty 暫時不必要檢查
                //if (int.TryParse(item.Item.ItemQty.ToString(), out intCheckTemp) == false)
                //{
                //    result.IsSuccess = false;
                //    result.Msg = "第 " + tempNumber + " 筆資料 ItemQty 錯誤";
                //    break;
                //}
                //else
                //{
                //    if (item.Item.ItemQty <= 0)
                //    {
                //        result.IsSuccess = false;
                //        result.Msg = "第 " + tempNumber + " 筆資料 ItemQty 必須大於 0";
                //        break;
                //    }
                //}
                #endregion
                #region MarketPrice
                if (string.IsNullOrEmpty(_itemSketchOne.Item.MarketPrice.ToString()) == false)
                {
                    if (decimal.TryParse(_itemSketchOne.Item.MarketPrice.ToString(), out decimalTemp) == false)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + "MarketPrice 錯誤; ";
                    }
                    else
                    {
                        if (_itemSketchOne.Item.MarketPrice - (int)_itemSketchOne.Item.MarketPrice != 0)
                        {
                            result.IsSuccess = false;
                            str_msg = str_msg + "MarketPrice 錯誤; ";
                        }
                        else
                        {
                            if (_itemSketchOne.Item.MarketPrice <= 0)
                            {
                                result.IsSuccess = false;
                                str_msg = str_msg + "MarketPrice 必須大於 0; ";
                            }
                        }
                    }
                }
                #endregion
                #region Note
                if (string.IsNullOrEmpty(_itemSketchOne.Item.Note) == false)
                {
                    scriptCheck = this.scriptCheck(_itemSketchOne.Item.Note);
                    if (scriptCheck.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg = "Note 錯誤: " + scriptCheck.Msg + "; ";
                    }
                }
                #region 必填時
                //if (string.IsNullOrEmpty(_itemSketchOne.Item.Note) == true)
                //{
                //    result.IsSuccess = false;
                //    str_msg = str_msg + "Note 錯誤; ";
                //}
                //else
                //{
                //    scriptCheck = this.scriptCheck(_itemSketchOne.Item.Note);
                //    if (scriptCheck.IsSuccess == false)
                //    {
                //        result.IsSuccess = false;
                //        str_msg = str_msg = "Note 錯誤: " + scriptCheck.Msg + "; ";
                //    }
                //}
                #endregion
                #endregion
                #region PriceCash
                if (decimal.TryParse(_itemSketchOne.Item.PriceCash.ToString(), out decimalTemp) == false)
                {
                    result.IsSuccess = false;
                    str_msg = str_msg + "PriceCash 錯誤; ";
                }
                else
                {
                    if (_itemSketchOne.Item.PriceCash - (int)_itemSketchOne.Item.PriceCash != 0)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + "PriceCash 錯誤; ";
                    }
                    else
                    {
                        if (_itemSketchOne.Item.PriceCash <= 0)
                        {
                            result.IsSuccess = false;
                            str_msg = str_msg + "PriceCash 必須大於 0; ";
                        }
                    }
                }
                #endregion
                #region QtyLimit
                if (string.IsNullOrEmpty(_itemSketchOne.Item.QtyLimit.ToString()) == false)
                {
                    if (int.TryParse(_itemSketchOne.Item.QtyLimit.ToString(), out intCheckTemp) == false)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + "QtyLimit 錯誤; ";
                    }
                    else
                    {
                        if (_itemSketchOne.Item.QtyLimit < 0)
                        {
                            result.IsSuccess = false;
                            str_msg = str_msg + "QtyLimit 必須大於 0; ";
                        }
                    }
                }
                #endregion
                #region Sdesc
                if (string.IsNullOrEmpty(_itemSketchOne.Item.Sdesc) == true)
                {
                    result.IsSuccess = false;
                    str_msg = str_msg + "Sdesc 錯誤; ";
                }
                else
                {
                    scriptCheck = this.scriptCheck(_itemSketchOne.Item.Sdesc);
                    if (scriptCheck.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg = "Sdesc 錯誤: " + scriptCheck.Msg + "; ";
                    }
                    else
                    {
                        var liCheckResult = this.liTagCHeck(_itemSketchOne.Item.Sdesc);
                        if (liCheckResult.IndexOf("F;") >= 0)
                        {
                            result.IsSuccess = false;
                            str_msg = str_msg + "Sdesc 錯誤： 內容每一點斷行需要以<li></li>做首尾，請檢查修改。; ";
                        }
                    }
                    //var liCheckResult = this.liTagCHeck(_itemSketchOne.Item.Sdesc);
                    //if (liCheckResult.IndexOf("F;") >= 0)
                    //{
                    //    result.IsSuccess = false;
                    //    str_msg = str_msg + "Sdesc 錯誤： 內容每一點斷行需要以<li></li>做首尾，請檢查修改。; ";
                    //}
                }
                #endregion
                #region ShipType
                if (string.IsNullOrEmpty(_itemSketchOne.Item.ShipType) == true)
                {
                    result.IsSuccess = false;
                    str_msg = str_msg + "ShipType 錯誤; ";
                }
                else
                {
                    if (_itemSketchOne.Item.ShipType.ToLower() != "s" && _itemSketchOne.Item.ShipType.ToLower() != "n")
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + "ShipType 錯誤，必須為 S(Seller) or N(Newegg); ";
                    }
                }
                #endregion
                #region Spechead
                if (string.IsNullOrEmpty(_itemSketchOne.Item.Spechead) == true)
                {
                    result.IsSuccess = false;
                    str_msg = str_msg + "Spechead 錯誤; ";
                }
                else
                {
                    scriptCheck = this.scriptCheck(_itemSketchOne.Item.Spechead);
                    if (scriptCheck.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg = "Spechead 錯誤: " + scriptCheck.Msg + "; ";
                    }
                }
                #endregion
                #region BarCode
                if (string.IsNullOrEmpty(_itemSketchOne.Product.BarCode) == false)
                {
                    scriptCheck = this.scriptCheck(_itemSketchOne.Product.BarCode);
                    if (scriptCheck.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg = "BarCode 錯誤: " + scriptCheck.Msg + "; ";
                    }
                }
                #region 必填時
                //if (string.IsNullOrEmpty(_itemSketchOne.Product.BarCode) == true)
                //{
                //    result.IsSuccess = false;
                //    str_msg = str_msg + "BarCode 錯誤; ";
                //}
                //else
                //{
                //    scriptCheck = this.scriptCheck(_itemSketchOne.Product.BarCode);
                //    if (scriptCheck.IsSuccess == false)
                //    {
                //        result.IsSuccess = false;
                //        str_msg = str_msg = "BarCode 錯誤: " + scriptCheck.Msg + "; ";
                //    }
                //}
                #endregion
                #endregion
                #region Cost
                if (decimal.TryParse(_itemSketchOne.Product.Cost.ToString(), out decimalTemp) == false)
                {
                    result.IsSuccess = false;
                    str_msg = str_msg + "Cost 錯誤; ";
                }
                else
                {
                    if (_itemSketchOne.Product.Cost - (int)_itemSketchOne.Product.Cost != 0)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + "Cost 錯誤; ";
                    }
                    else
                    {
                        if (_itemSketchOne.Product.Cost <= 0)
                        {
                            result.IsSuccess = false;
                            str_msg = str_msg + "Cost 必須大於 0; ";
                        }
                    }
                }
                #endregion
                #region Description
                if (string.IsNullOrEmpty(_itemSketchOne.Product.Description) == true)
                {
                    result.IsSuccess = false;
                    str_msg = str_msg + "Description 錯誤; ";
                }
                else
                {
                    scriptCheck = this.scriptCheck(_itemSketchOne.Product.Description);
                    if (scriptCheck.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg = "Description 錯誤: " + scriptCheck.Msg + "; ";
                    }
                }
                #endregion
                #region Height
                if (decimal.TryParse(_itemSketchOne.Product.Height.ToString(), out decimalTemp) == false)
                {
                    result.IsSuccess = false;
                    str_msg = str_msg + "Height 錯誤; ";
                }
                else
                {
                    if (_itemSketchOne.Product.Height <= 0)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + "Height  必須大於 0; ";
                    }
                }
                #endregion
                #region Is18
                if (string.IsNullOrEmpty(_itemSketchOne.Product.Is18) == false)
                {
                    if (_itemSketchOne.Product.Is18.ToLower() != "n" && _itemSketchOne.Product.Is18.ToLower() != "y")
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + "Is18 錯誤，必須為 Y(Yes) or N(No); ";
                    }
                }

                //if (string.IsNullOrEmpty(_itemSketchOne.Product.Is18) == true)
                //{
                //    result.IsSuccess = false;
                //    str_msg = str_msg + "Is18 錯誤; ";
                //}
                //else
                //{
                //    if (_itemSketchOne.Product.Is18.ToLower() != "n" && _itemSketchOne.Product.Is18.ToLower() != "y")
                //    {
                //        result.IsSuccess = false;
                //        str_msg = str_msg + "Is18 錯誤，必須為 Y(Yes) or N(No); ";
                //    }
                //}
                #endregion
                #region IsChokingDanger
                if (string.IsNullOrEmpty(_itemSketchOne.Product.IsChokingDanger) == false)
                {
                    if (_itemSketchOne.Product.IsChokingDanger.ToLower() != "y" && _itemSketchOne.Product.IsChokingDanger.ToLower() != "n")
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + "IsChokingDanger 錯誤，必須為 Y(Yes) or N(No); ";
                    }
                }

                //if (string.IsNullOrEmpty(_itemSketchOne.Product.IsChokingDanger) == true)
                //{
                //    result.IsSuccess = false;
                //    str_msg = str_msg + "IsChokingDanger 錯誤; ";
                //}
                //else
                //{
                //    if (_itemSketchOne.Product.IsChokingDanger.ToLower() != "y" && _itemSketchOne.Product.IsChokingDanger.ToLower() != "n")
                //    {
                //        result.IsSuccess = false;
                //        str_msg = str_msg + "IsChokingDanger 錯誤，必須為 Y(Yes) or N(No); ";
                //    }
                //}
                #endregion
                #region IsShipDanger
                if (string.IsNullOrEmpty(_itemSketchOne.Product.IsShipDanger) == true)
                {
                    result.IsSuccess = false;
                    str_msg = str_msg + "IsShipDanger 錯誤; ";
                }
                else
                {
                    if (_itemSketchOne.Product.IsShipDanger.ToLower() != "y" && _itemSketchOne.Product.IsShipDanger.ToLower() != "n")
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + "IsShipDanger 錯誤，必須為 Y(Yes) or N(No); ";
                    }
                }
                #endregion
                #region Length
                if (decimal.TryParse(_itemSketchOne.Product.Length.ToString(), out decimalTemp) == false)
                {
                    result.IsSuccess = false;
                    str_msg = str_msg + "Length 錯誤; ";

                }
                else
                {
                    if (_itemSketchOne.Product.Length <= 0)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + "Length 必須大於 0; ";
                    }
                }
                #endregion
                #region ManufactureID
                if (int.TryParse(_itemSketchOne.Product.ManufactureID.ToString(), out intCheckTemp) == false)
                {
                    result.IsSuccess = false;
                    str_msg = str_msg + "ManufactureID 錯誤; ";
                }
                else
                {
                    var manufactureInfoExist = spdb.Seller_ManufactureInfo.Where(p => p.SN == _itemSketchOne.Product.ManufactureID).FirstOrDefault();
                    if (manufactureInfoExist == null)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + "ManufactureID 不存在; ";
                    }
                    //if (_itemSketchOne.Product.ManufactureID <= 0)
                    //{
                    //    result.IsSuccess = false;
                    //    str_msg = str_msg + "ManufactureID 錯誤; ";
                    //}
                }
                #endregion
                #region Model
                if (string.IsNullOrEmpty(_itemSketchOne.Product.Model) == false)
                {
                    scriptCheck = this.scriptCheck(_itemSketchOne.Product.Model);
                    if (scriptCheck.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg = "Model 錯誤: " + scriptCheck.Msg + "; ";
                    }
                }
                //if (string.IsNullOrEmpty(_itemSketchOne.Product.Model) == true)
                //{
                //    result.IsSuccess = false;
                //    str_msg = str_msg + "Model 錯誤; ";
                //}
                //else
                //{
                //    scriptCheck = this.scriptCheck(_itemSketchOne.Product.Model);
                //    if (scriptCheck.IsSuccess == false)
                //    {
                //        result.IsSuccess = false;
                //        str_msg = str_msg = "Model 錯誤: " + scriptCheck.Msg + "; ";
                //    }
                //}
                #endregion
                #region Name
                if (string.IsNullOrEmpty(_itemSketchOne.Product.Name) == true)
                {
                    result.IsSuccess = false;
                    str_msg = str_msg + "Name 錯誤; ";
                }
                else
                {
                    scriptCheck = this.scriptCheck(_itemSketchOne.Product.Name);
                    if (scriptCheck.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg = "Name 錯誤: " + scriptCheck.Msg + "; ";
                    }
                }
                #endregion
                #region PicPatch_Edit
                if (_itemSketchOne.Product.PicPatch_Edit.Count > 7)
                {
                    result.IsSuccess = false;
                    str_msg = str_msg + "PicPatch_Edit 錯誤，最多 7 張圖";
                }
                //if (item.Product.PicPatch_Edit == null || item.Product.PicPatch_Edit.Count == 0)
                //{
                //    result.IsSuccess = false;
                //    result.Msg = "第 " + tempNumber + " 筆資料 PicPatch_Edit 錯誤";
                //    break;
                //}
                #endregion
                #region SellerProductID
                if (string.IsNullOrEmpty(_itemSketchOne.Product.SellerProductID) == false)
                {
                    //格式必須符合只能有字元跟數字
                    bool isSellerProductIdFormatSuccess = System.Text.RegularExpressions.Regex.IsMatch(_itemSketchOne.Product.SellerProductID, @"^[0-9a-zA-Z]*$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    if (isSellerProductIdFormatSuccess == false)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + "SellerProductID 錯誤; ";
                    }
                }
                //if (string.IsNullOrEmpty(_itemSketchOne.Product.SellerProductID) == true)
                //{
                //    result.IsSuccess = false;
                //    str_msg = str_msg + "SellerProductID 錯誤; ";
                //}
                //else
                //{
                //    //格式必須符合只能有字元跟數字
                //    bool isSellerProductIdFormatSuccess = System.Text.RegularExpressions.Regex.IsMatch(_itemSketchOne.Product.SellerProductID, @"^[0-9a-zA-Z]*$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //    if (isSellerProductIdFormatSuccess == false)
                //    {
                //        result.IsSuccess = false;
                //        str_msg = str_msg + "SellerProductID 錯誤; ";
                //    }
                //}
                #endregion
                #region UPC
                if (string.IsNullOrEmpty(_itemSketchOne.Product.UPC) == false)
                {
                    if (_itemSketchOne.Product.UPC.Length > 15)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + "UPC 錯誤 長度不能超過 15; ";
                    }
                    else
                    {
                        scriptCheck = this.scriptCheck(_itemSketchOne.Product.UPC);
                        if (scriptCheck.IsSuccess == false)
                        {
                            result.IsSuccess = false;
                            str_msg = str_msg = "UPC 錯誤: " + scriptCheck.Msg + "; ";
                        }
                    }
                }
                //if (string.IsNullOrEmpty(_itemSketchOne.Product.UPC) == true)
                //{
                //    result.IsSuccess = false;
                //    str_msg = str_msg + "UPC 錯誤; ";
                //}
                //else
                //{
                //    if (_itemSketchOne.Product.UPC.Length > 15)
                //    {
                //        result.IsSuccess = false;
                //        str_msg = str_msg + "UPC 錯誤 長度不能超過 15; ";
                //    }
                //    else
                //    {
                //        scriptCheck = this.scriptCheck(_itemSketchOne.Product.UPC);
                //        if (scriptCheck.IsSuccess == false)
                //        {
                //            result.IsSuccess = false;
                //            str_msg = str_msg = "UPC 錯誤: " + scriptCheck.Msg + "; ";
                //        }
                //    }
                //}
                #endregion
                #region Warranty
                if (string.IsNullOrEmpty(_itemSketchOne.Product.Warranty.ToString()) == false)
                {
                    if (int.TryParse(_itemSketchOne.Product.Warranty.ToString(), out intCheckTemp) == false)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + "Warranty 錯誤; ";
                    }
                }
                #endregion
                #region Weight
                if (decimal.TryParse(_itemSketchOne.Product.Weight.ToString(), out decimalTemp) == false)
                {
                    result.IsSuccess = false;
                    str_msg = str_msg + "Weight 錯誤; ";
                }
                else
                {
                    if (_itemSketchOne.Product.Weight <= 0)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + "Weight  必須大於 0; ";
                    }
                }
                #endregion
                #region Width
                if (decimal.TryParse(_itemSketchOne.Product.Width.ToString(), out decimalTemp) == false)
                {
                    result.IsSuccess = false;
                    str_msg = str_msg + "Width 錯誤; ";
                }
                else
                {
                    if (_itemSketchOne.Product.Width <= 0)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + "Width 必須大於 0; ";
                    }
                }
                #endregion
                #region CanSaleQty
                if (int.TryParse(_itemSketchOne.ItemStock.CanSaleQty.ToString(), out intCheckTemp) == false)
                {
                    result.IsSuccess = false;
                    str_msg = str_msg + "CanSaleQty 錯誤; ";
                }
                else
                {
                    if (_itemSketchOne.ItemStock.CanSaleQty <= 0)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + "CanSaleQty 必須大於 0; ";
                    }
                }
                #endregion
                #region InventorySafeQty
                if (int.TryParse(_itemSketchOne.ItemStock.InventorySafeQty.ToString(), out intCheckTemp) == false)
                {
                    result.IsSuccess = false;
                    str_msg = str_msg + "InventorySafeQty 錯誤; ";
                }
                else
                {
                    if (_itemSketchOne.ItemStock.InventorySafeQty <= 0)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + "InventorySafeQty 必須大於 0; ";
                    }
                }
                #endregion
                #region MainCategoryID_Layer2
                if (int.TryParse(_itemSketchOne.ItemCategory.MainCategoryID_Layer2.ToString(), out intCheckTemp) == false)
                {
                    result.IsSuccess = false;
                    str_msg = str_msg + "MainCategoryID_Layer2 錯誤; ";
                }
                else
                {
                    if (_itemSketchOne.ItemCategory.MainCategoryID_Layer2 <= 0)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + "MainCategoryID_Layer2 錯誤; ";
                    }
                }
                #endregion
                #region SubCategoryID_1_Layer2
                if (string.IsNullOrEmpty(_itemSketchOne.ItemCategory.SubCategoryID_1_Layer2.ToString()) == false)
                {
                    if (int.TryParse(_itemSketchOne.ItemCategory.SubCategoryID_1_Layer2.ToString(), out intCheckTemp) == false)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + "SubCategoryID_1_Layer2 錯誤; ";
                    }
                    else
                    {
                        if (_itemSketchOne.ItemCategory.SubCategoryID_1_Layer2 <= 0)
                        {
                            result.IsSuccess = false;
                            str_msg = str_msg + "SubCategoryID_1_Layer2 錯誤; ";
                        }
                    }
                }
                #endregion
                #region SubCategoryID_2_Layer2
                if (string.IsNullOrEmpty(_itemSketchOne.ItemCategory.SubCategoryID_2_Layer2.ToString()) == false)
                {
                    if (int.TryParse(_itemSketchOne.ItemCategory.SubCategoryID_2_Layer2.ToString(), out intCheckTemp) == false)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + "SubCategoryID_2_Layer2 錯誤; ";
                    }
                    else
                    {
                        if (_itemSketchOne.ItemCategory.SubCategoryID_2_Layer2 <= 0)
                        {
                            result.IsSuccess = false;
                            str_msg = str_msg + "SubCategoryID_2_Layer2 錯誤; ";
                        }
                    }
                }
                #endregion
                #region 檢查誇分類
                List<int> subcategoryCheck = new List<int>();
                if (_itemSketchOne.ItemCategory.SubCategoryID_1_Layer2 != 0 && _itemSketchOne.ItemCategory.SubCategoryID_1_Layer2 != null)
                {
                    subcategoryCheck.Add(_itemSketchOne.ItemCategory.SubCategoryID_1_Layer2.GetValueOrDefault());
                }
                if (_itemSketchOne.ItemCategory.SubCategoryID_2_Layer2 != 0 && _itemSketchOne.ItemCategory.SubCategoryID_2_Layer2 != null)
                {
                    subcategoryCheck.Add(_itemSketchOne.ItemCategory.SubCategoryID_2_Layer2.GetValueOrDefault());
                }
                if (subcategoryCheck.Count != 0)
                {
                    var categoryCheck = this.checkCrossCategory(_itemSketchOne.ItemCategory.MainCategoryID_Layer2.GetValueOrDefault(), subcategoryCheck);
                    if (categoryCheck.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + categoryCheck.Msg + "; ";
                    }
                }
                //var categoryCheck = this.checkCrossCategory(_itemSketchOne);
                //if (categoryCheck.IsSuccess == false)
                //{
                //    result.IsSuccess = false;
                //    str_msg = str_msg + categoryCheck.Msg + "; ";
                //}
                #endregion
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                str_msg = str_msg + "檢查資料錯誤; ";
                result.Msg = "檢查資料錯誤";
                logger.Error("MsgError: " + error.Message + "; [StackTrace]: " + error.StackTrace);
            }
            #endregion
            result.Msg = str_msg;
            return result;
        }
        #endregion
        #region 檢查是否在同一個誇分類底下
        public ActionResponse<string> checkCrossCategory(int mainCategoryId, List<int> subCategoryid/*ItemSketch itemSketchModel*/)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            int? MainCategoryID_Layer2, SubCategoryID_1_Layer2, SubCategoryID_2_Layer2;
            int main, sub1, sub2;
            MainCategoryID_Layer2 = mainCategoryId;
            try
            {
                main = CategoryService.LoadCategoryParentId(CategoryService.LoadCategoryParentId(MainCategoryID_Layer2.GetValueOrDefault()));
                if (subCategoryid.Count == 1)
                {
                    SubCategoryID_1_Layer2 = subCategoryid[0];
                    sub1 = CategoryService.LoadCategoryParentId(CategoryService.LoadCategoryParentId(SubCategoryID_1_Layer2.GetValueOrDefault()));
                    if (main != sub1)
                    {
                        result.IsSuccess = false;
                        result.Msg = "跨分類必須在同一個類別底下";
                    }
                    else
                    {
                        result.IsSuccess = true;
                    }
                }
                else if (subCategoryid.Count == 2)
                {
                    SubCategoryID_1_Layer2 = subCategoryid[0];
                    SubCategoryID_2_Layer2 = subCategoryid[1];
                    sub1 = CategoryService.LoadCategoryParentId(CategoryService.LoadCategoryParentId(SubCategoryID_1_Layer2.GetValueOrDefault()));
                    sub2 = CategoryService.LoadCategoryParentId(CategoryService.LoadCategoryParentId(SubCategoryID_2_Layer2.GetValueOrDefault()));
                    if (main == sub1)
                    {
                        if (main == sub2)
                        {
                            result.IsSuccess = true;
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Msg = "跨分類必須在同一個類別底下";
                        }
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Msg = "資料跨分類必須在同一個類別底下";
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = "資料跨分類判斷失敗";
                }
            }
            catch (Exception error)
            {
                logger.Error("Msg: " + error.Message + "; [StackTrace]" + error.StackTrace);
                result.IsSuccess = false;
                result.Msg = "資料跨分類必須在同一個類別底下";
            }
            #region 三個category 都為必填
            //MainCategoryID_Layer2 = itemSketchModel.ItemCategory.MainCategoryID_Layer2;
            //SubCategoryID_1_Layer2 = itemSketchModel.ItemCategory.SubCategoryID_1_Layer2;
            //SubCategoryID_2_Layer2 = itemSketchModel.ItemCategory.SubCategoryID_2_Layer2;
            //try
            //{
            //    main = CategoryService.LoadCategoryParentId(CategoryService.LoadCategoryParentId(MainCategoryID_Layer2.GetValueOrDefault()));
            //    sub1 = CategoryService.LoadCategoryParentId(CategoryService.LoadCategoryParentId(SubCategoryID_1_Layer2.GetValueOrDefault()));
            //    sub2 = CategoryService.LoadCategoryParentId(CategoryService.LoadCategoryParentId(SubCategoryID_2_Layer2.GetValueOrDefault()));
            //    if (main == sub1)
            //    {
            //        if (main == sub2)
            //        {
            //            result.IsSuccess = true;
            //        }
            //        else
            //        {
            //            result.IsSuccess = false;
            //            result.Msg = "跨分類必須在同一個類別底下";
            //        }
            //    }
            //    else
            //    {
            //        result.IsSuccess = false;
            //        result.Msg = "資料跨分類必須在同一個類別底下";
            //    }
            //}
            //catch (Exception error)
            //{
            //    logger.Error("Msg: " + error.Message + "; [StackTrace]" + error.StackTrace);
            //    result.IsSuccess = false;
            //    result.Msg = "資料跨分類必須在同一個類別底下";
            //}
            #endregion
            return result;
        }
        #endregion
        #region 檢查是否有遺漏<li></li>
        public string liTagCHeck(string _strCheck)
        {
            Dictionary<bool, string> result = new Dictionary<bool, string>();
            if (_strCheck.Length <= 500)
            {
                string splitDesctext = _strCheck.Replace(" ", "");
                List<string> checkDesctexts = new List<string>();
                do
                {
                    // 尋找 <li> 的位置
                    int startTag = splitDesctext.IndexOf("<li>");
                    // 尋找 </li> 的位置
                    int endTag = splitDesctext.IndexOf("</li>");
                    // 切割類型
                    string splitType = string.Empty;
                    // 商品簡要描述內容斷點長度
                    int splitLength = 0;

                    // 是否找到第2個 <li>
                    bool isSecondStartTag = false;
                    // 當 <li> 於開頭位置時，判斷 <li> 後面是否還有第2個 <li>
                    if (startTag == 0)
                    {
                        // 先隱藏第一個 <li>
                        string splitFirstStartTag = splitDesctext.Substring(4);
                        // 更新隱藏第一個 <li> 後，下一個 <li> 位置
                        startTag = splitFirstStartTag.IndexOf("<li>");
                        // 更新隱藏 <li> 後的 </li> 位置
                        endTag = splitFirstStartTag.IndexOf("</li>");
                        // 當 <li> 位置小於 </li> 位置時，將是否找到第2個 <li> 設為 true
                        if (startTag < endTag)
                        {
                            isSecondStartTag = true;
                        }
                    }
                    // 判斷是否有找到 <li> 或 </li>
                    if (startTag != -1 || endTag != -1)
                    {
                        // 如果只有找到 </li> 或先找到的是 </li>
                        if ((startTag == -1 && endTag != -1)
                         || (endTag < startTag))
                        {
                            // 使用 </li> 做為切割條件
                            splitType = "EndTag";
                        }
                        else if ((endTag == -1 && startTag != -1)
                              || (startTag < endTag))
                        {
                            // 如果只有找到 <li> 或先找到的是 <li>
                            // 使用 <li> 做為切割條件
                            splitType = "StartTag";
                        }
                    }
                    else
                    {
                        // 都沒找到，則全部內容視為一個斷點
                        splitType = "All";
                    }
                    switch (splitType)
                    {
                        case "StartTag":
                            {
                                // 如果有找到第2個 <li> 
                                if (isSecondStartTag)
                                {
                                    // 先隱藏第一個 <li> ，並在找下一個 <li> 位置後，將商品簡要描述內容斷點長度 + 4 (第一個隱藏的 <li> 字串長度)
                                    splitLength = splitDesctext.Substring(4).IndexOf("<li>") + 4;
                                }
                                else
                                {
                                    // 尋找 <li> 的斷點位置
                                    splitLength = splitDesctext.IndexOf("<li>");
                                }

                                break;
                            }
                        case "EndTag":
                            {
                                // 找到 </li> 位置，並將斷點設在 </li> 之後
                                splitLength = splitDesctext.IndexOf("</li>") + 5;

                                break;
                            }
                        default:
                        case "All":
                            {
                                // 將全部內容視為一個斷點
                                splitLength = splitDesctext.Length;
                                break;
                            }
                    }
                    // 將商品簡要描述內容斷點之前的內容，寫入 List 中
                    checkDesctexts.Add(splitDesctext.Substring(0, splitLength));

                    // 刪除已寫入 List 中的商品簡要描述內容
                    splitDesctext = splitDesctext.Remove(0, splitLength);
                    // 將商品簡要描述內容斷點之前的內容，寫入 List 中
                } while (!string.IsNullOrEmpty(splitDesctext));
                if (string.IsNullOrEmpty(_strCheck) == false)
                {
                    // 判斷商品簡要描述內容是否使用 <li> 及 </li> 包覆
                    bool iscolsebyli = true;
                    // 商品簡要描述內容計數 (商品簡要描述內容最多只允許3項)
                    int descCount = 0;
                    // 判斷商品簡要描述內容是否有空白行
                    bool isEmptyLine = false;
                    // 逐一檢查商品簡要描述內容 List
                    foreach (var text in checkDesctexts)
                    {
                        // 若只有單一的換行，則跳過內容檢查
                        if (text != "\n" && text != "\r" && text != "\r\n" && text != string.Empty)
                        {
                            if (text.IndexOf("<li>") == 0 && text.IndexOf("</li>") != -1)
                            {
                                // 有使用 <li> 及 </li> 包覆，將商品簡要描述內容計數 + 1
                                descCount++;
                            }
                            else if (text.IndexOf("\r\r") != -1 || text.IndexOf("\n\n") != -1)
                            {
                                // 輸入2行以上的換行，則顯示空白行提示
                                isEmptyLine = true;
                            }
                            else
                            {
                                // 未使用 <li> 及 </li> 包覆
                                iscolsebyli = false;

                                // 若只使用 <li> 或只使用 </li>，則將商品簡要描述內容計數 + 1
                                if ((text.IndexOf("<li>") != -1 && text.IndexOf("</li>") == -1)
                                 || (text.IndexOf("<li>") == -1 && text.IndexOf("</li>") != -1))
                                {
                                    descCount++;
                                }
                            }
                        }
                    }
                    // 判斷是否符合商品簡要描述內容
                    // 1.每一點斷行以<li></li>做首尾
                    // 2.最多以三點為上限
                    // 3.不可以有空白行
                    if (iscolsebyli && descCount <= 3 && !isEmptyLine)
                    {
                        //Item.Sdesc = ItemsInfoListDatafeed[40 + (Sequence * 32)];
                    }
                    else
                    {
                        string errorMessage = string.Format("上傳不成功：上傳檔案的Datafeed工作表的簡要描述(主賣點1)內容{0}{1}{2}請檢查修改。",
                            (!iscolsebyli) ? "每一點斷行需要以<li></li>做首尾，" : string.Empty,
                            (descCount > 3) ? "最多以三點為上限，" : string.Empty,
                            (isEmptyLine) ? "不可以有空白行，" : string.Empty);

                        // 回傳的狀態


                        //ResultCookie("【第" + Column + "行，第" + Row + "列】" + errorMessage);

                        return "F;" + errorMessage;
                    }
                }
            }
            else
            {
                return "F;資料錯誤";
            }
            return "T;";
        }
        #endregion
        #region 檢查是否有不合法的字串
        public ActionResponse<bool> scriptCheck(string _strScriptCheck)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            if (_strScriptCheck.IndexOf("<script") >= 0 || _strScriptCheck.IndexOf("</script>") >= 0)
            {
                result.IsSuccess = false;
                result.Msg = "資料錯誤, 此資料包含危險字串.";
                return result;
            }
            else
            {
                result.IsSuccess = true;
                return result;
            }

        }
        #endregion
        #region 取得GetDelvType
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
        #endregion
        #endregion
        #endregion
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
        public ActionResponse<string> ItemCategoryTemp(int itemTempid, List<int> subCategoryId, int userid)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.DB.TWSqlDBContext db_before = new TWSqlDBContext();
            foreach (int itemSubCategoryId in subCategoryId)
            {
                DB.TWSQLDB.Models.ItemCategorytemp _itemcategorytemp = new DB.TWSQLDB.Models.ItemCategorytemp();
                _itemcategorytemp.itemtempID = itemTempid;
                _itemcategorytemp.CategoryID = itemSubCategoryId;
                _itemcategorytemp.FromSystem = "1";//0: PM; 1: sellerPortal
                _itemcategorytemp.CreateUser = userid.ToString();
                _itemcategorytemp.CreateDate = DateTime.Now;
                _itemcategorytemp.UpdateDate = DateTime.Now;
                _itemcategorytemp.UpdateUser = userid.ToString();
                db_before.ItemCategorytemp.Add(_itemcategorytemp);
            }
            try
            {
                db_before.SaveChanges();
                result.IsSuccess = true;
            }
            catch (Exception error)
            {
                logger.Error("MsgError: " + error.Message + "; [StackTrace]: " + error.StackTrace);
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
            }
            return result;
        }
    }

    
}
