using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.API.View.Controllers
{
    public class SellerAccountController : Controller
    {
        //
        // GET: /SellerAccount/
        TWNewEgg.API.Models.Connector conn = new TWNewEgg.API.Models.Connector();
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        [Filter.LoginAuthorize]
        public ActionResult Index()
        {
            TWNewEgg.API.Models.Connector conn = new API.Models.Connector();
            TWNewEgg.API.Models.Cookie _cookie = new API.Models.Cookie();

            #region call the api and get the content of xml
            string xmlcontent = this.xmlContent();
            ViewBag.xmlContent = (xmlcontent);
            #endregion
            if (Request.Cookies["UEM"] != null)
            {
                _cookie.EmailCookie = Request.Cookies["UEM"].Value;
            }
            else
            {
                _cookie.EmailCookie = string.Empty;
            }
            if (Request.Cookies["AT"] != null)
            {
                _cookie.AccessToken = Request.Cookies["AT"].Value;
            }
            else
            {
                _cookie.AccessToken = string.Empty;
            }
            TWNewEgg.API.Models.ActionResponse<int> _Responsecookie = new API.Models.ActionResponse<int>();
            bool isIndex;
            try
            {
                _Responsecookie = conn.GetSellerUserIDCon(null, null, _cookie);
                isIndex = true;
            }
            catch (Exception error)
            {
                logger.Error("Message = " + error.Message + "; [StackTrace]" + error.StackTrace);
                isIndex = false;
            }
            if (isIndex == true)
            {
                if (_Responsecookie.Body == 0)
                {
                    return RedirectToAction("LandingPage", "Account", new { errorMessage = "登入逾時" });
                }
                return View();
            }
            else
            {
                return RedirectToAction("LandingPage", "Account", new { errorMessage = "系統錯誤" });
            }
        }

        public string xmlContent()
        {
            string resultXml = string.Empty;
            TWNewEgg.API.Models.ActionResponse<string> result = new API.Models.ActionResponse<string>();
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            try
            {
                result = conn.xmlBulletinsRead();
                if (result.IsSuccess == false)
                {
                    resultXml = string.Empty;
                }
                else
                {
                    resultXml = result.Body;
                }
            }
            catch (Exception error)
            {
                logger.Error("/SellerInvitation/BulletinsMessage connect to api error: " + error.Message);
            }
            return resultXml;
        }
    }
}
