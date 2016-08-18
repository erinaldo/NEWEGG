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
    public class BrandController : Controller
    {
        //
        // GET: /Brand/

        public ActionResult Index()
        {
            return View();
        }

    }
}
