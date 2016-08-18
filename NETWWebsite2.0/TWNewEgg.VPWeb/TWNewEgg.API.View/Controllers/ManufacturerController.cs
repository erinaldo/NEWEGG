using KendoGridBinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Models;
using TWNewEgg.API.View.Attributes;
using AutoMapper;
using System.Text.RegularExpressions;

namespace TWNewEgg.API.View.Controllers
{
    public class ManufacturerController : Controller
    {
        // GET: /Manufacturer/
        [HttpGet]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.ManageItems)]
        [FunctionName(FunctionNameAttributeValues.Manufacturer)]
        [ActiveKey(ActiveKeyAttributeValues.Menu)]
        [ActionDescription("製造商")]
        [Filter.PermissionFilter]
        public ActionResult Index()
        {
            return View();
        }
        TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();
        public JsonResult ManufacturerRequest()
        {


            ViewBag.isAdmin = sellerInfo.IsAdmin;
            string ManufacturerRequestView = RenderView("ManufacturerRequest");
            return Json(new { IsSuccess = true, ViewHtml = ManufacturerRequestView });
        }

        [HttpPost]
        public JsonResult ManufacturerRequest(KendoGridRequest request)
        {
            ViewBag.isAdmin = sellerInfo.IsAdmin;
            string ManufacturerRequestView = RenderView("ManufacturerRequest");
            return Json(new { IsSuccess = true, ViewHtml = ManufacturerRequestView });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="keywd"></param>
        /// <returns></returns>
        [HttpPost]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.ManageItems)]
        [FunctionName(FunctionNameAttributeValues.Manufacturer)]
        [ActiveKey(ActiveKeyAttributeValues.Menu)]
        [ActionDescription("製造商清單")]
        public JsonResult ManufacturerList(KendoGridRequest request, string keywd)
        {
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

            TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();

            Connector conn = new Connector();
            SearchDataModel searchManufacturerList = new SearchDataModel();
            API.Models.ActionResponse<List<Manufacturer>> manufacturerItemList = new ActionResponse<List<Manufacturer>>();
            manufacturerItemList.Body = new List<Manufacturer>();
            API.Models.ActionResponse<List<ManufacturerAdditional>> newManufacturerItemList = new ActionResponse<List<ManufacturerAdditional>>();
            newManufacturerItemList.Body = new List<ManufacturerAdditional>();

            searchManufacturerList.SearchType = SearchType.SearchofficialALLInfo;
            searchManufacturerList.KeyWord = keywd;

            // 取得ManufacturerList資訊
            try
            {
                manufacturerItemList = conn.SearchManufacturerInfo(searchManufacturerList);
            }
            catch (Exception e)
            {
                manufacturerItemList.IsSuccess = false;
                manufacturerItemList.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                manufacturerItemList.Msg = e.Message;
                logger.Error("SearchManufacturerInfo Connect Fail:[ErrorMessage] " + e.Message + " [ErrorStackTrace] " + e.StackTrace);
            }
            // 將舊的ManufacturerList Mapper 到新的model中
            try
            {
                newManufacturerItemList = this.ManufacturerListMapper(manufacturerItemList);
            }
            catch (Exception e)
            {
                manufacturerItemList.IsSuccess = false;
                manufacturerItemList.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                manufacturerItemList.Msg = e.Message;
            }

            // 將製造商清單的查詢結果，依製造商名稱排序
            if (newManufacturerItemList.IsSuccess && newManufacturerItemList.Body.Count > 0)
            {
                newManufacturerItemList.Body = newManufacturerItemList.Body.OrderBy(x => x.ManufactureName).ToList();
            }

            return Json(new KendoGrid<ManufacturerAdditional>(request, newManufacturerItemList.Body));
        }

        /// <summary>
        /// 更新Manufacturer List
        /// </summary>
        /// <param name="jsonManufacturerList">所要更新的Manufacturer List</param>
        /// <returns>返回更新結果</returns>
        [HttpPost]
        public JsonResult ManufacturerEdit(string jsonManufacturerList)
        {
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            Connector conn = new Connector();
            List<ManufacturerAdditional> manufacturerAdditionalList = JsonConvert.DeserializeObject<List<ManufacturerAdditional>>(jsonManufacturerList);
            TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();
            List<Manufacturer> manufacturerList = new List<Manufacturer>();
            
            // 更新Manufacturer List
            ActionResponse<string> result = new ActionResponse<string>();
            result.IsSuccess = true;
            foreach (ManufacturerAdditional manufacturerAdditional in manufacturerAdditionalList)
            {
                try
                {
                    Mapper.CreateMap<ManufacturerAdditional, Manufacturer>();
                    Manufacturer manufacturer = new Manufacturer();
                    // Mapper ManufacturerAdditional model 至 Manufacturer model
                    manufacturer = Mapper.Map<Manufacturer>(manufacturerAdditional);
                    // 格式驗證
                    ActionResponse<string> ruleCheck = ManufacturerVerification(manufacturerAdditional);
                    if (ruleCheck.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.Msg += ruleCheck.Msg;
                    }
                    // 解析與分存電話號碼資訊
                    ActionResponse<string> phoneNumber = AnalysePhoneNumber(manufacturerAdditional.PhoneNumberDetail);
                    if (phoneNumber.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.Msg += phoneNumber.Msg;
                    }

                    manufacturer.PhoneRegion = phoneNumber.Body.Split(',')[0];
                    manufacturer.Phone = phoneNumber.Body.Split(',')[1];
                    manufacturer.PhoneExt = phoneNumber.Body.Split(',')[2];
                    manufacturer.UpdateUserID = sellerInfo.UserID;
                    manufacturer.UpdateDate = DateTime.UtcNow.AddHours(8);

                    manufacturerList.Add(manufacturer);
                }
                catch (Exception e)
                {
                    logger.Error("ManufacturerAdditional Mapper Fail:[ErrorMessage] " + e.Message + " [ErrorStackTrace] " + e.StackTrace);
                    result.IsSuccess = false;
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.Msg = e.Message;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            
            if (result.IsSuccess == false)
            {
                logger.Error("EditManufacturerInfo 電話驗證錯誤 Fail:[ErrorMessage] " + result.Msg);
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            try
            {
                // 更新資料
                result = conn.EditManufacturerInfo(manufacturerList);
            }
            catch (Exception e)
            {
                logger.Error("EditManufacturerInfo Connect Fail:[ErrorMessage] " + e.Message + " [ErrorStackTrace] " + e.StackTrace);
                result.IsSuccess = false;
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                result.Msg = e.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
            //return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 驗證輸入欄位格式是否正確
        /// </summary>
        /// <param name="manufacturerAdditional"></param>
        /// <returns></returns>
        public API.Models.ActionResponse<string> ManufacturerVerification(ManufacturerAdditional manufacturerAdditional)
        {
            API.Models.ActionResponse<string> result = new ActionResponse<string>();
            // 驗證是話與手機號碼是否符合規定
            string strPhoneDetail = manufacturerAdditional.PhoneNumberDetail.Replace("(", "").Replace(")", "").Replace("#", "");
            try
            {
                if (!string.IsNullOrEmpty(strPhoneDetail))
                {
                    int checkPhoneNumber = Convert.ToInt32(strPhoneDetail);
                }

                result.IsSuccess = true;
            }
            catch
            {
                result.IsSuccess = false;
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                result.Msg = "電話號碼格式錯誤，請重新查核";
            }

            return result;
        }

        /// <summary>
        /// 執行新舊Manufacturer Mapper
        /// </summary>
        /// <param name="manufacturerItemList"></param>
        /// <returns></returns>
        public API.Models.ActionResponse<List<ManufacturerAdditional>> ManufacturerListMapper(API.Models.ActionResponse<List<Manufacturer>> manufacturerItemList)
        {
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            API.Models.ActionResponse<List<ManufacturerAdditional>> mapperManufacturerList = new ActionResponse<List<ManufacturerAdditional>>();
            TWNewEgg.API.View.Service.SellerInfoService _sellerInfoService = new Service.SellerInfoService();
            mapperManufacturerList.Body = new List<ManufacturerAdditional>();

            mapperManufacturerList.IsSuccess = manufacturerItemList.IsSuccess;
            mapperManufacturerList.Code = manufacturerItemList.Code;
            mapperManufacturerList.Msg = manufacturerItemList.Msg;

            bool _boolAdmin = _sellerInfoService.IsAdmin;
            
            if (manufacturerItemList.Body != null)
            {
                foreach (Manufacturer manufacturer in manufacturerItemList.Body)
                {
                    try
                    {
                        Mapper.CreateMap<Manufacturer, ManufacturerAdditional>();
                        ManufacturerAdditional manufacturerAdditional = new ManufacturerAdditional();
                        manufacturerAdditional = Mapper.Map<ManufacturerAdditional>(manufacturer);
                        // 組合完整電話資訊
                        manufacturerAdditional.PhoneNumberDetail = PhoneNumberCombine(manufacturerAdditional.PhoneRegion, manufacturerAdditional.Phone, manufacturerAdditional.PhoneExt);
                        //判斷是否為admin使用
                        manufacturerAdditional.isAdmin = _boolAdmin;
                        mapperManufacturerList.Body.Add(manufacturerAdditional);
                    }
                    catch (Exception e)
                    {
                        logger.Error("ManufacturerAdditional Mapper Fail:[ErrorMessage] " + e.Message + " [ErrorStackTrace] " + e.StackTrace);
                        throw new Exception(e.Message);
                    }
                }
            }

            return mapperManufacturerList;
        }

        /// <summary>
        /// 組合完整電話資訊
        /// </summary>
        /// <param name="phoneRegion">區碼</param>
        /// <param name="phone">號碼</param>
        /// <param name="phoneExt">分機</param>
        /// <returns>返回完整電話資訊</returns>
        public string PhoneNumberCombine(string phoneRegion, string phone, string phoneExt)
        {
            //string _strTotalPhone = string.Empty;
            //string _strPhoneRegion = string.Empty;
            //string _strPhone = string.Empty;
            //string _strPhoneExt = string.Empty;
            //if (string.IsNullOrEmpty(phoneRegion) == false)
            //{
            //    _strPhoneRegion = phoneRegion.Trim();
            //}

            //if (string.IsNullOrEmpty(phone) == false)
            //{
            //    _strPhone = phone.Trim();
            //}

            //if (string.IsNullOrEmpty(phoneExt) == false)
            //{
            //    _strPhoneExt = phoneExt.Trim();
            //}

            //_strTotalPhone = "(" + _strPhoneRegion + ")" + _strPhone + "#" + _strPhoneExt;
            //return _strTotalPhone;
            string phoneNumberDetail = string.Empty;
            if (!string.IsNullOrEmpty(phoneRegion))
            {
                phoneNumberDetail = "(" + phoneRegion.Trim() + ")";
            }

            if (!string.IsNullOrEmpty(phone))
            {
                phoneNumberDetail += phone.Trim();
            }

            if (!string.IsNullOrEmpty(phoneExt))
            {
                phoneNumberDetail += "#" + phoneExt.Trim();
            }

            return phoneNumberDetail;
        }

        /// <summary>
        /// 解析與分存電話號碼資訊
        /// </summary>
        /// <param name="phoneNumberDetail">所要解析的phoneNumberDetail</param>
        /// <returns>分存解析後電話號碼資訊，以逗號(',')為區隔 string = "phoneRegion,phone,phoneExt"</returns>
        public ActionResponse<string> AnalysePhoneNumber(string phoneNumberDetail)
        {
            ActionResponse<string> phoneNumber = new ActionResponse<string>();
            phoneNumber.IsSuccess = true;
            // 電話區碼
            string phoneRegion = string.Empty;
            // 電話主碼
            string phone = string.Empty;
            // 電話分機號碼
            string phoneExt = string.Empty;
            if (!string.IsNullOrEmpty(phoneNumberDetail))
            {
                #region 手機號碼解析
                phoneNumberDetail = phoneNumberDetail.Replace(" ", "");
                int symbol1 = phoneNumberDetail.IndexOf("(");
                int symbol2 = phoneNumberDetail.IndexOf(")");
                int symbol3 = phoneNumberDetail.IndexOf("#");
                // phoneRegion
                // 有'('
                if (symbol1 != -1)
                {
                    // 有'('有')'
                    if (symbol2 != -1)
                    {
                        phoneRegion = phoneNumberDetail.Substring(symbol1 + 1, symbol2 - (symbol1 + 1));
                        // 有'('有')'有'#'
                        if (symbol3 != -1)
                        {
                            phone = phoneNumberDetail.Substring(symbol2 + 1, symbol3 - (symbol2 + 1));
                            phoneExt = phoneNumberDetail.Substring(symbol3 + 1, phoneNumberDetail.Length - (symbol3 + 1));
                        }
                        else
                        {
                            // 有'('有')'無'#'
                            phone = phoneNumberDetail.Substring(symbol2 + 1, phoneNumberDetail.Length - (symbol2 + 1));
                        }
                    }
                    else
                    {
                        // 有'('無')'有'#'
                        if (symbol3 != -1)
                        {
                            phone = phoneNumberDetail.Substring(symbol1 + 1, symbol3 - (symbol1 + 1));
                            phoneExt = phoneNumberDetail.Substring(symbol3 + 1, phoneNumberDetail.Length - (symbol3 + 1));
                        }
                        else
                        {
                            // 有'('無')'無'#'
                            phone = phoneNumberDetail.Substring(symbol1 + 1, phoneNumberDetail.Length - (symbol1 + 1));
                        }
                    }
                }
                else
                {
                    // 無'('有')'
                    if (symbol2 != -1)
                    {
                        phoneRegion = phoneNumberDetail.Substring(0, symbol2);
                        // 無'('有')'有'#'
                        if (symbol3 != -1)
                        {
                            phone = phoneNumberDetail.Substring(symbol2 + 1, symbol3 - (symbol2 + 1));
                            phoneExt = phoneNumberDetail.Substring(symbol3 + 1, phoneNumberDetail.Length - (symbol3 + 1));
                        }
                        else
                        {
                            // 無'('有')'無'#'
                            phone = phoneNumberDetail.Substring(symbol2 + 1, phoneNumberDetail.Length - (symbol2 + 1));
                        }
                    }
                    else
                    {
                        // 無'('無')'有'#'
                        if (symbol3 != -1)
                        {
                            phone = phoneNumberDetail.Substring(0, symbol3);
                            phoneExt = phoneNumberDetail.Substring(symbol3 + 1, phoneNumberDetail.Length - (symbol3 + 1));
                        }
                        else
                        {
                            // 無'('無')'無'#'
                            phone = phoneNumberDetail;
                        }
                    }
                }
                #endregion
            }

            if (phoneRegion.Length > 6)
            {
                phoneNumber.IsSuccess = false;
                phoneNumber.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                phoneNumber.Msg += "<區碼長度錯誤 請檢核>";
            }

            if (phone.Length > 10)
            {
                phoneNumber.IsSuccess = false;
                phoneNumber.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                phoneNumber.Msg += "<號碼長度錯誤 請檢核>";
            }

            if (phoneExt.Length > 5)
            {
                phoneNumber.IsSuccess = false;
                phoneNumber.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                phoneNumber.Msg += "<分機號碼長度錯誤 請檢核>";
            }

            phoneNumber.Body = phoneRegion + "," + phone + "," + phoneExt;
            return phoneNumber;
        }

        /// <summary>
        /// 將該View轉成string
        /// </summary>
        /// <param name="partialView">View的名稱</param>
        /// <returns>返回string</returns>
        public string RenderView(string partialView)
        {
            string result = string.Empty;
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, partialView);
                ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                result = sw.GetStringBuilder().ToString();
            }

            return result;
        }

        /// <summary>
        /// 製造商管理資料查詢
        /// </summary>
        /// <param name="request">KENDOUI資料</param>
        /// <param name="_searchDataModel">查詢條件資料</param>
        /// <returns>查詢結果的MODEL</returns>
        [HttpPost]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.ManageItems)]
        [FunctionName(FunctionNameAttributeValues.Manufacturer)]
        [ActiveKey(ActiveKeyAttributeValues.Menu)]
        [ActionDescription("製造商管理")]
        public ActionResult MarkerSearch(KendoGridRequest request, TWNewEgg.API.Models.SearchDataModel _searchDataModel)
        {
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

            TWNewEgg.API.Models.Connector _connector = new Connector();
            TWNewEgg.API.View.Service.SellerInfoService _sellerInfoService = new Service.SellerInfoService();
            List<ManufacturerAdditional> _manufacturerAdditional = new List<ManufacturerAdditional>();
            API.Models.ActionResponse<List<Manufacturer>> result = new ActionResponse<List<Manufacturer>>();
            API.Models.ActionResponse<List<ManufacturerAdditional>> _manufactureSearchResult = new ActionResponse<List<ManufacturerAdditional>>();
            
            #region 收集SEARCH時所需要的資料
            _searchDataModel.SellerID = _sellerInfoService.currentSellerID;

            if (_sellerInfoService.IsAdmin && _sellerInfoService.SellerID == _sellerInfoService.currentSellerID)
            {
                _searchDataModel.SellerID = 0;
            }

            //預設為 Selection
            _searchDataModel.SearchType = TWNewEgg.API.Models.SearchType.Selection;
            _searchDataModel.SearchSN = false;
            #endregion


            int _intTotal = 0;
            try
            {
                // connect the search api
                result = _connector.SearchManufacturerInfo(_searchDataModel);
                // search success; success = 0, error = 1
                if (result.IsSuccess == true && result.Code == (int)ResponseCode.Success)
                {
                    _intTotal = result.Body.Count();
                    //if search result is not null, do the mapper method
                    if (result.Body != null)
                    {
                        // 做mapping的動作
                        _manufacturerAdditional = ManufacturerListMapper(result).Body;

                        // 根據時間做排序
                        //_manufacturerAdditional = _manufacturerAdditional.OrderBy(p => p.InDate).ToList();

                        // 將製造商建立請求的查詢結果，依照更新日期遞減排序
                        _manufacturerAdditional = _manufacturerAdditional.OrderByDescending(p => p.UpdateDate).ToList();
                    }
                }
                else if (result.IsSuccess == false && result.Code == (int)ResponseCode.Error)//search result is do data; success = 0, error = 1
                {
                }
                else
                {
                    // record the reason
                    logger.Info("Manufacturer/MarkerSearch: SearchManufacturerInfo else error: " + result.Msg);
                }
            }
            catch (Exception error)
            {
                // something error is not fouce on
                logger.Info("Manufacturer/MarkerSearch: SearchManufacturerInfo Api error: " + error.Message);
                return Json(new KendoGrid<TWNewEgg.API.View.ManufacturerAdditional>(request, null));
            }
            return Json(new KendoGrid<TWNewEgg.API.View.ManufacturerAdditional>(request, _manufacturerAdditional));

        }

        /// <summary>
        /// 修改製造商相關的資料
        /// </summary>
        /// <param name="jsonManufacturerList">反序列化的修改資料</param>
        /// <returns>修改後的提示訊息</returns>
        public JsonResult MarkerEdit(string jsonManufacturerList)
        {
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            Connector conn = new Connector();
            //Json string 反序列畫
            List<ManufacturerAdditional> manufacturerAdditionalList = JsonConvert.DeserializeObject<List<ManufacturerAdditional>>(jsonManufacturerList);
            TWNewEgg.API.View.Service.DataFormatCheck _dataFormatCheck = new Service.DataFormatCheck();
            List<Manufacturer> _listManufacturer = new List<Manufacturer>();
            TWNewEgg.API.Models.ActionResponse<string> _editResult = new ActionResponse<string>();
            string _strMessage = string.Empty;

            bool _boolCheck = true;

            foreach (var item in manufacturerAdditionalList)
            {
                string _strPhoneNumber = string.Empty;
                try
                {
                    //呼叫解析電話的 Method
                    _strPhoneNumber = AnalysePhoneNumber(item.PhoneNumberDetail).Body;
                    string[] phoneDetail = _strPhoneNumber.Split(',');
                    //區碼
                    item.PhoneRegion = phoneDetail[0];
                    //電話
                    item.Phone = phoneDetail[1];
                    //分機
                    item.PhoneExt = phoneDetail[2];
                    //驗證資料格式是否正確
                    var resultDataCheck = _dataFormatCheck.ValidateInputData(item);
                    //錯誤則記錄哪筆錯誤並停止
                    if (resultDataCheck == API.Models.ManufacturerValidateSummaryResult.Error)
                    {
                        _strMessage = "商家編號為: " + item.SN + "資料錯誤";
                        _boolCheck = false;
                        break;
                    }

                }
                catch (Exception error)
                {
                    _strMessage = error.Message;
                    logger.Info("Manufacturer/MarkerEdit: AnalysePhoneNumber function error: " + error.Message);
                    _boolCheck = false;
                    break;
                }
            }
            //修改資料無誤
            if (_boolCheck == true)
            {
                try
                {
                    //remapping the model
                    _listManufacturer = ManufacturerMapper(manufacturerAdditionalList);
                    //connect to api
                    _editResult = conn.EditManufacturerInfo(_listManufacturer);
                    //edit success; success = 0, error = 1
                    if (_editResult.IsSuccess == true && _editResult.Code == (int)ResponseCode.Success)
                    {
                        _strMessage = _editResult.Msg;
                    }
                    else if (_editResult.IsSuccess == false && _editResult.Code == (int)ResponseCode.Error)
                    {
                        _strMessage = _editResult.Msg;
                    }
                    else
                    {
                        _strMessage = "資料錯誤";
                        logger.Info("Manufacturer/MarkerEdit: EditManufacturerInfo else error");
                    }
                }
                catch (Exception error)
                {
                    logger.Info("Manufacturer/MarkerEdit: EditManufacturerInfo Api error: " + error.Message);
                    _strMessage = "資料錯誤";
                }
            }
            return Json(new { Msg = _strMessage });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manufacturerItemList"></param>
        /// <returns></returns>
        public List<Manufacturer> ManufacturerMapper(List<ManufacturerAdditional> manufacturerItemList)
        {
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

            List<Manufacturer> mapperManufacturerList = new List<Manufacturer>();
            TWNewEgg.API.View.Service.SellerInfoService _sellerInfoService = new Service.SellerInfoService();
            int? _intupdateUserID = _sellerInfoService.UserID;
            if (manufacturerItemList != null)
            {
                foreach (ManufacturerAdditional manufacturerAdditional in manufacturerItemList)
                {
                    Mapper.CreateMap<ManufacturerAdditional, Manufacturer>();
                    Manufacturer manufacturer = new Manufacturer();
                    manufacturer = Mapper.Map<ManufacturerAdditional>(manufacturerAdditional);
                    manufacturer.UpdateUserID = _intupdateUserID;
                    mapperManufacturerList.Add(manufacturer);
                }
            }
            return mapperManufacturerList;
        }
        #region 建立製造商

        private string createManufacturerErrorMessage = string.Empty;

        /// <summary>
        /// 顯示建立製造商 view
        /// </summary>
        /// <returns>建立製造商 view</returns>
        [HttpPost]
        [Filter.PermissionFilter]
        [Filter.LoginAuthorize]
        public ActionResult CreateManufacture()
        {
            // 讀取使用者 ID
            TWNewEgg.API.Models.ActionResponse<int> getUserID = GetUserID();

            // 取得商家 ID
            TWNewEgg.API.Models.ActionResponse<int> getSellerID = GetSellerID();

            // 判斷使用者是否為管理者
            TWNewEgg.API.Models.ActionResponse<bool> getIsAdmin = GetIsAdmin();

            if (getUserID.IsSuccess && getSellerID.IsSuccess && getIsAdmin.IsSuccess)
            {
                // 將是否有管理權限傳至 view
                ViewBag.isAdmin = getIsAdmin.Body;

                if (getIsAdmin.Body)
                {
                    // 連接至 API 的 Connector 
                    TWNewEgg.API.Models.Connector connector = new TWNewEgg.API.Models.Connector();

                    // 取得商家名稱
                    TWNewEgg.API.Models.ActionResponse<string> getSellerName = GetSellerName(getSellerID.Body);

                    // 取得審核結果通知對象清單
                    TWNewEgg.API.Models.ActionResponse<List<API.Models.ManufacturerEmailToListResultModel>> getEmailToList = GetEmailToList(getSellerID.Body);

                    if (getSellerName.IsSuccess && getEmailToList.IsSuccess)
                    {
                        // 將商家名稱傳至 view
                        ViewBag.sellerNsme = getSellerName.Body;

                        // 將審核結果通知對象清單傳至 view
                        ViewBag.emailToList = getEmailToList.Body;
                    }
                    else
                    {
                        createManufacturerErrorMessage = ReloadPageMessage();
                    }
                }
                else
                {
                    // 將商使用者 Email 傳至 view
                    ViewBag.userID = getUserID.Body;
                }
            }
            else
            {
                createManufacturerErrorMessage = ReLoginMessage();
            }

            string createNewManufacture = RenderView("CreateManufacture");
            return Json(new { IsSuccess = true, ViewHtml = createNewManufacture });
            //return View();
        }

        /// <summary>
        /// 建立製造商
        /// </summary>
        /// <param name="manufacturer">建立資料</param>
        /// <param name="autoApprove">自動核准勾選值</param>
        /// <returns>建立結果訊息</returns>
        [HttpPost]
        [Filter.PermissionFilter]
        [Filter.LoginAuthorize]
        public JsonResult CreateManufactureInfo(Manufacturer manufacturer, bool autoApprove)
        {
            // 記錄執行訊息
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            logger.Info("Manufacturer\\CreateManufactureInfo: Start。");

            // 檢查輸入資料格式
            TWNewEgg.API.Models.ActionResponse<bool> checkResult = CheckInputData(manufacturer);

            // 通過輸入檢查才建立製造商
            if (checkResult.Body)
            {
                // 組合製造商 model
                TWNewEgg.API.Models.ActionResponse<List<Manufacturer>> getManufacturer = GetManufacturer(manufacturer, autoApprove);

                if (getManufacturer.IsSuccess)
                {
                    // 連接至 API 的 Connector 
                    TWNewEgg.API.Models.Connector connector = new TWNewEgg.API.Models.Connector();

                    var createResult = connector.CreateManufacturerInfo(getManufacturer.Body);

                    if (createResult.IsSuccess)
                    {
                        logger.Info("Manufacturer\\CreateManufactureInfo: Success。");
                    }
                    else
                    {
                        createManufacturerErrorMessage = "建立失敗。";

                        if (createResult.Msg.IndexOf("You Create Manufacture Url double time:") != -1)
                        {
                            createManufacturerErrorMessage = "製造商網址或統一編號已存在。";
                        }
                        
                        logger.Info("Manufacturer\\CreateUserEmail: Error：" + createResult.Msg);
                    }
                }
                else
                {
                    createManufacturerErrorMessage = getManufacturer.Msg;
                }
            }
            else
            {
                createManufacturerErrorMessage = checkResult.Msg;
            }

            logger.Info("Manufacturer\\CreateManufactureInfo: End。");

            return Json(createManufacturerErrorMessage);
        }

        [HttpPost]
        [Filter.PermissionFilter]
        [Filter.LoginAuthorize]
        public ActionResult ShowUserEmail()
        {
            // 取得商家 ID
            TWNewEgg.API.Models.ActionResponse<int> getSellerID = GetSellerID();

            if (getSellerID.IsSuccess)
            {
                // 取得審核結果通知對象清單
                TWNewEgg.API.Models.ActionResponse<List<API.Models.ManufacturerEmailToListResultModel>> getEmailToList = GetEmailToList(getSellerID.Body);

                if (getEmailToList.IsSuccess)
                {
                    // 將審核結果通知對象清單傳至 view
                    ViewBag.emailToList = getEmailToList.Body;
                }
                else
                {
                    createManufacturerErrorMessage = ReloadPageMessage();
                }
            }
            else
            {
                createManufacturerErrorMessage = ReLoginMessage();
            }

            string createUserEmail = RenderView("CreateUserEmail");
            return Json(new { IsSuccess = true, ViewHtml = createUserEmail });
            //return View();
        }

        [HttpPost]
        [Filter.PermissionFilter]
        [Filter.LoginAuthorize]
        public JsonResult CreateUserEmail(string emailAddress)
        {
            // 記錄執行訊息
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            logger.Info("新增 UserEmail (製造商) 開始。");

            TWNewEgg.API.Models.ActionResponse<int> createResult = new ActionResponse<int>();

            // 讀取使用者 ID
            TWNewEgg.API.Models.ActionResponse<int> getUserID = GetUserID();

            // 取得商家 ID
            TWNewEgg.API.Models.ActionResponse<int> getSellerID = GetSellerID();

            if (getUserID.IsSuccess && getSellerID.IsSuccess)
            {
                logger.Info(string.Format("UserID = {0}; SellerID = {1}.", getUserID.Body, getSellerID.Body));

                if (!string.IsNullOrEmpty(emailAddress) && IsEmailAddress(emailAddress))
                {
                    // 連接至 API 的 Connector 
                    TWNewEgg.API.Models.Connector connector = new TWNewEgg.API.Models.Connector();

                    createResult = connector.CreateUserEmail(getSellerID.Body, emailAddress, getUserID.Body);

                    if (createResult.IsSuccess)
                    {
                        logger.Info("增加成功。");
                    }
                    else
                    {
                        createManufacturerErrorMessage = createResult.Msg;
                        logger.Info(string.Format("增加失敗; Message = {0}.", createResult.Msg));
                    }

                }
                else
                {
                    if (string.IsNullOrEmpty(emailAddress))
                    {
                        createManufacturerErrorMessage = "請輸入電子信箱位址。";
                    }
                    else if (!IsEmailAddress(emailAddress))
                    {
                        createManufacturerErrorMessage = "請輸入一組有效的電子信箱位址(例如：example@example.com)。";
                    }
                }
            }
            else
            {
                createManufacturerErrorMessage = ReLoginMessage();
            }

            logger.Info("新增 UserEmail (製造商) 結束。");

            return Json(new { message = createManufacturerErrorMessage, newUserID = createResult.Body });
        }

        #region 組合 model

        /// <summary>
        /// 組合製造商 model
        /// </summary>
        /// <param name="manufacturer">新建製造商資料</param>
        /// <param name="autoApprove">自動核准勾選值</param>
        /// <returns>製造商 model</returns>
        private TWNewEgg.API.Models.ActionResponse<List<Manufacturer>> GetManufacturer(Manufacturer manufacturer, bool autoApprove)
        {
            // 記錄執行訊息
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            logger.Info("Manufacturer\\GetManufacturer: Start。");

            TWNewEgg.API.Models.ActionResponse<List<Manufacturer>> result = new ActionResponse<List<Manufacturer>>();
            result.Body = new List<Manufacturer>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            // 取得 SellerID
            TWNewEgg.API.Models.ActionResponse<int> getSllerID = GetSellerID();

            // 取得 UserID
            TWNewEgg.API.Models.ActionResponse<int> getInUserID = GetUserID();

            if (getSllerID.IsSuccess && getInUserID.IsSuccess)
            {
                DateTime nowTime = DateTime.Now;

                try
                {
                    manufacturer.SellerID = getSllerID.Body;
                    manufacturer.InUserID = getInUserID.Body;
                    manufacturer.InDate = nowTime;
                    manufacturer.UpdateUserID = getInUserID.Body;
                    manufacturer.UpdateDate = nowTime;
                    manufacturer.ManufactureStatus = GetManufactureStatus(getInUserID.Body, autoApprove);
                
                    result.Body.Add(manufacturer);
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.Msg = TryAgainMessage();
                    logger.Info("Manufacturer\\GetManufacturer: 組合製造商 model 失敗(exception): " + ex.ToString());

                    return result;
                }
            }
            else
            {
                result.IsSuccess = false;
            }

            if (result.IsSuccess)
            {
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Success;
                logger.Info("Manufacturer\\GetManufacturer: Success。");
            }
            else
            {
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                result.Msg = ReLoginMessage();
            }

            logger.Info("Manufacturer\\GetManufacturer: End。");

            return result;
        }

        /// <summary>
        /// 取得 cookie 的 Current Seller ID
        /// </summary>
        /// <returns>Seller ID</returns>
        private TWNewEgg.API.Models.ActionResponse<int> GetSellerID()
        {
            // 記錄執行訊息
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            logger.Info("Manufacturer\\GetSellerID: Start。");

            TWNewEgg.API.Models.ActionResponse<int> result = new TWNewEgg.API.Models.ActionResponse<int>();
            result.Body = 0;
            result.IsSuccess = true;
            result.Msg = string.Empty;

            // 取得 cookie 資訊
            TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();

            try
            {
                result.Body = sellerInfo.currentSellerID;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = ReLoginMessage();
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                logger.Info("Manufacturer\\GetSellerID: Cookie 的 SellerID 讀取失敗(exception)：" + ex.ToString());

                return result;
            }

            if (result.Body == 0)
            {
                result.IsSuccess = false;
                result.Msg = ReLoginMessage();
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                logger.Info("Manufacturer\\GetSellerID: Cookie 的 SellerID 為 0。");
            }
            else
            {
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Success;
                logger.Info("Manufacturer\\GetSellerID: Success。");
            }

            logger.Info("Manufacturer\\GetSellerID: End。");

            return result;
        }

        /// <summary>
        /// 取得 cookie 的 User ID
        /// </summary>
        /// <returns>User ID</returns>
        private TWNewEgg.API.Models.ActionResponse<int> GetUserID()
        {
            // 記錄執行訊息
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            logger.Info("Manufacturer\\GetUserID: Start。");

            TWNewEgg.API.Models.ActionResponse<int> result = new TWNewEgg.API.Models.ActionResponse<int>();
            result.Body = 0;
            result.IsSuccess = true;
            result.Msg = string.Empty;

            // 取得 cookie 資訊
            TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();

            try
            {
                result.Body = sellerInfo.UserID;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = ReLoginMessage();
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                logger.Info("Manufacturer\\GetUserID: Cookie 的 UserID 讀取失敗(exception)：" + ex.ToString());

                return result;
            }

            if (result.Body == 0)
            {
                result.IsSuccess = false;
                result.Msg = ReLoginMessage();
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                logger.Info("Manufacturer\\GetUserID: Cookie 的 UserID 為 0。");
            }
            else
            {
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Success;
                logger.Info("Manufacturer\\GetUserID: Success。");
            }

            logger.Info("Manufacturer\\GetUserID: End。");

            return result;
        }

        /// <summary>
        /// 取得 cookie 的 User Email
        /// </summary>
        /// <returns>User Email</returns>
        private TWNewEgg.API.Models.ActionResponse<string> GetUserEmail()
        {
            // 記錄執行訊息
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            logger.Info("Manufacturer\\UserEmail: Start。");

            TWNewEgg.API.Models.ActionResponse<string> result = new TWNewEgg.API.Models.ActionResponse<string>();
            result.Body = string.Empty;
            result.IsSuccess = true;
            result.Msg = string.Empty;

            // 取得 cookie 資訊
            TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();

            try
            {
                result.Body = sellerInfo.UserEmail;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = ReLoginMessage();
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                logger.Info("Manufacturer\\UserEmail: Cookie 的 UserEmail 讀取失敗(exception)：" + ex.ToString());

                return result;
            }

            if (result.Body == string.Empty)
            {
                result.IsSuccess = false;
                result.Msg = ReLoginMessage();
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                logger.Info("Manufacturer\\UserEmail: Cookie 的 UserEmail 為空值。");
            }
            else
            {
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Success;
                logger.Info("Manufacturer\\UserEmail: Success。");
            }

            logger.Info("Manufacturer\\UserEmail: End。");

            return result;
        }

        /// <summary>
        /// 取得 cookie 的 IsAdmin
        /// </summary>
        /// <returns>IsAdmin</returns>
        private TWNewEgg.API.Models.ActionResponse<bool> GetIsAdmin()
        {
            // 記錄執行訊息
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            logger.Info("Manufacturer\\GetIsAdmin: Start。");

            TWNewEgg.API.Models.ActionResponse<bool> result = new TWNewEgg.API.Models.ActionResponse<bool>();
            result.Body = false;
            result.IsSuccess = true;
            result.Msg = string.Empty;

            // 取得 cookie 資訊
            TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();

            try
            {
                result.Body = sellerInfo.IsAdmin;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = ReLoginMessage();
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                logger.Info("Manufacturer\\GetIsAdmin: Cookie 的 IsAdmin 讀取失敗(exception)：" + ex.ToString());

                return result;
            }

            result.Code = (int)TWNewEgg.API.Models.ResponseCode.Success;
            logger.Info("Manufacturer\\GetIsAdmin: Success。");
            logger.Info("Manufacturer\\GetIsAdmin: End。");

            return result;
        }

        /// <summary>
        /// 取得製造商狀態
        /// </summary>
        /// <param name="userID">使用者 ID</param>
        /// <param name="autoApprove">自動核准的勾選值</param>
        /// <returns>製造商狀態</returns>
        private string GetManufactureStatus(int userID, bool autoApprove)
        {
            string manufactureStatus = "P";

            // 如果有勾選自動核准，則再確認是否有管理者權限
            if (autoApprove)
            {
                // 查詢管理者權限
                TWNewEgg.API.Models.ActionResponse<bool> getIsAdmin = GetIsAdmin();

                if (getIsAdmin.Body)
                {
                    manufactureStatus = "A";
                }
            }

            return manufactureStatus;
        }

        /// <summary>
        /// 取得商家名稱
        /// </summary>
        /// <param name="sellerID">商家 ID</param>
        /// <returns>商家名稱</returns>
        private TWNewEgg.API.Models.ActionResponse<string> GetSellerName(int sellerID)
        {
            // 記錄執行訊息
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            logger.Info("Manufacturer\\GetSellerName: Start。");

            TWNewEgg.API.Models.ActionResponse<string> result = new TWNewEgg.API.Models.ActionResponse<string>();
            result.Body = string.Empty;
            result.IsSuccess = true;
            result.Msg = string.Empty;

            // 連接至 API 的 Connector 
            TWNewEgg.API.Models.Connector connector = new TWNewEgg.API.Models.Connector();

            try
            {
                // 取得商家名稱
                result = connector.GetSellerName(sellerID);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                logger.Info("Manufacturer\\GetSellerName: 取得商家名稱失敗(exception)：" + ex.ToString());
            }

            if (result.IsSuccess)
            {
                result.Msg = string.Empty;
                logger.Info("Manufacturer\\GetSellerName: Success。");
            }
            else
            {
                logger.Info("Manufacturer\\GetSellerName: 取得商家名稱失敗：" + result.Msg);
                result.Msg = ReloadPageMessage();
            }

            logger.Info("Manufacturer\\GetSellerName: End。");

            return result;
        }

        /// <summary>
        /// 取得審核結果通知對象清單
        /// </summary>
        /// <param name="sellerID">商家 ID</param>
        /// <returns>審核結果通知對象清單</returns>
        private TWNewEgg.API.Models.ActionResponse<List<API.Models.ManufacturerEmailToListResultModel>> GetEmailToList(int sellerID)
        {
            // 記錄執行訊息
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            logger.Info("Manufacturer\\GetEmailToList: Start。");

            TWNewEgg.API.Models.ActionResponse<List<API.Models.ManufacturerEmailToListResultModel>> result = new TWNewEgg.API.Models.ActionResponse<List<API.Models.ManufacturerEmailToListResultModel>>();
            result.Body = new List<ManufacturerEmailToListResultModel>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            // 連接至 API 的 Connector 
            TWNewEgg.API.Models.Connector connector = new TWNewEgg.API.Models.Connector();

            try
            {
                // 取得商家名稱
                result = connector.GetEmailToList(sellerID);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                logger.Info("Manufacturer\\GetEmailToList: 審核結果通知對象清單失敗(exception)：" + ex.ToString());
            }

            if (result.IsSuccess)
            {
                result.Msg = string.Empty;
                logger.Info("Manufacturer\\GetEmailToList: Success。");
            }
            else
            {
                logger.Info("Manufacturer\\GetEmailToList: 審核結果通知對象清單失敗：" + result.Msg);
                result.Msg = ReloadPageMessage();
            }

            logger.Info("Manufacturer\\GetEmailToList: End。");

            return result;
        }

        #endregion 組合 model

        #region 錯誤訊息

        private string TryAgainMessage()
        {
            return "伺服器忙線中，請稍後再試，若仍持續發生此錯誤，請聯繫客服人員。";
        }

        private string ReLoginMessage()
        {
            return "登入資訊錯誤，請先登出後，再重新登入繼續進行操作，若仍發生此錯誤，請聯繫客服人員。";
        }

        private string CreateFail()
        {
            return "建立失敗，請檢查資料是否正確，再繼續進行操作。";
        }

        private string ReloadPageMessage()
        {
            return "畫面讀取錯誤，請重新整理畫面後，再重新進行操作，若仍發生此錯誤，請聯繫客服人員。";
        }

        #endregion 錯誤訊息

        #endregion 建立製造商

        #region 檢查輸入資料

        /// <summary>
        /// 檢查輸入資料格式
        /// </summary>
        /// <param name="manufacturer">建立資料</param>
        /// <returns>檢查結果</returns>
        private TWNewEgg.API.Models.ActionResponse<bool> CheckInputData(Manufacturer manufacturer)
        {
            // 記錄執行訊息
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            logger.Info("Manufacturer\\CheckInputData: Start。");

            TWNewEgg.API.Models.ActionResponse<bool> result = new ActionResponse<bool>();
            result.Body = false;
            result.IsSuccess = true;
            result.Msg = string.Empty;

            try
            {
                // 檢查項目
                result.Msg += CheckMailTo(manufacturer.UserID.Value);
                result.Msg += CheckManufactureName(manufacturer.ManufactureName);

                // 檢查製造商網址或統一編號
                var checkManufactureURLResult = CheckManufactureURL(manufacturer.ManufactureURL);
                // 更新造商網址或統一編號
                manufacturer.ManufactureURL = checkManufactureURLResult.Body;
                result.Msg += checkManufactureURLResult.Msg;

                result.Msg += CheckSupportEmail(manufacturer.SupportEmail);
                result.Msg += CheckPhoneRegion(manufacturer.PhoneRegion);
                result.Msg += CheckPhone(manufacturer.Phone);
                result.Msg += CheckphoneExt(manufacturer.PhoneExt);

                // 檢查製造商支援網址
                var checkSupportURLResult = CheckSupportURL(manufacturer.supportURL);
                // 更新製造商支援網址
                manufacturer.supportURL = checkSupportURLResult.Body;
                result.Msg += checkSupportURLResult.Msg;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = ReloadPageMessage();
                logger.Info("Manufacturer\\CheckInputData: exception：" + ex.ToString());
            }

            // 若沒有錯誤訊息，才算通過檢查
            if (result.IsSuccess && string.IsNullOrEmpty(result.Msg))
            {
                result.Body = true;
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Success;
                logger.Info("Manufacturer\\CheckInputData: Success。");
            }
            else
            {
                result.Body = false;
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
            }

            logger.Info("Manufacturer\\CheckInputData: End。");

            return result;
        }

        /// <summary>
        /// 檢查審核結果通知對象
        /// </summary>
        /// <param name="mailToUserID">審核結果通知對象的 User ID</param>
        /// <returns>錯誤訊息</returns>
        private string CheckMailTo(int mailToUserID)
        {
            string errorMessage = string.Empty;

            TWNewEgg.API.Models.ActionResponse<bool> getIsAdmin = GetIsAdmin();

            if (getIsAdmin.Body && mailToUserID <= 0)
            {
                errorMessage += "請選擇審核結果通知對象。";
            }
            else if (mailToUserID <= 0)
            {
                errorMessage += ReLoginMessage();
            }

            return errorMessage;
        }

        /// <summary>
        /// 檢查製造商名稱
        /// </summary>
        /// <param name="manufactureName">製造商名稱</param>
        /// <returns>錯誤訊息</returns>
        private string CheckManufactureName(string manufactureName)
        {
            string errorMessage = string.Empty;

            if (string.IsNullOrEmpty(manufactureName))
            {
                errorMessage += "製造商名稱為必填欄位。";
            }

            return errorMessage;
        }

        /// <summary>
        /// 檢查製造商網址或統一編號
        /// </summary>
        /// <param name="value">製造商網址或統一編號</param>
        /// <returns>製造商網址或統一編號及錯誤訊息</returns>
        private TWNewEgg.API.Models.ActionResponse<string> CheckManufactureURL(string value)
        {
            TWNewEgg.API.Models.ActionResponse<string> result = new TWNewEgg.API.Models.ActionResponse<string>();
            result.IsSuccess = true;
            result.Code = (int)TWNewEgg.API.Models.ResponseCode.Success;

            if (string.IsNullOrEmpty(value))
            {
                result.Msg += "製造商網址或統一編號為必填欄。";
            }
            else if (IsUrlCharactersAndNumber(value) == false)
            {
                result.Msg += "製造商網址或統一編號只能是英文字母、數字、#、%、&、-、.、/、:、=、?、_。";
            }
            else
            {
                // 判斷是統一編號還是網址：用 double.TryParse 去轉，若轉成功就是統一編號，不成功就網址處理
                double vatNumber = -1;

                try
                {
                    double.TryParse(value, out vatNumber);
                }
                catch(Exception ex)
                {
                    // 記錄執行訊息
                    log4net.ILog logger;
                    logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
                    logger.Info("Manufacturer\\CheckManufactureURL: TyrParse Error(exception)：" + ex.ToString());
                }


                if (vatNumber <= 0)
                {
                    // 判斷網址抬頭是否存在，沒有則加上
                    value = CheckURLTitle(value);

                    // 檢查文字內容是否只符合網址格式
                    if (!IsUrl(value))
                    {
                        result.Msg += "製造商支援網址請輸入一個有效的網址(例如：http://www.example.com)。";
                    }

                    // 檢查網址最後一個字元是否為斜線
                    if (CheckURLLastWordIsSlash(value))
                    {
                        result.Msg += "製造商網址最後一個字元不可以為 / ，請檢查修正。";
                    }
                }
            }

            result.Body = value;
            
            return result;
        }

        /// <summary>
        /// 檢查製造商支援信箱
        /// </summary>
        /// <param name="supportEmail">製造商支援信箱</param>
        /// <returns>錯誤訊息</returns>
        private string CheckSupportEmail(string supportEmail)
        {
            string errorMessage = string.Empty;

            // 有值，才驗證是否符合信箱格式
            if (!string.IsNullOrEmpty(supportEmail))
            {
                // 檢查是不是「只有英文、數字、部分特殊字符」而且「符合信箱格式」
                if (!IsEmailAddress(supportEmail))
                {
                    errorMessage += "製造商支援信箱請輸入一組有效的電子信箱(例如：example@example.com)。";
                }
            }

            return errorMessage;
        }

        /// <summary>
        /// 檢查製造商支援電話區碼
        /// </summary>
        /// <param name="phoneRegion">製造商支援電話區碼</param>
        /// <returns>錯誤訊息</returns>
        private string CheckPhoneRegion(string phoneRegion)
        {
            string errorMessage = string.Empty;

            if (!string.IsNullOrEmpty(phoneRegion) && !ISOnlyNumber(phoneRegion))
            {
                errorMessage += "製造商支援電話區碼只能是數字。";
            }

            return errorMessage;
        }

        /// <summary>
        /// 檢查製造商支援電話
        /// </summary>
        /// <param name="phone">製造商支援電話</param>
        /// <returns>錯誤訊息</returns>
        private string CheckPhone(string phone)
        {
            string errorMessage = string.Empty;

            if (!string.IsNullOrEmpty(phone) && !ISOnlyNumber(phone))
            {
                errorMessage += "製造商支援電話只能是數字。";
            }

            return errorMessage;
        }

        /// <summary>
        /// 檢查製造商支援電話分機
        /// </summary>
        /// <param name="phoneExt">製造商支援電話分機</param>
        /// <returns>錯誤訊息</returns>
        private string CheckphoneExt(string phoneExt)
        {
            string errorMessage = string.Empty;

            if (!string.IsNullOrEmpty(phoneExt) && !ISOnlyNumber(phoneExt))
            {
                errorMessage += "製造商支援電話分機只能是數字。";
            }

            return errorMessage;
        }

        /// <summary>
        /// 檢查製造商支援網址
        /// </summary>
        /// <param name="supportURL">製造商支援網址</param>
        /// <returns>製造商支援網址及錯誤訊息</returns>
        private TWNewEgg.API.Models.ActionResponse<string> CheckSupportURL(string supportURL)
        {
            TWNewEgg.API.Models.ActionResponse<string> result = new TWNewEgg.API.Models.ActionResponse<string>();
            result.IsSuccess = true;
            result.Code = (int)TWNewEgg.API.Models.ResponseCode.Success;
            
            // 有值，才驗證是否符合網址格式
            if (!string.IsNullOrEmpty(supportURL) && supportURL != "http://" && supportURL != "https://")
            {
                supportURL = CheckURLTitle(supportURL);

                // 檢查是否符合網址格式
                if (!IsUrl(supportURL))
                {
                    result.Msg = "製造商支援網址請輸入一個有效的網址(例如：http://www.example.com)。";
                }
            }

            result.Body = supportURL;

            return result;
        }

        /// <summary>
        /// 判斷網址抬頭是否存在，沒有則加上
        /// </summary>
        /// <param name="strUrl">待檢查網址</param>
        /// <returns>網址</returns>
        private string CheckURLTitle(string strUrl)
        {
            if (!(0 == strUrl.IndexOf(@"http://") || 0 == strUrl.IndexOf(@"https://")))
            {
                strUrl = string.Format("http://{0}", strUrl);
            }

            return strUrl;
        }

        /// <summary>
        /// 檢查文字內容是否只有英文字母、數字、網址會用到的符號
        /// </summary>
        /// <param name="value">待檢查內容</param>
        /// <returns>檢查結果</returns>
        private bool IsUrlCharactersAndNumber(string value)
        {
            Regex reg = new Regex(@"^[\x23\x25\x26\x2D-\x3A\x3D\x3F\x41-\x5A\x5F\x61-\x7A]+$");

            if (reg.IsMatch(value))
            {
                return true;
            }
            
            return false;
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
        /// 檢查文字內容是否只符合網址格式
        /// </summary>
        /// <param name="value">待檢查內容</param>
        /// <returns>檢查結果</returns>
        private bool IsUrl(string value)
        {
            Regex reg = new Regex(@"^http[s]?://[\w-_.%/:?=&#]+$");

            if (reg.IsMatch(value))
            {
                return true;
            }

            return false;
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
        /// 檢查文字內容是否只符合信箱格式
        /// </summary>
        /// <param name="value">待檢查內容</param>
        /// <returns>檢查結果</returns>
        private bool IsEmailAddress(string value)
        {
            Regex reg = new Regex(@"\w+([-+.']\w+)*@\w+([-+']\w+)*\.(\w+([-+']\w)*\.)*[A-Za-z]{2,4}$");

            if (reg.IsMatch(value))
            {
                return true;
            }

            return false;
        }
        
        #endregion 檢查輸入資料

        #region 製造商審核

        public JsonResult ManufactureUpdateStatus(/*KendoGridRequest request,*/ string command, List<Manufacturer> updateModel)
        {
            List<ManufactureUpdateStatusDataAdd> updateStatusDataList = new List<ManufactureUpdateStatusDataAdd>();
            if (updateModel != null)
            {
                int rejectDefault1Count = 0;
                int rejectDefault2Count = 0;
                int rejectDefault3Count = 0;
                foreach (var update_Index in updateModel)
                {
                    ManufactureUpdateStatusDataAdd data = new ManufactureUpdateStatusDataAdd();
                    data.ManufactureName = update_Index.ManufactureName;
                    data.ManufactureURL = update_Index.ManufactureURL;
                    data.DeclineReason = update_Index.DeclineReason;
                    data.SN = update_Index.SN;
                    data.UpdateCommand = command;
                    if (!string.IsNullOrEmpty(update_Index.DeclineReason))
                    {
                        if (update_Index.DeclineReason.IndexOf("重複") >= 0)
                        {
                            data.rejectDefault1 = true;
                            rejectDefault1Count++;
                        }

                        if (update_Index.DeclineReason.IndexOf("無效網址") >= 0)
                        {
                            data.rejectDefault2 = true;
                            rejectDefault2Count++;
                        }

                        if (update_Index.DeclineReason.IndexOf("連結至其他平台") >= 0)
                        {
                            data.rejectDefault3 = true;
                            rejectDefault3Count++;
                        }
                    }

                    updateStatusDataList.Add(data);
                }

                int updateModelCount = updateModel.Count;
                foreach (ManufactureUpdateStatusDataAdd updateStatusData in updateStatusDataList)
                {
                    if (rejectDefault1Count == updateModelCount)
                    {
                        updateStatusData.rejectAllDefault1 = true;
                    }

                    if (rejectDefault2Count == updateModelCount)
                    {
                        updateStatusData.rejectAllDefault2 = true;
                    }

                    if (rejectDefault3Count == updateModelCount)
                    {
                        updateStatusData.rejectAllDefault3 = true;
                    }
                }
            }
            //else
            //{
            //    updateModel = new List<Manufacturer>();
            //}
            ViewBag.updateStatusDataList = updateStatusDataList;
            string manufacturerStatusUpdate = RenderView("ManufactureStatusUpdate");
            return Json(new { IsSuccess = true, ViewHtml = manufacturerStatusUpdate });
            //return Json(new { returnModel = updateStatusDataList, total = updateStatusDataList.Count });
            //return Json(new KendoGrid<ManufactureUpdateStatusDataAdd>(request, updateStatusDataList));
        }

        public JsonResult UpdateManufactureUpdateStatus(string Command, List<Manufacturer> updateModel)
        {
            ManufacturerUpdateStatusInfo manufacturerUpdateModel = new ManufacturerUpdateStatusInfo();
            Connector conn = new Connector();
            TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();

            List<ManufacturerUpdateStatusData> updateStatusData = new List<ManufacturerUpdateStatusData>();

            foreach (var update_Index in updateModel)
            {
                ManufacturerUpdateStatusData data = new ManufacturerUpdateStatusData();
                data.ManufactureName = update_Index.ManufactureName;
                data.ManufactureURL = update_Index.ManufactureURL;
                data.DeclineReason = update_Index.DeclineReason;

                updateStatusData.Add(data);
            }

            manufacturerUpdateModel.UpdateUserID = sellerInfo.UserID;
            manufacturerUpdateModel.UpdateList = updateStatusData;
            manufacturerUpdateModel.Command = (ManufacturerUpdateStatusCommand)Enum.Parse(typeof(ManufacturerUpdateStatusCommand), Command);
            manufacturerUpdateModel.UpdateDate = DateTime.Now;

            try
            {
                var updateResult = conn.UpdateStatus(manufacturerUpdateModel);

                return Json(updateResult.Msg);
            }
            catch (Exception ex)
            {
                return Json("發生意外錯誤，請稍後再試");
            }
        }

        #endregion

    }
}
