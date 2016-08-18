using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.API.View.Controllers
{
    public class SharedController : Controller
    {
        //
        // GET: /Shared/

        public ActionResult MainMenu()
        {
            return View();
        }

    }
}
