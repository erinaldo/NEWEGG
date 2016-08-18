using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Service;

namespace TWNewEgg.API.Controllers
{
    public class SummaryController : Controller
    {

        #region 摘要SP_RPT_Summary

        // <summary>
        // 摘要SP
        // </summary>
        // <param name="SummaryResult"></param>
        // <returns></returns>
        [HttpGet]
        public JsonResult Index(int sellerID,string  beginDate,string endDate) //Models.SummaryResult SummaryResult)
        {
            Service.SummaryService summaryService = new Service.SummaryService();
            Models.ActionResponse<List<Models.SettlementInfo>> result = summaryService.GetDataSummary(sellerID, beginDate, endDate);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult summaryReport(TWNewEgg.API.Models.SummarySPSrarch SummarySPSrarch)
        {
            Service.SummaryService summaryService = new Service.SummaryService();
            Models.ActionResponse<List<Models.SummarySPResult>> result = summaryService.PostDataSummary(SummarySPSrarch);
            return Json(result);
        }
        #endregion


    }
}
