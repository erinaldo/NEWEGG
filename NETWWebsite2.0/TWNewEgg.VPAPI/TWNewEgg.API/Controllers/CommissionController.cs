using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.API.Controllers
{
    public class CommissionController : Controller
    {
        //
        // GET: /Commission/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetIndustryName()
        {
            TWNewEgg.API.Service.CommissionService cs = new TWNewEgg.API.Service.CommissionService();
            Models.ActionResponse<List<TWNewEgg.DB.TWSQLDB.Models.Category>> category = cs.GetCategory();
            return Json(category, JsonRequestBehavior.AllowGet);
        }

    }
}
