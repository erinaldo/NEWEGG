using System.Collections.Generic;
using System.Web.Mvc;
using TWNewEgg.API.Models;

namespace TWNewEgg.API.Controllers
{
    public class SellerInvitationController : Controller
    {
        #region 11/1移除

        /*
        [HttpGet]
        public JsonResult GetSellerCharge(string ChargeType)
        {
            API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_Charge>> list = new ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_Charge>>();

            Service.SellerInvitationService SIS = new Service.SellerInvitationService();

            if (ChargeType == "S")
                list = SIS.showStandardList();

            else
                list = SIS.getChargeList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveSellerCharge(TWNewEgg.API.Models.SellerInvitation sellerInvitation)
        {
            API.Models.ActionResponse<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo> result = new Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>();
            API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_Charge>> chargeResult = new Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_Charge>>();
            Service.SellerInvitationService si = new Service.SellerInvitationService();

            result = si.SellerInvitationInfo(sellerInvitation);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        */

        #endregion 11/1移除

        /// <summary>
        /// Seller Manage 取佣金資料
        /// 不需要查詢的條件請傳null
        /// 標準佣金查詢：SellerID = 0
        /// ChargeType：S-Standard、A-Anchor
        /// </summary>
        /// <returns></returns>
        ///
        [Attributes.ActionDescriptionAttribute("取得佣金資料")]
        //[Filters.PermissionFilter]
        // 11/11改成post silverlight測試 by jack.w
        [HttpPost]
        // 11/12改成model傳入 by Jack Lin
        public JsonResult GetSellerCharge(TWNewEgg.API.Models.GetSellerCharge getSellerCharge)//(int? sellerID, int? countryCode, string chargeType, int? categoryID)
        {
            TWNewEgg.API.Service.SellerInvitationService SIService = new Service.SellerInvitationService();

            API.Models.ActionResponse<List<Models.GetSellerChargeResult>> apiResult = SIService.GetChargeList(getSellerCharge);

            return Json(apiResult, JsonRequestBehavior.AllowGet);
        }

        [Attributes.ActionDescriptionAttribute("儲存佣金資料")]
        //[Filters.PermissionFilter]
        [HttpPost]
        public JsonResult SaveSellerCharge(TWNewEgg.API.Models.SaveSellerCharge saveSellerCharge)
        {
            TWNewEgg.API.Service.SellerInvitationService SIService = new Service.SellerInvitationService();

            API.Models.ActionResponse<string> apiResult = SIService.SaveSellerCharge(saveSellerCharge);

            return Json(apiResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取得商家區域列表
        /// </summary>
        /// <returns></returns>
        ///
        [Attributes.ActionDescriptionAttribute("取得區域列表")]
        //[Filters.PermissionFilter]
        [HttpGet]
        public JsonResult GetRegionList()
        {
            API.Models.ActionResponse<List<API.Models.GetRegionListResult>> apiResult = new ActionResponse<List<API.Models.GetRegionListResult>>();

            Service.SellerInvitationService SIService = new Service.SellerInvitationService();

            apiResult = SIService.GetRegionList();

            return Json(apiResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取得幣別列表
        /// </summary>
        /// <returns></returns>
        ///
        [Attributes.ActionDescriptionAttribute("取得幣別列表")]
        //[Filters.PermissionFilter]
        [HttpGet]
        public JsonResult GetCurrencyList()
        {
            API.Models.ActionResponse<List<API.Models.GetCurrencyListResult>> apiResult = new ActionResponse<List<API.Models.GetCurrencyListResult>>();

            Service.SellerInvitationService SIService = new Service.SellerInvitationService();

            apiResult = SIService.GetCurrencyList();

            return Json(apiResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 寄送邀請信
        /// </summary>
        /// <returns></returns>
        ///
        [Attributes.ActionDescriptionAttribute("寄送邀請信")]
        //[Filters.PermissionFilter]
        [HttpPost]
        public JsonResult SendInvitationEmail(TWNewEgg.API.Models.SendInvitationEmail sendInvitationEmail)//(TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo basicInfo)
        {
            Service.SellerInvitationService SIService = new Service.SellerInvitationService();

            //回傳成功與否
            API.Models.ActionResponse<TWNewEgg.API.Models.SendInvitationEmailResult> apiResult = new ActionResponse<TWNewEgg.API.Models.SendInvitationEmailResult>();
            apiResult = SIService.SendInvitationEmail(sendInvitationEmail);

            // 11/4 有問題先註解掉
            //return Json(bool, JsonRequestBehavior.AllowGet);
            return Json(apiResult, JsonRequestBehavior.AllowGet);
        }
    }
}