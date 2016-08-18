using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.API.Controllers
{
    public class FindSellerController : Controller
    {
        /// <summary>
        /// [Get] Get Seller Name
        /// </summary>
        
        [HttpGet]
        public JsonResult GetSellerName()
        {
            Models.ActionResponse<List<string>> SellerName = null;

            Service.FindSellerService MAS = new Service.FindSellerService();
            SellerName = MAS.SellerName();


            return Json(SellerName, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// [Get] Get Account Info
        /// </summary>
        /// <param name="searchword">Search table Seller ReturnAddress by keyword(searchword)</param>
        /// <param name="type">Search table Seller ReturnAddress seller by whitch type</param>
        
        [HttpGet]
        public JsonResult GetAccountInfo(int type = 0, string searchword = "")
        {
            Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>> BasicInfo = null;

            Service.FindSellerService MAS = new Service.FindSellerService();
            BasicInfo = MAS.SearchSeller(type, searchword);

            return Json(BasicInfo, JsonRequestBehavior.AllowGet);
        }
    }
}