using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.ECWeb.Auth;
using TWNewEgg.ECWeb.PrivilegeFilters.Api;

namespace TWNewEgg.ECWeb.Controllers
{
    [AllowNonSecures]
    [AllowAnonymous]
    public class DealsController : Controller
    {
        //
        // GET: /Deals/
        [AllowNonSecures]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Lottery()
        {
            return View();
        }

        [AllowNonSecures]
        [AllowAnonymous]
        public ActionResult fortunecookie_20151106()
        {
            return View();
        }

    }
}
