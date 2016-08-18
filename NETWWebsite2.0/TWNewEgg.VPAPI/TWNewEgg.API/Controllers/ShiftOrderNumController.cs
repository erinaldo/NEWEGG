using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using API = TWNewEgg.API.Models;

namespace TWNewEgg.API.Controllers
{
    /// <summary>
    /// 接收前台訂單編號進行 Mail 通知處理 Controller
    /// </summary>
    public class ShiftOrderNumController : Controller
    {      
        /// <summary>
        /// 處理接收到前台訂單編號，寄送訂單、庫存警示 Mail
        /// </summary>
        /// <param name="orderNum">訂單編號</param>
        /// <returns>傳回 Dictionary 列出信件寄送狀況</returns>
        [HttpPost]
        public JsonResult ProcessOrderNumMail(string orderNum)
        {
            API.Models.ActionResponse<Dictionary<string, string>> result = new Models.ActionResponse<Dictionary<string, string>>();

            API.Service.ProcessOrderNumService processOrderNumService = new Service.ProcessOrderNumService();

            result = processOrderNumService.ProcessOrderNumMail(orderNum);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 處理接收到前台訂單編號，寄送訂單取消 Mail
        /// </summary>
        /// <param name="orderNum">訂單編號</param>
        /// <returns>傳回 Dictionary 列出信件寄送狀況</returns>
        [HttpPost]
        public JsonResult ProcessVoidOrderNumMail(string orderNum)
        {
            API.Models.ActionResponse<Dictionary<string, string>> result = new Models.ActionResponse<Dictionary<string, string>>();

            API.Service.ProcessOrderNumService processOrderNumService = new Service.ProcessOrderNumService();

            result = processOrderNumService.ProcessVoidOrderNumMail(orderNum);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 處理接收到前台訂單編號，寄送訂單取消 Mail
        /// </summary>
        /// <param name="orderNum">訂單編號</param>
        /// <returns>傳回 Dictionary 列出信件寄送狀況</returns>
        [HttpPost]
        public JsonResult ProcessRMANumMail(string orderNum)
        {
            API.Models.ActionResponse<string> result = new Models.ActionResponse<string>();

            API.Service.ProcessOrderNumService processOrderNumService = new Service.ProcessOrderNumService();

            result = processOrderNumService.ProcessRMAOrderNumMail(orderNum);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 處理接收到前台訂單編號，寄送退貨通知 Mail 
        /// </summary>
        /// <param name="orderNum">訂單編號</param>
        /// <returns>傳回 Dictionary 列出信件寄送狀況</returns>
        [HttpPost]
        public JsonResult ProcessRMASuccessNumMail(string orderNum)
        {
            API.Models.ActionResponse<string> result = new Models.ActionResponse<string>();

            API.Service.ProcessOrderNumService processOrderNumService = new Service.ProcessOrderNumService();

            result = processOrderNumService.ProcessRMASuccess(orderNum);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
       
    }
}
