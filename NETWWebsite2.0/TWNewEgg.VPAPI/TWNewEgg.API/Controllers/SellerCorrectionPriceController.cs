using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Service;

using TWNewEgg.Models.DomainModels;

namespace TWNewEgg.API.Controllers
{
    /// <summary>
    /// 依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160721
    /// </summary>
    public class SellerCorrectionPriceController : Controller
    {

        /// <summary>
        /// 取得供應商對帳單新增調整項目
        /// </summary>
        /// <param name="salesorderCode">LBO</param>
        /// <param name="user_name">建立者</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetGroupBy(string finanStatus, int sellerID, string settlementID, string user_name)
        {
            JsonResult json_data = new JsonResult();
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM>> list_result = new TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM>>();
           
            list_result.IsSuccess = true;
            list_result.Body = SellerCorrectionPriceService.GetGroupBy(finanStatus, sellerID, settlementID, user_name);
            json_data = this.Json(list_result, JsonRequestBehavior.AllowGet);
            return json_data;
        }

        /// <summary>
        /// 儲存供應商對帳單新增調整項目
        /// </summary>
        /// <param name="sellerID"></param>
        /// <param name="settlementID"></param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Save1(string finanStatus, int sellerID, string settlementID, string user_name)
        {
            JsonResult json_data = new JsonResult();
            TWNewEgg.API.Models.ActionResponse<List<bool>> list_result = new TWNewEgg.API.Models.ActionResponse<List<bool>>();

            list_result.IsSuccess = true;

            list_result.Body = SellerCorrectionPriceService.Save1(finanStatus, sellerID, settlementID, user_name);
            json_data = this.Json(list_result, JsonRequestBehavior.AllowGet);
            return json_data;
        }

        

    }
}
