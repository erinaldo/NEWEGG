using KendoGridBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.DB.TWSELLERPORTALDB.Models;
using System.Runtime.Caching;

namespace TWNewEgg.API.View.Controllers
{
    public class SellerSearchController : Controller
    {
        //
        // GET: /SellerSearch/

        TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();
        TWNewEgg.API.Models.Connector connector = new TWNewEgg.API.Models.Connector();
        TWNewEgg.API.View.ServiceAPI.APIConnector APIConnector = new ServiceAPI.APIConnector();
        TWNewEgg.API.View.Service.AES aes = new Service.AES();
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        
        private static ObjectCache _cache = MemoryCache.Default;

        public List<AutoCompleteModel> sellerList
        {
            get
            {
                if (!_cache.Contains("SellerList"))
                    RefreshSellerList();
                return _cache.Get("SellerList") as List<AutoCompleteModel>;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void RefreshSellerList()
        {
            var list_Seller = GetAllComplete();

            CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
            cacheItemPolicy.AbsoluteExpiration = DateTime.Now.AddMinutes(10);

            if (list_Seller != null)
            {
                if (!_cache.Contains("SellerList"))
                {
                    _cache.Add("SellerList", list_Seller, cacheItemPolicy);
                }
                else
                {
                    _cache.Set("SellerList", list_Seller, cacheItemPolicy);
                }
            }
        }

        private List<AutoCompleteModel> GetAllComplete()
        {
            TWNewEgg.API.Models.SellerRMQuery smrq = new API.Models.SellerRMQuery();
            API.Models.ActionResponse<List<TWNewEgg.API.Models.VM_Seller_BasicInfo>> _searchAllDataResult = new API.Models.ActionResponse<List<API.Models.VM_Seller_BasicInfo>>();
            List<TWNewEgg.API.View.AutoCompleteModel> _autoCompleteModel = new List<AutoCompleteModel>();
            
            try
            {
                _searchAllDataResult = connector.GetSeller_BasicInfosbyQuery("", "", smrq);
                _autoCompleteModel = bindMode(_searchAllDataResult.Body);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + " , " + ex.StackTrace);
            }

            return _autoCompleteModel;
        }

        public string changeStatus(string status)
        {
            if (status == "A")
            {
                return "Active";
            }
            else if (status == "I")
            {
                return "Inactive";
            }
            else
            {
                return "Close";
            }
        }

        /// <summary>
        /// Autocomplete model bind 轉換用
        /// </summary>
        /// <param name="vM_Model"></param>
        /// <returns></returns>
        public List<TWNewEgg.API.View.AutoCompleteModel> bindMode(List<TWNewEgg.API.Models.VM_Seller_BasicInfo> vM_Model)
        {
            List<TWNewEgg.API.View.AutoCompleteModel> _autoCompleteModel = new List<AutoCompleteModel>();
            if (vM_Model != null)
            {
                foreach (var item in vM_Model)
                {
                    TWNewEgg.API.View.AutoCompleteModel _autoString = new AutoCompleteModel();
                    _autoString.sellerName = item.SellerName + " (" + item.SellerID + ") (" + changeStatus(item.SellerStatus) + ")";
                    _autoString.sellerid = item.SellerID;
                    _autoString.AccountTypeCode = item.AccountTypeCode;
                    _autoString.SellerStatus = item.SellerState;
                    _autoCompleteModel.Add(_autoString);
                }
            }
            return _autoCompleteModel;
        }

        /// <summary>
        /// Autocomplete model bind 轉換用
        /// </summary>
        /// <param name="vM_Model"></param>
        /// <returns></returns>
        public List<TWNewEgg.API.View.AutoCompleteModel> bindMode(List<TWNewEgg.API.Models.DomainModel.AutoCompleteModel> vM_Model)
        {
            List<TWNewEgg.API.View.AutoCompleteModel> _autoCompleteModel = new List<AutoCompleteModel>();
            if (vM_Model != null)
            {
                foreach (var item in vM_Model)
                {
                    TWNewEgg.API.View.AutoCompleteModel _autoString = new AutoCompleteModel();
                    _autoString.sellerName = item.sellerName + " (" + item.sellerid + ") (" + changeStatus(item.SellerStatus) + ")";
                    _autoString.sellerid = item.sellerid;
                    _autoString.AccountTypeCode = item.AccountTypeCode;
                    _autoString.SellerStatus = item.SellerStatus;
                    _autoCompleteModel.Add(_autoString);
                }
            }
            return _autoCompleteModel;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public JsonResult SellerSearchAutoComplete(string text2)
        {
            TWNewEgg.API.Models.SellerRMQuery smrq = new API.Models.SellerRMQuery();
            API.Models.ActionResponse<List<TWNewEgg.API.Models.VM_Seller_BasicInfo>> _searchAllDataResult = new API.Models.ActionResponse<List<API.Models.VM_Seller_BasicInfo>>();
            List<TWNewEgg.API.View.AutoCompleteModel> _autoCompleteModel = new List<AutoCompleteModel>();

            try
            {
                if (sellerList != null)
                {
                    _autoCompleteModel = sellerList;
                }
                else
                {
                    this.RefreshSellerList();
                    _autoCompleteModel = sellerList;
                }

                //_searchAllDataResult = connector.GetSeller_BasicInfosbyQuery("", "", smrq);
                //_autoCompleteModel = bindMode(_searchAllDataResult.Body);

                if (!string.IsNullOrWhiteSpace(text2))
                {
                    _autoCompleteModel = _autoCompleteModel.Where(p => p.sellerName.IndexOf(text2, StringComparison.OrdinalIgnoreCase) != -1).OrderBy(x => x.sellerName).ToList();
                }
                else
                {

                }
            }
            catch (Exception error)
            {
                logger.Info("SellerSearch\\SellerSearchAutoComplete error: " + error.Message + ", " +error.StackTrace);
            }
            var resultM = _autoCompleteModel.Select(p => new TWNewEgg.API.Models.SellerRMQuery
            {
                SellerID = p.sellerid,
                SellerName = p.sellerName
            });

            resultM = resultM.OrderBy(p => p.SellerName).ToList();
            return Json(resultM, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="smrq"></param>
        /// <param name="type"></param>
        /// <param name="searchword"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CancelSearch()
        {           
            try
            {
                if (Request.Cookies["RSD"] != null)
                {
                    Response.Cookies["RSD"].Value = aes.AesEncrypt("0");
                }
                if (Request.Cookies["CSD"] != null)
                {
                    Response.Cookies["CSD"].Value = aes.AesEncrypt(sellerInfo.SellerID.ToString());
                }
                if (Request.Cookies["ATC"] != null)
                {
                    Response.Cookies["ATC"].Value = aes.AesEncrypt(sellerInfo.LoginAccountType);
                }
                return Json("[Success]");
            }
            catch (Exception error)
            {
                logger.Info("SellerSearch/CancelSearch error: "  + error.Message);
                return Json("[Error]");
            }
            
        }
        [HttpPost]
        public JsonResult SearchAllSeller(KendoGridRequest request, TWNewEgg.API.Models.SellerRMQuery smrq, int type, string searchword)
        {
            TWNewEgg.API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>> BasicInfoResult = new TWNewEgg.API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>>();
            API.Models.ActionResponse<List<TWNewEgg.API.Models.VM_Seller_BasicInfo>> _searchAllDataResult = new TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.VM_Seller_BasicInfo>>();
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            //以SellerID 為查詢後資料排序條件
            smrq.SortField = "SellerID";
            smrq.StartRowIndex = (request.Page - 1) * request.PageSize;
            
            //分頁大小
            smrq.PageSize = request.PageSize;
            //return message
            string _strMsg = string.Empty;
            
            if (string.IsNullOrEmpty(searchword) == false)
            {
                searchword = searchword.Trim();
            }
            //type = 0->商家編號; type = 1->商家名稱; type = 2 -> 電子郵件(Email)
            if (type == 0)
            {
                if (string.IsNullOrEmpty(searchword) == false)
                {
                    int _intsellerid = 0;
                    bool _boolToInt = int.TryParse(searchword, out _intsellerid);
                    if (_boolToInt == false)
                    {
                        _strMsg = "請輸入正確的商家編號";
                        return Json(new { returnModel = _searchAllDataResult.Body, total = _searchAllDataResult.Code, msg = _strMsg, page = request.Page });
                    }
                    else
                    {
                        //convert searchword to int, if error _intsellerid will be 0
                        int.TryParse(searchword, out _intsellerid);
                        //give sellerName to smrp.SellerName as search condition
                        smrq.SellerID = _intsellerid;
                    }
                }
                
            }
            else if (type == 1)
            {
                //give sellerName to smrp.SellerName as search condition
                smrq.SellerName = searchword;
            }
            else if (type == 2)
            {
                //give UserEmail to smrp.UserEmail as search condition
                smrq.UserEmail = searchword;
            }
            else
            {
                logger.Info("SellerSearch/SearchAllSeller error: else error");
            }
            try
            {
                //connect to api
                _searchAllDataResult = connector.GetSeller_BasicInfosbyQuery("", "", smrq);
                _strMsg = _searchAllDataResult.Msg;
                _searchAllDataResult.Body = _searchAllDataResult.Body.OrderBy(p => p.SellerID).ToList();

                this.RefreshSellerList();
            }
            catch (Exception error)
            {
                logger.Info("SellerSearch/SearchAllSeller api connect error: " + error.Message);
            }

            return Json(new { returnModel = _searchAllDataResult.Body, total = _searchAllDataResult.Code, msg = _strMsg, page = request.Page });
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //[Filter.LoginAuthorize]


        [HttpGet]
        public ActionResult SearchBar()
        {
            TWNewEgg.API.View.Service.AES aes = new Service.AES();
            string _strSellerInfo = string.Empty;
            TWNewEgg.API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>> _sellerBasicInfo = new API.Models.ActionResponse<List<Seller_BasicInfo>>();
            bool isSellerExist = true;
            if (sellerInfo.IsAdmin == true)
            {
                try
                {
                    _sellerBasicInfo = connector.GetAccountInfo(0, sellerInfo.currentSellerID.ToString());
                    if (_sellerBasicInfo.Body.Count == 0 || _sellerBasicInfo.Body == null)
                    {
                        isSellerExist = false;
                    }
                    else
                    {
                        ViewBag.Seid = _sellerBasicInfo.Body[0].SellerID;
                        _strSellerInfo = _sellerBasicInfo.Body[0].SellerName + " (" + _sellerBasicInfo.Body[0].SellerID + ") (" + _sellerBasicInfo.Body[0].SellerStatus + ")";
                        Response.Cookies["CSD"].Value = aes.AesEncrypt(_sellerBasicInfo.Body[0].SellerID.ToString());
                        Response.Cookies["ATC"].Value = aes.AesEncrypt(_sellerBasicInfo.Body[0].AccountTypeCode);
                        ViewBag.sellerInfo = _strSellerInfo;
                    }
                }
                catch (Exception error)
                {
                    logger.Info("SellerSearch/SearchBar connect GetAccountInfo api error: " + error.Message);
                    isSellerExist = false;
                }
                
                if (isSellerExist == false)
                {
                    ClearCurrentSellerID();
                    return PartialView();
                }
                
                return PartialView();
            }
            return null;
        }

        /// <summary>
        /// Cookie內容清除回登入者的SellerID 、 AccountType
        /// </summary>
        public void ClearCurrentSellerID()
        {
            ViewBag.IsRember = false;
            ViewBag.ID = sellerInfo.SellerID;
            if (Request.Cookies["CSD"] != null)
            {
                Response.Cookies["CSD"].Value = aes.AesEncrypt(sellerInfo.SellerID.ToString());
            }
            if (Request.Cookies["ATC"] != null)
            {
                Response.Cookies["ATC"].Value = aes.AesEncrypt(sellerInfo.LoginAccountType);
            }
            //回寫cookie的RSD值回0(沒有勾選記住我)
            Response.Cookies["RSD"].Value = aes.AesEncrypt("0");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="csd"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SettingCSD(string csd)
        {
            // 加解密
            TWNewEgg.API.Models.SellerRMQuery smrq = new API.Models.SellerRMQuery();
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            //設定SellerID為預設排序條件
            smrq.SortField = "SellerID";
            if (string.IsNullOrEmpty(csd))
            {
                return Json(new { IsSuccess = false });
            }
            try
            {
                int _intCSD = 0;
                int.TryParse(csd, out _intCSD);
                //輸入的值或選擇的資料不是數字
                if (_intCSD == 0)
                {
                    List<TWNewEgg.API.View.AutoCompleteModel> _autoCompleteModel = new List<AutoCompleteModel>();
                    API.Models.ActionResponse<List<TWNewEgg.API.Models.VM_Seller_BasicInfo>> _searchAllDataResult = new API.Models.ActionResponse<List<API.Models.VM_Seller_BasicInfo>>();
                    //呼叫API並檢查資料是否存在
                    _searchAllDataResult = connector.GetSeller_BasicInfosbyQuery("", "", smrq);
                    //組合要用來比對資料的method
                    _autoCompleteModel = bindMode(_searchAllDataResult.Body);
                    //檢查資料是否存在
                    var searchInMode = _autoCompleteModel.Where(p => p.sellerName == csd).FirstOrDefault();
                    //資料不存在
                    if (searchInMode == null)
                    {
                        //Response.Cookies["CSD"].Value = aes.AesEncrypt(sellerInfo.SellerID.ToString());
                        logger.Info("SellerSearch/SettingCSD: 查詢資料庫裡沒有這一筆資料");
                        return Json(new { IsSuccess = false });
                    }
                    else
                    {
                        Response.Cookies["CSD"].Value = aes.AesEncrypt(searchInMode.sellerid.ToString());
                        Response.Cookies["ATC"].Value = aes.AesEncrypt(searchInMode.AccountTypeCode.ToString());
                        //Response.Cookies["SS"].Value = aes.AesEncrypt(searchInMode.SellerStatus.ToString());

                        return Json(new { IsSuccess = true });
                    }
                }
                else
                {
                    API.Models.ActionResponse<List<TWNewEgg.API.Models.VM_Seller_BasicInfo>> _searchAllDataResult = new TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.VM_Seller_BasicInfo>>();
                    //輸入的資料已經是數字
                    smrq.SellerID = _intCSD;
                    //查詢資料是否存在
                    _searchAllDataResult = connector.GetSeller_BasicInfosbyQuery("", "", smrq);
                    if (_searchAllDataResult.Body == null)
                    {
                        //Response.Cookies["CSD"].Value = aes.AesEncrypt(sellerInfo.currentSellerID.ToString());
                        logger.Info("SellerSearch/SettingCSD: csd = " + csd);
                        return Json(new { IsSuccess = false });
                    }
                    else
                    {
                        Response.Cookies["CSD"].Value = aes.AesEncrypt(csd.ToString());
                        Response.Cookies["ATC"].Value = aes.AesEncrypt(_searchAllDataResult.Body[0].AccountTypeCode);
                        return Json(new { IsSuccess = true });
                    }
                }
            }
            catch (Exception e)
            {
                logger.Info("SellerSearch/SettingCSD error: " + e.Message + "[StackTrace]:" + e.StackTrace);
                return Json(new { IsSuccess = false });
            }
        }

        [HttpPost]
        public JsonResult SettingCSDRember(int rember)
        {
            TWNewEgg.API.View.Service.AES aes = new Service.AES();

            try
            {
                Response.Cookies["RSD"].Value = aes.AesEncrypt(rember.ToString());
                return Json(new { IsSuccess = true });
            }
            catch (Exception e)
            {
                logger.Info("SellerSearch/SettingCSDRember error: " + e.Message);
                return Json(new { IsSuccess = false });
            }

        }
    }
}
