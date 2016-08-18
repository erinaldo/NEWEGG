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
    public class ErrorController : Controller
    {

        public ActionResult Error()
        {
            return View("404");
        }
    }
}
