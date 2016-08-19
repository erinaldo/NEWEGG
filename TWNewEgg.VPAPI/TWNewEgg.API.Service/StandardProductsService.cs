using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using TWNewEgg.API.Models;
using TWNewEgg.API.Models.Models;
using TWNewEgg.DB.TWSELLERPORTALDB.Models;
using log4net;
using log4net.Config;
using Remotion;

namespace TWNewEgg.API.Service
{
    public class StandardProductsService
    {
        // 宣告 Log4net 寫入 Log 
        private static ILog log = LogManager.GetLogger(typeof(StandardProductsService));
        TWNewEgg.API.Service.ItemSketchService sketchService = new ItemSketchService();

        #region 規格品查詢

        /// <summary>
        /// 查詢規格商品草稿
        /// </summary>
        /// <param name="condition">草稿查詢條件</param>
        /// <returns>規格商品草稿</returns>
        public ActionResponse<TWNewEgg.API.Models.DomainModel.CreateStandardProductSketch> GetTwoDimensionProduct(ItemSketchSearchCondition condition, bool isTempCopy = false)
        {
            ActionResponse<TWNewEgg.API.Models.DomainModel.CreateStandardProductSketch> result = new ActionResponse<Models.DomainModel.CreateStandardProductSketch>();
            result.Body = new Models.DomainModel.CreateStandardProductSketch();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            // 草稿查詢
            ActionResponse<List<ItemSketch>> getSketchData = GetSketchData(condition, isTempCopy);

            if (getSketchData.IsSuccess && getSketchData.Body.Count > 0)
            {
                result.Body.basicItemInfo = getSketchData.Body;

                // 規格商品群組 ID
                int? groupID = result.Body.basicItemInfo[0].GroupID;
                if (groupID != null && groupID.Value > 0)
                {
                    // 二維屬性查詢
                    ActionResponse<List<DB.TWSQLDB.Models.ItemSketchProperty>> searchStandardProduct = SearchStandardProduct(groupID.Value);

                    if (searchStandardProduct.IsSuccess && searchStandardProduct.Body.Count > 0)
                    {
                        result.Body.twodimProperty.ItemProperty = searchStandardProduct.Body;
                    }
                    else
                    {
                        if (searchStandardProduct.IsSuccess && searchStandardProduct.Body.Count == 0)
                        {
                            log.Info("查無二維屬性資料.");
                        }

                        if (searchStandardProduct.IsSuccess == false)
                        {
                            result.IsSuccess = false;
                        }
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    log.Info(string.Format("規格商品群組 ID 錯誤，無法查詢二維屬性資料; GroupID = {0}.", groupID));
                }
            }
            else
            {
                result.IsSuccess = false;
            }

            if (result.IsSuccess == false)
            {
                result.Body = new Models.DomainModel.CreateStandardProductSketch();
                log.Info("取得規格商品草稿失敗");
            }

            return result;
        }

        /// <summary>
        /// 草稿查詢
        /// </summary>
        /// <param name="condition">查詢條件</param>
        /// <returns>草稿清單</returns>
        public ActionResponse<List<ItemSketch>> GetSketchData(ItemSketchSearchCondition condition, bool isTempCopy = false)
        {
            ActionResponse<List<ItemSketch>> result = new ActionResponse<List<ItemSketch>>();
            result.Body = new List<ItemSketch>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            API.Service.ItemSketchService itemSketchService = new ItemSketchService();

            try
            {
                result = itemSketchService.GetItemSketchList(condition, isTempCopy);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                log.Info(string.Format("查詢草稿清單失敗(exception); ItemSketchID = {0}; ExceptionMessage = {1}; StackTrace = {2}.", condition.KeyWord, GetExceptionMessage(ex), ex.StackTrace));
            }

            #region 若草稿搜尋結果為多筆，則僅回傳 1 筆

            if (result.IsSuccess && result.Body.Count > 1)
            {
                // 草稿 ID
                int itemSketchID = -1;

                // 轉換草稿 ID 為 int
                try
                {
                    int.TryParse(condition.KeyWord, out itemSketchID);
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Body = new List<ItemSketch>();
                    log.Info(string.Format("轉換草稿 ID 失敗(exception); ItemSketchID = {0}; ExceptionMessage = {1}; StackTrace = {2}.", condition.KeyWord, GetExceptionMessage(ex), ex.StackTrace));
                }

                if (itemSketchID > 0)
                {
                    result.Body = result.Body.Where(x => x.ID == itemSketchID && x.GroupID != null).ToList();
                }
                else
                {
                    result.IsSuccess = false;
                    result.Body = new List<ItemSketch>();
                    log.Info("讀取單筆稿失敗");
                }
            }

            #endregion 若草稿搜尋結果為多筆，則僅回傳 1 筆

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 利用 GropuID 將供應商 二維屬性搜尋出來
        /// </summary>
        /// <param name="GroupID"></param>
        /// <returns></returns>
        public ActionResponse<List<DB.TWSQLDB.Models.ItemSketchProperty>> SearchStandardProduct(int? GroupID)
        {
            TWNewEgg.DB.TWSqlDBContext db = new DB.TWSqlDBContext();

            ActionResponse<List<DB.TWSQLDB.Models.ItemSketchProperty>> result = new ActionResponse<List<DB.TWSQLDB.Models.ItemSketchProperty>>();
            List<DB.TWSQLDB.Models.ItemSketchProperty> searchresult = new List<DB.TWSQLDB.Models.ItemSketchProperty>();

            try
            {
                if (GroupID.HasValue)
                {
                    searchresult = db.ItemSketchProperty.Where(x => x.GroupID == GroupID.Value).ToList();

                    if (searchresult != null)
                    {
                        result.Finish(true, (int)ResponseCode.Success, "搜尋成功", searchresult);
                    }
                    else
                    {
                        result.Finish(true, (int)ResponseCode.Success, "查無資料", null);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Exception: " + ex.Message + ", StackTrace: " + ex.StackTrace);
                result.Finish(false, (int)ResponseCode.Error, "發生意外錯誤，請稍後再試", null);
            }

            return result;
        }

        #endregion

        #region 規格品建立

        #region 規格品商品草稿建立

        /// <summary>
        /// 
        /// </summary>
        /// <param name="twodimItemSketch"></param>
        /// <returns></returns>
        public ActionResponse<string> twoDimPropertyCreate(TWNewEgg.API.Models.DomainModel.CreateStandardProductSketch twodimItemSketch)
        {
            ActionResponse<string> result = new ActionResponse<string>();

            if (twodimItemSketch != null && twodimItemSketch.basicItemInfo != null && twodimItemSketch.twodimProperty != null)
            {
                ActionResponse<string> getAccountType = this.sketchService.GetAccountType(twodimItemSketch.basicItemInfo[0].Item.SellerID.Value);

                try
                {
                    var basicItemResult = this.sketchService.SaveItemSketch(ItemSketchEditType.Create, twodimItemSketch.basicItemInfo[0], getAccountType.Body);

                    if (basicItemResult.IsSuccess == true && basicItemResult.Body != 0)
                    {
                        var itemsketchtwoDimProperty = this.insertItemSketchID(twodimItemSketch.twodimProperty, basicItemResult.Body);

                        var propertyresult = this.CreateStandardProducts(itemsketchtwoDimProperty);

                        if (propertyresult.IsSuccess == true)
                        {
                            result.Finish(true, (int)ResponseCode.Success, "規格商品建立成功", basicItemResult.Body.ToString());
                        }
                        else
                        {
                            log.Error("規格商品屬性建立失敗");
                            result.Finish(false, (int)ResponseCode.AccessError, "規格商品屬性建立失敗", null);
                        }
                    }
                    else
                    {
                        result.Finish(false, (int)ResponseCode.AccessError, "規格商品草稿建立失敗", null);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Exception: " + ex.Message + ", StackTrace: " + ex.StackTrace);
                    result.Finish(false, (int)ResponseCode.Error, "發生意外錯誤，規格商品建立失敗，請稍後再試", null);
                }
            }

            return result;
        }

        /// <summary>
        /// 塞入草稿 ID 至草稿二維屬性 Table
        /// </summary>
        /// <param name="standProductProperty">草稿二維屬性</param>
        /// <param name="itemSketchID"></param>
        /// <returns></returns>
        private Models.DomainModel.StandProductProperty insertItemSketchID(Models.DomainModel.StandProductProperty standProductProperty, int itemSketchID)
        {
            foreach (var itemsketchGroup in standProductProperty.ItemGroup)
            {
                itemsketchGroup.ItemSketchID = itemSketchID;
            }

            foreach (var itemsketchproperty in standProductProperty.ItemProperty)
            {
                itemsketchproperty.ItemSketchID = itemSketchID;
            }

            return standProductProperty;
        }

        #endregion

        #region 二維商品建立

        /// <summary>
        /// 
        /// </summary>
        /// <param name="twoDimProperty"></param>
        /// <returns></returns>
        public ActionResponse<string> CreateStandardProducts(TWNewEgg.API.Models.DomainModel.StandProductProperty twoDimProperty)
        {
            ActionResponse<string> result = new ActionResponse<string>();

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    var groupResult = this.createPropertyGroup(twoDimProperty.ItemGroup, null);
                    if (groupResult.IsSuccess == true)
                    {
                        var twoDimPropertyResult = this.createProperty(twoDimProperty.ItemProperty, groupResult.Body);

                        if (twoDimPropertyResult.IsSuccess == true)
                        {
                            bool updateResult = this.UpdateItemSketchGroupID(twoDimProperty.ItemGroup[0].ItemSketchID, groupResult.Body);

                            if (updateResult)
                            {
                                scope.Complete();
                                result.Finish(true, (int)ResponseCode.Success, "二維商品屬性建立成功", groupResult.Body);
                            }
                            else
                            {
                                scope.Dispose();
                                result.Finish(false, (int)ResponseCode.AccessError, "草稿GroupID 更新失敗", null);
                            }
                        }
                        else
                        {
                            result = twoDimPropertyResult;
                            scope.Dispose();
                        }
                    }
                    else
                    {
                        result = groupResult;
                        scope.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Exception: " + ex.Message + ", StackTrace: " + ex.StackTrace);
                    result.Finish(false, (int)ResponseCode.Error, "二維商品屬性建立發生例外錯誤，請稍後在試", null);
                }
            }

            return result;
        }

        private bool UpdateItemSketchGroupID(int itemSketchID, string groupID)
        {
            int int_groupID = 0;

            TWNewEgg.DB.TWSqlDBContext db = new DB.TWSqlDBContext();

            var updatemodel = db.ItemSketch.Where(x => x.ID == itemSketchID).FirstOrDefault();

            if (updatemodel != null)
            {
                if (int.TryParse(groupID, out int_groupID))
                {
                    updatemodel.GroupID = int_groupID;

                    db.Entry(updatemodel).State = System.Data.EntityState.Modified;

                    try
                    {
                        db.SaveChanges();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        log.Error("Exception: " + ex.Message + " ,StackTrace: " + ex.StackTrace);
                        return false;
                    }
                }
                else
                {
                    log.Error("GroupID 無法解析, " + groupID);
                    return false;
                }
            }
            else
            {
                log.Error("查無ItemSketch, " + itemSketchID);
                return false;
            }
        }

        private ActionResponse<string> createProperty(List<DB.TWSQLDB.Models.ItemSketchProperty> sketchProperty, string groupID)
        {
            TWNewEgg.DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();

            ActionResponse<string> createResult = new ActionResponse<string>();

            int int_groupID = 0;

            string result = string.Empty;

            if (int.TryParse(groupID, out int_groupID))
            {
                if (sketchProperty.Count > 0)
                {
                    foreach (var index in sketchProperty)
                    {
                        index.GroupID = int_groupID;
                        index.CreateDate = DateTime.Now;
                        index.UpdateDate = index.CreateDate;
                        index.UpdateUser = index.InUser;
                        try
                        {
                            dbFront.ItemSketchProperty.Add(index);

                            dbFront.SaveChanges();

                            createResult.Finish(true, (int)ResponseCode.Success, "ItemSketchProperty Create Success", index.ID.ToString());
                        }
                        catch (DbEntityValidationException ex)
                        {
                            var entityError = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
                            var getFullMessage = string.Join("; ", entityError);
                            result = string.Concat("errors are: ", getFullMessage);
                            log.Error(result);

                            createResult.Finish(false, (int)ResponseCode.AccessError, "ItemSketchProperty entity, Exception: " + result, null);
                        }
                        catch (Exception ex)
                        {
                            log.Error("Exception: " + GetExceptionMessage(ex) + ", StackTrace: " + ex.StackTrace);
                            result = ex.Message;
                            createResult.Finish(false, (int)ResponseCode.AccessError, "ItemSketchProperty, Exception: " + result, null);
                        }
                    }
                }
                else
                {
                    log.Info("無輸入 ItemSketchProperty 資料");
                    createResult.Finish(true, (int)ResponseCode.Success, "無輸入 ItemSketchProperty 資料", null);
                }
            }
            else
            {
                result = "GroupID 解析失敗，ItemSketchGroup建立失敗";
                log.Error(result);
                createResult.Finish(false, (int)ResponseCode.Error, result, null);
            }

            return createResult;
        }

        private ActionResponse<string> createPropertyGroup(List<DB.TWSQLDB.Models.ItemSketchGroup> itemSketchGroup, int? itemSketchGroupID)
        {
            TWNewEgg.DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();

            ActionResponse<string> createResult = new ActionResponse<string>();

            string result = string.Empty;

            int groupID = -1;
            if (itemSketchGroupID.HasValue && itemSketchGroupID > 0)
            {
                groupID = itemSketchGroupID.Value;
            }
            else
            {
                groupID = dbFront.ItemSketchGroup.Max(x => (int?)x.ID) ?? 0;
                groupID++;
            }

            if (itemSketchGroup != null && itemSketchGroup.Count > 0)
            {
                if (itemSketchGroup.Count <= 2)
                {
                    foreach (var index in itemSketchGroup)
                    {
                        index.ID = groupID;
                        index.Order = itemSketchGroup.IndexOf(index) + 1;
                        index.CreateDate = DateTime.Now;
                        index.UpdateDate = index.CreateDate;
                        index.UpdateUser = index.InUser;

                        dbFront.ItemSketchGroup.Add(index);

                        try
                        {
                            dbFront.SaveChanges();

                            createResult.Finish(true, (int)ResponseCode.Success, "ItemGroup Create Success", index.ID.ToString());
                        }
                        catch (DbEntityValidationException ex)
                        {
                            var entityError = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
                            var getFullMessage = string.Join("; ", entityError);
                            result = string.Concat("errors are: ", getFullMessage);
                            log.Error(result);

                            createResult.Finish(false, (int)ResponseCode.AccessError, "Group Entity, Exception: " + result, null);
                        }
                        catch (Exception ex)
                        {
                            log.Error("Exception: " + ex.Message + ", StackTrace: " + ex.StackTrace);
                            result = ex.Message;
                            createResult.Finish(false, (int)ResponseCode.AccessError, "Group 建立失敗, Exception: " + result, null);
                        }
                    }
                }
                else
                {
                    createResult.Finish(false, (int)ResponseCode.Error, "超過兩個屬性建立", null);
                }
            }
            else
            {
                createResult.Finish(false, (int)ResponseCode.Error, "無傳入資料建立", null);
            }


            return createResult;
        }

        #endregion

        #endregion 規格品建立

        #region 規格品編輯

        public ActionResponse<string> UpdateTwoDimensionProductDetailEdit(TWNewEgg.API.Models.DomainModel.CreateStandardProductSketch twodimItemSketch)
        {
            log.Info("規格品編輯開始");

            ActionResponse<string> result = new ActionResponse<string>();
            result.IsSuccess = true;
            result.Body = string.Empty;
            result.Msg = string.Empty;

            if (twodimItemSketch.basicItemInfo.Count > 0 && (twodimItemSketch.twodimProperty.ItemGroup.Count > 0 && twodimItemSketch.twodimProperty.ItemGroup.Count <= 2))
            {
                // 使用 TransactionScope 保持資料交易完整性
                using (TransactionScope scope = new TransactionScope())
                {
                    TWNewEgg.API.Service.ItemSketchService itemSketchService = new ItemSketchService();

                    try
                    {
                        // 更新草稿
                        ActionResponse<List<string>> editItemSketch = itemSketchService.EditItemSketch(ItemSketchEditType.DetailEdit, twodimItemSketch.basicItemInfo);

                        if (editItemSketch.IsSuccess)
                        {
                            // 更新群組
                            ActionResponse<int> updateItemSketchGroup = UpdateItemSketchGroup(twodimItemSketch.basicItemInfo[0].ID, twodimItemSketch.twodimProperty.ItemGroup);

                            if (updateItemSketchGroup.IsSuccess)
                            {
                                // 更新二維屬性
                                ActionResponse<string> updateItemSketchProperty = UpdateItemSketchProperty(twodimItemSketch.basicItemInfo[0].ID, updateItemSketchGroup.Body, twodimItemSketch.twodimProperty.ItemProperty);

                                if (updateItemSketchProperty.IsSuccess == false)
                                {
                                    result.IsSuccess = false;
                                    log.Info("規格品編輯-二維屬性儲存失敗。");
                                }
                            }
                            else
                            {
                                result.IsSuccess = false;
                                log.Info("規格品編輯-群組儲存失敗。");
                            }
                        }
                        else
                        {
                            result.IsSuccess = false;
                            log.Info("規格品編輯-草稿儲存失敗。");
                        }
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        log.Info(string.Format("規格品編輯失敗(exception); ExceptionMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
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
            else
            {
                #region 記錄錯誤訊息

                string errorMessage = string.Empty;

                if (!(twodimItemSketch.basicItemInfo.Count > 0))
                {
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        errorMessage += "，";
                    }

                    errorMessage += "未傳入草稿(ItemSketch)內容";
                }

                if (twodimItemSketch.twodimProperty.ItemGroup.Count > 2)
                {
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        errorMessage += "，";
                    }

                    if (twodimItemSketch.twodimProperty.ItemGroup.Count == 0)
                    {
                        errorMessage += "未傳入群組(ItemSketchGroup)內容";
                    }
                    else
                    {
                        errorMessage += "傳入的群組(ItemSketchGroup)資料數錯誤 ( {0} 筆)";
                    }
                }

                log.Info(string.Format("規格品編輯失敗，{0}。", errorMessage));

                if (twodimItemSketch.twodimProperty.ItemGroup.Count > 0 && (twodimItemSketch.twodimProperty.ItemGroup.Count > 0 && twodimItemSketch.twodimProperty.ItemGroup.Count > 2))
                {
                    log.Info("群組(ItemSketchGroup)內容");

                    foreach (TWNewEgg.DB.TWSQLDB.Models.ItemSketchGroup itemSketchGroup in twodimItemSketch.twodimProperty.ItemGroup)
                    {
                        log.Info(string.Format("Order = {0}; PropertyID = {1}; PropertyName = {2}.", itemSketchGroup.Order, itemSketchGroup.PropertyID, itemSketchGroup.PropertyName));
                    }
                }

                #endregion 記錄錯誤訊息
            }
            result.Code = SetResponseCode(result.IsSuccess);

            log.Info("規格品編輯結束");

            return result;
        }

        #region 更新群組

        /// <summary>
        /// 更新規格品群組資料
        /// </summary>
        /// <param name="itemSketchID">草稿 ID</param>
        /// <param name="itemGroup">規格品群組資料</param>
        /// <returns>群組ID</returns>
        private ActionResponse<int> UpdateItemSketchGroup(int itemSketchID, List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchGroup> itemGroup)
        {
            ActionResponse<int> result = new ActionResponse<int>();
            result.IsSuccess = true;
            result.Body = -1;
            result.Msg = string.Empty;

            ActionResponse<int> deleteItemSketchGroup = DeleteItemSketchGroup(itemSketchID);

            if (deleteItemSketchGroup.IsSuccess)
            {
                DateTime nowTime = DateTime.Now;

                if (itemGroup.Count <= 2)
                {
                    foreach (TWNewEgg.DB.TWSQLDB.Models.ItemSketchGroup item in itemGroup)
                    {
                        item.ItemSketchID = itemSketchID;
                    }

                    ActionResponse<string> createPropertyGroupResult = createPropertyGroup(itemGroup, deleteItemSketchGroup.Body);
                    result.IsSuccess = createPropertyGroupResult.IsSuccess;
                }
                else
                {
                    result.IsSuccess = false;

                    string errorMessage = string.Empty;
                    log.Info(string.Format("規格品群組資料數錯誤 ( {0} 筆).", itemGroup.Count));
                }

                if (result.IsSuccess)
                {
                    result.Body = deleteItemSketchGroup.Body;
                }
            }

            return result;
        }

        /// <summary>
        /// 刪除規格品群組資料
        /// </summary>
        /// <param name="itemSketchID">草稿 ID</param>
        /// <returns>群組 ID</returns>
        private ActionResponse<int> DeleteItemSketchGroup(int itemSketchID)
        {
            ActionResponse<int> result = new ActionResponse<int>();
            result.Body = -1;
            result.IsSuccess = true;
            result.Msg = string.Empty;

            TWNewEgg.DB.TWSqlDBContext frontDB = new DB.TWSqlDBContext();

            try
            {
                var deletedb = frontDB.ItemSketchGroup.Where(x => x.ItemSketchID == itemSketchID).ToList();

                if (deletedb.Count > 0)
                {
                    result.Body = deletedb[0].ID;

                    foreach (var index in deletedb)
                    {
                        frontDB.Entry(index).State = System.Data.EntityState.Deleted;
                    }

                    frontDB.SaveChanges();
                }
                else
                {
                    log.Info(string.Format("查無群組資訊; ItemKetchID = {0};", itemSketchID));
                }
            }
            catch (Exception ex)
            {
                result.Body = -1;
                result.IsSuccess = false;
                log.Info(string.Format("刪除群組資訊失敗(exception); ItemSketchID = {0}; ExceptionMessage = {1}; StackTrace = {2}.", itemSketchID, GetExceptionMessage(ex), ex.StackTrace));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        #endregion 更新群組

        #region 更新二維屬性

        /// <summary>
        /// 更新二維屬性資料
        /// </summary>
        /// <param name="itemSketchID">草稿 ID</param>
        /// <param name="groupID">群組 ID</param>
        /// <param name="itemProperty">二維屬性資料</param>
        /// <returns>成功、失敗資訊</returns>
        private ActionResponse<string> UpdateItemSketchProperty(int itemSketchID, int groupID, List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchProperty> itemProperty)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            result.IsSuccess = true;
            result.Body = string.Empty;
            result.Msg = string.Empty;

            ActionResponse<string> deleteItemSketchProperty = DeleteItemSketchProperty(itemSketchID);

            if (deleteItemSketchProperty.IsSuccess)
            {
                if (itemProperty.Count > 0)
                {
                    foreach (TWNewEgg.DB.TWSQLDB.Models.ItemSketchProperty itemSketchProperty in itemProperty)
                    {
                        itemSketchProperty.ItemSketchID = itemSketchID;
                    }

                    ActionResponse<string> addProperty = createProperty(itemProperty, groupID.ToString());
                    result.IsSuccess = addProperty.IsSuccess;
                }
                else
                {
                    log.Info("無輸入二維屬性(ItemSketchProperty)資料.");
                }
            }
            else
            {
                result.IsSuccess = false;
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 刪除二維屬性資料
        /// </summary>
        /// <param name="itemSketchID">草稿ID</param>
        /// <returns>成功、失敗訊息</returns>
        public ActionResponse<string> DeleteItemSketchProperty(int itemSketchID)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            result.IsSuccess = true;
            result.Body = string.Empty;
            result.Msg = string.Empty;

            TWNewEgg.DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();

            List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchProperty> deletePropertyInfo = dbFront.ItemSketchProperty.Where(x => x.ItemSketchID == itemSketchID).ToList();

            if (deletePropertyInfo.Count > 0)
            {
                foreach (TWNewEgg.DB.TWSQLDB.Models.ItemSketchProperty deleteIndex in deletePropertyInfo)
                {
                    dbFront.Entry(deleteIndex).State = System.Data.EntityState.Deleted;
                }

                try
                {
                    dbFront.SaveChanges();
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    log.Info(string.Format("刪除二維屬性(ItemSketchProperty)資料失敗(exception); ItemSketchID = {0}; ExceptionMessage = {1}; StackTrace = {2}.", itemSketchID, GetExceptionMessage(ex), ex.StackTrace));
                }
            }
            else
            {
                log.Info("查無二維屬性(ItemSketchProperty)資料.");
            }

            return result;
        }

        #endregion 更新二維屬性

        #endregion 規格品編輯

        #region 規格品建立送審
        public ActionResponse<string> TwoDimensionProductCreateExamine(TWNewEgg.API.Models.DomainModel.CreateStandardProductSketch twodimItemSketch, int userid, int sellerid, bool isNewItem)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            ActionResponse<string> createResult = new ActionResponse<string>();
            TWNewEgg.API.Service.ItemSketchPropertyService itemSPS = new ItemSketchPropertyService();
            bool isCreateNoException = true;
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    #region 新建商品的送審
                    //新建商品的送審
                    if (isNewItem == true)
                    {                      
                        // 先把資料新建到草稿區
                        createResult = this.twoDimPropertyCreate(twodimItemSketch);
                        //新建到草稿區的資料發生錯誤
                        if (createResult.IsSuccess == false)
                        {
                            result.IsSuccess = createResult.IsSuccess;
                            result.Msg = createResult.Msg;
                            isCreateNoException = false;
                        }
                        else
                        {
                            ActionResponse<string> toSketchExamine = new ActionResponse<string>();
                            //把新建好的草稿 ID 取出來
                            int itemsketch_id = Convert.ToInt32(createResult.Body);

                            List<int> to_Examine_sketch_id = new List<int>();
                            to_Examine_sketch_id.Add(itemsketch_id);
                            //開始送審
                            toSketchExamine = itemSPS.ItemSketchExamine(to_Examine_sketch_id, userid, sellerid);
                            //送審失敗
                            if (toSketchExamine.IsSuccess == false)
                            {
                                result.IsSuccess = false;
                                result.Msg = toSketchExamine.Msg;
                                isCreateNoException = false;
                            }
                            else
                            {
                                //送審成功
                                result.IsSuccess = true;
                                result.Msg = toSketchExamine.Msg;
                                isCreateNoException = true;
                            }
                        }
                    }
                    #endregion
                    #region 修改的送審
                    else
                    {
                        //編輯資料的送審
                        // 先把資料儲存到草稿區
                        createResult = this.UpdateTwoDimensionProductDetailEdit(twodimItemSketch);
                        //儲存到草稿區的資料發生錯誤
                        if (createResult.IsSuccess == false)
                        {
                            result.IsSuccess = createResult.IsSuccess;
                            result.Msg = createResult.Msg;
                            isCreateNoException = false;
                        }
                        else
                        {
                            ActionResponse<string> toSketchExamine = new ActionResponse<string>();
                            //因為這邊的編輯送審只會有一筆, 所以 itemsketch id 取第一筆的就可以
                            int itemsketch_id = Convert.ToInt32(twodimItemSketch.basicItemInfo[0].ID);
                            List<int> to_Examine_sketch_id = new List<int>();
                            to_Examine_sketch_id.Add(itemsketch_id);
                            //開始送審
                            toSketchExamine = itemSPS.ItemSketchExamine(to_Examine_sketch_id, userid, sellerid);
                            //送審失敗
                            if (toSketchExamine.IsSuccess == false)
                            {
                                result.IsSuccess = false;
                                result.Msg = toSketchExamine.Msg;
                                isCreateNoException = false;
                            }
                            else
                            {
                                //送審成功
                                result.IsSuccess = true;
                                result.Msg = toSketchExamine.Msg;
                                isCreateNoException = true;
                            }

                        }
                    }

                }
                    #endregion
                catch (Exception error)
                {
                    result.IsSuccess = false;
                    result.Msg = "新增送審資料錯誤";
                    log.Error("[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
                    isCreateNoException = false;
                }
                if (isCreateNoException == false)
                {
                    scope.Dispose();
                    return result;
                }
                else
                {
                    scope.Complete();
                    return result;
                }
            }
        }
        #endregion

        // 規格品草稿送審

        #region 讀取 Exception Message、填寫 Response Code

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

        #endregion 讀取 Exception Message、填寫 Response Code
    }
}
