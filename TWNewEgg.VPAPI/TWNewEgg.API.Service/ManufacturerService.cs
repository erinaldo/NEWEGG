using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;
using System.Transactions;
using log4net;
using log4net.Config;

namespace TWNewEgg.API.Service
{
    /// <summary>
    /// Manufacturer Service
    /// </summary>
    [System.Runtime.InteropServices.GuidAttribute("B4546AD2-4F49-4198-964B-0E82B963E2F0")]
    public class ManufacturerService
    {
        #region 宣告

        /// <summary>
        /// 回傳類型
        /// </summary>
        private enum ResponseCode
        {
            // 成功
            Success = 0,

            // 失敗
            Error = 1
        }

        /// <summary>
        /// 錯誤列舉
        /// </summary>
        private enum ErrorCode
        {
            // 無錯誤
            None,

            // 製造商網址重複
            URLIsExist,
            SellerID,
            UserID,
            InUserID,
            ManufactureName,
            ManufactureStatus,
            InDate,
            UpdateDate,
            UpdateUserID,

            // 最後字元為斜線
            SlashLastWord
        }

        // 連接 DB
        private DB.TWSellerPortalDBContext db = new DB.TWSellerPortalDBContext();

        // 系統操作記錄
        private static ILog log = LogManager.GetLogger(typeof(ManufacturerService));

        #endregion 宣告

        #region Create

        /// <summary>
        /// 檢查並新增製造商
        /// </summary>
        /// <param name="listManufactutrerInfo">要寫入資料庫的新增清單</param>
        /// <returns>成功、失敗資訊</returns>
        public API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo_Edit>> CreateListManufacturerInfo(List<DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo_Edit> listManufactutrerInfo)
        {
            API.Models.ActionResponse<string> queryResult = new Models.ActionResponse<string>();

            // 放入有 Error 的List
            List<DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo_Edit> errorList = new List<DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo_Edit>();

            // 放入判斷是否有重複的List
            List<DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo_Edit> reapteList = new List<DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo_Edit>();

            API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo_Edit>> returnList = new Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo_Edit>>();

            // 判斷輸入的資料是否為null
            if (null == listManufactutrerInfo || listManufactutrerInfo.Count() == 0)
            {
                returnList.IsSuccess = false;
                returnList.Code = (int)ResponseCode.Error;
                returnList.Body = null;
                returnList.Msg = "No Data input.";

                return returnList;
            }

            // 錯誤訊息
            string errorListMsg = string.Empty;

            // 重複訊息
            string repeateMsg = string.Empty;

            // 已存在 DB 的製造商 URL 清單
            string[] manufacturerURLs = db.Seller_ManufactureInfo_Edit.Select(x => x.ManufactureURL.ToLower()).ToArray();

            // 判斷此次批次建立，Url 是否有重複 & 檢查每一筆資料是否有誤
            foreach (var item in listManufactutrerInfo)
            {
                // 1. 判斷新增 list 中的 URL 是否有重複
                // 2. 判斷 DB 中的 URL 是否有重複
                if (listManufactutrerInfo.Where(x => x.ManufactureURL == item.ManufactureURL).Count() > 1
                 || manufacturerURLs.Contains(item.ManufactureURL.ToLower()))
                {
                    reapteList.Add(item);

                    if (repeateMsg == string.Empty)
                    {
                        repeateMsg += "You Create Manufacture Url double time: " + item.ManufactureName + " ";
                    }
                    else
                    {
                        repeateMsg += "," + item.ManufactureName;
                    }
                }

                // 檢查每一筆資料是否有誤
                // 確認批次建立的各筆資料是否資料有缺
                queryResult = CheckMsgType(item);

                if (queryResult.IsSuccess == false)
                {
                    errorList.Add(item);

                    if (listManufactutrerInfo.Count() == 1)
                    {
                        errorListMsg += queryResult.Msg;
                    }
                    else
                    {
                        if (errorListMsg == "")
                        {
                            errorListMsg += item.ManufactureName + " " + queryResult.Msg;
                        }
                        else
                        {
                            errorListMsg += "," + item.ManufactureName + " " + queryResult.Msg;
                        }
                    }
                }
            }

            // 判斷ErrorList 是否有內容以及批次建立Url 是否有重複，沒有就代表資料全部正確無誤開始批次建立製造商
            if (errorListMsg.Count() > 0 || reapteList.Count() > 0)
            {
                returnList.IsSuccess = false;
                returnList.Code = (int)ResponseCode.Error;
                returnList.Msg = reapteList.Count() > 0 ? repeateMsg : errorListMsg;
                returnList.Msg = returnList.Msg + ".";
                returnList.Body = reapteList.Count() > 0 ? reapteList : errorList;
            }
            else
            {
                // 建立每一筆製造商資料
                foreach (var item in listManufactutrerInfo)
                {
                    // 使用 TransactionScope 保持資料交易完整性
                    using (TransactionScope scope = new TransactionScope())
                    {
                        bool isSuccess = CreateManufacturerInfo(item);

                        // 成功建立製造商 TransactionScope 結束
                        if (isSuccess)
                        {
                            scope.Complete();
                        }
                    }
                }

                // 全部建立完回傳成功訊息
                returnList.IsSuccess = true;
                returnList.Code = (int)ResponseCode.Success;
                returnList.Msg = listManufactutrerInfo.Count() > 1 ? "Create ManufactureInfo List Success." : "Create ManufactureInfo Success";
                returnList.Body = null;

                // 寄送審核需求信給管理者 Jack.W.Wu 0612
                SendManufactureVerifyMail();
            }

            return returnList;
        }

        /// <summary>
        /// 新增製造商
        /// </summary>
        /// <param name="manufacturer_Edit">要新增的製造商資訊</param>
        /// <returns>ture：新增成功，false：新增失敗</returns>
        private bool CreateManufacturerInfo(DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo_Edit manufacturer_Edit)
        {
            // 回傳新增製造商是成功或失敗
            bool isSuccess = false;

            // 記錄訊息
            log.Info(string.Format(
                "開始新增製造商，製造商名稱：{0}，製造商網址：{1}。",
                manufacturer_Edit.ManufactureName,
                manufacturer_Edit.ManufactureURL));

            DateTime nowTmie = DateTime.UtcNow.AddHours(8);
            manufacturer_Edit.InDate = nowTmie;
            manufacturer_Edit.UpdateDate = nowTmie;

            try
            {
                // 新增編輯表資料
                db.Seller_ManufactureInfo_Edit.Add(manufacturer_Edit);
                db.SaveChanges();

                // 如果建立時已是 Approve 狀態(勾選自已核准此要求)，則再呼叫「審核製造商」以核准方式將資料寫入正式表中
                if (manufacturer_Edit.ManufactureStatus == "A")
                {
                    // 建立審核製造商傳入 Model
                    API.Models.ManufacturerUpdateStatusInfo updateStatusInfo = new Models.ManufacturerUpdateStatusInfo();

                    // 建立審核項目
                    List<API.Models.ManufacturerUpdateStatusData> updateStatusData = new List<API.Models.ManufacturerUpdateStatusData>();
                    updateStatusData.Add(new API.Models.ManufacturerUpdateStatusData
                    {
                        ManufactureURL = manufacturer_Edit.ManufactureURL,
                        ManufactureName = manufacturer_Edit.ManufactureName,
                        DeclineReason = string.Empty
                    });

                    // 寫入審核製造商傳入 Model 的資料
                    updateStatusInfo.UpdateUserID = manufacturer_Edit.UpdateUserID;
                    updateStatusInfo.UpdateDate = manufacturer_Edit.UpdateDate;
                    updateStatusInfo.UpdateList = updateStatusData;
                    updateStatusInfo.Command = API.Models.ManufacturerUpdateStatusCommand.Approve;

                    // 呼叫「審核製造商」進行核准審核
                    UpdateStatus(updateStatusInfo);
                }

                // 回傳新增製造商成功
                isSuccess = true;

                // 記錄訊息
                log.Info(string.Format(
                    "新增製造商成功，製造商名稱：{0}，製造商網址：{1}。",
                    manufacturer_Edit.ManufactureName,
                    manufacturer_Edit.ManufactureURL));
            }
            catch (Exception ex)
            {
                // 回傳新增製造商失敗
                isSuccess = false;

                // 記錄訊息
                log.Info(string.Format("新增製造商失敗，失敗原因：{0}。", ex.Message));
            }

            return isSuccess;
        }

        /// <summary>
        /// 檢查錯誤
        /// </summary>
        /// <param name="manufacturer">要新增的製造商資訊</param>
        /// <returns>成功、失敗訊息</returns>
        private API.Models.ActionResponse<string> CheckMsgType(DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo_Edit manufacturer)
        {
            API.Models.ActionResponse<string> queryResult = new Models.ActionResponse<string>();

            #region 判斷網址是否空白或不合法

            // 檢查網址是否空白
            if (!string.IsNullOrEmpty(manufacturer.ManufactureURL))
            {
                //2015 3月 需求要打統編，故 Mark
                //// 檢查網址抬頭，沒有就加上
                //if (!CheckURLTitle(manufacturer.ManufactureURL))
                //{
                //    manufacturer.ManufactureURL = AddURLTitle(manufacturer.ManufactureURL);
                //}

                //// 檢查是否符合網址格式
                //queryResult.IsSuccess = ValidateURL(manufacturer.ManufactureURL);

                // 中文字的檢查在畫面上有
                queryResult.IsSuccess = true;
            }
            else
            {
                queryResult.IsSuccess = false;
            }

            // 若網址未通過檢驗，則回傳錯誤訊息
            if (!queryResult.IsSuccess)
            {
                queryResult.Code = (int)ResponseCode.Error;
                queryResult.Msg = "ManufactureURL not validate";
                queryResult.Body = null;

                return queryResult;
            }

            #endregion 判斷網址是否空白或不合法

            // 檢查錯誤，並回傳錯誤列舉
            ErrorCode checkEmpty = CheckFieldEmpty(manufacturer);

            // 依錯誤列舉，填寫錯誤訊息
            queryResult = ResponseHandle(checkEmpty);

            return queryResult;
        }

