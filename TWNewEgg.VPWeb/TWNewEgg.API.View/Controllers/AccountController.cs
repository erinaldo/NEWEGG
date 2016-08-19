using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Models;

namespace TWNewEgg.API.View.Controllers
{
    public class AccountController : Controller
    {
        private log4net.ILog logger;
        /// <summary>
        /// login page
        /// </summary>
        /// <returns></returns>
        public ActionResult LandingPage(string errorMessage)
        {
            //判斷登入必要的COOKIE是否存在
            if (Request.Cookies["UEM"] != null && Request.Cookies["AT"] != null && Request.Cookies["ATC"] != null)
            {
                //存在則進入自動登入
                AutoLogin();
                return RedirectToAction("Index", "SellerAccount");
            }

            ViewBag.message = errorMessage;
            return View();
        }

        /// <summary>
        /// use login action
        /// </summary>
        /// <param name="UserEmail"></param>
        /// <param name="UserPwd"></param>
        /// <param name="IsRemember"></param>
        /// <param name="VendorSeller"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LandingPage(string UserEmail, string UserPwd, string IsRemember, string VendorSeller)
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            Connector conn = new Connector();
            TWNewEgg.API.Models.UserLogin userloginInfo = new UserLogin();
            TWNewEgg.API.Models.Cookie _cookie = new Cookie();
            userloginInfo.UserEmail = UserEmail;
            userloginInfo.Password = UserPwd;
            userloginInfo.VendorSeller = VendorSeller;

            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.UserLoginResult> loginResult = new ActionResponse<UserLoginResult>();
            bool isLogin = true;

            try
            {
                loginResult = conn.Login(null, null, userloginInfo);
            }
            catch (Exception error)
            {
                logger.Error("/Account/landingPage error: " + error.Message);
                isLogin = false;
            }
            if (isLogin == false)
            {
                Response.Write("<script>alert('系統錯誤')</script>");
                ErrorcookieProcess();
                return View();
            }

            switch (loginResult.Code)
            {
                case (int)TWNewEgg.API.Models.UserLoginingResponseCode.Success:
                    {
                        if (IsRemember == "on")
                        {
                            SuccesscookieProcess(loginResult.Body, true);
                        }
                        else
                        {
                            SuccesscookieProcess(loginResult.Body, false);
                        }
                        return RedirectToAction("Index", "SellerAccount");
                        break;
                    }
                case (int)TWNewEgg.API.Models.UserLoginingResponseCode.Error:
                case (int)TWNewEgg.API.Models.UserLoginingResponseCode.PasswordFaild:
                    {
                        Response.Write("<script>alert('登入帳號或密碼錯誤!')</script>");
                        ErrorcookieProcess();
                        break;
                    }

                case (int)TWNewEgg.API.Models.UserLoginingResponseCode.AccountTypeError:
                    {
                        Response.Write("<script>alert('Account Type Error!')</script>");
                        ErrorcookieProcess();

                        break;
                    }
                case (int)TWNewEgg.API.Models.UserLoginingResponseCode.Accountalreadystop:
                    {
                        Response.Write("<script>alert('此商家帳號已經停止使用，請聯絡新蛋客服。')</script>");
                        ErrorcookieProcess();
                        break;
                    }

            }
            return View();
        }

