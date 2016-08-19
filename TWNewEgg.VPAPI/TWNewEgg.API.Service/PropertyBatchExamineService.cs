using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB;
using TWNewEgg.API.Models;
using System.Security.Cryptography;
using System.Transactions;


namespace TWNewEgg.API.Service
{
    public class PropertyBatchExamineService
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        TWNewEgg.BackendService.Interface.ICategoryService CategoryService = new TWNewEgg.BackendService.Service.CategoryService();
        


        public ActionResponse<string> propertyExamine(List<TWNewEgg.API.Models.BatchExamineModel> batchExamineModel, string userEmail, string passWord)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWSellerPortalDBContext spdb = new TWSellerPortalDBContext();
            Dictionary<int, TWNewEgg.API.Models.BatchExamineModel> dic_model = new Dictionary<int, BatchExamineModel>();//存放要送審的資料
            Dictionary<int, string> dic_msg = new Dictionary<int, string>();//存放處理訊息
            string returnMessage = string.Empty;
            int _intTempNumber = 1;
            #region 檢查使用批次上傳的使用者是否為合法的使用者
            //檢查使用批次上傳的使用者是否為合法的使用者
            ActionResponse<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_User> seller_user_check = new ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_User>();
            seller_user_check = this.checkUserExist(userEmail, passWord);
            //使用者不合法
            if (seller_user_check.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = seller_user_check.Msg;
                //recordErrorMsg.Add(seller_user_check.Msg);
                result.Body = seller_user_check.Msg;
                return result;
            }
            #endregion 
            #region 取得 sellerid userid Account
            int sellerid = seller_user_check.Body.SellerID.Value;
            int userid = seller_user_check.Body.UserID;
            string AccountTypeCode = spdb.Seller_BasicInfo.Where(p => p.SellerID == sellerid).Select(p => p.AccountTypeCode).FirstOrDefault();
            #endregion
            #region 檢查 sellerid or userid 是否為空的或是 0
            if (sellerid == 0 || sellerid == null)
            {
                result.IsSuccess = false;
                result.Msg = "此使用者為錯誤使用者";
                result.Body = "此使用者為錯誤使用者";
                logger.Error("[Message]: Email: " + userEmail + ", 此使用者為錯誤使用者, sellerid = 0 or sellerid = null");
                return result;
            }
            if (userid == 0 || userid == null)
            {
                result.IsSuccess = false;
                result.Msg = "此使用者為錯誤使用者";
                result.Body = "此使用者為錯誤使用者";
                logger.Error("[Message]: Email: " + userEmail + ", 此使用者為錯誤使用者, userid = 0 or userid = null");
                return result;
            }
            #endregion
            #region 對每一個要送審的資料做檢查
            List<int> product_ManufactureID = batchExamineModel.Select(q => q.Product.ManufactureID.Value).ToList();
            //先從DB 取出對應製造商的 product_ManufactureID
            var manufactureInfoExist = spdb.Seller_ManufactureInfo.Where(p => product_ManufactureID.Contains(p.SN)).AsQueryable();
            var List_manufactureInfoExist = manufactureInfoExist.ToList();

