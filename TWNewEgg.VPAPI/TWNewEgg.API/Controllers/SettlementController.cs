using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Service;
using TWNewEgg.API.Models;

namespace TWNewEgg.API.Controllers
{
    public class SettlementController : Controller
    {

        #region 結算SP_RPT_Storage
      
        [HttpGet]
        public JsonResult Index(int inputSellerID, string inputStartDate, string inputEndDate)
        {
            Service.SettlementService SettlementService = new Service.SettlementService();
            Models.ActionResponse<List<Models.SettlementSPResult>> result = SettlementService.GetDataSettlement(inputSellerID, inputStartDate, inputEndDate);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult settlementReport(TWNewEgg.API.Models.SettlementSPSearch SettlementSPSearch)
        {
            Service.SettlementService SettlementService = new Service.SettlementService();
            Models.ActionResponse<List<Models.SettlementSPResult>> result = SettlementService.PostDataSettlement(SettlementSPSearch);
            return Json(result,JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
