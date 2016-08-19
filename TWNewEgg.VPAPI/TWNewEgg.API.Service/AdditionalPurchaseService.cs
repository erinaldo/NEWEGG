using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.API.Models;

namespace TWNewEgg.API.Service
{
    public class AdditionalPurchaseService
    {
        // 記錄訊息
        private static ILog log = LogManager.GetLogger(typeof(TempService));

        #region 查詢加價購

        /// <summary>
        /// 三種搜尋模式及進階查詢加價購商品
        /// </summary>
        /// <param name="AdditionalPurchaseItemSearchResult">itemSearch</param>
        /// <returns>IQueryable</returns>
        public ActionResponse<List<TWNewEgg.API.Models.AdditionalPurchase>> AdditionalPurchaseItemSearchResult(TWNewEgg.API.Models.ItemSketchSearchCondition itemSearch, bool boolDefault)
        {

            ActionResponse<List<TWNewEgg.API.Models.AdditionalPurchase>> result = new ActionResponse<List<AdditionalPurchase>>();
            TWNewEgg.Framework.ServiceApi.ResponsePacket<Dictionary<int, int>> serviceresult = new Framework.ServiceApi.ResponsePacket<Dictionary<int, int>>();
            Models.AdditionalPurchase AdditionalPurchaseModel = new AdditionalPurchase();
            List<Models.AdditionalPurchase> AdditionalPurchaseList = new List<Models.AdditionalPurchase>();
            switch (itemSearch.KeyWordScarchTarget)
            {
                // 新蛋商品編號
                case ItemSketchKeyWordSearchTarget.ItemID:
                    if (itemSearch.KeyWord != string.Empty && itemSearch.KeyWord != null)
                    {
                        int intKeyword = -1;

                        if (Int32.TryParse(itemSearch.KeyWord, out intKeyword))
                        {
                            try
                            {
                                serviceresult = TWNewEgg.Framework.ServiceApi.Processor.Request<Dictionary<int, int>, Dictionary<int, int>>("AdditionItemSearchService", "checkAdditionItem", intKeyword);
                            }
                            catch (Exception e)
                            {
                                result.Code = (int)ResponseCode.Error;
                                result.IsSuccess = false;
                                log.Error("AdditionalPurchaseItemSearchResult查詢時發生例外 : " + e.Message + ", Stacktrace: " + e.StackTrace);
                            }
                            if (string.IsNullOrEmpty(serviceresult.error))
                            {
                                result.Code = (int)ResponseCode.Success;
                                if (serviceresult.results != null)
                                {
                                    AdditionalPurchaseModel.ShowOrder = serviceresult.results[intKeyword];
                                    AdditionalPurchaseModel.ItemID = intKeyword;
                                    AdditionalPurchaseList.Add(AdditionalPurchaseModel);
                                    result.Body = AdditionalPurchaseList;
                                }
                                result.IsSuccess = true;
                            }
                            else
                            {
                                result.Code = (int)ResponseCode.Error;
                                result.IsSuccess = false;
                                log.Error(serviceresult.error);
                            }
                        }
                        else
                        {
                            result.Code = (int)ResponseCode.Error;
                            result.IsSuccess = false;
                            log.Error("傳入加價購查詢keyword有誤 :" + itemSearch.KeyWord);
                        }
                    }

                    break;

                //itemtempid
                case ItemSketchKeyWordSearchTarget.ItemTempID:
                    if (itemSearch.KeyWord != string.Empty && itemSearch.KeyWord != null)
                    {
                        int intKeyword = -1;
                        if (Int32.TryParse(itemSearch.KeyWord, out intKeyword))
                        {
                            try
                            {
                                serviceresult = TWNewEgg.Framework.ServiceApi.Processor.Request<Dictionary<int, int>, Dictionary<int, int>>("AdditionItemSearchService", "checkAdditionTemp", intKeyword);
                            }
                            catch (Exception e)
                            {
                                result.Code = (int)ResponseCode.Error;
                                result.IsSuccess = false;
                                log.Error("AdditionalPurchaseItemSearchResult查詢時發生例外 : " + e.Message + ", Stacktrace: " + e.StackTrace);
                            }
                            if (string.IsNullOrEmpty(serviceresult.error))
                            {
                                result.Code = (int)ResponseCode.Success;
                                if (serviceresult.results != null)
                                {
                                    AdditionalPurchaseModel.ShowOrder = serviceresult.results[intKeyword];
                                    AdditionalPurchaseModel.ItemTempID = intKeyword;
                                    AdditionalPurchaseList.Add(AdditionalPurchaseModel);
                                    result.Body = AdditionalPurchaseList;
                                }
                                result.IsSuccess = true;
                            }
                            else
                            {
                                result.IsSuccess = true;
                                result.Code = (int)ResponseCode.Error;
                                result.IsSuccess = false;
                                log.Error(serviceresult.error);
                            }
                        }
                        else
                        {
                            result.Code = (int)ResponseCode.Error;
                            result.IsSuccess = false;
                            log.Error("傳入加價購查詢keyword有誤 :" + itemSearch.KeyWord);
                        }
                    }
                    break;

                //草稿ID
                case ItemSketchKeyWordSearchTarget.ItemSketchID:
                    if (itemSearch.KeyWord != string.Empty && itemSearch.KeyWord != null)
                    {
                        int intKeyword = -1;
                        if (Int32.TryParse(itemSearch.KeyWord, out intKeyword))
                        {
                            try
                            {
                                serviceresult = TWNewEgg.Framework.ServiceApi.Processor.Request<Dictionary<int, int>, Dictionary<int, int>>("AdditionItemSearchService", "checkAdditionSketch", intKeyword);
                            }
                            catch (Exception e)
                            {
                                result.Code = (int)ResponseCode.Error;
                                result.IsSuccess = false;
                                log.Error("AdditionalPurchaseItemSearchResult查詢時發生例外 : " + e.Message + ", Stacktrace: " + e.StackTrace);
                            }
                            if (string.IsNullOrEmpty(serviceresult.error))
                            {
                                result.Code = (int)ResponseCode.Success;
                                if (serviceresult.results != null)
                                {
                                    AdditionalPurchaseModel.ShowOrder = serviceresult.results[intKeyword];
                                    AdditionalPurchaseModel.ItemSketchID = intKeyword;
                                    AdditionalPurchaseList.Add(AdditionalPurchaseModel);
                                    result.Body = AdditionalPurchaseList;
                                }
                                result.IsSuccess = true;
                            }
                            else
                            {
                                result.Code = (int)ResponseCode.Error;
                                result.IsSuccess = false;
                                log.Error(serviceresult.error);

                            }
                        }
                        else
                        {
                            result.Code = (int)ResponseCode.Error;
                            result.IsSuccess = false;
                            log.Error("傳入加價購查詢keyword有誤 :" + itemSearch.KeyWord);
                        }
                    }
                    break;

            }
            
            return result;
        }