            foreach (var itemcheck in batchExamineModel)
            {
                var dataCheckResult = this.propertyExamineCheck(itemcheck, List_manufactureInfoExist);
                if (dataCheckResult.IsSuccess == false)
                {
                    //若資料檢查有錯誤，則把第幾筆資料跟錯誤訊息記錄起來
                    dic_msg.Add(_intTempNumber, dataCheckResult.Msg);
                }
                else
                {
                    //把沒有錯的資料存進要送審的 Dictionary
                    #region y->Y; n->N; otherwise N
                    itemcheck.Item.IsNew = this.ConvertYN(itemcheck.Item.IsNew);
                    itemcheck.Item.ShipType = this.ConvertYN(itemcheck.Item.ShipType);
                    itemcheck.Product.Is18 = this.ConvertYN(itemcheck.Product.Is18);
                    itemcheck.Product.IsChokingDanger = this.ConvertYN(itemcheck.Product.IsChokingDanger);
                    itemcheck.Product.IsShipDanger = this.ConvertYN(itemcheck.Product.IsShipDanger);
                    #endregion y->Y; n->N; otherwise N
                    dic_model.Add(_intTempNumber, itemcheck);
                }
                //紀錄第幾筆資料
                _intTempNumber++;
            }
            #endregion
            //有正確的資料再送審
            if (dic_model.Count != 0)
            {
                //開始送審
                ActionResponse<Dictionary<int, string>> ToExamineResult = this.Examine_Rail(dic_model, sellerid, userid, AccountTypeCode);
                //把所有的送審回傳訊息回存到 Dictionary
                foreach (var AddTodic_msg in ToExamineResult.Body)
                {
                    dic_msg.Add(AddTodic_msg.Key, AddTodic_msg.Value);
                }
                //對 Dictionary 根據 KEY 做排序
                dic_msg = dic_msg.OrderBy(key => key.Key).ToDictionary(keyvalue => keyvalue.Key, keyValue => keyValue.Value);
                //組合要送回前端的潠審資料所有訊息
                foreach (var itemMsg in dic_msg)
                {
                    returnMessage += "第 " + itemMsg.Key + " 筆資料: " + itemMsg.Value + ";";
                }
            }
            else
            {
                //送審資料全錯誤, 所以沒必要送審
                dic_msg = dic_msg.OrderBy(key => key.Key).ToDictionary(keyvalue => keyvalue.Key, keyValue => keyValue.Value);
                foreach (var itemMsg in dic_msg)
                {
                    returnMessage += "第 " + itemMsg.Key + " 筆資料: " + itemMsg.Value;
                }
            }
            result.Msg = returnMessage;
            result.Body = returnMessage;
            return result;
        }
        #region 送審
        public ActionResponse<Dictionary<int, string>> Examine_Rail(Dictionary<int, TWNewEgg.API.Models.BatchExamineModel> Dic_ExaminepropertyModel, int sellerid, int userid, string AccountTypeCode)
        {
            ActionResponse<Dictionary<int, string>> result = new ActionResponse<Dictionary<int, string>>();
            result.Body = new Dictionary<int, string>();

            foreach (var dic_item in Dic_ExaminepropertyModel)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //二維屬性
                    if (dic_item.Value.colorsizeProperty.Count != 0 && dic_item.Value.oneArrayProperty.Count == 0)
                    {
                        #region 二維屬性開始送審
                        ActionResponse<string> TwoDimesionResult = new ActionResponse<string>();
                        try
                        {
                            //二維屬性開始送審
                            TwoDimesionResult = this.TwodimensionExamine(dic_item.Value, sellerid, userid, AccountTypeCode);
                            if (TwoDimesionResult.IsSuccess == true)
                            {
                                result.Body.Add(dic_item.Key, TwoDimesionResult.Msg);
                                scope.Complete();
                            }
                            else
                            {
                                result.Body.Add(dic_item.Key, TwoDimesionResult.Msg);
                                scope.Dispose();
                            }
                        }
                        catch (Exception error)
                        {
                            result.Body.Add(dic_item.Key, "系統錯誤");
                            scope.Dispose();
                            logger.Error("[Message]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[ExceptionMessaage]: " + this.ExceptionMessaage(error));
                        }
                        #endregion
                    }
                    else if (dic_item.Value.colorsizeProperty.Count == 0 && dic_item.Value.oneArrayProperty.Count != 0)
                    {
                        #region 一維屬性開始送審
                        //一維屬性
                        ActionResponse<string> OneDimension_Result = this.OneDimension(dic_item.Value, sellerid, userid, AccountTypeCode);
                        try
                        {
                            if (OneDimension_Result.IsSuccess == false)
                            {
                                result.Body.Add(dic_item.Key, OneDimension_Result.Msg);
                                scope.Dispose();
                            }
                            else
                            {
                                result.Body.Add(dic_item.Key, OneDimension_Result.Msg);
                                scope.Complete();
                            }
                        }
                        catch (Exception error)
                        {
                            result.Body.Add(dic_item.Key, "系統錯誤");
                            scope.Dispose();
                            logger.Error("[Message]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[ExceptionMessaage]: " + this.ExceptionMessaage(error));
                        }
                        #endregion
                    }
                    else
                    {
                        #region 不是二維也不是一維
                        //不是二維也不是一維
                        result.Body.Add(dic_item.Key, "此筆資料不為一維屬性商品也不為二維屬性商品");
                        scope.Dispose();
                        #endregion
                    }
                }

            }
            return result;
        }
        #region 一維送審
        public ActionResponse<string> OneDimension(TWNewEgg.API.Models.BatchExamineModel batchExamineModel, int sellerid, int userid, string AccountTypeCode)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            #region join propertyValue 和 propertyName
            ActionResponse<IEnumerable<TWNewEgg.API.Models.propertyJoinModel>> JoinProperty_Value_Name_Result = new ActionResponse<IEnumerable<propertyJoinModel>>();
            //join propertyValue 和 propertyName
            JoinProperty_Value_Name_Result = this.propertyName_Value_Join(batchExamineModel, TWNewEgg.API.Models.OneOrTwoDimension.OneDimension);
            //Join 有錯誤
            if (JoinProperty_Value_Name_Result.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = JoinProperty_Value_Name_Result.Msg;
                //result.Body = "資料錯誤";
                return result;
            }
            var JoinProperty_Value_Name_Result_List = JoinProperty_Value_Name_Result.Body.ToList();
            ActionResponse<string> ExamineResult = new ActionResponse<string>();
            //一維屬性送審開始
            ExamineResult = this.insertDataToOneDimesionTable(batchExamineModel, JoinProperty_Value_Name_Result_List, sellerid, userid, AccountTypeCode);
            if (ExamineResult.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = ExamineResult.Msg;
            }
            else
            {
                result.IsSuccess = true;
                result.Msg = ExamineResult.Msg;
            }
            #endregion
            return result;
        }
        #endregion
        #region 二維送審
        public ActionResponse<string> TwodimensionExamine(TWNewEgg.API.Models.BatchExamineModel batchExamineModel, int sellerid, int userid, string AccountTypeCode)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            //result.Body = new List<string>();
            #region join propertyValue 和 propertyName
            ActionResponse<IEnumerable<TWNewEgg.API.Models.propertyJoinModel>> JoinProperty_Value_Name_Result = new ActionResponse<IEnumerable<propertyJoinModel>>();
            //join propertyValue 和 propertyName
            JoinProperty_Value_Name_Result = this.propertyName_Value_Join(batchExamineModel, TWNewEgg.API.Models.OneOrTwoDimension.TwoDimension);
            //Join 有錯誤
            if (JoinProperty_Value_Name_Result.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                result.Body = "資料錯誤";
                return result;
            }
            var JoinProperty_Value_Name_Result_List = JoinProperty_Value_Name_Result.Body.ToList();
            #endregion
            ActionResponse<string> ExamineResult = new ActionResponse<string>();
            ExamineResult = this.insertDataToTwoDimesionTable(batchExamineModel, /*JoinProperty_Value_Name_Result.Body*/ JoinProperty_Value_Name_Result_List, sellerid, userid, AccountTypeCode);
            if (ExamineResult.IsSuccess == true)
            {
                result.IsSuccess = true;
                result.Msg = ExamineResult.Msg;
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = ExamineResult.Msg;
            }
            return result;
        }
        #endregion
        #region 開始寫入資料到資料庫(一維)
        public ActionResponse<string> insertDataToOneDimesionTable(TWNewEgg.API.Models.BatchExamineModel batchExamineModel, List<TWNewEgg.API.Models.propertyJoinModel> joinPropertyNameValueModel, int sellerid, int userid, string AccountTypeCode)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            #region 把送審資料寫入 ItemGroup table
            ActionResponse<int> ItemGroup = this.ItemGroupOneDimesion(batchExamineModel, joinPropertyNameValueModel, sellerid, userid);
            if (ItemGroup.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                return result;
            }
            int groupid = ItemGroup.Body;
            #endregion
            #region 把送審資料寫到 ItemGroupProperty
            ActionResponse<string> ItemGroupProperty = this.ItemGroupPropertyOneDimesion(batchExamineModel, joinPropertyNameValueModel, sellerid, userid, groupid);
            if (ItemGroupProperty.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                return result;
            }
            #endregion
            ActionResponse<string> basicAndItemGroupDetailPropertyInsert = this.basicAndItemGroupDetailPropertyOneDimesionTable(batchExamineModel, joinPropertyNameValueModel, sellerid, userid, groupid, AccountTypeCode);
            if (basicAndItemGroupDetailPropertyInsert.IsSuccess == true)
            {
                result.IsSuccess = true;
                result.Msg = basicAndItemGroupDetailPropertyInsert.Msg;
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = basicAndItemGroupDetailPropertyInsert.Msg;
            }
            return result;
        }
        #endregion
        #region 開始寫入資料到資料庫(二維)
        public ActionResponse<string> insertDataToTwoDimesionTable(TWNewEgg.API.Models.BatchExamineModel batchExamineModel, List<TWNewEgg.API.Models.propertyJoinModel> joinPropertyNameValueModel, int sellerid, int userid, string AccountTypeCode)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            #region 把送審資料寫入 ItemGroup table
            ActionResponse<int> ItemGroup_Result = this.ItemGroupTwoDimesion(batchExamineModel, joinPropertyNameValueModel, sellerid, userid);
            //把資料寫入 ItemGroup 失敗
            if (ItemGroup_Result.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                return result;
            }
            int groupid = ItemGroup_Result.Body;
            #endregion
            #region 把送審資料寫到 ItemGroupProperty
            ActionResponse<string> ItemGroupProperty_Result = this.ItemGroupPropertyTwoDimesion(batchExamineModel, joinPropertyNameValueModel, sellerid, userid, groupid);
            if (ItemGroupProperty_Result.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                return result;
            }
            #endregion 
            ActionResponse<string> basicAndItemGroupDetailPropertyInsert = this.basicAndItemGroupDetailPropertyTwoDimesionTable(batchExamineModel, joinPropertyNameValueModel, sellerid, userid, groupid, AccountTypeCode);
            if (basicAndItemGroupDetailPropertyInsert.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = basicAndItemGroupDetailPropertyInsert.Msg;
            }
            else
            {
                result.IsSuccess = true;
                result.Msg = basicAndItemGroupDetailPropertyInsert.Msg;
            }
            return result;
        }
        #endregion
        #region 把資料 insert into table
        #region 把資料寫到 ItemGroup(一維屬性)
        public ActionResponse<int> ItemGroupOneDimesion(TWNewEgg.API.Models.BatchExamineModel batchExamineModel, List<TWNewEgg.API.Models.propertyJoinModel> joinPropertyNameValueModel, int sellerid, int userid)
        {
            ActionResponse<int> result = new ActionResponse<int>();
            TWNewEgg.DB.TWSqlDBContext dbFront = new TWSqlDBContext();
            int amount = batchExamineModel.oneArrayProperty.Count;
            int masterTemp = batchExamineModel.oneArrayProperty[0].firstGroupVauleId;
            int master = joinPropertyNameValueModel.Where(p => p.ItemPropertyValue_ID == masterTemp).Select(p => p.ItemPropertyName_ID).FirstOrDefault();
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
                logger.Error("[Msg] " + error.Message + "; [StackTrace]: " + error.StackTrace + " ;[ExceptionMessaage]: " + this.ExceptionMessaage(error));
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
            }
            return result;
        }
        #endregion
        #region 把資料寫到 ItemGroup(二維屬性)
        public ActionResponse<int> ItemGroupTwoDimesion(TWNewEgg.API.Models.BatchExamineModel batchExamineModel, List<TWNewEgg.API.Models.propertyJoinModel> joinPropertyNameValueModel, int sellerid, int userid)
        {
            ActionResponse<int> result = new ActionResponse<int>();
            TWNewEgg.DB.TWSqlDBContext dbFront = new TWSqlDBContext();
            int amount = batchExamineModel.colorsizeProperty.Count;
            int masterTemp = batchExamineModel.colorsizeProperty[0].colorValueId;
            int master = joinPropertyNameValueModel.Where(p => p.ItemPropertyValue_ID == masterTemp).Select(p => p.ItemPropertyName_ID).FirstOrDefault();
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
                logger.Error("[Msg] " + error.Message + "; [StackTrace]: " + error.StackTrace + " ;[ExceptionMessaage]: " + this.ExceptionMessaage(error));
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
            }
            return result;
        }
        #endregion
        #region 把資料寫到 ItemGroupProperty(一維屬性)
        public ActionResponse<string> ItemGroupPropertyOneDimesion(TWNewEgg.API.Models.BatchExamineModel batchExamineModel, List<TWNewEgg.API.Models.propertyJoinModel> joinPropertyNameValueModel, int sellerid, int userid, int groupid)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWSqlDBContext dbFront = new TWSqlDBContext();
            #region 因為是一維屬性, 所以 order 只會等於 1
            TWNewEgg.DB.TWSQLDB.Models.ItemGroupProperty _itemgroupPropertyOrder1 = new DB.TWSQLDB.Models.ItemGroupProperty();
            int groupValueidTemp = batchExamineModel.oneArrayProperty[0].firstGroupVauleId;
            int groupValueid = joinPropertyNameValueModel.Where(p => p.ItemPropertyValue_ID == groupValueidTemp).Select(p => p.ItemPropertyName_ID).FirstOrDefault();
            _itemgroupPropertyOrder1.GroupID = groupid;
            _itemgroupPropertyOrder1.PropertyID = groupValueid;
            _itemgroupPropertyOrder1.PropertyName = joinPropertyNameValueModel.Where(p => p.ItemPropertyName_ID == groupValueid).Select(p => p.ItemPropertyName_propertyName).FirstOrDefault();
            _itemgroupPropertyOrder1.Order = 1;
            _itemgroupPropertyOrder1.CreateDate = DateTime.Now;
            _itemgroupPropertyOrder1.InUser = userid;
            _itemgroupPropertyOrder1.UpdateDate = DateTime.Now;
            _itemgroupPropertyOrder1.UpdateUser = userid;
            dbFront.ItemGroupProperty.Add(_itemgroupPropertyOrder1);
            try
            {
                dbFront.SaveChanges();
                result.IsSuccess = true;
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "資料處理錯誤";
                logger.Error("[Message]: " + error.Message + "; [StackTrace]: " + error.StackTrace + " ;[ExceptionMessaage]: " + this.ExceptionMessaage(error));
            }
            #endregion
            return result;
        }
        #endregion
        #region 把資料寫到 ItemGroupProperty(二維屬性)
        public ActionResponse<string> ItemGroupPropertyTwoDimesion(TWNewEgg.API.Models.BatchExamineModel batchExamineModel, List<TWNewEgg.API.Models.propertyJoinModel> joinPropertyNameValueModel, int sellerid, int userid, int groupid)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWSqlDBContext dbFront = new TWSqlDBContext();
            #region 第一順位
            //order = 1
            TWNewEgg.DB.TWSQLDB.Models.ItemGroupProperty _itemgroupPropertyOrder1 = new DB.TWSQLDB.Models.ItemGroupProperty();
            int groupValueidTemp = batchExamineModel.colorsizeProperty[0].colorValueId;
            int groupValueid = joinPropertyNameValueModel.Where(p => p.ItemPropertyValue_ID == groupValueidTemp).Select(p => p.ItemPropertyName_ID).FirstOrDefault();
            _itemgroupPropertyOrder1.GroupID = groupid;
            _itemgroupPropertyOrder1.PropertyID = groupValueid;
            _itemgroupPropertyOrder1.PropertyName = joinPropertyNameValueModel.Where(p => p.ItemPropertyName_ID == groupValueid).Select(p => p.ItemPropertyName_propertyName).FirstOrDefault();
            _itemgroupPropertyOrder1.Order = 1;
            _itemgroupPropertyOrder1.CreateDate = DateTime.Now;
            _itemgroupPropertyOrder1.InUser = userid;
            _itemgroupPropertyOrder1.UpdateDate = DateTime.Now;
            _itemgroupPropertyOrder1.UpdateUser = userid;
            dbFront.ItemGroupProperty.Add(_itemgroupPropertyOrder1);
            #endregion
            #region 第二順位
            TWNewEgg.DB.TWSQLDB.Models.ItemGroupProperty _itemgroupPropertyOrder2 = new DB.TWSQLDB.Models.ItemGroupProperty();
            int valueidTemp = batchExamineModel.colorsizeProperty[0].listProperty[0].proValueId;
            int valueid = joinPropertyNameValueModel.Where(p => p.ItemPropertyValue_ID == valueidTemp).Select(p => p.ItemPropertyName_ID).FirstOrDefault();
            _itemgroupPropertyOrder2.GroupID = groupid;
            _itemgroupPropertyOrder2.PropertyID = valueid;
            _itemgroupPropertyOrder2.PropertyName = joinPropertyNameValueModel.Where(p => p.ItemPropertyName_ID == valueid).Select(p => p.ItemPropertyName_propertyName).FirstOrDefault();
            _itemgroupPropertyOrder2.Order = 2;
            _itemgroupPropertyOrder2.CreateDate = DateTime.Now;
            _itemgroupPropertyOrder2.InUser = userid;
            _itemgroupPropertyOrder2.UpdateDate = DateTime.Now;
            _itemgroupPropertyOrder2.UpdateUser = userid;
            dbFront.ItemGroupProperty.Add(_itemgroupPropertyOrder2);
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
                logger.Error("[Message]: " + error.Message + "; [StackTrace]: " + error.StackTrace + " ;[ExceptionMessaage]: " + this.ExceptionMessaage(error));
            }
            return result;
        }
        #endregion
        #region 把資料寫入 productTemp
        public ActionResponse<int> productTemp(TWNewEgg.API.Models.BatchExamineModel batchExamineModelint, int sellerid, string AccountTypeCode, int userid)
        {
            ActionResponse<int> result = new ActionResponse<int>();
            TWSqlDBContext db_Front = new TWSqlDBContext();
            DateTime dateTimeMillisecond = this.NowTime();
            #region 開始寫入資料
            DB.TWSQLDB.Models.ProductTemp productTempAdd = new DB.TWSQLDB.Models.ProductTemp();
            productTempAdd.SellerProductID = batchExamineModelint.Product.SellerProductID;
            productTempAdd.Name = batchExamineModelint.Product.Name;
            productTempAdd.NameTW = batchExamineModelint.Product.Name;
            productTempAdd.Description = batchExamineModelint.Product.Description;
            productTempAdd.DescriptionTW = batchExamineModelint.Product.Description;
            productTempAdd.SPEC = ""; //SPEC已不使用;
            productTempAdd.ManufactureID = batchExamineModelint.Product.ManufactureID.GetValueOrDefault();
            productTempAdd.Model = batchExamineModelint.Product.Model;
            productTempAdd.BarCode = batchExamineModelint.Product.BarCode;
            productTempAdd.SellerID = sellerid;
            var delcType = this.GetDelvType(AccountTypeCode, batchExamineModelint.Item.ShipType.ToUpper());
            if (delcType.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = "找不到對應的 DelvType";
                logger.Error("sellerid is: " + sellerid + "; userid is: " + userid + "; " + delcType.Msg + ". 找不到對應的 DelvType");
            }
            else
            {
                productTempAdd.DelvType = delcType.Body;
            }
            if (batchExamineModelint.Product.PicPatch_Edit.Count == 0)
            {
                productTempAdd.PicStart = 0;
                productTempAdd.PicEnd = 0;
            }
            else
            {
                productTempAdd.PicStart = 1;
                productTempAdd.PicEnd = batchExamineModelint.Product.PicPatch_Edit.Count;
            }
            productTempAdd.Cost = batchExamineModelint.Product.Cost;
            productTempAdd.Status = 1;
            productTempAdd.InvoiceType = 0;//default value
            productTempAdd.SaleType = 0;//default value
            productTempAdd.Length = batchExamineModelint.Product.Length;
            productTempAdd.Width = batchExamineModelint.Product.Width;
            productTempAdd.Height = batchExamineModelint.Product.Height;
            productTempAdd.Weight = batchExamineModelint.Product.Weight;
            productTempAdd.CreateUser = userid.ToString();
            productTempAdd.CreateDate = dateTimeMillisecond;
            productTempAdd.Updated = 0;//default value
            productTempAdd.UpdateDate = dateTimeMillisecond;
            productTempAdd.UpdateUser = userid.ToString();
            productTempAdd.TradeTax = 0;
            productTempAdd.Tax = 0;//default value
            productTempAdd.Warranty = batchExamineModelint.Product.Warranty;
            productTempAdd.UPC = batchExamineModelint.Product.UPC;
            productTempAdd.Note = batchExamineModelint.Item.Note;
            productTempAdd.IsMarket = "Y";
            productTempAdd.Is18 = batchExamineModelint.Product.Is18;
            productTempAdd.IsShipDanger = batchExamineModelint.Product.IsShipDanger;
            productTempAdd.IsChokingDanger = batchExamineModelint.Product.IsChokingDanger;
            productTempAdd.MenufacturePartNum = "";//製造商商品編號 (沒有輸入欄位) 暫時給空的
            db_Front.ProductTemp.Add(productTempAdd);
            #endregion
            try
            {
                db_Front.SaveChanges();
                result.IsSuccess = true;
                result.Body = productTempAdd.ID;
                result.Msg = delcType.Body.ToString();//拿 Msg 存 delcType
            }
            catch (Exception error)
            {
                result.Msg = "資料處理錯誤";
                result.IsSuccess = false;
                logger.Error("sellerid is: " + sellerid + "; userid is: " + userid + ". errorMsg: " + error.Message + " [ErrorStackTrace]: " + error.StackTrace + " ;[ExceptionMessaage]: " + this.ExceptionMessaage(error));
            }
            return result;
        }
        #endregion
        #region  把資料寫入 itemTemp
        public ActionResponse<int> itemTemp(TWNewEgg.API.Models.BatchExamineModel batchExamineModel, int productTempid, int userid, int? DelvType, int sellerid)
        {
            ActionResponse<int> result = new ActionResponse<int>();
            TWNewEgg.DB.TWSqlDBContext dbFront = new TWSqlDBContext();
            DateTime dateTimeMillisecond = this.NowTime();
            #region 開始把資料寫入 itemtemp
            DB.TWSQLDB.Models.ItemTemp itemtempAdd = new DB.TWSQLDB.Models.ItemTemp();
            itemtempAdd.ProduttempID = productTempid;
            itemtempAdd.Name = batchExamineModel.Product.Name;
            itemtempAdd.Sdesc = batchExamineModel.Item.Sdesc;
            itemtempAdd.DescTW = batchExamineModel.Product.Description;
            itemtempAdd.SpecDetail = "";//defalut value
            itemtempAdd.Spechead = batchExamineModel.Item.Spechead;
            itemtempAdd.SaleType = 1;//defalut value
            itemtempAdd.PayType = 0;//defalut value
            itemtempAdd.Layout = 0;//defalut value
            itemtempAdd.DelvType = DelvType.GetValueOrDefault();
            itemtempAdd.DelvData = batchExamineModel.Item.DelvDate ?? "";
            itemtempAdd.ItemNumber = "";//defalut value
            itemtempAdd.CategoryID = batchExamineModel.ItemCategory.MainCategoryID_Layer2.GetValueOrDefault();
            itemtempAdd.Model = string.IsNullOrEmpty(batchExamineModel.Product.Model) == true ? "" : batchExamineModel.Product.Model;
            itemtempAdd.SellerID = sellerid;
            itemtempAdd.DateStart = batchExamineModel.Item.DateStart;
            itemtempAdd.DateEnd = batchExamineModel.Item.DateEnd;
            itemtempAdd.DateDel = batchExamineModel.Item.DateEnd.AddDays(1);
            itemtempAdd.Pricesgst = 0;//defalut value
            itemtempAdd.PriceCard = batchExamineModel.Item.PriceCash.GetValueOrDefault();
            itemtempAdd.PriceCash = batchExamineModel.Item.PriceCash.GetValueOrDefault();
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
            itemtempAdd.Qty = 0;// batchExamineModel.Item.CanSaleLimitQty.GetValueOrDefault();
            itemtempAdd.SafeQty = 0;
            itemtempAdd.QtyLimit = batchExamineModel.Item.QtyLimit.GetValueOrDefault();
            itemtempAdd.LimitRule = "";//defalut value
            itemtempAdd.QtyReg = 0;//defalut value
            itemtempAdd.PhotoName = "";//defalut value
            itemtempAdd.HtmlName = "";//defalut value
            itemtempAdd.Showorder = 0;//defalut value
            itemtempAdd.Class = 1;//defalut value
            itemtempAdd.Status = 1;//defalut value
            itemtempAdd.ManufactureID = batchExamineModel.Product.ManufactureID.GetValueOrDefault();
            itemtempAdd.StatusNote = "";//defalut value
            itemtempAdd.StatusDate = dateTimeMillisecond;
            itemtempAdd.Note = ""; //Note 不維護
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
            if (batchExamineModel.Product.PicPatch_Edit.Count == 0)
            {
                itemtempAdd.PicStart = 0;
                itemtempAdd.PicEnd = 0;
            }
            else
            {
                itemtempAdd.PicStart = 1;
                itemtempAdd.PicEnd = batchExamineModel.Product.PicPatch_Edit.Count;
            }
            itemtempAdd.MarketPrice = batchExamineModel.Item.MarketPrice;
            itemtempAdd.WareHouseID = 0;
            itemtempAdd.ShipType = batchExamineModel.Item.ShipType;
            itemtempAdd.Taxfee = 0;//defalut value
            itemtempAdd.ItemPackage = batchExamineModel.Item.ItemPackage;
            itemtempAdd.IsNew = batchExamineModel.Item.IsNew;
            if ((batchExamineModel.Item.PriceCash != null && batchExamineModel.Product.Cost != null) && (batchExamineModel.Item.PriceCash > 0 && batchExamineModel.Product.Cost >= 0))
            {
                decimal grossMargin = ((batchExamineModel.Item.PriceCash.Value - batchExamineModel.Product.Cost.Value) / batchExamineModel.Item.PriceCash.Value) * 100;
                itemtempAdd.GrossMargin = System.Math.Round(grossMargin, 2);
            }
            else
            {
                itemtempAdd.GrossMargin = 0;
            }
            itemtempAdd.SubmitMan = userid.ToString();
            itemtempAdd.SubmitDate = dateTimeMillisecond;//不給值會出現datetime2無法轉換成datetime錯誤
            itemtempAdd.ApproveDate = null;
            dbFront.ItemTemp.Add(itemtempAdd);      
            #endregion
            try
            {
                //必須先savechenge 才會有itemtempAdd id
                dbFront.SaveChanges();
                result.Body = itemtempAdd.ID;
                result.IsSuccess = true;
                result.Msg = "success";
                result.Code = (int)ResponseCode.Success;
            }
            catch (Exception error)
            {
                logger.Error("ErrorMessage: " + error.Message + " [ErrorStackTrace] " + error.StackTrace + " ;[ExceptionMessaage]: " + this.ExceptionMessaage(error));
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                result.Code = (int)ResponseCode.Error;
            }
            return result;
        }
        #endregion
        #region 把資料寫入 itemstocktemp(一維屬性)
        public ActionResponse<string> itemstocktempOneDimesion(oneArrayProperty _oneArrayProperty, int productTempId, int userid, int? InventorySafeQty)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.DB.TWSqlDBContext dbFront = new TWSqlDBContext();
            DateTime dateTimeMillisecond = this.NowTime();
            DB.TWSQLDB.Models.ItemStocktemp _itemStocktemp = new DB.TWSQLDB.Models.ItemStocktemp();
            _itemStocktemp.producttempID = productTempId;
            _itemStocktemp.Qty = _oneArrayProperty.Qty;
            _itemStocktemp.QtyReg = 0;
            _itemStocktemp.SafeQty = InventorySafeQty.GetValueOrDefault();
            _itemStocktemp.Fdbcklmt = 0;
            _itemStocktemp.CreateUser = userid.ToString();
            _itemStocktemp.Updated = 0;
            _itemStocktemp.CreateDate = dateTimeMillisecond;
            _itemStocktemp.UpdateUser = userid.ToString();
            _itemStocktemp.UpdateDate = dateTimeMillisecond;
            dbFront.ItemStocktemp.Add(_itemStocktemp);
            try
            {
                dbFront.SaveChanges();
                result.IsSuccess = true;
                result.Code = (int)ResponseCode.Success;
                result.Msg = "success";
            }
            catch (Exception error)
            {
                logger.Error("ErrorMsg:  " + error.Message + " [ErrorStackTrace] " + error.StackTrace + " ;[ExceptionMessaage]: " + this.ExceptionMessaage(error));
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                result.Code = (int)ResponseCode.Error;
            }
            return result;
        }
        #endregion
        #region 把資料寫入 itemstocktemp(二維屬性)
        public ActionResponse<string> itemstocktempTwoDimesion(propertyModel propertyModel, int productTempId, int userid, int? InventorySafeQty)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.DB.TWSqlDBContext dbFront = new TWSqlDBContext();
            DateTime dateTimeMillisecond = this.NowTime();
            #region 開始把資料寫入 itemstocktemp
            DB.TWSQLDB.Models.ItemStocktemp _itemStocktemp = new DB.TWSQLDB.Models.ItemStocktemp();
            _itemStocktemp.producttempID = productTempId;
            _itemStocktemp.Qty = propertyModel.proQty;
            _itemStocktemp.QtyReg = 0;
            _itemStocktemp.SafeQty = InventorySafeQty.GetValueOrDefault();
            _itemStocktemp.Fdbcklmt = 0;
            _itemStocktemp.CreateUser = userid.ToString();
            _itemStocktemp.Updated = 0;
            _itemStocktemp.CreateDate = dateTimeMillisecond;
            _itemStocktemp.UpdateUser = userid.ToString();
            _itemStocktemp.UpdateDate = dateTimeMillisecond;
            dbFront.ItemStocktemp.Add(_itemStocktemp);
            #endregion
            try
            {
                dbFront.SaveChanges();
                result.IsSuccess = true;
                result.Code = (int)ResponseCode.Success;
                result.Msg = "success";
            }
            catch (Exception error)
            {
                logger.Error("ErrorMsg:  " + error.Message + " [ErrorStackTrace] " + error.StackTrace + " ;[ExceptionMessaage]: " + this.ExceptionMessaage(error));
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                result.Code = (int)ResponseCode.Error;
            }
            return result;
        }
        #endregion
        #region 把資料寫入 ItemCategoryTemp
        public ActionResponse<string> ItemCategoryTemp(TWNewEgg.API.Models.BatchExamineModel batchExamineModel, int itemtempid, int userid)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            List<int> categoryid = new List<int>();
            TWNewEgg.DB.TWSqlDBContext dbFront = new TWSqlDBContext();
            //判斷是否有傳入誇分類 1
            if (batchExamineModel.ItemCategory.SubCategoryID_1_Layer2 != 0 && batchExamineModel.ItemCategory.SubCategoryID_1_Layer2 != null)
            {
                categoryid.Add(batchExamineModel.ItemCategory.SubCategoryID_1_Layer2.GetValueOrDefault());
            }
            //判斷是否有傳入誇分類 2
            if (batchExamineModel.ItemCategory.SubCategoryID_2_Layer2 != 0 && batchExamineModel.ItemCategory.SubCategoryID_2_Layer2 != null)
            {
                categoryid.Add(batchExamineModel.ItemCategory.SubCategoryID_2_Layer2.GetValueOrDefault());
            }
            //有誇分類資料再寫入資料
            if (categoryid.Count != 0)
            {
                foreach (int itemSubCategoryId in categoryid)
                {
                    DB.TWSQLDB.Models.ItemCategorytemp _itemcategorytemp = new DB.TWSQLDB.Models.ItemCategorytemp();
                    _itemcategorytemp.itemtempID = itemtempid;
                    _itemcategorytemp.CategoryID = itemSubCategoryId;
                    _itemcategorytemp.FromSystem = "1";//0: PM; 1: sellerPortal
                    _itemcategorytemp.CreateUser = userid.ToString();
                    _itemcategorytemp.CreateDate = DateTime.Now;
                    _itemcategorytemp.UpdateDate = DateTime.Now;
                    _itemcategorytemp.UpdateUser = userid.ToString();
                    dbFront.ItemCategorytemp.Add(_itemcategorytemp);
                }
                try
                {
                    dbFront.SaveChanges();
                    result.IsSuccess = true;
                }
                catch (Exception error)
                {
                    logger.Error("MsgError: " + error.Message + "; [StackTrace]: " + error.StackTrace + " ;[ExceptionMessaage]: " + this.ExceptionMessaage(error));
                    result.IsSuccess = false;
                    result.Msg = "資料錯誤";
                }
            }
            else
            {
                //沒有誇分類
                result.IsSuccess = true;
            }
            return result;
        }
        #endregion
        #region(一維屬性)
        public ActionResponse<string> productPropertytempOneDimension(oneArrayProperty _oneArrayProperty, List<TWNewEgg.API.Models.propertyJoinModel> joinPropertyNameValueModel, int sellerid, int produtTempId, int userid, string inputValue)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            List<TWNewEgg.API.Models.SaveProductProperty> _listSaveProductProperty = new List<SaveProductProperty>();
            TWNewEgg.DB.TWSqlDBContext dbFront = new TWSqlDBContext();
            #region 第一順位(因為維喔維屬性商品, 所以只會有第一順位)
            var firstOrder = joinPropertyNameValueModel.Where(p => p.ItemPropertyValue_ID == _oneArrayProperty.firstGroupVauleId).FirstOrDefault();
            TWNewEgg.API.Models.SaveProductProperty propertyOrder1 = new SaveProductProperty();
            propertyOrder1.GroupID = firstOrder.ItemPropertyName_groupId;
            propertyOrder1.PropertyID = firstOrder.ItemPropertyValue_propertyNameID;
            propertyOrder1.ValueID = firstOrder.ItemPropertyValue_ID;
            propertyOrder1.InputValue = string.Empty;
            propertyOrder1.UpdateUser = userid.ToString();
            _listSaveProductProperty.Add(propertyOrder1);
            TWNewEgg.API.Service.ProductPorpertyTempService productPorpertyTempService = new ProductPorpertyTempService();
            var productPropertyTemp = productPorpertyTempService.SaveProductPropertyTempClick(_listSaveProductProperty, produtTempId, userid);
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
            #endregion
        }
        #endregion
        #region 把資料寫入 productPropertytemp(二維屬性)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="colorsizeModel">第一順位(顏色)</param>
        /// <param name="property"></param>第二順位(尺寸)
        /// <param name="joinPropertyNameValueModel"></param>
        /// <param name="sellerid"></param>
        /// <param name="produtTempId"></param>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        public ActionResponse<string> productPropertytempTwoDimension(colorSizeModel colorsizeModel, propertyModel property, List<TWNewEgg.API.Models.propertyJoinModel> joinPropertyNameValueModel, int sellerid, int produtTempId, int userid, string inputValue)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            List<TWNewEgg.API.Models.SaveProductProperty> _listSaveProductProperty = new List<SaveProductProperty>();
            TWNewEgg.DB.TWSqlDBContext dbFront = new TWSqlDBContext();
            #region 第一順位
            var firstOrder = joinPropertyNameValueModel.Where(p => p.ItemPropertyValue_ID == colorsizeModel.colorValueId).FirstOrDefault();
            TWNewEgg.API.Models.SaveProductProperty propertyOrder1 = new SaveProductProperty();
            propertyOrder1.GroupID = firstOrder.ItemPropertyName_groupId;
            propertyOrder1.PropertyID = firstOrder.ItemPropertyValue_propertyNameID;
            propertyOrder1.ValueID = firstOrder.ItemPropertyValue_ID;
            propertyOrder1.InputValue = string.Empty;
            propertyOrder1.UpdateUser = userid.ToString();
            _listSaveProductProperty.Add(propertyOrder1);
            #endregion
            #region 第二順位
            var SecondOrder = joinPropertyNameValueModel.Where(p => p.ItemPropertyValue_ID == property.proValueId).FirstOrDefault();
            TWNewEgg.API.Models.SaveProductProperty propertyOrder2 = new SaveProductProperty();
            propertyOrder2.GroupID = SecondOrder.ItemPropertyName_groupId;
            propertyOrder2.PropertyID = SecondOrder.ItemPropertyValue_propertyNameID;
            propertyOrder2.ValueID = SecondOrder.ItemPropertyValue_ID;
            propertyOrder2.InputValue = "";
            propertyOrder2.UpdateUser = userid.ToString();
            _listSaveProductProperty.Add(propertyOrder2);
            #endregion
            TWNewEgg.API.Service.ProductPorpertyTempService productPorpertyTempService = new ProductPorpertyTempService();
            var productPropertyTemp = productPorpertyTempService.SaveProductPropertyTempClick(_listSaveProductProperty, produtTempId, userid);
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
        #region 把資料寫入 ItemGroupDetailProperty(一維屬性)
        public ActionResponse<string> ItemGroupDetailPropertyOneDimension(List<TWNewEgg.API.Models.propertyJoinModel> joinPropertyNameValueModel, oneArrayProperty _oneArrayProperty, int groupId, int itemtempId, int sellerid, int userid)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWSqlDBContext dbFront = new TWSqlDBContext();
            TWNewEgg.DB.TWSQLDB.Models.ItemGroupDetailProperty itemGroupDetailProperty_insert = new DB.TWSQLDB.Models.ItemGroupDetailProperty();
            // 利用 colorValueId 在 jModel 抓取對應的 MasterPropertyID 和 GroupValueId(第一順位)
            var MasterPropertyID_GroupValueId = joinPropertyNameValueModel.Where(p => p.ItemPropertyValue_ID == _oneArrayProperty.firstGroupVauleId).FirstOrDefault();
            //群組 id
            itemGroupDetailProperty_insert.GroupID = groupId;
            //itemtemp 待審區 id
            itemGroupDetailProperty_insert.ItemTempID = itemtempId;
            //目前的 sellerid
            itemGroupDetailProperty_insert.SellerID = sellerid;
            //第一順位的 ItemPropertyName_ID
            itemGroupDetailProperty_insert.MasterPropertyID = MasterPropertyID_GroupValueId.ItemPropertyName_ID;
            //第二順位的 ItemPropertyName_ID, 0: 此商品為一維屬性
            itemGroupDetailProperty_insert.PropertyID = 0;
            //第一順位的 ItemPropertyValue_ID
            itemGroupDetailProperty_insert.GroupValueID = MasterPropertyID_GroupValueId.ItemPropertyValue_ID;
            //第二順位的 ItemPropertyValue_ID
            itemGroupDetailProperty_insert.ValueID = 0;
            //第二順位對應的 ItemPropertyValue_propertyValue
            itemGroupDetailProperty_insert.ValueName = "";
            //第二順位的自定義
            itemGroupDetailProperty_insert.InputValue = _oneArrayProperty.inputValue;
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
                logger.Error("[Message]: " + error.Message + "; [StackTrace]: " + error.StackTrace + " ;[ExceptionMessaage]: " + this.ExceptionMessaage(error));
            }
            return result;

        }
        #endregion
        #region 把資料寫入 ItemGroupDetailProperty(二維屬性)
        public ActionResponse<string> ItemGroupDetailPropertyTwoDimension(List<TWNewEgg.API.Models.propertyJoinModel> joinPropertyNameValueModel, colorSizeModel colorSizeModel, propertyModel propertyModel, int groupId, int itemtempId, int sellerid, int userid)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWSqlDBContext dbFront = new TWSqlDBContext();
            TWNewEgg.DB.TWSQLDB.Models.ItemGroupDetailProperty itemGroupDetailProperty_insert = new DB.TWSQLDB.Models.ItemGroupDetailProperty();
            // 利用 colorValueId 在 jModel 抓取對應的 MasterPropertyID 和 GroupValueId(第一順位)
            var MasterPropertyID_GroupValueId = joinPropertyNameValueModel.Where(p => p.ItemPropertyValue_ID == colorSizeModel.colorValueId).FirstOrDefault();
            //利用 proValueId  在 jModel 抓取對應的 valueName 和 propertyid(第二順位)
            var valueId_valueName_propertyid = joinPropertyNameValueModel.Where(p => p.ItemPropertyValue_ID == propertyModel.proValueId).FirstOrDefault();
            //群組 id
            itemGroupDetailProperty_insert.GroupID = groupId;
            //itemtemp 待審區 id
            itemGroupDetailProperty_insert.ItemTempID = itemtempId;
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
            itemGroupDetailProperty_insert.InputValue = colorSizeModel.inputValue;
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
                logger.Error("[Message]: " + error.Message + "; [StackTrace]: " + error.StackTrace + " ;[ExceptionMessaage]: " + this.ExceptionMessaage(error));
            }
            return result;
        }
        #endregion
        #endregion
        #region 一維屬性新增資料到主要的 Table 和 detail property
        public ActionResponse<string> basicAndItemGroupDetailPropertyOneDimesionTable(TWNewEgg.API.Models.BatchExamineModel batchExamineModel, List<TWNewEgg.API.Models.propertyJoinModel> joinPropertyNameValueModel, int sellerid, int userid, int groupid, string AccountTypeCode)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            string inputValue = string.Empty;
            int GroupValueId = 0;
            int itemTempid = 0;
            int imageProcessCount = 0;
            foreach (var firstOrder in batchExamineModel.oneArrayProperty)
            {
                inputValue = string.Empty;
                inputValue = firstOrder.inputValue;
                //第一順位 GroupValueID (Color)
                GroupValueId = firstOrder.firstGroupVauleId;
                #region 把送審資料寫入 productTemp
                ActionResponse<int> productTemp = this.productTemp(batchExamineModel, sellerid, AccountTypeCode, userid);
                //送審有資料錯誤
                if (productTemp.IsSuccess == false)
                {
                    result.IsSuccess = false;
                    result.Msg = productTemp.Msg;
                    break;
                }
                int productTempId = productTemp.Body;
                int? delcType = Convert.ToInt16(productTemp.Msg);
                //判斷 productTempId 有沒有回寫
                if (productTempId == 0 || productTempId == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "系統錯誤";
                    logger.Error("sellerid: " + sellerid + "; userid: " + userid + "; productTempId 沒有回寫");
                    break;
                }
                #endregion
                #region 把送審資料寫入 itemTemp
                ActionResponse<int> itemTemp = this.itemTemp(batchExamineModel, productTempId, userid, delcType, sellerid);
                if (itemTemp.IsSuccess == false)
                {
                    result.IsSuccess = false;
                    result.Msg = productTemp.Msg;
                    break;
                }
                itemTempid = itemTemp.Body;
                //判斷是否 itemtempId 是否有回填
                if (itemTempid == 0 || itemTempid == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "系統錯誤";
                    logger.Error("sellerid: " + sellerid + "; userid: " + userid + "; itemTempId 沒有回寫");
                }
                #endregion
                #region 把送審資料寫入 itemstocktemp
                ActionResponse<string> itemstocktemp = this.itemstocktempOneDimesion(firstOrder, productTempId, userid, batchExamineModel.ItemStock.InventorySafeQty);
                if (itemstocktemp.IsSuccess == false)
                {
                    result.IsSuccess = false;
                    result.Msg = itemstocktemp.Msg;
                    break;
                }
                #endregion
                #region 把送審資料寫入 ItemCategoryTemp
                ActionResponse<string> ItemCategoryTemp = this.ItemCategoryTemp(batchExamineModel, itemTempid, userid);
                if (ItemCategoryTemp.IsSuccess == false)
                {
                    result.IsSuccess = false;
                    result.Msg = productTemp.Msg;
                    break;
                }
                #endregion
                #region 把資料寫入 productPropertytemp
                ActionResponse<string> productPropertytemp = this.productPropertytempOneDimension(firstOrder, joinPropertyNameValueModel, sellerid, productTempId, userid, inputValue);
                if (productPropertytemp.IsSuccess == false)
                {
                    result.IsSuccess = false;
                    result.Msg = productPropertytemp.Msg;
                    break;
                }
                #endregion
                #region 把資料寫入 ItemGroupDetailProperty
                ActionResponse<string> ItemGroupDetailProperty = this.ItemGroupDetailPropertyOneDimension(joinPropertyNameValueModel, firstOrder, groupid, itemTempid, sellerid, userid);
                if (ItemGroupDetailProperty.IsSuccess == false)
                {
                    result.IsSuccess = false;
                    result.Msg = ItemGroupDetailProperty.Msg;
                    break;
                }
                #endregion
                #region 圖片處理
                ActionResponse<string> ImageProcess = this.ImageProcess(batchExamineModel, itemTempid, imageProcessCount);
                if (ImageProcess.IsSuccess == false)
                {
                    logger.Error("sellerid: " + sellerid + ", userid: " + userid + "itemtempid: " + itemTempid + ", productTempid: " + productTempId + ", 圖片下載失敗");
                }
                imageProcessCount++;
                //上面把資料加入 table 都無錯誤之後, 就把 result.IsSuccess 設定為 True
                result.IsSuccess = true;
                #endregion
            }
            if (result.IsSuccess == true)
            {
                result.Msg = "送審成功";
            }
            return result;
        }
        #endregion
        #region 二維屬性新增資料到主要的 Table 和 detail property
        public ActionResponse<string> basicAndItemGroupDetailPropertyTwoDimesionTable(TWNewEgg.API.Models.BatchExamineModel batchExamineModel, List<TWNewEgg.API.Models.propertyJoinModel> joinPropertyNameValueModel, int sellerid, int userid, int groupid, string AccountTypeCode)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            string inputValue = string.Empty;
            int colorValueId = 0;
            int itemTempid = 0;
            int imageProcessCount = 0;
            //第一順位屬性
            foreach (var firstOrder in batchExamineModel.colorsizeProperty)
            {
                inputValue = string.Empty;
                inputValue = firstOrder.inputValue;
                //第一順位 GroupValueID (Color)
                colorValueId = firstOrder.colorValueId;
                //第二順位屬性
                foreach (var secondOrder in firstOrder.listProperty)
                {
                    #region 把送審資料寫入 productTemp
                    ActionResponse<int> productTemp = this.productTemp(batchExamineModel, sellerid, AccountTypeCode, userid);
                    //送審有資料錯誤
                    if (productTemp.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = productTemp.Msg;
                        break;
                    }
                    int productTempId = productTemp.Body;
                    int? delcType = Convert.ToInt16(productTemp.Msg);
                    //判斷 productTempId 有沒有回寫
                    if (productTempId == 0 || productTempId == null)
                    {
                        result.IsSuccess = false;
                        result.Msg = "系統錯誤";
                        logger.Error("sellerid: " + sellerid + "; userid: " + userid + "; productTempId 沒有回寫");
                        break;
                    }
                    #endregion
                    #region 把送審資料寫入 itemTemp
                    ActionResponse<int> itemTemp = this.itemTemp(batchExamineModel, productTempId, userid, delcType, sellerid);
                    if (itemTemp.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = productTemp.Msg;
                        break;
                    }
                    itemTempid = itemTemp.Body;
                    //判斷是否 itemtempId 是否有回填
                    if (itemTempid == 0 || itemTempid == null)
                    {
                        result.IsSuccess = false;
                        result.Msg = "系統錯誤";
                        logger.Error("sellerid: " + sellerid + "; userid: " + userid + "; itemTempId 沒有回寫");
                    }
                    #endregion
                    #region 把送審資料寫入 itemstocktemp
                    ActionResponse<string> itemstocktemp = this.itemstocktempTwoDimesion(secondOrder, productTempId, userid, batchExamineModel.ItemStock.InventorySafeQty);
                    if (itemstocktemp.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = productTemp.Msg;
                        break;
                    }
                    #endregion
                    #region 把送審資料寫入 ItemCategoryTemp
                    ActionResponse<string> ItemCategoryTemp = this.ItemCategoryTemp(batchExamineModel, itemTempid, userid);
                    if (ItemCategoryTemp.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = productTemp.Msg;
                        break;
                    }
                    #endregion
                    #region 把資料寫入 productPropertytemp
                    ActionResponse<string> productPropertytemp = this.productPropertytempTwoDimension(firstOrder, secondOrder, joinPropertyNameValueModel, sellerid, productTempId, userid, inputValue);
                    if (productPropertytemp.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = productTemp.Msg;
                        break;
                    }
                    #endregion
                    #region 把資料寫入 ItemGroupDetailProperty
                    ActionResponse<string> ItemGroupDetailProperty = this.ItemGroupDetailPropertyTwoDimension(joinPropertyNameValueModel, firstOrder, secondOrder, groupid, itemTempid, sellerid, userid);
                    if (ItemGroupDetailProperty.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = productTemp.Msg;
                        break;
                    }
                    #endregion
                    #region 圖片處理
                    ActionResponse<string> ImageProcess = this.ImageProcess(batchExamineModel, itemTempid, imageProcessCount);
                    if (ImageProcess.IsSuccess == false)
                    {
                        logger.Error("sellerid: " + sellerid + ", userid: " + userid + "itemtempid: " + itemTempid + ", productTempid: " + productTempId + ", 圖片下載失敗");
                    }
                    //上面把資料加入 table 都無錯誤之後, 就把 result.IsSuccess 設定為 True
                    result.IsSuccess = true;
                    #endregion
                }
                imageProcessCount++;
                //第二層迴圈有錯時, 停止執行, 跳出最外層迴圈
                if (result.IsSuccess == false)
                {
                    break;
                }
                else
                {
                    result.IsSuccess = true;
                    result.Msg = "送審成功";
                }
            }
            return result;
        }
        #endregion



        #region 從 propertyName 和 propertyValue 把相關資料 join 回來
        public ActionResponse<IEnumerable<TWNewEgg.API.Models.propertyJoinModel>> propertyName_Value_Join(TWNewEgg.API.Models.BatchExamineModel batchNodelJoin, TWNewEgg.API.Models.OneOrTwoDimension DimensionType)
        {
            ActionResponse<IEnumerable<TWNewEgg.API.Models.propertyJoinModel>> result = new ActionResponse<IEnumerable<propertyJoinModel>>();
            TWSqlDBContext dbFront = new TWSqlDBContext();
            List<int> idModel = new List<int>();
            //一維
            if (DimensionType == OneOrTwoDimension.OneDimension)
            {
                foreach (var iditem in batchNodelJoin.oneArrayProperty)
                {
                    idModel.Add(iditem.firstGroupVauleId);
                }
            }
            else if (DimensionType == OneOrTwoDimension.TwoDimension)//二維
            {
                foreach (var iditem in batchNodelJoin.colorsizeProperty)
                {
                    //第一順位的 GroupVauleId
                    idModel.Add(iditem.colorValueId);
                    //colorPropertyNameID.Add(iditem.colorValueId);
                    foreach (var itemPro in iditem.listProperty)
                    {
                        //第二屬順位的 ValueId
                        idModel.Add(itemPro.proValueId);
                    }
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = "資料組合錯誤";
                logger.Error("屬性送審資料不為一維屬性和二維屬性");
                return result;
            }
            IEnumerable<TWNewEgg.API.Models.propertyJoinModel> JoinModel = null;
            try
            {
                JoinModel = (from p in idModel
                             join q in dbFront.ItemPropertyValue on p equals q.ID
                             join r in dbFront.ItemPropertyName on q.PropertyNameID equals r.ID
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
                             }).AsEnumerable();
                if (idModel.Count != JoinModel.Count())
                {
                    result.IsSuccess = false;
                    result.Msg = "資料錯誤，請檢查屬性相關資料的資料是否有填寫錯誤";
                    logger.Error("輸入的 idModel count 與 JoinModel Count 不相等");
                }
                else
                {
                    //IEnumerator<int> checkGroupId = null;
                    IEnumerable<int> checkGroupId = batchNodelJoin.colorsizeProperty.Select(p => p.colorValueId).AsQueryable();
                    //防止輸入的 groupValueId 會是不同的 ItemPropertyName_ID
                    int ItemPropertyName_ID_Count = JoinModel.Select(p => checkGroupId.Contains(p.ItemPropertyName_ID)).Distinct().Count();
                    if (ItemPropertyName_ID_Count != 1)
                    {
                        result.IsSuccess = false;
                        result.Msg = "資料錯誤，請檢查屬性相關資料的資料是否有填寫錯誤";
                        logger.Error("輸入的 groupValueId 是不同的 ItemPropertyName_ID");
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
                result.Body = null;
                logger.Error("[Message]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[ExceptionMessaage]: " + this.ExceptionMessaage(error));
            }
            return result;
        }
        #endregion
        #endregion
        #region 圖片處理
        public ActionResponse<string> ImageProcess(TWNewEgg.API.Models.BatchExamineModel batchExamineModel, int itemtempid, int imageProcessCount)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            List<string> photoURLList = new List<string>();
            TWNewEgg.API.Service.ImageService imgService = new ImageService();
            try
            {
                photoURLList.Add(batchExamineModel.Product.PicPatch_Edit[imageProcessCount]);
                var photoPocess = imgService.ItemSketchBatchImgToTemp(photoURLList, itemtempid);
                if (photoPocess.IsSuccess == true)
                {
                    result.IsSuccess = true;
                    result.Msg = photoPocess.Msg;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = photoPocess.Msg;
                }
            }
            catch (Exception error)
            {
                logger.Error("[Message]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[ExceptionMessaage]: " + this.ExceptionMessaage(error));
            }
            return result;
        }
        #endregion
        #region 批次送審包含屬性的資料完整性型別檢查
        public ActionResponse<string> propertyExamineCheck(TWNewEgg.API.Models.BatchExamineModel _itemSketchOne, List<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo> list_ManufactureInfo)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.DB.TWSellerPortalDBContext spdb = new TWSellerPortalDBContext();
            result.IsSuccess = true;
            result.Msg = "Success";
            //用來判斷是否整數
            bool isInteger;
            //用來判斷時間格式是否正確或錯誤
            DateTime timeTemp = DateTime.Now;
            //用來記錄錯誤訊息
            string str_msg = string.Empty;
            //用來判斷是否 decimal 是否格式正確
            decimal decimalTemp;
            // 用來判斷是否 int 是否格式正確
            int intCheckTemp;
            try
            {
                #region CanSaleLimitQty
                //檢查是否符合格式
                isInteger = System.Text.RegularExpressions.Regex.IsMatch(_itemSketchOne.Item.CanSaleLimitQty.ToString(), @"^-?\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (isInteger == false)
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
                if (DateTime.TryParse(_itemSketchOne.Item.DateStart.ToString(), out timeTemp) == false)
                {
                    result.IsSuccess = false;
                    str_msg = str_msg + "DateStart 錯誤; ";
                }
                #endregion
                #region DelvDate
                if (string.IsNullOrEmpty(_itemSketchOne.Item.DelvDate) == false)
                {
                    //檢查是否有包含危險字串
                    ActionResponse<string>scriptCheck = this.scriptCheck(_itemSketchOne.Item.DelvDate);
                    if (scriptCheck.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg = "DelvDate 錯誤: " + scriptCheck.Msg + "; ";
                    }
                }
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
                #region Note 不維護
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
                //檢查是否符合格式
                isInteger = System.Text.RegularExpressions.Regex.IsMatch(_itemSketchOne.Item.QtyLimit.ToString(), @"^-?\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (isInteger == false)
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
                #endregion
                #region Sdesc
                if (string.IsNullOrEmpty(_itemSketchOne.Item.Sdesc) == true)
                {
                    result.IsSuccess = false;
                    str_msg = str_msg + "Sdesc 錯誤; ";
                }
                else
                {
                    ActionResponse<string> scriptCheckSdec = this.scriptCheck(_itemSketchOne.Item.Sdesc);
                    if (scriptCheckSdec.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg = "Sdesc 錯誤: " + scriptCheckSdec.Msg + "; ";
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
                    ActionResponse<string> scriptCheckSpechead = this.scriptCheck(_itemSketchOne.Item.Spechead);
                    if (scriptCheckSpechead.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg = "Spechead 錯誤: " + scriptCheckSpechead.Msg + "; ";
                    }
                }
                #endregion
                #region BarCode
                if (string.IsNullOrEmpty(_itemSketchOne.Product.BarCode) == false)
                {
                    ActionResponse<string> scriptCheckBarCode = this.scriptCheck(_itemSketchOne.Product.BarCode);
                    if (scriptCheckBarCode.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg = "BarCode 錯誤: " + scriptCheckBarCode.Msg + "; ";
                    }
                }
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
                    ActionResponse<string> scriptCheckDescription = this.scriptCheck(_itemSketchOne.Product.Description);
                    if (scriptCheckDescription.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg = "Description 錯誤: " + scriptCheckDescription.Msg + "; ";
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
                    var manufactureInfoExist = list_ManufactureInfo.Where(p => p.SN == _itemSketchOne.Product.ManufactureID).FirstOrDefault();
                    if (manufactureInfoExist == null)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + "ManufactureID 不存在; ";
                    }
                    //var manufactureInfoExist = spdb.Seller_ManufactureInfo.Where(p => p.SN == _itemSketchOne.Product.ManufactureID).FirstOrDefault();
                    //if (manufactureInfoExist == null)
                    //{
                    //    result.IsSuccess = false;
                    //    str_msg = str_msg + "ManufactureID 不存在; ";
                    //}
                }
                #endregion
                #region Model
                if (string.IsNullOrEmpty(_itemSketchOne.Product.Model) == false)
                {
                    ActionResponse<string> scriptCheckModel = this.scriptCheck(_itemSketchOne.Product.Model);
                    if (scriptCheckModel.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg = "Model 錯誤: " + scriptCheckModel.Msg + "; ";
                    }
                }
                #endregion
                #region Name
                if (string.IsNullOrEmpty(_itemSketchOne.Product.Name) == true)
                {
                    result.IsSuccess = false;
                    str_msg = str_msg + "Name 錯誤; ";
                }
                else
                {
                    ActionResponse<string> scriptCheckName = this.scriptCheck(_itemSketchOne.Product.Name);
                    if (scriptCheckName.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg = "Name 錯誤: " + scriptCheckName.Msg + "; ";
                    }
                }
                #endregion
                #region PicPatch_Edit
                if (_itemSketchOne.Product.PicPatch_Edit.Count > 7)
                {
                    result.IsSuccess = false;
                    str_msg = str_msg + "PicPatch_Edit 錯誤，最多 7 張圖";
                }
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
                        ActionResponse<string> scriptCheckUPC = this.scriptCheck(_itemSketchOne.Product.UPC);
                        if (scriptCheckUPC.IsSuccess == false)
                        {
                            result.IsSuccess = false;
                            str_msg = str_msg = "UPC 錯誤: " + scriptCheckUPC.Msg + "; ";
                        }
                    }
                }
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
                isInteger = System.Text.RegularExpressions.Regex.IsMatch(_itemSketchOne.ItemStock.CanSaleQty.ToString(), @"^-?\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (isInteger == false)
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
                isInteger = System.Text.RegularExpressions.Regex.IsMatch(_itemSketchOne.ItemStock.InventorySafeQty.ToString(), @"^-?\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (isInteger == false)
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
                #endregion
                #region 屬性檢查
                //沒有屬性相關的資料
                if (_itemSketchOne.colorsizeProperty.Count == 0 && _itemSketchOne.oneArrayProperty.Count == 0)
                {
                    result.IsSuccess = false;
                    str_msg = str_msg + "沒有屬性相關的資料; ";
                }
                else
                {
                    //屬性商品不可同時為一維屬性和二維屬性
                    if (_itemSketchOne.colorsizeProperty.Count != 0 && _itemSketchOne.oneArrayProperty.Count != 0)
                    {
                        result.IsSuccess = false;
                        str_msg = str_msg + "屬性商品不可同時為一維屬性和二維屬性; ";
                    }
                    else
                    {
                        //送審一維屬性商品檢查
                        if (_itemSketchOne.colorsizeProperty.Count == 0 && _itemSketchOne.oneArrayProperty.Count != 0)
                        {
                            #region 檢查一維屬性商品屬性的檢查
                            int counter = 1;
                            foreach (var secondProperty in _itemSketchOne.oneArrayProperty)
                            {
                                //檢查是否有填寫 firstGroupVauleId
                                if (secondProperty.firstGroupVauleId == -1)
                                {
                                    result.IsSuccess = false;
                                    str_msg = str_msg + "[第 " + counter + " 屬性 請填寫 firstGroupVauleId] ;";
                                }
                                else
                                {
                                    //檢查第一順位屬性的 GroupVauleId(firstGroupVauleId) 格式是否正確
                                    isInteger = System.Text.RegularExpressions.Regex.IsMatch(secondProperty.firstGroupVauleId.ToString(), @"^-?\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                                    if (isInteger == false)
                                    {
                                        result.IsSuccess = false;
                                        str_msg = str_msg + "[第 "+ counter + " 屬性 firstGroupVauleId 格式錯誤]; ";
                                    }
                                    else
                                    {
                                        //檢查屬性的 Qty 格式是否正確
                                        isInteger = System.Text.RegularExpressions.Regex.IsMatch(secondProperty.Qty.ToString(), @"^-?\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                                        if (isInteger == false)
                                        {
                                            result.IsSuccess = false;
                                            str_msg = str_msg + "[第 " + counter + " 屬性 Qty 格式錯誤]; ";
                                        }
                                        else
                                        {
                                            //可售數量不可小於 0
                                            if (secondProperty.Qty < 0)
                                            {
                                                result.IsSuccess = false;
                                                str_msg = str_msg + "[第 " + counter + " 屬性 Qty 不可小於 0]; ";
                                            }
                                        }
                                    }
                                }
                                counter++;
                            }
                            #region 圖片的數量要跟商品數量一樣
                            if (_itemSketchOne.Product.PicPatch_Edit.Count != _itemSketchOne.oneArrayProperty.Count)
                            {
                                result.IsSuccess = false;
                                str_msg = str_msg = "圖的數量跟商品數量不符合; ";
                            }
                            #endregion
                            #endregion
                        }
                        else
                        {
                            #region 送審二維屬性商品檢查
                            //送審二維屬性商品檢查
                            int twoDimensionCountFirst = 1;
                            foreach (var colorsizePropertyModel in _itemSketchOne.colorsizeProperty)
                            {
                                //檢查第一順位的 groupvalueid(colorValueId) 是否格式正確
                                isInteger = System.Text.RegularExpressions.Regex.IsMatch(colorsizePropertyModel.colorValueId.ToString(), @"^-?\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                                if (isInteger == false)
                                {
                                    result.IsSuccess = false;
                                    str_msg = str_msg + "[第 " + twoDimensionCountFirst + "筆 colorValueId 格式錯誤]; ";
                                }
                                else
                                {
                                    #region 開始檢查第二順位的資料(尺寸和可售數量)
                                    //開始檢查第二順位的資料格式是否正確
                                    int twoDimensionCountSecond = 1;
                                    foreach (var secondSizeProperty in colorsizePropertyModel.listProperty)
                                    {
                                        //檢查 proValueId (第二順位 ValueId) 格式是否正確
                                        isInteger = System.Text.RegularExpressions.Regex.IsMatch(secondSizeProperty.proValueId.ToString(), @"^-?\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                                        if (isInteger == false)
                                        {
                                            result.IsSuccess = false;
                                            str_msg = str_msg + "[第 " + twoDimensionCountFirst + " 筆的第 " + twoDimensionCountSecond + " 筆 " + "proValueId 格式錯誤]; ";
                                            //str_msg = str_msg + "[第 " + twoDimensionCountSecond + "筆 proValueId 格式錯誤]; ";
                                        }
                                        else
                                        {
                                            //檢查是否有填寫 proValueId
                                            if (secondSizeProperty.proValueId == -1)
                                            {
                                                result.IsSuccess = false;
                                                str_msg = str_msg + "[第 " + twoDimensionCountFirst + " 筆的第 " + twoDimensionCountSecond + " 筆 " + "proValueId 格式錯誤]; ";
                                                //str_msg = str_msg + "[第 " + twoDimensionCountSecond + "筆 proValueId 錯誤]; ";
                                            }
                                            else
                                            {
                                                //檢查是否有填寫 proQty
                                                if (secondSizeProperty.proQty == -1)
                                                {
                                                    result.IsSuccess = false;
                                                    str_msg = str_msg + "[第 " + twoDimensionCountFirst + " 筆的第 " + twoDimensionCountSecond + " 筆 " + "proQty 錯誤]; ";
                                                    //str_msg = str_msg + "[第 " + twoDimensionCountSecond + "筆 proQty 錯誤]; ";
                                                }
                                                else
                                                {
                                                    //檢查 proQty 格式是否正確
                                                    isInteger = System.Text.RegularExpressions.Regex.IsMatch(secondSizeProperty.proQty.ToString(), @"^-?\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                                                    if (isInteger == false)
                                                    {
                                                        result.IsSuccess = false;
                                                        str_msg = str_msg + "[第 " + twoDimensionCountFirst + " 筆的第 " + twoDimensionCountSecond + " 筆 " + "proQty 格式錯誤]; ";
                                                        //str_msg = str_msg + "[第 " + twoDimensionCountSecond + "筆 proQty 格式錯誤]; ";
                                                    }
                                                    else
                                                    {
                                                        // 檢查 proQty 不可小於 0
                                                        if (secondSizeProperty.proQty < 0)
                                                        {
                                                            result.IsSuccess = false;
                                                            str_msg = str_msg + "[第 " + twoDimensionCountFirst + " 筆的第 " +  twoDimensionCountSecond + " 筆 "+ "proQty 不可小於 0]; ";
                                                            //str_msg = str_msg + "[第 " + twoDimensionCountSecond + "筆 proQty 不可小於 0]; ";
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        twoDimensionCountSecond++;
                                    }
                                    #endregion
                                }
                                twoDimensionCountFirst++;
                            }
                            #region 圖片的數量要跟商品數量一樣
                            if (_itemSketchOne.Product.PicPatch_Edit.Count != _itemSketchOne.colorsizeProperty.Count)
                            {
                                result.IsSuccess = false;
                                str_msg = str_msg = "圖的數量跟商品數量不符合; ";
                            }
                            #endregion
                            #endregion
                        }
                    }
                }
                #endregion
                if (result.IsSuccess == false)
                {
                    result.Msg = str_msg;
                    result.Body = str_msg;
                }
                else
                {
                    result.Msg = "Success";
                    result.Body = "Success";
                }

            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                logger.Error("[Message]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[ExceptionMessaage]: " + this.ExceptionMessaage(error));
            }
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
        #region 檢查上傳的 Email and password 在 DB 使否有對應的使用者
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
        #region 檢查是否有不合法的字串
        public ActionResponse<string> scriptCheck(string _strScriptCheck)
        {
            ActionResponse<string> result = new ActionResponse<string>();
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
        #region 轉換 y -> Y; n -> N
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
        #region 轉換對應的 GetDelvType
        public ActionResponse<int?> GetDelvType(string accountType, string shipType)
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
        #region 抓取現在時間
        public DateTime NowTime()
        {
            DateTime dateTimeMillisecond = new DateTime(0001, 01, 01, 01, 01, 01, 01);
            string timeString = dateTimeMillisecond.ToString("yyyy-MM-dd hh:mm:ss fff");
            dateTimeMillisecond = DateTime.Now;
            return dateTimeMillisecond;
        }
        #endregion
        #region 轉換 SetResponseCode
        public int SetResponseCode(bool isSuccess)
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
        #endregion
        #region 抓取 inner Exception error message
        public string ExceptionMessaage(Exception error)
        {
            string returnExceptionMessage = string.Empty;
            try
            {
                returnExceptionMessage = error.InnerException != null ? error.InnerException.Message : "";
            }
            catch (Exception ex)
            {
                returnExceptionMessage = "";
                logger.Error("[Message]: " + ex.Message + " ;[StackTrace]: " + ex.StackTrace);
            }
            return returnExceptionMessage;
        }
        #endregion
    }
}

