using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.ECWeb.Services.Account;

namespace TWNewEgg.ECWeb.Controllers
{
    [AllowAnonymous]
    public class LogoutController : Controller
    {
        AccountService _accountService;
        //
        // GET: /Logout/
        public ActionResult Index(string returnUrl)
        {
            _accountService = new AccountService(AccountAuthFactory.AuthType.ecweb.ToString());
            var isLogout = _accountService.Logout();

            if (!string.IsNullOrEmpty(returnUrl) && returnUrl.IndexOf("%2f") != -1)
            {
                returnUrl = returnUrl.Replace("%2f", "/");
            }

            return RedirectToLocal(returnUrl);
            //return RedirectToAction("Index", "Home");
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
