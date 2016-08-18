using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text.RegularExpressions;
using TWNewEgg.Models.ViewModels.Login;
using TWNewEgg.ECWeb.Services.Account;
using TWNewEgg.Framework.ServiceApi.Configuration;
using TWNewEgg.ECWeb.PrivilegeFilters.Core;
using TWNewEgg.ECWeb.Auth;
using TWNewEgg.Models.ViewModels.Register;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DomainModels.Answer;
using TWNewEgg.Models.ViewModels.Answer;
using TWNewEgg.Models.DomainModels.Account;
using TWNewEgg.Models.ViewModels.Account;
using TWNewEgg.Framework.ServiceApi.Models;
using TWNewEgg.Models.ViewModels.Message;
using TWNewEgg.ECWeb.PrivilegeFilters;
using TWNewEgg.Models.ViewModels.Track;
using TWNewEgg.Framework.Autofac;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DomainModels.Account;
using TWNewEgg.Models.DomainModels.Answer;
using TWNewEgg.AccountEnprypt.Interface;
using TWNewEgg.Redeem.Service.CouponService;

namespace TWNewEgg.ECWeb.Controllers
{
    public class MyAccountController : Controller
    {
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        private IAesService _aesCode;
        private AccountService _accountService;

        public MyAccountController()
        {
            _aesCode = AutofacConfig.Container.Resolve<IAesService>();
        }
        //
        // GET: /MyAccount/
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Signup()
        {
            return View();
        }
        /// <summary>
        /// 註冊
        /// </summary>
        /// <param name="saveAccountVM"></param>
        /// <param name="activity"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Signup(RegisterVM saveAccountVM, string activity)
        {
            ECWebResponse response = new ECWebResponse();
            AccountInfoVM accountInfo = new AccountInfoVM();
            ResponsePacket<AccountInfoVM> registerResult;
            this._accountService = new AccountService(AccountAuthFactory.AuthType.ecweb.ToString());
            //RegisterVM registerVM = new RegisterVM();
            RegistrationError errorMsg = this._accountService.CheckRegister(saveAccountVM);
            if (!errorMsg.error)
            {
                TWNewEgg.Models.DomainModels.Account.AccountDM accountDM = new TWNewEgg.Models.DomainModels.Account.AccountDM();
                AutoMapper.Mapper.Map(saveAccountVM, accountDM);
                accountDM.ACTName = activity;
                registerResult = Processor.Request<AccountInfoVM, AccountInfoDM>("AccountService", "Register", accountDM);
                if (string.IsNullOrWhiteSpace(registerResult.error))
                {
                    response.Data = registerResult.results;
                    response.Status = (int)ECWebResponse.StatusCode.成功;
                    Login model = new TWNewEgg.Models.ViewModels.Login.Login();
                    model.user = saveAccountVM.Email;
                    model.pass = _aesCode.Decrypt(saveAccountVM.PWDtxt);
                  
                    var exist = _accountService.CheckLogin(model);
                    var loginStatus = (exist != null) ? true : false;
                    if (loginStatus)
                    {
                        // 註冊活動判斷
                        if (activity != null)
                        {
                            SendActivityGift(activity, registerResult.results.AVM);
                        }

                        return Json("0");
                    }
                    else
                    {
                        response.Error = new ErrorMessage() { Code = 1, Detail = "系統出現異常，請聯絡管理員" };
                        response.Status = (int)ECWebResponse.StatusCode.系統錯誤;
                        return Json("1");
                    }
                }
                else
                {
                    response.Error = new ErrorMessage() { Code = 1, Detail = "系統出現異常，請聯絡管理員" };
                    response.Status = (int)ECWebResponse.StatusCode.系統錯誤;
                    return Json("1");
                }
            }
            else
            {
                return Json(errorMsg.errormessage);
            }
        }

        /// <summary>
        /// 活動判斷
        /// </summary>
        /// <param name="Activity"></param>
        /// <returns></returns>
        public bool SendActivityGift(string Activity, AccountVM saveAccountVM)
        {
            DateTime datetimeNow = DateTime.UtcNow.AddHours(8);
            var InternalSendMail = new TWNewEgg.InternalSendMail.Service.GeneratorView();
            //TWNewEgg.Redeem.Service.CouponService.CouponServiceRepository objCouponService = new Redeem.Service.CouponService.CouponServiceRepository();
            switch (Activity)
            {
                case "trueyoga_0801":
                    if (datetimeNow > Convert.ToDateTime("2015/07/31") && datetimeNow < Convert.ToDateTime("2015/09/01"))
                    {
                        InternalSendMail.ActivityViewPage(saveAccountVM, "ActivityTrueYoga20150708");
                    }
                    break;
                case "sharegift_0807":
                    if (datetimeNow > Convert.ToDateTime("2015/07/31") && datetimeNow < Convert.ToDateTime("2015/09/01"))
                    {
                        //objCouponService.addDynamicCouponByEventIdAndUserAccount(241, saveAccountVM.ID.ToString());
                        Processor.Request<CouponServiceRepository.AddCouponStatusOption, CouponServiceRepository.AddCouponStatusOption>("Service.CouponService.CouponServiceRepository", "addDynamicCouponByEventIdAndUserAccount", 241, saveAccountVM.ID.ToString());
                        InternalSendMail.ActivityViewPage(saveAccountVM, "ActivitySignup");
                    }
                    break;
                default:
                    List<int> eventIDs;
                    DateTime startDate;
                    DateTime endDate;
                    eventIDs = new List<int>() { 338, 343 };
#if DEBUG
                    eventIDs = new List<int>() { 313, 314 };
#endif
                    startDate = new DateTime(2016, 04, 01, 00, 00, 00);
                    endDate = new DateTime(2016, 05, 01, 00, 00, 00);
#if DEBUG
                    startDate = new DateTime(2016, 03, 30, 10, 00, 00);
                    endDate = new DateTime(2016, 05, 01, 00, 00, 00);
#endif
                    if (DateTime.Compare(datetimeNow, startDate) >= 0 && DateTime.Compare(datetimeNow, endDate) < 0)
                    {
                        foreach (var eventID in eventIDs)
                        {
                            var results = Processor.Request<CouponServiceRepository.AddCouponStatusOption, CouponServiceRepository.AddCouponStatusOption>("Service.CouponService.CouponServiceRepository", "addDynamicCouponByEventIdAndUserAccount", eventID, saveAccountVM.ID.ToString());
                        }
                    }
                    break;
            }
            return true;
        }
        