        /// <summary>
        /// Do the logout
        /// </summary>
        /// <param name="message">error message</param>
        /// <returns></returns>
        public ActionResult Logout(string errorMessage)
        {
            #region            
            //UserID
            //if (Request.Cookies["UD"] != null)
            //{
            //    HttpCookie UD = new HttpCookie("UD");
            //    UD.Expires = DateTime.Now.AddDays(-1d);
            //    Response.Cookies.Add(UD);
            //}
            ////SellerID
            //if (Request.Cookies["SD"] != null)
            //{
            //    HttpCookie SD = new HttpCookie("SD");
            //    SD.Expires = DateTime.Now.AddDays(-1d);
            //    Response.Cookies.Add(SD);
            //}
            ////
            //if (Request.Cookies["CSD"] != null)
            //{
            //    HttpCookie CSD = new HttpCookie("CSD");
            //    CSD.Expires = DateTime.Now.AddDays(-1d);
            //    Response.Cookies.Add(CSD);
            //}
            ////AccessToken
            //if (Request.Cookies["AT"] != null)
            //{
            //    HttpCookie AT = new HttpCookie("AT");
            //    AT.Expires = DateTime.Now.AddDays(-1d);
            //    Response.Cookies.Add(AT);
            //}
            ////AccountTypeCode
            //if (Request.Cookies["ATC"] != null)
            //{
            //    HttpCookie ATC = new HttpCookie("ATC");
            //    ATC.Expires = DateTime.Now.AddDays(-1d);
            //    Response.Cookies.Add(ATC);
            //}
            ////UserEmail
            //if (Request.Cookies["UEM"] != null)
            //{
            //    HttpCookie EM = new HttpCookie("UEM");
            //    EM.Expires = DateTime.Now.AddDays(-1d);
            //    Response.Cookies.Add(EM);
            //}
            ////GroupID
            //if (Request.Cookies["GD"] != null)
            //{
            //    HttpCookie GD = new HttpCookie("GD");
            //    GD.Expires = DateTime.Now.AddDays(-1d);
            //    Response.Cookies.Add(GD);
            //}

            //if (Request.Cookies["VS"] != null)
            //{
            //    HttpCookie VS = new HttpCookie("VS");
            //    VS.Expires = DateTime.Now.AddDays(-1d);
            //    Response.Cookies.Add(VS);
            //}
            ////IsRemember
            //if (Request.Cookies["IR"] != null)
            //{
            //    HttpCookie IR = new HttpCookie("IR");
            //    IR.Expires = DateTime.Now.AddDays(-1d);
            //    Response.Cookies.Add(IR);
            //}
            //if (Request.Cookies["RSD"] != null)
            //{
            //    Response.Cookies["RSD"].Expires = DateTime.Now.AddDays(-1);
            //}
            #endregion
            this.ErrorcookieProcess();
            return RedirectToAction("LandingPage", "Account", new { errorMessage = errorMessage });
        }
        /// <summary>
        /// 登入錯誤時清除cookie
        /// </summary>
        public void ErrorcookieProcess()
        {
            //UserID
            if (Request.Cookies["UD"] != null)
            {
                Response.Cookies["UD"].Expires = DateTime.Now.AddDays(-1);
            }
            //SellerID
            if (Request.Cookies["SD"] != null)
            {
                Response.Cookies["SD"].Expires = DateTime.Now.AddDays(-1);
            }
            //Currently SellerID
            if (Request.Cookies["CSD"] != null)
            {
                Response.Cookies["CSD"].Expires = DateTime.Now.AddDays(-1);
            }
            //AccessToken
            if (Request.Cookies["AT"] != null)
            {
                Response.Cookies["AT"].Expires = DateTime.Now.AddDays(-1);
            }
            //AccountTypeCode
            if (Request.Cookies["ATC"] != null)
            {
                Response.Cookies["ATC"].Expires = DateTime.Now.AddDays(-1);
            }
            //UserEmail
            if (Request.Cookies["UEM"] != null)
            {
                Response.Cookies["UEM"].Expires = DateTime.Now.AddDays(-1);
            }
            //GroupID
            if (Request.Cookies["GD"] != null)
            {
                Response.Cookies["GD"].Expires = DateTime.Now.AddDays(-1);
            }
            //VendorSeller
            if (Request.Cookies["VS"] != null)
            {
                Response.Cookies["VS"].Expires = DateTime.Now.AddDays(-1);
            }
            //IsRemember
            if (Request.Cookies["IR"] != null)
            {
                Response.Cookies["IR"].Expires = DateTime.Now.AddDays(-1);
            }
            // Remeber seller id
            if (Request.Cookies["RSD"] != null)
            {
                Response.Cookies["RSD"].Expires = DateTime.Now.AddDays(-1);
            }
            // Remember Seller ID
            if (Request.Cookies["LATC"] != null)
            {
                Response.Cookies["LATC"].Expires = DateTime.Now.AddDays(-1);
        }
            // Local AccountType
            if (Request.Cookies["LSS"] != null)
            {
                Response.Cookies["LSS"].Expires = DateTime.Now.AddDays(-1);
            }
            // Local Seller Status
            if (Request.Cookies["SS"] != null)
            {
                Response.Cookies["SS"].Expires = DateTime.Now.AddDays(-1);
            }
        }

