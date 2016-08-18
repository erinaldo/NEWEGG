using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Service;

namespace TWNewEgg.API.Controllers
{
    public class StorageDetailController : Controller
    {
        #region 倉儲明細SP_RPT_Storage

        [HttpGet]
        public JsonResult Index(string inputSellerName, string inputSellerID, string ProductID, string SellerProductID)
        {
            Service.StorageDetailService StorageDetailService = new Service.StorageDetailService();
            Models.ActionResponse<List<Models.StorageDetailSPResult>> result = StorageDetailService.GetDataStorageDetail(inputSellerName,inputSellerID,ProductID,SellerProductID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// exec SP_RPT_Storage
        /// </summary>
        /// <param name="StorageDetailSPSearch"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult storageDetailReport(TWNewEgg.API.Models.StorageDetailSPSearch StorageDetailSPSearch)
        {
            Service.StorageDetailService StorageDetailService = new Service.StorageDetailService();
            Models.ActionResponse<List<Models.StorageDetailSPResult>> result = StorageDetailService.PostDataStorageDetail(StorageDetailSPSearch);
            return Json(result);
        }
        #endregion

    }
}
