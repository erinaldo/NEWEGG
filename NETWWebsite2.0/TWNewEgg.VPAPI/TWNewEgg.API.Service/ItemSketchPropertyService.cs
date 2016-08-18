using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.API.Models;
using TWNewEgg.DB;
using System.Transactions;
using log4net;
using log4net.Config;

namespace TWNewEgg.API.Service
{
    public class ItemSketchPropertyService
    {
        private static ILog log = LogManager.GetLogger(typeof(ItemSketchPropertyService));
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        TWNewEgg.API.Service.ItemSketchService itemSketchService = new ItemSketchService();

        #region 草稿區
        #region 草稿查詢
        public ActionResponse<List<TWNewEgg.API.Models.ItemSketch>> ItemSketchSearch(ItemSketchSearchCondition condition, bool IsTempCopy = false)
        {
            TWSqlDBContext dbfront = new TWSqlDBContext();
            ActionResponse<List<TWNewEgg.API.Models.ItemSketch>> result = new ActionResponse<List<ItemSketch>>();
            //呼叫原本的 itemsketch search api
            var itemSketchServiceResult = itemSketchService.GetItemSketchList(condition, IsTempCopy);
            if (itemSketchServiceResult.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = itemSketchServiceResult.Msg;
                return result;
            }
            List<ItemSketch> hasPropertyItemSketch = new List<ItemSketch>();
            //List<int> itemSketchDeleteId = new List<int>();
            try
            {
                //只取有屬性的草稿資料
                hasPropertyItemSketch = itemSketchServiceResult.Body.Where(p => dbfront.ItemSketchGroup.Select(q => q.ItemSketchID).Contains(p.ID)).ToList();
                //沒有符合的資料
                if (hasPropertyItemSketch.Count == 0)
                {
                    result.IsSuccess = false;
                    result.Msg = "查無資料";
                }
                else
                {
                    #region 把不會秀在畫面的資訊清除，避免回傳時轉換成 JSON STRING 時超過長度限制, 也避免造成修改轉換成 JSON STRING 時 有危險的符號
                    //把不會秀在畫面的資訊清除，避免回傳時轉換成 JSON STRING 時超過長度限制, 也避免造成修改轉換成 JSON STRING 時 有危險的符號
                    hasPropertyItemSketch.ForEach(p => p.Item.Sdesc = "");
                    hasPropertyItemSketch.ForEach(p => p.Product.Description = "");
                    hasPropertyItemSketch.ForEach(p => p.Item.Spechead = "");
                    hasPropertyItemSketch.ForEach(p => p.ItemStock.CanSaleQty = p.ItemStock.InventoryQty);
                    hasPropertyItemSketch.ForEach(p => p.Item.Note = "");
                    itemSketchServiceResult.Body = hasPropertyItemSketch;
                    #region 把不會秀在畫面的資訊清除 foreach version
                    //把不會秀在畫面的資訊清除，避免回傳時轉換成 JSON STRING 時超過長度限制, 也避免造成修改轉換成 JSON STRING 時 有危險的符號
                    //foreach (var cleanItem in itemSketchServiceResult.Body)
                    //{
                    //    cleanItem.Item.Sdesc = "";
                    //    cleanItem.Product.Description = "";
                    //    cleanItem.Item.Spechead = "";
                    //    cleanItem.ItemStock.CanSaleQty = cleanItem.ItemStock.InventoryQty;
                    //    cleanItem.Item.Note = "";
                    //}
                    #endregion
                    #endregion
                    result.IsSuccess = true;
                    result.Body = itemSketchServiceResult.Body;
                    result.Msg = "查詢成功";
                }
            }
            catch (Exception error)
            {
                result.Msg = "查詢失敗";
                result.IsSuccess = false;
                logger.Error("[Msg]: " + error.Message + " ;StackTrace: " + error.StackTrace + " ;[ExceptionInnerMessage]: " + this.ExceptionInnerMessage(error));
            }
            return result;
        }
        #endregion
        #region 草稿區修改(Batch)
        public ActionResponse<string> ItemSketchEdit(ItemSketchEditType editType, List<ItemSketch> itemSketchCell)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            ActionResponse<List<string>> updateResult = new ActionResponse<List<string>>();
            bool exceptionNoError = true;
            try
            {
                //呼較原有的修改 API
                updateResult = itemSketchService.EditItemSketch(editType, itemSketchCell);
                updateResult.IsSuccess = true;
                exceptionNoError = true;
            }
            catch (Exception error)
            {
                logger.Error("[Msg]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[ExceptionInnerMessage]: " + this.ExceptionInnerMessage(error));
                updateResult.IsSuccess = false;
                exceptionNoError = false;
            }
            //exception error, 則直接回傳
            if (exceptionNoError == false)
            {
                result.IsSuccess = false;
                result.Msg = "資料處理錯誤";
                return result;
            }
            if (updateResult.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = updateResult.Msg;
                return result;
            }
            else
            {
                result.IsSuccess = true;
                result.Msg = updateResult.Msg;
                return result;
            }
        }
        #endregion
        #region 草稿區刪除
        public ActionResponse<string> ItemSketchDelete(int toDeleteId = -1)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.API.Service.ItemSketchService itemSketchService = new ItemSketchService();
            if (toDeleteId == -1)
            {
                result.Msg = "刪除資料處理錯誤";
                result.IsSuccess = false;
                logger.Error("刪除沒有刪除的 itemSketch id");
                return result;
            }
            try
            {
                result = itemSketchService.DeleteItemSketch(toDeleteId);
            }
            catch (Exception error)
            {
                result.Msg = "刪除資料處理錯誤";
                result.IsSuccess = false;
                logger.Error("[Msg]: " + error.Message + "; [StackTrace]: " + error.StackTrace);
            }
            return result;
        }
        #endregion
        #region 草稿區送審
        public ActionResponse<string> ItemSketchExamine(List<int> toExamineId, int userid, int sellerid)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.DB.TWSqlDBContext dbFront = new TWSqlDBContext();
            
            #region 檢查資料是否有送審過
            //先檢查是否有資料已經送審過
            //bool isAlreadyExamineId = false;
            if (toExamineId == null)
            {
                result.IsSuccess = false;
                result.Msg = "沒有送審的資料";
                return result;
            }
            int alreadyExistCount = dbFront.ItemSketch.Where(p => toExamineId.Contains(p.ID) && p.Status == 99).ToList().Count;
            if (alreadyExistCount != 0)
            {
                result.IsSuccess = false;
                result.Msg = "有資料已經送審過，請從新整理。";
                return result;
            }
            //foreach (int? id in toExamineId)
            //{
            //    TWNewEgg.DB.TWSQLDB.Models.ItemSketch existIsExamineItemSketch = new DB.TWSQLDB.Models.ItemSketch();
            //    existIsExamineItemSketch = dbFront.ItemSketch.Where(p => p.ID == id && p.Status == 99).FirstOrDefault();
            //    if (existIsExamineItemSketch != null)
            //    {
            //        result.IsSuccess = false;
            //        result.Msg = "有資料已經送審過，請從新整理。";
            //        isAlreadyExamineId = true;
            //        break;
            //    }
            //}
            //要送審的資料中, 有已經送審過
            //if (isAlreadyExamineId == true)
            //{
            //    return result;
            //}
            #endregion
            #region 可售數量為 0 不送審(暫時拿掉)
            //var sizeProperty =  dbFront.ItemSketchProperty.Where(p => toExamineId.Contains(p.ItemSketchID)).ToList();
            //sizeProperty = sizeProperty.Where(x => x.Qty > 0).ToList();
         
            //if (sizeProperty == null || sizeProperty.Count == 0)
            //{
            //    result.IsSuccess = false;
            //    result.Msg = "送審失敗：請確認每一筆草稿可售數量不可全部為 0";
            //    return result;
            //}
            //else
            //{
            //    result.IsSuccess = true;
            //}
            #endregion
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    //開始送審
                    var examineResult = this.OneOrTwoArrayExamine(toExamineId, userid, sellerid);
                    //送審失敗
                    if (examineResult.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = examineResult.Msg;
                    }
                    else
                    {
                        //送審成功
                        result.IsSuccess = true;
                        result.Msg = examineResult.Msg;
                    }
                }
                catch (Exception error)
                {
                    result.IsSuccess = false;
                    result.Msg = "送審資料處理錯誤，請稍後再試";
                    logger.Error("[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[InnerException.Message]: " + this.ExceptionInnerMessage(error));
                }
                if (result.IsSuccess == false)
                {
                    scope.Dispose();
                }
                else
                {
                    scope.Complete();
                }
            }
            return result;

        }
        public ActionResponse<string> OneOrTwoArrayExamine(List<int> To_Sketch_id, int userid, int sellerid)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.DB.TWSqlDBContext dbFront = new TWSqlDBContext();
            result.IsSuccess = true;
            result.Msg = string.Empty;
            #region 檢查傳進來的 To_Sketch_id List 是否為空
            if (To_Sketch_id == null || To_Sketch_id.Count == 0)
            {
                result.IsSuccess = false;
                result.Msg = "無送審資料";
                return result;
            }
            #endregion
            //利用 itemsketchid 抓取 order = 1, 2 的屬性資料
            List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchGroup> ItemSketchGroupOrderCountCheck = dbFront.ItemSketchGroup.Where(p => To_Sketch_id.Contains(p.ItemSketchID)).ToList();
            #region 檢查前台的資料是否有存在資料庫裏面
            if (ItemSketchGroupOrderCountCheck.Count == 0 || ItemSketchGroupOrderCountCheck == null)
            {
                result.IsSuccess = false;
                result.Msg = "無送審資料";
                return result;
            }
            #endregion
            //一維屬性送審用的 List
            List<int> oneArray = new List<int>();
            //二維屬性送審用的 List
            List<int> twoArray = new List<int>();
            #region 判斷商品是一維的還是二維的或者更多
            //用來判斷是否有不是一維跟二維的商品
            bool isNoOverTwoArray = true;
            foreach (int itemsketch in To_Sketch_id)
            {
                //判斷是一維屬性還是二維屬性
                int orderCount = ItemSketchGroupOrderCountCheck.Where(p => p.ItemSketchID == itemsketch).ToList().Count;
                //一維屬性
                if (orderCount == 1)
                {   
                    //新增到一維屬性的 List
                    oneArray.Add(itemsketch);
                    isNoOverTwoArray = true;
                }
                else if (orderCount == 2)//二維屬性
                {
                    //新增到二維屬性的 List
                    twoArray.Add(itemsketch);
                    isNoOverTwoArray = true;
                }
                else
                {
                    //不是一維屬性也不是二維屬性, 則發生資料錯誤, 停止繼續動作回傳錯誤
                    isNoOverTwoArray = false;
                }
                //有非一維屬性和二維屬性的送審資料
                if (isNoOverTwoArray == false)
                {
                    result.IsSuccess = false;
                    result.Msg = "資料發生錯誤, 商品屬性最多為二維屬性";
                    break;
                }
            }
            #endregion
            //送審商品檢查發生錯誤
            if (result.IsSuccess == false)
            {
                return result;
            }
            else
            {
                //一維屬性送審結果
                ActionResponse<string> oneArrayExamineResult = new ActionResponse<string>();
                oneArrayExamineResult.IsSuccess = true;
                //二維屬性送審結果
                ActionResponse<string> TwoArrayExamineResult = new ActionResponse<string>();
                TwoArrayExamineResult.IsSuccess = true;
                //一維屬性送審, 有資料再送
                if (oneArray.Count != 0)
                {
                    //開始一維送審
                    TWNewEgg.API.Service.OneDimensionPropertyProcessService OneDimensionPropertyProcessService = new Service.OneDimensionPropertyProcessService();
                    //oneArrayExamineResult = this.oneArrayExamine_Temp(oneArray, userid, sellerid);
                    oneArrayExamineResult = OneDimensionPropertyProcessService.OneDimensionExamine(oneArray, userid, sellerid);
                    //一維送審有誤, 不繼續做二維屬性商品, 直接 return
                    if (oneArrayExamineResult.IsSuccess == false)
                    {
                        result.IsSuccess = oneArrayExamineResult.IsSuccess;
                        result.Msg = oneArrayExamineResult.Msg;
                        return result;
                    }
                    else
                    {
                        //有二維商品需要送審再送審
                        if (twoArray.Count != 0)
                        {
                            //二維屬性送審
                            TwoArrayExamineResult = this.propertyExamineListSketch(twoArray, userid, sellerid);
                            //二維屬性送審失敗
                            if (TwoArrayExamineResult.IsSuccess == false)
                            {
                                result.IsSuccess = false;
                                result.Msg = TwoArrayExamineResult.Msg;
                                return result;
                            }
                            else
                            {
                                //二維屬性送審成功
                                result.IsSuccess = true;
                                result.Msg = TwoArrayExamineResult.Msg;
                                return result;
                            }
                        }
                        else
                        {
                            //沒有二維商品要送審, 所有結果只要抓取一維送審結果
                            result.IsSuccess = oneArrayExamineResult.IsSuccess;
                            result.Msg = oneArrayExamineResult.Msg;
                            return result;
                        }
                    }
                }
                else//沒有一維屬性的商品要送審, 所以直接進入二維屬性送審
                {
                    //沒有二維屬性要送審
                    if (twoArray.Count != 0)
                    {
                        //二維屬性送審
                        TwoArrayExamineResult = this.propertyExamineListSketch(twoArray, userid, sellerid);
                        //二維屬性送審失敗
                        if (TwoArrayExamineResult.IsSuccess == false)
                        {
                            result.IsSuccess = false;
                            result.Msg = TwoArrayExamineResult.Msg;
                            return result;
                        }
                        else
                        {
                            //二維屬性送審成功
                            result.IsSuccess = true;
                            result.Msg = TwoArrayExamineResult.Msg;
                            return result;
                        }
                    }
                    else
                    {
                        //沒有一維屬性也沒有二維屬性商品要送審
                        result.IsSuccess = false;
                        result.Msg = "無送審資料";
                        return result; 
                    }
                }
            }
        }
        public ActionResponse<string> propertyExamineListSketch(List<int> To_Sketch_id, int userid, int sellerid)
        {
            TWNewEgg.DB.TWSqlDBContext DB_Front = new TWSqlDBContext();
            ActionResponse<string> result = new ActionResponse<string>();
            //先從 DB 裡面拿取對應 itemSketch ID 的 itemSketch 資料
            List<TWNewEgg.DB.TWSQLDB.Models.ItemSketch> _listItemSketch = DB_Front.ItemSketch.Where(p => To_Sketch_id.Contains(p.ID)).ToList();
            #region 呼叫之前草稿區送審時的資料檢查, 檢查要送審的資料是否正確
            TWNewEgg.API.Service.ItemSketchService itemSketchService = new ItemSketchService();
            var checkResult = checkModelElement(_listItemSketch);
            if (checkResult.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = checkResult.Msg;
                return result;
            }
            #endregion
            List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchProperty> _listItemSketchProperty = DB_Front.ItemSketchProperty.Where(p => To_Sketch_id.Contains(p.ItemSketchID)).ToList();
            List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchGroup> _listItemSketchGroup = DB_Front.ItemSketchGroup.Where(p => To_Sketch_id.Contains(p.ItemSketchID)).ToList();
            List<TWNewEgg.DB.TWSQLDB.Models.ItemCategorySketch> _listItemCategorySketch = DB_Front.ItemCategorySketch.Where(p => To_Sketch_id.Contains(p.ItemSketchID)).ToList();
            foreach (var itemSketchId in To_Sketch_id)
            {
                TWNewEgg.API.Models.BatchExamineModel batchExamineModel = new BatchExamineModel();
                //要送審的 basic itemsketch model
                TWNewEgg.DB.TWSQLDB.Models.ItemSketch itemsketch = _listItemSketch.Where(p => p.ID == itemSketchId).FirstOrDefault();
                //從 [ItemSketchGroup] 取得 order = 1: 第一順位(顏色)
                var ItemSketchGroupOrder1 = _listItemSketchGroup.Where(p => p.ItemSketchID == itemSketchId && p.Order == 1).FirstOrDefault();
                //從 [ItemSketchProperty] 取得第一順位的相關資料
                var _listItemSketchPropertyOrder1Color = _listItemSketchProperty.Where(p => p.PropertyID == ItemSketchGroupOrder1.PropertyID && p.ItemSketchID == itemSketchId).ToList();
                //組合屬性的迴圈
                foreach (var colorItem in _listItemSketchPropertyOrder1Color)
                {
                    TWNewEgg.API.Models.colorSizeModel _colorSizeModel = new Models.colorSizeModel();
                    //第一順位自定義的 inputValue
                    _colorSizeModel.inputValue = colorItem.InputValue;
                    _colorSizeModel.colorValueId = colorItem.GroupValueID;
                    //利用 GroupValueID 取回第一順位的屬性相關資料
                    var GroupValueID = colorItem.GroupValueID;
                    //var ValueID = colorItem.ValueID;
                    //利用 GroupValueID 取得在 ItemSketchProperty 對應的第二順位資料(尺寸)
                    var sizeProperty = _listItemSketchProperty.Where(p => p.ItemSketchID == itemSketchId && p.GroupValueID == GroupValueID).ToList();
                   
                    List<TWNewEgg.API.Models.propertyModel> listpropertyModel = new List<propertyModel>();
                    //把第二屬性的屬性放回對應的 Model
                    foreach (var sizeitem in sizeProperty)
                    {
                        //如果兩值一樣，就是顏色，不 add 到 尺寸的 model
                        if (sizeitem.GroupValueID == sizeitem.ValueID)
                        {
                            logger.Error("itemSketchId = " + itemSketchId + ", \"如果兩值一樣，就是顏色，不 add 到 尺寸的 model\" 錯誤 ");
                        }
                        else
                        {
                            logger.Info("二維屬性送審屬性 Model 組合: itemSketchId = " + itemSketchId + ", ValueID = " + sizeitem.ValueID + ", Qty = " + sizeitem.Qty);
                            TWNewEgg.API.Models.propertyModel propertyModel = new propertyModel();
                            propertyModel.proValueId = sizeitem.ValueID;
                            propertyModel.proQty = sizeitem.Qty;
                            listpropertyModel.Add(propertyModel);
                        }
                    }
                    _colorSizeModel.listProperty = listpropertyModel;
                    batchExamineModel.colorsizeProperty.Add(_colorSizeModel);
                }
                #region join 要送審的資料
                ActionResponse<List<TWNewEgg.API.Models.propertyJoinModel>> joinResult = new ActionResponse<List<propertyJoinModel>>();
                //TWNewEgg.API.Service.ExamineBatchService examineBatchService = new ExamineBatchService();
                try
                {
                    //join 要送審的資料，值由其他 TABLE JOIN 拉回來
                    joinResult = this.joinPropertymodel(batchExamineModel);
                    //join 失敗或是資料不符合送審的條件
                    if (joinResult.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = joinResult.Msg;
                    }
                    else
                    {
                        result.IsSuccess = true;
                    }
                }
                catch (Exception error)
                {
                    result.IsSuccess = false;
                    result.Msg = joinResult.Msg;
                    logger.Error("[Msg]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
                }
                //join 失敗直接 break 回傳失敗
                if (result.IsSuccess == false)
                {
                    break;
                }
                #endregion join 要送審的資料
                #region 送審開始
                List<int> subCategoryid = new List<int>();
                subCategoryid = _listItemCategorySketch.Where(p => p.ItemSketchID == itemSketchId).Select(p => p.CategoryID).ToList();
                if (subCategoryid.Count == 0 || subCategoryid == null)
                {
                }
                else
                {
                    if (subCategoryid.Count == 1)
                    {
                        batchExamineModel.ItemCategory.SubCategoryID_1_Layer2 = subCategoryid[0];
                    }
                    else
                    {
                        batchExamineModel.ItemCategory.SubCategoryID_1_Layer2 = subCategoryid[0];
                        batchExamineModel.ItemCategory.SubCategoryID_2_Layer2 = subCategoryid[1];
                    }
                }
                var ExaminePropertyResult = this.ItemGroup_itemGroupProperty(itemsketch, batchExamineModel, userid, sellerid, joinResult);
                //送審失敗，則 break 後續不做
                if (ExaminePropertyResult.IsSuccess == false)
                {
                    result.IsSuccess = false;
                    result.Msg = ExaminePropertyResult.Msg;
                    break;
                }
                else
                {
                    //送審成功
                    result.IsSuccess = true;
                    result.Msg = ExaminePropertyResult.Msg;
                }
                #endregion 送審結束
            }

            return result;
        }
        public ActionResponse<string> ItemGroup_itemGroupProperty(TWNewEgg.DB.TWSQLDB.Models.ItemSketch itemsketch, TWNewEgg.API.Models.BatchExamineModel batchExamineModel, int userid, int sellerid, ActionResponse<List<TWNewEgg.API.Models.propertyJoinModel>> joinResult)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.API.Service.ExamineBatchService examineBatchService = new ExamineBatchService();
            //ActionResponse<List<TWNewEgg.API.Models.propertyJoinModel>> joinResult = new ActionResponse<List<propertyJoinModel>>();
            TWNewEgg.DB.TWSqlDBContext dbFront = new TWSqlDBContext();
            try
            {
                int amount = batchExamineModel.colorsizeProperty.Count;
                //把資料寫入 ItemGroup 表
                var itemgroup = this.ItemGroup(amount, batchExamineModel, userid, sellerid);
                //Insert 失敗
                if (itemgroup.IsSuccess == false)
                {
                    result.IsSuccess = false;
                    result.Msg = itemgroup.Msg;
                }
                else
                {
                    int groupid = itemgroup.Body;
                    int propertyId = batchExamineModel.colorsizeProperty[0].colorValueId;
                    var itemGroupProperty = this.ItemGroupProperty(batchExamineModel, joinResult.Body, groupid, userid, propertyId);
                    if (itemGroupProperty.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = itemGroupProperty.Msg;
                    }
                    else
                    {
                        var nextExamine = sketchPropertyExamine(batchExamineModel, itemsketch, sellerid, userid, joinResult.Body, groupid);
                        //送審 insert table success
                        if (nextExamine.IsSuccess == false)
                        {
                            result.IsSuccess = false;
                            result.Msg = nextExamine.Msg;
                        }
                        else
                        {
                            //送審成功則把 Status = 99，再次搜索時就不會搜索到
                            //var changeStatus = dbFront.ItemSketch.Where(p => p.ID == itemsketch.ID).FirstOrDefault();
                            //changeStatus.Status = 99;
                            //changeStatus.ItemTempGroupID = groupid;
                            ////回填 groupid 回 itemsketch
                            ////var itemSketchGroupid = dbFront.ItemSketch.Where(p => p.ID == itemsketch.ID).FirstOrDefault();
                            ////itemSketchGroupid.ItemTempGroupID = groupid;
                            ////itemsketch.Status = 99;
                            //dbFront.SaveChanges();
                            result.IsSuccess = true;
                            result.Msg = "送審成功";
                        }

                    }
                }
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "送審資料處理錯誤，請稍後再試";
                logger.Error("[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }
            return result;
        }
        
        public ActionResponse<string> sketchPropertyExamine(TWNewEgg.API.Models.BatchExamineModel pModel, TWNewEgg.DB.TWSQLDB.Models.ItemSketch itemSketchModel, int sellerid, int userid, List<TWNewEgg.API.Models.propertyJoinModel> jModel, int groupId)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.DB.TWSqlDBContext dbFront = new TWSqlDBContext();
            result.IsSuccess = true;
            result.Body = string.Empty;

            int itemTempid = 0;
            int productTempid = 0;
            string inputValue = string.Empty;
            int colorValueId = 0;
            int mapTempint = 1;
            #region 用商品的各個第一順位(顏色)開始送審
            //用商品的各個尺寸開始送審
            foreach (var itemModel in pModel.colorsizeProperty)
            {
                inputValue = string.Empty;
                inputValue = itemModel.inputValue;
                colorValueId = itemModel.colorValueId;
                #region 先建立顏色的 itemGroupPropertyDetail 後面再建立尺寸的 itemGroupPropertyDetail (暫時不用)
                //利用顏色的 colorValueId 從 Join 回還得 Model 取的對應的資料
                //var jsubColorModel = jModel.Where(p => p.ItemPropertyValue_ID == itemModel.colorValueId).FirstOrDefault();
                //var colorProperty = jModel.Where(p => p.ItemPropertyValue_ID == itemModel.colorValueId).Select(p => p.ItemPropertyValue_ID).FirstOrDefault();
                #endregion 先建立顏色的 itemGroupPropertyDetail 後面再建立尺寸的 itemGroupPropertyDetail
                //itemModel.listProperty 包含尺寸跟數量
                #region 對第二順位的資料開始把資料 insert 進去 table
                foreach (var itemProperty in itemModel.listProperty)
                {
                    #region 開始 insert 送審時需要 insert 的 table
                    #region productTemp
                    var productTemp = this.productTempInser(/*pModel,*/ itemSketchModel, sellerid, userid, itemModel, itemProperty, jModel);
                    productTempid = productTemp.Body;
                    if (productTemp.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = productTemp.Msg;
                        break;
                    }
                    //判斷是 productTempid 有回傳回來
                    if (productTempid == 0 || productTempid == null)
                    {
                        result.IsSuccess = false;
                        result.Msg = "資料處理錯誤";
                        logger.Error("屬性送審失敗, SellerId: " + sellerid + ": productTempid 沒有回寫, productTempid = " + productTempid);
                        break;
                    }
                    #endregion
                    #region itemTemp
                    var itemtemp = this.itemtempInsert(itemSketchModel, productTempid, userid);
                    if (itemtemp.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = itemtemp.Msg;
                        break;
                    }
                    itemTempid = itemtemp.Body;
                    //判斷是否 itemTempid 有沒有回寫
                    if (itemTempid == 0 || itemTempid == null)
                    {
                        result.IsSuccess = false;
                        result.Msg = "資料處理錯誤";
                        logger.Error("屬性商品送審失敗, sellerid: " + sellerid + ", itemTempid 沒有回寫, itemTempid = " + itemTempid);
                        break;
                    }
                    #endregion
                    #region Itemstocktemp
                    var Itemstocktemp = this.ItemstocktempInsert(itemProperty.proQty, productTempid, userid, itemSketchModel.InventorySafeQty);
                    if (Itemstocktemp.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = Itemstocktemp.Msg;
                        break;
                    }
                    #endregion
                    #region ItemCategoryTemp
                    List<int> categoryid = new List<int>();
                    //判斷是否有傳入誇分類 1
                    if (pModel.ItemCategory.SubCategoryID_1_Layer2 != 0 && pModel.ItemCategory.SubCategoryID_1_Layer2 != null)
                    {
                        categoryid.Add(pModel.ItemCategory.SubCategoryID_1_Layer2.GetValueOrDefault());
                    }
                    //判斷是否有傳入誇分類 2
                    if (pModel.ItemCategory.SubCategoryID_2_Layer2 != 0 && pModel.ItemCategory.SubCategoryID_2_Layer2 != null)
                    {
                        categoryid.Add(pModel.ItemCategory.SubCategoryID_2_Layer2.GetValueOrDefault());
                    }
                    //有選填誇分類，subCategoryId.Count 不是 1 就是 2，是 0 的話就沒必要建立誇分類
                    if (categoryid.Count != 0)
                    {
                        var itemCategoryTemp = this.ItemCategoryTemp(itemTempid, categoryid, userid);
                        if (itemCategoryTemp.IsSuccess == false)
                        {
                            result.IsSuccess = false;
                            result.Msg = itemCategoryTemp.Msg;
                            break;
                        }
                    }
                    #endregion
                    #region productPropertytemp
                    //itemProperty: 第二順位屬性
                    var productPropertytemp = this.productPropertytempInsert(itemProperty, jModel, sellerid, userid, productTempid, inputValue, colorValueId);
                    if (productPropertytemp.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = productPropertytemp.Msg;
                        break;
                    }
                    #endregion
                    #region ItemGroupDetailProperty
                    var itemGroupPropertyDetailSize = this.ItemGroupDetailPropertyInsert(groupId, itemTempid, sellerid, userid, jModel, itemModel, itemProperty);
                    if (itemGroupPropertyDetailSize.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = itemGroupPropertyDetailSize.Msg;
                        break;
                    }
                    #endregion
                    #endregion 結束 insert 送審時需要 insert 的 table
                    #region 圖片處理
                    var Imgresult = this.imgProcess(itemSketchModel.PicStart, itemSketchModel.PicEnd, itemSketchModel.ID, itemTempid, mapTempint);
                    if (Imgresult.IsSuccess == false)
                    {
                        logger.Error("itemTempid: " + itemTempid + "; productTempid: " + productTempid + "; itemSketchModel_id: " + itemSketchModel.ID + "; 圖片處理失敗");
                    }
                    #endregion
                }
                #endregion
                mapTempint++;
                //判斷 insert 資料進入 table 的時候是否有錯誤，有錯誤則 break 停止迴圈
                if (result.IsSuccess == false)
                {
                    break;
                }
                else
                {
                    result.Msg = "送審成功";
                }
            }//foreach end
            #endregion
            #region 送審成功之後把 Status, itemtemp, productTemp 回寫回 itemsketch
            TWNewEgg.DB.TWSQLDB.Models.ItemSketch itemsketchUpdate = dbFront.ItemSketch.Where(p => p.ID == itemSketchModel.ID).FirstOrDefault();
            itemsketchUpdate.Status = 99;
            itemsketchUpdate.ItemTempGroupID = groupId;
            itemsketchUpdate.ProducttempID = productTempid;
            itemsketchUpdate.itemtempID = itemTempid;
            try
            {
                dbFront.SaveChanges();
                result.IsSuccess = true;
                result.Msg = "送審成功";
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                logger.Error("[Message]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[ExceptionInnerMessage]: " + this.ExceptionInnerMessage(error));
            }
            #endregion
            return result;
        }
        #region 把資料 insert 到 ItemGroup
        public ActionResponse<int> ItemGroup(int amount, TWNewEgg.API.Models.BatchExamineModel batchExamineModel, int userid, int sellerid)
        {
            TWNewEgg.DB.TWSqlDBContext dbFront = new TWSqlDBContext();
            ActionResponse<int> result = new ActionResponse<int>();
            int masterTemp = batchExamineModel.colorsizeProperty[0].colorValueId;
            int master = dbFront.ItemPropertyValue.Where(p => p.ID == masterTemp).Select(p => p.PropertyNameID).FirstOrDefault();
            try
            {
                TWNewEgg.DB.TWSQLDB.Models.ItemGroup insertItemGroup = new DB.TWSQLDB.Models.ItemGroup();
                insertItemGroup.Amount = amount;
                insertItemGroup.MasterPropertyID = master;
                insertItemGroup.SellerID = sellerid;
                insertItemGroup.CreateDate = DateTime.Now;
                insertItemGroup.InUser = userid;
                insertItemGroup.UpdateDate = DateTime.Now;
                insertItemGroup.UpdateUser = userid;
                dbFront.ItemGroup.Add(insertItemGroup);
                dbFront.SaveChanges();
                result.IsSuccess = true;
                result.Body = insertItemGroup.ID;//groupId
            }
            catch (Exception error)
            {
                logger.Error("[Msg] " + error.Message + "; [StackTrace]: " + error.StackTrace);
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
            }
            return result;
        }
        #endregion
        #region 把資料 insert 到 productTemp
        public ActionResponse<int> productTempInser(/*TWNewEgg.API.Models.BatchExamineModel BModel,*/ TWNewEgg.DB.TWSQLDB.Models.ItemSketch IModel, int sellerid, int userid, colorSizeModel itemModel, propertyModel itemProperty, List<TWNewEgg.API.Models.propertyJoinModel> jModel)
        {
            ActionResponse<int> result = new ActionResponse<int>();
            TWSqlDBContext db_before = new TWSqlDBContext();
            DateTime dateTimeMillisecond = new DateTime(0001, 01, 01, 01, 01, 01, 01);
            TWNewEgg.API.Service.ImageService imgService = new ImageService();
            string timeString = dateTimeMillisecond.ToString("yyyy-MM-dd hh:mm:ss fff");
            dateTimeMillisecond = DateTime.Now;
            DB.TWSQLDB.Models.ProductTemp productTempAdd = new DB.TWSQLDB.Models.ProductTemp();
            productTempAdd.SellerProductID = IModel.SellerProductID;
            #region 填寫商品名稱

            #region 取得主項目屬性名稱

            string firstPropertyValueName = string.Empty;

            propertyJoinModel firstPropertyJoinModel = jModel.Where(p => p.ItemPropertyValue_ID == itemModel.colorValueId).FirstOrDefault();

            if (string.IsNullOrEmpty(firstPropertyJoinModel.ItemPropertyValue_propertyValueTW))
            {
                firstPropertyValueName = firstPropertyJoinModel.ItemPropertyValue_propertyValue;
            }
            else
            {
                firstPropertyValueName = firstPropertyJoinModel.ItemPropertyValue_propertyValueTW;
            }

            #endregion 取得主項目屬性名稱

            #region 取得次項目屬性名稱

            string secondPropertyValueName = string.Empty;

            propertyJoinModel secondPropertyJoinModel = jModel.Where(p => p.ItemPropertyValue_ID == itemProperty.proValueId).FirstOrDefault();

            if (string.IsNullOrEmpty(secondPropertyJoinModel.ItemPropertyValue_propertyValueTW))
            {
                secondPropertyValueName = secondPropertyJoinModel.ItemPropertyValue_propertyValue;
            }
            else
            {
                secondPropertyValueName = secondPropertyJoinModel.ItemPropertyValue_propertyValueTW;
            }

            #endregion 取得次項目屬性名稱

            string propertyName = string.Empty;

            if (!string.IsNullOrEmpty(firstPropertyValueName) || !string.IsNullOrEmpty(secondPropertyValueName))
            {
                propertyName += firstPropertyValueName;

                if (!string.IsNullOrEmpty(propertyName) && !string.IsNullOrEmpty(secondPropertyValueName))
                {
                    propertyName += " ";
                }

                propertyName += secondPropertyValueName;

                if (!string.IsNullOrEmpty(propertyName))
                {
                    propertyName = "(" + propertyName + ")";
                }
            }

            string tempName = string.Empty;
            if (IModel.ShowOrder == (int)TWNewEgg.DB.TWSQLDB.Models.Item.ShowOrderStatus.AddtionalItemForCart)
            {
                if (!string.IsNullOrEmpty(IModel.Name))
                {
                    if (IModel.Name.IndexOf("加購_") == 0)
                    {
                        tempName = IModel.Name.Replace("加購_", "");
                    }
                }
            }
            else
            {
                tempName = IModel.Name;
            }

            productTempAdd.Name = tempName + propertyName;
            productTempAdd.NameTW = tempName + propertyName;

            #endregion 填寫商品名稱
            productTempAdd.Description = IModel.Description;
            productTempAdd.DescriptionTW = IModel.Description;
            productTempAdd.SPEC = ""; //SPEC已不使用;
            productTempAdd.ManufactureID = IModel.ManufactureID.GetValueOrDefault();
            productTempAdd.Model = IModel.Model;
            productTempAdd.BarCode = IModel.BarCode;
            productTempAdd.SellerID = sellerid;
            productTempAdd.DelvType = IModel.DelvType;
            productTempAdd.PicStart = 1;
            productTempAdd.PicEnd = 1;
            //productTempAdd.PicStart = IModel.PicStart;
            //productTempAdd.PicEnd = IModel.PicEnd;
            productTempAdd.Cost = IModel.Cost;
            productTempAdd.Status = 1;
            productTempAdd.InvoiceType = 0;//default value
            productTempAdd.SaleType = 0;//default value
            productTempAdd.Length = IModel.Length;
            productTempAdd.Width = IModel.Width;
            productTempAdd.Height = IModel.Height;
            productTempAdd.Weight = IModel.Weight;
            productTempAdd.CreateUser = userid.ToString();
            productTempAdd.CreateDate = dateTimeMillisecond;
            productTempAdd.Updated = 0;//default value
            productTempAdd.UpdateDate = dateTimeMillisecond;
            productTempAdd.UpdateUser = userid.ToString();
            if (IModel.TradeTax == null)
            {
                productTempAdd.TradeTax = 0;
            }
            else
            {
                productTempAdd.TradeTax = IModel.TradeTax;
            }
            //productTempAdd.TradeTax = 0;
            productTempAdd.Tax = 0;//default value
            productTempAdd.Warranty = IModel.Warranty;
            productTempAdd.UPC = IModel.UPC;
            productTempAdd.Note = IModel.Note;
            productTempAdd.IsMarket = "Y";
            productTempAdd.Is18 = IModel.Is18;
            productTempAdd.IsShipDanger = IModel.IsShipDanger;
            productTempAdd.IsChokingDanger = IModel.IsChokingDanger;
            productTempAdd.MenufacturePartNum = IModel.MenufacturePartNum;
            //productTempAdd.MenufacturePartNum = "";//製造商商品編號 (沒有輸入欄位) 暫時給空的
            db_before.ProductTemp.Add(productTempAdd);
            try
            {
                db_before.SaveChanges();
                result.IsSuccess = true;
                result.Body = productTempAdd.ID;
                //result.Msg = delcType.Body.ToString();//拿 Msg 存 delcType
            }
            catch (Exception error)
            {
                result.Msg = "資料處理錯誤";
                result.IsSuccess = false;
                logger.Error("sellerid is: " + sellerid + "; userid is: " + userid + ". errorMsg: " + error.Message + " [ErrorStackTrace]: " + error.StackTrace + " ;[ExceptionInnerMessage]: " + this.ExceptionInnerMessage(error));
            }
            return result;
        }
        #endregion
        #region 把資料 insert 到 itemtemp
        public ActionResponse<int> itemtempInsert(TWNewEgg.DB.TWSQLDB.Models.ItemSketch IModel, int productId, int userid)
        {
            ActionResponse<int> result = new ActionResponse<int>();
            DB.TWSqlDBContext db_before = new TWSqlDBContext();
            DateTime dateTimeMillisecond = new DateTime(0001, 01, 01, 01, 01, 01, 01);
            string timeString = dateTimeMillisecond.ToString("yyyy-MM-dd hh:mm:ss fff");
            dateTimeMillisecond = DateTime.Now;

            DB.TWSQLDB.Models.ItemTemp itemtempAdd = new DB.TWSQLDB.Models.ItemTemp();
            itemtempAdd.ProduttempID = productId;
            itemtempAdd.Name = IModel.Name;
            itemtempAdd.Sdesc = IModel.Sdesc;
            itemtempAdd.DescTW = IModel.Description;
            itemtempAdd.ItemTempDesc = IModel.Description;
            itemtempAdd.SpecDetail = "";//defalut value
            itemtempAdd.Spechead = IModel.Spechead;
            itemtempAdd.SaleType = 1;//defalut value
            itemtempAdd.PayType = 0;//defalut value
            itemtempAdd.Layout = 0;//defalut value
            itemtempAdd.DelvType = IModel.DelvType.GetValueOrDefault();
            itemtempAdd.DelvData = IModel.DelvDate ?? "";
            itemtempAdd.ItemNumber = "";//defalut value
            itemtempAdd.CategoryID = IModel.CategoryID.GetValueOrDefault();
            if (IModel.Model == null)
            {
                itemtempAdd.Model = "";
            }
            else
            {
                itemtempAdd.Model = IModel.Model;
            }
            itemtempAdd.SellerID = IModel.SellerID.GetValueOrDefault();
            itemtempAdd.DateStart = IModel.DateStart.GetValueOrDefault();
            itemtempAdd.DateEnd = IModel.DateEnd.GetValueOrDefault();
            itemtempAdd.DateDel = IModel.DateDel.GetValueOrDefault().AddDays(1);
            itemtempAdd.Pricesgst = 0;//defalut value
            itemtempAdd.PriceCard = IModel.PriceCard.GetValueOrDefault();
            itemtempAdd.PriceCash = IModel.PriceCash.GetValueOrDefault();
            itemtempAdd.ServicePrice = 0;
            itemtempAdd.PricehpType1 = 0;//defalut value
            itemtempAdd.Pricehpinst1 = 0;//defalut value
            itemtempAdd.PricehpType2 = 0;//defalut value
            itemtempAdd.Pricehpinst2 = 0;//defalut value
            itemtempAdd.Inst0Rate = 0;//defalut value
            itemtempAdd.RedmfdbckRate = 0;//defalut value
            itemtempAdd.Coupon = "0";//defalut value
            itemtempAdd.PriceCoupon = 0;//defalut value
            if (IModel.PriceLocalship == null)
            {
                itemtempAdd.PriceLocalship = 0;
            }
            else
            {
                itemtempAdd.PriceLocalship = (int)IModel.PriceLocalship;
            }
            itemtempAdd.PriceGlobalship = 0;//default value
            //if (itemTemp.PriceGlobalship == null)
            //{
            //    itemTemp.PriceGlobalship = 0;
            //}
            //else
            //{
            //    itemtempAdd.PriceGlobalship = (int)itemTemp.PriceGlobalship;
            //}
            //itemtemp.PriceTrade 不使用
            itemtempAdd.Qty = IModel.ItemQty.GetValueOrDefault();
            itemtempAdd.SafeQty = 0;
            itemtempAdd.QtyLimit = IModel.QtyLimit.GetValueOrDefault();
            itemtempAdd.LimitRule = "";//defalut value
            itemtempAdd.QtyReg = 0;//defalut value
            itemtempAdd.PhotoName = "";//defalut value
            itemtempAdd.HtmlName = "";//defalut value
            itemtempAdd.Showorder = IModel.ShowOrder ?? default(int);
            itemtempAdd.Class = 1;//defalut value
            itemtempAdd.Status = 1;//defalut value
            itemtempAdd.ManufactureID = IModel.ManufactureID.GetValueOrDefault();
            itemtempAdd.StatusNote = "";//defalut value
            itemtempAdd.StatusDate = dateTimeMillisecond;
            if (string.IsNullOrEmpty(itemtempAdd.Note) == true)
            {
                itemtempAdd.Note = "";
            }
            else
            {
                itemtempAdd.Note = IModel.Note;
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
            itemtempAdd.CreateUser = IModel.CreateUser;
            itemtempAdd.Updated = 0;//defalut value
            itemtempAdd.UpdateUser = IModel.UpdateUser;
            //UpdateDate(更新日期)：不給值(因為有更新才給)
            //itemtemp.UpdateDate
            itemtempAdd.PicStart = 1;
            itemtempAdd.PicEnd = 1;
            //itemtempAdd.PicStart = IModel.PicStart;
            //itemtempAdd.PicEnd = IModel.PicEnd;
            itemtempAdd.MarketPrice = IModel.MarketPrice;
            itemtempAdd.WareHouseID = IModel.WarehouseID;
            itemtempAdd.ShipType = IModel.ShipType;
            itemtempAdd.Taxfee = 0;//defalut value
            itemtempAdd.ItemPackage = IModel.ItemPackage;
            itemtempAdd.IsNew = IModel.IsNew;
            itemtempAdd.GrossMargin = IModel.GrossMargin;
            //itemtemp.ApproveMan = item
            //itemtemp.ApproveDate = 
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
                logger.Error("ErrorMessage: " + error.Message + " [ErrorStackTrace] " + error.StackTrace + " ;[ExceptionInnerMessage]: " + this.ExceptionInnerMessage(error) + "; ItemTemp insert error");
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                result.Code = (int)ResponseCode.Error;
            }
            return result;
        }
        #endregion
        #region 把資料 insert 到 ItemGroupProperty
        public ActionResponse<string> ItemGroupProperty(TWNewEgg.API.Models.BatchExamineModel batchExamineModel ,List<TWNewEgg.API.Models.propertyJoinModel> JModel, int groupid, int userid, int propertyId)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWSqlDBContext dbFront = new TWSqlDBContext();
            //order = 1
            int groupValueidTemp = batchExamineModel.colorsizeProperty[0].colorValueId;
            int groupValueid = dbFront.ItemPropertyValue.Where(p => p.ID == groupValueidTemp).Select(p => p.PropertyNameID).FirstOrDefault();
            //order = 2
            int valueidTemp = batchExamineModel.colorsizeProperty[0].listProperty[0].proValueId;
            int valueid = dbFront.ItemPropertyValue.Where(p => p.ID == valueidTemp).Select(p => p.PropertyNameID).FirstOrDefault();
            #region 第一順位
            TWNewEgg.DB.TWSQLDB.Models.ItemGroupProperty _itemgroupPropertyOrder1 = new DB.TWSQLDB.Models.ItemGroupProperty();
            _itemgroupPropertyOrder1.GroupID = groupid;
            _itemgroupPropertyOrder1.PropertyID = groupValueid;
            _itemgroupPropertyOrder1.PropertyName = JModel.Where(p => p.ItemPropertyName_ID == groupValueid).Select(p=>p.ItemPropertyName_propertyName).FirstOrDefault();
            _itemgroupPropertyOrder1.Order = 1;
            _itemgroupPropertyOrder1.CreateDate = DateTime.Now;
            _itemgroupPropertyOrder1.InUser = userid;
            _itemgroupPropertyOrder1.UpdateDate = DateTime.Now;
            _itemgroupPropertyOrder1.UpdateUser = userid;
            dbFront.ItemGroupProperty.Add(_itemgroupPropertyOrder1);
            #endregion
            #region 第二順位
            TWNewEgg.DB.TWSQLDB.Models.ItemGroupProperty _itemgroupPropertyOrder2 = new DB.TWSQLDB.Models.ItemGroupProperty();
            _itemgroupPropertyOrder2.GroupID = groupid;
            _itemgroupPropertyOrder2.PropertyID = valueid;
            _itemgroupPropertyOrder2.PropertyName = JModel.Where(p => p.ItemPropertyName_ID == valueid).Select(p => p.ItemPropertyName_propertyName).FirstOrDefault();
            _itemgroupPropertyOrder2.Order = 2;
            _itemgroupPropertyOrder2.CreateDate = DateTime.Now;
            _itemgroupPropertyOrder2.InUser = userid;
            _itemgroupPropertyOrder2.UpdateDate = DateTime.Now;
            _itemgroupPropertyOrder2.UpdateUser = userid;
            dbFront.ItemGroupProperty.Add(_itemgroupPropertyOrder2);
            #endregion
            #region version 20150825
            //listitemgrouppropertyModel = listitemgrouppropertyModel.Take(2).ToList();
            //foreach (var item in listitemgrouppropertyModel)
            //{
            //    TWNewEgg.DB.TWSQLDB.Models.ItemGroupProperty _itemgroupProperty = new DB.TWSQLDB.Models.ItemGroupProperty();
            //    _itemgroupProperty.GroupID = groupid;
            //    _itemgroupProperty.PropertyID = item.ItemPropertyName_ID;
            //    //order-> 1: color; 2: size
            //    if (item.ItemPropertyValue_ID == propertyId)
            //    {
            //        _itemgroupProperty.Order = 1;
            //    }
            //    else
            //    {
            //        _itemgroupProperty.Order = 2;
            //    }
            //    if (string.IsNullOrEmpty(item.ItemPropertyName_propertyName) == true)
            //    {
            //        _itemgroupProperty.PropertyName = item.ItemPropertyName_propertyNameTW;
            //    }
            //    else
            //    {
            //        _itemgroupProperty.PropertyName = item.ItemPropertyName_propertyName;
            //    }
            //    _itemgroupProperty.CreateDate = DateTime.Now;
            //    _itemgroupProperty.InUser = userid;
            //    _itemgroupProperty.UpdateDate = DateTime.Now;
            //    _itemgroupProperty.UpdateUser = userid;
            //    dbFront.ItemGroupProperty.Add(_itemgroupProperty);
            //}
            #endregion
            try
            {
                dbFront.SaveChanges();
                result.IsSuccess = true;
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "資料處理錯誤";
                logger.Error("[Message]: " + error.Message + "; [StackTrace]: " + error.StackTrace + " ;[ExceptionInnerMessage]: " + this.ExceptionInnerMessage(error));
            }
            return result;
        }
        #endregion
        #region 把資料 insert 到 Itemstocktemp
        public ActionResponse<string> ItemstocktempInsert(int proQty, int productTempId, int userid, int? InventorySafeQty)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            DB.TWSqlDBContext db_before = new TWSqlDBContext();
            DateTime dateTimeMillisecond = new DateTime(0001, 01, 01, 01, 01, 01, 01);
            string timeString = dateTimeMillisecond.ToString("yyyy-MM-dd hh:mm:ss fff");
            dateTimeMillisecond = DateTime.Now;
            DB.TWSQLDB.Models.ItemStocktemp _itemStocktemp = new DB.TWSQLDB.Models.ItemStocktemp();
            _itemStocktemp.producttempID = productTempId;
            //_itemStocktemp.Qty = _itemSketchtemp.ItemStock.CanSaleQty.GetValueOrDefault();
            //_itemStocktemp.Qty = _itemSketchtemp.proQty;
            _itemStocktemp.Qty = proQty;
            _itemStocktemp.QtyReg = 0;
            _itemStocktemp.SafeQty = InventorySafeQty.GetValueOrDefault();
            //_itemStocktemp.SafeQty = _itemSketchtemp.ItemStock.InventorySafeQty.GetValueOrDefault();
            _itemStocktemp.Fdbcklmt = 0;
            _itemStocktemp.CreateUser = userid.ToString();
            _itemStocktemp.Updated = 0;
            _itemStocktemp.CreateDate = dateTimeMillisecond;
            _itemStocktemp.UpdateUser = userid.ToString();
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
                logger.Error("ErrorMsg:  " + error.Message + " [ErrorStackTrace] " + error.StackTrace + " ;[ExceptionInnerMessage]: " + this.ExceptionInnerMessage(error) + "; ItemStocktemp insert error");
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                result.Code = (int)ResponseCode.Error;
            }
            return result;
        }
        #endregion
        #region 把資料 insert 到 ItemCategory
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
                logger.Error("MsgError: " + error.Message + "; [StackTrace]: " + error.StackTrace + " ;[ExceptionInnerMessage]: " + this.ExceptionInnerMessage(error));
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
            }
            return result;
        }
        #endregion
        #region 把資料 insert 到 productPropertytemp
        public ActionResponse<string> productPropertytempInsert(propertyModel _propertymodel, List<TWNewEgg.API.Models.propertyJoinModel> joinModelint, int sellerid, int userid, int productTempId, string inputValue, int propertyid)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            result.IsSuccess = true;
            DateTime dateTimeMillisecond = new DateTime(0001, 01, 01, 01, 01, 01, 01);
            string timeString = dateTimeMillisecond.ToString("yyyy-MM-dd hh:mm:ss fff");
            dateTimeMillisecond = DateTime.Now;
            TWSqlDBContext dbFront = new TWSqlDBContext();

            List<TWNewEgg.API.Models.SaveProductProperty> _listSaveProductProperty = new List<SaveProductProperty>();

            #region color
            TWNewEgg.API.Models.SaveProductProperty saveproPropertyColor = new SaveProductProperty();
            var colorProperty = joinModelint.Where(p => p.ItemPropertyValue_ID == propertyid).FirstOrDefault();
            saveproPropertyColor.GroupID = colorProperty.ItemPropertyName_groupId;
            saveproPropertyColor.PropertyID = colorProperty.ItemPropertyValue_propertyNameID;
            saveproPropertyColor.ValueID = colorProperty.ItemPropertyValue_ID;
            saveproPropertyColor.InputValue = string.Empty;
            saveproPropertyColor.UpdateUser = userid.ToString();
            _listSaveProductProperty.Add(saveproPropertyColor);
            #endregion
            #region size
            TWNewEgg.API.Models.SaveProductProperty saveproPropertySize = new SaveProductProperty();
            var sizeProperty = joinModelint.Where(p => p.ItemPropertyValue_ID == _propertymodel.proValueId).FirstOrDefault();
            saveproPropertySize.GroupID = sizeProperty.ItemPropertyName_groupId;
            saveproPropertySize.PropertyID = sizeProperty.ItemPropertyValue_propertyNameID;
            saveproPropertySize.ValueID = sizeProperty.ItemPropertyValue_ID;
            saveproPropertySize.InputValue = "";
            saveproPropertySize.UpdateUser = userid.ToString();
            _listSaveProductProperty.Add(saveproPropertySize);
            #endregion
            TWNewEgg.API.Service.ProductPorpertyTempService productPorpertyTempService = new ProductPorpertyTempService();
            var productPropertyTemp = productPorpertyTempService.SaveProductPropertyTempClick(_listSaveProductProperty, productTempId, userid);
            if (productPropertyTemp.IsSuccess == true)
            {
                result.IsSuccess = true;
                return result;
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = productPropertyTemp.Msg;
                return result;
            }
        }
        #endregion
        #region 把資料 insert 到 ItemGroupDetailProperty
        public ActionResponse<string> ItemGroupDetailPropertyInsert(int groupid, int itemTempid, int sellerid, int userid, List<TWNewEgg.API.Models.propertyJoinModel> jModel, colorSizeModel color_size_model, propertyModel property)
        {
            TWSqlDBContext dbFront = new TWSqlDBContext();
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.DB.TWSQLDB.Models.ItemGroupDetailProperty itemGroupDetailProperty_insert = new DB.TWSQLDB.Models.ItemGroupDetailProperty();
            // 利用 colorValueId 在 jModel 抓取對應的 MasterPropertyID 和 GroupValueId(第一順位)
            var MasterPropertyID_GroupValueId = jModel.Where(p => p.ItemPropertyValue_ID == color_size_model.colorValueId).FirstOrDefault();
            //利用 proValueId  在 jModel 抓取對應的 valueName 和 propertyid(第二順位)
            var valueId_valueName_propertyid = jModel.Where(p => p.ItemPropertyValue_ID == property.proValueId).FirstOrDefault();
            //群組 id
            itemGroupDetailProperty_insert.GroupID = groupid;
            //itemtemp 待審區 id
            itemGroupDetailProperty_insert.ItemTempID = itemTempid;
            //目前的 sellerid
            itemGroupDetailProperty_insert.SellerID = sellerid;
            //第一順位的 ItemPropertyName_ID
            itemGroupDetailProperty_insert.MasterPropertyID = MasterPropertyID_GroupValueId.ItemPropertyName_ID;
            //第二順位的 ItemPropertyName_ID, 0: 此商品為一維屬性
            itemGroupDetailProperty_insert.PropertyID = valueId_valueName_propertyid == null ? 0 : valueId_valueName_propertyid.ItemPropertyName_ID;
            //第一順位的 ItemPropertyValue_ID
            itemGroupDetailProperty_insert.GroupValueID = MasterPropertyID_GroupValueId.ItemPropertyValue_ID;
            //第二順位的 ItemPropertyValue_ID
            itemGroupDetailProperty_insert.ValueID = valueId_valueName_propertyid.ItemPropertyValue_ID;
            //第二順位對應的 ItemPropertyValue_propertyValue
            itemGroupDetailProperty_insert.ValueName = valueId_valueName_propertyid == null ? string.Empty : valueId_valueName_propertyid.ItemPropertyValue_propertyValue;
            //第二順位的自定義
            itemGroupDetailProperty_insert.InputValue = color_size_model.inputValue;
            itemGroupDetailProperty_insert.CreateDate = DateTime.Now;
            itemGroupDetailProperty_insert.InUser = userid;
            itemGroupDetailProperty_insert.UpdateDate = DateTime.Now;
            itemGroupDetailProperty_insert.UpdateUser = userid.ToString();
            dbFront.ItemGroupDetailProperty.Add(itemGroupDetailProperty_insert);
            try
            {
                dbFront.SaveChanges();
                result.IsSuccess = true;
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "資料處理錯誤";
                logger.Error("[Message]: " + error.Message + "; [StackTrace]: " + error.StackTrace + " ;[ExceptionInnerMessage]: " + this.ExceptionInnerMessage(error));
            }
            return result;
        }
        #endregion
        #region 圖片處理
        public ActionResponse<List<string>> imgProcess(int? picStart, int? picEnd, int itemsketchid, int itemTempid, int mapindex)
        {
            TWNewEgg.API.Service.ImageService imgService = new ImageService();
            ActionResponse<List<string>> result = new ActionResponse<List<string>>();
            bool ConfigIsNoError = true;
            string images = string.Empty;
            int picID = itemsketchid;
            // 使用ItemID產生對應圖片網址        
            string pid = picID.ToString("00000000");
            string pidf4 = pid.Substring(0, 4);
            string pidl4 = pid.Substring(4, 4);
            try
            {
                images = System.Configuration.ConfigurationManager.AppSettings["Images"];
                ConfigIsNoError = true;
            }
            catch (Exception error)
            {
                ConfigIsNoError = false;
                logger.Error("[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }
            if (ConfigIsNoError == false)
            {
                result.IsSuccess = false;
                result.Msg = "送審資料處理錯誤";
                return result;
            }
            List<string> urlList = new List<string>();
            string url = images + "/pic/itemsketch/" + pid.Substring(0, 4) + "/" + pid.Substring(4, 4) + "_" + mapindex + "_640.jpg";
            logger.Info("itemsketchid: " + itemsketchid + " ;itemTempid: " + itemTempid + " ;url: " + url);
            urlList.Add(url);
            //for (int index = 1; index <= picEnd; index++)
            //{
            //    string url = images + "/pic/itemsketch/" + pid.Substring(0, 4) + "/" + pid.Substring(4, 4) + "_" + index + "_640.jpg";
            //    urlList.Add(url);
            //}
            var ImgProcess = imgService.ImageProcess(urlList, "pic\\itemtemp", "pic\\pic\\itemtemp", itemTempid);
            if (ImgProcess.IsSuccess == true)
            {
                result.IsSuccess = true;
                result.Msg = ImgProcess.Msg;
                return result;
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = ImgProcess.Msg;
                return result;
            }
        }
        #endregion
        #region 把相關資料 join 回來, function name: joinPropertymodel
        public ActionResponse<List<TWNewEgg.API.Models.propertyJoinModel>> joinPropertymodel(TWNewEgg.API.Models.BatchExamineModel batchNodelJoin)
        {
            ActionResponse<List<TWNewEgg.API.Models.propertyJoinModel>> result = new ActionResponse<List<propertyJoinModel>>();
            TWSqlDBContext dbFront = new TWSqlDBContext();
            List<int> idModel = new List<int>();
            List<int> colorPropertyNameID = new List<int>();
            foreach (var iditem in batchNodelJoin.colorsizeProperty)
            {
                idModel.Add(iditem.colorValueId);
                colorPropertyNameID.Add(iditem.colorValueId);
                foreach (var itemPro in iditem.listProperty)
                {
                    idModel.Add(itemPro.proValueId);
                }
            }
            List<TWNewEgg.API.Models.propertyJoinModel> JoinModel = new List<propertyJoinModel>();
            IEnumerable<TWNewEgg.DB.TWSQLDB.Models.ItemPropertyValue> itempropertyValueAsQuery = dbFront.ItemPropertyValue.AsEnumerable();
            IEnumerable<TWNewEgg.DB.TWSQLDB.Models.ItemPropertyName> iItemPropertyNameAsQuery = dbFront.ItemPropertyName.AsEnumerable();
            
            try
            {
                var asQueryTemp = (from p in idModel.AsEnumerable()
                                   join q in itempropertyValueAsQuery on p equals q.ID
                                   join r in iItemPropertyNameAsQuery on q.PropertyNameID equals r.ID
                                   select new TWNewEgg.API.Models.propertyJoinModel
                                    {
                                        ItemPropertyValue_ID = p,
                                        ItemPropertyValue_propertyNameID = q.PropertyNameID,
                                        ItemPropertyValue_propertyValue = q.PropertyValue,
                                        ItemPropertyValue_propertyValueTW = q.PropertyValueTW,
                                        ItemPropertyName_groupId = r.GroupID,
                                        ItemPropertyName_ID = r.ID,
                                        ItemPropertyName_propertyName = r.PropertyName,
                                        ItemPropertyName_propertyNameTW = r.PropertyNameTW
                                    }).AsQueryable();
                JoinModel = asQueryTemp.ToList();
                //JoinModel = (from p in idModel
                //             join q in dbFront.ItemPropertyValue on p equals q.ID
                //             join r in dbFront.ItemPropertyName on q.PropertyNameID equals r.ID
                //             select new TWNewEgg.API.Models.propertyJoinModel
                //             {
                //                 ItemPropertyValue_ID = p,
                //                 ItemPropertyValue_propertyNameID = q.PropertyNameID,
                //                 ItemPropertyValue_propertyValue = q.PropertyValue,
                //                 ItemPropertyValue_propertyValueTW = q.PropertyValueTW,
                //                 ItemPropertyName_groupId = r.GroupID,
                //                 ItemPropertyName_ID = r.ID,
                //                 ItemPropertyName_propertyName = r.PropertyName,
                //                 ItemPropertyName_propertyNameTW = r.PropertyNameTW
                //             }).ToList();
                if (idModel.Count != JoinModel.Count)
                {
                    result.IsSuccess = false;
                    result.Msg = "資料錯誤";
                    logger.Error("輸入的 idModel count 與 JoinModel Count 不相等");
                }
                else
                {
                    var checkPropertyIDModel = JoinModel.Where(p => colorPropertyNameID.Contains(p.ItemPropertyValue_ID)).Select(p => p.ItemPropertyValue_propertyNameID).Distinct().ToList();
                    if (checkPropertyIDModel.Count != 1)
                    {
                        result.IsSuccess = false;
                        result.Msg = "資料錯誤";
                        logger.Error("color property_id 不相等");
                    }
                    else
                    {
                        result.IsSuccess = true;
                        result.Body = JoinModel;
                    }
                }
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "資料處理錯誤";
                logger.Error("[Message]: " + error.Message + "; [StackTrace]: " + error.StackTrace);
            }
            return result;
        }
        #endregion
        #region 草稿區送審時檢查資料完整性
        public ActionResponse<string> checkModelElement(List<DB.TWSQLDB.Models.ItemSketch> itemsketch)
        {

            ActionResponse<string> result = new ActionResponse<string>();
            result.IsSuccess = true;
            result.Msg = string.Empty;
            TWNewEgg.DB.TWSqlDBContext dbFront = new TWSqlDBContext();
            List<int> _listSketchId = new List<int>();
            _listSketchId = itemsketch.Select(p => p.ID).ToList();
            List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchProperty> list_ItemSketchProperty = new List<DB.TWSQLDB.Models.ItemSketchProperty>();
            list_ItemSketchProperty = dbFront.ItemSketchProperty.Where(p => _listSketchId.Contains(p.ItemSketchID)).ToList();
            foreach (var item in itemsketch)
            {
                #region 商品類別
                if (item.CategoryID == null || item.CategoryID == 0)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請選擇商品主分類";
                    break;
                }
                #endregion
                #region 製造商
                if (item.ManufactureID == null || item.ManufactureID == 0)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請選擇製造商";
                    break;
                }
                #endregion
                #region 材積(公分) 長
                if (item.Length == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選材積\"長\"";
                    break;
                }
                else
                {
                    if (item.Length <= 0)
                    {
                        result.IsSuccess = false;
                        result.Msg = "產品編號為: " + item.ID + ",材積(公分) 長: 必須大於等於 0";
                        break;
                    }
                }
                #endregion
                #region 材積(公分) 寬
                if (item.Width == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選材積\"寬\"";
                    break;
                }
                else
                {
                    if (item.Width <= 0)
                    {
                        result.IsSuccess = false;
                        result.Msg = "產品編號為: " + item.ID + ",材積(公分) 寬: 必須大於等於 0";
                        break;
                    }
                }
                #endregion
                #region 材積(公分) 高
                if (item.Height == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選材積\"高\"";
                    break;
                }
                else
                {
                    if (item.Height <= 0)
                    {
                        result.IsSuccess = false;
                        result.Msg = "產品編號為: " + item.ID + ",材積(公分) 長: 必須大於等於 0";
                        break;
                    }
                }
                #endregion
                #region 重量(公斤)
                if (item.Weight == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選材積\"重量\"";
                    break;
                }
                else
                {
                    if (item.Weight <= 0)
                    {
                        result.IsSuccess = false;
                        result.Msg = "產品編號為: " + item.ID + ",重量(公斤) : 必須大於等於 0";
                        break;
                    }
                }
                #endregion
                #region 窒息危險性
                if (string.IsNullOrEmpty(item.IsChokingDanger) == true)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\"窒息危險性\"";
                    break;
                }
                #endregion
                #region 遞送危險物料
                if (string.IsNullOrEmpty(item.IsShipDanger) == true)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\"遞送危險物料\"";
                    break;
                }
                #endregion
                #region 遞送方式
                if (string.IsNullOrEmpty(item.ItemPackage) == true)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\"遞送方式\"";
                    break;
                }
                #endregion
                #region 售價
                if (item.PriceCash == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\"售價\"";
                    break;
                }
                #endregion
                #region 成本
                if (item.Cost == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\"成本\"";
                    break;
                }
                #endregion
                #region 毛利率

                if ((item.PriceCash.HasValue && item.Cost.HasValue) && item.Cost.Value > item.PriceCash.Value)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",\"毛利\"率為負數，請重新設定售價或成本";
                    break;
                }

                #endregion 毛利率
                #region 安全庫存
                if (item.InventorySafeQty <= 0)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",\"安全庫存\"不可為0";
                    break;
                }
                #endregion
                #region 商品名稱
                if (string.IsNullOrEmpty(item.Name) == true)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\"商品名稱\"";
                    break;
                }
                #endregion
                #region 商品簡要描述
                if (string.IsNullOrEmpty(item.Spechead) == true)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\" 商品簡要描述\"";
                    break;
                }
                #endregion
                #region 商品特色標題
                if (string.IsNullOrEmpty(item.Sdesc) == true)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\" 商品特色標題\"";
                    break;
                }
                #endregion
                #region 商品中文說明
                if (string.IsNullOrEmpty(item.Description) == true)
                {
                    result.IsSuccess = false;
                    result.Msg = "產品編號為: " + item.ID + ",請填選\" 商品中文說明\"";
                    break;
                }
                #endregion
                #region 賣場開賣時間
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
                #endregion
                #region 安全庫存不可小於現購數量

                list_ItemSketchProperty = list_ItemSketchProperty.Where(p => p.ItemSketchID == item.ID && p.GroupValueID != p.ValueID).ToList();
                    foreach (var check_item_ItemSketchProperty in list_ItemSketchProperty)
                    {
                        if ((item.InventorySafeQty - check_item_ItemSketchProperty.Qty) > 0)
                        {
                            result.IsSuccess = false;
                            result.Msg = "產品編號為: " + item.ID + ",\"安全庫存不可大於可售數量\"";
                            break;
                        }

                    }
                    if (result.IsSuccess == false)
                    {
                        break;
                    }

                #endregion
            }

            return result;
        }
        #endregion
        #region 20150820 送審全部統一成一個 row
        public ActionResponse<string> ItemGroupDetailPropertyInsert_20150820(TWNewEgg.API.Models.propertyJoinModel listItemGroupDetailproperty, int itemtempid, string inputValue, int userid, /*bool isColorProperty,*/ int groupValueID, int groupId, int sellerid)
        {
            //TWSqlDBContext dbFront = new TWSqlDBContext();
            ActionResponse<string> result = new ActionResponse<string>();
            //TWNewEgg.DB.TWSQLDB.Models.ItemGroupDetailProperty _itemGDProperty = new DB.TWSQLDB.Models.ItemGroupDetailProperty();

            //if (isColorProperty == true)
            //{
            //    _itemGDProperty.GroupID = groupId;
            //    //_itemGDProperty.GroupID = listItemGroupDetailproperty.ItemPropertyName_groupId;
            //    _itemGDProperty.PropertyID = listItemGroupDetailproperty.ItemPropertyValue_propertyNameID;
            //    _itemGDProperty.GroupValueID = listItemGroupDetailproperty.ItemPropertyValue_ID;
            //    _itemGDProperty.ValueID = listItemGroupDetailproperty.ItemPropertyValue_ID;
            //    _itemGDProperty.ValueName = listItemGroupDetailproperty.ItemPropertyValue_propertyValueTW;
            //    _itemGDProperty.InputValue = inputValue;
            //    _itemGDProperty.CreateDate = DateTime.Now;
            //    _itemGDProperty.InUser = userid;
            //    _itemGDProperty.SellerID = sellerid;
            //    _itemGDProperty.UpdateDate = DateTime.Now;
            //    _itemGDProperty.UpdateUser = userid.ToString();
            //    dbFront.ItemGroupDetailProperty.Add(_itemGDProperty);
            //}
            //else
            //{
            //    _itemGDProperty.GroupID = groupId;
            //    //_itemGDProperty.GroupID = listItemGroupDetailproperty.ItemPropertyName_groupId;
            //    _itemGDProperty.ItemTempID = itemtempid;
            //    _itemGDProperty.PropertyID = listItemGroupDetailproperty.ItemPropertyValue_propertyNameID;
            //    //_itemGDProperty.GroupValueID = listItemGroupDetailproperty.ItemPropertyValue_ID;
            //    _itemGDProperty.GroupValueID = groupValueID;
            //    _itemGDProperty.ValueID = listItemGroupDetailproperty.ItemPropertyValue_ID;
            //    _itemGDProperty.ValueName = listItemGroupDetailproperty.ItemPropertyValue_propertyValueTW;
            //    _itemGDProperty.InputValue = inputValue;
            //    //_itemGDProperty.InputValue = "";
            //    _itemGDProperty.CreateDate = DateTime.Now;
            //    _itemGDProperty.InUser = userid;
            //    _itemGDProperty.UpdateDate = DateTime.Now;
            //    _itemGDProperty.SellerID = sellerid;
            //    _itemGDProperty.UpdateUser = userid.ToString();
            //    dbFront.ItemGroupDetailProperty.Add(_itemGDProperty);
            //}
            //try
            //{
            //    dbFront.SaveChanges();
            //    result.IsSuccess = true;
            //}
            //catch (Exception error)
            //{
            //    result.IsSuccess = false;
            //    result.Msg = "資料處理錯誤";
            //    logger.Error("[Message]: " + error.Message + "; [StackTrace]: " + error.StackTrace);
            //}
            return result;
        }
        public ActionResponse<string> ItemGroupDetailPropertyInsert1(TWNewEgg.API.Models.propertyJoinModel listItemGroupDetailproperty, int colorproperty, int itemtempid, string inputValue, int userid, int ItemPropertyValue_ID, int groupId, int sellerid)
        {
            TWSqlDBContext dbFront = new TWSqlDBContext();
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.DB.TWSQLDB.Models.ItemGroupDetailProperty _itemGDProperty = new DB.TWSQLDB.Models.ItemGroupDetailProperty();
            _itemGDProperty.GroupID = groupId;
            _itemGDProperty.ItemTempID = itemtempid;
            _itemGDProperty.SellerID = sellerid;
            _itemGDProperty.MasterPropertyID = ItemPropertyValue_ID;
            _itemGDProperty.PropertyID = listItemGroupDetailproperty.ItemPropertyValue_propertyNameID;
            _itemGDProperty.GroupValueID = colorproperty;
            _itemGDProperty.ValueID = listItemGroupDetailproperty.ItemPropertyValue_ID;
            _itemGDProperty.ValueName = listItemGroupDetailproperty.ItemPropertyValue_propertyValueTW;
            _itemGDProperty.InputValue = inputValue;
            _itemGDProperty.CreateDate = DateTime.Now;
            _itemGDProperty.InUser = userid;
            _itemGDProperty.UpdateDate = DateTime.Now;
            _itemGDProperty.UpdateUser = userid.ToString();
            dbFront.ItemGroupDetailProperty.Add(_itemGDProperty);
            try
            {
                dbFront.SaveChanges();
                result.IsSuccess = true;
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "資料處理錯誤";
                logger.Error("[Message]: " + error.Message + "; [StackTrace]: " + error.StackTrace);
            }
            return result;

        }
        #endregion
        #endregion 草稿區送審
        #region 預覽規格品草稿查詢
        public ActionResponse<List<TWNewEgg.API.Models.ItemGroupJoinitemsketchListData>> ItemSketchDetailSearch(ItemSketchSearchCondition condition, bool IsTempCopy = false)
        {
            TWSqlDBContext dbfront = new TWSqlDBContext();
            ActionResponse<List<TWNewEgg.API.Models.ItemGroupJoinitemsketchListData>> result = new ActionResponse<List<TWNewEgg.API.Models.ItemGroupJoinitemsketchListData>>();
            List<TWNewEgg.API.Models.ItemGroupJoinitemsketchListData> searchdata = new List<TWNewEgg.API.Models.ItemGroupJoinitemsketchListData>();
            List<TWNewEgg.API.Models.ItemGroupJoinitemsketchListData> basicitemdata = new List<TWNewEgg.API.Models.ItemGroupJoinitemsketchListData>();
            List<TWNewEgg.API.Models.ItemGroupJoinitemsketchListData> searchresult = new List<TWNewEgg.API.Models.ItemGroupJoinitemsketchListData>();
            List<TWNewEgg.API.Models.ItemGroupJoinitemsketchListData> searchitem = new List<TWNewEgg.API.Models.ItemGroupJoinitemsketchListData>();
            int sketchid = int.Parse(condition.KeyWord);
            try
            {
                IEnumerable<TWNewEgg.DB.TWSQLDB.Models.ItemSketchProperty> ItemSketchProperty_Asquery = dbfront.ItemSketchProperty.AsEnumerable();
                IEnumerable<TWNewEgg.DB.TWSQLDB.Models.ItemPropertyValue> ItemPropertyValue_Asquery = dbfront.ItemPropertyValue.AsEnumerable();
                IEnumerable<TWNewEgg.DB.TWSQLDB.Models.ItemSketchGroup> ItemSketchGroup_Asquery = dbfront.ItemSketchGroup.AsEnumerable();
                searchdata = (from p in ItemSketchProperty_Asquery
                              join q in ItemPropertyValue_Asquery
                              on new { id = p.ItemSketchID, GroupValueID = p.GroupValueID } equals
                              new { id = sketchid, GroupValueID = q.ID }
                              select new TWNewEgg.API.Models.ItemGroupJoinitemsketchListData
                              {
                                  ItemTempId = p.ItemSketchID,
                                  groupID = p.GroupID,
                                  PropertyID = int.Parse(p.PropertyID.ToString()),
                                  GroupValueID = p.GroupValueID,
                                  ValueID = p.ValueID,
                                  ValueName = p.ValueName,
                                  definitions = string.IsNullOrEmpty(p.InputValue) == true ? q.PropertyValueTW : p.InputValue

                              }).AsQueryable().ToList();

                basicitemdata = searchdata.Where(x => x.GroupValueID == x.ValueID).ToList();
                foreach (var basicitem in basicitemdata)
                {
                    searchitem = searchdata
                                    .Where(x => x.GroupValueID == basicitem.GroupValueID && x.GroupValueID != x.ValueID)
                                    .Select(x => new TWNewEgg.API.Models.ItemGroupJoinitemsketchListData
                                    {
                                        ItemTempId = x.ItemTempId,
                                        groupID = x.groupID,
                                        MasterPropertyID = basicitem.PropertyID,
                                        PropertyID = x.PropertyID,
                                        GroupValueID = x.GroupValueID,
                                        ValueID = x.ValueID,
                                        ValueName = x.ValueName,
                                        definitions = basicitem.definitions,
                                    }).AsQueryable().ToList();
                    searchresult.AddRange(searchitem);

                }
                if (basicitemdata.Count != 0 && searchresult.Count == 0)
                {
                    basicitemdata = basicitemdata
                                    .Select(x => new TWNewEgg.API.Models.ItemGroupJoinitemsketchListData
                                    {
                                        ItemTempId = x.ItemTempId,
                                        groupID = x.groupID,
                                        MasterPropertyID = x.PropertyID,
                                        PropertyID = x.PropertyID,
                                        GroupValueID = x.GroupValueID,
                                        ValueID = x.ValueID,
                                        //ValueName = x.ValueName,
                                        definitions = x.definitions,
                                    }).AsQueryable().ToList();
                    searchresult = basicitemdata;
                }
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "草稿預覽查詢資料發生錯誤";
                logger.Error("草稿預覽查詢錯誤發生 ;" + "[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }


            if (searchresult.Count == 0)
            {
                result.IsSuccess = false;
                result.Msg = "草稿預覽查無資料";
            }
            else
            {
                result.IsSuccess = true;
                result.Msg = "草稿預覽成功查詢";
                result.Body = searchresult;
                logger.Info("草稿預覽群組查詢成功");
            }
            return result;
        }
        #endregion
        #region 草稿預覽單品查詢
        public ActionResponse<List<TWNewEgg.API.Models.ItemSketch>> ItemSketchPreviewSearch(ItemSketchSearchCondition condition, bool IsTempCopy = false)
        {
            TWSqlDBContext dbfront = new TWSqlDBContext();
            ActionResponse<List<TWNewEgg.API.Models.ItemSketch>> result = new ActionResponse<List<ItemSketch>>();
            //呼叫原本的 itemsketch search api
            var itemSketchServiceResult = itemSketchService.GetItemSketchList(condition, IsTempCopy);
            if (itemSketchServiceResult.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = itemSketchServiceResult.Msg;
                return result;
            }
            List<ItemSketch> hasPropertyItemSketch = new List<ItemSketch>();
            //List<int> itemSketchDeleteId = new List<int>();
            try
            {
                //只取有屬性的草稿資料
                hasPropertyItemSketch = itemSketchServiceResult.Body.Where(p => dbfront.ItemSketchGroup.Select(q => q.ItemSketchID).Contains(p.ID)).ToList();
                //沒有符合的資料
                if (hasPropertyItemSketch.Count == 0)
                {
                    result.IsSuccess = false;
                    result.Msg = "查無資料";
                }
                else
                {
                    #region 把不會秀在畫面的資訊清除，避免回傳時轉換成 JSON STRING 時超過長度限制, 也避免造成修改轉換成 JSON STRING 時 有危險的符號
                    //把不會秀在畫面的資訊清除，避免回傳時轉換成 JSON STRING 時超過長度限制, 也避免造成修改轉換成 JSON STRING 時 有危險的符號
                    //hasPropertyItemSketch.ForEach(p => p.Item.Sdesc = "");
                    //hasPropertyItemSketch.ForEach(p => p.Product.Description = "");
                    //hasPropertyItemSketch.ForEach(p => p.Item.Spechead = "");
                    hasPropertyItemSketch.ForEach(p => p.ItemStock.CanSaleQty = p.ItemStock.InventoryQty);
                    hasPropertyItemSketch.ForEach(p => p.Item.Note = "");
                    itemSketchServiceResult.Body = hasPropertyItemSketch;

                    #endregion
                    result.IsSuccess = true;
                    result.Body = itemSketchServiceResult.Body;
                    result.Msg = "查詢成功";
                }
            }
            catch (Exception error)
            {
                result.Msg = "查詢失敗";
                result.IsSuccess = false;
                logger.Error("[Msg]: " + error.Message + " ;StackTrace: " + error.StackTrace + " ;[ExceptionInnerMessage]: " + this.ExceptionInnerMessage(error));
            }
            return result;
        }
        #endregion
        #endregion 草稿區















        #region 待審區
        #region 待審區查詢
        public ActionResponse<List<TWNewEgg.API.Models.sketchPropertyExamine>> ItemSketchPropertyListSearch(ItemSketchSearchCondition itemsketchListData, bool isSearch = true)
        {
            ActionResponse<List<TWNewEgg.API.Models.sketchPropertyExamine>> result = new ActionResponse<List<sketchPropertyExamine>>();
            List<TWNewEgg.API.Models.sketchPropertyExamine> _listSketchPropertyExamine = new List<Models.sketchPropertyExamine>();
            
            TWNewEgg.API.Service.TempService tempService = new TempService();
            DB.TWSqlDBContext dbFront = new TWSqlDBContext();
            TWNewEgg.API.Service.PropertyService propertyService = new PropertyService();
            int sellerid = itemsketchListData.SellerID;
            //呼叫查詢的 service action
            var searchPropertyResult = propertyService.PropertyServiceList(itemsketchListData);
            if (searchPropertyResult.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = searchPropertyResult.Msg;
                return result;
            }
            //利用 sellerid JOIN 查詢相關的資料, 利用 join 回來的屬性資料進行顏色和尺寸的回填
            IEnumerable<TWNewEgg.DB.TWSQLDB.Models.ItemGroupDetailProperty> ItemGroupDetailProperty_Asquery = dbFront.ItemGroupDetailProperty.AsEnumerable();
            IEnumerable<TWNewEgg.DB.TWSQLDB.Models.ItemPropertyValue> ItemPropertyValuey_Asquery = dbFront.ItemPropertyValue.AsEnumerable();
            var joinItemGroupDetailProperty_ItemPropertyValue = (from p in ItemGroupDetailProperty_Asquery
                                                                 join q in ItemPropertyValuey_Asquery
                                                                     on p.GroupValueID equals q.ID
                                                                 where p.SellerID == itemsketchListData.SellerID
                                                                 select new TWNewEgg.API.Models.ItemGroupJoinitemsketchListData
                                                                 {
                                                                     SellerId = p.SellerID,
                                                                     ItemTempId = p.ItemTempID,
                                                                     groupID = p.GroupID,
                                                                     GroupValueID = p.GroupValueID,
                                                                     ValueID = p.ValueID,
                                                                     ValueName = p.ValueName,
                                                                     definitions = p.InputValue,
                                                                     propertyValue = q.PropertyValue
                                                                 }).AsQueryable();
            #region join version 1 and 2
            //var joinItemGroupDetailProperty_ItemPropertyValue1 = (from p in dbFront.ItemGroupDetailProperty
            //                                                     join q in dbFront.ItemPropertyValue on p.GroupValueID equals q.ID
            //                                                     where p.SellerID == itemsketchListData.SellerID
            //                                                     select new TWNewEgg.API.Models.ItemGroupJoinitemsketchListData
            //                                                     {
            //                                                         SellerId = p.SellerID,
            //                                                         ItemTempId = p.ItemTempID,
            //                                                         groupID = p.GroupID,
            //                                                         GroupValueID = p.GroupValueID,
            //                                                         ValueID = p.ValueID,
            //                                                         ValueName = p.ValueName,
            //                                                         definitions = p.InputValue,
            //                                                         propertyValue = q.PropertyValue
            //                                                     }).AsQueryable();
                                                                                                            
            //var joindata = (from p in dbFront.ItemGroup
            //                join q in dbFront.ItemGroupDetailProperty on p.SellerID equals q.SellerID
            //                //where p.ID == q.GroupID //&& q.SellerID == itemsketchListData.SellerID
            //                where p.SellerID == itemsketchListData.SellerID
            //                select new TWNewEgg.API.Models.ItemGroupJoinitemsketchListData
            //                {
            //                    SellerId = p.SellerID,
            //                    ItemTempId = q.ItemTempID,
            //                    ValueName = q.ValueName,
            //                    GroupValueID = q.GroupValueID,
            //                    MasterPropertyID = p.MasterPropertyID,
            //                    PropertyID = q.PropertyID,
            //                    groupID = p.ID,
            //                    ValueID = q.ValueID,
            //                    definitions = q.InputValue
            //                }).AsQueryable();//ToList().Where(p => searchPropertyResult.Body.Select(q => q.Item.ID).Contains(p.ItemTempId.GetValueOrDefault())).ToList();
            //利用 join 回來的屬性資料進行顏色和尺寸的回填
            //joindata = joindata.Where(p => searchPropertyResult.Body.Select(q => q.Item.ID).Contains(p.ItemTempId.GetValueOrDefault())).ToList();
            #endregion
            //把要回傳的 MODEL AUTOMAP 回來
            var bindResult = this.bindData(searchPropertyResult.Body, joinItemGroupDetailProperty_ItemPropertyValue, sellerid, isSearch);
            if (bindResult.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = "查詢失敗，請稍後再試";
                return result;
            }
            else
            {
                int totalNum = bindResult.Body.Count;

                if (itemsketchListData.pageInfo != null)
                {
                    if (itemsketchListData.pageInfo.PageIndex >= 0 || itemsketchListData.pageInfo.PageSize > 0)
                    {
                        bindResult.Body = bindResult.Body.Skip(itemsketchListData.pageInfo.PageIndex * itemsketchListData.pageInfo.PageSize).Take(itemsketchListData.pageInfo.PageSize).ToList();
                    }
                }

                result.IsSuccess = true;
                result.Body = bindResult.Body;
                result.Msg = totalNum.ToString();
                return result;
            }  
        }
        #endregion
        #region 待審區查詢 把資料 automap 成畫面上的 MODEL
        public ActionResponse<List<TWNewEgg.API.Models.sketchPropertyExamine>> bindData(List<Models.sketchPropertyExamine> itemsketch, IEnumerable<TWNewEgg.API.Models.ItemGroupJoinitemsketchListData> jData, int sellerid, bool isSearch = true)
        {
            ActionResponse<List<TWNewEgg.API.Models.sketchPropertyExamine>> result = new ActionResponse<List<sketchPropertyExamine>>();
            TWNewEgg.DB.TWSqlDBContext dbfront = new TWSqlDBContext();
            result.Body = new List<sketchPropertyExamine>();
            var jDataTolist = jData.ToList();
            try
            {
                foreach (var item in itemsketch)
                {
                    if (isSearch == true)
                    {
                        //把沒有必要秀在畫面上和會造成 JSON 錯誤的資訊拿掉
                        item.Item.Sdesc = "";
                        item.Product.Description = "";
                        item.Item.Spechead = "";
                        //item.ItemStock.CanSaleQty = item.ItemStock.InventoryQty;
                        item.Item.Note = "";
                        AutoMapper.Mapper.CreateMap<TWNewEgg.API.Models.ItemSketch, TWNewEgg.API.Models.sketchPropertyExamine>();
                        TWNewEgg.API.Models.sketchPropertyExamine autoData = AutoMapper.Mapper.Map<TWNewEgg.API.Models.ItemSketch, TWNewEgg.API.Models.sketchPropertyExamine>(item);
                    }
                    else
                    {
                        //item.ItemStock.CanSaleQty = item.ItemStock.InventoryQty;
                        AutoMapper.Mapper.CreateMap<TWNewEgg.API.Models.ItemSketch, TWNewEgg.API.Models.sketchPropertyExamine>();
                        TWNewEgg.API.Models.sketchPropertyExamine autoData = AutoMapper.Mapper.Map<TWNewEgg.API.Models.ItemSketch, TWNewEgg.API.Models.sketchPropertyExamine>(item);
                    }
                    #region property
                    var nowProperty = jDataTolist.Where(p => p.ItemTempId == item.Item.ID).FirstOrDefault();
                    if (nowProperty == null)
                    {
                        logger.Info("Itemtempid");
                        item.size = "";
                        item.color = "";
                        item.definitions = "";
                    }
                    else
                    {
                        item.size = string.IsNullOrEmpty(nowProperty.ValueName) == true ? "" : nowProperty.ValueName;
                        item.color = string.IsNullOrEmpty(nowProperty.propertyValue) == true ? "" : nowProperty.propertyValue;
                        item.definitions = string.IsNullOrEmpty(nowProperty.definitions) == true ? "" : nowProperty.definitions;
                    }
                    #endregion
                }
                result.Body = itemsketch;
                result.IsSuccess = true;
            }
            catch (Exception error)
            {
                logger.Error("[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
                result.IsSuccess = false;
                result.Msg = "資料處理錯誤";
            }
            return result;
        }
        #endregion
        #region 待審區刪除
        public ActionResponse<List<string>> ItemSketchPropertyListDelete(List<int> deleteID)
        {
            ActionResponse<List<string>> result = new ActionResponse<List<string>>();
            TWNewEgg.API.Service.TempService tempService = new TempService();
            try
            {
                result = tempService.DeleteTemp(deleteID);
            }
            catch (Exception error)
            {
                result.Msg = "刪除失敗，請稍後再試";
                result.IsSuccess = false;
                logger.Error("[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }
            return result;
        }
        #endregion
        #region 待審區修改(Grid)
        public ActionResponse<string> ItemSketchPropertyListUpdate(List<TWNewEgg.API.Models.ItemSketch> updateModel)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWSqlDBContext dbFront = new TWSqlDBContext();
            string flagEditSwitch = string.Empty;
            #region 初始化
            result.IsSuccess = true;
            result.Msg = string.Empty;
            #endregion 初始化
            TWNewEgg.API.Service.TempService tempService = new TempService();
            ActionResponse<List<string>> checkResult = new ActionResponse<List<string>>();
            try
            {
                checkResult = tempService.TempListEdit(updateModel);
                result.IsSuccess = true;
            }
            catch (Exception error)
            {
                logger.Error("[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
                result.IsSuccess = false;
            }
            if (result.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = "資料處理錯誤";
                return result;
            }
            if (checkResult.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = checkResult.Msg;
                return result;
            }
            else
            {
                result.IsSuccess = checkResult.IsSuccess;
                result.Msg = checkResult.Msg;
                return result;
            }
            //return result;
        }
        #endregion
        #region 待審區 detail 修改
        public ActionResponse<string> itemPropertyDetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            result.IsSuccess = true;
            result.Msg = string.Empty;
            TWNewEgg.DB.TWSqlDBContext dbFront = new TWSqlDBContext();

            #region 檢查appsetting 狀態
            bool isNoException = true;
            string OpenViewEdidSwithch = string.Empty;
            //AppSettings on: 同步修改;off: 不允許同步修改 
            try
            {
                OpenViewEdidSwithch = System.Web.Configuration.WebConfigurationManager.AppSettings["SketchPropertyListEditAll"];
            }
            catch (Exception error)
            {
                isNoException = false;
                logger.Error("[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }
            //AppSettings 找不到或是沒有加上
            if (isNoException == false)
            {
                result.IsSuccess = false;
                result.Msg = "修改資料錯誤";
                return result;
            }
            #endregion
            var itemtempModel = dbFront.ItemTemp.Where(x => x.ID == itemSketch.Item.ID).FirstOrDefault();

            switch (itemtempModel.Status)
            {
                default:
                    break;
                case 0:
                case 1:
                    {
                        result = this.UpdateTempAndOffical(itemSketch);
                        break;    
                    }
                case 2:
                    {
                        // 舊品未通過 商品修改
                        break;
                    }
            }
            return null;
        }
        #endregion
        #region 同步修改正式賣場跟 Temp
        public ActionResponse<string> UpdateTempAndOffical(TWNewEgg.API.Models.ItemSketch itemSketch)
        {
            return null;
        }
        #endregion
        #region 待審區 detail 修改
        public ActionResponse<string> ItemPropertyOpenViewEdit(TWNewEgg.API.Models.ItemSketch itemSketch)
        {
            string OpenViewEdidSwithch = string.Empty;
            ActionResponse<string> result = new ActionResponse<string>();
            bool isNoException = true;
            //AppSettings on: 同步修改;off: 不允許同步修改 
            try
            {
                OpenViewEdidSwithch = System.Web.Configuration.WebConfigurationManager.AppSettings["SketchPropertyListEditAll"];
            }
            catch (Exception error)
            {
                isNoException = false;
                logger.Error("[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }
            //AppSettings 找不到或是沒有加上
            if (isNoException == false)
            {
                result.IsSuccess = false;
                result.Msg = "修改資料錯誤";
                return result;
            }
            if (OpenViewEdidSwithch == "on")
            {
                var editResult = this.groupSynchronizeEdit(itemSketch);
                if (editResult.IsSuccess == false)
                {
                    result.IsSuccess = false;
                    result.Msg = editResult.Msg;
                }
                else
                {
                    result.IsSuccess = true;
                    result.Msg = editResult.Msg;
                }
                return result;
            }
            else
            {
                return result;
            }
            
        }
        #endregion
        #region 待審區 detail 修改 同步群組修改
        #region 20150814
        public ActionResponse<string> groupSynchronizeEdit(TWNewEgg.API.Models.ItemSketch itemSketch)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
            TWNewEgg.API.Service.PropertyService propertyService = new PropertyService();
            
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
                                                                                         select new TWNewEgg.API.Models.ItemTempJoinProductTemp
                                                                                     {
                                                                                         itemtemp_id = i.ID,
                                                                                         item_id = i.ItemID,
                                                                                         productTemp_id = p.ID,
                                                                                         product_id = p.ProductID
                                                                                     }).ToList();
            ActionResponse<string> editResult = this.PropertyGroupEdit(itemSketch, itemTempJoinProductTemp);
            if (editResult.IsSuccess == true)
            {
                result.IsSuccess = true;
                result.Msg = editResult.Msg;
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = editResult.Msg;
            }
            return result;
        }
        public ActionResponse<string> PropertyGroupEdit(TWNewEgg.API.Models.ItemSketch itemSketch, List<TWNewEgg.API.Models.ItemTempJoinProductTemp> _itemTempJoinProductTemp)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.API.Service.PropertyService propertyService = new PropertyService();
            TWSqlDBContext dbFront = new TWSqlDBContext();
            //dbFront.ProductTemp.updae
            var PropertyGroupEditResult = propertyService.PropertyOpenViewEdit(itemSketch, _itemTempJoinProductTemp);
            if (PropertyGroupEditResult.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = PropertyGroupEditResult.Msg;
                return result;
            }
            else
            {
                result.IsSuccess = true;
                result.Msg = PropertyGroupEditResult.Msg;
                return result;
            }
        }
        #endregion
        public ActionResponse<string> propertyDetailEdit(TWNewEgg.API.Models.ItemSketch itemSketch)
        {
            TWNewEgg.API.Service.PropertyService propertyService = new PropertyService();
            ActionResponse<string> result = new ActionResponse<string>();
            try
            {
                result = propertyService.propertyDetailEdit(itemSketch);
            }
            catch (Exception ex)
            {
                string innerExceptionMsg = string.Empty;
                if (ex.InnerException != null)
                {
                    innerExceptionMsg = ex.InnerException.Message;
                }
                result.IsSuccess = false;
                result.Msg = "修改資料錯誤";
                logger.Error("[Msg] = " + ex.Message + " [StackTrace]: " + ex.StackTrace + " ;[innerExceptionMsg]: " + innerExceptionMsg);
            }
            return result;
        }

        #endregion
        #region 預覽規格品待審區查詢
        public ActionResponse<List<TWNewEgg.API.Models.ItemGroupJoinitemsketchListData>> ItemSketchDetailPropertyListSearch(ItemSketchSearchCondition itemsketchListData, bool isSearch = true)
        {
            ActionResponse<List<TWNewEgg.API.Models.ItemGroupJoinitemsketchListData>> result = new ActionResponse<List<ItemGroupJoinitemsketchListData>>();
            List<TWNewEgg.API.Models.sketchPropertyExamine> _listSketchPropertyExamine = new List<Models.sketchPropertyExamine>();

            TWNewEgg.API.Service.TempService tempService = new TempService();
            DB.TWSqlDBContext dbFront = new TWSqlDBContext();
            TWNewEgg.API.Service.PropertyService propertyService = new PropertyService();
            int sellerid = itemsketchListData.SellerID;
            //呼叫查詢的 service action
            var searchPropertyResult = propertyService.PropertyServiceList(itemsketchListData);
            List<TWNewEgg.API.Models.ItemGroupJoinitemsketchListData> bindResult = new List<TWNewEgg.API.Models.ItemGroupJoinitemsketchListData>();
            if (searchPropertyResult.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = searchPropertyResult.Msg;
                return result;
            }
            //利用 sellerid JOIN 查詢相關的資料, 利用 join 回來的屬性資料
            IEnumerable<TWNewEgg.DB.TWSQLDB.Models.ItemGroupDetailProperty> ItemGroupDetailProperty_Asquery = dbFront.ItemGroupDetailProperty.AsEnumerable();
            IEnumerable<TWNewEgg.DB.TWSQLDB.Models.ItemPropertyValue> ItemPropertyValuey_Asquery = dbFront.ItemPropertyValue.AsEnumerable();
            var joinItemGroupDetailProperty_ItemPropertyValue = (from p in ItemGroupDetailProperty_Asquery
                                                                 join q in ItemPropertyValuey_Asquery
                                                                     on p.GroupValueID equals q.ID
                                                                 where p.SellerID == itemsketchListData.SellerID
                                                                 select new TWNewEgg.API.Models.ItemGroupJoinitemsketchListData
                                                                 {
                                                                     SellerId = p.SellerID,
                                                                     ItemTempId = p.ItemTempID,
                                                                     groupID = p.GroupID,
                                                                     MasterPropertyID = p.MasterPropertyID,
                                                                     PropertyID = int.Parse(p.PropertyID.ToString()),
                                                                     GroupValueID = p.GroupValueID,
                                                                     ValueID = p.ValueID,
                                                                     ValueName = p.ValueName,
                                                                     definitions = p.InputValue,
                                                                     propertyValue = q.PropertyValue
                                                                 }).AsQueryable();
            try
            {
                bindResult = joinItemGroupDetailProperty_ItemPropertyValue.Where(p => p.groupID == searchPropertyResult.Body[0].group_id).ToList();
                result.IsSuccess = true;
            }
            catch (Exception error)
            {
                logger.Error("[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
                result.IsSuccess = false;
                result.Msg = "待審預覽資料處理錯誤";
            }

            if (result.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = "待審預覽查詢失敗，請稍後再試";
                return result;
            }
            else
            {
                result.IsSuccess = true;
                result.Body = bindResult;
                result.Msg = string.Empty;
                logger.Info("待審預覽查詢群組成功");
                return result;
            }

        }
        #endregion

        public ActionResponse<List<TWNewEgg.API.Models.ItemSketch>> autoMapBackToItemSketch(List<TWNewEgg.API.Models.sketchPropertyExamine> _listSketchProEx)
        {
            ActionResponse<List<TWNewEgg.API.Models.ItemSketch>> result = new ActionResponse<List<ItemSketch>>();
            result.Body = new List<ItemSketch>();
            try
            {
                AutoMapper.Mapper.CreateMap<sketchPropertyExamine, DB.TWSQLDB.Models.ItemSketch>();
                AutoMapper.Mapper.Map(_listSketchProEx, result.Body);
                result.IsSuccess = true;

            }
            catch (Exception error)
            {
                logger.Error("[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[ExceptionInnerMessage]: " + this.ExceptionInnerMessage(error));
                result.IsSuccess = false;
            }
            return result;
        }

        #region 待審區匯出 Excel
        public ActionResponse<string> excelSearchProperty(ItemSketchSearchCondition itemsketchListData)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.API.Service.PropertyService propertyService = new PropertyService();
            DB.TWSqlDBContext dbFront = new TWSqlDBContext();
            var searchPropertyResult = propertyService.PropertyServiceList(itemsketchListData);
            int sellerid = itemsketchListData.SellerID;
            if (searchPropertyResult.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = searchPropertyResult.Msg;
                return result;
            }
            var joindata = (from p in dbFront.ItemGroup
                            join q in dbFront.ItemGroupDetailProperty on p.SellerID equals q.SellerID
                            join r in dbFront.Item on q.ItemID equals r.ID
                            join s in dbFront.Product on r.ProductID equals s.ID
                            join x in dbFront.ItemPropertyValue on q.GroupValueID equals x.ID
                            where p.ID == q.GroupID //&& q.SellerID == itemsketchListData.SellerID
                            select new TWNewEgg.API.Models.ItemGroupJoinitemsketchListData
                            {
                                SellerId = p.SellerID,
                                itemid = q.ItemID.HasValue ? q.ItemID : 0,
                                ItemTempId = q.ItemTempID,
                                Cost = s.Cost,
                                PriceCash = r.PriceCash,
                                ProductName = r.Name,//productName 不維護
                                ValueName = q.ValueName,
                                GroupValueID = q.GroupValueID,
                                MasterPropertyID = p.MasterPropertyID,
                                PropertyID = q.PropertyID.HasValue ? q.PropertyID.Value : 0,
                                groupID = p.ID,
                                ValueID = q.ValueID,
                                definitions = string.IsNullOrEmpty(q.InputValue) == true ? x.PropertyValue : q.InputValue
                            }).AsQueryable();

            //List<int> hasItemid = new List<int>();
            IEnumerable<int> hasItemid = searchPropertyResult.Body.Where(p => p.Item.ItemID != null).Select(p => p.Item.ItemID.Value).AsQueryable();
            var hasItemidlist = hasItemid.ToList();
            joindata = joindata.Where(p => hasItemid.Contains(p.itemid.Value)).AsQueryable();

            var ExcelModel = this.searchModelToExcelModel(searchPropertyResult.Body, joindata);
            if (ExcelModel.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = "產生 Excel 失敗";
                return result;
            }
            var PropertyListExcelCreateResult = this.PropertyListExcelCreate(ExcelModel.Body);
            if (PropertyListExcelCreateResult.IsSuccess == true)
            {
                result.IsSuccess = true;
                result.Msg = PropertyListExcelCreateResult.Msg;
                result.Body = PropertyListExcelCreateResult.Body;
                return result;
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = PropertyListExcelCreateResult.Msg;
                return result;
            }
        }

        #endregion
        #region 把搜索出來的 Model 轉換成要匯出 Excel 的 model
        public ActionResponse<List<TWNewEgg.API.Models.PropertyExcelModel>> searchModelToExcelModel(List<Models.sketchPropertyExamine> itemsketch, IEnumerable<TWNewEgg.API.Models.ItemGroupJoinitemsketchListData> igjiskListModel)
        {
            ActionResponse<List<TWNewEgg.API.Models.PropertyExcelModel>> result = new ActionResponse<List<PropertyExcelModel>>();
            List<TWNewEgg.API.Models.PropertyExcelModel> propertyExcelModelList = new List<PropertyExcelModel>();
            itemsketch = itemsketch.Where(p => p.Item.ItemID != null).ToList();
            DB.TWSqlDBContext dbFront = new TWSqlDBContext();
            try
            {
                var igjiskListModel_list = igjiskListModel.ToList();
                foreach (var item in itemsketch)
                {
                    TWNewEgg.API.Models.PropertyExcelModel propertyExcelModel = new PropertyExcelModel();
                    //從 Join 裡的 Model 抓取對應的資料
                    var Price_Cost_Name = igjiskListModel_list.Where(p => p.itemid == item.Item.ItemID).FirstOrDefault();
                    if (Price_Cost_Name == null)
                    {
                        result.IsSuccess = false;
                        result.Msg = "組合 Excel 錯誤";
                        logger.Error("[Message]: 組合 Excel 錯誤, 從 Join 的資料中找不到對應的 itemid 資料");
                        break;
                    }
                    //第一層類別
                    propertyExcelModel.categoryID_Layer0 = item.ItemCategory.MainCategoryName_Layer0;
                    //第二層類別
                    propertyExcelModel.categoryID_Layer1 = item.ItemCategory.MainCategoryName_Layer1;
                    //第三層類別
                    propertyExcelModel.categoryID_Layer2 = item.ItemCategory.MainCategoryName_Layer2;
                    //第一跨分類次類別
                    propertyExcelModel.SubCategoryLayer1 = string.IsNullOrEmpty(item.ItemCategory.SubCategoryID_1_Layer1_Name) ? null : item.ItemCategory.SubCategoryID_1_Layer1_Name + "(" + item.ItemCategory.SubCategoryID_1_Layer1 + ")";
                    //第一跨分類子類別
                    propertyExcelModel.ItemCategoryLayer1 = string.IsNullOrEmpty(item.ItemCategory.SubCategoryID_1_Layer2_Name) ? null : item.ItemCategory.SubCategoryID_1_Layer2_Name + "(" + item.ItemCategory.SubCategoryID_1_Layer2 + ")";
                    //第二跨分類次類別
                    propertyExcelModel.SubcategoryLayer2 = string.IsNullOrEmpty(item.ItemCategory.SubCategoryID_2_Layer1_Name) ? null : item.ItemCategory.SubCategoryID_2_Layer1_Name + "(" + item.ItemCategory.SubCategoryID_2_Layer1 + ")";
                    //第二跨分類子類別
                    propertyExcelModel.ItemCategoryLayer2 = string.IsNullOrEmpty(item.ItemCategory.SubCategoryID_2_Layer2_Name) ? null : item.ItemCategory.SubCategoryID_2_Layer2_Name + "(" + item.ItemCategory.SubCategoryID_2_Layer2 + ")";
                    //新蛋賣場編號
                    propertyExcelModel.ItemID = item.Item.ItemID.HasValue ? item.Item.ItemID.Value : 0;
                    //商家商品編號
                    propertyExcelModel.ProductSellerProductID = item.Product.SellerProductID;
                    //成本
                    propertyExcelModel.Cost = Price_Cost_Name.Cost;
                    //商品名稱(品名), 抓的是 item name , product name 不維護
                    propertyExcelModel.WebsiteShortTitle = Price_Cost_Name.ProductName;
                    //市場建議售價
                    propertyExcelModel.MarketPrice = item.Item.MarketPrice;
                    //售價
                    propertyExcelModel.PriceCash = Price_Cost_Name.PriceCash.Value;
                    //可售數量
                    propertyExcelModel.CanSaleQty = item.ItemStock.CanSaleQty;/*item.ItemStock.InventorySafeQty;*/
                    //出貨方
                    propertyExcelModel.ShipType = item.Item.ShipType == "N" ? "Newegg" : "供應商";
                    //商品狀態
                    if (item.Item.ItemStatus == 0)
                    {
                        propertyExcelModel.GoodsStatus = "上架";
                    }
                    else if (item.Item.ItemStatus == 1)
                    {
                        propertyExcelModel.GoodsStatus = "下架";
                    }
                    else
                    {
                        propertyExcelModel.GoodsStatus = "狀態異常";
                    }
                    //var propertyTemp = igjiskListModel.Where(p => p.ItemTempId == item.Item.ID).AsQueryable().FirstOrDefault();
                    //顏色
                    propertyExcelModel.color = string.IsNullOrEmpty(Price_Cost_Name.definitions) == true ? Price_Cost_Name.propertyValue : Price_Cost_Name.definitions;
                    //尺寸
                    propertyExcelModel.size = string.IsNullOrEmpty(Price_Cost_Name.ValueName) == true ? "" : Price_Cost_Name.ValueName;
                    //propertyExcelModel.size = igjiskListModel.Where(p => p.ItemTempId == item.Item.ID).Select(p => p.ValueName).FirstOrDefault() == null ? "" : igjiskListModel.Where(p => p.ItemTempId == item.Item.ID).Select(p => p.ValueName).FirstOrDefault();
                    propertyExcelModelList.Add(propertyExcelModel);
                }
                result.IsSuccess = true;
                result.Body = propertyExcelModelList;
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                logger.Error("[Message]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }
            return result;
        }
        #endregion
        public ActionResponse<string> PropertyListExcelCreate(List<TWNewEgg.API.Models.PropertyExcelModel> propertyExcelModel)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            try
            {
                var ExcelResult = DataToExcel.Export.ListToExcel(propertyExcelModel, "屬性商品清單", "WorkSheet", 1);
                if (ExcelResult.IndexOf("Success") >= 0)
                {
                    result.Msg = ExcelResult.Substring(8).Trim();
                    result.Body = string.Format("{0}{1}_{2}.xls", System.Configuration.ConfigurationSettings.AppSettings["ReturnExcel"], "屬性商品清單", ExcelResult.Substring(8).Trim());
                    result.IsSuccess = true;
                }
                else
                {
                    result.Msg = ExcelResult;
                    result.IsSuccess = false;
                }
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "產生 Excel 失敗";
                logger.Error("[Message]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }
            return result;
        }
        #endregion

        #region 抓取 ExceptionInnerMessage
        public string ExceptionInnerMessage(Exception error)
        {
            string resultMsg = string.Empty;
            try
            {
                resultMsg = error.InnerException != null ? error.InnerException.Message : "";
            }
            catch (Exception ex)
            {
                logger.Error("[Message]: " + ex.Message + " ;[StackTrace]: " + ex.StackTrace + ", 抓取 ExceptionInnerMessage 錯誤");
            }
            return resultMsg;
        }
        #endregion

    }
}
