using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;

namespace TWNewEgg.API.Controllers
{
    public class HiLifePaymentCollectionController : Controller
    {       
        [HttpGet]
        public JsonResult QueryOrderInfo(string OrderNum)
        {
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.HiLifePaymentInfo> result = new Models.ActionResponse<Models.HiLifePaymentInfo>();

            TWNewEgg.API.Service.HiLifePaymentCollectionService hiLifeservice = new Service.HiLifePaymentCollectionService();

            result = hiLifeservice.queryPaymentInfo(OrderNum);

            return Json(result, JsonRequestBehavior.AllowGet);

        }
    }
}
