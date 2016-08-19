using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.API.Controllers
{
    public class SellerFinancialController : Controller
    {
        /// <summary>
        /// [Get] Get Seller Financial
        /// </summary>
        /// <param name="Seller">Search table Seller Financial by keyword(Seller)</param>
        /// <param name="type">Search table Seller Financial seller by whitch type</param>
        
        [HttpGet]
        public JsonResult GetSeller_Financial(string Seller, int type)
        {
            Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_Financial> FinancialById = null;

            Service.SellerFinancialService SFS = new Service.SellerFinancialService();

            FinancialById = SFS.GetSeller_Financial(Seller, type);

            return Json(FinancialById, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// [Post] Save Seller Financial
        /// </summary>
        /// <param name="Financial">Save Seller Financial Data</param>
        
        [HttpPost]
        public JsonResult SaveSeller_Financial(DB.TWSELLERPORTALDB.Models.Seller_Financial Financial)
        {
            Models.ActionResponse<string> massage = null;

            Service.SellerFinancialService SFS = new Service.SellerFinancialService();

            massage = SFS.SaveSeller_Financial(Financial);

            return Json(massage, JsonRequestBehavior.AllowGet);
        }

    }
}
