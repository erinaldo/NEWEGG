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
    public class InfoController : Controller
    {
        //
        // GET: /Info/

        public ActionResult Corporate()
        {
            return View();
        }
        public ActionResult AboutUs()
        {
            return View();
        }

    }
}