        #endregion

        #region using ECService

        #region 修改加價購

        /// <summary>
        /// 加價購修改
        /// </summary>
        /// <param name="ID">更新商品(ItemTempID、ItemSketchID)</param>
        /// <returns>成功失敗訊息</returns>
        public ActionResponse<string> ECAdditionalPurchaseItemEdit(TWNewEgg.API.Models.AdditionalPurchase AdditionalPurchaseItem)
        {
            ActionResponse<string> result = new ActionResponse<string>();            
            result.IsSuccess = true;
            result.Msg = string.Empty;

            result = this.ECUpdateTempAndOffical(AdditionalPurchaseItem);                                                       

            return result;
        }
        
        #endregion

        #region 更新正式待審草稿

        private ActionResponse<string> ECUpdateTempAndOffical(TWNewEgg.API.Models.AdditionalPurchase AdditionalPurchaseItem)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            result.Body = string.Empty;
            result.IsSuccess = true;
            result.Msg = string.Empty;
            ActionResponse<bool> UpdateItemSketch_DetailEditResult = new ActionResponse<bool>();
            ActionResponse<bool> UpdateItemTemp_DetailEditResult = new ActionResponse<bool>();
            ActionResponse<bool> UpdateItem_DetailEditResult = new ActionResponse<bool>();

