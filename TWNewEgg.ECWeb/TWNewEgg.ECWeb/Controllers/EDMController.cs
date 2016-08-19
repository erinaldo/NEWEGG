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
    [AllowNonSecures]
    [AllowAnonymous]
    public class EDMController : Controller
    {
        //
        // GET: /EDM/

        public ActionResult Index()
        {
            EDMBookVM edmVM = Processor.Request<EDMBookVM, EDMBookDM>("EDMService", "GetLatestEDM").results;
            if (edmVM == null)
            {
                edmVM.EDMName = "No EDM";
                edmVM.HtmlContext = "";
                edmVM.ID = 0;
            }
            return View(edmVM);
        }

    }
}
