using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.ECWeb.PrivilegeFilters;

namespace TWNewEgg.ECWeb.Controllers
{
    [AllowNonSecures]
    [AllowAnonymous]
    public class PoliciesController : Controller
    {
        //
        // GET: /Policies/

        public ActionResult Member()
        {
            return View();
        }
        public ActionResult Privacy()
        {
            return View();
        }
        public ActionResult Disclaimer()
        {
            return View();
        }
        public ActionResult Return()
        {
            return View();
        }
        public ActionResult Refund()
        {
            return View();
        }

    }
}