        /// <summary>
        /// 登入成功時把相關資料寫入cookie
        /// </summary>
        /// <param name="loginResult"></param>
        /// <param name="isRemberME"></param>
        public void SuccesscookieProcess(TWNewEgg.API.Models.UserLoginResult loginResult, bool isRemberME)
        {
            Connector connector = new Connector();
            TWNewEgg.API.View.Service.AES aes = new Service.AES();
            var _sellerBasicInfo  = connector.GetSeller_BasicInfo(aes.AesDecrypt(loginResult.SellerID), 0);
            //有勾選記住我，COOKIE時間為3個月
            if (isRemberME == true)
            {
                Response.Cookies["UD"].Value = loginResult.UserID;
                Response.Cookies["SD"].Value = loginResult.SellerID;
                Response.Cookies["CSD"].Value = loginResult.SellerID;
                Response.Cookies["AT"].Value = loginResult.AccessToken;
                Response.Cookies["ATC"].Value = loginResult.AccountTypeCode;
                Response.Cookies["UEM"].Value = loginResult.UserEmail;
                Response.Cookies["GD"].Value = loginResult.GroupID;
                Response.Cookies["VS"].Value = loginResult.AccountTypeCode;
                Response.Cookies["IR"].Value = "T";
                Response.Cookies["LATC"].Value = loginResult.AccountTypeCode;
                Response.Cookies["LSS"].Value = aes.AesEncrypt(_sellerBasicInfo.Body.SellerStatus);
                Response.Cookies["SS"].Value = aes.AesEncrypt(_sellerBasicInfo.Body.SellerStatus);

                Response.Cookies["UD"].Expires = DateTime.Now.AddMonths(3);
                Response.Cookies["SD"].Expires = DateTime.Now.AddMonths(3);
                Response.Cookies["CSD"].Expires = DateTime.Now.AddMonths(3);
                Response.Cookies["AT"].Expires = DateTime.Now.AddMonths(3);
                Response.Cookies["ATC"].Expires = DateTime.Now.AddMonths(3);
                Response.Cookies["UEM"].Expires = DateTime.Now.AddMonths(3);
                Response.Cookies["GD"].Expires = DateTime.Now.AddMonths(3);
                Response.Cookies["VS"].Expires = DateTime.Now.AddMonths(3);
                Response.Cookies["IR"].Expires = DateTime.Now.AddMonths(3);
                Response.Cookies["LATC"].Expires = DateTime.Now.AddMonths(3);
                Response.Cookies["LSS"].Expires = DateTime.Now.AddMonths(3);
                Response.Cookies["SS"].Expires = DateTime.Now.AddMonths(3);

            }
            else
            {
                //沒有勾選記住我，COOKIE時間為5天
                Response.Cookies["UD"].Value = loginResult.UserID;
                Response.Cookies["SD"].Value = loginResult.SellerID;
                Response.Cookies["CSD"].Value = loginResult.SellerID;
                Response.Cookies["AT"].Value = loginResult.AccessToken;
                Response.Cookies["ATC"].Value = loginResult.AccountTypeCode;
                Response.Cookies["UEM"].Value = loginResult.UserEmail;
                Response.Cookies["GD"].Value = loginResult.GroupID;
                Response.Cookies["VS"].Value = loginResult.AccountTypeCode;
                Response.Cookies["IR"].Value = "F";
                Response.Cookies["LATC"].Value = loginResult.AccountTypeCode;
                Response.Cookies["LSS"].Value = aes.AesEncrypt(_sellerBasicInfo.Body.SellerStatus);
                Response.Cookies["SS"].Value = aes.AesEncrypt(_sellerBasicInfo.Body.SellerStatus);

                Response.Cookies["UD"].Expires = DateTime.Now.AddDays(5);
                Response.Cookies["SD"].Expires = DateTime.Now.AddDays(5);
                Response.Cookies["CSD"].Expires = DateTime.Now.AddDays(5);
                Response.Cookies["AT"].Expires = DateTime.Now.AddDays(5);
                Response.Cookies["ATC"].Expires = DateTime.Now.AddDays(5);
                Response.Cookies["UEM"].Expires = DateTime.Now.AddDays(5);
                Response.Cookies["GD"].Expires = DateTime.Now.AddDays(5);
                Response.Cookies["VS"].Expires = DateTime.Now.AddDays(5);
                Response.Cookies["IR"].Expires = DateTime.Now.AddDays(5);
                Response.Cookies["LATC"].Expires = DateTime.Now.AddDays(5);
                Response.Cookies["LSS"].Expires = DateTime.Now.AddDays(5);
                Response.Cookies["SS"].Expires = DateTime.Now.AddDays(5);
            }
        }