        [HttpPost]
        public JsonResult UpdatePasswordPost(RegisterVM saveAccountVM)
        {
            ECWebResponse response = new ECWebResponse();
            response.Message = "系統錯誤!";
            response.Status = (int)ECWebResponse.StatusCode.系統錯誤;

            var IsLogin = Processor.Request<AccountInfoVM, AccountInfoDM>("AccountService", "Login", saveAccountVM.Email, _aesCode.Enprypt(saveAccountVM.OldPWD));

            if (!string.IsNullOrEmpty(IsLogin.error))
            {
                response.Message = "帳號密碼錯誤!";
                response.Status = (int)ECWebResponse.StatusCode.系統錯誤;
            }
            else if (IsLogin.results == null || IsLogin.results.AVM == null)
            {
                response.Message = "帳號密碼錯誤!";
                response.Status = (int)ECWebResponse.StatusCode.系統錯誤;
            }
            else
            {
                TWNewEgg.ECWeb.Services.Account.ECAccountAuth ECAccountAuth = new TWNewEgg.ECWeb.Services.Account.ECAccountAuth();
                var ECAccountAuthresponse = ECAccountAuth.CheckPassword(saveAccountVM);

                if (ECAccountAuthresponse.error == false)
                {
                    var UpdatePassword = Processor.Request<AccountInfoVM, AccountInfoDM>("AccountService", "UpdatePassword", saveAccountVM.Email, saveAccountVM.PWD, "UpdatePasswordPost");
                    if (!string.IsNullOrEmpty(UpdatePassword.error))
                    {
                        response.Message = "帳號密碼錯誤!";
                        response.Status = (int)ECWebResponse.StatusCode.系統錯誤;
                    }
                    else if (IsLogin.results == null || IsLogin.results.AVM == null)
                    {
                        response.Message = "帳號密碼錯誤!";
                        response.Status = (int)ECWebResponse.StatusCode.系統錯誤;
                    }
                    else
                    {
                        response.Message = "密碼更新成功，下次登入請使用新密碼！";
                        response.Status = (int)ECWebResponse.StatusCode.成功;
                    }
                }
                else
                {
                    response.Message = "帳號密碼錯誤!";
                    response.Status = (int)ECWebResponse.StatusCode.系統錯誤;
                }
            }

            return Json(response, JsonRequestBehavior.AllowGet);          
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (NEUser.IsAuthticated)
            {
                return RedirectToAction("Index", "Home");
            }
            _accountService = new AccountService(AccountAuthFactory.AuthType.ecweb.ToString());
            Login model = new Login();
            model.ratm = _accountService.GenerateTryTime(string.Format("{0}~{1}~{2}",
                "0",
                ConfigurationManager.GetTaiwanTime(),
                NetPacketUtilities.GetUserIPAddress()));
            model.rytm = false;
            model.type = AccountAuthFactory.AuthType.ecweb.ToString();
            model.acty = _accountService.GetActivityNow();
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(Login model, string returnUrl)
        {
            _accountService = new AccountService(AccountAuthFactory.AuthType.ecweb.ToString());
            bool loginStatus = new bool(), checkStatus = true;

            if (string.IsNullOrEmpty(model.user))
            {
                checkStatus = false;
                model.Iderro = "帳號格式錯誤，請重新輸入";
            }

            if (string.IsNullOrEmpty(model.pass))
            {
                checkStatus = false;
                model.Pwderro = "密碼錯誤，請重新輸入";
            }
            
            string[] decryContent = _accountService.DecryptRandString(model.ratm);

            if (decryContent.Length < 3 && decryContent[3] != NetPacketUtilities.GetUserIPAddress())
            {
                checkStatus = false;
                model.erro = "驗證碼失敗，請再試一次";
            }
            if (int.Parse(decryContent[0]) > 3)
            {
                if (!_accountService.CheckGoogleReCaptcha(model.gcap))
                {
                    checkStatus = false;
                    model.erro = "驗證碼失敗，請再試一次";
                }
            }

            if (checkStatus)
            {
                var exist = _accountService.CheckLogin(model);
                loginStatus = (exist != null) ? true : false;
            }
            if (ModelState.IsValid && loginStatus)
            {
                //Add cookies item into shopping cart.
                List<CartTrack> allCartCookies = TWNewEgg.Framework.Common.JSONSerialization.Deserializer<List<CartTrack>>(CookiesUtilities.CookiesUtility.ReadCookies("sc") ?? "[]");
                var ttt = NEUser.ID;
                if (allCartCookies != null || allCartCookies.Count > 0)
                {
                    Api.TrackController addTrack = new Api.TrackController();
                    List<string> results = addTrack.Post(allCartCookies);
                }
                CookiesUtilities.CookiesUtility.CreateCookie("ipwtf", "true", null, null, null);
                Processor.Request<bool, bool>("TrackService", "CleanOldAndUpdateTracks", NEUser.ID, ConfigurationManager.GetTaiwanTime().AddDays(-30));
                return RedirectToLocal(returnUrl);
            }
            else {
                checkStatus = false;
                model.Pwderro = "密碼錯誤，請重新輸入";
            }



            //return failed message...
            model.gcap = string.Empty;
            model.pass = string.Empty;
            model.rytm = _accountService.IsTryManyTime(decryContent);
            model.ratm = _accountService.GenerateTryTime(string.Format("{0}~{1}~{2}",
                (int.Parse(decryContent[0]) + 1).ToString(),
                ConfigurationManager.GetTaiwanTime(),
                NetPacketUtilities.GetUserIPAddress()));
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        public ActionResult Facebook(Login model, string returnUrl)
        {
            _accountService = new AccountService(AccountAuthFactory.AuthType.facebook.ToString());
            var exist = _accountService.CheckLogin(model);
            bool loginStatus = (exist != null) ? true : false;
            string[] decryContent = _accountService.DecryptRandString(model.ratm);
            if (decryContent.Length < 3 && decryContent[3] != NetPacketUtilities.GetUserIPAddress())
            {
                loginStatus = false;
            }

            if (ModelState.IsValid && loginStatus)
            {
                //TODO(bw52): Add cookies item into shopping cart.

                return RedirectToLocal(returnUrl);
            }



            //return failed message...
            model.gcap = string.Empty;
            model.pass = string.Empty;
            model.erro = "使用者名稱或密碼錯誤。";
            model.rytm = _accountService.IsTryManyTime(decryContent);
            model.ratm = _accountService.GenerateTryTime(string.Format("{0}~{1}~{2}",
                (int.Parse(decryContent[0]) + 1).ToString(),
                ConfigurationManager.GetTaiwanTime(),
                NetPacketUtilities.GetUserIPAddress()));
            ViewBag.ReturnUrl = returnUrl;
            return View("Index");
        }

        public ActionResult GetValidateCode()
        {
            ValidateCode vCode = new ValidateCode();
            string code = vCode.CreateValidateCode(6); //設定驗證碼出現多少個字數
            Response.Cookies["ValidateCode"].Value = code;
            ViewBag.ValidateCode = code;
            byte[] bytes = vCode.CreateValidateGraphic(code);
            return File(bytes, @"image/jpeg");
        }

        // 會員註冊頁面
        [AllowNonSecures]
        [OutputCache(Duration = 0)]
        public ActionResult GuestRegister()
        {
            ClearCookie(Request, Response);
            return View();
        }

        public void ClearCookie(HttpRequestBase Request, HttpResponseBase Response)
        {
            // 清除表單驗證的 cookies
            //FormsAuthentication.SignOut();
            /*
            if (Request.Cookies["newEgg_Login"] != null)
            {
                HttpCookie rCookie = new HttpCookie("newEgg_Login");
                rCookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(rCookie);
            }*/
            string mainDomain = "";
            string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
            if (environment == "DEV")
            {
                mainDomain = System.Configuration.ConfigurationManager.AppSettings["DEV_MainDomain"];
            }
            else
            {
                mainDomain = System.Configuration.ConfigurationManager.AppSettings["ECWeb_MainDomain"];
            }
            if (Request.Cookies["Accountid"] != null)
            {
                HttpCookie accountidcookie = new HttpCookie("Accountid");
                accountidcookie.Domain = mainDomain;
                accountidcookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(accountidcookie);
            }
            if (Request.Cookies["LoginStatus"] != null)
            {
                HttpCookie loginstatus = new HttpCookie("LoginStatus");
                loginstatus.Domain = mainDomain;
                loginstatus.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(loginstatus);
            }
            if (Request.Cookies["IE"] != null)
            {
                HttpCookie IE = new HttpCookie("IE");
                IE.Domain = mainDomain;
                IE.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(IE);
            }
            if (Request.Cookies["em"] != null)
            {
                HttpCookie emailcookie = new HttpCookie("em");
                emailcookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(emailcookie);
            }
            if (Request.Cookies["ex"] != null)
            {
                HttpCookie expirescookie = new HttpCookie("ex");
                expirescookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(expirescookie);
            }
            if (Request.Cookies["NewLinks"] != null)
            {
                HttpCookie newlinkscookie = new HttpCookie("NewLinks");
                newlinkscookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(newlinkscookie);
            }
            if (Request.Cookies["Password"] != null)
            {
                HttpCookie passwordcookie = new HttpCookie("Password");
                passwordcookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(passwordcookie);
            }
            if (Request.Cookies["ServerUser"] != null)
            {
                HttpCookie serverusercookie = new HttpCookie("ServerUser");
                serverusercookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(serverusercookie);
            }
            // 清除所有曾經寫入過的 Session 資料
            //Session.Clear();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GuestLogin(string returnUrl)
        {
            _accountService = new AccountService(AccountAuthFactory.AuthType.ecweb.ToString());
            Login model = new Login();
            model.ratm = _accountService.GenerateTryTime(string.Format("{0}~{1}~{2}",
                "0",
                ConfigurationManager.GetTaiwanTime(),
                NetPacketUtilities.GetUserIPAddress()));
            model.rytm = false;
            model.type = AccountAuthFactory.AuthType.ecweb.ToString();
            model.acty = _accountService.GetActivityNow();
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GuestLogin(Login model, int agreePaper, string returnUrl)
        {
            _accountService = new AccountService(AccountAuthFactory.AuthType.ecweb.ToString());
            bool loginStatus = new bool(), checkStatus = true;
            
            if (string.IsNullOrEmpty(model.user))
            {
                checkStatus = false;
                model.Iderro = "帳號格式錯誤，請重新輸入";
            }

            if (agreePaper != 1)
            {
                checkStatus = false;
                model.erro = "請閱讀非會員使用條款並勾選已閱讀完畢";
            }

            string[] decryContent = _accountService.DecryptRandString(model.ratm);

            if (decryContent.Length < 3 && decryContent[3] != NetPacketUtilities.GetUserIPAddress())
            {
                checkStatus = false;
                model.erro = "驗證碼失敗，請再試一次";
            }

            if (int.Parse(decryContent[0]) > 3)
            {
                if (!_accountService.CheckGoogleReCaptcha(model.gcap))
                {
                    checkStatus = false;
                    model.erro = "驗證碼失敗，請再試一次";
                }
            }

            AccountVM IsEmailExisted = Processor.Request<AccountVM, AccountDM>("AccountService", "GetAccountByEmail", model.user).results;
            bool officialAccount = false;
            if (IsEmailExisted != null)
            {
                if (IsEmailExisted.GuestLogin != 1)
                {
                    officialAccount = true;
                    checkStatus = false;
                    model.Iderro = "帳號已存在，請更換帳號";
                }
            }

            if (checkStatus)
            {
                var exist = _accountService.CheckGuestLogin(model);
                loginStatus = (exist != null) ? true : false;
            }

            if (ModelState.IsValid)
            {
                if (!loginStatus && officialAccount == false)
                {
                    // 創建新account & member
                    ECWebResponse response = new ECWebResponse();
                    //AccountInfoVM accountInfo = new AccountInfoVM();
                    ResponsePacket<AccountInfoVM> registerResult;
                    RegisterVM saveAccountVM = new RegisterVM();
                    this._accountService = new AccountService(AccountAuthFactory.AuthType.ecweb.ToString());
                    saveAccountVM.GuestLogin = 1;
                    saveAccountVM.Email = model.user;
                    string tempPWD = DateTime.Now.Ticks.ToString();
                    tempPWD = tempPWD.Substring(0, 16);
                    saveAccountVM.PWDtxt = _aesCode.Enprypt(tempPWD);
                    saveAccountVM.PWD = _aesCode.Enprypt(tempPWD);
                    saveAccountVM.AgreePaper = agreePaper;
                    //RegistrationError errorMsg = this._accountService.CheckGuestRegister(saveAccountVM);
                    //if (!errorMsg.error)
                    //{
                        TWNewEgg.Models.DomainModels.Account.AccountDM accountDM = new TWNewEgg.Models.DomainModels.Account.AccountDM();
                        AutoMapper.Mapper.Map(saveAccountVM, accountDM);
                        registerResult = Processor.Request<AccountInfoVM, AccountInfoDM>("AccountService", "Register", accountDM);
                        if (string.IsNullOrWhiteSpace(registerResult.error))
                        {
                            loginStatus = true;
                            _accountService.CheckGuestLogin(model);
                        }
                        else
                        {
                            checkStatus = false;
                            model.erro = "系統出現異常，請聯絡管理員";
                        }
                    //}
                    //else
                    //{
                    //    checkStatus = false;
                    //    model.erro = "系統出現異常，請聯絡管理員";
                    //}
                }

                if (loginStatus)
                {
                    //Add cookies item into shopping cart.
                    List<CartTrack> allCartCookies = TWNewEgg.Framework.Common.JSONSerialization.Deserializer<List<CartTrack>>(CookiesUtilities.CookiesUtility.ReadCookies("sc") ?? "[]");
                    var ttt = NEUser.ID;
                    if (allCartCookies != null || allCartCookies.Count > 0)
                    {
                        Api.TrackController addTrack = new Api.TrackController();
                        List<string> results = addTrack.Post(allCartCookies);
                    }
                    Processor.Request<bool, bool>("TrackService", "CleanOldAndUpdateTracks", NEUser.ID, ConfigurationManager.GetTaiwanTime().AddDays(-30));
                    return RedirectToLocal(returnUrl);
                }
            }
            else
            {
                checkStatus = false;
                model.Pwderro = "帳號格是錯誤，請重新輸入";
            }

            //return failed message...
            model.gcap = string.Empty;
            model.pass = string.Empty;
            model.rytm = _accountService.IsTryManyTime(decryContent);
            model.ratm = _accountService.GenerateTryTime(string.Format("{0}~{1}~{2}",
                (int.Parse(decryContent[0]) + 1).ToString(),
                ConfigurationManager.GetTaiwanTime(),
                NetPacketUtilities.GetUserIPAddress()));
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult EditPersonInfo()
        {
            //MemberVM memberVM = new MemberVM();

            var memberInfo = Processor.Request<AccountInfoVM, AccountInfoDM>("AccountService", "EditPersonInfo", NEUser.Email);
            memberInfo.results.AVM.PWD = "";
            memberInfo.results.AVM.Loginon = null;
            
            return View(memberInfo.results);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult EditPersonInfo(MemberVM SaveMemberVM, int EDM)
        {
           // MemberVM memberVM = new MemberVM();
            if (SaveMemberVM == null || SaveMemberVM.Birthday == null)
            {
                return Json(false);
            }
            var result = Processor.Request<bool, bool>("AccountService", "EditPersonInformation", SaveMemberVM, EDM, NEUser.Email);


            return Json(result.results);

        }

        [HttpGet]
        public ActionResult EditAccount()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EditAccount(string email2)
        {
            var result = Processor.Request<string, string>("AccountService", "EditAccount", NEUser.Email, email2);
            return Json(result.results);
        }

        public ActionResult EditPassword()
        {
            int accID = NEUser.ID;
            return View();
        }
        [AllowAnonymous]
        public ActionResult AskQuestionGuest(int itemid = 0)
        {
            #region 如果有帶入 itemid 表示我要發問是從賣場頁過來, 並用 ViewBag 紀錄 item id 以方便顯示在前台畫面上. 沒有就是從賣場以外的地方過來
            //判斷是否有帶入 item id, 並用 ViewBag 紀錄 item id 以方便顯示在前台畫面上
            if (itemid == 0)
            {
                ViewBag.itemid = string.Empty;
            }
            else
            {
                ViewBag.itemid = itemid.ToString();
            }
            #endregion
            return View();
        }
        /// <summary>
        /// 提出問題, 並判斷是否登入, 定導向對應的畫面
        /// </summary>
        /// <param name="SalesOrderCode"></param>
        /// <param name="rtgood"></param>
        /// <param name="itemid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult AskQuestion(string SalesOrderCode, string rtgood, int itemid = 0)
        {
            #region 判斷是否登入, 非登入狀態表示為 guest 導向 guest 的 action, 並帶入 itemid
            //判斷是否登入
            if (NEUser.IsAuthticated == false)
            {
                return RedirectToAction("AskQuestionGuest", "MyAccount", new { itemid = itemid });
            }
            #endregion
            int accID = NEUser.ID;
            string Name = NEUser.Name;
         
            AnswerViewModel AnswerInfo = new AnswerViewModel();

            var Answer = Processor.Request<AnswerInfo, AnswerInfo>("AnswerService", "GetSalceOrderInfo", SalesOrderCode, accID, Name);
            AutoMapper.Mapper.Map(Answer.results.SalesOrder, AnswerInfo);
            //如果有訂單, 表示可以從訂單的 model 中取出賣場編號
            if (Answer.results != null && Answer.results.SalesOrderItem != null)
            {
                AnswerInfo.ItemID = Answer.results.SalesOrderItem[0].ItemID;
            }
            else
            {
                AnswerInfo.RecvName = NEUser.Name;
                AnswerInfo.Email = NEUser.Email;
                //我要發問的來源可能是從賣場來, 所以必須把賣場編號 assign 給 AnswerInfo.ItemID, 在前端判斷是否從賣場頁來, 並且判斷是否要顯示賣場編號
                AnswerInfo.ItemID = itemid == 0 ? 0 : itemid;
            }
            if (rtgood == "true")
            {
                AnswerInfo.rtgood = true;
            }

            return View(AnswerInfo);
        }
        public ActionResult AddAskQestion(string ReturnPostString, short? faqtypeval, string maintext)
        {
            int accID = NEUser.ID;
            var ReturnPost = TWNewEgg.Framework.Common.JSONSerialization.Deserializer<AnswerViewModel>(ReturnPostString);
            
            AnswerInfo AnswerInfo = new TWNewEgg.Models.DomainModels.Answer.AnswerInfo();
            TWNewEgg.Models.DomainModels.Answer.SalesOrderInfo SalesOrderDetail = new TWNewEgg.Models.DomainModels.Answer.SalesOrderInfo();
            AutoMapper.Mapper.Map(ReturnPost, SalesOrderDetail);
            SalesOrderDetail.Mobile = string.IsNullOrEmpty(SalesOrderDetail.RecvMobile) == true ? string.Empty : SalesOrderDetail.RecvMobile;
            SalesOrderDetail.Name = string.IsNullOrEmpty(ReturnPost.RecvName) == true ? string.Empty : ReturnPost.RecvName;
            AnswerInfo.SalesOrder = SalesOrderDetail;
            var Answer = Processor.Request<TWNewEgg.Models.ViewModels.Redeem.ActionResponse<AnswerInfo>, TWNewEgg.Models.ViewModels.Redeem.ActionResponse<AnswerInfo>>("AnswerService", "AddSalseOrderForAnswerInfo", AnswerInfo.SalesOrder, ReturnPost.ItemID, faqtypeval, maintext, accID);
            return Json(Answer.results.Msg);
      
        }

        #region 非會員我要發問問題
        /// <summary>
        /// 非登入時提出問題並按下送出
        /// </summary>
        /// <param name="ReturnPostString">Json字串</param>
        /// <param name="faqtypeval">問題主旨</param>
        /// <param name="maintext">說明內容</param>
        /// <returns>Json Result</returns>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult AddAskQestionGuest(string ReturnPostString, short? faqtypeval, string maintext, string OrderlistValue)
        {
            //讀取 accID
            int accID = NEUser.ID;
            AnswerViewModel ReturnPost = new AnswerViewModel();
            #region try catch area (防止反序列時錯誤)
            //用來判斷是否發生 Exception flag
            bool isNoException = true;
            try
            {
                //對 json 字串進行反序列化
                ReturnPost = TWNewEgg.Framework.Common.JSONSerialization.Deserializer<AnswerViewModel>(ReturnPostString);
                isNoException = true;
            }
            catch (Exception error)
            {
                logger.Error(this.ExceptionMsg(error));
                isNoException = false;
            }
            //發生 Exception
            if (isNoException == false)
            {
                return Json("False");
            }
            #endregion
            #region 檢查傳進來的資料是否正確
            //利用前端傳過來問題類型去做對應的數值轉換
            int problemType = this.ConvertProblemTypeToInt(OrderlistValue);
            //-99 即轉換失敗
            if (problemType == -99)
            {
                return Json("False");
            }
            else
            {
                //開始檢查資料完整性
                var CheckDataCurrent = this.CheckProblemSubmit(problemType, faqtypeval, maintext, ReturnPost);
                if (CheckDataCurrent.IsSuccess == false)
                {
                    return Json("[Error]: " + CheckDataCurrent.Msg);
                }
            }
            #endregion
            #region 開始新增問題單
            AnswerInfo AnswerInfo = new TWNewEgg.Models.DomainModels.Answer.AnswerInfo();
            TWNewEgg.Models.DomainModels.Answer.SalesOrderInfo SalesOrderDetail = new TWNewEgg.Models.DomainModels.Answer.SalesOrderInfo();
            AutoMapper.Mapper.Map(ReturnPost, SalesOrderDetail);
            //把發問人的名字放回 SalesOrderDetail.Name 再傳到 service 進行 insert
            SalesOrderDetail.Name = string.IsNullOrEmpty(ReturnPost.RecvName) == true ? string.Empty : ReturnPost.RecvName;
            //把發問人的電話放回 SalesOrderDetail.Mobile 再傳到 service 進行 insert
            SalesOrderDetail.Mobile = string.IsNullOrEmpty(ReturnPost.RecvMobile) == true ? string.Empty : ReturnPost.RecvMobile;
            AnswerInfo.SalesOrder = SalesOrderDetail;
            //呼叫 service
            var Answer = Processor.Request<TWNewEgg.Models.ViewModels.Redeem.ActionResponse<AnswerInfo>, TWNewEgg.Models.ViewModels.Redeem.ActionResponse<AnswerInfo>>("AnswerService", "AddSalseOrderForAnswerInfo", AnswerInfo.SalesOrder, ReturnPost.ItemID, faqtypeval, maintext, accID);
            //sercvice 發生錯誤
            if (Answer.error != null)
            {
                return Json("False");
            }
            return Json(Answer.results.Msg);
            #endregion
        }
        #region 利用前端傳過來問題類型去做對應的數值轉換
        /// <summary>
        /// 利用前端傳過來問題類型去做對應的數值轉換
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public int ConvertProblemTypeToInt(string Value)
        {
            int returnInt;
            try
            {
                //利用問題類型去做 Enum 轉換
                //true 表示忽略大小寫, false 表示區分大小寫.
                returnInt = (int)((problemType)Enum.Parse(typeof(problemType), Value, true));
            }
            catch (Exception error)
            {
                //轉換失敗
                logger.Error(this.ExceptionMsg(error));
                returnInt = -99;
            }

            return returnInt;
        }
        #endregion
        #region 檢查前端過來的資料是否符合完整性
        /// <summary>
        /// 檢查前端過來的資料是否符合完整性
        /// </summary>
        /// <param name="problemType"></param>
        /// <param name="faqtypeval"></param>
        /// <param name="maintext"></param>
        /// <param name="answerViewModel"></param>
        /// <returns></returns>
        public TWNewEgg.Models.ViewModels.Redeem.ActionResponse<string> CheckProblemSubmit(int problemType, short? faqtypeval, string maintext, AnswerViewModel answerViewModel)
        {
            TWNewEgg.Models.ViewModels.Redeem.ActionResponse<string> result = new TWNewEgg.Models.ViewModels.Redeem.ActionResponse<string>();
            //檢查問題類型是否對應到正確的問題主旨
            var CheckProblemTypeResult = this.CheckProblemType(problemType, faqtypeval);
            if (CheckProblemTypeResult.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = CheckProblemTypeResult.Msg;
                return result;
            }
            else
            {
                //檢查序列化後的 model 資料是否有誤
                var CheckAnswerViewModelResult = this.CheckAnswerViewModel(answerViewModel, maintext);
                if (CheckAnswerViewModelResult.IsSuccess == true)
                {
                    result.IsSuccess = true;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = CheckAnswerViewModelResult.Msg;
                }
                return result;
            }

        }
        #endregion
        #region 用問題類型去檢查是否是對應到對的問題主旨
        /// <summary>
        /// 用問題類型去檢查是否是對應到對的問題主旨
        /// </summary>
        /// <param name="problemType">問題類型</param>
        /// <param name="problempurpose">問題主旨</param>
        /// <param name="answerinfo"></param>
        /// <returns></returns>
        public TWNewEgg.Models.ViewModels.Redeem.ActionResponse<string> CheckProblemType(int problemType, short? problempurpose)
        {
            TWNewEgg.Models.ViewModels.Redeem.ActionResponse<string> result = new TWNewEgg.Models.ViewModels.Redeem.ActionResponse<string>();
            switch (problemType)
            {
                #region 根據問題類型去檢查是否是對應的問題主旨
                case (int)TWNewEgg.Models.ViewModels.Answer.problemType.Salceorder:
                    {
                        #region 訂單進度
                        if (problempurpose == (int)TWNewEgg.Models.ViewModels.Answer.status.訂購是否成功 ||
                            problempurpose == (int)TWNewEgg.Models.ViewModels.Answer.status.更改訂單資料 ||
                            problempurpose == (int)TWNewEgg.Models.ViewModels.Answer.status.查詢出貨進度 ||
                            problempurpose == (int)TWNewEgg.Models.ViewModels.Answer.status.取消訂單 ||
                            problempurpose == (int)TWNewEgg.Models.ViewModels.Answer.status.報關文件相關問題)
                        {
                            result.IsSuccess = true;
                            result.Msg = "Success";
                            result.Body = null;
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Msg = "請選填正確的問題類型與問題主旨";
                            result.Body = null;
                        }
                        break;
                        #endregion
                    }
                case (int)TWNewEgg.Models.ViewModels.Answer.problemType.Retgood:
                    {
                        #region 退換維修
                        if (problempurpose == (int)TWNewEgg.Models.ViewModels.Answer.status.七天鑑賞期退貨 ||
                            problempurpose == (int)TWNewEgg.Models.ViewModels.Answer.status.新品瑕疵換貨 ||
                            problempurpose == (int)TWNewEgg.Models.ViewModels.Answer.status.商品缺件或不符 ||
                            problempurpose == (int)TWNewEgg.Models.ViewModels.Answer.status.商品維修及保固)
                        {
                            result.IsSuccess = true;
                            result.Msg = "Success";
                            result.Body = null;
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Msg = "請選填正確的問題類型與問題主旨";
                            result.Body = null;
                        }
                        break;
                        #endregion
                    }
                case (int)TWNewEgg.Models.ViewModels.Answer.problemType.Invoice:
                    {
                        #region 帳款與發票
                        if (problempurpose == (int)TWNewEgg.Models.ViewModels.Answer.status.付款是否成功 ||
                            problempurpose == (int)TWNewEgg.Models.ViewModels.Answer.status.退款問題 ||
                            problempurpose == (int)TWNewEgg.Models.ViewModels.Answer.status.發票問題 ||
                            problempurpose == (int)TWNewEgg.Models.ViewModels.Answer.status.折價券問題)
                        {
                            result.IsSuccess = true;
                            result.Msg = "Success";
                            result.Body = null;
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Msg = "請選填正確的問題類型與問題主旨";
                            result.Body = null;
                        }
                        break;
                        #endregion
                    }
                case (int)TWNewEgg.Models.ViewModels.Answer.problemType.Item:
                    {
                        #region 賣場相關
                        if (problempurpose == (int)TWNewEgg.Models.ViewModels.Answer.status.商品規格 ||
                            problempurpose == (int)TWNewEgg.Models.ViewModels.Answer.status.訂購及付款方式 ||
                            problempurpose == (int)TWNewEgg.Models.ViewModels.Answer.status.行銷活動諮詢)
                        {
                            result.IsSuccess = true;
                            result.Msg = "Success";
                            result.Body = null;
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Msg = "請選填正確的問題類型與問題主旨";
                            result.Body = null;
                        }
                        break;
                        #endregion
                    }
                case (int)TWNewEgg.Models.ViewModels.Answer.problemType.Other:
                    {
                        #region 其他問題
                        if (problempurpose == (int)TWNewEgg.Models.ViewModels.Answer.status.詐騙相關問題 ||
                            problempurpose == (int)TWNewEgg.Models.ViewModels.Answer.status.其他問題 ||
                            problempurpose == (int)TWNewEgg.Models.ViewModels.Answer.status.系統網頁問題)
                        {
                            result.IsSuccess = true;
                            result.Msg = "Success";
                            result.Body = null;
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Msg = "請選填正確的問題類型與問題主旨";
                            result.Body = null;
                        }
                        break;
                        #endregion
                    }
                default:
                    {
                        #region 沒有對應到現有的問題類型
                        result.IsSuccess = false;
                        result.Msg = "請選填正確的問題類型與問題主旨";
                        result.Body = null;
                        break;
                        #endregion
                    }
                #endregion
            }
            return result;
        }
        #endregion
        #region 檢查姓名, 電話, Email, 說明
        TWNewEgg.Models.ViewModels.Redeem.ActionResponse<string> CheckAnswerViewModel(AnswerViewModel answerViewModel, string maintext)
        {
            TWNewEgg.Models.ViewModels.Redeem.ActionResponse<string> result = new TWNewEgg.Models.ViewModels.Redeem.ActionResponse<string>();
            result.IsSuccess = true;
            result.Msg = "Success";
            try
            {
                #region 檢查姓名
                if (string.IsNullOrEmpty(answerViewModel.RecvName.Replace(" ", "")) == true)
                {
                    result.IsSuccess = false;
                    result.Msg = "請填寫姓名";
                    return result;
                }
                #endregion
                #region 檢查電話, 有填寫再去檢查
                if (string.IsNullOrEmpty(answerViewModel.RecvMobile.Replace(" ", "")) == false)
                {
                    //檢查電話的格式
                    var CheckMobileFormatResult = this.CheckMobileFormat(answerViewModel.RecvMobile);
                    if (CheckMobileFormatResult.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = CheckMobileFormatResult.Msg;
                        return result;
                    }
                }
                #endregion
                #region 檢查 Email
                if (string.IsNullOrEmpty(answerViewModel.Email) == true)
                {
                    result.IsSuccess = false;
                    result.Msg = "連絡E-mail必須為完整的e-mail，如xxx@xxx.xxx.xx";
                    return result;
                }
                //檢查 Email 格式
                var CheckEmailFormatResult = this.CheckEmailFormat(answerViewModel.Email);
                if (CheckEmailFormatResult.IsSuccess == false)
                {
                    result.IsSuccess = false;
                    result.Msg = "連絡E-mail必須為完整的e-mail，如xxx@xxx.xxx.xx";
                    return result;
                }
                #endregion
                #region 檢查請說明
                if (string.IsNullOrEmpty(maintext.Replace(" ", "")) == true)
                {
                    result.IsSuccess = false;
                    result.Msg = "問題尚未填寫";
                    return result;
                }
                #endregion
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "系統發生錯誤";
                logger.Error(ExceptionMsg(error));
            }
            return result;
        }
        #endregion
        #region Email 格式檢查
        TWNewEgg.Models.ViewModels.Redeem.ActionResponse<string> CheckEmailFormat(string Email)
        {
            TWNewEgg.Models.ViewModels.Redeem.ActionResponse<string> result = new TWNewEgg.Models.ViewModels.Redeem.ActionResponse<string>();
            bool isEmail = System.Text.RegularExpressions.Regex.IsMatch(Email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4})$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (isEmail == false)
            {
                result.IsSuccess = false;
                result.Msg = "連絡E-mail必須為完整的e-mail，如xxx@xxx.xxx.xx";
            }
            else
            {
                result.IsSuccess = true;
                result.Msg = "Success";
            }
            return result;
        }
        #endregion
        #region 檢查電話格式
        TWNewEgg.Models.ViewModels.Redeem.ActionResponse<string> CheckMobileFormat(string MobilePhoneNumber)
        {
            TWNewEgg.Models.ViewModels.Redeem.ActionResponse<string> result = new TWNewEgg.Models.ViewModels.Redeem.ActionResponse<string>();
            if (System.Text.RegularExpressions.Regex.IsMatch(MobilePhoneNumber, @"[+]{0,1}[2-8]{0,3}[0]{0,1}[2-8]{1}[-+]{0,1}[0-9]{7,8}$", System.Text.RegularExpressions.RegexOptions.IgnoreCase) == true ||
                System.Text.RegularExpressions.Regex.IsMatch(MobilePhoneNumber, @"09[0-9]{8}$", System.Text.RegularExpressions.RegexOptions.IgnoreCase) == true)
            {
                result.IsSuccess = true;
                result.Msg = "Success";
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = "請填寫正確電話號碼!!";
            }
            return result;
        }
        #endregion
        #endregion


        public ActionResult QuestionRecord(int Mouth = 3, string Salceorder = "")
        {
            int accID = NEUser.ID;
            string Email = NEUser.Email;
            AnswerInfo AnswerInfo = new TWNewEgg.Models.DomainModels.Answer.AnswerInfo();
            TWNewEgg.Models.DomainModels.Answer.SalesOrderInfo SalesOrderDetail = new TWNewEgg.Models.DomainModels.Answer.SalesOrderInfo();
            var Info = Processor.Request<List<AnswerInfo>, List<AnswerInfo>>("AnswerService", "GetPrblmRecode", accID, Mouth, Email, Salceorder);
            List<TWNewEgg.Models.ViewModels.Answer.Probelm> Problemlist = new List<Probelm>();
            foreach (var Detile in Info.results)
             {
                TWNewEgg.Models.ViewModels.Answer.Probelm Prblm = new Probelm();
                 AutoMapper.Mapper.Map(Detile.Probelm, Prblm);
                 Prblm.Prblmtype = Enum.GetName(typeof(TWNewEgg.Models.ViewModels.Answer.status), Detile.Probelm.IntClass).ToString();
                 List<TWNewEgg.Models.ViewModels.Answer.Answer> asnlist = new List<Answer>();
                 foreach (var AnswerDetil in Detile.AnswerList) 
                 {
                     TWNewEgg.Models.ViewModels.Answer.Answer asn = new Answer();
                     
                     AutoMapper.Mapper.Map(AnswerDetil, asn);
                     asn.CreateDate = (DateTime)AnswerDetil.Date;
                     asnlist.Add(asn);
                 }
                 Prblm.Answerlist = new List<Answer>();
                 Prblm.Answerlist = asnlist;
                 if (Detile.SalesOrderItem != null)
                 {
                     Prblm.Name = Detile.SalesOrderItem[0].Name;
                 }
                 Problemlist.Add(Prblm);
                
             }
             return View(Problemlist); 
        }

        //public ActionResult Order()
        //{
        //    return View();
        //}

        public ActionResult Invoice()
        {
            return View();
        }

        public ActionResult Coupon()
        {
            List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listAllCoupon = null;
            string strAccountId = "";
            List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listActiveCoupon = null;   //已生效可使用的Coupon
            List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listUsedCoupon = null; //已經使用過的Coupon
            List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listExpiredCoupon = null; //過期的Coupon
            List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listWaitingForActivecoupon = null; //待生效的Coupon
            List<TWNewEgg.DB.TWSQLDB.Models.SalesOrderItem> listSalesOrderItem = null;
            List<string> listSalesOrderItemCode = null;
            TWNewEgg.DB.TWSQLDB.Models.SalesOrderItem oSalesOrderItem = null;
            TWNewEgg.DB.TWSqlDBContext oDb = null;
            Dictionary<int, List<TWNewEgg.Models.ViewModels.Redeem.Coupon>> oDictResult = null;
            DateTime dateTimeNow = DateTime.Now;

            try
            {
            strAccountId = NEUser.ID.ToString();
            oDictResult = new Dictionary<int, List<TWNewEgg.Models.ViewModels.Redeem.Coupon>>();

            // 將Coupon分類
            // 已生效但未使用
            listActiveCoupon = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>, List<TWNewEgg.Models.DomainModels.Redeem.Coupon>>("Service.CouponService.CouponServiceRepository", "getActiveCouponListByAccount", strAccountId).results;
            // 修改Categories的說明
            if (listActiveCoupon != null)
            {
                this.getCouponCategoriesDesc(ref listActiveCoupon);
            }
            oDictResult.Add(1, listActiveCoupon);

            // 已使用:僅列出3個月內的消費記錄
            listUsedCoupon = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>, List<TWNewEgg.Models.DomainModels.Redeem.Coupon>>("Service.CouponService.CouponServiceRepository", "getUsedCouponIn3MonthListByAccount", strAccountId).results;
            if (listUsedCoupon != null && listUsedCoupon.Count > 0)
            {
                // 置換x.ordcode為顯示商品名稱, 而非消費者看不懂的FBS序號
                listSalesOrderItemCode = listUsedCoupon.Select(x => x.ordcode).ToList();
                oDb = new DB.TWSqlDBContext();
                listSalesOrderItem = oDb.SalesOrderItem.Where(x => listSalesOrderItemCode.Contains(x.Code)).ToList();
                if (listSalesOrderItem != null)
                {
                    foreach (TWNewEgg.Models.ViewModels.Redeem.Coupon oSubCoupon in listUsedCoupon)
                    {
                        oSalesOrderItem = listSalesOrderItem.FirstOrDefault(x => x.Code == oSubCoupon.ordcode);
                        if (oSalesOrderItem != null)
                            oSubCoupon.ordcode = "<div style='display:none;'>" + oSubCoupon.ordcode + "</div>" + oSalesOrderItem.Name;
                        // 判斷coupon的Used狀況, 若是「使用後訂單取消」也要特別顯示
                        if (oSubCoupon.usestatus == (int)TWNewEgg.Models.ViewModels.Redeem.Coupon.CouponUsedStatusOption.UsedButCancel)
                            oSubCoupon.ordcode += "<br><span style='color:red'>此項商品已取消訂購</span>";
                    }// end foreach
                }
                oDb.Dispose();
                // 修改Categories的說明
                this.getCouponCategoriesDesc(ref listUsedCoupon);
            }
            oDictResult.Add(2, listUsedCoupon);

            // 未使用但過期
            listExpiredCoupon = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>, List<TWNewEgg.Models.DomainModels.Redeem.Coupon>>("Service.CouponService.CouponServiceRepository", "getExpiredCouponListByAccount", strAccountId).results;
            // 修改Categories的說明
            if (listExpiredCoupon != null)
            {
                //縮減為一年內的記錄
                listExpiredCoupon = listExpiredCoupon.Where(x => x.validend >= DateTime.Now.AddDays(-365)).ToList();
                if (listExpiredCoupon.Count > 0)
                {
                    this.getCouponCategoriesDesc(ref listExpiredCoupon);
                    listExpiredCoupon = listExpiredCoupon.OrderByDescending(x => x.validend).ToList();
                }
                else
                {
                    listExpiredCoupon = null;
                }
            }
            oDictResult.Add(3, listExpiredCoupon);

            // 待生效 : Event設定可以使用coupon
            listWaitingForActivecoupon = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>, List<TWNewEgg.Models.DomainModels.Redeem.Coupon>>("Service.CouponService.CouponServiceRepository", "getWaitingActiveCouponByAccount", strAccountId).results;
            // 修改Categories的說明
            if (listWaitingForActivecoupon != null)
            {
                this.getCouponCategoriesDesc(ref listWaitingForActivecoupon);
            }
            oDictResult.Add(4, listWaitingForActivecoupon);

            //釋放記憶體
            listSalesOrderItem = null;
            listSalesOrderItemCode = null;
            oSalesOrderItem = null;
            }
            catch(Exception ex)
            {
                logger.Info("[MyAccount][Coupon-mesage]：" +ex.Message);
                logger.Info("[MyAccount][Coupon-StackTrace]：" + ex.StackTrace);
            }

            return View(oDictResult);
        }

        public ActionResult NeweggCash()
        {
            return View();
        }

        public ActionResult RecordBooks()
        {
            int getAccID = NEUser.ID;
            CartMemberInfoVM getCartMemberInfo = Processor.Request<CartMemberInfoVM, CartMemberInfoDM>("GetMemberService", "GetCartMemberInfo", getAccID).results;
            return View(getCartMemberInfo);
        }

        public ActionResult Partial_MemberAddressBook(CartMemberInfoVM getCartMemberInfo)
        {
            return PartialView(getCartMemberInfo);
        }

        public ActionResult Partial_RecvAddressBook(CartMemberInfoVM getCartMemberInfo)
        {
            return PartialView(getCartMemberInfo);
        }

        /// <summary>
        /// 將該View轉成string
        /// </summary>
        /// <param name="partialView">View的名稱</param>
        /// <returns>返回string</returns>
        private string RenderView(string partialView)
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

        public ActionResult CSAddressBook()
        {
            return View();
        }

        public ActionResult VATBook(CartMemberInfoVM getCartMemberInfo)
        {
            return PartialView(getCartMemberInfo);
        }
        public ActionResult Order(int? length, int start = 0)
        {
            TWNewEgg.ECWeb.Services.Page.CalculationsPage Calculer = new TWNewEgg.ECWeb.Services.Page.CalculationsPage();
             TWNewEgg.Website.ECWeb.Service.OrderDetail OrderDetail = new Website.ECWeb.Service.OrderDetail();
         
              const int recentOrderSpan = 4;
              List<TWNewEgg.Models.ViewModels.Page.ShowPage> getShowPages = new List<TWNewEgg.Models.ViewModels.Page.ShowPage>();
            int accID = NEUser.ID;
            logger.Info("ACCID" + accID);
            if (accID > 0)
            {
                int count = OrderDetail.SalesOrderData(length, accID);
                int Page = (int)Math.Ceiling(((double)count / (double)recentOrderSpan));
                getShowPages = Calculer.GetShowPageWithoutLimit(Page, start, 4);
                start = start / recentOrderSpan * recentOrderSpan;
                if (start >= count)
                {
                    start = (count - 1) / recentOrderSpan * recentOrderSpan;
                }
                if (start < 0)
                {
                    start = 0;
                }
                ViewBag.tabOrder = 0;
                ViewBag.length = count;
                ViewBag.span = recentOrderSpan;
                ViewBag.start = start;
                bool ajax = Request.IsAjaxRequest();


                ViewBag.Calcular = getShowPages;
             
                TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.OrderHistory> RecentOrderItemDetail = OrderDetail.OrderHistory(length, start, accID);
                TWNewEgg.Models.ViewModels.MyAccount.OrderHistory OrderHistory = new TWNewEgg.Models.ViewModels.MyAccount.OrderHistory();
                TWNewEgg.Models.ViewModels.Cart.ItemGroup_View ItemGroup_View = new TWNewEgg.Models.ViewModels.Cart.ItemGroup_View();
                ItemGroup_View.ViewPage = 1;
                ItemGroup_View.TotalPage = Page;
                ViewBag.ItemGroup_View = ItemGroup_View;
                ViewBag.tabOrder = 0;
                string resultString = "";
                
                OrderHistory = RecentOrderItemDetail.Body;
             
                if (RecentOrderItemDetail.Body != null)
                {
                    logger.Info("Body" + RecentOrderItemDetail.Body.totalpage);
                    if (RecentOrderItemDetail.Body.SalceOrderList != null && RecentOrderItemDetail.Body.SalceOrderList.Count() > 0)
                    {
                        logger.Info("Code" + RecentOrderItemDetail.Body.SalceOrderList[0].Code);
                        logger.Info("Status" + RecentOrderItemDetail.Body.SalceOrderList[0].Status);
                        logger.Info("PiceSum" + RecentOrderItemDetail.Body.SalceOrderList[0].PiceSum);
                    }
                }
                if (RecentOrderItemDetail != null && RecentOrderItemDetail.Body != null)
                {
                    OrderHistory.totalpage = RecentOrderItemDetail.Body.SalceOrderList.Count();
                }
                else
                {
                    OrderHistory = new TWNewEgg.Models.ViewModels.MyAccount.OrderHistory();
                    OrderHistory.totalpage = 0;
                }

                return View("Order", OrderHistory);
            }
            else if (!Request.IsAjaxRequest())
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/MyAccount/Order" });
            }
            else
                return JavaScript("window.location = '" + Url.Action("Login", "Account", new { returnUrl = "/MyAccount/Order" }) + "'");
        
      

           
        }
        public ActionResult NextOrderDetail(string stringGetDetil, int start, int? length)
        {
            var ReturnPost = TWNewEgg.Framework.Common.JSONSerialization.Deserializer<TWNewEgg.Models.ViewModels.Cart.ItemGroup_View>(stringGetDetil);
            TWNewEgg.ECWeb.Services.Page.CalculationsPage Calculer = new TWNewEgg.ECWeb.Services.Page.CalculationsPage();
            TWNewEgg.Website.ECWeb.Service.OrderDetail OrderDetail = new Website.ECWeb.Service.OrderDetail();
            //第幾頁
            start = ReturnPost.ViewPage;
            const int recentOrderSpan = 4;
            length = null;
            List<TWNewEgg.Models.ViewModels.Page.ShowPage> getShowPages = new List<TWNewEgg.Models.ViewModels.Page.ShowPage>();
            int accID = NEUser.ID;
            logger.Info("ACCID" + accID);
            if (accID > 0)
            {
                int count = OrderDetail.SalesOrderData(length, accID);
                int Page = (int)Math.Ceiling(((double)count / (double)recentOrderSpan));
                getShowPages = Calculer.GetShowPageWithoutLimit(Page, ReturnPost.ViewPage, 4);
                //start = start / recentOrderSpan * recentOrderSpan;
                start = (start - 1) * recentOrderSpan;
                if (start >= count)
                {
                    start = (count - 1) / recentOrderSpan * recentOrderSpan;
                }
                if (start < 0)
                {
                    start = 0;
                }
                ViewBag.tabOrder = 0;
                //總筆數
                ViewBag.length = count;
                //每頁筆數
                ViewBag.span = recentOrderSpan;
                //開始筆數
                ViewBag.start = start;
                bool ajax = Request.IsAjaxRequest();



                ViewBag.Calcular = getShowPages;

                TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.OrderHistory> RecentOrderItemDetail = OrderDetail.OrderHistory(length, start, accID);
                TWNewEgg.Models.ViewModels.MyAccount.OrderHistory OrderHistory = new TWNewEgg.Models.ViewModels.MyAccount.OrderHistory();
                TWNewEgg.Models.ViewModels.Cart.ItemGroup_View ItemGroup_View = new TWNewEgg.Models.ViewModels.Cart.ItemGroup_View();
                ItemGroup_View.ViewPage = ReturnPost.ViewPage;
                ItemGroup_View.TotalPage = Page;
    
                ViewBag.tabOrder = 0;
                string resultString = "";

                OrderHistory = RecentOrderItemDetail.Body;

                if (RecentOrderItemDetail.Body != null)
                {
                    logger.Info("Body" + RecentOrderItemDetail.Body.totalpage);
                    if (RecentOrderItemDetail.Body.SalceOrderList != null && RecentOrderItemDetail.Body.SalceOrderList.Count() > 0)
                    {
                        logger.Info("Code" + RecentOrderItemDetail.Body.SalceOrderList[0].Code);
                        logger.Info("Status" + RecentOrderItemDetail.Body.SalceOrderList[0].Status);
                        logger.Info("PiceSum" + RecentOrderItemDetail.Body.SalceOrderList[0].PiceSum);
                    }
                }
                if (RecentOrderItemDetail != null && RecentOrderItemDetail.Body != null)
                {
                    OrderHistory.totalpage = RecentOrderItemDetail.Body.SalceOrderList.Count();
                }
                else
                {
                    OrderHistory = new TWNewEgg.Models.ViewModels.MyAccount.OrderHistory();
                    OrderHistory.totalpage = 0;
                }
                string returnString = "";
                TWNewEgg.Models.ViewModels.MyAccount.ReturnPost retspost = new TWNewEgg.Models.ViewModels.MyAccount.ReturnPost();

                using (StringWriter sw = new StringWriter())
                {
                    ViewData.Model = OrderHistory;
                    ViewBag.ItemGroup_View = ItemGroup_View;
                    ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Partial_Order");
                    ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                    viewResult.View.Render(viewContext, sw);
                    returnString = sw.GetStringBuilder().ToString();
                }
                if (Request.IsAjaxRequest())
                {
                    return Json(Regex.Replace(returnString, @"[\r\n]+\s{0,}[\r\n]+", " "), JsonRequestBehavior.AllowGet);
                }

                return View("Order", OrderHistory);


            }
            else if (!Request.IsAjaxRequest())
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/MyAccount/Order" });
            }
            else
                return JavaScript("window.location = '" + Url.Action("Login", "Account", new { returnUrl = "/MyAccount/Order" }) + "'");




        }
        public ActionResult GetReturnDetail(string SOCode) 
        { 
             int accID = NEUser.ID;

            TWNewEgg.ECWeb.Services.OldCart.ReturnService.ReturnService ReturnService = new Services.OldCart.ReturnService.ReturnService();
            TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.ReturnPost> ActionResponse = new TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.ReturnPost>();

         
            ActionResponse = ReturnService.Return(SOCode, accID);
            if (ActionResponse.IsSuccess == true)
            {

                string resultString = "";
                TWNewEgg.Models.ViewModels.MyAccount.ReturnPost retspost = new TWNewEgg.Models.ViewModels.MyAccount.ReturnPost();
                retspost = ActionResponse.Body;
                using (StringWriter sw = new StringWriter())
                {
                    ViewData.Model = retspost;
                    ViewBag.SOCode = SOCode;
                    ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Partial_ReturnOrder");
                    ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                    viewResult.View.Render(viewContext, sw);
                    resultString = sw.GetStringBuilder().ToString();
                }
                if (Request.IsAjaxRequest())
                {
                    return Json(Regex.Replace(resultString, @"[\r\n]+\s{0,}[\r\n]+", " "), JsonRequestBehavior.AllowGet);
                }
                return RedirectToAction("Login", "Account", new { returnUrl = "/MyAccount/GetReturnDetail" });
            }
            else 
            {
                    return RedirectToAction("Logout", "MyAccount", new { returnUrl = "/Home" });
             
            }

          
       
        }
        public ActionResult ReturnPost(string ReturnPostString, int? return_reasonval, string return_reasontext)
        {       
            int accID = NEUser.ID;
            var ReturnPost = TWNewEgg.Framework.Common.JSONSerialization.Deserializer<TWNewEgg.Models.ViewModels.MyAccount.ReturnPost>(ReturnPostString);
         
            TWNewEgg.ECWeb.Services.OldCart.ReturnService.ReturnService ReturnService = new Services.OldCart.ReturnService.ReturnService();
            TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.ReturnPost> ReturnPostActionResponse = new TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.ReturnPost>();
            ReturnPostActionResponse = ReturnService.Returnpost(ReturnPost, return_reasonval, return_reasontext, accID);
            if (ReturnPostActionResponse.IsSuccess == true)
            {           
                return Json("Success");
            }
            else 
            {
                return Json(ReturnPostActionResponse.Msg); 
            }       
        }

        public ActionResult RefundPost(string ReturnPostString, int? return_reasonval, string return_reasontext)
        {
            int accID = NEUser.ID;

            var ReturnPost = TWNewEgg.Framework.Common.JSONSerialization.Deserializer<TWNewEgg.Models.ViewModels.MyAccount.ReturnPost>(ReturnPostString);


            TWNewEgg.ECWeb.Services.OldCart.ReturnService.ReturnService ReturnService = new Services.OldCart.ReturnService.ReturnService();
            TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.ReturnPost> ReturnPostActionResponse = new TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.ReturnPost>();
            ReturnPostActionResponse = ReturnService.Refundpost(ReturnPost, return_reasonval, return_reasontext, accID);
            if (ReturnPostActionResponse.IsSuccess == true)
            {

                return Json("1");
            }
            else
            {

                return Json(ReturnPostActionResponse.Msg);

            }



        }
        public ActionResult GetDetail(string SOCode)
        {
            List<TWNewEgg.Models.ViewModels.MyAccount.SalesOrderItem> SalesOrderItem = new List<TWNewEgg.Models.ViewModels.MyAccount.SalesOrderItem>();
            int accID = NEUser.ID;
            List<TWNewEgg.Models.ViewModels.MyAccount.SalceOrder> SalesOrder = new List<TWNewEgg.Models.ViewModels.MyAccount.SalceOrder>();
            TWNewEgg.Models.ViewModels.MyAccount.SalceOrder SalesOrderdetil = new TWNewEgg.Models.ViewModels.MyAccount.SalceOrder();
            List<TWNewEgg.Models.ViewModels.MyAccount.SalesOrderItem> SalesOrderItemList = new List<TWNewEgg.Models.ViewModels.MyAccount.SalesOrderItem>();
            TWNewEgg.Website.ECWeb.Service.OrderDetail OrderDetail = new Website.ECWeb.Service.OrderDetail();
            TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.OrderHistory> OrderHistory = new TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.OrderHistory>();
            OrderHistory = OrderDetail.SalesOrderItem(accID, SOCode);
            //SalesOrderItemList = OrderHistory.Body.SalceOrderList[0].SalesOrderItemDetil.ToList();
            TWNewEgg.Models.ViewModels.MyAccount.OrderHistory OrderHistorydetil = new TWNewEgg.Models.ViewModels.MyAccount.OrderHistory();
            SalesOrderdetil = OrderHistory.Body.SalceOrderList.Where(x => x.Code == SOCode).FirstOrDefault();

            SalesOrder.Add(SalesOrderdetil);
            OrderHistorydetil.SalceOrderList = SalesOrder;
            string resultString = "";
            using (StringWriter sw = new StringWriter())
            {
                ViewData.Model = OrderHistorydetil;
                ViewBag.SOCode = SOCode;
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Partial_RecentOrder");
                ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                resultString = sw.GetStringBuilder().ToString();
            }
            if (Request.IsAjaxRequest())
            {
                return Json(Regex.Replace(resultString, @"[\r\n]+\s{0,}[\r\n]+", " "), JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("Login", "Account", new { returnUrl = "/MyAccount/Order" });
        }
        public ActionResult GetRefundDetail(string SOCode)
        {
            int accID = NEUser.ID;

            TWNewEgg.ECWeb.Services.OldCart.ReturnService.ReturnService ReturnService = new Services.OldCart.ReturnService.ReturnService();
            TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.ReturnPost> ActionResponse = new TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.ReturnPost>();


            ActionResponse = ReturnService.Refund(SOCode, accID);
            if (ActionResponse.IsSuccess == true)
            {

                string resultString = "";
                TWNewEgg.Models.ViewModels.MyAccount.ReturnPost retspost = new TWNewEgg.Models.ViewModels.MyAccount.ReturnPost();
                retspost = ActionResponse.Body;
                using (StringWriter sw = new StringWriter())
                {
                    ViewData.Model = retspost;
                    ViewBag.SOCode = SOCode;
                    ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Partial_RefundOrder");
                    ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                    viewResult.View.Render(viewContext, sw);
                    resultString = sw.GetStringBuilder().ToString();
                }
                if (Request.IsAjaxRequest())
                {
                    return Json(Regex.Replace(resultString, @"[\r\n]+\s{0,}[\r\n]+", " "), JsonRequestBehavior.AllowGet);
                }
                return RedirectToAction("Login", "Account", new { returnUrl = "/MyAccount/GetReturnDetail" });
            }
            else
            {
                return RedirectToAction("Logout", "MyAccount", new { returnUrl = "/Home" });

            }



        }

        [HttpPost]
        public ActionResult GetOrderInvoice(string SOCode)
        {
            int accID = NEUser.ID;

            TWNewEgg.ECWeb.Services.OldCart.ReturnService.ReturnService ReturnService = new Services.OldCart.ReturnService.ReturnService();
            TWNewEgg.Website.ECWeb.Service.OrderDetail OrderDetail = new Website.ECWeb.Service.OrderDetail();
            TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.OrderHistory> OrderHistory = new TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.OrderHistory>();

            OrderHistory = OrderDetail.SalesOrderItem(accID, SOCode);

            if (OrderHistory.IsSuccess == true)
            {

                string resultString = "";
                TWNewEgg.Models.ViewModels.MyAccount.OrderHistory retspost = new TWNewEgg.Models.ViewModels.MyAccount.OrderHistory();
                retspost.SalceOrderList = OrderHistory.Body.SalceOrderList.Where(x => x.Code == SOCode).ToList();

                using (StringWriter sw = new StringWriter())
                {
                    ViewData.Model = retspost;
                    ViewBag.SOCode = SOCode;
                    ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Partial_OrderInvoice");
                    ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                    viewResult.View.Render(viewContext, sw);
                    resultString = sw.GetStringBuilder().ToString();
                }
                if (Request.IsAjaxRequest())
                {
                    return Json(Regex.Replace(resultString, @"[\r\n]+\s{0,}[\r\n]+", " "), JsonRequestBehavior.AllowGet);
                }
                return RedirectToAction("Login", "Account", new { returnUrl = "/MyAccount/GetReturnDetail" });
            }
            else
            {
                return RedirectToAction("Logout", "MyAccount", new { returnUrl = "/Home" });

            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        ///修改Coupon顯示的可使用類別
        /// </summary>
        /// <param name="CouponNumber"></param>
        /// <returns></returns>
        private void getCouponCategoriesDesc(ref List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listCoupon)
        {
            TWNewEgg.DB.TWSqlDBContext oDb = null;
            Redeem.Service.CouponService.CouponServiceRepository oCouponService = null;
            List<int> listEventId = null;
            List<TWNewEgg.DB.TWSQLDB.Models.Event> listEvent = null;
            TWNewEgg.DB.TWSQLDB.Models.Event oEvent = null;

            if (listCoupon == null)
                return;

            listEventId = listCoupon.Select(x => x.eventid).Distinct().ToList<int>();

            oDb = new DB.TWSqlDBContext();
            listEvent = oDb.Event.Where(x => listEventId.Contains(x.id)).ToList<TWNewEgg.DB.TWSQLDB.Models.Event>();
            oDb = null;

            if (listEvent != null)
            {
                foreach (TWNewEgg.Models.ViewModels.Redeem.Coupon oSubCoupon in listCoupon)
                {
                    oEvent = listEvent.FirstOrDefault(x => x.id == oSubCoupon.eventid);
                    if (oEvent == null)
                        continue;

                    if (oEvent.limitdescription.Length > 0)
                        oSubCoupon.categories = oEvent.limitdescription;
                    else if (oSubCoupon.categories.Equals(";0;"))
                        oSubCoupon.categories = "全館";
                    else
                        oSubCoupon.categories = "部份商品";

                    if (oSubCoupon.title == null || oSubCoupon.title.Length <= 0)
                    {
                        oSubCoupon.title = oEvent.name;
                    }
                }//end foreach
            }

            //釋放記憶體
            if (oDb != null)
            {
                oDb.Dispose();
            }

            listEvent = null;
            listEventId = null;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPassword(string q)
        {
            if (NEUser.IsAuthticated)
            {
                return RedirectToAction("Index", "Logout");
            }
            _accountService = new AccountService(AccountAuthFactory.AuthType.ecweb.ToString());
            Dictionary<string, string> obj = _accountService.DecodeResetPasswordLink(q);
            string email = obj[_accountService.body];
            string datetimeTicks = obj[_accountService.head];
            long ticks;
            if (!long.TryParse(datetimeTicks, out ticks))
            {
                return RedirectToAction("Index", "Home");
            }
            DateTime oldTime = new DateTime(ticks);
            oldTime = oldTime.AddMinutes(30);
            if (DateTime.Compare(oldTime, TWNewEgg.Framework.ServiceApi.Configuration.ConfigurationManager.GetTaiwanTime()) < 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(obj);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ResetPassword(string email, string trick, string password, string confirmPWD)
        {
            ECWebResponse response = new ECWebResponse();
            response.Message = "系統錯誤!";
            response.Status = (int)ECWebResponse.StatusCode.系統錯誤;
            if (password != confirmPWD)
            {
                response.Message = "密碼不同!";
                response.Status = (int)ECWebResponse.StatusCode.系統錯誤;
                return Json(response, JsonRequestBehavior.DenyGet);    
            }
            _accountService = new AccountService(AccountAuthFactory.AuthType.ecweb.ToString());
            if (!_accountService.CorrectEmailFormat(email))
            {
                response.Message = "格式錯誤!";
                response.Status = (int)ECWebResponse.StatusCode.系統錯誤;
                return Json(response, JsonRequestBehavior.DenyGet);    
            }
            long ticks;
            if (!long.TryParse(trick, out ticks))
            {
                response.Message = "資料錯誤!";
                response.Status = (int)ECWebResponse.StatusCode.系統錯誤;
                return Json(response, JsonRequestBehavior.DenyGet);    
            }
            DateTime oldTime = new DateTime(ticks);
            oldTime = oldTime.AddMinutes(30);
            if (DateTime.Compare(oldTime, TWNewEgg.Framework.ServiceApi.Configuration.ConfigurationManager.GetTaiwanTime()) < 0)
            {
                response.Message = "超過30分鐘!";
                response.Status = (int)ECWebResponse.StatusCode.系統錯誤;
                return Json(response, JsonRequestBehavior.DenyGet);    
            }

            var memberInfo = Processor.Request<AccountInfoVM, AccountInfoDM>("AccountService", "EditPersonInfo", email);
            if (!string.IsNullOrEmpty(memberInfo.error))
            {
                response.Message = "系統錯誤!";
                response.Status = (int)ECWebResponse.StatusCode.系統錯誤;
                return Json(response, JsonRequestBehavior.DenyGet);    
            }
            if (memberInfo.results.AVM == null)
            {
                response.Message = "帳號錯誤!";
                response.Status = (int)ECWebResponse.StatusCode.系統錯誤;
                return Json(response, JsonRequestBehavior.DenyGet);    
            }
            var UpdatePassword = Processor.Request<AccountInfoVM, AccountInfoDM>("AccountService", "UpdatePassword", email, _aesCode.Enprypt(password), "UpdatePasswordPost");
            if (!string.IsNullOrEmpty(UpdatePassword.error))
            {
                response.Message = "帳號密碼錯誤!";
                response.Status = (int)ECWebResponse.StatusCode.系統錯誤;
                return Json(response, JsonRequestBehavior.DenyGet);    
            }
            else
            {
                response.Message = "密碼更新成功，下次登入請使用新密碼！";
                response.Status = (int)ECWebResponse.StatusCode.成功;
            }
            return Json(response, JsonRequestBehavior.DenyGet);    
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgetPassword(int? ecode)
        {
            if (NEUser.IsAuthticated)
            {
                return RedirectToAction("Index", "Logout", new { returnUrl = HttpUtility.UrlEncode("/MyAccount/ForgetPassword") });
            }
            string errorMessage = string.Empty;
            switch (ecode)
            {
                case 1:
                    errorMessage = "請輸入正確的帳號格式";
                    break;
                case 2:
                    errorMessage = "請輸入正確的帳號";
                    break;
                case 99:
                    errorMessage = "請至信箱收信並點取連結修改新密碼";
                    break;
                default:
                    break;
            }
            ViewBag.errorMessage = errorMessage;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgetPassword(string email)
        {
            _accountService = new AccountService(AccountAuthFactory.AuthType.ecweb.ToString());
            if (!_accountService.CorrectEmailFormat(email))
            {
                return RedirectToAction("ForgetPassword", new { ecode = 1 });
            }

            var memberInfo = Processor.Request<AccountInfoVM, AccountInfoDM>("AccountService", "EditPersonInfo", email);
            if (string.IsNullOrEmpty(memberInfo.error))
            {
                if (memberInfo.results == null)
                {
                    return RedirectToAction("ForgetPassword", new { ecode = 2 });
                }
                if (memberInfo.results.AVM == null)
                {
                    return RedirectToAction("ForgetPassword", new { ecode = 2 });
                }
            }
            DateTime datetimeNow = TWNewEgg.Framework.ServiceApi.Configuration.ConfigurationManager.GetTaiwanTime();
            string head = "<html><body>請於30分鐘內點選下列連結進行密碼修改";
            string body = string.Format("<a href=\"{0}{1}{2}{3}{4}\">{0}{1}{2}{3}{4}</a>", "https://", System.Configuration.ConfigurationManager.AppSettings["ECWebHttpsDomain"], (System.Configuration.ConfigurationManager.AppSettings["ECWebHttpsPort"] == "443") ? "" : ":" + System.Configuration.ConfigurationManager.AppSettings["ECWebHttpsPort"], "/MyAccount/ResetPassword?q=", HttpUtility.UrlEncode(_accountService.GenerateResetPasswordLink(email, datetimeNow.Ticks.ToString())));
            string foot = "新蛋全球生活網</body></html>";
            SendReSetPassMail(email, "新蛋全球生活網忘記密碼修改信", string.Format("{0}<br /><br />{1}<br /><br />{2}", head, body, foot));
            return RedirectToAction("ForgetPassword", new { ecode = 99 });
        }

        private static void SendReSetPassMail(string reciver, string subject, string bodyMessage)
        {
            ServiceApiCacheAppSettings serviceCacheAppSetting = TWNewEgg.Framework.Cache.CacheConfiguration.Instance.GetFromCache<ServiceApiCacheAppSettings>(ConfigurationManager.SERVICEAPICACHEAPPSETTING, null, false);
            TWNewEgg.Framework.Common.SMTP.Models.SendMessage settingMailMessage = new TWNewEgg.Framework.Common.SMTP.Models.SendMessage();
            settingMailMessage.attachmentsPath = new List<string>();
            settingMailMessage.bccsAddess = new Dictionary<string, string>();
            settingMailMessage.ccsAddress = new Dictionary<string, string>();
            //string fromAddress = serviceCacheAppSetting.serviceApiCacheAppSettings.Where(x => x.Key == ConfigurationManager.CACHE_DEBUGSENDMAILADDRESS).First().Value;
            settingMailMessage.fromAddress = serviceCacheAppSetting.serviceApiCacheAppSettings.Where(x => x.Key == ConfigurationManager.CACHE_DEBUGSENDMAILADDRESS).First().Value;
            settingMailMessage.fromName = "新蛋全球生活網";
            settingMailMessage.isBodyHtml = true;
            settingMailMessage.mailBody = bodyMessage;
            settingMailMessage.mailEncoding = System.Text.Encoding.UTF8;
            settingMailMessage.mailPriority = TWNewEgg.Framework.Common.SMTP.Models.SendPriority.High;
            settingMailMessage.mailSubject = subject;
            //List<string> recipientsAddress = serviceCacheAppSetting.serviceApiCacheAppSettings.Where(x => x.Key == ConfigurationManager.CACHE_DEBUGRECEIVEMAILADDRESS).First().Value.Split(',').ToList();
            settingMailMessage.recipientsAddress = SettingMailAddressToDict(new List<string>() { reciver });
            settingMailMessage.replysAddress = new Dictionary<string, string>();
            TWNewEgg.Framework.Common.SMTP.Models.SmtpMessage settingMailSmtp = new TWNewEgg.Framework.Common.SMTP.Models.SmtpMessage();
            settingMailSmtp.isCredentials = false;
            settingMailSmtp.isSsl = false;
            settingMailSmtp.userName = string.Empty;
            settingMailSmtp.userPass = string.Empty;
            //string smtpServerIP = serviceCacheAppSetting.serviceApiCacheAppSettings.Where(x => x.Key == ConfigurationManager.CACHE_SMTPSERVERIP).First().Value;
            settingMailSmtp.smtpServerIP = serviceCacheAppSetting.serviceApiCacheAppSettings.Where(x => x.Key == ConfigurationManager.CACHE_SMTPSERVERIP).First().Value;
            //string smtpServerPort = serviceCacheAppSetting.serviceApiCacheAppSettings.Where(x => x.Key == ConfigurationManager.CACHE_SMTPSERVERPORT).First().Value;
            settingMailSmtp.smtpServerPort = TWNewEgg.Framework.Common.ConverterUtility.ToInt32(serviceCacheAppSetting.serviceApiCacheAppSettings.Where(x => x.Key == ConfigurationManager.CACHE_SMTPSERVERPORT).First().Value);
            //MailMessage errorMail = Common.SMTP.Service.SendMail.SettingEmailMessage(settingMailMessage);
            TWNewEgg.Framework.Common.SMTP.Service.SendMail.SendNow(settingMailSmtp, TWNewEgg.Framework.Common.SMTP.Service.SendMail.SettingEmailMessage(settingMailMessage));
        }
        private static Dictionary<string, string> SettingMailAddressToDict(List<string> mailAddress)
        {
            Dictionary<string, string> mailAddressDict = new Dictionary<string, string>();
            for (int i = 0; i < mailAddress.Count; i++)
            {
                if (!mailAddressDict.ContainsKey(mailAddress[i]))
                {
                    mailAddressDict.Add(mailAddress[i], "");
                }

            }
            return mailAddressDict;
        }

        #region 查看問題單的歷史紀錄
        /// <summary>
        /// 查看問題單的歷史紀錄
        /// </summary>
        /// <param name="ProbelmId"></param>
        /// <returns></returns>
        public ActionResult QuestionRecordSelect(string ProbelmId = "")
        {
            #region 判斷是否有傳入正確的 ProbelmId
            //沒有傳進 ProbelmId 則發生錯誤,導回問題單的首頁
            if (string.IsNullOrEmpty(ProbelmId) == true)
            {
                return RedirectToAction("QuestionRecord", "MyAccount");
            }
            #endregion
            #region 給予預設值
            int accID = NEUser.ID;
            string Email = NEUser.Email;
            string Salceorder = "";
            int Mouth = 24;//固定值, 為了讀取所有的問題單
            #endregion
            #region 呼叫 service 並判斷是否發生錯誤
            //呼叫 service
            var Info = Processor.Request<AnswerInfo, AnswerInfo>("AnswerService", "GetPrblmRecodeSelect", accID, Mouth, Email, Salceorder, ProbelmId);
            //service 有錯誤或 service 邏輯運算有錯誤
            if (Info.error != null)
            {
                logger.Error("問題單呼叫 Service 錯誤: var Info = Processor.Request<AnswerInfo, AnswerInfo>(\"AnswerService\", \"GetPrblmRecodeSelect\", accID, Mouth, Email, Salceorder, ProbelmId);");
                //發生錯誤 導回問題單的首頁
                return RedirectToAction("QuestionRecord", "MyAccount");
            }
            //沒有對應的問題單則導回問題單的首頁
            if (Info.results == null)
            {
                logger.Error("問題單號: " + ProbelmId + ", 從 Service 回來沒有資料");
                return RedirectToAction("QuestionRecord", "MyAccount");
            }
            #endregion
            #region 防止利用回傳資料做列舉轉換時發生錯誤, 或者是讀取資料時發生錯誤
            //用來記錄是否發生 Exception
            bool isNoException = true;
            try
            {
                //Enum 轉換, 轉換錯誤則進去 Exception
                ViewBag.ProblemType = Enum.GetName(typeof(TWNewEgg.Models.ViewModels.Answer.status), Info.results.Probelm.IntClass).ToString();
                //讀取商品名稱
                Info.results.Probelm.Name = Info.results.SalesOrderItem[0].Name == null ? string.Empty : Info.results.SalesOrderItem[0].Name;
                isNoException = true;
            }
            catch (Exception error)
            {
                logger.Error(this.ExceptionMsg(error));
                isNoException = false;
                ViewBag.ProblemType = string.Empty;
            }
            //發生 Exception, 導回問題單的首頁
            if (isNoException == false)
            {
                return RedirectToAction("QuestionRecord", "MyAccount");
            }
            #endregion
            #region 根據問題單種類導向對應的畫面
            switch (Info.results.Probelm.IntClass)
            {
                case (int)TWNewEgg.Models.ViewModels.Answer.status.訂購是否成功:
                case (int)TWNewEgg.Models.ViewModels.Answer.status.更改訂單資料:
                case (int)TWNewEgg.Models.ViewModels.Answer.status.查詢出貨進度:
                case (int)TWNewEgg.Models.ViewModels.Answer.status.取消訂單:
                case (int)TWNewEgg.Models.ViewModels.Answer.status.報關文件相關問題:
                case (int)TWNewEgg.Models.ViewModels.Answer.status.七天鑑賞期退貨:
                case (int)TWNewEgg.Models.ViewModels.Answer.status.新品瑕疵換貨:
                case (int)TWNewEgg.Models.ViewModels.Answer.status.商品缺件或不符:
                case (int)TWNewEgg.Models.ViewModels.Answer.status.商品維修及保固:
                case (int)TWNewEgg.Models.ViewModels.Answer.status.付款是否成功:
                case (int)TWNewEgg.Models.ViewModels.Answer.status.退款問題:
                case (int)TWNewEgg.Models.ViewModels.Answer.status.發票問題:
                case (int)TWNewEgg.Models.ViewModels.Answer.status.折價券問題:
                    {
                        var dicAboutInfoToShow = this.ProductaboutOrder_Return_invoice(Info.results);
                        if (dicAboutInfoToShow == null)
                        {
                            return RedirectToAction("QuestionRecord", "MyAccount");
                        }
                        ViewBag.dicAboutInfoToShow = dicAboutInfoToShow;
                        return View("QuestionRecordProduct", Info.results);
                        break;
                    }
                case (int)TWNewEgg.Models.ViewModels.Answer.status.商品規格:
                case (int)TWNewEgg.Models.ViewModels.Answer.status.訂購及付款方式:
                case (int)TWNewEgg.Models.ViewModels.Answer.status.行銷活動諮詢:
                    {
                        return View("QuestionRecordItem", Info.results);
                        break;
                    }
                case (int)TWNewEgg.Models.ViewModels.Answer.status.詐騙相關問題:
                case (int)TWNewEgg.Models.ViewModels.Answer.status.其他問題:
                case (int)TWNewEgg.Models.ViewModels.Answer.status.系統網頁問題:
                    {
                        return View("QuestionRecordOther", Info.results);
                        break;
                    }
                default:
                    {
                        //都沒有對應的 Enum 所以導回問題單的首頁
                        return RedirectToAction("QuestionRecord", "MyAccount");
                    }
            }
            #endregion
            return RedirectToAction("QuestionRecord", "MyAccount");

            #region 分類問題單種類, 根據不同種類導向不同的 View
            //分類問題單種類, 根據不同種類導向不同的 View
            //if (Info.results.Probelm.IntClass == (int)TWNewEgg.Models.ViewModels.Answer.status.訂購是否成功 ||
            //    Info.results.Probelm.IntClass == (int)TWNewEgg.Models.ViewModels.Answer.status.更改訂單資料 ||
            //    Info.results.Probelm.IntClass == (int)TWNewEgg.Models.ViewModels.Answer.status.查詢出貨進度 ||
            //    Info.results.Probelm.IntClass == (int)TWNewEgg.Models.ViewModels.Answer.status.取消訂單 ||
            //    Info.results.Probelm.IntClass == (int)TWNewEgg.Models.ViewModels.Answer.status.報關文件相關問題 ||
            //    Info.results.Probelm.IntClass == (int)TWNewEgg.Models.ViewModels.Answer.status.七天鑑賞期退貨 ||
            //    Info.results.Probelm.IntClass == (int)TWNewEgg.Models.ViewModels.Answer.status.新品瑕疵換貨 ||
            //    Info.results.Probelm.IntClass == (int)TWNewEgg.Models.ViewModels.Answer.status.商品缺件或不符 ||
            //    Info.results.Probelm.IntClass == (int)TWNewEgg.Models.ViewModels.Answer.status.商品維修及保固 ||
            //    Info.results.Probelm.IntClass == (int)TWNewEgg.Models.ViewModels.Answer.status.付款是否成功 ||
            //    Info.results.Probelm.IntClass == (int)TWNewEgg.Models.ViewModels.Answer.status.退款問題 ||
            //    Info.results.Probelm.IntClass == (int)TWNewEgg.Models.ViewModels.Answer.status.發票問題 ||
            //    Info.results.Probelm.IntClass == (int)TWNewEgg.Models.ViewModels.Answer.status.折價券問題)
            //{
            //    //計算總價
            //    decimal? total = 0;
            //    try
            //    {
            //        foreach (var computMoneyItem in Info.results.SalesOrderItem)
            //        {
            //            decimal? moneyTemp = computMoneyItem.Price + computMoneyItem.ShippingExpense + computMoneyItem.ServiceExpense + computMoneyItem.InstallmentFee - computMoneyItem.Pricecoupon - computMoneyItem.ApportionedAmount;
            //            total = total + moneyTemp;
            //        }
            //        //total 是空則直接給空, 預防顯示在前端時發生錯誤
            //        if (total == null)
            //        {
            //            ViewBag.totalPrice = string.Empty;
            //        }
            //        else
            //        {
            //            //去除小數點(EX: 100.00 -> 100)
            //            ViewBag.totalPrice = (Math.Round(total.Value, 0)).ToString();
            //        }
            //    }
            //    catch (Exception error)
            //    {
            //        logger.Error(ExceptionMsg(error));
            //        ViewBag.totalPrice = string.Empty;
            //    }
            //    //判斷是否有訂購日期, 沒有就給空值
            //    ViewBag.saleOrderDate = Info.results.SalesOrderItem[0].Date.HasValue == false ? string.Empty : string.Format("{0: yyyy/MM/dd}", Info.results.SalesOrderItem[0].Date.Value);
            //    //判斷有沒有訂單編號, 沒有的話表示也不會有發票號碼
            //    if (string.IsNullOrEmpty(Info.results.Probelm.BlngCode) == true)
            //    {
            //        ViewBag.invoiceData = null;
            //    }
            //    else
            //    {
            //        //讀取並判斷是否有發票號碼
            //        var invoiceData = this.orderHistory(accID, Info.results.Probelm.BlngCode);
            //        ViewBag.invoiceData = invoiceData.IsSuccess == false ? null : invoiceData.Body;
            //    }
            //    return View("QuestionRecordProduct", Info.results);
            //}
            //else if (Info.results.Probelm.IntClass == (int)TWNewEgg.Models.ViewModels.Answer.status.商品規格 ||
            //         Info.results.Probelm.IntClass == (int)TWNewEgg.Models.ViewModels.Answer.status.訂購及付款方式 ||
            //         Info.results.Probelm.IntClass == (int)TWNewEgg.Models.ViewModels.Answer.status.行銷活動諮詢)
            //{
            //    return View("QuestionRecordItem", Info.results);
            //}
            //else if (Info.results.Probelm.IntClass == (int)TWNewEgg.Models.ViewModels.Answer.status.詐騙相關問題 ||
            //         Info.results.Probelm.IntClass == (int)TWNewEgg.Models.ViewModels.Answer.status.其他問題 ||
            //         Info.results.Probelm.IntClass == (int)TWNewEgg.Models.ViewModels.Answer.status.系統網頁問題)
            //{
            //    return View("QuestionRecordOther", Info.results);
            //}
            //else
            //{
            //    //都沒有對應的 Enum 所以導回問題單的首頁
            //    return RedirectToAction("QuestionRecord", "MyAccount");
            //}
            #endregion
        }
        #endregion
        #region 讀取相關的 cart 資料 用來檢查有無發票可以顯示
        /// <summary>
        /// 讀取相關的 cart 資料 用來檢查有無發票可以顯示
        /// </summary>
        /// <param name="accID"></param>
        /// <param name="soCode"></param>
        /// <returns></returns>
        public TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.OrderHistory> orderHistory(int accID, string soCode)
        {
            TWNewEgg.Website.ECWeb.Service.OrderDetail orderDetail = new Website.ECWeb.Service.OrderDetail();
            TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.OrderHistory> result = new TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.OrderHistory>();
            try
            {
                result = orderDetail.SalesOrderItem(accID, soCode);
                //發票的規則
                if (string.IsNullOrEmpty(result.Body.SalceOrderList[0].InvoiceNo) || string.IsNullOrEmpty(result.Body.SalceOrderList[0].SalesOrderItemDetil[0].DelivNO) && result.Body.SalceOrderList[0].Delivtype != (int)TWNewEgg.DB.TWSQLDB.Models.Item.tradestatus.三角)
                {
                    result.IsSuccess = false;
                }
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                //呼叫組合錯誤訊息的 method
                logger.Error(this.ExceptionMsg(error));
            }
            return result;
        }
        #endregion
        #region 收集總價, 訂購日期, 有無發票號碼
        /// <summary>
        /// 收集總價, 訂購日期, 有無發票號碼
        /// </summary>
        /// <param name="answer"></param>
        /// <returns></returns>
        public Dictionary<string, string> ProductaboutOrder_Return_invoice(AnswerInfo answer)
        {
            Dictionary<string, string> returnDic = new Dictionary<string, string>();
            int accID = NEUser.ID;
            //計算總價
            decimal? total = 0;
            try
            {
                foreach (var computMoneyItem in answer.SalesOrderItem)
                {
                    decimal? moneyTemp = computMoneyItem.Price + computMoneyItem.ShippingExpense + computMoneyItem.ServiceExpense + computMoneyItem.InstallmentFee - computMoneyItem.Pricecoupon - computMoneyItem.ApportionedAmount;
                    total = total + moneyTemp;
                }
                //total 是空則直接給空, 預防顯示在前端時發生錯誤
                if (total == null)
                {
                    returnDic.Add("totalPrice", string.Empty);
                }
                else
                {
                    //去除小數點(EX: 100.00 -> 100)
                    returnDic.Add("totalPrice", (Math.Round(total.Value, 0)).ToString());
                }
                //判斷是否有訂購日期, 沒有就給空值
                returnDic.Add("saleOrderDate", answer.SalesOrderItem[0].Date.HasValue == false ? string.Empty : string.Format("{0: yyyy/MM/dd}", answer.SalesOrderItem[0].Date.Value));
            }
            catch (Exception error)
            {
                logger.Error(ExceptionMsg(error));
                returnDic.Add("totalPrice", string.Empty);
                returnDic.Add("saleOrderDate", string.Empty);
            }
            
            //判斷有沒有訂單編號, 沒有的話表示也不會有發票號碼
            //if (string.IsNullOrEmpty(answer.Probelm.BlngCode) == true)
            //{
            //    returnDic.Add("invoiceData", "False");
            //}
            //else
            //{
            //    //讀取並判斷是否有發票號碼
            //    var invoiceData = this.orderHistory(accID, answer.Probelm.BlngCode);
            //    returnDic.Add("invoiceData", invoiceData.IsSuccess == false ? "False" : "True");
            //}
            return returnDic;
        }
        #endregion
        #region 組合錯誤訊息
        /// <summary>
        /// 組合錯誤訊息
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public string ExceptionMsg(Exception error)
        {
            string innerMsg = string.Empty;
            innerMsg = error.InnerException == null ? "No innerException Msg" : error.InnerException.Message;
            return "[Msg]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[innerMsg]: " + innerMsg;
        }
        #endregion




    }
}
