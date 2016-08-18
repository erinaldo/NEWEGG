using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.API.View.Controllers
{
    public class BusinessReportController : Controller
    {
        //
        // GET: /BusinessReport/

        public ActionResult PaymentReport()
        {
            return View();
        }

    }
}