        /// <summary>
        /// 填寫回傳的錯誤訊息
        /// </summary>
        /// <param name="errorDetect">錯誤列舉</param>
        /// <returns>成功、失敗訊息</returns>
        private API.Models.ActionResponse<string> ResponseHandle(ErrorCode errorDetect)
        {
            API.Models.ActionResponse<string> result = new Models.ActionResponse<string>();

            // 填寫錯誤訊息
            switch (errorDetect)
            {
                case ErrorCode.None:
                    {
                        result.Msg = "Manufacturer Info no Empty";
                        break;
                    }

                case ErrorCode.URLIsExist:
                    {
                        result.Msg = "Manufacturer URL is already existed";
                        break;
                    }

                case ErrorCode.SlashLastWord:
                    {
                        result.Msg = "Manufacturer URL last word can't be /";
                        break;
                    }

                default:
                    {
                        //未來搭配多語系，回傳建立的訊息
                        result.Msg = "Manufacturer " + errorDetect.ToString() + " Can't be null or empty ";
                        break;
                    }
            }

            // 填寫成功、失敗資訊
            if (errorDetect == ErrorCode.None)
            {
                result.IsSuccess = true;
                result.Code = (int)ResponseCode.Success;
                result.Body = null;
            }
            else
            {
                result.IsSuccess = false;
                result.Code = (int)ResponseCode.Error;
                result.Body = null;
            }

            return result;
        }

        /// <summary>
        /// 檢查各欄位是否有空值或是不正確
        /// </summary>
        /// <param name="inputManufactureModel">要新增的製造商資訊</param>
        /// <returns>錯誤列舉</returns>
        private ErrorCode CheckFieldEmpty(DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo_Edit inputManufactureModel)
        {
            ErrorCode isEmpty = new ErrorCode();
            bool empty = false;

            // 判斷製造商網址是否有重複                                                                                                    
            var q = db.Seller_ManufactureInfo_Edit.Where(p => p.ManufactureURL == inputManufactureModel.ManufactureURL);
            if (q.ToList().Count() != 0)
            {
                isEmpty = ErrorCode.URLIsExist;
                empty = true;
            }

            // 判斷製造商名稱是否為空值
            if (string.IsNullOrEmpty(inputManufactureModel.ManufactureName))
            {
                isEmpty = ErrorCode.ManufactureName;
                empty = true;
            }

            if ((object)inputManufactureModel.SellerID == null)
            {
                isEmpty = ErrorCode.SellerID;
                empty = true;
            }

            if (inputManufactureModel.UserID.HasValue == false)
            {
                isEmpty = ErrorCode.UserID;
                empty = true;
            }

            // 判斷製造商狀態是否為空值
            if (String.IsNullOrEmpty(inputManufactureModel.ManufactureStatus))
            {
                isEmpty = ErrorCode.ManufactureStatus;
                empty = true;
            }

            if (inputManufactureModel.InDate.HasValue == false)
            {
                isEmpty = ErrorCode.InDate;
                empty = true;
            }

            if (inputManufactureModel.UpdateDate.HasValue == false)
            {
                isEmpty = ErrorCode.UpdateDate;
                empty = true;
            }

            if (inputManufactureModel.UpdateUserID.HasValue == false)
            {
                isEmpty = ErrorCode.UpdateUserID;
                empty = true;
            }

            // 如果最後一個字是 / 的話，設定錯誤提示
            if (CheckURLLastWordIsSlash(inputManufactureModel.ManufactureURL))
            {
                isEmpty = ErrorCode.SlashLastWord;
                empty = true;
            }

            if (!empty)
            {
                isEmpty = ErrorCode.None;
            }

            return isEmpty;
        }

        #endregion Create

        #region CreateUserEmail
        /// <summary>
        /// 建立新Email(呼叫 UserService 的 CreateUser 來建立，並將前台輸入資料，打包為 CreateUser 的db模式)
        /// </summary>
        /// <param name="sellerID">商家ID</param>
        /// <param name="userEmail">新增的使用者信箱</param>
        /// <param name="inUserID">建立人UserID</param>
        /// <returns>新建立的使用者ID</returns>
        public API.Models.ActionResponse<int> CreateUserEmail(int sellerID, string userEmail, int inUserID)
        {
            log.Info("新增 UserEmail (製造商) 開始");
            API.Models.Connector sendEmailConnector = new Models.Connector();
            Models.Mail sendEmail = new Models.Mail();

            //將資料打包成 UserService 的 CreateUser 傳入資料型態
            Models.UserCreation createData = new Models.UserCreation();

            // 新增的使用者所屬商家ID
            createData.SellerID = sellerID;

            // 新增的使用者信箱
            createData.Email = userEmail;

            // User群組 (6 指的是由 Manufacturer 所建立的 User)
            createData.GroupID = 6;

            // 建立人UserID
            createData.InUserID = inUserID;

            // 權限類別
            createData.PurviewType = "N";

            log.Info(string.Format("API Add Info(寫入資料庫前): LoginUserID = {0}; LoginSellerID = {1}; AddUserEmail = {2}.", inUserID, sellerID, userEmail));

            API.Models.ActionResponse<int> result = new API.Models.ActionResponse<int>();
            result.Body = -1;
            result.IsSuccess = true;
            result.Msg = string.Empty;

            try
            {
                // 呼叫 UserService 的 CreateUser 以新建新用戶
                TWNewEgg.API.Service.UserService connectUserService = new UserService();
                API.Models.ActionResponse<Models.UserCreationResult> createUserResult = connectUserService.CreateUser(createData);

                // 轉換回傳格式
                result.Code = createUserResult.Code;
                result.IsSuccess = createUserResult.IsSuccess;

                if (createUserResult.Code == (int)TWNewEgg.API.Models.UserLoginingResponseCode.UserExist)
                {
                    result.Msg = "增加失敗，此 Email 已存在。";
                    log.Info(result.Msg);
                }
                else
                {
                    result.Msg = "增加失敗。";
                    log.Info(string.Format("增加失敗; Message = {0}.", result.Msg));
                }
            }
            catch (Exception ex)
            {
                result.Code = (int)ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "增加失敗。";
                result.Body = -1;

                log.Info(string.Format("增加失敗(expection); LoginUserID = {4} LoginSellerID = {0}; AddUserEmail = {1}; ExpectionMessage = {2}; StackTrace = {3}.",
                    sellerID,
                    userEmail,
                    ex.Message,
                    ex.StackTrace,
                    inUserID));
            }

            log.Info("新增 UserEmail (製造商) 結束。");

            return result;
        }
        #endregion CreateUserEmail

        #region Edit
        /// <summary>
        /// 編輯製造商資訊
        /// </summary>
        /// <param name="list_EditData">要寫入資料庫的編輯清單</param>
        /// <returns>編輯成功、失敗資訊</returns>
        public API.Models.ActionResponse<string> EditManufacturerInfo(List<Models.Manufacturer> list_EditData)
        {
            // 回傳訊息
            API.Models.ActionResponse<string> jsonResult = new API.Models.ActionResponse<string>();

            // 如果編輯資料傳入失敗，直接結束編輯
            if (list_EditData == null || list_EditData.Count() == 0)
            {
                jsonResult.Code = (int)ResponseCode.Error;
                jsonResult.IsSuccess = false;
                jsonResult.Body = null;
                jsonResult.Msg = "沒有編輯資料。";

                return jsonResult;
            }

            // 成功、失敗訊息
            API.Models.ManufacturerTempResultMessage tempReslultMessage = new Models.ManufacturerTempResultMessage();

            // 驗證前，先檢查網址抬頭
            for (int i = 0; i < list_EditData.Count(); i++)
            {
                // 若網址有值，才檢查是否有網址抬頭，沒有就加上
                if (!string.IsNullOrEmpty(list_EditData[i].supportURL))
                {
                    if (!CheckURLTitle(list_EditData[i].supportURL))
                    {
                        list_EditData[i].supportURL = AddURLTitle(list_EditData[i].supportURL);
                    }
                }
            }

            // 驗證輸入及儲存編輯
            foreach (var item in list_EditData)
            {
                // 驗證輸入內容，驗證成功才進行儲存編輯的流程
                if (ValidateInputData(item) == Models.ManufacturerValidateSummaryResult.Success)
                {
                    // 使用 TransactionScope 保持資料交易完整性
                    using (TransactionScope scope = new TransactionScope())
                    {
                        bool isSuccess = EditManufacturer(item, ref tempReslultMessage);

                        // 成功編輯製造商 TransactionScope 結束
                        if (isSuccess)
                        {
                            scope.Complete();
                        }
                    }
                }
                else
                {
                    // 若驗證失敗，則寫入失敗訊息
                    tempReslultMessage.ErrorMessage = LogMessage(tempReslultMessage.ErrorMessage, item.ManufactureName);
                }
            }

            // 填寫回傳資訊
            // 如果編輯清單中的所有項目都儲存成功，才會顯示成功訊息，否則就算失敗
            if (string.IsNullOrEmpty(tempReslultMessage.ErrorMessage))
            {
                jsonResult.Code = (int)ResponseCode.Success;
                jsonResult.IsSuccess = true;

                // 編輯的資料，提交給管理者審查  Add by Jack.C 
                SendManufactureVerifyMail();
            }
            else
            {
                jsonResult.Code = (int)ResponseCode.Error;
                jsonResult.IsSuccess = false;
            }

            // 將成功、失敗的訊息合併
            jsonResult.Msg = string.Format(
                "編輯成功：{0}; 編輯失敗：{1}",
                string.IsNullOrEmpty(tempReslultMessage.SuccessMessage) ? "無" : tempReslultMessage.SuccessMessage,
                string.IsNullOrEmpty(tempReslultMessage.ErrorMessage) ? "無" : tempReslultMessage.ErrorMessage);

            // 暫無資料回傳
            jsonResult.Body = string.Empty;

            return jsonResult;
        }

