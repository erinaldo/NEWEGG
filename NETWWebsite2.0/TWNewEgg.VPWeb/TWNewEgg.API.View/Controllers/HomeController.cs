using KendoGridBinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Models;
using log4net;

namespace TWNewEgg.API.View.Controllers
{
    public class HomeController : Controller
    {
        log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 顯示功能選單
        /// </summary>
        /// <returns>顯示該使用者可用之功能選單</returns>
        //[Filter.LoginAuthorize]
        //public JsonResult DisplayMenuList()
        //{
        //    // 加解密
        //    TWNewEgg.API.View.Service.AES aes = new Service.AES();
        //    string userID = string.Empty;
        //    string sellerID = string.Empty;
        //    string groupID = string.Empty;
        //    string userEmail = string.Empty;
        //    string accountTypeCode = string.Empty;

        //    userID = aes.AesDecrypt(Request.Cookies["UD"].Value);
        //    sellerID = aes.AesDecrypt(Request.Cookies["SD"].Value);
        //    groupID = aes.AesDecrypt(Request.Cookies["GD"].Value);
        //    userEmail = aes.AesDecrypt(Request.Cookies["UEM"].Value);
        //    accountTypeCode = aes.AesDecrypt(Request.Cookies["VS"].Value);

        //    Connector conn = new Connector();
        //    TWNewEgg.API.Models.ActionResponse<List<MenuList>> menuListResult = new ActionResponse<List<MenuList>>();
        //    // 取得功能選單清單
        //    menuListResult = conn.GetMenuListAPI(userID, sellerID, groupID, userEmail, accountTypeCode);
        //    if (menuListResult.IsSuccess)
        //    {
        //        ViewBag.MenuList = menuListResult.Body;
        //        string strMenuList = RenderView("MenuList");

        //        int checkGroupID = 0;
        //        int.TryParse(groupID, out checkGroupID);
        //        string strManagePurview = string.Empty;
        //        //if (checkGroupID == 3 || checkGroupID == 5)
        //        //{
        //        //    strManagePurview = RenderView("ManagePurview");
        //        //}

        //        return Json(new { IsSuccess = menuListResult.IsSuccess, showMenuList = strMenuList, showManagePurview = strManagePurview },JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new { IsSuccess = menuListResult.IsSuccess, errorMessage = "登入時間逾期，請重新登入!" },JsonRequestBehavior.AllowGet);
        //    }
        //}

        //[Filter.LoginAuthorize]
        public ActionResult DisplayMenuList()
        {
            // 加解密
            TWNewEgg.API.View.Service.AES aes = new Service.AES();
            string userID = string.Empty;
            string sellerID = string.Empty;
            string groupID = string.Empty;
            string userEmail = string.Empty;
            string accountTypeCode = string.Empty;

            userID = aes.AesDecrypt(Request.Cookies["UD"].Value);
            sellerID = aes.AesDecrypt(Request.Cookies["SD"].Value);
            groupID = aes.AesDecrypt(Request.Cookies["GD"].Value);
            userEmail = aes.AesDecrypt(Request.Cookies["UEM"].Value);
            accountTypeCode = aes.AesDecrypt(Request.Cookies["VS"].Value);

            Connector conn = new Connector();
            TWNewEgg.API.Models.ActionResponse<List<MenuList>> menuListResult = new ActionResponse<List<MenuList>>();
            
            if (userID == "0" || sellerID == "0" || groupID == "0")
            {
                return PartialView("Homelogout");
            }

            // 取得功能選單清單
            menuListResult = conn.GetMenuListAPI(userID, sellerID, groupID, userEmail, accountTypeCode);
            if (menuListResult.IsSuccess)
            {
                ViewBag.MenuList = menuListResult.Body;
                return PartialView("MenuList");
            }
            else
            {
                return RedirectToAction("logout", "account", new { errorMessage = "登入時間逾期，請重新登入!" });
            }
        }

        [HttpGet]
        public ActionResult UserResponseView()
        {
            TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();

            ViewBag.UserEmail = sellerInfo.UserEmail;

            ViewBag.LoginUserID = sellerInfo.UserID;

            return PartialView("UserResponseView");
        }

        [HttpPost]
        public JsonResult SendResponse(string Message)
        {
            TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();

            TWNewEgg.API.Models.Connector conn = new Connector();
            TWNewEgg.API.Models.Mail userMail = new Mail();

            try
            {
                userMail.UserName = sellerInfo.UserEmail + "(" + sellerInfo.UserID + ")";
                userMail.UserEmail = "";
                userMail.MailMessage = Message;
                userMail.MailType = Mail.MailTypeEnum.UserResponse;

                conn.SendMail(null, null, userMail);
            }
            catch (Exception ex)
            {
                log.Error("Message: " + ex.Message + ", Stack Trace: " + ex.StackTrace);
                return Json(new { msg = "發生意外錯誤，請稍後再試!" });
            }
            return Json(new { msg = "成功寄出，謝謝你的回饋!" });
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
    }
}
