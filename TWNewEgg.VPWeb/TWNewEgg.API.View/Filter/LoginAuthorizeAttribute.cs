using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.API.View.Filter
{
    /// <summary>
    /// 判斷 User 登入內容是否存在 Filter
    /// </summary>
    public class LoginAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        /// <summary>
        /// 判斷 User 登入內容是否存在
        /// </summary>
        /// <param name="filterContext">HttpContext</param>
        public override void OnAuthorization(AuthorizationContext filterContext)
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
                if (!filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    var url = new UrlHelper(filterContext.RequestContext);
                    var verifyEmailUrl = url.Action("Logout", "Account", new { errorMessage = "登入時間逾期，請重新登入!" });
                    filterContext.Result = new RedirectResult(verifyEmailUrl);
                }
                else
                {
                    JsonResult jr = new JsonResult();
                    jr.Data = new { IsSuccess = false, errorMessage = "登入時間逾期，請重新登入!" };
                    jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    filterContext.Result = jr;
                }
            }
        }

        
    }
}
