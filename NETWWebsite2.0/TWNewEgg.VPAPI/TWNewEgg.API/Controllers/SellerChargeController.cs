using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.API.Controllers
{
    public class SellerChargeController : Controller
    {
        //
        // GET: /SellerCharge/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetSellerCharge(string countryCode)
        {
            TWNewEgg.API.Service.SellerChargeService scs = new Service.SellerChargeService();

            API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_Charge>> result = scs.GetChargeList(countryCode);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /*[HttpPost]
        public JsonResult SaveSellerCharge(TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_Charge sellerCharge)
        {
            TWNewEgg.API.Service.SellerChargeService scs = new Service.SellerChargeService();

            Models.ActionResponse<string> result = scs.SaveSellerCharge(sellerCharge);

            return Json(result, JsonRequestBehavior.AllowGet);
        }*/

        [HttpPost]
        public JsonResult SaveSellerCharge(TWNewEgg.API.Models.SaveSellerCharge sellerInvitation)
        {
            TWNewEgg.API.Service.SellerChargeService scs = new Service.SellerChargeService();

            Models.ActionResponse<string> result = scs.SaveSellerCharge(sellerInvitation);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
