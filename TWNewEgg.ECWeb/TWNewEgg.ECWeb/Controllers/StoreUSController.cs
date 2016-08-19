using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.ECWeb.PrivilegeFilters;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DomainModels.Store;

namespace TWNewEgg.ECWeb.Controllers
{
    [AllowNonSecures]
    [AllowAnonymous]
    public class StoreUSController : Controller
    {
        /// <summary>
        /// 美國直購館
        /// </summary>
        /// <returns>StroeInfo Model.</returns>
        public ActionResult Index(int? StoreId)
        {
            //檢查輸入
            if (StoreId == null)
            {
                return RedirectToAction("Index", "Home", new { returnUrl = "/Home" });
            }
            ViewBag.StoreId = StoreId;

            StoreInfo storeInfo = new StoreInfo();
            storeInfo = Processor.Request<StoreInfo, StoreInfo>("StoreService", "GetStoreInfo", StoreId, new List<int>(new int[] { 10 })).results;
            return View(storeInfo);
        }

        /// <summary>
        /// Left menu for store US.
        /// </summary>
        /// <returns>Left menu of store us.</returns>
        public ActionResult GetLeftMenu()
        {
            List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem> listTreeItem = null;
            listTreeItem = Processor.Request<List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem>, List<TWNewEgg.Models.DomainModels.Category.Category_TreeItem>>("Service.BaseService", "XML2Tree", "XML\\Category_Menu_ALL.XML").results;

            return View(listTreeItem);
        }
    }
}
