using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.API.View.Controllers
{
    public class ManageOrderController : Controller
    {
        //
        // GET: /ManageOrder/

        public ActionResult OrderList()
        {
            return View();
        }

        //
        // GET: /ManageOrder/

        public ActionResult ReturnList()
        {
            return View();
        }
    }
}
