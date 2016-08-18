using KendoGridBinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.View.Attributes;

namespace TWNewEgg.API.View.Controllers
{
    public class SellerRelationshipManagementController : Controller
    {
        //
        // GET: /SellerRelationshipManagement/

        TWNewEgg.API.Models.Connector conn = new API.Models.Connector();
        Service.SellerInfoService sellerInfo_svs = new Service.SellerInfoService();
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        /// <summary>
        /// 商家關係維護與管理 Index主頁面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.UserManage)]
        [FunctionName(FunctionNameAttributeValues.SellerRelationshipManagement)]
        [ActiveKey(ActiveKeyAttributeValues.Menu)]
        [ActionDescription("商家關係維護與管理")]
        [Filter.PermissionFilter]
        public ActionResult Index()
        {
            ViewBag.currencyList = conn.GetCurrencyList().Body;
            ViewBag.countryRegionList = conn.GetRegionList().Body;
            
            return View();
        }

        /// <summary>
        /// 搜尋seller_BasicInfo資料
        /// </summary>
        /// <param name="request"></param>
        /// <param name="sellerStatus"></param>
        /// <param name="createDateStart"></param>
        /// <param name="createDateEnd"></param>
        /// <param name="updateDateStart"></param>
        /// <param name="updateDateEnd"></param>
        /// <param name="sellerCountryCode"></param>
        /// <returns></returns>
        [HttpPost]
        [Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        public ActionResult Search(KendoGridRequest request, string sellerStatus, string createDateStart, string createDateEnd, string updateDateStart, string updateDateEnd, string sellerCountryCode)
        {
            TWNewEgg.API.Models.SellerRelationshipSearchCondition searchCondition = new TWNewEgg.API.Models.SellerRelationshipSearchCondition();

            #region 狀態篩選

            switch (sellerStatus)
            {
                default:
                    {
                        searchCondition.SellerStatus = TWNewEgg.API.Models.SellerStatus.All;
                        break;
                    }
                case "A":
                    {
                        searchCondition.SellerStatus = TWNewEgg.API.Models.SellerStatus.Active;
                        break;
                    }
                case "C":
                    {
                        searchCondition.SellerStatus = TWNewEgg.API.Models.SellerStatus.Closed;
                        break;
                    }
                case "I":
                    {
                        searchCondition.SellerStatus = TWNewEgg.API.Models.SellerStatus.Inactive;
                        break;
                    }
            }

            #endregion 狀態篩選

            #region 日期篩選

            DateTime dateTime;
            if (!DateTime.TryParse(createDateStart, out dateTime))
            {
                searchCondition.CreateDateStart = null;
            }
            else
            {
                searchCondition.CreateDateStart = createDateStart;
            }

            if (!DateTime.TryParse(createDateEnd, out dateTime))
            {
                searchCondition.CreateDateEnd = null;
            }
            else
            {
                searchCondition.CreateDateEnd = createDateEnd;
            }

            if (!DateTime.TryParse(updateDateStart, out dateTime))
            {
                searchCondition.UpdateDateStart = null;
            }
            else
            {
                searchCondition.UpdateDateStart = updateDateStart;
            }

            if (!DateTime.TryParse(updateDateEnd, out dateTime))
            {
                searchCondition.UpdateDateEnd = null;
            }
            else
            {
                searchCondition.UpdateDateEnd = updateDateEnd;
            }

            #endregion 日期篩選

            #region 地區篩選

            switch (sellerCountryCode)
            {
                default:
                    {
                        searchCondition.SellerCountryCode = API.Models.CountryCode.All;
                        break;
                    }
                case "CA":
                    {
                        searchCondition.SellerCountryCode = API.Models.CountryCode.Canada;
                        break;
                    }
                case "CN":
                    {
                        searchCondition.SellerCountryCode = API.Models.CountryCode.China;
                        break;
                    }
                case "HK":
                    {
                        searchCondition.SellerCountryCode = API.Models.CountryCode.HongKong;
                        break;
                    }
                case "TW":
                    {
                        searchCondition.SellerCountryCode = API.Models.CountryCode.Taiwan;
                        break;
                    }
                case "US":
                    {
                        searchCondition.SellerCountryCode = API.Models.CountryCode.UnitedStates;
                        break;
                    }
            }

            #endregion 地區篩選

            // 是否有管理權限
            searchCondition.IsAdmin = sellerInfo_svs.IsAdmin;

            // 設定商家 ID
            searchCondition.SellerID = sellerInfo_svs.currentSellerID;

            // 判斷是否搜尋全部的商家
            if (searchCondition.IsAdmin && sellerInfo_svs.SellerID == sellerInfo_svs.currentSellerID)
            {
                searchCondition.SellerID = null;
            }
            
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.VM_Seller_BasicInfo>> result_GetSeller_BasicInfosbyQuery = new API.Models.ActionResponse<List<TWNewEgg.API.Models.VM_Seller_BasicInfo>>();
            string msg = string.Empty;
            bool isSuccess = false;

            // 連線至connector查詢 Seller_BasicInfo
            try
            {
                result_GetSeller_BasicInfosbyQuery = conn.SearchSellerForRelationship(null, null, searchCondition);

                isSuccess = result_GetSeller_BasicInfosbyQuery.IsSuccess;
                msg += result_GetSeller_BasicInfosbyQuery.Msg;

                //搜尋完畢所得 data model: VM_Seller_BasicInfo 
                AutoMapper.Mapper.CreateMap<TWNewEgg.API.Models.VM_Seller_BasicInfo, TWNewEgg.API.View.SellerRelationshipManagement.SellerRelationshipDetails>();
                List<TWNewEgg.API.View.SellerRelationshipManagement.SellerRelationshipDetails> data = new List<SellerRelationshipManagement.SellerRelationshipDetails>();
                foreach (TWNewEgg.API.Models.VM_Seller_BasicInfo single_sellerinfo in result_GetSeller_BasicInfosbyQuery.Body)
                {
                    //map data model >> view model
                    AutoMapper.Mapper.CreateMap<TWNewEgg.API.Models.VM_Seller_BasicInfo, TWNewEgg.API.View.SellerRelationshipManagement.SellerRelationshipDetails>();
                    TWNewEgg.API.View.SellerRelationshipManagement.SellerRelationshipDetails sellerInfoList = new TWNewEgg.API.View.SellerRelationshipManagement.SellerRelationshipDetails();

                    sellerInfoList = AutoMapper.Mapper.Map<TWNewEgg.API.Models.VM_Seller_BasicInfo, TWNewEgg.API.View.SellerRelationshipManagement.SellerRelationshipDetails>(single_sellerinfo);
                    sellerInfoList.SellerCountryCodeName = single_sellerinfo.SellerCountryCodeName;
                    sellerInfoList.BillingCycle = sellerInfoList.BillingCycle == null ? 0 : single_sellerinfo.BillingCycle;
                    sellerInfoList.Identy = single_sellerinfo.Identy == null ? 0 : single_sellerinfo.Identy;
                    sellerInfoList.CompanyCode = single_sellerinfo.CompanyCode;
                    //if (sellerInfoList.CreateDate == null)
                    //{
                    //    sellerInfoList.CreateDate = DateTime.MinValue;
                    //}

                    data.Add(sellerInfoList);
                }

                //回傳kendoGrid model, grid 所需的view model
                return Json(new KendoGrid<TWNewEgg.API.View.SellerRelationshipManagement.SellerRelationshipDetails>(request, data));
            }
            catch (Exception ex)
            {
                log4net.ILog logger;
                logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
                logger.Info("連線至connector查詢Seller_BasicInfo發生Exception:" + ex.Message);
                msg += "exception, " + ex.Message;

                //return Json(new KendoGrid<TWNewEgg.API.View.SellerRelationshipManagement>(request, null));
                return Json(new { isSuccess = isSuccess, msg = msg});
            }
        }

        /// <summary>
        /// 取得(所有)國家/區域資料list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        public ActionResult GetCountryRegionList()
        {
            try
            {
                List<TWNewEgg.API.Models.GetRegionListResult> countryList = conn.GetRegionList().Body;
                return Json(countryList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
                //throw;
            }
        }

        /// <summary>
        /// 取得(所有)幣別資料list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        public ActionResult GetCurrencyList()
        {
            try
            {
                List<TWNewEgg.API.Models.GetCurrencyListResult> currencyList = conn.GetCurrencyList().Body;
                return Json(currencyList.Select(x => new { text = x.CurrencyCode + "(" + x.Name + ")", value = x.CurrencyCode }), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
                //throw;
            }
        }

        /// <summary>
        /// 將幣別由 代碼 轉換為 "代碼(幣別名稱)" 的格式
        /// </summary>
        /// <param name="currCode"></param>
        /// <param name="currName"></param>
        /// <returns></returns>
        private string currencyToDisplay(string currCode)
        {
            List<TWNewEgg.API.Models.GetCurrencyListResult> currencyList = conn.GetCurrencyList().Body;

            string currencyStrToDisplay = currencyList.Where(x => x.CurrencyCode == currCode).Select(x => new { text = x.CurrencyCode + "(" + x.Name + ")" }).SingleOrDefault().text;

            return currencyStrToDisplay;
        }
        
        /// <summary>
        /// 將sellerStatus顯示為sellerStatus全部(英文)名稱
        /// </summary>
        /// <param name="sellerStatus"></param>
        /// <returns></returns>
        private string sellerStatusToDisplay(string sellerStatus)
        {
            switch (sellerStatus.ToUpper())
            {
                case "A":
                    return SellerRelationshipManagement.enumSellerStatus.Active.ToString();
                case "I":
                    return SellerRelationshipManagement.enumSellerStatus.Inactive.ToString();
                case "C":
                    return SellerRelationshipManagement.enumSellerStatus.Closed.ToString();
                default:
                    return "";
            }
        }

        /// <summary>
        /// 寄送邀請信件
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="sellerInfo"></param>
        /// <param name="sellerCharge"></param>
        /// <returns></returns>
        [HttpGet]
        [Filter.PermissionFilter]
        [Filter.LoginAuthorize]
        public JsonResult SendMail(string auth, string token, string sellerEmail, string sellerName)
        {
            string resultMsg = string.Empty;
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

            //log 監控寄信過程
            logger.Info("開始連接至connector進行\"再次邀請\"寄送信件");
            try
            {
                var result_sendMail = conn.SendMail(auth, token, new TWNewEgg.API.Models.Mail() { MailType = TWNewEgg.API.Models.Mail.MailTypeEnum.SallerInvitationEmail, UserEmail = sellerEmail, UserName = sellerName });

                if (result_sendMail.IsSuccess)
                {
                    logger.Info("寄送再次邀請信件成功");
                    resultMsg += "寄信成功";
                }
                else
                {
                    logger.Info("寄送再次邀請信件失敗:" + result_sendMail.Msg);
                    resultMsg += "寄信失敗 ";
                }
            }
            catch (Exception ex)
            {
                logger.Info("寄送再次邀請信件時候發生exception:" + ex.Message);
                throw;
            }

            return Json(new { msg = resultMsg }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 開新分頁編輯 商家資訊(seller_BasicInfo)
        /// </summary>
        /// <param name="SellerID"></param>
        /// <returns></returns>
        [HttpGet]
        [Filter.PermissionFilter]
        [Filter.LoginAuthorize]
        public ActionResult LinkToAccountSettings(string SellerID)
        {
            TWNewEgg.API.View.Service.AES aes = new Service.AES();
            Response.Cookies["CSD"].Value = aes.AesEncrypt(SellerID.ToString());
            Response.Cookies["RSD"].Value = aes.AesEncrypt(1.ToString());

            return RedirectToAction("Index", "AccountSettings", new { SellerID = SellerID });
        }

        /// <summary>
        /// 更新商家關係維護與管理 畫面上"狀態"與 "幣別"至 seller_BasicInfo
        /// </summary>
        /// <param name="sellerInfosToUpdate">待反序列化的 List<seller_BasicInfo>欲修改資料</param>
        /// <returns></returns>
        [HttpPost]
        [Filter.PermissionFilter]
        [Filter.LoginAuthorize]
        public ActionResult UpdateSellerInfo(string sellerInfosToUpdateStr)
        {
            string _msg = string.Empty;
            #region 反序列化, 一起檢查反序列化是否有錯
            List<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo> sellerInfoListToUpdate = new List<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>();
            bool NoException = true;
            try
            {
                //Json string反序列化
                sellerInfoListToUpdate = JsonConvert.DeserializeObject<List<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>>(sellerInfosToUpdateStr);
                NoException = true;
            }
            catch (Exception error)
            {
                logger.Error("[Message]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
                NoException = false;
            }
            if (NoException == false)
            {
                return Json(new { isSuccess = NoException, msg = "資料錯誤" });
            }
            #endregion
            #region 先檢查廠商身分別和幣別的關係
            TWNewEgg.API.Models.ActionResponse<string> checkIdentyCyrrencyType = this.checkIdentyCurrencyType(sellerInfoListToUpdate);
            if (checkIdentyCyrrencyType.IsSuccess == false)
            {
                return Json(new { isSuccess = checkIdentyCyrrencyType.IsSuccess, msg = checkIdentyCyrrencyType.Msg });
            }
            #endregion
            TWNewEgg.API.Models.ActionResponse<string> UpdateResult = new API.Models.ActionResponse<string>();
            try
            {
                UpdateResult = conn.sellerrelationshipmanagement_Edit(sellerInfoListToUpdate, sellerInfo_svs.UserID.ToString());
            }
            catch (Exception error)
            {
                UpdateResult.IsSuccess = false;
                UpdateResult.Msg = "資料錯誤修改錯誤";
                logger.Error("[Message]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }
            if (UpdateResult.IsSuccess == false)
            {
                return Json(new { isSuccess = false, msg = UpdateResult.Msg });
            }
            else
            {
                return Json(new { isSuccess = true, msg = UpdateResult.Msg });
            }
        }
        #region 檢查廠商身分別, 幣別
        public TWNewEgg.API.Models.ActionResponse<string> checkIdentyCurrencyType(List<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo> sellerInfoListToUpdate_modified)
        {
            TWNewEgg.API.Models.ActionResponse<string> result = new API.Models.ActionResponse<string>();
            TWNewEgg.API.View.Service.TaxID_IdentityCard_CheckService taxID_Identity = new Service.TaxID_IdentityCard_CheckService();
            #region 初始化回傳的資料
            result.IsSuccess = true;
            result.Msg = "無資料錯誤";
            #endregion
            string errorMessage = string.Empty;
            string str_Identy = string.Empty;
            foreach (var item in sellerInfoListToUpdate_modified)
            {
                #region 國內廠商或個人戶, 只能選台幣
                //國內廠商或個人戶, 只能選台幣
                if (item.Identy == 1 || item.Identy == 3)
                {
                    if (item.Currency != "TWD")
                    {
                        try
                        {
                            bool isExist = Enum.IsDefined(typeof(TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo.IdentyType), item.Identy);
                            if (isExist == false)
                            {
                                errorMessage = "資料錯誤";
                                result.IsSuccess = false;
                                result.Msg = errorMessage;
                                logger.Error("item.SellerID = " + item.SellerID + ", 找不到對應的 TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo.SellerIdenty. Identy = " + item.Identy);
                                break;
                            }
                            else
                            {
                                TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo.IdentyType sellerIdenty = (TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo.IdentyType)Enum.ToObject(typeof(TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo.IdentyType), item.Identy);
                                str_Identy = sellerIdenty.ToString();
                                errorMessage = "編號: " + item.SellerID + ", 廠商身分別為: " + str_Identy + ", 幣別必須為 TWD";
                                result.IsSuccess = false;
                                result.Msg = errorMessage;
                                break;
                            }

                        }
                        catch (Exception error)
                        {
                            result.IsSuccess = false;
                            result.Msg = "資料錯誤";
                            logger.Error("[Message]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
                        }
                    }
                }
                #endregion
                #region 檢查廠商身分別和統編/身分證字號的關係
                //國內廠商
                if (item.Identy == 1)
                {
                    //檢查統編是否錯誤
                    TWNewEgg.API.Models.ActionResponse<string> checkTaxresult = taxID_Identity.TaxIDCheck(item.CompanyCode);
                    if (checkTaxresult.IsSuccess == false)
                    {
                        TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo.IdentyType sellerIdenty = (TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo.IdentyType)Enum.ToObject(typeof(TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo.IdentyType), item.Identy);
                        str_Identy = sellerIdenty.ToString();
                        errorMessage = "編號: " + item.SellerID + ", 名稱: " + item.SellerName + ", 廠商身分別為: " + str_Identy + ", 統編格式錯誤";
                        result.IsSuccess = false;
                        result.Msg = errorMessage;
                        break;
                    }
                    else
                    {
                        result.IsSuccess = true;
                    }

                }
                else if (item.Identy == 3)
                {
                    //個人戶
                    TWNewEgg.API.Models.ActionResponse<string> checkPersonalresult = taxID_Identity.IdentityCardCheck(item.CompanyCode);
                    if (checkPersonalresult.IsSuccess == false)
                    {
                        TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo.IdentyType sellerIdenty = (TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo.IdentyType)Enum.ToObject(typeof(TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo.IdentyType), item.Identy);
                        str_Identy = sellerIdenty.ToString();
                        errorMessage = "編號: " + item.SellerID + ", 名稱: " + item.SellerName + ", 廠商身分別為: " + str_Identy + ", 身份證字號錯誤";
                        result.IsSuccess = false;
                        result.Msg = errorMessage;
                        break;
                    }
                    else
                    {
                        result.IsSuccess = true;
                    }
                }
                else if (item.Identy == 2)
                {
                    //國外廠商,目前沒有限制
                }
                else
                {
                    result.IsSuccess = false;
                    errorMessage = "編號: " + item.SellerID + ", 名稱: " + item.SellerName + ", 廠商身分別為: " + str_Identy + ", 資料錯誤: 是否有選擇廠商身分別";
                    result.Msg = errorMessage;
                    break;
                }
                #endregion
            }
            return result;
        }
        #endregion

    }
}
