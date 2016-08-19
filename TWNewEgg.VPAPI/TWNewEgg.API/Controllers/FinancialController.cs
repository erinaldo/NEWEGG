using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.API.Controllers
{
    public class FinancialController : Controller
    {
        #region 主單
        
        /// <summary>
        /// 對帳單主單查詢
        /// </summary>
        /// <param name="searchCondition">搜尋條件</param>
        /// <returns>主單清單</returns>
        [HttpPost]
        public JsonResult GetMainStatement(TWNewEgg.API.Models.MainStatementSearchCondition searchCondition)
        {
            TWNewEgg.API.Service.FinancialService financialService = new Service.FinancialService();
            var result = financialService.GetMainStatement(searchCondition);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion 主單

        public JsonResult GetDetailData(string SettlementID, int sellerid)
        {
            TWNewEgg.API.Service.FinancialService finan = new Service.FinancialService();
            var result = finan.GetDetailData(SettlementID, sellerid);
            return Json(result);
        }
        //public JsonResult pushInvoNumAndInvoDate(string InvoDate, string InvoNum,string SettlementID)
        //{
        //    TWNewEgg.API.Service.FinancialService finan = new Service.FinancialService();
        //    var result = finan.pushInvoNumAndInvoDate(InvoDate, InvoNum, SettlementID);
        //    return Json(result);
        //}

        //依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160721
        public JsonResult pushInvoNumAndInvoDate(string InvoDate, string InvoNum, string SettlementID, int sellerID)
        {
            TWNewEgg.API.Service.FinancialService finan = new Service.FinancialService();
            var result = finan.pushInvoNumAndInvoDate(InvoDate, InvoNum, SettlementID, sellerID);
            return Json(result);
        }

        public JsonResult _FinancialExportExcel(string SettlementIDNumber, int sellerid)
        {
            TWNewEgg.API.Service.FinancialService finan = new Service.FinancialService();
            var result = finan._FinancialExportExcel(SettlementIDNumber, sellerid);
            return Json(result);
        }

    }
}
