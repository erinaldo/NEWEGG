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
    public class WebsiteMapController : Controller
    {
        //
        // GET: /WebsiteMap/

        public ActionResult Index()
        {
            return View();
        }

    }
}
