using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CategoryRepoAdapters.Interface;
using TWNewEgg.ItemGroupDetailPropertyRepoAdapters.Interface;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.PublicAPI.BatchUpdata.Interface;
using TWNewEgg.PublicApiModels;
using TWNewEgg.Seller_AuthTokenRepoAdapters.Interface;
using TWNewEgg.Seller_UserRepoAdapters.Interface;

namespace TWNewEgg.PublicAPI.BatchUpdata
{
    public class BatchUpdata : IBatchUpdata
    {
        private string TokenTimeSetting = System.Configuration.ConfigurationManager.AppSettings["TokenTime"] == null ? "10" : System.Configuration.ConfigurationManager.AppSettings["TokenTime"].ToString();
        private string BatchDataCount = System.Configuration.ConfigurationManager.AppSettings["BatchDataCount"] == null ? "100" : System.Configuration.ConfigurationManager.AppSettings["BatchDataCount"].ToString();
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        private ISeller_UserRepoAdapters _iSeller_UserRepoAdapters;
        private ISeller_AuthTokenRepoAdapters _iSeller_AuthTokenRepoAdapters;
        private IItemRepoAdapter _iItemRepoAdapter;
        private IItemStockRepoAdapter _iItemStockRepoAdapter;
        private IItemGroupDetailPropertyRepoAdapters _iItemGroupDetailPropertyRepoAdapters;
        private ICategoryRepoAdapter _iCategoryRepoAdapter;

