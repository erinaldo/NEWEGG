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
    public class ActController : Controller
    {
        //
        // GET: /Act/

        public ActionResult Deal(string name)
        {
            if (name == "ticket888_0801")
            {
                return RedirectToAction("show", "Activity");

            }
            if (name == "wonderfulworld_0801")
            {
                return RedirectToAction("show", "Activity");

            }
            if (name == "sharegift_0807")
            {
                return RedirectToAction("ShareGift_0807", "VotingActivity");
            }
            int number = 0;
            return View(name);
        }

    }
}
