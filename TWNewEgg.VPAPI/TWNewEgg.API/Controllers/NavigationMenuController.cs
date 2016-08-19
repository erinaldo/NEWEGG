using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Models;

namespace TWNewEgg.API.Controllers
{
    public class NavigationMenuController : Controller
    {
        // GET api/<controller>
        [HttpGet]
        public JsonResult GetSeller_FuctionBySellerLanguage(string sellerID, string languageCode)
        {
            Models.ActionResponse<List<Seller_FunctionJoinCategory>> Functions = null;

            Service.NavigationMenuService NMS = new Service.NavigationMenuService();

            Functions = NMS.GetSeller_FuctionBySellerLanguage(sellerID, languageCode);

            return Json(Functions, JsonRequestBehavior.AllowGet);
        }

        // POST api/<controller>
        [HttpPost]
        public JsonResult GetSeller_FuctionsByQuery(QueryFunctionCondition query)
        {
            Models.ActionResponse<List<Seller_FunctionJoinCategory>> functions = null;

            Service.NavigationMenuService NMS = new Service.NavigationMenuService();

            functions = NMS.GetSeller_FuctionsByQuery(query);

            return Json(functions, JsonRequestBehavior.AllowGet);
        }


        // GET api/<controller>
        [HttpGet]
        public JsonResult GetSeller_FunctionCategoryByLanguage(string language)
        {
            Models.ActionResponse<List<Seller_FunctionCategoryLocalized>> categories = null;

            Service.NavigationMenuService NMS = new Service.NavigationMenuService();

            categories = NMS.GetSeller_FunctionCategoryByLanguage(language);

            return Json(categories, JsonRequestBehavior.AllowGet);
        }


        
    }
}