        /// <summary>
        /// 自動登入
        /// </summary>
        /// <returns></returns>
        public ActionResult AutoLogin()
        {
            Connector conn = new Connector();
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            TWNewEgg.API.Models.UserLogin userloginInfo = new UserLogin();
            TWNewEgg.API.Models.Cookie _cookie = new Cookie();
            TWNewEgg.API.View.Service.AES aes = new Service.AES();

            #region 檢查cookie預防錯誤
            userloginInfo.UserEmail = string.Empty;
            userloginInfo.Password = string.Empty;
            userloginInfo.VendorSeller = string.Empty;
            #endregion
            // userEmail
            if (Request.Cookies["UEM"] != null)
            {
                userloginInfo.UserEmail = aes.AesDecrypt(Request.Cookies["UEM"].Value);
            }
            //AccessToken
            if (Request.Cookies["AT"] != null)
            {
                userloginInfo.Password = Request.Cookies["AT"].Value;
            }
            //AccountTypeCode
            if (Request.Cookies["ATC"] != null)
            {
                userloginInfo.VendorSeller = aes.AesDecrypt(Request.Cookies["ATC"].Value);
            }

            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.UserLoginResult> loginResult = new ActionResponse<UserLoginResult>();
            bool isAutoLogin = true;
            try
            {
                //呼叫登入的API進行資料查詢，此帳號是否存在DB裡面
                loginResult = conn.AutoLogin(null, null, userloginInfo);
            }
            catch (Exception error)
            {
                logger.Error("/Account/AutoLogin error: " + error.Message);
                isAutoLogin = false;
            }
            if (isAutoLogin == false)
            {
                ErrorcookieProcess();
                return RedirectToAction("LandingPage", "Account", new { errorMessage = "系統錯誤" });
            }


            switch (loginResult.Code)
            {
                case (int)TWNewEgg.API.Models.UserLoginingResponseCode.Success:
                    {
                        return RedirectToAction("Index", "SellerAccount");
                        break;
                    }
                case (int)TWNewEgg.API.Models.UserLoginingResponseCode.Error:
                    {
                        #region 登入錯誤 所以清除所有cookie
                        Response.Write("<script>alert('登入帳號或密碼錯誤!')</script>");
                        ErrorcookieProcess();
                        #endregion
                        break;
                    }
                case (int)TWNewEgg.API.Models.UserLoginingResponseCode.PasswordFaild:
                    {
                        #region 登入錯誤 所以清除所有cookie
                        Response.Write("<script>alert('登入帳號或密碼錯誤!')</script>");
                        ErrorcookieProcess();
                        #endregion
                        break;
                    }

                case (int)TWNewEgg.API.Models.UserLoginingResponseCode.AccountTypeError:
                    {
                        #region AccountTypeError錯誤 所以清除所有cookie
                        Response.Write("<script>alert('Account Type Error!')</script>");
                        ErrorcookieProcess();
                        #endregion
                        break;
                    }
                case (int)TWNewEgg.API.Models.UserLoginingResponseCode.Accountalreadystop:
                    {
                        #region Accountalreadystop錯誤 所以清除所有cookie
                        Response.Write("<script>alert('此商家帳號已經停止使用，請聯絡新蛋客服。')</script>");
                        ErrorcookieProcess();
                        #endregion
                        break;
                    }
                default:
                    {
                        ErrorcookieProcess();
                        return RedirectToAction("LandingPage", "Account");
                        break;
                    }
            }

            return RedirectToAction("LandingPage", "Account");
        }

