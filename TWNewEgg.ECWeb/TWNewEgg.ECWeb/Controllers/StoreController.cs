using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.ECWeb.PrivilegeFilters;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DomainModels.Store;
using TWNewEgg.Models.DomainModels.Category;

namespace TWNewEgg.ECWeb.Controllers
{
    [AllowNonSecures]
    [AllowAnonymous]
    public class StoreController : Controller
    {
        /// <summary>
        /// 訪問一般分類頁.
        /// </summary>
        /// <returns>StroeInfo Model.</returns>
        public ActionResult Index(int? StoreId)
        {
            //檢查輸入
            if (StoreId == null)
            {
                return RedirectToAction("Index", "Home", new { returnUrl = "/Home" });
            }
            ////美國新蛋直購
            if (StoreId == 737)
            {
                return RedirectToAction("Index", "StoreUS", new { StoreID=StoreId });
            }
            ViewData["storeId"] = StoreId;
            
            StoreInfo storeInfo = new StoreInfo();
            storeInfo = TWNewEgg.Framework.ServiceApi.Processor.Request<StoreInfo, StoreInfo>("StoreService", "GetStoreInfo", StoreId, new List<int>(new int[] { 10 })).results;            
            return View(storeInfo);
        }
    }
}