        /// <summary>
        /// 編輯製造商
        /// </summary>
        /// <param name="item">要修改的製造商資訊</param>
        /// <param name="tempReslultMessage">成功、失敗訊息</param>
        /// <returns>ture：編輯成功，false：編輯失敗</returns>
        private bool EditManufacturer(Models.Manufacturer item, ref API.Models.ManufacturerTempResultMessage tempReslultMessage)
        {
            // 回傳編輯製造商是成功或失敗
            bool isSuccess = false;

            try
            {
                // 依 URL 於 DB 編輯表中，讀取要被修改的資料
                DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo_Edit dbData_Edit = db.Seller_ManufactureInfo_Edit.Where(x => x.ManufactureURL == item.ManufactureURL).FirstOrDefault();

                // 若有成功讀取到要修改的資料，則覆寫要修改的值
                if (dbData_Edit != null)
                {
                    // 記錄訊息
                    log.Info(string.Format(
                        "開始編輯製造商，製造商名稱：{0}，製造商網址：{1}。",
                        dbData_Edit.ManufactureName,
                        dbData_Edit.ManufactureURL));

                    // 填寫修改值
                    dbData_Edit.ManufactureName = item.ManufactureName;
                    dbData_Edit.SupportEmail = item.SupportEmail;
                    dbData_Edit.Phone = item.Phone;
                    dbData_Edit.PhoneExt = item.PhoneExt;
                    dbData_Edit.PhoneRegion = item.PhoneRegion;
                    dbData_Edit.supportURL = item.supportURL;

                    // 更新修改日期、修改人
                    dbData_Edit.UpdateUserID = item.UpdateUserID;
                    dbData_Edit.UpdateDate = DateTime.UtcNow.AddHours(8);

                    // 更改資料後，將審核狀態設為 Peding
                    dbData_Edit.ManufactureStatus = "P";

                    // 存入資料庫
                    db.SaveChanges();

                    // 儲存成功後，寫入成功訊息
                    tempReslultMessage.SuccessMessage = LogMessage(tempReslultMessage.SuccessMessage, dbData_Edit.ManufactureName);

                    // 回傳新增製造商成功
                    isSuccess = true;

                    // 記錄訊息
                    log.Info(string.Format(
                        "編輯製造商成功，製造商名稱：{0}，製造商網址：{1}。",
                        dbData_Edit.ManufactureName,
                        dbData_Edit.ManufactureURL));
                }
                else
                {
                    // 若沒有找到要修改的資料，則寫入失敗訊息
                    tempReslultMessage.ErrorMessage = LogMessage(tempReslultMessage.ErrorMessage, item.ManufactureName);

                    // 回傳新增製造商失敗
                    isSuccess = false;

                    // 記錄訊息
                    log.Info("編輯製造商失敗，失敗原因：資料庫讀取失敗。");
                }
            }
            catch (Exception ex)
            {
                // 若從資料庫讀取、寫入時發生錯誤，則寫入失敗訊息
                tempReslultMessage.ErrorMessage = LogMessage(tempReslultMessage.ErrorMessage, item.ManufactureName);

                // 回傳新增製造商失敗
                isSuccess = false;

                // 記錄訊息
                log.Info(string.Format("編輯製造商失敗，失敗原因：{0}。", ex.Message));
            }

            return isSuccess;
        }
        #endregion Edit

        #region GetEmailToList
        /// <summary>
        /// 讀取審核結果通知對象清單
        /// </summary>
        /// <param name="intSellerID">商家 ID</param>
        /// <returns>審核結果通知對象清單</returns>
        public API.Models.ActionResponse<List<Models.ManufacturerEmailToListResultModel>> GetEmailToList(int intSellerID)
        {
            // 記錄訊息
            log.Info("開始讀取 Mail To 清單");

            // 回傳訊息
            API.Models.ActionResponse<List<Models.ManufacturerEmailToListResultModel>> jsonResult = new API.Models.ActionResponse<List<Models.ManufacturerEmailToListResultModel>>();

            // 先檢查此 SellerID 是否存在，有的話才進行列清單的流程
            if (db.Seller_BasicInfo.Any(x => x.SellerID == intSellerID))
            {
                try
                {
                    // 列入清單條件：
                    // 1.同一個 SellerID
                    // 2.所有管理者的資料 (目前管理者的判斷條件為 GroupID = 3)
                    string sellerEmail = db.Seller_BasicInfo.Where(x => x.SellerID == intSellerID).Select(y => y.SellerEmail).FirstOrDefault();

                    // 找出雙重身分的 SellerID
                    List<int> sellerIDs = db.Seller_BasicInfo.Where(x => x.SellerEmail == sellerEmail).Select(y => y.SellerID).ToList();

                    // 避免有 sellerID = -1 的情形，將以前身分是 S 所建立的 userEmail 也加入
                    jsonResult.Body = db.Seller_User.OrderBy(x => x.UserEmail)
                       .Where(x => sellerIDs.Contains(x.SellerID.Value) || x.GroupID == 3 || x.UserEmail == sellerEmail)
                       .Select(x => new Models.ManufacturerEmailToListResultModel()
                       {
                           UserID = x.UserID,
                           UserEmail = x.UserEmail
                       }).ToList();

                    // 讀取成功，將回傳訊息設為 true
                    jsonResult.IsSuccess = true;
                }
                catch
                {
                    // 從資料庫讀取時發生錯誤，將回傳訊息設為 false
                    jsonResult.IsSuccess = false;
                }
            }
            else
            {
                // 找不到此 SellerID，將回傳訊息設為 false
                jsonResult.IsSuccess = false;
            }

            // 填寫回傳訊息
            if (jsonResult.IsSuccess)
            {
                // 讀取成功，填寫成功訊息
                jsonResult.Code = (int)ResponseCode.Success;
                jsonResult.Msg = "讀取審核結果通知對象清單成功。";

                // 記錄訊息
                log.Info("讀取 Mail To 清單：成功。");
            }
            else
            {
                //填寫失敗訊息
                jsonResult.Code = (int)ResponseCode.Error;
                jsonResult.Msg = "讀取審核結果通知對象清單失敗。";
                jsonResult.Body = null;

                // 記錄訊息
                log.Info("讀取 Mail To 清單：失敗。");
            }

            return jsonResult;
        }
        #endregion GetSellerEmailList

        #region GetSellerName
        /// <summary>
        /// 取得製造商名稱
        /// </summary>
        /// <param name="intSellerID">商家ID</param>
        /// <returns>製造商名稱(商家ID)</returns>
        public API.Models.ActionResponse<string> GetSellerName(int intSellerID = -1)
        {
            API.Models.ActionResponse<string> jsonResult = new API.Models.ActionResponse<string>();

            string sellerName = db.Seller_BasicInfo.Where(x => x.SellerID == intSellerID).Select(x => x.SellerName).FirstOrDefault();

            if (!string.IsNullOrEmpty(sellerName))
            {
                jsonResult.IsSuccess = true;
                jsonResult.Code = (int)ResponseCode.Success;
                jsonResult.Body = string.Format("{0} ({1})", sellerName, intSellerID);
                jsonResult.Msg = "Get Seller Name Success.";
            }
            else
            {
                jsonResult.IsSuccess = false;
                jsonResult.Code = (int)ResponseCode.Error;
                jsonResult.Body = string.Empty;
                jsonResult.Msg = "Get Seller Name Fail!";
            }

            return jsonResult;
        }
        #endregion GetSellerName

        #region IsRatifyPermission (審核權限)
        /// <summary>
        /// 查詢是否有審核權限
        /// </summary>
        /// <param name="userID">使用者 ID</param>
        /// <returns>ture：有審核權限，false：無審核權限</returns>
        public API.Models.ActionResponse<bool> IsRatifyPermission(int userID)
        {
            // 回傳訊息
            API.Models.ActionResponse<bool> jsonResult = new Models.ActionResponse<bool>();

            // 宣告 GroupID 做為權限判斷依據
            int groupID = 0;

            // 先依 UserID 去查詢 GroupID
            if (userID > 0)
            {
                groupID = db.Seller_User.Where(x => x.UserID == userID).Select(x => x.GroupID).FirstOrDefault();
            }

            // 當 GroupID 值為 3、4、5 時，具有審核權限
            if (groupID >= 3 && groupID <= 5)
            {
                jsonResult.Code = (int)ResponseCode.Success;
                jsonResult.IsSuccess = true;
                jsonResult.Msg = string.Empty;
                jsonResult.Body = true;
            }
            else
            {
                jsonResult.Code = (int)ResponseCode.Error;
                jsonResult.IsSuccess = false;
                jsonResult.Msg = string.Empty;
                jsonResult.Body = false;
            }

            return jsonResult;
        }
        #endregion 權限

        #region LogMessage
        /// <summary>
        /// 記錄訊息
        /// </summary>
        /// <param name="originalMessage">要新增訊息的存放空間</param>
        /// <param name="newMessage">新增訊息內容</param>
        /// <returns>增加後的訊息</returns>
        private string LogMessage(string originalMessage, string newMessage)
        {
            // 若要新增訊息的存放空間，已有訊息的話，則加上間隔符號
            if (!string.IsNullOrEmpty(originalMessage))
            {
                originalMessage += ", ";
            }

            return string.Format("{0}{1}", originalMessage, newMessage);
        }
        #endregion LogMessage

        #region Search