        public BatchUpdata(ISeller_UserRepoAdapters iSeller_UserRepoAdapters, ISeller_AuthTokenRepoAdapters iSeller_AuthTokenRepoAdapters,
             IItemRepoAdapter iItemRepoAdapter, IItemStockRepoAdapter iItemStockRepoAdapter, IItemGroupDetailPropertyRepoAdapters iItemGroupDetailPropertyRepoAdapters,
             ICategoryRepoAdapter iCategoryRepoAdapter)
        {
            this._iSeller_UserRepoAdapters = iSeller_UserRepoAdapters;
            this._iSeller_AuthTokenRepoAdapters = iSeller_AuthTokenRepoAdapters;
            this._iItemRepoAdapter = iItemRepoAdapter;
            this._iItemStockRepoAdapter = iItemStockRepoAdapter;
            this._iItemGroupDetailPropertyRepoAdapters = iItemGroupDetailPropertyRepoAdapters;
            this._iCategoryRepoAdapter = iCategoryRepoAdapter;
        }
        public ActionResponse<List<ItemSketchEdit>> UpdateCheck(string emailAccount, string fromIP, string JsonStr, string AuthToken)
        {
            ActionResponse<List<ItemSketchEdit>> resultChceck = new ActionResponse<List<ItemSketchEdit>>();
            BatchEditModel checkTemp = new BatchEditModel();
            //檢查是否註冊、IP是否正確、JSON 轉換是否正確、Token 相關檢查
            checkTemp = this.checkRegisterIPJsonStrToken(emailAccount, fromIP, AuthToken);
            //00: Success
            if (checkTemp.codeNumber != "00")
            {
                resultChceck.IsSuccess = false;
                resultChceck.Msg = checkTemp.codeMessage;
                resultChceck.Code = checkTemp.codeNumber;
                resultChceck.Body = null;
                return resultChceck;
            }
            //轉換 Json 字串為對應的 model
            var JsonStr2ModelResult = this.JsonString2ItemSketchEdit(JsonStr);
            //轉換失敗
            if (JsonStr2ModelResult.IsSuccess == false)
            {
                checkTemp = this.createResultModel(JsonStr2ModelResult.Msg, CodeStatue.Datas_error);
                resultChceck.IsSuccess = false;
                resultChceck.Code = checkTemp.codeNumber;
                resultChceck.Msg = checkTemp.infoMessage;
                resultChceck.Body = null;
                return resultChceck;
            }
            //給予修改資料編號以方便後續訊息處理
            List<ItemSketchEdit> editModelDatas = this.giveNumber(JsonStr2ModelResult.Body);
            string selleridStr = checkTemp.infoMessage.Split(';')[0];
            string UseridStr = checkTemp.infoMessage.Split(';')[1];
            int selleridInt = this.string2Int(selleridStr);
            //檢查是否有傳入 itemid, itemid 是否有對應賣場, 是否為加購商品, itemid 是否為正確對應的 sellerid
            editModelDatas = this.hasItemIDRepeatAddWithSeller(editModelDatas, selleridInt);
            //檢查欲修改資料的型態限制與跨分類
            editModelDatas = this.categoryFieldCheck(editModelDatas);
            var hasToEdit = editModelDatas.Where(p => p.dataCheckStatus.isCorrect == true).ToList();
            //檢查結果沒有一個是正確的
            if (hasToEdit.Count == 0 || hasToEdit == null)
            {
                
                resultChceck.IsSuccess = false;
                resultChceck.Body = editModelDatas;
                resultChceck.Code = ((int)CodeStatue.Datas_error).ToString().Length == 1 ? "0" + ((int)CodeStatue.Datas_error).ToString() : ((int)CodeStatue.Datas_error).ToString();
                resultChceck.Msg = CodeStatue.Datas_error.ToString().Replace("_", " ");
                return resultChceck;
            }
            else
            {
                resultChceck.IsSuccess = true;
                resultChceck.Body = editModelDatas;
                resultChceck.Code = "00";
                resultChceck.Msg = UseridStr;
                return resultChceck;
            }
            
        }
        #region  檢查是否註冊過 vendor, ip是否合法, token 是否正確是否過期
        private BatchEditModel checkRegisterIPJsonStrToken(string emailAccount, string fromIP, string AuthTokenFromUser)
        {
            BatchEditModel checkResultTemp = new BatchEditModel();
            string sellerid = string.Empty;
            string userid = string.Empty;
            string tokenStr = string.Empty;
            DateTime terminateToken = default(DateTime);
            #region  檢查是否註冊過 vendor
            var isRegisterCheck = this.IsRegistered(emailAccount);
            if (isRegisterCheck.codeNumber != "00")
            {
                checkResultTemp = isRegisterCheck;
                return checkResultTemp;
            }
            else
            {
                sellerid = isRegisterCheck.infoMessage.Split(';')[0];
                userid = isRegisterCheck.infoMessage.Split(';')[1];
            }
            #endregion
            #region 檢查 IP 是否合法
            var isIPValidedCheck = this.isIPValided(fromIP, emailAccount);
            if (isIPValidedCheck.IsSuccess == false)
            {
                checkResultTemp.codeMessage = isIPValidedCheck.Msg;
                checkResultTemp.codeNumber = isIPValidedCheck.Code;
                return checkResultTemp;
            }
            else
            {
                tokenStr = isIPValidedCheck.Body.Token;
                terminateToken = isIPValidedCheck.Body.TerminateDate.Value;
            }
            #endregion
            #region 檢查 token 正確的話，更新 token 時間
            var isAuthTokenCurrectCheck = this.isAuthTokenCurrect(tokenStr, AuthTokenFromUser, terminateToken);
            if (isAuthTokenCurrectCheck.codeNumber != "00")
            {
                checkResultTemp = isAuthTokenCurrectCheck;
            }
            else
            {
                int TokenTimeSettingInt = 0;
                int.TryParse(TokenTimeSetting, out TokenTimeSettingInt);
                isIPValidedCheck.Body.TerminateDate = DateTime.Now.AddMinutes(TokenTimeSettingInt);
                this._iSeller_AuthTokenRepoAdapters.Update(isIPValidedCheck.Body);
                checkResultTemp = this.createResultModel(sellerid.ToString() + ";" + userid.ToString(), CodeStatue.Success);
            }
            return checkResultTemp;
            #endregion
        }
        #region 檢查是否有註冊過 vendor
        private BatchEditModel IsRegistered(string emailAccount)
        {
            BatchEditModel IsRegisteredResult = new BatchEditModel();
            if (string.IsNullOrEmpty(emailAccount) == true)
            {
                IsRegisteredResult = this.createResultModel("the emailAccount is null or empty", CodeStatue.No_datas);
                return IsRegisteredResult;
            }
            else
            {
                emailAccount = emailAccount.Trim();
            }
            //利用 email 讀取對應的資料
            IQueryable<TWNewEgg.VendorModels.DBModels.Model.Seller_User> sellerUserAsQuery = this._iSeller_UserRepoAdapters.GetAllSellerUser().Where(p => p.UserEmail == emailAccount);
            //無對應資料表示無權限
            if (sellerUserAsQuery.Any() == false)
            {
                IsRegisteredResult = this.createResultModel("不為 SellerPortal 註冊帳號", CodeStatue.Account_is_not_registered);
            }
            else
            {
                int sellerID = sellerUserAsQuery.FirstOrDefault().SellerID.Value;
                int userid = sellerUserAsQuery.FirstOrDefault().UserID;
                IsRegisteredResult = this.createResultModel(sellerID.ToString() + ";" + userid.ToString(), CodeStatue.Success);
            }
            return IsRegisteredResult;
        }
        #endregion
        #region 檢查IP是否合法
        private ActionResponse<TWNewEgg.VendorModels.DBModels.Model.Seller_AuthToken> isIPValided(string strIP, string emailAccount)
        {
            ActionResponse<TWNewEgg.VendorModels.DBModels.Model.Seller_AuthToken> _result = new ActionResponse<VendorModels.DBModels.Model.Seller_AuthToken>();
            
            if (string.IsNullOrEmpty(emailAccount) == true || string.IsNullOrEmpty(strIP) == true)
            {
                _result.IsSuccess = false;
                _result.Code = "0" + ((int)CodeStatue.No_datas).ToString();
                _result.Msg = CodeStatue.No_datas.ToString().Replace("_", " ");
                return _result;
            }
            else
            {
                emailAccount = emailAccount.Trim();
                strIP = strIP.Trim();
            }
            //檢查帳號有無權限
            IQueryable<TWNewEgg.VendorModels.DBModels.Model.Seller_AuthToken> _sellerAuthAsQuery = this._iSeller_AuthTokenRepoAdapters.GetAll().Where(p => p.AccountIdEmail == emailAccount);
            if (_sellerAuthAsQuery.Any() == false)
            {
                _result.IsSuccess = false;
                _result.Code = "0" + ((int)CodeStatue.Account_is_invalid).ToString();
                _result.Msg = CodeStatue.Account_is_invalid.ToString().Replace("_", " ");
                return _result;
            }
            TWNewEgg.VendorModels.DBModels.Model.Seller_AuthToken _sellerAuthData = _sellerAuthAsQuery.FirstOrDefault();
            #region 檢查 IP 是否在合法的範圍內
            var _IPRangeCheck = this.IPRangeCheck(strIP, _sellerAuthData.StartIP, _sellerAuthData.EndIP);
            if (_IPRangeCheck.codeNumber != "00")
            {
                _result.IsSuccess = false;
                _result.Code = _IPRangeCheck.codeNumber;
                _result.Msg = _IPRangeCheck.codeMessage;
                return _result;
            }
            else
            {
                _result.IsSuccess = true;
                _result.Body = _sellerAuthData;
            }
            #endregion
            return _result;
        }
        #endregion
        #region 檢查 IP 是否在合法的範圍內
        private BatchEditModel IPRangeCheck(string _fromIP, string StartIP, string EndIP)
        {
            BatchEditModel _result = new BatchEditModel();
            try
            {
                long fromIP = this.IP2Long(_fromIP);
                long longStartIP = this.IP2Long(StartIP);
                long longEndIP = this.IP2Long(EndIP);
                if (fromIP >= longStartIP && fromIP <= longEndIP)
                {
                    _result = this.createResultModel("Success", CodeStatue.Success);
                   
                }
                else
                {
                    _result = this.createResultModel("IP 無效", CodeStatue.IP_is_invalid);
                }
            }
            catch (Exception ex)
            {
                _result = this.createResultModel("System Error or ip error", CodeStatue.System_Error);
                logger.Error(ex.ToString());
            }
            return _result;
        }
        #endregion
        #region 檢查token是否過期 是否正確
        private BatchEditModel isAuthTokenCurrect(string authToken, string AuthTokenFromUser, DateTime authTime)
        {
            BatchEditModel tokenResult = new BatchEditModel();
            if (authToken != AuthTokenFromUser)
            {
                tokenResult = this.createResultModel("AuthToken 錯誤", CodeStatue.AuthToken_error);
                return tokenResult;
            }
            else
            {
                DateTime dateTimeNow = DateTime.Now;
                if (authTime >= dateTimeNow)
                {
                    tokenResult = this.createResultModel("Success", CodeStatue.Success);
                }
                else
                {
                    tokenResult = this.createResultModel("AuthToken 過期", CodeStatue.AuthToken_is_Expired);
                }
                return tokenResult;
            }
        }
        #endregion
        #endregion
        #region JsonString 轉換成 model
        private ActionResponse<List<ItemSketchEdit>> JsonString2ItemSketchEdit(string JsonString)
        {
            ActionResponse<List<ItemSketchEdit>> result = new ActionResponse<List<ItemSketchEdit>>();
            try
            {
                JsonString = System.Web.HttpUtility.HtmlDecode(JsonString);
                var convertModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ItemSketchEdit>>(JsonString);
                convertModel.ForEach(p =>
                {
                    //XSS Attack check
                    p.SellerProductID = Microsoft.Security.Application.Sanitizer.GetSafeHtmlFragment(p.SellerProductID);
                    p.Description = Microsoft.Security.Application.Sanitizer.GetSafeHtmlFragment(p.Description);
                    p.Name = Microsoft.Security.Application.Sanitizer.GetSafeHtmlFragment(p.Name);
                    p.Sdesc.Sdesc1 = Microsoft.Security.Application.Sanitizer.GetSafeHtmlFragment(p.Sdesc.Sdesc1);
                    p.Sdesc.Sdesc2 = Microsoft.Security.Application.Sanitizer.GetSafeHtmlFragment(p.Sdesc.Sdesc2);
                    p.Sdesc.Sdesc3 = Microsoft.Security.Application.Sanitizer.GetSafeHtmlFragment(p.Sdesc.Sdesc3);
                    p.Spechead = Microsoft.Security.Application.Sanitizer.GetSafeHtmlFragment(p.Spechead);

                    p.SellerProductID = System.Web.HttpUtility.HtmlDecode(p.SellerProductID);
                    p.Description = System.Web.HttpUtility.HtmlDecode(p.Description);
                    p.Name = System.Web.HttpUtility.HtmlDecode(p.Name);
                    p.Sdesc.Sdesc1 = System.Web.HttpUtility.HtmlDecode(p.Sdesc.Sdesc1);
                    p.Sdesc.Sdesc2 = System.Web.HttpUtility.HtmlDecode(p.Sdesc.Sdesc2);
                    p.Sdesc.Sdesc3 = System.Web.HttpUtility.HtmlDecode(p.Sdesc.Sdesc3);
                    p.Spechead = System.Web.HttpUtility.HtmlDecode(p.Spechead);
                });
                result.Body = convertModel;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Body = null;
                result.IsSuccess = false;
                result.Msg = "Model 轉換失敗";
                logger.Error(ex.ToString());
            }
            return result;
        }
        #endregion
        #region 檢查是否有傳入 itemid, itemid 是否有對應賣場, 是否為加購商品, itemid 是否為正確對應的 sellerid, 跨分類修改是否重複
        public List<ItemSketchEdit> hasItemIDRepeatAddWithSeller(List<ItemSketchEdit> editModel, int sellerid)
        {
            List<Nullable<int>> itemid = editModel.Select(p => p.ItemID).ToList();
            //取出重複的  item id 
            List<int> isRepeatItemid = itemid.GroupBy(n => n.HasValue == true ? n.Value : -1)
                .Where(n => n.Count() > 1)
                .Select(n => n.Key).ToList();
            //過濾掉 -1
            isRepeatItemid = isRepeatItemid.Where(p => p != -1).ToList();
            int ItemCount = this.string2Int(BatchDataCount);
            int editModelTotal = editModel.Count;
            int takeCount = editModelTotal / ItemCount;
            editModel = editModel.OrderBy(p => p.dataCheckStatus.index).ToList();
            for (int i = 0; i <= takeCount; i++)
            {
                var editModelTemp = editModel.Skip(i * ItemCount).Take(ItemCount).ToList();
                //取出要修改的 item id, 如沒有 item id 則給 -1
                List<int> itemidListTemp = editModelTemp.Select(p => p.ItemID.HasValue == true ? p.ItemID.Value : -1).ToList();
                //過濾itemid = -1
                itemidListTemp = itemidListTemp.Where(p => p != -1).ToList();
                //取出對應的 item id 的賣場資料
                List<TWNewEgg.Models.DBModels.TWSQLDB.Item> listItemDataTemp = this._iItemRepoAdapter.GetAll().Where(p => itemidListTemp.Contains(p.ID)).ToList();
                for (int j = 0; j < editModelTemp.Count; j++)
                {

                    if (editModelTemp[j].ItemID.HasValue == false)
                    {
                        editModelTemp[j].dataCheckStatus.isCorrect = false;
                        editModelTemp[j].dataCheckStatus.reason = "沒有賣場編號";
                    }
                    else
                    {
                        bool isCurrect = true;
                        List<string> strMsgList = new List<string>();
                        //檢查賣場編號重複
                        List<int> isRepeatCount = isRepeatItemid.Where(r => r == editModelTemp[j].ItemID.Value).ToList();
                        if (isRepeatCount.Count != 0)
                        {
                            editModelTemp[j].dataCheckStatus.isCorrect = false;
                            strMsgList.Add("賣場編號重複");
                            isCurrect = false;
                        }
                        //檢查是否有此賣場
                        TWNewEgg.Models.DBModels.TWSQLDB.Item itemDataTemp = listItemDataTemp.Where(p => p.ID == editModelTemp[j].ItemID.Value).FirstOrDefault();
                        if (itemDataTemp == null)
                        {
                            editModelTemp[j].dataCheckStatus.isCorrect = false;
                            strMsgList.Add("無此賣場");
                            isCurrect = false;
                        }
                        else
                        {
                            //檢查商品賣場是否為正確的商家賣場資料, 如果沒權限以下賣場相關資料就不必要繼續檢查
                            if (itemDataTemp.SellerID.Equals(sellerid) == false)
                            {
                                editModelTemp[j].dataCheckStatus.isCorrect = false;
                                strMsgList.Add("無權限");
                                isCurrect = false;
                            }
                            else
                            {
                                //檢查加價購商品
                                if (itemDataTemp.ShowOrder == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.ShowOrderStatus.AddtionalItemForCart)
                                {
                                    editModelTemp[j].dataCheckStatus.isCorrect = false;
                                    strMsgList.Add("商品為加價購商品，無法修改");
                                    isCurrect = false;
                                }
                                //檢查跨分類
                                ActionResponse<string> _checkCategoryWithMinus99 = this.checkCategoryWithMinus99(editModelTemp[j].SubCategoryID_1_Layer2, editModelTemp[j].SubCategoryID_2_Layer2, itemDataTemp.CategoryID);
                                if (_checkCategoryWithMinus99.IsSuccess == false)
                                {
                                    editModelTemp[j].dataCheckStatus.isCorrect = false;
                                    strMsgList.Add(_checkCategoryWithMinus99.Msg);
                                    isCurrect = false;
                                }
                                
                            }
                        }
                        //最後檢查結果
                        if (isCurrect == true)
                        {
                            editModelTemp[j].dataCheckStatus.isCorrect = true;
                        }
                        editModelTemp[j].dataCheckStatus.reason = (strMsgList.Count == 0 ? string.Empty : string.Join(", ", strMsgList.ToArray()));
                    }
                }
            }
            return editModel;
        }

