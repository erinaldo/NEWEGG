using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Service;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.API.Attributes;
using System.Transactions;
using TWNewEgg.API.Models;

namespace TWNewEgg.API.Controllers
{
    /// <summary>
    /// AdditionalPurchaseController
    /// </summary>
    public class AdditionalPurchaseController : Controller
    {
        Service.AdditionalPurchaseService AdditionalPurchaseService = new Service.AdditionalPurchaseService();

        [HttpPost]
        public JsonResult Search(TWNewEgg.API.Models.ItemSketchSearchCondition itemSearch, bool boolDefault)
        {
            TWNewEgg.DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            API.Models.ActionResponse<List<Models.AdditionalPurchase>> result = AdditionalPurchaseService.AdditionalPurchaseItemSearchResult(itemSearch, boolDefault);

            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Edit(TWNewEgg.API.Models.AdditionalPurchase AdditionalPurchaseItem)
        {
            TWNewEgg.DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            ActionResponse<string>  result = AdditionalPurchaseService.AdditionalPurchaseItemEdit(AdditionalPurchaseItem);

            return this.Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