        /// <summary>
        /// 搜尋製造商 
        /// </summary>
        /// <param name="searchData">搜尋條件</param>
        /// <returns>製造商列表</returns>
        public API.Models.ActionResponse<List<Models.ManufacturerListResultModel>> SearchManufacturerInfo(Models.SearchDataModel searchData)
        {
            Models.ActionResponse<List<Models.ManufacturerListResultModel>> result = new Models.ActionResponse<List<Models.ManufacturerListResultModel>>();
            //防止當 result.Body 為 null 時，在計算 result.Body.Count() 時的錯誤
            result.Body = new List<Models.ManufacturerListResultModel>();

            //若未輸入搜尋關鍵字，則呈現所有的資料
            if (string.IsNullOrEmpty(searchData.KeyWord))
            {
                searchData.KeyWord = string.Empty;
            }

            switch (searchData.SearchType)
            {
                default:
                case Models.SearchType.Approve:
                    {
                        result.Body = Search(searchData).Where(x => x.ManufactureStatus == "A").ToList();

                        if (null != result.Body || 0 != result.Body.Count())
                        {
                            result.Msg = "Return Search Data.(Only Approve)";
                        }

                        break;
                    }

                case Models.SearchType.Selection:
                    {
                        // 若未指定搜尋的狀態，則指定為搜尋所有的狀態
                        if (null == searchData.Status)
                        {
                            searchData.Status = string.Empty;
                        }

                        // 排除錯誤的狀態
                        if (searchData.Status != string.Empty
                         && searchData.Status != "A"
                         && searchData.Status != "P"
                         && searchData.Status != "D")
                        {
                            result.Msg = "Status Error!";
                            break;
                        }
                        else if (searchData.Status == string.Empty

                                 // 如果資料庫中沒有當前指定的狀態值，則跳過 KeyWord 的 Sarch
                              || db.Seller_ManufactureInfo_Edit.Any(x => x.ManufactureStatus == searchData.Status))
                        {
                            result.Body = Search(searchData);

                            // 若有指定搜尋的狀態，才進行狀態的篩選
                            if ("" != searchData.Status)
                            {
                                result.Body = result.Body.Where(x => x.ManufactureStatus == searchData.Status).ToList();
                            }
                        }

                        if (null != result.Body || 0 != result.Body.Count())
                        {
                            result.Msg = "Return All Search Data.(by Selection)";
                        }

                        break;
                    }

                case Models.SearchType.SearchofficialInfobyURL:
                    {
                        result.Body = db.Seller_ManufactureInfo.Where(x => x.ManufactureURL.Contains(searchData.KeyWord.Trim()))
                                     .Select(x => new Models.ManufacturerListResultModel
                                    {
                                        SN = x.SN,
                                        SellerID = x.SellerID,
                                        ManufactureStatus = x.ManufactureStatus,
                                        ManufactureName = x.ManufactureName,
                                        ManufactureURL = x.ManufactureURL,
                                        SupportEmail = x.SupportEmail,
                                        PhoneRegion = x.PhoneRegion,
                                        Phone = x.Phone,
                                        PhoneExt = x.PhoneExt,
                                        supportURL = x.supportURL,
                                        UpdateDate = x.UpdateDate,
                                        DeclineReason = x.DeclineReason
                                    }).Where(x => x.ManufactureStatus == "A").ToList();
                        break;
                    }

                case Models.SearchType.SearchofficialALLInfo:
                    {
                        result.Body = Search(searchData).Where(x => x.ManufactureStatus == "A").ToList();

                        API.Models.ActionResponse<List<Models.ManufacturerListResultModel>> compareDBFrontManufacturer = CompareDBFrontManufacturer(result.Body);
                        if (compareDBFrontManufacturer.IsSuccess && compareDBFrontManufacturer.Body.Count > 0)
                        {
                            result.Body = compareDBFrontManufacturer.Body;
                        }

                        break;
                    }
            }

            // 當篩選 Status 的結果不為空，且有指定 SellerID 時，才進行 SellerID 的篩選
            if ((null != result.Body || 0 != result.Body.Count()) && 0 != searchData.SellerID)
            {
                // 如果篩選 Status 的結果中有當前指定的 SellerID，進行 SellerID 的篩選
                if (result.Body.Any(x => x.SellerID == searchData.SellerID))
                {
                    result.Body = result.Body.Where(x => x.SellerID == searchData.SellerID).ToList();
                }
                else
                {
                    // Status 的結果中沒有當前指定的 SellerID，則結果爲空
                    result.Body = null;
                    result.Msg = "No Data!";
                }
            }

            if (null == result.Body || 0 == result.Body.Count())
            {
                result.IsSuccess = false;
                result.Code = (int)ResponseCode.Error;
                result.Body = null;

                if (string.IsNullOrEmpty(result.Msg))
                {
                    result.Msg = "No Data!";
                }
            }
            else
            {
                result.IsSuccess = true;
                result.Code = (int)ResponseCode.Success;

                //將 Status 的值，由代號轉為文字
                result.Body = TransModelStatus(result.Body);
            }

            #region old code
            #region List (已與 Search 結合)
            ///// <summary>
            ///// 列出製造商清單
            ///// </summary>
            ///// <returns></returns>
            //public API.Models.ActionResponse<List<Models.ManufacturerListResultModel>> ListManufacturerInfo(int intSellerID)
            //{
            //    Models.ActionResponse<List<Models.ManufacturerListResultModel>> result = new Models.ActionResponse<List<Models.ManufacturerListResultModel>>();

            //    if (intSellerID == 0) //管理者登入
            //    {
            //        result = showList();
            //    }
            //    else
            //    {
            //        result.Body = GetResultModelList().Where(x => x.SellerID == intSellerID && (x.ManufactureStatus == "P" || x.ManufactureStatus == "A")).ToList<Models.ManufacturerListResultModel>();

            //        if (result.Body == null || result.Body.Count() == 0)
            //        {
            //            result.Code = (int)ResponseCode.Error;
            //            result.IsSuccess = false;
            //            result.Msg = "查無資料";
            //            result.Body = null;
            //        }
            //        else
            //        {
            //            TransModelStatus(result.Body);

            //            result.Code = (int)ResponseCode.Success;
            //            result.IsSuccess = true;
            //            result.Msg = "List資料傳回";
            //            result.Body = result.Body.OrderBy(x => x.ManufactureName).OrderByDescending(x => x.ManufactureStatus).ToList();
            //        }
            //    }
            //    return result;

            //}

            ///// <summary>
            ///// 取得DB內的資料
            ///// </summary>
            ///// <returns></returns>
            //public API.Models.ActionResponse<List<Models.ManufacturerListResultModel>> showList()
            //{
            //    Models.ActionResponse<List<Models.ManufacturerListResultModel>> result = new Models.ActionResponse<List<Models.ManufacturerListResultModel>>();

            //    result.Body = GetResultModelList();

            //    if (result.Body.Count == 0)
            //    {
            //        result.Code = (int)ResponseCode.Error;
            //        result.IsSuccess = false;
            //        result.Msg = "查無資料";
            //        result.Body = null;
            //    }
            //    else
            //    {
            //        TransModelStatus(result.Body);

            //        result.Code = (int)ResponseCode.Success;
            //        result.IsSuccess = true;
            //        result.Msg = "List資料傳回";
            //        result.Body = result.Body.OrderByDescending(x => x.InDate).ToList();
            //    }
            //    return result;
            //}
            #endregion

            #region old search
            // List<Models.ManufacturerListResultModel> rm = new List<Models.ManufacturerListResultModel>();
            // try
            // {
            //     switch (state)
            //     {
            //         case "P":
            //             //rm=db.Seller_ManufactureInfo.Where(x => x.ManufactureStatus.Contains("P")).ToList<DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo>();
            //             rm = searchManinfo("P", searchword, SellerID);
            //             break;
            //         case "A":
            //          //   rm=db.Seller_ManufactureInfo.Where(x => x.ManufactureStatus.Contains("A")).ToList<DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo>();
            //             rm = searchManinfo("A", searchword, SellerID);
            //             break;
            //         case "D":
            //             //   rm=db.Seller_ManufactureInfo.Where(x => x.ManufactureStatus.Contains("D")).ToList<DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo>();
            //             rm = searchManinfo("D", searchword, SellerID);
            //             break;

            //         default:
            //             rm = searchManinfo("", searchword, SellerID);
            //             break;
            //     }

            //     if (rm.Count == 0)
            //     {
            //         result.Code = (int)ResponseCode.Error;
            //         result.IsSuccess = false;
            //         result.Msg = "Search Data failed";
            //         result.Body = null;
            //     }
            //     else
            //     {
            //         result.Code = (int)ResponseCode.Success;
            //         result.IsSuccess = true;
            //         result.Msg = "Return Search Result";
            //         result.Body = rm.Distinct().OrderByDescending(x => x.InDate).ToList();
            //     }

            // }
            // catch (Exception ex)
            // {
            //     result.Code = (int)ResponseCode.Error;
            //     result.IsSuccess = false;
            //     result.Msg = ex.Message ?? "" + ((ex.InnerException != null) ? ex.InnerException.Message : "");    
            // }
            #endregion
            #endregion

            return result;
        }