        private ActionResponse<string> checkCategoryWithMinus99(int? SubCategoryID_1_Layer2, int? SubCategoryID_2_Layer2, int mainCategoryID)
        {
            ActionResponse<string> checkResult = new ActionResponse<string>();
            checkResult.IsSuccess = true;
            checkResult.Msg = string.Empty;
            if (SubCategoryID_1_Layer2.HasValue == true && SubCategoryID_2_Layer2.HasValue == true)
            {
                //兩個跨分類都有值
                if (SubCategoryID_1_Layer2.Value == -99 && SubCategoryID_2_Layer2.Value == -99)
                {
                    //不檢查
                    checkResult.IsSuccess = true;
                }
                else
                {
                    if (SubCategoryID_1_Layer2.Value == -99)
                    {
                        if (SubCategoryID_2_Layer2.Value == mainCategoryID)
                        {
                            checkResult.IsSuccess = false;
                            checkResult.Msg = "第二層跨分類和主分類重複重複";
                        }
                    }
                    else if (SubCategoryID_2_Layer2.Value == -99)
                    {
                        if (SubCategoryID_1_Layer2.Value == mainCategoryID)
                        {
                            checkResult.IsSuccess = false;
                            checkResult.Msg = "第一層跨分類和主分類重複重複";
                        }
                    }
                    else
                    {
                        //兩個值都不是-99
                        if (SubCategoryID_1_Layer2.Value == SubCategoryID_2_Layer2.Value)
                        {
                            checkResult.IsSuccess = false;
                            checkResult.Msg = "第一層跨分類和第二層跨分類重複";
                        }
                        else
                        {
                            if (SubCategoryID_1_Layer2.Value == mainCategoryID || SubCategoryID_2_Layer2.Value == mainCategoryID)
                            {
                                checkResult.IsSuccess = false;
                                checkResult.Msg = "第一層跨分類或第二層跨分類與主跨分類重複";

                            }
                        }
                    }
                }
            }
            else//輸入的跨分類沒有重複輸入再檢查是否跟要修改的賣場跨分類重複
            {
                //檢查要修改的第一層跨分類是否重複
                if (SubCategoryID_1_Layer2.HasValue == true)
                {
                    if (SubCategoryID_1_Layer2.Value == mainCategoryID)
                    {
                        checkResult.IsSuccess = false;
                        checkResult.Msg = "第一層跨分類修改重複";
                    }
                }
                //檢查要修改的第二層跨分類是否重複
                if (SubCategoryID_2_Layer2.HasValue == true)
                {
                    if (SubCategoryID_2_Layer2.Value == mainCategoryID)
                    {
                        checkResult.IsSuccess = false;
                        checkResult.Msg = "第二層跨分類修改重複";
                    }
                }
            }
            return checkResult;
        }
            
        
        #endregion
        #region 檢查欲修改資料的型態限制與跨分類
        public List<ItemSketchEdit> categoryFieldCheck(List<ItemSketchEdit> checkItemSketchModel)
        {
            List<Nullable<int>> itemid = checkItemSketchModel.Select(p => p.ItemID).ToList();
            //取出重複的  item id 
            List<int> isRepeatItemid = itemid.GroupBy(n => n.HasValue == true ? n.Value : -1)
                .Where(n => n.Count() > 1)
                .Select(n => n.Key).ToList();
            //過濾掉 -1
            isRepeatItemid = isRepeatItemid.Where(p => p != -1).ToList();
            //計算一次要執行的資料量
            int ItemCount = 0;
            int.TryParse(BatchDataCount, out ItemCount);
            int editModelTotal = checkItemSketchModel.Count;
            int takeCount = editModelTotal / ItemCount;
            checkItemSketchModel = checkItemSketchModel.OrderBy(p => p.dataCheckStatus.index).ToList();
            for (int i = 0; i <= takeCount; i++)
            {
                var editModelTemp = checkItemSketchModel.Skip(i * ItemCount).Take(ItemCount).ToList();
                //取出要修改的 item id, 如沒有 item id 則給 -1
                List<int> itemidListTemp = editModelTemp.Select(p => p.ItemID.HasValue == true ? p.ItemID.Value : -1).ToList();
                //過濾itemid = -1
                itemidListTemp = itemidListTemp.Where(p => p != -1).ToList();
                //取出對應的 item id 的賣場資料
                List<CheckModel2Use> _createCheckDataModel = this.itemCategoryProductProperty(itemidListTemp);
                List<ItemWithsubLayer2Check> _itemWithsubLayer2Check = this.getItemAllCategory(_createCheckDataModel, editModelTemp);
                for (int j = 0; j < editModelTemp.Count; j++)
                {
                    if (editModelTemp[j].dataCheckStatus.isCorrect == false)
                    {
                        continue;
                    }
                    var CheckModel2UseTemp = _createCheckDataModel.Where(p => p.itemID == editModelTemp[j].ItemID.Value).FirstOrDefault();
                    ActionResponse<string> _checkEditDataField = this.checkEditDataField(editModelTemp[j], CheckModel2UseTemp);
                    if (_checkEditDataField.IsSuccess == false)
                    {
                        editModelTemp[j].dataCheckStatus.isCorrect = false;
                        editModelTemp[j].dataCheckStatus.reason = _checkEditDataField.Body;
                    }
                    else
                    {
                        if (editModelTemp[j].SubCategoryID_1_Layer2.HasValue == false && editModelTemp[j].SubCategoryID_2_Layer2.HasValue == false)
                        {
                            editModelTemp[j].dataCheckStatus.isCorrect = true;
                            editModelTemp[j].dataCheckStatus.reason = string.Empty;
                        }
                        else
                        {
                            
                            var _itemWithsubLayer2CheckTemp = _itemWithsubLayer2Check.Where(p => p.itemid == editModelTemp[j].ItemID.Value).FirstOrDefault();
                            if (_itemWithsubLayer2CheckTemp == null)
                            {
                                editModelTemp[j].dataCheckStatus.isCorrect = false;
                                editModelTemp[j].dataCheckStatus.reason = "無對應資料";
                            }
                            else
                            {
                                int mainCategoryID = _itemWithsubLayer2CheckTemp.MainCategoryID_0_Layer0.Value;
                                if (editModelTemp[j].SubCategoryID_1_Layer2.HasValue == true && mainCategoryID != _itemWithsubLayer2CheckTemp.SubCategoryID_1_Layer0 && editModelTemp[j].SubCategoryID_1_Layer2.Value != -99)
                                {
                                    editModelTemp[j].dataCheckStatus.isCorrect = false;
                                    editModelTemp[j].dataCheckStatus.reason = "第一跨分類不在同一個跨分類";
                                    continue;
                                }
                                if (editModelTemp[j].SubCategoryID_2_Layer2.HasValue == true && mainCategoryID != _itemWithsubLayer2CheckTemp.SubCategoryID_2_Layer0 && editModelTemp[j].SubCategoryID_2_Layer2.Value != -99)
                                {
                                    editModelTemp[j].dataCheckStatus.isCorrect = false;
                                    editModelTemp[j].dataCheckStatus.reason = "第二跨分類不在同一個跨分類";
                                    continue;
                                }
                                editModelTemp[j].SubCategoryID_1_Layer1 = _itemWithsubLayer2CheckTemp.SubCategoryID_1_Layer1;
                                editModelTemp[j].SubCategoryID_2_Layer1 = _itemWithsubLayer2CheckTemp.SubCategoryID_2_Layer1;
                                editModelTemp[j].dataCheckStatus.isCorrect = true;
                                editModelTemp[j].dataCheckStatus.reason = "跨分類檢查無錯誤";
                            }
                        }
                    }
                }
            }
            return checkItemSketchModel;
        }
        #endregion
        private List<CheckModel2Use> itemCategoryProductProperty(List<int> itemidListTemp)
        {
            List<CheckModel2Use> result = new List<CheckModel2Use>();
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.Item> ItemQuery = this._iItemRepoAdapter.GetAll().Where(p => itemidListTemp.Contains(p.ID));
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemStock> ItemStockQuery = this._iItemStockRepoAdapter.GetAll();
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupDetailProperty> ItemGroupDetailPropertyQuery = this._iItemGroupDetailPropertyRepoAdapters.GetAll();
            result = (from p in ItemQuery
                      join q in ItemStockQuery on p.ProductID equals q.ProductID into query
                      from r in query.DefaultIfEmpty()
                      join s in ItemGroupDetailPropertyQuery on p.ID equals s.ItemID.Value into detail
                      from x in detail.DefaultIfEmpty()
                      select new CheckModel2Use
                      {
                          itemID = p.ID,
                          categoryID = p.CategoryID,
                          InventorySafeQty = r == null ? default(Nullable<int>) : r.SafeQty,
                          productID = p.ProductID,
                          isItemGroupDetailProperty = x == null ? false : true
                      }).ToList();
            return result;
        }
        private List<ItemWithsubLayer2Check> getItemAllCategory(List<CheckModel2Use> itemidmodel, List<ItemSketchEdit> itemEditModel)
        {
            List<ItemWithsubLayer2Check> returnModel = new List<ItemWithsubLayer2Check>();
            returnModel = (from p in itemidmodel
                           join q in itemEditModel on p.itemID equals q.ItemID.Value
                           select new ItemWithsubLayer2Check
                           {
                               itemid = p.itemID,
                               MainCategoryID_0_Layer2 = p.categoryID,
                               SubCategoryID_1_Layer2 = q.SubCategoryID_1_Layer2,
                               SubCategoryID_2_Layer2 = q.SubCategoryID_2_Layer2
                           }).ToList();
            returnModel = this.categoryLayer0(returnModel);
            returnModel = this.categoryLayer1_2(returnModel);
            return returnModel;

        }
        private List<ItemWithsubLayer2Check> categoryLayer0(List<ItemWithsubLayer2Check> resultCheck)
        {
            List<ItemWithsubLayer2Check> result = new List<ItemWithsubLayer2Check>();
            //取出所有主分類的layer2
            List<int> MainCategoryID_0_Layer2_List = resultCheck.Select(p => p.MainCategoryID_0_Layer2.Value).ToList();
            var categoryMainLayer2 = this._iCategoryRepoAdapter.Category_GetAll().Where(p => MainCategoryID_0_Layer2_List.Contains(p.ID)).Select(p => new
            {
                layer2Parent = p.ParentID,
                layer2 = p.ID
            });
            //select 出 layer1 的 category 相關資料
            List<int> MainCategoryID_0_Layer1_List = categoryMainLayer2.Select(p => p.layer2Parent).ToList();
            var categoryMainLayer1 = this._iCategoryRepoAdapter.Category_GetAll().Where(p => MainCategoryID_0_Layer1_List.Contains(p.ID)).Select(p => new
            {
                layer1Parent = p.ParentID,
                layer1 = p.ID
            });
            //合併 layer2 and layer1 的資料成完整的 category 0, 1, 2 資料
            var MainLayerData = (from p in categoryMainLayer2
                                 join q in categoryMainLayer1 on p.layer2Parent equals q.layer1
                                 select new
                                 {
                                     Mainlayer0 = q.layer1Parent,
                                     Mainlayer1 = q.layer1,
                                     Mainlayer2 = p.layer2
                                 });
            //組合自定義的 model 並回傳完整的 category 資料
            result = (from p in resultCheck
                      join q in MainLayerData on p.MainCategoryID_0_Layer2 equals q.Mainlayer2
                      select new ItemWithsubLayer2Check
                      {
                          itemid = p.itemid,
                          MainCategoryID_0_Layer2 = p.MainCategoryID_0_Layer2,
                          SubCategoryID_2_Layer2 = p.SubCategoryID_2_Layer2,
                          SubCategoryID_1_Layer2 = p.SubCategoryID_1_Layer2,
                          MainCategoryID_0_Layer1 = q.Mainlayer1,
                          MainCategoryID_0_Layer0 = q.Mainlayer0
                      }).ToList();
            return result;
        }
        private List<ItemWithsubLayer2Check> categoryLayer1_2(List<ItemWithsubLayer2Check> resultCheck)
        {
            //select 出所有 layer2_2 的跨分類 ID
            List<int> subLayer_2_2 = resultCheck.Select(p => p.SubCategoryID_2_Layer2.HasValue == true ? p.SubCategoryID_2_Layer2.Value : -1).ToList();
            //select 出所有 layer1_2 的跨分類 ID
            List<int> subLayer_1_2 = resultCheck.Select(p => p.SubCategoryID_1_Layer2.HasValue == true ? p.SubCategoryID_1_Layer2.Value : -1).ToList();
            List<int> sybLayer2Total = new List<int>();
            sybLayer2Total.AddRange(subLayer_1_2);
            sybLayer2Total.AddRange(subLayer_2_2);
            //select 所有對應的 layer2 的 category 資料
            var subCategoryLayer2 = this._iCategoryRepoAdapter.Category_GetAll().Where(p => sybLayer2Total.Contains(p.ID) && p.Layer == 2).Select(p => new
            {
                layer2Parent = p.ParentID,
                layer2 = p.ID
            });
            //select 所有對應的 layer1 的 category 資料
            List<int> subLayer1Tota = subCategoryLayer2.Select(p => p.layer2Parent).ToList();
            var subCategoryLayer1 = this._iCategoryRepoAdapter.Category_GetAll().Where(p => subLayer1Tota.Contains(p.ID) && p.Layer == 1).Select(p => new
            {
                layer1Parent = p.ParentID,
                layer1 = p.ID
            });
            //合併 layer 2 和 layer 1 的 category 資料
            var layerTotal = (from p in subCategoryLayer2
                              join q in subCategoryLayer1 on p.layer2Parent equals q.layer1
                              select new
                              {
                                  layer2 = p.layer2,
                                  layer1 = p.layer2Parent,
                                  layer0 = q.layer1Parent
                              });

            resultCheck = (from p in resultCheck
                           join q in layerTotal on (p.SubCategoryID_2_Layer2.HasValue == true ? p.SubCategoryID_2_Layer2.Value : -1) equals q.layer2 into _l1
                           from q in _l1.DefaultIfEmpty()
                           join r in layerTotal on (p.SubCategoryID_1_Layer2.HasValue == true ? p.SubCategoryID_1_Layer2.Value : -1) equals r.layer2 into _l2
                           from r in _l2.DefaultIfEmpty()
                           select new ItemWithsubLayer2Check
                           {
                               itemid = p.itemid,
                               MainCategoryID_0_Layer0 = p.MainCategoryID_0_Layer0,
                               MainCategoryID_0_Layer1 = p.MainCategoryID_0_Layer1,
                               MainCategoryID_0_Layer2 = p.MainCategoryID_0_Layer2,
                               SubCategoryID_2_Layer2 = p.SubCategoryID_2_Layer2,
                               SubCategoryID_2_Layer1 = q != null ? q.layer1 : default(int?),
                               SubCategoryID_2_Layer0 = q != null ? q.layer0 : default(int?),
                               SubCategoryID_1_Layer2 = p.SubCategoryID_1_Layer2,
                               SubCategoryID_1_Layer1 = r != null ? r.layer1 : default(int?),
                               SubCategoryID_1_Layer0 = r != null ? r.layer0 : default(int?),
                           }).ToList();
            return resultCheck;
        }
        private ActionResponse<string> checkEditDataField(ItemSketchEdit checkModel, CheckModel2Use _checkModel2Use)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            List<string> checkResultList = new List<string>();
            #region 是否為規格品
            if (_checkModel2Use.isItemGroupDetailProperty == true)
            {
                checkResultList.Add("商品為規格品，不能進行修改");
            }
            #endregion
            #region 商品簡要描述
            if (string.IsNullOrEmpty(checkModel.Spechead) == false)
            {
                if (checkModel.Spechead.Length > 30)
                {
                    checkResultList.Add("商品簡要描述 限制為30個字");
                }
            }
            #endregion
            #region 商品特色標籤
            if (string.IsNullOrEmpty(checkModel.Sdesc.Sdesc1) == false)
            {
                //if (checkModel.Sdesc.Sdesc1.Equals("[CLEAR]"))
                //{
                //    checkResultList.Add("商品特色標籤第一描述不可填寫\"[CLEAR]\"");
                //}
                if (checkModel.Sdesc.Sdesc1.ToLower().Equals("null") == true)
                {
                    checkResultList.Add("商品特色標籤第一描述不可填寫\"null\" or \"NULL\"");
                }
            }
            #endregion
            #region 成本
            if (checkModel.Cost.HasValue == true)
            {
                if (checkModel.Cost < 0)
                {
                    checkResultList.Add("成本不可小於0");
                }
            }
            #endregion
            #region 售價(user價)
            if (checkModel.PriceCash.HasValue == true)
            {
                if (checkModel.PriceCash < 0)
                {
                    checkResultList.Add("售價(user價)不可小於0");
                }
            }
            #endregion
            #region 保固
            if (checkModel.Warranty.HasValue == true)
            {
                if (checkModel.Warranty < 0)
                {
                    checkResultList.Add("保固月份不可小於0");
                }
            }
            #endregion
            #region 建議售價
            if (checkModel.MarketPrice.HasValue == true)
            {
                if (checkModel.MarketPrice < 0)
                {
                    checkResultList.Add("建議售價不可小於0");
                }
            }
            #endregion
            #region 安全庫存量
            if (checkModel.InventorySafeQty.HasValue == true)
            {
                if (checkModel.InventorySafeQty < 0)
                {
                    checkResultList.Add("安全庫存量不小於0");
                }
            }
            #endregion
            #region 可售數量
            if (checkModel.CanSaleQty.HasValue == true)
            {
                if (checkModel.CanSaleQty < 0)
                {
                    checkResultList.Add("可售數量不小於0");
                }
            }
            #endregion
            #region 可售數量與安全庫存量判斷
            if (checkModel.InventorySafeQty.HasValue == false)
            {
                if (checkModel.CanSaleQty.HasValue == true)
                {
                    if (checkModel.InventorySafeQty < _checkModel2Use.InventorySafeQty)
                    {
                        checkResultList.Add("可售數量必須大於安全庫存量(原賣場)");
                    }
                }
            }
            else
            {
                if (checkModel.CanSaleQty.HasValue == true)
                {
                    if (checkModel.CanSaleQty.Value == 0 && checkModel.InventorySafeQty.Value == 0)
                    {
                    }
                    else
                    {
                        if (checkModel.CanSaleQty < checkModel.InventorySafeQty)
                        {
                            checkResultList.Add("可售數量必須大於安全庫存量");
                        }
                    }
                }
            }
            #endregion
            if (checkResultList.Count == 0)
            {
                result.IsSuccess = true;
            }
            else
            {
                result.IsSuccess = false;
                result.Body = result.Msg = string.Join(", ", checkResultList.ToArray());
            }
            return result;
        }
        private List<ItemSketchEdit> giveNumber(List<ItemSketchEdit> model)
        {
            int i = 1;
            model.ForEach(p =>
            {
                p.dataCheckStatus.index = i;
                i++;
            });
            return model;
        }
        private BatchEditModel createResultModel(string messageStr, CodeStatue _codeStstus)
        {
            BatchEditModel resultModel = new BatchEditModel();
            string codeStrNumber = ((int)_codeStstus).ToString();
            if (codeStrNumber.Length == 1)
            {
                resultModel.codeNumber = "0" + codeStrNumber;
            }
            else
            {
                resultModel.codeNumber = codeStrNumber;
            }
            resultModel.codeMessage = _codeStstus.ToString().Replace("_", " ");
            resultModel.infoMessage = messageStr;
            return resultModel;
        }
        private long IP2Long(string ip)
        {

            if (string.IsNullOrEmpty(ip) == false)
            {
                ip = ip.Trim();
            }
            string[] ipBytes;
            double num = 0;
            ipBytes = ip.Split('.');
            for (int i = ipBytes.Length - 1; i >= 0; i--)
            {
                num += ((int.Parse(ipBytes[i]) % 256) * Math.Pow(256, (3 - i)));
            }
            return (long)num;
        }
        private int string2Int(string convertStr)
        {
            int resultInt = 0;
            int.TryParse(convertStr, out resultInt);
            return resultInt;
        }
    }
}
