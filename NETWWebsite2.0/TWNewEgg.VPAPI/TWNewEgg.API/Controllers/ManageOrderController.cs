using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace TWNewEgg.API.Controllers
{
    public class ManageOrderController : Controller
    {
        //
        // GET: /ManageOrder/

        public JsonResult ReturnList()
        {
            TWNewEgg.API.Service.ReturnList returnlist = new Service.ReturnList();
            var returnlistResult = returnlist.Getretgood().Body.ToList();

            return Json(returnlistResult, JsonRequestBehavior.AllowGet);
        }
    }
}
