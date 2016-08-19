using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.ECWeb_Mobile.Services.Account;

namespace TWNewEgg.ECWeb_Mobile.Controllers
{
    [AllowAnonymous]
    public class LogoutController : Controller
    {
        AccountService _accountService;
        //
        // GET: /Logout/
        public ActionResult Index(string returnUrl)
        {
            Response.Buffer = true;
            Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1);
            Response.Expires = 0;
            Response.CacheControl = "no-cache";
            _accountService = new AccountService(AccountAuthFactory.AuthType.ecweb.ToString());
            var isLogout = _accountService.Logout();
            return RedirectToLocal(returnUrl);
            //return RedirectToAction("Index", "Home");
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            returnUrl = HttpUtility.UrlDecode(returnUrl);
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
