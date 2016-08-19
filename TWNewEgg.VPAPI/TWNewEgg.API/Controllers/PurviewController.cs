using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.API.Controllers
{
    public class PurviewController : Controller
    {
        //
        // GET: /Purview/
        public JsonResult GetSeller_PurviewCount(string seller)
        {
            Models.ActionResponse<int> purviewCount = null;

            Service.PurviewService PrvSvc = new Service.PurviewService();

            purviewCount = PrvSvc.GetSeller_PurviewCount(seller);

            return Json(purviewCount, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUser_PurviewCount(int userID)
        {
            Models.ActionResponse<int> purviewCount = null;

            Service.PurviewService PrvSvc = new Service.PurviewService();

            purviewCount = PrvSvc.GetUser_PurviewCount(userID);

            return Json(purviewCount, JsonRequestBehavior.AllowGet);
        }

        #region Written by Jack Lin

        /// <summary>
        /// 取得用戶權限
        /// </summary>
        /// <returns></returns>
        [Attributes.ActionDescriptionAttribute("取得用戶權限")]
        //[Filters.PermissionFilter]
        [HttpPost]
        public JsonResult GetUserPurview(int userID, string accounttypecode)
        {
            Service.PurviewService PrvSvc = new Service.PurviewService();

            API.Models.ActionResponse<List<TWNewEgg.API.Models.GetPurviewResult>> apiResult = new API.Models.ActionResponse<List<TWNewEgg.API.Models.GetPurviewResult>>();
            apiResult = PrvSvc.GetUserPurview(userID, accounttypecode);

            return Json(apiResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 儲存用戶權限
        /// </summary>
        /// <returns></returns>
        [Attributes.ActionDescriptionAttribute("儲存用戶權限")]
        //[Filters.PermissionFilter]
        [HttpPost]
        public JsonResult SaveUserPurview(Models.SaveUserPurview userPurview)
        {
            Service.PurviewService PrvSvc = new Service.PurviewService();

            API.Models.ActionResponse<string> apiResult = new API.Models.ActionResponse<string>();
            apiResult = PrvSvc.SaveUserPurview(userPurview);

            return Json(apiResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取得Seller權限
        /// </summary>
        /// <returns></returns>
        [Attributes.ActionDescriptionAttribute("取得Seller權限")]
        //[Filters.PermissionFilter]
        [HttpPost]
        public JsonResult GetSellerPurview(int sellerID)
        {
            Service.PurviewService PrvSvc = new Service.PurviewService();

            API.Models.ActionResponse<List<TWNewEgg.API.Models.GetPurviewResult>> apiResult = new API.Models.ActionResponse<List<TWNewEgg.API.Models.GetPurviewResult>>();
            apiResult = PrvSvc.GetSellerPurview(sellerID);

            return Json(apiResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取得Group權限
        /// </summary>
        /// <returns></returns>
        [Attributes.ActionDescriptionAttribute("取得Group權限")]
        //[Filters.PermissionFilter]
        [HttpPost]
        public JsonResult GetGroupPurview(int groupID)
        {
            Service.PurviewService PrvSvc = new Service.PurviewService();

            API.Models.ActionResponse<List<TWNewEgg.API.Models.GetPurviewResult>> apiResult = new API.Models.ActionResponse<List<TWNewEgg.API.Models.GetPurviewResult>>();
            apiResult = PrvSvc.GetGroupPurview(groupID);

            return Json(apiResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取得user列表
        /// </summary>
        /// <returns></returns>
        [Attributes.ActionDescriptionAttribute("取得user列表")]
        //[Filters.PermissionFilter]
        [HttpPost]
        public JsonResult GetUserList(int sellerID)
        {
            Service.PurviewService PrvSvc = new Service.PurviewService();

            API.Models.ActionResponse<List<Models.GetUserListResult>> apiResult = new API.Models.ActionResponse<List<Models.GetUserListResult>>();
            apiResult = PrvSvc.GetUserList(sellerID);

            return Json(apiResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取得user列表
        /// </summary>
        /// <returns></returns>
        [Attributes.ActionDescriptionAttribute("取得功能列表")]
        //[Filters.PermissionFilter]
        [HttpGet]
        public JsonResult GetFunctionList()
        {
            Service.PurviewService PrvSvc = new Service.PurviewService();

            API.Models.ActionResponse<List<Models.GetFunctionListResult>> apiResult = new API.Models.ActionResponse<List<Models.GetFunctionListResult>>();
            apiResult = PrvSvc.GetFunctionList();

            return Json(apiResult, JsonRequestBehavior.AllowGet);
        }


        #endregion
    }
}
