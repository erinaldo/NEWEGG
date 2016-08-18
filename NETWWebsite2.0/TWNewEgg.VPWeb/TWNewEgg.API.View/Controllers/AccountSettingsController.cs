using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.View.Attributes;
using TWNewEgg.API.Models;
using TWNewEgg.DB.TWSELLERPORTALDB.Models;
using AutoMapper;

namespace TWNewEgg.API.View.Controllers
{
    public class AccountSettingsController : Controller
    {
        TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        Connector conn = new Connector();
        TWNewEgg.API.View.Service.AES aes = new Service.AES();

        //
        // GET: /AccountSettings/
        [HttpGet]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.ManageAccount)]
        [FunctionName(FunctionNameAttributeValues.AccountSettings)]
        [ActiveKey(ActiveKeyAttributeValues.Menu)]
        [ActionDescription("帳戶設定")]
        [Filter.PermissionFilter]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.ManageAccount)]
        [FunctionName(FunctionNameAttributeValues.AccountSettings)]
        [ActiveKey(ActiveKeyAttributeValues.Menu)]
        [ActionDescription("帳戶設定")]
        public ActionResult Index(string viewName)
        {
            switch (viewName.ToLower())
            {
                case "sellerinformation":
                    Seller_BasicInfo basicInfo = getSellerBasicInfo(sellerInfo.currentSellerID).Body;
                    ViewBag.basicInfo = basicInfo;
                    break;
                case "useraccountsettings":
                    try
                    {
                        //Seller_BasicInfo basicInfo1 = getSellerBasicInfo(sellerInfo.currentSellerID).Body;
                        TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.GetFunctionListResult>> _getFunctionListResult = new ActionResponse<List<GetFunctionListResult>>();
                        _getFunctionListResult = conn.GetFunctionList();

                        TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.GetUserListResult>> _getUserListResult = new ActionResponse<List<GetUserListResult>>();
                        _getUserListResult = conn.GetUserList("", "", sellerInfo.currentSellerID);

                        TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.GetPurviewResult>> _getPurviewResult = new ActionResponse<List<GetPurviewResult>>();
                        _getPurviewResult = conn.GetUserPurview("", "", sellerInfo.UserID, sellerInfo.AccountTypeCode);

                        //List<TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.GetPurviewResult>>> _listpur = new List<ActionResponse<List<GetPurviewResult>>>();
                        List<TWNewEgg.API.Models.GetPurviewResult> _listGetPurResult = new List<GetPurviewResult>();
                        foreach (var item in _getUserListResult.Body)
                        {
                            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.GetPurviewResult>> _getPurviewResultTemp = new ActionResponse<List<GetPurviewResult>>();
                            _getPurviewResultTemp = conn.GetUserPurview("", "", item.UserID, sellerInfo.AccountTypeCode);
                            foreach (var itemPurView in _getPurviewResultTemp.Body)
                            {
                                _listGetPurResult.Add(itemPurView);
                            }
                            //_listpur.Add(GetPurviewResult1);
                        }

                        var joinGetData = (from p in _getFunctionListResult.Body
                                           join q in _getPurviewResult.Body on p.FunctionID equals q.FunctionID
                                           select new TWNewEgg.API.View.AdminModel
                                           {
                                               CategotyID = p.CategotyID,
                                               CategotyName = p.CategotyName,
                                               FunctionID = p.FunctionID,
                                               FunctionName = p.FunctionName,
                                               Enable = q.Enable,
                                               PurviewType = q.PurviewType,
                                               UserEmail = q.UserEmail
                                           }).OrderBy(p => p.CategotyID).ThenBy(p => p.FunctionID).ToList();
                        string tempstr = "";
                        int i = 0;
                        foreach (var item in joinGetData)
                        {
                            i++;
                            tempstr = tempstr + ";" + (item.FunctionName.Split(';')[1].ToString());
                        }
                        ViewBag.FunctionNameList = tempstr;
                        ViewBag.FunctionListLength = i;
                        var FunctionCategoty = _getFunctionListResult.Body.Select(p => p.CategotyName).Distinct().ToList();
                        ViewBag.FunctionCategoty = FunctionCategoty;
                        ViewBag.FunctionListResult = _getFunctionListResult.Body;
                        ViewBag.UserList = _getUserListResult.Body;
                        ViewBag.basicInfo = joinGetData;
                        ViewBag.listGetPurResult = _listGetPurResult;
                    }
                    catch (Exception error)
                    {
                        logger.Error("/AccountSetting/Index useraccountsettings error: " + error.Message);
                    }
                    break;
                default:
                    break;

            }
            
            string result = RenderView(viewName);
            return Json(new { ViewHtml = result, IsSuccess = true, Msg = "" });
        }

        [HttpGet]
        public ActionResult AccountInformation()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AccountInformation(string viewName)
        {
            string result = string.Empty;
            string resultMsg = string.Empty;
            bool isSuccess = false;
            switch (viewName.ToLower())
            {
                case "businessinformation":
                    // 根據API:
                    // type = 0: 以SellerID查詢
                    // type = 1: 以SellerName查詢
                    var GetSellerInfo = conn.GetSeller_BasicInfo(sellerInfo.currentSellerID.ToString(), 0);
                    var GetCountryInfo = conn.GetRegionList();
                    List<TWNewEgg.API.Models.GetRegionListResult> countrySelectItemList = GetCountryInfo.Body;
                    DB.TWSELLERPORTALDB.Models.Seller_BasicInfo sellerBasicInfo = GetSellerInfo.Body;

                    //處理 地址解析
                    AddressParseController addCtl = new AddressParseController();
                    if (sellerBasicInfo.CountryCode == "TW")
                    {
                        if (string.IsNullOrWhiteSpace(sellerBasicInfo.SellerAddress))
                        {
                            sellerBasicInfo.SellerAddress = "地址為空，隨意存值避免檢測當掉";
                        }
                        var addrInfo = addCtl.ParseAddress(sellerBasicInfo.City, sellerBasicInfo.SellerAddress, "", sellerBasicInfo.Zipcode);
                        ViewBag.addrInfo = addrInfo;
                    }

                    if (sellerBasicInfo.ComCountryCode == "TW")
                    {
                        if (string.IsNullOrWhiteSpace(sellerBasicInfo.ComSellerAdd))
                        {
                            sellerBasicInfo.ComSellerAdd = "地址為空，隨意存值避免檢測當掉";
                        }
                        var addrInfo_Com = addCtl.ParseAddress(sellerBasicInfo.ComCity, sellerBasicInfo.ComSellerAdd, "", sellerBasicInfo.ComZipcode);
                        ViewBag.addrInfo_bis = addrInfo_Com;
                    }

                    ViewBag.countrySelectItemList = countrySelectItemList;
                    ViewBag.sellerBasicInfo = sellerBasicInfo;
                    #region 利用 ViewBag 回傳廠商身分別和付款方式
                    try
                    {
                        //判斷列舉的值是否存在對應
                        bool isExist = Enum.IsDefined(typeof(TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo.IdentyType), sellerBasicInfo.Identy);
                        if (isExist == true)
                        {
                            var IdentyViewBag = (TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo.IdentyType)Enum.ToObject(typeof(TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo.IdentyType), sellerBasicInfo.Identy);
                            ViewBag.IdentyViewBag = IdentyViewBag.ToString();
                            string IdentyAES = aes.AesEncrypt(sellerBasicInfo.Identy.ToString());
                            ViewBag.IdentyAES = IdentyAES;
                        }
                        else
                        {
                            ViewBag.IdentyViewBag = "無資料，請洽詢客服更新資料";
                            ViewBag.IdentyAES = string.Empty;
                        }
                        
                    }
                    catch (Exception error)
                    {
                        ViewBag.IdentyViewBag = "無資料，請洽詢客服更新資料";
                        logger.Error("[Message]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
                    }
                    try
                    {
                        //判斷列舉的值是否存在對應
                        bool isExist = Enum.IsDefined(typeof(TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo.BillingCycleType), sellerBasicInfo.BillingCycle);
                        if (isExist == true)
                        {
                            var BillingCycleViewBag = (TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo.BillingCycleType)Enum.ToObject(typeof(TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo.BillingCycleType), sellerBasicInfo.BillingCycle);
                            ViewBag.BillingViewBag = BillingCycleViewBag.ToString();
                        }
                        else
                        {
                            ViewBag.BillingViewBag = "無資料，請洽詢客服更新資料";
                        }
                        
                    }
                    catch (Exception error)
                    {
                        ViewBag.BillingViewBag = "無資料，請洽詢客服更新資料";
                        logger.Error("[Message]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
                    }
                    
                    #endregion
                    isSuccess = (GetSellerInfo.IsSuccess && GetCountryInfo.IsSuccess);
                    break;
                case "financialinformation":
                    SellerFinancialModels sellerFinancialModels = GetFinancialInformation();
                    ViewBag.SellerInfo = sellerFinancialModels.SellerFinancial;
                    ViewBag.GetRegionListResult = sellerFinancialModels.GetRegionListResultList;
                    isSuccess = true;
                    break;
                case "addressinformation":
                    break;
                case "notificationoptions":
                    ActionResponse<NotificationOptionsData> getNoticeData = getNotificationOptionsData();
                    isSuccess = getNoticeData.IsSuccess;
                    ViewBag.NotificationOptionsData = getNoticeData.Body;
                    break;
                default:
                    break;
            }

            result = RenderView(viewName);

            return Json(new { ViewHtml = result, IsSuccess = isSuccess, Msg = resultMsg });
        }


        #region 商業資訊

        /// <summary>
        /// 取得商家區域列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        public ActionResult GetCountryRegionList()
        {
            bool _isSuccess = false;
            string _msg = string.Empty;
            object _data = null;

            try
            {
                var getCountryInfo = conn.GetRegionList();

                _isSuccess = getCountryInfo.IsSuccess;
                _msg = getCountryInfo.Msg;
                if (_isSuccess)
                {
                    _data = getCountryInfo.Body;
                }
            }
            catch (Exception ex)
            {
                _msg += ex.Message;
            }


            return Json(_data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 帳戶管理--帳戶設定--商家資訊 主頁面/進入頁面
        /// (或 
        /// 帳戶管理--帳戶設定--帳戶資訊--商家資訊)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        public ActionResult BusinessInformation()
        {
            return View();
        }

        /// <summary>
        /// 儲存 商家資訊
        /// (帳戶管理--帳戶設定--商家資訊)
        /// (或
        /// 帳戶管理--帳戶設定--帳戶資訊--商家資訊)
        /// </summary>
        /// <param name="seller_basicPROinfo">傳入需儲存的model(商家資訊)</param>
        /// <remarks>請注意並非使用原資料庫 Seller_BasicInfo 的model (依據silverlight version)</remarks>
        /// <returns>是否儲存成功, 儲存成功訊息 或 失敗訊息失敗原因訊息,</returns>
        [HttpPost]
        [Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        public ActionResult SaveBusinessInformation(Seller_BasicProInfo sellerInfoToSave, string IdentyAES)
        {
            
            TWNewEgg.API.Models.Seller_BasicProInfo seller_BasicProInfo = sellerInfoToSave;
            string msg = string.Empty;
            bool isSuccess_result = false;
            string identy = aes.AesDecrypt(IdentyAES);

            // 驗證資料
            if (!IsValid_SellerBasicPROInfo(seller_BasicProInfo, out msg))
            {
                return Json(new { isSuccess = false, msg = msg });
            }

            // 補存入 登入者資訊
            seller_BasicProInfo.SellerID = sellerInfo.currentSellerID;
            seller_BasicProInfo.UpdateUserID = sellerInfo.UserID;
            if (seller_BasicProInfo.CountryCode == "TW")
            {
                seller_BasicProInfo.SellerAddress = sellerInfoToSave.SellerAddress;
            }
            if (seller_BasicProInfo.ComCountryCode == "TW")
            {
                seller_BasicProInfo.ComSellerAdd = sellerInfoToSave.ComSellerAdd;
            }

            // 連結至 API 儲存商家資訊
            try
            {
                // 儲存商家資訊
                var _result = conn.SaveSeller_BasicProInfo("999", "8lKxHarVfh68NTugxtSGFSHJQ3K352aCfdVRaFX+z2k=", seller_BasicProInfo);
                // 執行結果資訊
                isSuccess_result = _result.IsSuccess;
                msg += _result.Msg;
                if (_result.IsSuccess == true)
                {
                    Response.Cookies["CSD"].Value = aes.AesEncrypt(_result.Body.ToString());
                }
                else
                {
                    Response.Cookies["CSD"].Value = aes.AesEncrypt(sellerInfo.SellerID.ToString());
                }
                // 回傳執行結果訊息
                return Json(new { isSuccess = isSuccess_result, msg = msg });
            }
            catch (Exception ex)
            {
                // exception
                isSuccess_result = false;
                msg += "連線至connection失敗:exception:" + ex.Message;
                logger.Error(msg);
                Response.Cookies["CSD"].Value = aes.AesEncrypt(sellerInfo.SellerID.ToString());
                //throw;
                return Json(new { isSuccess = isSuccess_result, msg = msg });
            }
        }

        /// <summary>
        /// server端資料驗證
        /// </summary>
        /// <param name="seller_BasicProInfo"></param>
        /// <param name="validationResultMsg"></param>
        /// <returns>true/false</returns>
        private bool IsValid_SellerBasicPROInfo(TWNewEgg.API.Models.Seller_BasicProInfo seller_BasicProInfo, out string validationResultMsg)
        {
            validationResultMsg = string.Empty;

            if (string.IsNullOrEmpty(seller_BasicProInfo.SellerName))
            {
                validationResultMsg += "店家名稱必填!";
                return false;
            }
            if (string.IsNullOrEmpty(seller_BasicProInfo.SellerShortName))
            {
                validationResultMsg += "店家簡稱必填!";
                return false;
            }
            if (string.IsNullOrEmpty(seller_BasicProInfo.FirstName))
            {
                validationResultMsg += "負責人名字必填!";
                return false;
            }
            if (string.IsNullOrEmpty(seller_BasicProInfo.LastName))
            {
                validationResultMsg += "負責人姓氏必填!";
                return false;
            }

            if (string.IsNullOrEmpty(seller_BasicProInfo.CompanyCode))
            {
                validationResultMsg += "公司統編必填!";
                return false;
            }
            if (!string.IsNullOrEmpty(seller_BasicProInfo.Identy))
            {
                TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo.IdentyType identy = (TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo.IdentyType)Convert.ToInt32(seller_BasicProInfo.Identy);

                switch (identy)
                {
                    case Seller_BasicInfo.IdentyType.個人戶:
                        break;
                    case Seller_BasicInfo.IdentyType.國內廠商:
                        {
                            if (!string.IsNullOrEmpty(seller_BasicProInfo.CompanyCode))
                            {
                                int _int = 0;
                                if (!Int32.TryParse(seller_BasicProInfo.CompanyCode, out _int))
                                {
                                    validationResultMsg += "公司統編須為數字";
                                    return false;
                                }
                            }
                        }
                        break;
                    case Seller_BasicInfo.IdentyType.國外廠商:
                        break;
                    default:
                        break;
                }
            }

            //登記地址
            if (string.IsNullOrEmpty(seller_BasicProInfo.CountryCode))
            {
                validationResultMsg += "登記地址:國家/地區必填!";
                return false;
            }
            if (string.IsNullOrEmpty(seller_BasicProInfo.SellerState))
            {
                validationResultMsg += "登記地址:州 必填!";
                return false;
            }
            if (string.IsNullOrEmpty(seller_BasicProInfo.SellerAddress))
            {
                validationResultMsg += "登記地址:地址 必填!";
                return false;
            }
            if (string.IsNullOrEmpty(seller_BasicProInfo.City))
            {
                validationResultMsg += "登記地址:城市 必填!";
                return false;
            }
            if (string.IsNullOrEmpty(seller_BasicProInfo.Zipcode))
            {
                validationResultMsg += "登記地址:郵遞區號 必填!";
                return false;
            }
            else
            {
                System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"^([0-9]|[a-zA-Z]){0,10}$", System.Text.RegularExpressions.RegexOptions.Singleline);
                if (!reg.IsMatch(seller_BasicProInfo.Zipcode))
                {
                    validationResultMsg += "登記地址:郵遞區號 只能為10碼以內 英/數字 !";
                    return false;
                }
            }
            if (!string.IsNullOrEmpty(seller_BasicProInfo.PhoneRegion)
                | !string.IsNullOrEmpty(seller_BasicProInfo.Phone)
                | !string.IsNullOrEmpty(seller_BasicProInfo.PhoneExt))
            {
                //電話號碼非必填
                //若輸入電話號碼，區碼+號碼+分機 總長度需小於14碼 (TWSQLDB seller 長度限制) >> 2015/05/19:討論,決定區碼<=3,號碼<=14,分機<=6
                //if (((seller_BasicProInfo.PhoneRegion??"").Length + (seller_BasicProInfo.Phone??"").Trim().Length + (seller_BasicProInfo.PhoneExt??"").Trim().Length) > 14)
                //{
                //    validationResultMsg += "電話號碼過長! 請修改!(合計小於14碼)";
                //    return false;
                //}

                //加入註解:2015/05/19:討論,決定區碼<=3,號碼<=14,分機<=6
                System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"^([0-9]){0,}$", System.Text.RegularExpressions.RegexOptions.Singleline);
                if (!reg.IsMatch((seller_BasicProInfo.PhoneRegion ?? "")))
                {
                    validationResultMsg += "電話號碼 區碼有誤! 請輸入數字";
                    return false;
                }
                if (!reg.IsMatch((seller_BasicProInfo.Phone ?? "")))
                {
                    validationResultMsg += "電話號碼有誤! 請輸入數字";
                    return false;
                }
                if (!reg.IsMatch((seller_BasicProInfo.PhoneExt ?? "")))
                {
                    validationResultMsg += "電話號碼 分機有誤! 請輸入數字";
                    return false;
                }

                //判斷電話號碼長度
                if ((seller_BasicProInfo.PhoneRegion ?? "").Length > 3)
                {
                    validationResultMsg += "電話號碼 區碼太長! 請輸入3個數字以下";
                    return false;
                }
                if ((seller_BasicProInfo.Phone ?? "").Length > 14)
                {
                    validationResultMsg += "電話號碼太長! 請輸入14個數字以下";
                    return false;
                }
                if ((seller_BasicProInfo.PhoneExt ?? "").Length > 6)
                {
                    validationResultMsg += "電話區域號碼有誤! 請輸入6個數字以下";
                    return false;
                }
            }

            //營業地址
            if (string.IsNullOrEmpty(seller_BasicProInfo.ComCountryCode))
            {
                validationResultMsg += "營業地址:國家/地區必填!";
                return false;
            }
            if (string.IsNullOrEmpty(seller_BasicProInfo.ComSellerState))
            {
                validationResultMsg += "營業地址:州 必填!";
                return false;
            }
            if (string.IsNullOrEmpty(seller_BasicProInfo.ComSellerAdd))
            {
                validationResultMsg += "營業地址:地址 必填!";
                return false;
            }
            if (string.IsNullOrEmpty(seller_BasicProInfo.ComCity))
            {
                validationResultMsg += "營業地址:城市 必填!";
                return false;
            }
            if (string.IsNullOrEmpty(seller_BasicProInfo.ComZipcode))
            {
                validationResultMsg += "營業地址:郵遞區號 必填!";
                return false;
            }
            else
            {
                System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"^([0-9]|[a-zA-Z]){0,10}$", System.Text.RegularExpressions.RegexOptions.Singleline);
                if (!reg.IsMatch(seller_BasicProInfo.ComZipcode))
                {
                    validationResultMsg += "營業地址:郵遞區號 只能為10碼以內 英/數字!";
                    return false;
                }
            }

            return true;
        }
        #endregion

        /// <summary>
        /// 搜尋NotificationOptions資料
        /// </summary>
        /// <returns>返回搜尋的NotificationOptions結果</returns>
        public ActionResponse<NotificationOptionsData> getNotificationOptionsData()
        {
            ActionResponse<NotificationOptionsData> getNoticeData = new ActionResponse<NotificationOptionsData>();
            Connector conn = new Connector();

            ActionResponse<List<Seller_Notification>> searchSellerNotification = new ActionResponse<List<Seller_Notification>>();
            try
            {
                searchSellerNotification = conn.GetSeller_Notification(sellerInfo.currentSellerID.ToString(), 0);
            }
            catch (Exception e)
            {
                logger.Error("getNotificationOptionsData Fail:[ErrorMessage] " + e.Message + " [ErrorStackTrace] " + e.StackTrace);
            }

            try
            {
                #region 查詢資料轉存
                getNoticeData = searchNotificationOptionsMapper(searchSellerNotification);
                #endregion
                getNoticeData.IsSuccess = true;
            }
            catch (Exception e)
            {
                getNoticeData.IsSuccess = false;
                logger.Error("getNotificationOptionsData 查詢資料轉存 Fail:[ErrorMessage] " + e.Message + " [ErrorStackTrace] " + e.StackTrace);
            }

            return getNoticeData;
        }

        /// <summary>
        /// NotificationOptions查詢資料轉存
        /// </summary>
        /// <param name="searchSellerNotification">需轉存的查詢資料</param>
        /// <returns>返回轉存後結果</returns>
        public ActionResponse<NotificationOptionsData> searchNotificationOptionsMapper(ActionResponse<List<Seller_Notification>> searchSellerNotification)
        {
            ActionResponse<NotificationOptionsData> getNoticeData = new ActionResponse<NotificationOptionsData>();
            getNoticeData.Body = new NotificationOptionsData();
            if (searchSellerNotification.Body != null)
            {
                getNoticeData.Body.SellerID = searchSellerNotification.Body.FirstOrDefault().SellerID;
                getNoticeData.Body.InUserID = searchSellerNotification.Body.FirstOrDefault().InUserID;

                foreach (Seller_Notification sellerNotification in searchSellerNotification.Body)
                {
                    if (sellerNotification.NotificationTypeCode.ToLower() == "on")
                    {
                        if (sellerNotification.Enabled.ToLower() == "y")
                        {
                            getNoticeData.Body.orderNoticeOpen = true;
                        }
                        else
                        {
                            getNoticeData.Body.orderNoticeClose = true;
                        }
                        getNoticeData.Body.orderNoticeSN = sellerNotification.SN;
                        getNoticeData.Body.orderNoticeEmail_1st = sellerNotification.EmailAddress1;
                        getNoticeData.Body.orderNoticeEmail_2nd = sellerNotification.EmailAddress2;
                        getNoticeData.Body.orderNoticeEmail_3rd = sellerNotification.EmailAddress3;
                    }
                    else if (sellerNotification.NotificationTypeCode.ToLower() == "von")
                    {
                        if (sellerNotification.Enabled.ToLower() == "y")
                        {
                            getNoticeData.Body.cancelNoticeOpen = true;
                        }
                        else
                        {
                            getNoticeData.Body.cancelNoticeClose = true;
                        }
                        getNoticeData.Body.cancelNoticeSN = sellerNotification.SN;
                        getNoticeData.Body.cancelNoticeEmail_1st = sellerNotification.EmailAddress1;
                        getNoticeData.Body.cancelNoticeEmail_2nd = sellerNotification.EmailAddress2;
                        getNoticeData.Body.cancelNoticeEmail_3rd = sellerNotification.EmailAddress3;
                    }
                    else if (sellerNotification.NotificationTypeCode.ToLower() == "bn")
                    {
                        getNoticeData.Body.businessNoticeSN = sellerNotification.SN;
                        getNoticeData.Body.businessNoticeEmail_1st = sellerNotification.EmailAddress1;
                        getNoticeData.Body.businessNoticeEmail_2nd = sellerNotification.EmailAddress2;
                        getNoticeData.Body.businessNoticeEmail_3rd = sellerNotification.EmailAddress3;
                    }
                    else if (sellerNotification.NotificationTypeCode.ToLower() == "fn")
                    {
                        getNoticeData.Body.financialNoticeSN = sellerNotification.SN;
                        getNoticeData.Body.financialNoticeEmail_1st = sellerNotification.EmailAddress1;
                        getNoticeData.Body.financialNoticeEmail_2nd = sellerNotification.EmailAddress2;
                        getNoticeData.Body.financialNoticeEmail_3rd = sellerNotification.EmailAddress3;
                    }
                    else if (sellerNotification.NotificationTypeCode.ToLower() == "rma")
                    {
                        if (sellerNotification.Enabled.ToLower() == "y")
                        {
                            getNoticeData.Body.returnsNoticeOpen = true;
                        }
                        else
                        {
                            getNoticeData.Body.returnsNoticeClose = true;
                        }
                        getNoticeData.Body.returnsNoticeSN = sellerNotification.SN;
                        getNoticeData.Body.returnsNoticeEmail_1st = sellerNotification.EmailAddress1;
                        getNoticeData.Body.returnsNoticeEmail_2nd = sellerNotification.EmailAddress2;
                        getNoticeData.Body.returnsNoticeEmail_3rd = sellerNotification.EmailAddress3;
                    }
                }
            }

            return getNoticeData;
        }

        /// <summary>
        /// NotificationOptions資料儲存與更新
        /// </summary>
        /// <param name="updateNoticeData"></param>
        /// <returns></returns>
        public JsonResult updateNotificationOptions(NotificationOptionsData updateNoticeData)
        {
            string msg = string.Empty;
            ActionResponse<List<string>> updateNotification = new ActionResponse<List<string>>();
            List<Seller_Notification> notificationList = new List<Seller_Notification>();
            Connector conn = new Connector();
            // 儲存資料轉存
            try
            {
                notificationList = updateNotificationOptionsMapper(updateNoticeData);
            }
            catch (Exception e)
            {
                logger.Error("updateNotificationOptions 儲存資料轉存 Fail:[ErrorMessage] " + e.Message + " [ErrorStackTrace] " + e.StackTrace);
            }
            // 資料更新
            try
            {
                updateNotification = conn.SaveSeller_Notification("", sellerInfo.AccessToken, notificationList);
            }
            catch (Exception e)
            {
                logger.Error("updateNotificationOptions 資料儲存失敗 Fail:[ErrorMessage] " + e.Message + " [ErrorStackTrace] " + e.StackTrace);
            }

            if (updateNotification.Body != null)
            {
                logger.Error("updateNotificationOptions 資料儲存成功 [Message] " + updateNotification.Body);
                return Json(new { IsSuccess = updateNotification.IsSuccess, Msg = "資料修改成功" });
            }
            else
            {
                return Json(new { IsSuccess = false, Msg = "資料儲存失敗" });
            }
        }

        /// <summary>
        /// NotificationOptions更新資料轉存
        /// </summary>
        /// <param name="updateNoticeData">需要轉存的Model</param>
        /// <returns>返回轉存後的NotificationOptions List</returns>
        public List<Seller_Notification> updateNotificationOptionsMapper(NotificationOptionsData updateNoticeData)
        {
            List<Seller_Notification> notificationList = new List<Seller_Notification>();
            #region ON
            Seller_Notification notice_ON = new Seller_Notification();
            notice_ON.NotificationTypeCode = "ON";
            notice_ON.SellerID = sellerInfo.currentSellerID;
            notice_ON.SN = updateNoticeData.orderNoticeSN;
            notice_ON.InUserID = sellerInfo.UserID;
            if (updateNoticeData.orderNoticeOpen == true && updateNoticeData.orderNoticeClose == false)
            {
                notice_ON.Enabled = "Y";
            }
            else
            {
                notice_ON.Enabled = "N";
            }

            notice_ON.EmailAddress1 = updateNoticeData.orderNoticeEmail_1st;
            notice_ON.EmailAddress2 = updateNoticeData.orderNoticeEmail_2nd;
            notice_ON.EmailAddress3 = updateNoticeData.orderNoticeEmail_3rd;
            #endregion ON
            #region VON
            Seller_Notification notice_VON = new Seller_Notification();
            notice_VON.NotificationTypeCode = "VON";
            notice_VON.SellerID = sellerInfo.currentSellerID;
            notice_VON.SN = updateNoticeData.cancelNoticeSN;
            notice_VON.InUserID = sellerInfo.UserID;
            if (updateNoticeData.cancelNoticeOpen == true && updateNoticeData.cancelNoticeClose == false)
            {
                notice_VON.Enabled = "Y";
            }
            else
            {
                notice_VON.Enabled = "N";
            }

            notice_VON.EmailAddress1 = updateNoticeData.cancelNoticeEmail_1st;
            notice_VON.EmailAddress2 = updateNoticeData.cancelNoticeEmail_2nd;
            notice_VON.EmailAddress3 = updateNoticeData.cancelNoticeEmail_3rd;
            #endregion VON
            #region BN
            Seller_Notification notice_BN = new Seller_Notification();
            notice_BN.NotificationTypeCode = "BN";
            notice_BN.SellerID = sellerInfo.currentSellerID;
            notice_BN.SN = updateNoticeData.businessNoticeSN;
            notice_BN.InUserID = sellerInfo.UserID;
            notice_BN.Enabled = "N";
            notice_BN.EmailAddress1 = updateNoticeData.businessNoticeEmail_1st;
            notice_BN.EmailAddress2 = updateNoticeData.businessNoticeEmail_2nd;
            notice_BN.EmailAddress3 = updateNoticeData.businessNoticeEmail_3rd;
            #endregion ON
            #region FN
            Seller_Notification notice_FN = new Seller_Notification();
            notice_FN.NotificationTypeCode = "FN";
            notice_FN.SellerID = sellerInfo.currentSellerID;
            notice_FN.SN = updateNoticeData.financialNoticeSN;
            notice_FN.InUserID = sellerInfo.UserID;
            notice_FN.Enabled = "N";
            notice_FN.EmailAddress1 = updateNoticeData.financialNoticeEmail_1st;
            notice_FN.EmailAddress2 = updateNoticeData.financialNoticeEmail_2nd;
            notice_FN.EmailAddress3 = updateNoticeData.financialNoticeEmail_3rd;
            #endregion FN
            #region RMA
            Seller_Notification notice_RMA = new Seller_Notification();
            notice_RMA.NotificationTypeCode = "RMA";
            notice_RMA.SellerID = sellerInfo.currentSellerID;
            notice_RMA.SN = updateNoticeData.returnsNoticeSN;
            notice_RMA.InUserID = sellerInfo.UserID;
            if (updateNoticeData.returnsNoticeOpen == true && updateNoticeData.returnsNoticeClose == false)
            {
                notice_RMA.Enabled = "Y";
            }
            else
            {
                notice_RMA.Enabled = "N";
            }

            notice_RMA.EmailAddress1 = updateNoticeData.returnsNoticeEmail_1st;
            notice_RMA.EmailAddress2 = updateNoticeData.returnsNoticeEmail_2nd;
            notice_RMA.EmailAddress3 = updateNoticeData.returnsNoticeEmail_3rd;
            #endregion RMA
            notificationList.Add(notice_ON);
            notificationList.Add(notice_VON);
            notificationList.Add(notice_BN);
            notificationList.Add(notice_FN);
            notificationList.Add(notice_RMA);

            return notificationList;
        }

        /// <summary>
        /// 將該View轉成string
        /// </summary>
        /// <param name="partialView">View的名稱</param>
        /// <returns>返回string</returns>
        public string RenderView(string partialView)
        {
            string result = string.Empty;
            try
            {
                using (StringWriter sw = new StringWriter())
                {
                    ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, partialView);
                    ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                    viewResult.View.Render(viewContext, sw);
                    result = sw.GetStringBuilder().ToString();
                }
            }
            catch (Exception e)
            {
                logger.Error("[ErrorMessage] 查無此頁 " + partialView);
                result = "";
            }

            return result;
        }

        #region 財務資訊頁
        /// <summary>
        /// 財務資訊頁
        /// </summary>
        /// <param name="BasicInfo">銀行與付款資訊</param>
        /// <returns></returns>
        public SellerFinancialModels GetFinancialInformation()
        {
            SellerFinancialModels sellerFinancialModels = new SellerFinancialModels();
            Connector Connect = new Connector();
            TWNewEgg.API.Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_Financial> GetFInfo = new ActionResponse<Seller_Financial>();
            TWNewEgg.API.Models.ActionResponse<List<GetRegionListResult>> GetRegionListResult = new ActionResponse<List<TWNewEgg.API.Models.GetRegionListResult>>();
            string errorMsg = string.Empty;

            try
            {
                sellerFinancialModels.SellerFinancial = Connect.GetSeller_Financial(sellerInfo.currentSellerID.ToString(), 0).Body;
                sellerFinancialModels.GetRegionListResultList = Connect.GetRegionList().Body;
            }
            catch (Exception e)
            {
                errorMsg = e.Message;
            }

            return sellerFinancialModels;
        }

        /// <summary>
        /// 儲存頁面上資訊
        /// </summary>
        /// <param name="UpdateInfo">要儲存的資料</param>
        /// <returns>成功/錯誤訊息</returns>
        [HttpPost]
        public JsonResult SaveFinancialInformation(Seller_Financial UpdateInfo)
        {
            Connector Connect = new Connector();
            Seller_Financial FinancialInfo = new Seller_Financial();
            TWNewEgg.API.Models.ActionResponse<string> Message = new TWNewEgg.API.Models.ActionResponse<string>();
            string strJavascript = string.Empty;

            #region 將字串中空白去掉

            // 銀行名稱
            if (!string.IsNullOrEmpty(UpdateInfo.BankName))
            {
                UpdateInfo.BankName = UpdateInfo.BankName.Replace(" ", "");
            }

            // 銀行代碼
            if (!string.IsNullOrEmpty(UpdateInfo.BankCode))
            {
                UpdateInfo.BankCode = UpdateInfo.BankCode.Replace(" ", "");
            }

            // 分行名稱
            if (!string.IsNullOrEmpty(UpdateInfo.BankBranchName))
            {
                UpdateInfo.BankBranchName = UpdateInfo.BankBranchName.Replace(" ", "");
            }

            // 分行代碼
            if (!string.IsNullOrEmpty(UpdateInfo.BankBranchCode))
            {
                UpdateInfo.BankBranchCode = UpdateInfo.BankBranchCode.Replace(" ", "");
            }

            // 銀行帳號
            if (!string.IsNullOrEmpty(UpdateInfo.BankAccountNumber))
            {
                UpdateInfo.BankAccountNumber = UpdateInfo.BankAccountNumber.Replace(" ", "");
            }

            // 帳戶名稱
            if (!string.IsNullOrEmpty(UpdateInfo.BeneficiaryName))
            {
                UpdateInfo.BeneficiaryName = UpdateInfo.BeneficiaryName.Replace(" ", "");
            }

            // 發票國家/區域
            if (!string.IsNullOrEmpty(UpdateInfo.BeneficiaryCountryCode))
            {
                UpdateInfo.BeneficiaryCountryCode = UpdateInfo.BeneficiaryCountryCode.Replace(" ", "");
            }

            // 發票州/省
            if (!string.IsNullOrEmpty(UpdateInfo.BeneficiaryState))
            {
                UpdateInfo.BeneficiaryState = UpdateInfo.BeneficiaryState.Replace(" ", "");
            }

            // 發票地址
            if (!string.IsNullOrEmpty(UpdateInfo.BeneficiaryAddress))
            {
                UpdateInfo.BeneficiaryAddress = UpdateInfo.BeneficiaryAddress.Replace(" ", "");
            }

            // 發票城市
            if (!string.IsNullOrEmpty(UpdateInfo.BeneficiaryCity))
            {
                UpdateInfo.BeneficiaryCity = UpdateInfo.BeneficiaryCity.Replace(" ", "");
            }

            // 發票郵遞區號
            if (!string.IsNullOrEmpty(UpdateInfo.BeneficiaryZipcode))
            {
                UpdateInfo.BeneficiaryZipcode = UpdateInfo.BeneficiaryZipcode.Replace(" ", "");
            }

            // 銀行所在州/省
            if (!string.IsNullOrEmpty(UpdateInfo.BankState))
            {
                UpdateInfo.BankState = UpdateInfo.BankState.Replace(" ", "");
            }

            // 銀行地址
            if (!string.IsNullOrEmpty(UpdateInfo.BankAddress))
            {
                UpdateInfo.BankAddress = UpdateInfo.BankAddress.Replace(" ", "");
            }

            // 銀行所在城市
            if (!string.IsNullOrEmpty(UpdateInfo.BankCity))
            {
                UpdateInfo.BankCity = UpdateInfo.BankCity.Replace(" ", ""); 
            }

            // 銀行郵遞區號
            if (!string.IsNullOrEmpty(UpdateInfo.BankZipCode))
            {
                UpdateInfo.BankZipCode = UpdateInfo.BankZipCode.Replace(" ", "");
            }

            #endregion 將字串中空白去掉

            /* 檢查字串 */
            strJavascript = checkIsEmpty(UpdateInfo);
            if (!string.IsNullOrEmpty(strJavascript))
            {
                return Json(new { SaveMessage = strJavascript });
            }

            UpdateInfo.SellerID = sellerInfo.currentSellerID;

            try
            {
                FinancialInfo = Connect.GetSeller_Financial(UpdateInfo.SellerID.ToString(), 0).Body;

                if (FinancialInfo == null)
                {
                    UpdateInfo.InUserID = sellerInfo.UserID;
                    UpdateInfo.UpdateUserID = sellerInfo.currentSellerID;
                }
                else
                {
                    UpdateInfo.UpdateUserID = sellerInfo.UserID;
                }
            
                Message = Connect.SaveSeller_Financial("", "", UpdateInfo);

                if (Message.IsSuccess)
                {
                    strJavascript = "資料更新成功。";
                    return Json(new { SaveMessage = strJavascript });
                }
                else
                {
                    return Json(new { SaveMessage = Message.Msg });
                }
            }
            catch (Exception e)
            {
                strJavascript = e.Message;
            }

            return Json(new { SaveMessage = "" });
        }

        /// <summary>
        /// 檢查必填欄位是否填值
        /// </summary>
        /// <param name="UpdateInfo">要檢查的UpdateInfo</param>
        /// <returns>回傳錯誤訊息，若是Empty則正確</returns>
        public string checkIsEmpty(Seller_Financial UpdateInfo)
        {
            string CheckIsCorrectResult = string.Empty;

            // SWIFT碼
            if (!string.IsNullOrEmpty(UpdateInfo.SWIFTCode) && UpdateInfo.SWIFTCode.Length > 50)
            {
                CheckIsCorrectResult += "SWIFT碼不可超過50個字。";

                if (!ISNumberAndAlphabet(UpdateInfo.BankAccountNumber))
                {
                    CheckIsCorrectResult += "SWIFT碼僅能為數字和英文字母。";
            }
            }

            // 銀行名稱
            if (!string.IsNullOrEmpty(UpdateInfo.BankName))
            {
                if (UpdateInfo.BankName.Length > 60)
                {
                    CheckIsCorrectResult += "銀行名稱不可超過60個字。";
                }
            }
            else
            {
                CheckIsCorrectResult += "銀行名稱為必填。";
            }

            // 銀行代碼
            if (!string.IsNullOrEmpty(UpdateInfo.BankCode))
            {
                if (UpdateInfo.BankCode.Length > 15)
                {
                    CheckIsCorrectResult += "銀行代碼不可超過15個字。";
                }

                if (!ISOnlyNumber(UpdateInfo.BankCode))
                {
                    CheckIsCorrectResult += "銀行代碼僅能為數字。";
                }
            }
            else
            {
                CheckIsCorrectResult += "銀行代碼為必填。";
            }

            // 分行名稱
            if (!string.IsNullOrEmpty(UpdateInfo.BankBranchName))
            {
                if (UpdateInfo.BankBranchName.Length > 60)
                {
                    CheckIsCorrectResult += "分行名稱不可超過60個字。";
                }
            }
            else
            {
                CheckIsCorrectResult += "分行名稱為必填。";
            }

            // 分行代碼
            if (!string.IsNullOrEmpty(UpdateInfo.BankBranchCode))
            {
                if (UpdateInfo.BankBranchCode.Length > 15)
                {
                    CheckIsCorrectResult += "分行代碼不可超過15個字。";
            }

                if (!ISOnlyNumber(UpdateInfo.BankBranchCode))
            {
                    CheckIsCorrectResult += "分行代碼僅能為數字。";
            }
            }
            else
            {
                CheckIsCorrectResult += "分行代碼為必填。";
            }

            // 銀行帳號
            if (!string.IsNullOrEmpty(UpdateInfo.BankAccountNumber))
            {
                if (UpdateInfo.BankAccountNumber.Length > 50)
                {
                    CheckIsCorrectResult += "銀行帳號不可超過50個字。";
                }

                if (!ISOnlyNumber(UpdateInfo.BankAccountNumber))
            {
                    CheckIsCorrectResult += "銀行帳號僅能為數字。";
            }
            }
            else
            {
                CheckIsCorrectResult += "銀行帳號為必填。";
            }

            // 銀行所在國家/地區(此為下拉式選單填寫項目，若錯誤則改為記錄log)
            if (UpdateInfo.BankCountryCode.Length > 2)
            {
                logger.Info(string.Format("銀行所在國家/地區輸入字元數超過2字元(BankCountryCode = {0})", UpdateInfo.BankCountryCode));
            }

            // 銀行所在州/省
            if (!string.IsNullOrEmpty(UpdateInfo.BankState) && UpdateInfo.BankState.Length > 20)
            {
                CheckIsCorrectResult += "銀行所在州/省不可超過20個字。";
            }

            // 銀行地址
            if (!string.IsNullOrEmpty(UpdateInfo.BankAddress) && UpdateInfo.BankAddress.Length > 150)
            {
                CheckIsCorrectResult += "銀行地址不可超過150個字。";
            }

            // 銀行所在城市
            if (!string.IsNullOrEmpty(UpdateInfo.BankCity) && UpdateInfo.BankCity.Length > 20)
            {
                CheckIsCorrectResult += "銀行所在城市不可超過20個字。";
            }

            // 銀行郵遞區號
            if (!string.IsNullOrEmpty(UpdateInfo.BankZipCode) && UpdateInfo.BankZipCode.Length > 10)
            {
                CheckIsCorrectResult += "銀行郵遞區號不可超過10個字。";

                if (!ISNumberAndAlphabet(UpdateInfo.BankAccountNumber))
                {
                    CheckIsCorrectResult += "銀行郵遞區號僅能為數字和英文字母。";
            }
            }

            // 帳戶名稱
            if (!string.IsNullOrEmpty(UpdateInfo.BeneficiaryName))
            {
                if (UpdateInfo.BeneficiaryName.Length > 60)
                {
                    CheckIsCorrectResult += "帳戶名稱不可超過60個字。";
                }
            }
            else
            {
                CheckIsCorrectResult += "帳戶名稱為必填。";
            }

            // 發票國家/區域(此為下拉式選單填寫項目，若錯誤則改為記錄log)
            if (!string.IsNullOrEmpty(UpdateInfo.BeneficiaryCountryCode))
            {
                if (UpdateInfo.BeneficiaryCountryCode.Length > 2)
                {
                    logger.Info(string.Format("發票國家/區域輸入字元數超過2字元(BankCountryCode = {0})", UpdateInfo.BeneficiaryCountryCode));
                }
                else if (UpdateInfo.BeneficiaryCountryCode != "CN" && UpdateInfo.BeneficiaryCountryCode != "CA" && UpdateInfo.BeneficiaryCountryCode != "TW" && UpdateInfo.BeneficiaryCountryCode != "US" && UpdateInfo.BeneficiaryCountryCode != "HK")
            {
                    logger.Info(string.Format("發票國家/區域輸入內容錯誤(BankCountryCode = {0})", UpdateInfo.BeneficiaryCountryCode));
                }
            }
            else
            {
                CheckIsCorrectResult += "發票國家/區域為必填。";
            }

            // 發票州/省
            if (!string.IsNullOrEmpty(UpdateInfo.BeneficiaryState))
            {
                if (UpdateInfo.BeneficiaryState.Length > 20)
                {
                    CheckIsCorrectResult += "發票州/省不可超過20個字。";
                }
            }
            else
            {
                CheckIsCorrectResult += "發票州/省為必填。";
            }

            // 發票地址
            if (!string.IsNullOrEmpty(UpdateInfo.BeneficiaryAddress))
            {
                if (UpdateInfo.BeneficiaryAddress.Length > 150)
                {
                    CheckIsCorrectResult += "發票地址不可超過150個字。";
                }
            }
            else
            {
                CheckIsCorrectResult += "發票地址為必填。";
            }

            // 發票城市
            if (!string.IsNullOrEmpty(UpdateInfo.BeneficiaryCity))
            {
                if (UpdateInfo.BeneficiaryCity.Length > 20)
                {
                    CheckIsCorrectResult += "發票城市不可超過20個字。";
                }
            }
            else
            {
                CheckIsCorrectResult += "發票城市為必填。";
            }

            // 發票郵遞區號
            if (!string.IsNullOrEmpty(UpdateInfo.BeneficiaryZipcode))
            {
                if (UpdateInfo.BeneficiaryZipcode.Length > 10)
                {
                    CheckIsCorrectResult += "發票郵遞區號不可超過10個字。";
                }

                if (!ISNumberAndAlphabet(UpdateInfo.BankAccountNumber))
                {
                    CheckIsCorrectResult += "銀行帳號僅能為數字和英文字母。";
                }
            }
            else
            {
                CheckIsCorrectResult += "發票郵遞區號為必填。";
            }

            return CheckIsCorrectResult;
        }

        #endregion

        #region 商家資訊

        private ActionResponse<Seller_BasicInfo> getSellerBasicInfo(int sellerID)
        {
            ActionResponse<Seller_BasicInfo> result = new ActionResponse<Seller_BasicInfo>();
            try
            {
                result = conn.GetSeller_BasicInfo(sellerID.ToString(), 0);
                if (!string.IsNullOrWhiteSpace(result.Body.SellerLogoURL))
                {
                    result.Body.SellerLogoURL = ".." + result.Body.SellerLogoURL;
                }
                else
                {
                    result.Body.SellerLogoURL = "../Themes/Images/Item/no-pic_60x45.jpg";
                }
            }
            catch (Exception ex)
            {
                result.Body = null;
                logger.Error(ex.Message);
            }


            return result;
        }

        [HttpPost]
        public JsonResult SaveSellerBasicInfo(string SellerAboutInfo, string SellerLogoUrl)
        {
            Seller_BasicInfo originalModel = new Seller_BasicInfo();
            API.Models.Seller_BasicafterInfo saveModel = new Seller_BasicafterInfo();

            string message = string.Empty;

            int RegUrl_index = SellerLogoUrl.IndexOf("Pic");
            SellerLogoUrl = SellerLogoUrl.Substring(RegUrl_index - 1);

            try
            {
                originalModel = getSellerBasicInfo(sellerInfo.currentSellerID).Body;

                if (originalModel != null)
                {
                    saveModel = SaveBasicInfo(originalModel, SellerAboutInfo, SellerLogoUrl);
                    message = conn.SaveSeller_BasicafterInfo(null, null, saveModel).Body;
                }
                else
                {
                    return Json(new { Msg = "發生意外錯誤，請稍後再試" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);

                return Json(new { Msg = "發生意外錯誤，請稍後再試!" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Msg = message }, JsonRequestBehavior.AllowGet);
        }

        private API.Models.Seller_BasicafterInfo SaveBasicInfo(Seller_BasicInfo originalModel, string SellerAboutInfo, string SellerLogoUrl)
        {
            API.Models.Seller_BasicafterInfo apiModel = new Seller_BasicafterInfo();

            try
            {
                Mapper.CreateMap<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo, API.Models.Seller_BasicafterInfo>()
                .ForMember(x => x.UpdateUserID, y => y.Ignore())
                .ForMember(x => x.UpdateDate, y => y.Ignore());

                apiModel = Mapper.Map<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo, API.Models.Seller_BasicafterInfo>(originalModel);
                apiModel.AboutInfo = SellerAboutInfo;
                apiModel.SellerLogoURL = SellerLogoUrl;
                apiModel.UpdateUserID = sellerInfo.UserID;
                apiModel.UpdateDate = DateTime.Now;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            return apiModel;
        }

        public JsonResult ImageUpload(IEnumerable<HttpPostedFileBase> files)
        {
            string physicalPath = string.Empty;
            // The Name of the Upload component is "files"
            if (files != null)
            {
                foreach (var file in files)
                {
                    try
                    {
                        byte[] imageBuffer = new byte[file.ContentLength];

                        file.InputStream.Read(imageBuffer, 0, file.ContentLength);
                        //將使用者上傳的圖片塞進 MemoryStream
                        MemoryStream imageMS = new MemoryStream(imageBuffer);

                        //建立 Image 物件
                        Image inputImage = Image.FromStream(imageMS);

                        //判斷圖片的大小是否超過限制
                        if (inputImage.Width > 127 || inputImage.Height > 33)
                        {
                            return Json(new { img = "", Msg = "圖片超過尺寸 Width:127 pixels x Height: 33 pixels!" });
                        }
                        else
                        {
                            // Some browsers send file names with full path.
                            // We are only interested in the file name.
                            var fileName = Path.GetFileName(file.FileName);

                            physicalPath = Path.Combine(Server.MapPath(@"~/Pic/SellerLogo"), "SellerPortal_" + sellerInfo.currentSellerID.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg");

                            // The files are not actually saved in this demo
                            file.SaveAs(physicalPath);

                            // 回傳到前面顯示
                            int RegUrl_index = physicalPath.IndexOf("Pic");
                            physicalPath = ".." + physicalPath.Substring(RegUrl_index - 1);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                        return Json(new { img = "", Msg = "發生意外錯誤，請稍後再試!" });
                    }

                }
            }

            // Return an empty string to signify success
            return Json(new { img = physicalPath, Msg = "圖片上傳成功!" });
        }

        private IEnumerable<string> GetFileInfo(IEnumerable<HttpPostedFileBase> files)
        {
            return
                from a in files
                where a != null
                select string.Format("{0} ({1} bytes)", Path.GetFileName(a.FileName), a.ContentLength);
        }
        #endregion


        public ActionResult NewAccountOpen()
        {
            return PartialView();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CreateNewEmail(string Email)
        {
            TWNewEgg.API.Models.UserCreation _userCreation = new UserCreation();
            if (string.IsNullOrEmpty(Email) == true)
            {
                return Json("[Error]: 請填寫Email");
            }
            bool isEmail = System.Text.RegularExpressions.Regex.IsMatch(Email, @"\w+([-+.']\w+)*@\w+([-+']\w+)*\.(\w+([-+']\w)*\.)*[A-Za-z]{2,4}$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (isEmail == false)
            {
                return Json("[Error]: Email 格式錯誤");
            }
            _userCreation.Email = Email;
            _userCreation.SellerID = sellerInfo.currentSellerID;
            _userCreation.GroupID = 2;
            _userCreation.InUserID = sellerInfo.UserID;
            _userCreation.PurviewType = "S";
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.UserCreationResult> _userCreationResult = new ActionResponse<UserCreationResult>();
            _userCreationResult = conn.CreateUser("", "", _userCreation);
            if (_userCreationResult.IsSuccess == true)
            {
                TWNewEgg.API.Models.Mail _mail = new Mail();
                _mail.UserEmail = Email;
                _mail.MailType = TWNewEgg.API.Models.Mail.MailTypeEnum.SallerInvitationEmail;
                _mail.RecipientBcc = null;
                var result = conn.SendMail("", "", _mail);
                if (result.Code == (int)UserLoginingResponseCode.Success && result.IsSuccess == true)
                {
                    return Json("[Success]: " + result.Msg);
                }
                else
                {
                    return Json("[Error]: " + result.Msg);
                }
            }
            return Json("[Error]: " + _userCreationResult.Msg);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sentData"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult StorePurView(string sentData, int UserId)
        {
            string[] functionPurView = sentData.Split(',');
            TWNewEgg.API.Models.SaveUserPurview _daveUserPurview = new SaveUserPurview();
            List<TWNewEgg.API.Models.SaveUserPurview.PurviewListInfo> _listPurviewListInfo = new List<SaveUserPurview.PurviewListInfo>();
            _daveUserPurview.UserID = UserId;
            _daveUserPurview.UpdateUserID = sellerInfo.UserID;
            foreach (var item in functionPurView)
            {
                TWNewEgg.API.Models.SaveUserPurview.PurviewListInfo _purViewListInfo = new SaveUserPurview.PurviewListInfo();

                _purViewListInfo.Enable = item.Split(';')[0].ToString();
                _purViewListInfo.FunctionID = Convert.ToInt16(item.Split(';')[1]);
                _listPurviewListInfo.Add(_purViewListInfo);
            }

            _daveUserPurview.PurviewList = _listPurviewListInfo;
            var result = conn.SaveUserPurview("", "", _daveUserPurview);
            if (result.Code == (int)ResponseCode.Success && result.IsSuccess == true)
            {
                return Json("[Success]: " + result.Msg);
            }
            else
            {
                return Json("[Error]: " + result.Msg);
            }

        }
        [HttpPost]
        public JsonResult StatusChange(string Status)
        {
            UserChangeStatus _userChangeStatus = new UserChangeStatus();
            string[] idStatus;
            string _strEmail = string.Empty;
            string _strStatus = string.Empty;
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.UserCheckStatusResult> result = new ActionResponse<UserCheckStatusResult>();
            if (string.IsNullOrEmpty(Status) == true)
            {
                return Json("[Error]: 錯誤");
            }
            try
            {
                idStatus = Status.Split(';');
                _strEmail = idStatus[0].ToString();
                _strStatus = idStatus[1].ToString();
            }
            catch (Exception error)
            {
                logger.Info("AccountSetting/StatusChange error: " + error.Message);
            }
            _userChangeStatus.UserEmail = _strEmail;
            _userChangeStatus.UpdateUserID = sellerInfo.UserID;
            //狀態為邀請中
            if (_strStatus == "I")
            {
                TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.MailResult> mailresult = new ActionResponse<MailResult>();
                TWNewEgg.API.Models.Mail _mail = new Mail();
                _mail.UserEmail = _strEmail;
                _mail.MailType = TWNewEgg.API.Models.Mail.MailTypeEnum.SallerInvitationEmail;
                _mail.RecipientBcc = null;
                try
                {
                    mailresult = conn.SendMail("", "", _mail);
                }
                catch (Exception error)
                {
                    logger.Info("AccountSetting/StatusChange _strStatus = I error: " + error.Message);
                    return Json("[Error]: 錯誤");
                }
                if (mailresult.Code == (int)UserLoginingResponseCode.Success && mailresult.IsSuccess == true)
                {
                    return Json("[Success]: 寄送成功;I");
                }
                else
                {
                    return Json("[Error]: 錯誤");
                }
            }
            else if (_strStatus == "D")//標記為關閉
            {
                _userChangeStatus.Status = "E";
            }
            else if (_strStatus == "E")//標記為開啟
            {
                _userChangeStatus.Status = "D";
            }
            else
            {
                return Json("[Error]: 錯誤");
            }
            try
            {
                result = conn.ChangeUserStatus("", "", _userChangeStatus);
            }
            catch (Exception error)
            {
                logger.Info("AccountSetting/StatusChange connect api \"ChangeUserStatus\" error: " + error.Message);
                return Json("[Error]: 錯誤");
            }
            if (result.IsSuccess == true)
            {
                return Json("[Success]: " + result.Msg);
            }
            else
            {
                return Json("[Error]: " + result.Msg);
            }
        }

        /// <summary>
        /// 檢查文字內容是否只有數字
        /// </summary>
        /// <param name="value">待檢查內容</param>
        /// <returns>檢查結果</returns>
        private bool ISOnlyNumber(string value)
        {
            Regex reg = new Regex(@"^[\x30-\x39]+$");

            if (reg.IsMatch(value))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 檢查文字內容是否只有數字和英文字母
        /// </summary>
        /// <param name="value">待檢查內容</param>
        /// <returns>檢查結果</returns>
        private bool ISNumberAndAlphabet(string value)
        {
            Regex reg = new Regex(@"^[\x30-\x39A-Za-z]+$");

            if (reg.IsMatch(value))
            {
                return true;
            }

            return false;
        } 
    }
}
