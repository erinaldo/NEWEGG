using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.Website.ECWeb.Service;

namespace TWNewEgg.ECWeb.Controllers
{
    public class NEGGTestController : Controller
    {
        //
        // GET: /NEGGTest/
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        public ActionResult CheckUSItemPrice(int id)
        {
            TWNewEgg.Website.ECWeb.Service.ItemService sv = new Website.ECWeb.Service.ItemService();
            bool res1 = sv.CheckItemPriceNeweggUSA(id);

            return Content(res1.ToString());
        }
        public ActionResult CheckUSItemStatus(int id)
        {
            TWNewEgg.Website.ECWeb.Service.ItemService sv = new Website.ECWeb.Service.ItemService();
            bool res2 = sv.CheckItemStatusNeweggUSA(id);

            return Content(res2.ToString());
        }
        public ActionResult CheckUSItemStock(int id, int qty)
        {
            bool res3 = TWNewEgg.Website.ECWeb.Service.ItemStockService.CheckStockQtyNeweggUSA(id, qty);
            return Content(res3.ToString());
        }
    }
}
