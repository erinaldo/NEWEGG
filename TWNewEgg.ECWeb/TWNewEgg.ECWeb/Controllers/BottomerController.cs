using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.ECWeb.PrivilegeFilters;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.ViewModels;

namespace TWNewEgg.ECWeb.Controllers
{
    [AllowNonSecures]
    [AllowAnonymous]
    public class BottomerController : Controller
    {
        // GET: /Bottomer/
        public ActionResult Index(int advId)
        {
            List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay> listAdvEventDisplay = null;

            //根據AdvTypeCode取得所有AdvType的列表, 並以Country欄位作為排序
            listAdvEventDisplay = Processor.Request<List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay>, List<TWNewEgg.Models.DomainModels.Advertising.AdvEventDisplay>>("Service.AdvEventItemService", "GetActiveAdvEventDisplayListByAdvEventTypeId", advId).results;
            return PartialView("_Bottomer", listAdvEventDisplay);
        }
    }
}
