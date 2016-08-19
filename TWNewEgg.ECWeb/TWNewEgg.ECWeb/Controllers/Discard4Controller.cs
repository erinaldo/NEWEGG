using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.ECWeb.PrivilegeFilters;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DomainModels.EDM;
using TWNewEgg.Models.ViewModels.EDM;

namespace TWNewEgg.ECWeb.Controllers
{
    public class Discard4Controller : Controller
    {
        public ActionResult Index()
        {          
            return View();
        }
    }
}