        /// <summary>
        /// 搜尋製造商
        /// </summary>
        /// <param name="searchDataInfo">搜尋條件</param>
        /// <returns>製造商列表</returns>
        private List<Models.ManufacturerListResultModel> Search(Models.SearchDataModel searchDataInfo)
        {
            string searchword = searchDataInfo.KeyWord;

            // 判斷是否將 SN 列入搜尋範圍
            bool searchSN = searchDataInfo.SearchSN;

            IQueryable<Models.ManufacturerListResultModel> query = null;
            List<Models.ManufacturerListResultModel> result = new List<Models.ManufacturerListResultModel>();

            switch (searchDataInfo.SearchType)
            {
                case Models.SearchType.SearchofficialALLInfo:
                    query = from q in db.Seller_ManufactureInfo
                            select new Models.ManufacturerListResultModel
                                {
                                    SN = q.SN,
                                    SellerID = q.SellerID,
                                    ManufactureStatus = q.ManufactureStatus,
                                    ManufactureName = q.ManufactureName,
                                    ManufactureURL = q.ManufactureURL,
                                    SupportEmail = q.SupportEmail,
                                    PhoneRegion = q.PhoneRegion,
                                    Phone = q.Phone,
                                    PhoneExt = q.PhoneExt,
                                    supportURL = q.supportURL,
                                    UpdateDate = q.UpdateDate,
                                    DeclineReason = q.DeclineReason
                                };
                    break;
                default:
                    query = from q in db.Seller_ManufactureInfo_Edit
                            select new Models.ManufacturerListResultModel
                                {
                                    SN = q.SN,
                                    SellerID = q.SellerID,
                                    ManufactureStatus = q.ManufactureStatus,
                                    ManufactureName = q.ManufactureName,
                                    ManufactureURL = q.ManufactureURL,
                                    SupportEmail = q.SupportEmail,
                                    PhoneRegion = q.PhoneRegion,
                                    Phone = q.Phone,
                                    PhoneExt = q.PhoneExt,
                                    supportURL = q.supportURL,
                                    UpdateDate = q.UpdateDate,
                                    DeclineReason = q.DeclineReason
                                };
                    break;
            }

            // 增加使用者搜尋關鍵字空白判斷 by jack 2013.11.04
            if (searchword == null)
            {
                searchword = "";
            }

            // 使用者搜尋的字串，以逗號和空白分割
            char[] delimiterChars = { ' ', ',' };
            string[] keyword = DelEmpty(searchword.Split(delimiterChars));

            if (searchword == "")
            {
                result = query.ToList();
            }
            else
            {
                result.AddRange(query.Where(x => x.ManufactureName.Contains(searchword)).ToList());

                foreach (string a in keyword)
                {
                    result.AddRange(query.Where(x => x.ManufactureName.Contains(a)).ToList());
                }

                foreach (string a in keyword)
                {
                    result.AddRange(query.Where(x => x.ManufactureName.Contains(a)
                                                  || x.ManufactureURL.Contains(a)
                                                  || x.SupportEmail.Contains(a)
                                                  || x.supportURL.Contains(a)
                                                  || x.Phone.Contains(a)).ToList());

                    //判斷是否要搜尋 SN
                    if (searchSN)
                    {
                        int sn;
                        //判斷輸入 keyword 是否為數字SN，並進行搜尋
                        if (int.TryParse(a, out sn))
                        {
                            result.AddRange(query.Where(x => x.SN == sn).ToList());
                        }
                    }
                }
            }

            //排除狀態為 L (作廢)的搜尋結果
            result = result.Where(x => x.ManufactureStatus != "L").ToList();

            result = result.GroupBy(x => x.SN).Select(g => g.First()).ToList();

            return result;
        }

        /// <summary>
        /// 比對前台製造商，若兩邊的製造商編號和製造商名稱一致才列入結果清單
        /// </summary>
        /// <param name="sellerPortalManufacturerCell">Seller Portal 製造商清單</param>
        /// <returns>製造商清單</returns>
        private API.Models.ActionResponse<List<Models.ManufacturerListResultModel>> CompareDBFrontManufacturer(List<Models.ManufacturerListResultModel> sellerPortalManufacturerCell)
        {
            API.Models.ActionResponse<List<Models.ManufacturerListResultModel>> result = new Models.ActionResponse<List<Models.ManufacturerListResultModel>>();
            result.Body = new List<Models.ManufacturerListResultModel>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
            List<DB.TWSQLDB.Models.Manufacture> frontManufacturerCell = new List<DB.TWSQLDB.Models.Manufacture>();

            try
            {
                dbFront.Manufacture.ToList().ForEach(x =>
                {
                    DB.TWSQLDB.Models.Manufacture tempManufacture = new DB.TWSQLDB.Models.Manufacture();
                    tempManufacture.ID = x.ID;
                    tempManufacture.WebAddress = x.WebAddress;
                    frontManufacturerCell.Add(tempManufacture);
                });
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                log.Info(string.Format("查詢前台製造商資料失敗(expection); ErrorMessage = {0}; StackTrace = {1}。", ex.Message, ex.StackTrace));
            }

            if (result.IsSuccess && frontManufacturerCell.Count > 0)
            {
                foreach (var sellerPortalManufacturer in sellerPortalManufacturerCell)
                {
                    if (frontManufacturerCell.Any(x => x.WebAddress == sellerPortalManufacturer.ManufactureURL && x.ID == sellerPortalManufacturer.SN))
                    {
                        result.Body.Add(sellerPortalManufacturer);
                    }
                }
            }
            else
            {
                result.IsSuccess = false;
                log.Info("查無前台製造商資料。");
            }

            return result;
        }

        /// <summary>
        /// 刪除空白的搜尋關鍵字
        /// </summary>
        /// <param name="searchWord">待刪除空白的關鍵字列表</param>
        /// <returns>刪除空白後的關鍵字列表</returns>
        public string[] DelEmpty(string[] searchWord)
        {
            List<string> keyWord = new List<string>();
            foreach (var word in searchWord)
            {
                if (word != "")
                {
                    keyWord.Add(word);
                }
            }

            return keyWord.ToArray();
        }

        /// <summary>
        /// 將狀態代號改成文字
        /// </summary>
        /// <param name="transModelList">要轉換的製造商列表</param>
        /// <returns>轉換完的製造商列表</returns>
        private List<Models.ManufacturerListResultModel> TransModelStatus(List<Models.ManufacturerListResultModel> transModelList)
        {
            #region 代號轉換成文字
            foreach (var item in transModelList)
            {
                switch (item.ManufactureStatus)
                {
                    case "P":
                        {
                            item.ManufactureStatus = "Pending";
                            break;
                        }

                    case "A":
                        {
                            item.ManufactureStatus = "Approve";
                            break;
                        }

                    case "D":
                        {
                            item.ManufactureStatus = "Decline";
                            break;
                        }

                    case "L":
                        {
                            item.ManufactureStatus = "Delete";
                            break;
                        }

                    default:
                        {
                            item.ManufactureStatus = string.Empty;
                            break;
                        }
                }
            }
            #endregion 代號轉換成文字

            return transModelList;
        }

        #endregion search

        #region SendEmail

        #region 審核結果通知 (通知 seller)

        /// <summary>
        /// 寄出製造商審核結果通知信
        /// </summary>
        /// <param name="sendMailInfo">審核結果通知信資訊</param>
        /// <param name="chineseResult">中文審核結果</param>
        public void SendManufactureEmail(Models.ManufacturerUpdateStatusSendMailInfo sendMailInfo, string chineseResult)
        {
            Models.Connector connector = new Models.Connector();
            Models.Mail mail = new Models.Mail();

            // 指定 mail 的格式為製造商通知信
            mail.MailType = Models.Mail.MailTypeEnum.ManufactureRequestNotification;

            // 查詢通知對象的信箱地址
            mail.UserEmail = db.Seller_User.Where(x => x.UserID == sendMailInfo.DataOwner).Select(x => x.UserEmail).FirstOrDefault();

            // 若有找到通知對象的信箱地址
            if (!string.IsNullOrEmpty(mail.UserEmail))
            {
                // 填寫信件內容
                mail.MailMessage = ManufactureRequestNotification_MailMessage(sendMailInfo, chineseResult, false);

                // 寄出信件
                connector.SendMail(null, null, mail);

                //若修改資料者非當筆資料建立者，則對修改者另外寄出審核結果通知
                if (sendMailInfo.SendType == Models.ManufacturerUpdateStatusSendType.Modified)
                {
                    mail.UserEmail = db.Seller_User.Where(x => x.UserID == sendMailInfo.EditUser).Select(x => x.UserEmail).FirstOrDefault();

                    // 若有找到通知對象的信箱地址
                    if (!string.IsNullOrEmpty(mail.UserEmail))
                    {
                        // 填寫信件內容
                        mail.MailMessage = ManufactureRequestNotification_MailMessage(sendMailInfo, chineseResult, true);

                        // 寄出信件
                        connector.SendMail(null, null, mail);
                    }
                }
            }
        }

        /// <summary>
        /// 填寫製造商審核結果通知信內容
        /// </summary>
        /// <param name="sendMailInfo">審核結果通知信資訊</param>
        /// <param name="chineseResult">中文審核結果</param>
        /// <param name="isSnedToEditUser">是否寄給編輯者</param>
        /// <returns>製造商審核結果通知信內容</returns>
        private string ManufactureRequestNotification_MailMessage(Models.ManufacturerUpdateStatusSendMailInfo sendMailInfo, string chineseResult, bool isSnedToEditUser)
        {
            #region 判斷審核通知信中，是否增加 "被修改" 的文字

            // 審核通知信中被修改的通知內容
            string modifiedNote = string.Empty;

            // 如果是被編輯或是管理者直接改變審核值，在通知信中就要加上被修改的文字
            // isSnedToEditUser 為 true，則代表此次的信件是寄給編輯者，所以不加上被修改的文字
            if ((sendMailInfo.SendType == Models.ManufacturerUpdateStatusSendType.Modified && isSnedToEditUser == false)
             || sendMailInfo.SendType == Models.ManufacturerUpdateStatusSendType.StatusSwitch)
            {
                modifiedNote = "資訊已被修改，並";
            }
            else
            {
                modifiedNote = "，";
            }

            #endregion 判斷審核通知信中，是否增加 "被修改" 的文字

            #region 判斷審核通知信中，是否增加拒絕原因

            // 審核通知信中拒絕原因內容
            string declineReason = string.Empty;

            // 如果有拒絕原因，而且審核指令是拒絕或作廢的話，則在通知信中加上拒絕原因
            if ((chineseResult == "拒絕" || chineseResult == "作廢")
             && !string.IsNullOrEmpty(sendMailInfo.DeclineReason))
            {
                declineReason = chineseResult + "原因：" + sendMailInfo.DeclineReason;
            }

            #endregion 判斷審核通知信中，是否增加拒絕原因

            return string.Format(
                "您所{0}的製造商「{1}」{2}經過審核的結果為「{3}」。;{4}",
                sendMailInfo.SendType_Chinese,
                sendMailInfo.ManufactureName,
                modifiedNote,
                chineseResult,
                declineReason);
        }

