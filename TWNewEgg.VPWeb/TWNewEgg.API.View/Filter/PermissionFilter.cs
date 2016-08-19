using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;

namespace TWNewEgg.API.View.Filter
{
    /// <summary>
    /// 判斷用者權限
    /// </summary>
    public class PermissionFilter : ActionFilterAttribute
    {
        private static ILog log = LogManager.GetLogger(typeof(PermissionFilter));

        /// <summary>
        /// 判斷登入者是否有權限使用此畫面
        /// </summary>
        /// <param name="filterContext">HttpContext</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Check UserID & AccessToken
            bool hasPermission = false;
            bool hasLogAuth = false;
           
            hasLogAuth = this.logauth();

            try
            {
                if (hasLogAuth == false)
                {
                    var url = new UrlHelper(filterContext.RequestContext);
                    var verifyEmailUrl = url.Action("Logout", "Account", new { errorMessage = "登入時間逾期，請重新登入!" });
                    filterContext.Result = new RedirectResult(verifyEmailUrl);
                }
                else
                {
                    // 登入狀態下，檢查是否有功能選單的權限
                    hasPermission = this.Permissauth(filterContext);

                    if (hasPermission == false)
                    {
                        var url = new UrlHelper(filterContext.RequestContext);
                        var verifyEmailUrl = url.Action("Logout", "Account", new { errorMessage = "無權限操作此頁面!" });
                        filterContext.Result = new RedirectResult(verifyEmailUrl);
                    }
                }
            }
            catch (Exception ex)
            {
                var url = new UrlHelper(filterContext.RequestContext);
                var verifyEmailUrl = url.Action("Logout", "Account", new { errorMessage = "系統發生意外錯誤，將登出並導回首頁!" });
                filterContext.Result = new RedirectResult(verifyEmailUrl);

                log.Error(this.GetExceptionMessage(ex));
            }
            
            //base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// 判斷 User 是否有權限
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        private bool Permissauth(ActionExecutingContext filterContext)
        {
            bool hasPermission = false;

            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string actionName = filterContext.ActionDescriptor.ActionName;

            int intUserID = 0;
            int intSellerID = 0;
            int intGroupID = 0;

            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();

            // cookie 內有 三個 才進行權限判斷
            if (System.Web.HttpContext.Current.Request.Cookies["SD"] != null
                && System.Web.HttpContext.Current.Request.Cookies["UD"] != null
                && System.Web.HttpContext.Current.Request.Cookies["GD"] != null)
            {
                // 目前 Controller 內的 action 都會使用同一個 FunctionID
                int functionID = spdb.Seller_Action.Where(x => x.ControllerName == controllerName).Select(y => y.FunctionID).FirstOrDefault();

                TWNewEgg.API.View.Service.AES aesService = new Service.AES();

                // 進行 UserID 解密
                string userID = aesService.AesDecrypt(System.Web.HttpContext.Current.Request.Cookies["UD"].Value);
                string sellerID = aesService.AesDecrypt(System.Web.HttpContext.Current.Request.Cookies["SD"].Value);
                string groupID = aesService.AesDecrypt(System.Web.HttpContext.Current.Request.Cookies["GD"].Value);

                string authEnable = string.Empty;

                // 判斷由 Cookie 內傳入的是否能夠轉成 int
                if (int.TryParse(userID, out intUserID) && int.TryParse(sellerID, out intSellerID) && int.TryParse(groupID, out intGroupID))
                {
                    // 首先判斷此 User 帳號是否啟用中
                    if (spdb.Seller_User.Where(x => x.UserID == intUserID && x.Status == "E").Any())
                    {
                        string purViewType = spdb.Seller_User.Where(x => x.UserID == intUserID).Select(y => y.PurviewType).FirstOrDefault();

                        switch (purViewType)
                        {
                            default:
                            case "G":
                                authEnable = spdb.Group_Purview.Where(x => x.GroupID == intGroupID && x.FunctionID == functionID).Select(y => y.Enable).FirstOrDefault();
                                break;
                            case "S":
                                authEnable = spdb.Seller_Purview.Where(x => x.SellerID == intSellerID && x.FunctionID == functionID).Select(y => y.Enable).FirstOrDefault();
                                break;
                            case "U":
                                authEnable = spdb.User_Purview.Where(x => x.UserID == intUserID && x.FunctionID == functionID).Select(y => y.Enable).FirstOrDefault();
                                break;
                        }

                        // 目前若 S or U 無權限，改以 G 的權限判斷，避免 Vendor 被強制登出
                        if (string.IsNullOrWhiteSpace(authEnable))
                        {
                            authEnable = spdb.Group_Purview.Where(x => x.GroupID == intGroupID && x.FunctionID == functionID).Select(y => y.Enable).FirstOrDefault();
                        }

                        // 判斷使用者權限是否開啟
                        hasPermission = CheckAuth(authEnable);
                    }
                    else
                    {
                        // 非啟用直接跳出無此權限
                        hasPermission = false;
                    }
                }
            }
            return hasPermission;
        }

