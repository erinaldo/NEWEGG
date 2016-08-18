using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.ECWeb.PrivilegeFilters;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DomainModels.MobileStore;

namespace TWNewEgg.ECWeb_Mobile.Controllers
{
    [AllowNonSecures]
    [AllowAnonymous]
    public class CategoryController : Controller
    {
        public ActionResult Index(int categoryID)
        {
            if (categoryID <= 0)
            {
                return RedirectToAction("index", "home");
            }
            return View();
        }
    }
}