        #endregion 審核結果通知 (通知 seller)

        #region 待審核通知 (通知 newegg 管理員)

        /// <summary>
        /// Seller提出製造商申請，發mail給管理者，請管理者進行審核 Jack.W.Wu 0612
        /// </summary>
        private void SendManufactureVerifyMail()
        {
            // 從 AppSettings 中，讀取寄信對象
            string adminEmail = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"];

            // 將寄信對象拆分成 list
            List<string> adminEmailList = new List<string>();
            adminEmailList = adminEmail.Split(',').ToList();

            // 寄出信件
            foreach (var email in adminEmailList)
            {
                Models.Mail mail = new Models.Mail();
                Models.Connector connector = new Models.Connector();

                mail.UserEmail = email;
                mail.UserName = email.Split('@')[0];
                mail.MailType = Models.Mail.MailTypeEnum.NewManufactureVerifyEmail;
                connector.SendMail(string.Empty, string.Empty, mail);
            }
        }

        #endregion 待審核通知 (通知 newegg 管理員)

        #endregion SendEmail

        #region UpdateStatus
        /// <summary>
        /// 審核製造商
        /// </summary>
        /// <param name="updateInfo">審核資訊</param>
        /// <returns>審核成功及失敗訊息</returns>
        public API.Models.ActionResponse<string> UpdateStatus(Models.ManufacturerUpdateStatusInfo updateInfo)
        {
            // 記錄訊息
            log.Info("開始審核製造商。");

            API.Models.ActionResponse<string> jsonResult = new API.Models.ActionResponse<string>();

            // 查詢審核權限
            bool ratifyPermission = IsRatifyPermission(updateInfo.UpdateUserID.Value).Body;

            // 判斷審核資訊是否完整
            // 1.有更新人、更新日期的資訊
            // 2.有審核項目
            // 3.有審核指令
            // 4.有審核權限
            if ((updateInfo.UpdateUserID > 0 && updateInfo.UpdateDate != null)
             && (updateInfo.UpdateList != null && updateInfo.UpdateList.Count() > 0)
             && updateInfo.Command != null
             && ratifyPermission)
            {
                UpdateStatus_EditStatus(updateInfo, ref jsonResult);
            }
            else
            {
                jsonResult.IsSuccess = false;
                jsonResult.Code = (int)ResponseCode.Error;
                jsonResult.Body = string.Empty;

                // 審核資訊錯誤訊息
                #region ErrorMessage
                // 更新人錯誤
                if (null == updateInfo.UpdateUserID)
                {
                    jsonResult.Msg += string.Format("{0}未輸入審核人資訊", string.IsNullOrEmpty(jsonResult.Msg) ? "" : "，");
                }

                // 更新日期錯誤
                if (null == updateInfo.UpdateDate)
                {
                    jsonResult.Msg += string.Format("{0}未輸入審核日期", string.IsNullOrEmpty(jsonResult.Msg) ? "" : "，");
                }

                // 審核項目錯誤
                if (null == updateInfo.UpdateList || 0 == updateInfo.UpdateList.Count())
                {
                    jsonResult.Msg += string.Format("{0}未輸入審核項目", string.IsNullOrEmpty(jsonResult.Msg) ? "" : "，");
                }

                // 審核指令錯誤
                if (null == updateInfo.Command)
                {
                    jsonResult.Msg += string.Format("{0}審核指令錯誤", string.IsNullOrEmpty(jsonResult.Msg) ? "" : "，");
                }

                // 審核權限錯誤
                if (!ratifyPermission)
                {
                    jsonResult.Msg += string.Format("{0}審核權限錯誤", string.IsNullOrEmpty(jsonResult.Msg) ? "" : "，");
                }
                #endregion ErrorMessage
            }

            // 記錄訊息
            log.Info(string.Format(
                "結束審核製造商，審核結果：{0}。",
                jsonResult.IsSuccess ? "成功" : string.Format("失敗，失敗原因：{0}", jsonResult.Msg)));

            return jsonResult;
        }

        /// <summary>
        /// 編輯狀態值
        /// </summary>
        /// <param name="updateInfo">審核資訊</param>
        /// <param name="jsonResult">審核成功及失敗訊息</param>
        private void UpdateStatus_EditStatus(Models.ManufacturerUpdateStatusInfo updateInfo, ref API.Models.ActionResponse<string> jsonResult)
        {
            API.Service.TWService updateTWService = new TWService();
            API.Service.TWService editTWServive = new TWService();

            // 審核結果通知信資訊列表
            List<Models.ManufacturerUpdateStatusSendMailInfo> list_SendMailInfo = new List<Models.ManufacturerUpdateStatusSendMailInfo>();

            // 成功、失敗訊息
            API.Models.ManufacturerTempResultMessage tempReslultMessage = new Models.ManufacturerTempResultMessage();

            foreach (Models.ManufacturerUpdateStatusData updateData in updateInfo.UpdateList)
            {
                // 讀取編輯表資料
                DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo_Edit dbData_Edit = db.Seller_ManufactureInfo_Edit.Where(x => x.ManufactureURL == updateData.ManufactureURL).FirstOrDefault();

                // 填入拒絕原因 (若審核指令為 Approve 則將拒絕原設定為空白，否則就填入拒絕原因)
                if (updateInfo.Command == Models.ManufacturerUpdateStatusCommand.Approve)
                {
                    dbData_Edit.DeclineReason = string.Empty;
                }
                else
                {
                    dbData_Edit.DeclineReason = updateData.DeclineReason;
                }

                // 儲存審核結果通知的相關資訊 (審核後，UpdateUserID 和 UpdateDate 的資料會變成審核人的；為了能寄信通修改人，所以在讀入資料後，先將寄送 mail 時要用到的欄位先暫存起來)
                UpdateStatus_SaveMailInfo(dbData_Edit, ref list_SendMailInfo);

                if (dbData_Edit != null)
                {
                    // 使用 TransactionScope 保持資料交易完整性
                    using (TransactionScope scope = new TransactionScope())
                    {
                        switch (updateInfo.Command)
                        {
                            case Models.ManufacturerUpdateStatusCommand.Approve:
                                {
                                    // 更新編輯表狀態
                                    dbData_Edit = UpdateStatus_ChangeEditTableStatus(updateInfo, dbData_Edit, ref list_SendMailInfo);

                                    // 若編輯表儲存成功
                                    if (list_SendMailInfo[list_SendMailInfo.Count() - 1].IsSuccess)
                                    {
                                        // 更新正式表資料
                                        UpdateStatus_SaveInfoTableData(dbData_Edit, ref list_SendMailInfo);
                                    }

                                    break;
                                }

                            case Models.ManufacturerUpdateStatusCommand.Decline:
                                {
                                    // 檢查正式表內是否有資料
                                    if (UpdateStatus_IsDataExistInfoTable(dbData_Edit.ManufactureURL))
                                    {
                                        // 有資料，將正式表資料寫回編輯表
                                        UpdateStatus_RollBackEditTable(dbData_Edit, ref list_SendMailInfo);
                                    }
                                    else
                                    {
                                        // 沒資料，僅更新編輯表狀態
                                        UpdateStatus_ChangeEditTableStatus(updateInfo, dbData_Edit, ref list_SendMailInfo);
                                    }

                                    break;
                                }

                            case Models.ManufacturerUpdateStatusCommand.Delete:
                                {
                                    // 更新編輯表狀態
                                    UpdateStatus_ChangeEditTableStatus(updateInfo, dbData_Edit, ref list_SendMailInfo);

                                    // 若編輯表儲存成功且正式表內有資料
                                    if (list_SendMailInfo[list_SendMailInfo.Count() - 1].IsSuccess && UpdateStatus_IsDataExistInfoTable(dbData_Edit.ManufactureURL))
                                    {
                                        // 更新正式表的狀態
                                        UpdateStatus_ChangeInfoTableStatus(updateInfo, updateData.ManufactureURL, ref list_SendMailInfo);
                                    }

                                    break;
                                }
                        }

                        // 更新正式表資料成功 TransactionScope 結束
                        if (list_SendMailInfo[list_SendMailInfo.Count() - 1].IsSuccess)
                        {
                            scope.Complete();
                        }
                    }

                    // 更新台蛋製造商資料
                    editTWServive.UpdateTWManufacture(dbData_Edit.ManufactureURL);

                    // 寄出審核結果通知信
                    SendManufactureEmail(list_SendMailInfo[list_SendMailInfo.Count() - 1], updateInfo.ChineseResult);
                }

                // 記錄審核訊息
                if (list_SendMailInfo[list_SendMailInfo.Count() - 1].IsSuccess)
                {
                    // 記錄審核成功訊息
                    tempReslultMessage.SuccessMessage = LogMessage(tempReslultMessage.SuccessMessage, updateData.ManufactureName);
                }
                else
                {
                    // 記錄審核失敗訊息
                    tempReslultMessage.ErrorMessage = LogMessage(tempReslultMessage.ErrorMessage, updateData.ManufactureName);
                }
            }

            // 如果清單中的所有項目都審核項目都成功，才會顯示成功訊息，否則就算失敗
            if (list_SendMailInfo.Any(x => x.IsSuccess == false))
            {
                jsonResult.Code = (int)ResponseCode.Error;
                jsonResult.IsSuccess = false;
            }
            else
            {
                jsonResult.Code = (int)ResponseCode.Success;
                jsonResult.IsSuccess = true;
            }

            //將成功、失敗的訊息合併
            jsonResult.Msg = string.Format(
                "審核成功：{0}; 審核失敗：{1}",
                string.IsNullOrEmpty(tempReslultMessage.SuccessMessage) ? "無" : tempReslultMessage.SuccessMessage,
                string.IsNullOrEmpty(tempReslultMessage.ErrorMessage) ? "無" : tempReslultMessage.ErrorMessage);
        }