        /// <summary>
        /// 判斷權限是否有開啟
        /// </summary>
        /// <param name="enable">"Y" or "N"</param>
        /// <returns>true or false</returns>
        private bool CheckAuth(string enable)
        {          
            if (enable == "Y")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool logauth()
        {
            TWNewEgg.API.Models.Connector conn = new TWNewEgg.API.Models.Connector();
            TWNewEgg.API.Models.Cookie _cookie = new TWNewEgg.API.Models.Cookie();

            int sellerID = 0;

            if (System.Web.HttpContext.Current.Request.Cookies["UD"] != null &&
                System.Web.HttpContext.Current.Request.Cookies["SD"] != null &&
                System.Web.HttpContext.Current.Request.Cookies["CSD"] != null &&
                System.Web.HttpContext.Current.Request.Cookies["GD"] != null &&
                System.Web.HttpContext.Current.Request.Cookies["AT"] != null &&
                System.Web.HttpContext.Current.Request.Cookies["ATC"] != null &&
                System.Web.HttpContext.Current.Request.Cookies["UEM"] != null &&
                System.Web.HttpContext.Current.Request.Cookies["VS"] != null)
            { 
                // 加解密
                TWNewEgg.API.View.Service.AES aes = new Service.AES();
                string strUserID = string.Empty;
                string strSellerID = string.Empty;
                string strCurrentSellerID = string.Empty;
                string strGroupID = string.Empty;
                string strAccessToken = string.Empty;
                string strAccountTypeCode = string.Empty;
                string strUserEmail = string.Empty;
                string strVendorSeller = string.Empty;

                strUserID = aes.AesDecrypt(System.Web.HttpContext.Current.Request.Cookies["UD"].Value);
                strSellerID = aes.AesDecrypt(System.Web.HttpContext.Current.Request.Cookies["SD"].Value);
                strCurrentSellerID = aes.AesDecrypt(System.Web.HttpContext.Current.Request.Cookies["CSD"].Value);
                strGroupID = aes.AesDecrypt(System.Web.HttpContext.Current.Request.Cookies["GD"].Value);
                strAccessToken = aes.AesDecrypt(System.Web.HttpContext.Current.Request.Cookies["AT"].Value);
                strAccountTypeCode = aes.AesDecrypt(System.Web.HttpContext.Current.Request.Cookies["ATC"].Value);
                strUserEmail = aes.AesDecrypt(System.Web.HttpContext.Current.Request.Cookies["UEM"].Value);
                strVendorSeller = aes.AesDecrypt(System.Web.HttpContext.Current.Request.Cookies["VS"].Value);

                // 判斷Cookie是為空值
                if (!string.IsNullOrEmpty(strUserID)
                    && !string.IsNullOrEmpty(strSellerID)
                    && !string.IsNullOrEmpty(strCurrentSellerID)
                    && !string.IsNullOrEmpty(strGroupID)
                    && !string.IsNullOrEmpty(strAccessToken)
                    && !string.IsNullOrEmpty(strAccountTypeCode)
                    && !string.IsNullOrEmpty(strUserEmail)
                    && !string.IsNullOrEmpty(strVendorSeller)
                    && strUserID != "0"
                    && strSellerID != "0")
                {
                    _cookie.EmailCookie = System.Web.HttpContext.Current.Request.Cookies["UEM"].Value;
                    _cookie.AccessToken = System.Web.HttpContext.Current.Request.Cookies["AT"].Value;

                    TWNewEgg.API.Models.ActionResponse<int> _responsecookie = conn.GetSellerUserIDCon(null, null, _cookie);

                    sellerID = _responsecookie.Body;
                }
            }

            if (sellerID == 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

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
    }
}