        /// <summary>
        /// 廠商被邀請後設置密碼
        /// </summary>
        /// <param name="UserEmail">廠商Email</param>
        /// <param name="RanCo">RanCode</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SetPassword(string UserEmail, string RanCo)
        {
            Connector Connector = new Connector();
            UserCheckStatus Checker = new UserCheckStatus();
            ActionResponse<TWNewEgg.API.Models.UserCheckStatusResult> CheckResult = new ActionResponse<UserCheckStatusResult>();

            Checker.UserEmail = UserEmail;
            Checker.RanCode = RanCo;

            if ((UserEmail == "" || UserEmail == null) && (RanCo == "" || RanCo == null))
            {
                return RedirectToAction("LandingPage", "Account");
            }

            CheckResult = Connector.CheckStatus("", "", Checker);

            if (CheckResult.IsSuccess == false)
            {
                return RedirectToAction("LandingPage", "Account");
            }

            ViewBag.RanCode = RanCo;
            ViewBag.UserEmail = UserEmail;
            ViewBag.UserId = CheckResult.Body.UserID;

            return View();
        }

        [HttpPost]
        public ActionResult SetPassword(string UserPwd, string ConfirmUserPwd, string UserEmail, string RanCode, int UserId)
        {
            // 為了防止自動登入原已登入帳號，因此先將原登入帳號登出(清除 cookie)
            Logout(string.Empty);

            UserChangePassword UserChangePwd = new UserChangePassword();
            UserChangePwd.UserEmail = UserEmail;
            UserChangePwd.RanCode = RanCode;
            UserChangePwd.NewPassword = UserPwd;
            UserChangePwd.UpdateUserID = UserId;
            string sJavaScript = "";

            Connector Connector = new Connector();
            try
            {
                ActionResponse<UserLoginResult> Result = Connector.SetPassword("", "", UserChangePwd);

                if (Result.IsSuccess == true)
                {
                    TWNewEgg.API.Models.UserLogin userloginInfo = new UserLogin();
                    //做自動登入時，所需要的資料
                    userloginInfo.UserEmail = Result.Body.UserEmail;
                    userloginInfo.Password = Result.Body.AccessToken;
                    userloginInfo.VendorSeller = Result.Body.AccountTypeCode;
                    //連接API做自動登入的動作
                    TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.UserLoginResult> loginResult = Connector.AutoLogin(null, null, userloginInfo);
                    //自動登入成功
                    if (loginResult.IsSuccess == true)
                    {
                        //因為是第一次登入，所以必須寫入cookie，否則會被登出，再進行一次登入
                        SuccesscookieProcess(loginResult.Body, true);
                    }
                    else
                    {
                        //登入失敗，直接洗掉所有cookie
                        ErrorcookieProcess();
                    }
                    return RedirectToAction("LandingPage", "Account");
                }
                else
                {
                    Response.Write(Result.Msg);
                    sJavaScript = "<script language=javascript>\n";
                    sJavaScript += "alert('Error!');\n";
                    sJavaScript += "window.location = \"LandingPage\";\n";
                    sJavaScript += "</script>";
                    Response.Write(sJavaScript);
                }
            }
            catch (Exception e)
            {
                logger.Error("/Account/SetPassword error: " + e.Message);
                sJavaScript = e.Message;
            }
            return View();
        }