        /// <summary>
        /// 更新編輯表狀態
        /// </summary>
        /// <param name="updateInfo">審核資訊</param>
        /// <param name="dbData_Edit">編輯表資料</param>
        /// <param name="list_SendMailInfo">審核結果通知資訊列表</param>
        /// <returns>更新後的編輯表資料</returns>
        private DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo_Edit UpdateStatus_ChangeEditTableStatus(Models.ManufacturerUpdateStatusInfo updateInfo, DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo_Edit dbData_Edit, ref List<Models.ManufacturerUpdateStatusSendMailInfo> list_SendMailInfo)
        {
            try
            {
                // 記錄訊息
                log.Info(string.Format(
                    "開始更新編輯表狀態，審核指令：{2}，製造商名稱：{0}，製造商網址：{1}。",
                    dbData_Edit.ManufactureName,
                    dbData_Edit.ManufactureURL,
                    updateInfo.Command));

                // 更改審核值、更新日期、更新人ID、拒絕原因
                dbData_Edit.ManufactureStatus = updateInfo.StatusValue;
                dbData_Edit.UpdateDate = DateTime.UtcNow.AddHours(8);
                dbData_Edit.UpdateUserID = updateInfo.UpdateUserID;

                //更新編輯表
                db.Entry(dbData_Edit).State = System.Data.EntityState.Modified;
                db.SaveChanges();

                // 儲存完成，將審核結果設為 true
                list_SendMailInfo[list_SendMailInfo.Count() - 1].IsSuccess = true;

                // 記錄訊息
                log.Info(string.Format(
                    "更新編輯表狀態成功，製造商名稱：{0}，製造商網址：{1}。",
                    dbData_Edit.ManufactureName,
                    dbData_Edit.ManufactureURL));
            }
            catch (Exception ex)
            {
                // 若儲存發生問題，將審核結果設為 false ，並記錄審核失敗原因
                list_SendMailInfo[list_SendMailInfo.Count() - 1].IsSuccess = false;
                list_SendMailInfo[list_SendMailInfo.Count() - 1].ErrorMessage = API.Models.ManufacturerUpdateStatusErrorType.SaveError;

                // 記錄訊息
                log.Info(string.Format("更新編輯表狀態失敗，失敗原因：{0}。", ex.Message));
            }

            return dbData_Edit;
        }

        /// <summary>
        /// 更新正式表狀態
        /// </summary>
        /// <param name="updateInfo">審核資訊</param>
        /// <param name="manufacturerURL">製造商網址</param>
        /// <param name="list_SendMailInfo">審核結果通知資訊列表</param>
        private void UpdateStatus_ChangeInfoTableStatus(Models.ManufacturerUpdateStatusInfo updateInfo, string manufacturerURL, ref List<Models.ManufacturerUpdateStatusSendMailInfo> list_SendMailInfo)
        {
            try
            {
                // 讀取正式表資料
                DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo dbData = db.Seller_ManufactureInfo.Where(x => x.ManufactureURL == manufacturerURL).FirstOrDefault();

                // 記錄訊息
                log.Info(string.Format(
                    "開始更新正式表狀態，審核指令：{2}，製造商名稱：{0}，製造商網址：{1}。",
                    dbData.ManufactureName,
                    dbData.ManufactureURL,
                    updateInfo.Command));

                // 更改審核值、更新日期、更新人ID
                dbData.ManufactureStatus = updateInfo.StatusValue;
                dbData.UpdateDate = DateTime.UtcNow.AddHours(8);
                dbData.UpdateUserID = updateInfo.UpdateUserID;
                dbData.DeclineReason = updateInfo.UpdateList.Where(x => x.ManufactureURL == manufacturerURL).Select(x => x.DeclineReason).FirstOrDefault();

                //更新編輯表
                db.Entry(dbData).State = System.Data.EntityState.Modified;
                db.SaveChanges();

                // 儲存完成，將審核結果設為 true
                list_SendMailInfo[list_SendMailInfo.Count() - 1].IsSuccess = true;

                // 記錄訊息
                log.Info(string.Format(
                    "更新正式表狀態成功，製造商名稱：{0}，製造商網址：{1}。",
                    dbData.ManufactureName,
                    dbData.ManufactureURL));
            }
            catch (Exception ex)
            {
                // 若儲存發生問題，將審核結果設為 false ，並記錄審核失敗原因
                list_SendMailInfo[list_SendMailInfo.Count() - 1].IsSuccess = false;
                list_SendMailInfo[list_SendMailInfo.Count() - 1].ErrorMessage = API.Models.ManufacturerUpdateStatusErrorType.SaveError;

                // 記錄訊息
                log.Info(string.Format("更新正式表狀態失敗，失敗原因：{0}。", ex.Message));
            }
        }

        /// <summary>
        /// 檢查正式表內是否有資料
        /// </summary>
        /// <param name="manufactureURL">製造商網址</param>
        /// <returns>true：有資料，false：無資料</returns>
        private bool UpdateStatus_IsDataExistInfoTable(string manufactureURL)
        {
            return db.Seller_ManufactureInfo.Any(x => x.ManufactureURL == manufactureURL);
        }

        /// <summary>
        /// 判斷資料是否有改值
        /// </summary>
        /// <param name="dbData">正式表資料</param>
        /// <param name="dbData_Edit">編輯表資料</param>
        /// <returns>true：沒有改變，false：有改變</returns>
        private bool UpdateStatus_IsNoDataChange(DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo dbData, DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo_Edit dbData_Edit)
        {
            return dbData.ManufactureName == dbData_Edit.ManufactureName
                && dbData.SupportEmail == dbData_Edit.SupportEmail
                && dbData.PhoneRegion == dbData_Edit.PhoneRegion
                && dbData.Phone == dbData_Edit.Phone
                && dbData.PhoneExt == dbData_Edit.PhoneExt
                && dbData.supportURL == dbData_Edit.supportURL;
        }

        /// <summary>
        /// 更新正式表資料
        /// </summary>
        /// <param name="dbData_Edit">編輯表資料</param>
        /// <param name="list_SendMailInfo">審核結果通知信資訊列表</param>
        private void UpdateStatus_SaveInfoTableData(DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo_Edit dbData_Edit, ref List<Models.ManufacturerUpdateStatusSendMailInfo> list_SendMailInfo)
        {
            try
            {
                // 記錄訊息
                log.Info(string.Format(
                    "開始更新正式表資料，審核指令：{2}，製造商名稱：{0}，製造商網址：{1}。",
                    dbData_Edit.ManufactureName,
                    dbData_Edit.ManufactureURL,
                    dbData_Edit.ManufactureStatus));

                // 正式表資料
                DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo dbData = new DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo();

                // 檢查正式表內是否有資料
                if (UpdateStatus_IsDataExistInfoTable(dbData_Edit.ManufactureURL))
                {
                    // 若有資料，先讀入正式表內原有資料，再更新至正式表
                    dbData = db.Seller_ManufactureInfo.Where(x => x.ManufactureURL == dbData_Edit.ManufactureURL).FirstOrDefault();

                    // 更新使用者輸入資料
                    dbData.ManufactureName = dbData_Edit.ManufactureName;
                    dbData.SupportEmail = dbData_Edit.SupportEmail;
                    dbData.PhoneRegion = dbData_Edit.PhoneRegion;
                    dbData.Phone = dbData_Edit.Phone;
                    dbData.PhoneExt = dbData_Edit.PhoneExt;
                    dbData.supportURL = dbData_Edit.supportURL;

                    // 更新審核值、更新日期、更新人ID
                    dbData.ManufactureStatus = dbData_Edit.ManufactureStatus;
                    dbData.UpdateDate = DateTime.UtcNow.AddHours(8);
                    dbData.UpdateUserID = dbData_Edit.UpdateUserID;

                    db.Entry(dbData).State = System.Data.EntityState.Modified;
                }
                else
                {
                    // 若無資料，則先使用 AutoMapper 將編輯表資料格式，轉為正式表資料格式，再新增至正式表
                    AutoMapper.Mapper.CreateMap<DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo_Edit, DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo>();
                    dbData = AutoMapper.Mapper.Map(dbData_Edit, dbData);

                    db.Seller_ManufactureInfo.Add(dbData);
                }

                //更新正式表
                db.SaveChanges();

                // 儲存完成，將審核結果設為 true
                list_SendMailInfo[list_SendMailInfo.Count() - 1].IsSuccess = true;

                // 記錄訊息
                log.Info(string.Format(
                    "更新正式表資料成功，製造商名稱：{0}，製造商網址：{1}。",
                    dbData.ManufactureName,
                    dbData.ManufactureURL));
            }
            catch (Exception ex)
            {
                // 若儲存發生問題，將審核結果設為 false ，並記錄審核失敗原因
                list_SendMailInfo[list_SendMailInfo.Count() - 1].IsSuccess = false;
                list_SendMailInfo[list_SendMailInfo.Count() - 1].ErrorMessage = API.Models.ManufacturerUpdateStatusErrorType.SaveError;

                // 記錄訊息
                log.Info(string.Format("更新正式表資料失敗，失敗原因：{0}。", ex.Message));
            }
        }

