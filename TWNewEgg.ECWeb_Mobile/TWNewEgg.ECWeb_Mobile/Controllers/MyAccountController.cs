using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.AccountEnprypt.Interface;
using TWNewEgg.ECWeb.PrivilegeFilters.Core;
using TWNewEgg.ECWeb_Mobile.Auth;
using TWNewEgg.ECWeb_Mobile.Services.Account;
using TWNewEgg.Framework.Autofac;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Framework.ServiceApi.Configuration;
using TWNewEgg.Models.DomainModels.Account;
using TWNewEgg.Models.ViewModels.Account;
using TWNewEgg.Models.ViewModels.Login;
using TWNewEgg.Models.ViewModels.Message;
using TWNewEgg.Models.ViewModels.Register;
using TWNewEgg.Models.ViewModels.Track;

namespace TWNewEgg.ECWeb_Mobile.Controllers
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

        public ActionResult Index()
        {
            return View();
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
                            //SendActivityGift(activity, registerResult.results.AVM);
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
            else
            {
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


        
    }
}
