using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.Models.ViewModels.Login;
using TWNewEgg.ECWeb.Services.Account;
using TWNewEgg.Framework.ServiceApi.Configuration;
using TWNewEgg.ECWeb.PrivilegeFilters.Core;

namespace TWNewEgg.ECWeb.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        AccountService _accountService;
        //
        // GET: /Login/

        public ActionResult Index(string returnUrl)
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

        [HttpPost]
        public ActionResult Index(Login model, string returnUrl)
        {
            _accountService = new AccountService(AccountAuthFactory.AuthType.ecweb.ToString());
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