        /// <summary>
        /// 儲存審核結果通知信資訊
        /// </summary>
        /// <remarks>審核前先將修改人資訊記錄下來，以利 mail 發送；否則審核後，修改人資訊會變成審核人的</remarks>
        /// <param name="dbData_Edit">待審核資訊</param>
        /// <param name="list_SendMailInfo">審核結果通知信資訊列表</param>
        private void UpdateStatus_SaveMailInfo(DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo_Edit dbData_Edit, ref List<Models.ManufacturerUpdateStatusSendMailInfo> list_SendMailInfo)
        {
            // 判斷待審核資訊是否讀取成功
            if (null != dbData_Edit)
            {
                // 讀取成功，儲存審核結果通知信會用到的相關資訊
                list_SendMailInfo.Add(new Models.ManufacturerUpdateStatusSendMailInfo
                {
                    ManufactureURL = dbData_Edit.ManufactureURL,
                    ManufactureName = dbData_Edit.ManufactureName,
                    DeclineReason = dbData_Edit.DeclineReason,
                    SendType = UpdateStatus_TellMailSendType(dbData_Edit),
                    EditUser = dbData_Edit.UpdateUserID.Value,
                    DataOwner = (0 == dbData_Edit.UserID) ? dbData_Edit.InUserID.Value : dbData_Edit.UserID.Value,
                    // 先將 IsSuccess 設為 false; 等審核成功會再將 IsSuccess 改為 ture
                    IsSuccess = false
                });
            }
            else
            {
                // 讀取失敗，記錄失敗訊息
                list_SendMailInfo.Add(new Models.ManufacturerUpdateStatusSendMailInfo
                {
                    ManufactureURL = dbData_Edit.ManufactureURL,
                    ManufactureName = dbData_Edit.ManufactureName,
                    IsSuccess = false,
                    ErrorMessage = Models.ManufacturerUpdateStatusErrorType.NotFindData
                });
            }
        }

        /// <summary>
        /// 判斷寄信模式
        /// </summary>
        /// <param name="dbData_Edit">待審核資訊</param>
        /// <returns>寄信模式</returns>
        private API.Models.ManufacturerUpdateStatusSendType UpdateStatus_TellMailSendType(DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo_Edit dbData_Edit)
        {
            API.Models.ManufacturerUpdateStatusSendType? mailSendType;
            DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo dbData = db.Seller_ManufactureInfo.Where(x => x.ManufactureURL == dbData_Edit.ManufactureURL).FirstOrDefault();

            // 若正式表裡沒有相符的資料就是 Create(建立)
            if (dbData == null)
            {
                mailSendType = Models.ManufacturerUpdateStatusSendType.Create;
            }
            else
            {
                // 若正式表裡有相符的資料，而且：
                // 1.沒有更動資料就是 StatusSwitch(只切換製造商狀態)
                // 2.更新人和資料建立人id相同就是 Edit(建立者編輯)
                // 3.更新人和資料建立人id不相同就是 BeModified(非建立者編輯)
                if (null != dbData)
                {
                    if (UpdateStatus_IsNoDataChange(dbData, dbData_Edit))
                    {
                        mailSendType = API.Models.ManufacturerUpdateStatusSendType.StatusSwitch;
                    }
                    else if (dbData_Edit.UpdateUserID == dbData_Edit.UserID)
                    {
                        mailSendType = API.Models.ManufacturerUpdateStatusSendType.Edit;
                    }
                    else
                    {
                        mailSendType = API.Models.ManufacturerUpdateStatusSendType.Modified;
                    }
                }
                else
                {
                    mailSendType = null;
                }
            }

            return mailSendType.Value;
        }

        /// <summary>
        /// 復原編輯表資料
        /// </summary>
        /// <param name="dbData_Edit">編輯表資料</param>
        /// <param name="list_SendMailInfo">審核結果通知資訊列表</param>
        private void UpdateStatus_RollBackEditTable(DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo_Edit dbData_Edit, ref List<Models.ManufacturerUpdateStatusSendMailInfo> list_SendMailInfo)
        {
            try
            {
                DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo dbData = db.Seller_ManufactureInfo.Where(x => x.ManufactureURL == dbData_Edit.ManufactureURL).FirstOrDefault();

                // 記錄訊息
                log.Info(string.Format(
                    "開始復原編輯表，製造商名稱：{0}，製造商網址：{1}。",
                    dbData_Edit.ManufactureName,
                    dbData_Edit.ManufactureURL));

                // 復原輸入內容
                dbData_Edit.ManufactureName = dbData.ManufactureName;
                dbData_Edit.SupportEmail = dbData.SupportEmail;
                dbData_Edit.PhoneRegion = dbData.PhoneRegion;
                dbData_Edit.Phone = dbData.Phone;
                dbData_Edit.PhoneExt = dbData.PhoneExt;
                dbData_Edit.supportURL = dbData.supportURL;

                // 復原審核值、更新日期、更新人ID
                dbData_Edit.ManufactureStatus = dbData.ManufactureStatus;
                dbData_Edit.UpdateUserID = dbData.UpdateUserID;
                dbData_Edit.UpdateDate = dbData.UpdateDate;

                db.Entry(dbData_Edit).State = System.Data.EntityState.Modified;

                // 儲存編輯表
                db.SaveChanges();

                // 儲存完成，將審核結果設為 true
                list_SendMailInfo[list_SendMailInfo.Count() - 1].IsSuccess = true;

                // 記錄訊息
                log.Info(string.Format(
                    "復原編輯表成功，製造商名稱：{0}，製造商網址：{1}。",
                    dbData_Edit.ManufactureName,
                    dbData_Edit.ManufactureURL));
            }
            catch (Exception ex)
            {
                // 若儲存發生問題，將審核結果設為 false ，並記錄審核失敗原因
                list_SendMailInfo[list_SendMailInfo.Count() - 1].IsSuccess = false;
                list_SendMailInfo[list_SendMailInfo.Count() - 1].ErrorMessage = API.Models.ManufacturerUpdateStatusErrorType.SaveError;

                // 記錄訊息
                log.Info(string.Format("復原編輯表失敗，失敗原因：{0}。", ex.Message));
            }
        }
        #endregion UpdateStatus

        #region Validate
        /// <summary>
        /// 驗證輸入資料
        /// </summary>
        /// <param name="model_InputData">待驗證資料</param>
        /// <returns>Success：通過驗證，Error：未通過驗證 (只要一項未通過就會回傳 Error)</returns>
        private API.Models.ManufacturerValidateSummaryResult ValidateInputData(API.Models.Manufacturer model_InputData)
        {
            // 驗證輸入項目的結果暫存空間
            API.Models.ManufacturerValidateInfo validateInfo = new Models.ManufacturerValidateInfo();

            // 驗證製造商名稱
            // 驗證內容：不得為空值
            if (!string.IsNullOrEmpty(model_InputData.ManufactureName))
            {
                validateInfo.ManufactureName = true;
            }
            else
            {
                validateInfo.ManufactureName = false;
            }

            // 驗證製造商支援信箱
            // 驗證內容：有值，才驗證是否符合電子信箱格式
            if (string.IsNullOrEmpty(model_InputData.SupportEmail))
            {
                // 若沒值，不驗證，直接通過
                validateInfo.SupportEmail = true;
            }
            else
            {
                // 若有值，驗證是否符合電子信箱格式
                if (ValidateEmail(model_InputData.SupportEmail))
                {
                    validateInfo.SupportEmail = true;
                }
                else
                {
                    validateInfo.SupportEmail = false;
                }
            }

            // 驗證製造商支援網址
            // 驗證項目：有值，才驗證是否符合網址格式
            if (string.IsNullOrEmpty(model_InputData.supportURL))
            {
                // 若沒值，不驗證，直接通過
                validateInfo.supportURL = true;
            }
            else
            {
                // 若有值，驗證是否符合網址格式
                if (ValidateURL(model_InputData.supportURL))
                {
                    validateInfo.supportURL = true;
                }
                else
                {
                    validateInfo.supportURL = false;
                }
            }

            return validateInfo.SummaryResult;
        }

        /// <summary>
        /// 加上網址抬頭
        /// </summary>
        /// <param name="strURL">增加抬頭前的網址</param>
        /// <returns>增加抬頭後的網址</returns>
        private string AddURLTitle(string strURL)
        {
            return string.Format("http://{0}", strURL);
        }

        /// <summary>
        /// 檢查網址是否有抬頭
        /// </summary>
        /// <param name="strURL">被檢查網址</param>
        /// <returns>ture：有，false：沒有</returns>
        private bool CheckURLTitle(string strURL)
        {
            return (strURL.IndexOf(@"http://") == 0 || strURL.IndexOf(@"https://") == 0);
        }

        /// <summary>
        /// 檢查網址最後一個字元是否為斜線
        /// </summary>
        /// <param name="strURL">被檢查網址</param>
        /// <returns>ture：是斜線 false：不是斜線</returns>
        private bool CheckURLLastWordIsSlash(string strURL)
        {
            // 讀取製造商網址最後一個字
            string urlLastWord = strURL.Substring(strURL.Length - 1);

            return urlLastWord == "/";
        }

        /// <summary>
        /// 驗證是否符合電子信箱格式
        /// </summary>
        /// <param name="strEmail">被驗證電子信箱位址</param>
        /// <returns>ture：驗證成功，false：驗證失敗</returns>
        private bool ValidateEmail(string strEmail)
        {
            // 輸入不為空才進行電子信箱格式檢查
            if (!string.IsNullOrEmpty(strEmail))
            {
                // 指定電子信箱格式
                string strPattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

                return Regex.IsMatch(strEmail.Trim(), strPattern);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 驗證是否符合網址格式
        /// </summary>
        /// <param name="strURL">被驗證網址</param>
        /// <returns>ture：驗證成功，false：驗證失敗</returns>
        private bool ValidateURL(string strURL)
        {
            // 輸入不為空才進行網址格式檢查
            if (!string.IsNullOrEmpty(strURL))
            {
                // 指定網址格式
                string strPattern = @"^http[s]?://[\w-_.%/:?=&#]+$";

                return Regex.IsMatch(strURL.Trim(), strPattern);
            }
            else
            {
                return false;
            }
        }

        #endregion Validate
    }
}