            switch(AdditionalPurchaseItem.SearchTarget)
            {
                case AdditionalPurchase.AdditionalPurchaseSearchTarget.ItemSketchID :

                    // 更新草稿
                    UpdateItemSketch_DetailEditResult = this.ECUpdateItemSketch_DetailEdit(AdditionalPurchaseItem);

                    if (UpdateItemSketch_DetailEditResult.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = UpdateItemSketch_DetailEditResult.Msg;
                        log.Info("加價購更新草稿失敗。");
                    }
                    else
                    {
                        log.Info("加價購更新草稿成功。");
                    }
                    break;

                case AdditionalPurchase.AdditionalPurchaseSearchTarget.ItemTempID :

                    // 更新 Temp 表
                    UpdateItemTemp_DetailEditResult = this.ECUpdateItemTemp_DetailEdit(AdditionalPurchaseItem);

                    if (UpdateItemTemp_DetailEditResult.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = UpdateItemTemp_DetailEditResult.Msg;
                        log.Info("加價購更新 Temp 表失敗。");
                    }
                    if (result.IsSuccess)
                    {
                        log.Info("加價購更新 Temp 表成功。");

                        DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
                        DB.TWSQLDB.Models.Item ItemEntry = new DB.TWSQLDB.Models.Item();
                        try
                        {
                            ItemEntry = dbFront.Item.Where(x => x.ItemtempID == AdditionalPurchaseItem.ItemTempID).FirstOrDefault();
                        }
                        catch (Exception e)
                        {
                            result.IsSuccess = false;
                            result.Msg = "加價購查詢正式時發生例外";
                            log.Error("ItemTempID  :" + AdditionalPurchaseItem.ItemTempID + "加價購查詢正式時發生例外 : " + e.Message + ", Stacktrace: " + e.StackTrace);
                            return result;
                        }

                        if (ItemEntry != null)
                        {
                            // 更新正式表
                            AdditionalPurchaseItem.ItemID = ItemEntry.ID;
                            AdditionalPurchaseItem.SearchTarget = API.Models.AdditionalPurchase.AdditionalPurchaseSearchTarget.ItemID;
                            UpdateItem_DetailEditResult = ECUpdateItem_DetailEdit(AdditionalPurchaseItem);
                            if (UpdateItem_DetailEditResult.IsSuccess == false)
                            {
                                result.IsSuccess = false;
                                result.Msg = UpdateItem_DetailEditResult.Msg;
                                log.Info("加價購更新正式表失敗。");
                            }
                            else
                            {
                                log.Info("加價購更新正式表成功。");
                            }
                        }
                    }
                    break;
                
                default :
                    break;
           
            }
            return result;
        }

        #endregion

        #region 更新草稿

        /// <summary>
        /// 加價購更新草稿
        /// </summary>
        /// <param name="AdditionalPurchaseItem">更新資訊</param>      
        /// <returns>成功失敗訊息</returns>
        private ActionResponse<bool> ECUpdateItemSketch_DetailEdit(TWNewEgg.API.Models.AdditionalPurchase AdditionalPurchaseItem)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;
            TWNewEgg.Framework.ServiceApi.ResponsePacket<bool> serviceresult = new Framework.ServiceApi.ResponsePacket<bool>();
            try
            {
                //加價購
                if (AdditionalPurchaseItem.ShowOrder == (int)DB.TWSQLDB.Models.Item.ShowOrderStatus.AddtionalItemForCart)
                {
                    serviceresult = TWNewEgg.Framework.ServiceApi.Processor.Request<bool, bool>("SetAdditionItemService", "EnableAdditionItemforItemSketch", AdditionalPurchaseItem.ItemSketchID, AdditionalPurchaseItem.CreateAndUpdate.UpdateUser.ToString());
                }
                //非加價購
                else if (AdditionalPurchaseItem.ShowOrder == (int)DB.TWSQLDB.Models.Item.ShowOrderStatus.正常)
                {
                    serviceresult = TWNewEgg.Framework.ServiceApi.Processor.Request<bool, bool>("SetAdditionItemService", "DisableAdditionItemSketch", AdditionalPurchaseItem.ItemSketchID, AdditionalPurchaseItem.CreateAndUpdate.UpdateUser.ToString());
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = "加價購草稿編輯失敗";
                    log.Error("ItemSketchID  :" + AdditionalPurchaseItem.ItemSketchID + "加價購草稿編輯失敗 : 傳入 showorder 為 " + AdditionalPurchaseItem.ShowOrder);
                    return result;
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Msg = "加價購草稿編輯發生例外";
                log.Error("ItemSketchID  :" + AdditionalPurchaseItem.ItemSketchID + "加價購草稿編輯發生例外 : " + e.Message + ", Stacktrace: " + e.StackTrace);
                return result;
            }
            if (serviceresult.results == true)
            {
                result.IsSuccess = true;
            }
            else
            {
                log.Error("ItemSketchID  :" + AdditionalPurchaseItem.ItemSketchID + "ECservice 發生錯誤 : " + serviceresult.error);
                result.IsSuccess = false;
                result.Msg = "加價購草稿編輯失敗";
            }
            return result;
        }