        /// <summary>
        /// 廠商忘記密碼，重設
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(string UserEmail)
        {
            Connector Connector = new Connector();
            ResetPassword ResetPassword = new ResetPassword();
            string sJavaScript = "";

            ResetPassword.UserEmail = UserEmail;

            ActionResponse<ResetPasswordResult> Result = Connector.ResetPassword("", "", ResetPassword);

            if (Result.IsSuccess == true)
            {
                sJavaScript = "<script language=javascript>\n";
                sJavaScript += "alert('If the email address you provided is associated with a Newegg Seller Portal ID, you will be emailed within 24 hours with a link that will allow you to reset your password.\\n\\n Redirect to login page?');\n";
                sJavaScript += "window.location = \"LandingPage\";\n";
                sJavaScript += "</script>";
                Response.Write(sJavaScript);
            }
            else
            {
                sJavaScript = "<script language=javascript>\n";
                sJavaScript += "alert('" + Result.Msg + "');\n";
                sJavaScript += "window.location = \"LandingPage\";\n";
                sJavaScript += "</script>";
                Response.Write(sJavaScript);
            }

            return View();
        }

        [HttpGet]
        public ActionResult ChangePassWord()
        {
            return View();
        }

        TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();

        [HttpPost]
        public ActionResult ChangePassWord(string UserPwd, string ConfirmUserPwd)
        {
            string sJavaScript = "";
            TWNewEgg.API.Models.Connector conn = new Connector();
            TWNewEgg.API.Models.UserChangePassword userchangePassword = new UserChangePassword();
            userchangePassword.UserEmail = sellerInfo.UserEmail;
            userchangePassword.UpdateUserID = sellerInfo.UserID;
            userchangePassword.NewPassword = ConfirmUserPwd;
            userchangePassword.OldPassword = UserPwd;
            try
            {
                ActionResponse<UserLoginResult> result = conn.ChangeOldPassword("", "", userchangePassword);

                if (result.IsSuccess == true)
                {
                    // 改完密碼後自動登入
                    return RedirectToAction("Logout", "Account");
                }
                else
                {
                    sJavaScript = "<script language=javascript>\n";
                    sJavaScript += "alert('密碼更換錯誤，請聯絡新蛋客服');\n";
                    sJavaScript += "window.location = \"Logout\";\n";
                    sJavaScript += "</script>";
                    Response.Write(sJavaScript);
                }
            }
            catch (Exception ex)
            {
                sJavaScript = "<script language=javascript>\n";
                sJavaScript += "alert('密碼更換錯誤，請聯絡新蛋客服');\n";
                sJavaScript += "window.location = \"Logout\";\n";
                sJavaScript += "</script>";
                Response.Write(sJavaScript);
                // 改完密碼後自動登入
                return RedirectToAction("Logout", "Account");
            }

            return null;
        }

        [HttpGet]
        public ActionResult ErrorHandle()
        {
            string sJavaScript = string.Empty;
            sJavaScript = "<script language=javascript>\n";
            sJavaScript += "alert('系統發生錯誤，將導回首頁');\n";
            sJavaScript += "window.location = \"Logout\";\n";
            sJavaScript += "</script>";
            Response.Write(sJavaScript);
            return null;
        }
    }
}