        #endregion

        #region 更新待審

        /// <summary>
        /// 加價購更新itemtemp
        /// </summary>
        /// <param name="AdditionalPurchaseItem">更新資訊</param>      
        /// <returns>成功失敗訊息</returns>
        private ActionResponse<bool> ECUpdateItemTemp_DetailEdit(TWNewEgg.API.Models.AdditionalPurchase AdditionalPurchaseItem)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;
            TWNewEgg.Framework.ServiceApi.ResponsePacket<bool> serviceresult = new Framework.ServiceApi.ResponsePacket<bool>();
            try
            {
                //加價購
                if (AdditionalPurchaseItem.ShowOrder == (int)DB.TWSQLDB.Models.Item.ShowOrderStatus.AddtionalItemForCart)
                {
                    serviceresult = TWNewEgg.Framework.ServiceApi.Processor.Request<bool, bool>("SetAdditionItemService", "EnableAdditionItemforItemTemp", AdditionalPurchaseItem.ItemTempID, AdditionalPurchaseItem.CreateAndUpdate.UpdateUser.ToString());
                }
                //非加價購
                else if (AdditionalPurchaseItem.ShowOrder == (int)DB.TWSQLDB.Models.Item.ShowOrderStatus.正常)
                {
                    serviceresult = TWNewEgg.Framework.ServiceApi.Processor.Request<bool, bool>("SetAdditionItemService", "DisableAdditionItemTemp", AdditionalPurchaseItem.ItemTempID, AdditionalPurchaseItem.CreateAndUpdate.UpdateUser.ToString());
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = "加價購待審編輯失敗";
                    log.Error("ItemTempID  :" + AdditionalPurchaseItem.ItemTempID + "加價購待審編輯失敗 : 傳入 showorder 為 " + AdditionalPurchaseItem.ShowOrder);
                    return result;
                }
    
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Msg = "賣場待審編號 :" + AdditionalPurchaseItem.ItemTempID + "加價購修改待審時發生例外";
                log.Error("itemtempid :" + AdditionalPurchaseItem.ItemTempID + "加價購修改待審時發生例外 : " + e.Message + ", Stacktrace: " + e.StackTrace);
                return result;
            }
            if (serviceresult.results == true)
            {
                result.IsSuccess = true;
            }
            else
            {
                log.Error("ItemSketchID  :" + AdditionalPurchaseItem.ItemSketchID + "ECservice 發生錯誤 : " + serviceresult.error);
                result.IsSuccess = false;
                result.Msg = "加價購待審編輯失敗";
            } 
            return result;
        }

        #endregion

        #region 更新正式

        /// <summary>
        /// 加價購更新item
        /// </summary>
        /// <param name="AdditionalPurchaseItem">更新資訊</param>      
        /// <returns>成功失敗訊息</returns>
        private ActionResponse<bool> ECUpdateItem_DetailEdit(TWNewEgg.API.Models.AdditionalPurchase AdditionalPurchaseItem)
        {         
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;
            TWNewEgg.Framework.ServiceApi.ResponsePacket<bool> serviceresult = new Framework.ServiceApi.ResponsePacket<bool>();
            
            try
            {
                //加價購
                if (AdditionalPurchaseItem.ShowOrder == (int)DB.TWSQLDB.Models.Item.ShowOrderStatus.AddtionalItemForCart)
                {
                    serviceresult = TWNewEgg.Framework.ServiceApi.Processor.Request<bool, bool>("SetAdditionItemService", "EnableAdditionItemforItem", AdditionalPurchaseItem.ItemID, AdditionalPurchaseItem.CreateAndUpdate.UpdateUser.ToString());
                }
                //非加價購
                else if (AdditionalPurchaseItem.ShowOrder == (int)DB.TWSQLDB.Models.Item.ShowOrderStatus.正常)
                {
                    serviceresult = TWNewEgg.Framework.ServiceApi.Processor.Request<bool, bool>("SetAdditionItemService", "DisableAdditionItem", AdditionalPurchaseItem.ItemID, AdditionalPurchaseItem.CreateAndUpdate.UpdateUser.ToString());
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = "加價購正式編輯失敗";
                    log.Error("ItemID  :" + AdditionalPurchaseItem.ItemID + "加價購正式編輯失敗 : 傳入 showorder 為 " + AdditionalPurchaseItem.ShowOrder);
                    return result;
                }

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Msg = "賣場正式編號 :" + AdditionalPurchaseItem.ItemID + "加價購修改正式時發生例外";
                log.Error("ItemID :" + AdditionalPurchaseItem.ItemID + "加價購修改正式時發生例外 : " + e.Message + ", Stacktrace: " + e.StackTrace);
                return result;
            }
            if (serviceresult.results == true)
            {
                result.IsSuccess = true;
            }
            else
            {
                log.Error("ItemSketchID  :" + AdditionalPurchaseItem.ItemSketchID + "ECservice 發生錯誤 : " + serviceresult.error);
                result.IsSuccess = false;
                result.Msg = "加價購正式編輯失敗";
            }
            return result;
        }

        #endregion

        #endregion using ECService

        #region using DBsavechange

        #region 修改加價購

        /// <summary>
        /// 加價購修改
        /// </summary>
        /// <param name="ID">更新商品(ItemTempID、ItemSketchID)</param>
        /// <returns>成功失敗訊息</returns>
        public ActionResponse<string> AdditionalPurchaseItemEdit(TWNewEgg.API.Models.AdditionalPurchase AdditionalPurchaseItem)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            result = this.UpdateTempAndOffical(AdditionalPurchaseItem);

            return result;
        }

        #endregion

        #region 更新正式待審草稿

        private ActionResponse<string> UpdateTempAndOffical(TWNewEgg.API.Models.AdditionalPurchase AdditionalPurchaseItem)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            result.Body = string.Empty;
            result.IsSuccess = true;
            result.Msg = string.Empty;
            ActionResponse<bool> UpdateItemSketch_DetailEditResult = new ActionResponse<bool>();
            ActionResponse<bool> UpdateItemTemp_DetailEditResult = new ActionResponse<bool>();
            ActionResponse<bool> UpdateItem_DetailEditResult = new ActionResponse<bool>();

            switch (AdditionalPurchaseItem.SearchTarget)
            {
                case AdditionalPurchase.AdditionalPurchaseSearchTarget.ItemSketchID:

                    // 更新草稿
                    UpdateItemSketch_DetailEditResult = this.UpdateItemSketch_DetailEdit(AdditionalPurchaseItem);

                    if (UpdateItemSketch_DetailEditResult.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = UpdateItemSketch_DetailEditResult.Msg;
                        log.Info("加價購更新草稿失敗。");
                    }
                    else
                    {
                        log.Info("加價購更新草稿成功。");
                    }
                    break;

                case AdditionalPurchase.AdditionalPurchaseSearchTarget.ItemTempID:

                    // 更新 Temp 表
                    UpdateItemTemp_DetailEditResult = this.UpdateItemTemp_DetailEdit(AdditionalPurchaseItem);

                    if (UpdateItemTemp_DetailEditResult.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = UpdateItemTemp_DetailEditResult.Msg;
                        log.Info("加價購更新 Temp 表失敗。");
                    }
                    if (result.IsSuccess)
                    {
                        log.Info("加價購更新 Temp 表成功。");

                        DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
                        DB.TWSQLDB.Models.Item ItemEntry = new DB.TWSQLDB.Models.Item();
                        try
                        {
                            ItemEntry = dbFront.Item.Where(x => x.ItemtempID == AdditionalPurchaseItem.ItemTempID).FirstOrDefault();
                        }
                        catch (Exception e)
                        {
                            result.IsSuccess = false;
                            result.Msg = "加價購查詢正式時發生例外";
                            log.Error("ItemTempID  :" + AdditionalPurchaseItem.ItemTempID + "加價購查詢正式時發生例外 : " + e.Message + ", Stacktrace: " + e.StackTrace);
                            return result;
                        }

                        if (ItemEntry != null)
                        {
                            // 更新正式表
                            AdditionalPurchaseItem.ItemID = ItemEntry.ID;
                            AdditionalPurchaseItem.SearchTarget = API.Models.AdditionalPurchase.AdditionalPurchaseSearchTarget.ItemID;
                            UpdateItem_DetailEditResult = UpdateItem_DetailEdit(AdditionalPurchaseItem);
                            if (UpdateItem_DetailEditResult.IsSuccess == false)
                            {
                                result.IsSuccess = false;
                                result.Msg = UpdateItem_DetailEditResult.Msg;
                                log.Info("加價購更新正式表失敗。");
                            }
                            else
                            {
                                log.Info("加價購更新正式表成功。");
                            }
                        }
                    }
                    break;

                default:
                    break;

            }
            return result;
        }

        #endregion

        #region 更新草稿

        /// <summary>
        /// 加價購更新草稿
        /// </summary>
        /// <param name="AdditionalPurchaseItem">更新資訊</param>      
        /// <returns>成功失敗訊息</returns>
        private ActionResponse<bool> UpdateItemSketch_DetailEdit(TWNewEgg.API.Models.AdditionalPurchase AdditionalPurchaseItem)
        {
            DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
            DB.TWSQLDB.Models.ItemSketch ItemSketchEntry = new DB.TWSQLDB.Models.ItemSketch();

            dbFront.Entry(ItemSketchEntry).State = EntityState.Unchanged;

            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            //讀取草稿原本資料
            try
            {
                ItemSketchEntry = dbFront.ItemSketch.Where(x => x.ID == AdditionalPurchaseItem.ItemSketchID).FirstOrDefault();
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Msg = "產品編號 :" + AdditionalPurchaseItem.ItemSketchID + "加價購查詢草稿時發生例外";
                log.Error("產品編號 :" + AdditionalPurchaseItem.ItemSketchID + "加價購查詢草稿時發生例外 : " + e.Message + ", Stacktrace: " + e.StackTrace);
                return result;
            }
            if (ItemSketchEntry != null)
            {
                if (AdditionalPurchaseItem.ShowOrder != 0 || ItemSketchEntry.ShowOrder == (int)DB.TWSQLDB.Models.Item.ShowOrderStatus.AddtionalItemForCart)
                {
                    ItemSketchEntry.ShowOrder = AdditionalPurchaseItem.ShowOrder;
                }
                dbFront.Entry(ItemSketchEntry).Property(x => x.ShowOrder).IsModified = true;

                //加價購
                if (ItemSketchEntry.ShowOrder == (int)DB.TWSQLDB.Models.Item.ShowOrderStatus.AddtionalItemForCart)
                {
                    if (ItemSketchEntry.Name != null)
                    {
                        if (ItemSketchEntry.Name.IndexOf("加購_") != 0)
                        {
                            ItemSketchEntry.Name = "加購_" + ItemSketchEntry.Name;
                            dbFront.Entry(ItemSketchEntry).Property(x => x.Name).IsModified = true;
                        }
                    }
                    else
                    {
                        ItemSketchEntry.Name = "加購_";
                        dbFront.Entry(ItemSketchEntry).Property(x => x.Name).IsModified = true;
                    }
                }
                //非加價購
                else
                {
                    if (ItemSketchEntry.Name != null)
                    {
                        if (ItemSketchEntry.Name.IndexOf("加購_") == 0)
                        {
                            ItemSketchEntry.Name = ItemSketchEntry.Name.Replace("加購_", "");
                            dbFront.Entry(ItemSketchEntry).Property(x => x.Name).IsModified = true;
                        }
                    }
                }
                //更新 更新者 更新時間
                ItemSketchEntry.UpdateUser = AdditionalPurchaseItem.CreateAndUpdate.UpdateUser.ToString();
                ItemSketchEntry.UpdateDate = DateTime.Now;
                dbFront.Entry(ItemSketchEntry).Property(x => x.UpdateUser).IsModified = true;
                dbFront.Entry(ItemSketchEntry).Property(x => x.UpdateDate).IsModified = true;

                try
                {
                    dbFront.SaveChanges();
                }
                catch (Exception e)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號 :" + AdditionalPurchaseItem.ItemSketchID + "加價購修改草稿時發生例外";
                    log.Error("產品編號 :" + AdditionalPurchaseItem.ItemSketchID + "加價購修改草稿時發生例外 : " + e.Message + ", Stacktrace: " + e.StackTrace);
                    return result;
                }



            }
            else
            {
                result.Code = (int)ResponseCode.Error;
                result.Msg = "產品編號 : " + AdditionalPurchaseItem.ItemSketchID + "查無此草稿資料";
                result.IsSuccess = false;
                log.Info("AdditionalPurchaseService/UpdateItemSketch_DetailEdit :  itemSketchID : " + AdditionalPurchaseItem.ItemSketchID + " 查無此草稿資料");
            }
            return result;
        }

        #endregion

        #region 更新待審

        /// <summary>
        /// 加價購更新itemtemp
        /// </summary>
        /// <param name="AdditionalPurchaseItem">更新資訊</param>      
        /// <returns>成功失敗訊息</returns>
        private ActionResponse<bool> UpdateItemTemp_DetailEdit(TWNewEgg.API.Models.AdditionalPurchase AdditionalPurchaseItem)
        {
            DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
            DB.TWSQLDB.Models.ItemTemp ItemTempEntry = new DB.TWSQLDB.Models.ItemTemp();

            dbFront.Entry(ItemTempEntry).State = EntityState.Unchanged;

            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            //讀取待審原本資料
            try
            {
                ItemTempEntry = dbFront.ItemTemp.Where(x => x.ID == AdditionalPurchaseItem.ItemTempID).FirstOrDefault();
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Msg = "加價購查詢待審時發生例外";
                log.Error("itemtempid  :" + AdditionalPurchaseItem.ItemTempID + "加價購查詢待審時發生例外 : " + e.Message + ", Stacktrace: " + e.StackTrace);
                return result;
            }
            if (ItemTempEntry != null)
            {
                if (AdditionalPurchaseItem.ShowOrder != 0 || ItemTempEntry.Showorder == (int)DB.TWSQLDB.Models.Item.ShowOrderStatus.AddtionalItemForCart)
                {
                    ItemTempEntry.Showorder = AdditionalPurchaseItem.ShowOrder ?? default(int);
                }
                dbFront.Entry(ItemTempEntry).Property(x => x.Showorder).IsModified = true;

                //加價購
                if (ItemTempEntry.Showorder == (int)DB.TWSQLDB.Models.Item.ShowOrderStatus.AddtionalItemForCart)
                {
                    if (ItemTempEntry.Name != null)
                    {
                        if (ItemTempEntry.Name.IndexOf("加購_") != 0)
                        {
                            ItemTempEntry.Name = "加購_" + ItemTempEntry.Name;
                            dbFront.Entry(ItemTempEntry).Property(x => x.Name).IsModified = true;
                        }
                    }
                    else
                    {
                        ItemTempEntry.Name = "加購_";
                        dbFront.Entry(ItemTempEntry).Property(x => x.Name).IsModified = true;
                    }
                }
                //非加價購
                else
                {
                    if (ItemTempEntry.Name != null)
                    {
                        if (ItemTempEntry.Name.IndexOf("加購_") == 0)
                        {
                            ItemTempEntry.Name = ItemTempEntry.Name.Replace("加購_", "");
                            dbFront.Entry(ItemTempEntry).Property(x => x.Name).IsModified = true;
                        }
                    }
                }
                //更新 更新者 更新時間
                ItemTempEntry.UpdateUser = AdditionalPurchaseItem.CreateAndUpdate.UpdateUser.ToString();
                ItemTempEntry.UpdateDate = DateTime.Now;
                dbFront.Entry(ItemTempEntry).Property(x => x.UpdateUser).IsModified = true;
                dbFront.Entry(ItemTempEntry).Property(x => x.UpdateDate).IsModified = true;
                try
                {
                    dbFront.SaveChanges();
                }
                catch (Exception e)
                {
                    result.IsSuccess = false;
                    result.Msg = "賣場待審編號 :" + AdditionalPurchaseItem.ItemTempID + "加價購修改待審時發生例外";
                    log.Error("itemtempid :" + AdditionalPurchaseItem.ItemTempID + "加價購修改待審時發生例外 : " + e.Message + ", Stacktrace: " + e.StackTrace);
                    return result;
                }


            }
            else
            {
                result.Code = (int)ResponseCode.Error;
                result.Msg = "賣場待審編號 : " + AdditionalPurchaseItem.ItemTempID + "查無此待審資料";
                result.IsSuccess = false;
                log.Info("AdditionalPurchaseService/UpdateTemp_DetailEdit :  itemtempid : " + AdditionalPurchaseItem.ItemTempID + " 查無此待審資料");
            }
            return result;
        }

        #endregion

        #region 更新正式

        /// <summary>
        /// 加價購更新item
        /// </summary>
        /// <param name="AdditionalPurchaseItem">更新資訊</param>      
        /// <returns>成功失敗訊息</returns>
        private ActionResponse<bool> UpdateItem_DetailEdit(TWNewEgg.API.Models.AdditionalPurchase AdditionalPurchaseItem)
        {
            DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
            DB.TWSQLDB.Models.Item ItemEntry = new DB.TWSQLDB.Models.Item();

            dbFront.Entry(ItemEntry).State = EntityState.Unchanged;

            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            //讀取正式原本資料
            try
            {
                ItemEntry = dbFront.Item.Where(x => x.ID == AdditionalPurchaseItem.ItemID).FirstOrDefault();
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Msg = "加價購查詢正式時發生例外";
                log.Error("itemid  :" + AdditionalPurchaseItem.ItemID + "加價購查詢正式時發生例外 : " + e.Message + ", Stacktrace: " + e.StackTrace);
                return result;
            }
            if (ItemEntry != null)
            {
                if (ItemEntry.ShowOrder != AdditionalPurchaseItem.ShowOrder)
                {
                    if (AdditionalPurchaseItem.ShowOrder != 0 || ItemEntry.ShowOrder == (int)DB.TWSQLDB.Models.Item.ShowOrderStatus.AddtionalItemForCart)
                    {
                        ItemEntry.ShowOrder = AdditionalPurchaseItem.ShowOrder ?? default(int);
                    }
                    dbFront.Entry(ItemEntry).Property(x => x.ShowOrder).IsModified = true;

                    //加價購
                    if (ItemEntry.ShowOrder == (int)DB.TWSQLDB.Models.Item.ShowOrderStatus.AddtionalItemForCart)
                    {
                        if (ItemEntry.Name != null)
                        {
                            if (ItemEntry.Name.IndexOf("加購_") != 0)
                            {
                                ItemEntry.Name = "加購_" + ItemEntry.Name;
                                dbFront.Entry(ItemEntry).Property(x => x.Name).IsModified = true;
                            }
                        }
                        else
                        {
                            ItemEntry.Name = "加購_";
                            dbFront.Entry(ItemEntry).Property(x => x.Name).IsModified = true;
                        }
                    }
                    //非加價購
                    else
                    {
                        if (ItemEntry.Name != null)
                        {
                            if (ItemEntry.Name.IndexOf("加購_") == 0)
                            {
                                ItemEntry.Name = ItemEntry.Name.Replace("加購_", "");
                                dbFront.Entry(ItemEntry).Property(x => x.Name).IsModified = true;
                            }
                        }
                    }
                    //更新 更新者 更新時間
                    ItemEntry.UpdateUser = AdditionalPurchaseItem.CreateAndUpdate.UpdateUser.ToString();
                    ItemEntry.UpdateDate = DateTime.Now;
                    dbFront.Entry(ItemEntry).Property(x => x.UpdateUser).IsModified = true;
                    dbFront.Entry(ItemEntry).Property(x => x.UpdateDate).IsModified = true;
                    try
                    {
                        dbFront.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        result.IsSuccess = false;
                        result.Msg = "賣場正式編號 :" + AdditionalPurchaseItem.ItemID + "加價購修改正式時發生例外";
                        log.Error("itemid :" + AdditionalPurchaseItem.ItemID + "加價購修改正式時發生例外 : " + e.Message + ", Stacktrace: " + e.StackTrace);
                        return result;
                    }
                }

            }
            else
            {
                result.Code = (int)ResponseCode.Error;
                result.Msg = "賣場編號 : " + AdditionalPurchaseItem.ItemID + "查無此正式資料";
                result.IsSuccess = false;
                log.Info("AdditionalPurchaseService/UpdateItem_DetailEdit :  itemid : " + AdditionalPurchaseItem.ItemID + " 查無此正式資料");
            }
            return result;
        }

        #endregion

        #endregion using DBsavechange
    }
             
}