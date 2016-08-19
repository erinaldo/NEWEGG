using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Models;

namespace TWNewEgg.API.Controllers
{
    public class ReturnController : Controller
    {
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);  //2014.7.8 log4net add by ice

        #region 主單

        /// <summary>
        /// 取得退貨清單
        /// </summary>
        /// <param name="searchCondition">搜尋條件</param>
        /// <returns>退貨清單</returns>
        [HttpPost]
        public JsonResult GetMainRetgood(MainRetgoodSearchCondition searchCondition)
        {
            ActionResponse<List<MainRetgood>> result = new ActionResponse<List<MainRetgood>>();

            Service.ReturnService returnService = new Service.ReturnService();
            result = returnService.GetMainRegood(searchCondition);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion 主單

        /// <summary>
        /// 退貨商品相關資訊
        /// </summary>
        /// <param name="cartid">訂單編號</param>
        /// <returns>返回退貨商品相關資訊</returns>
        public JsonResult retgoodInfomation(string cartid)
        {
            TWNewEgg.API.Service.ReturnService returnService = new Service.ReturnService();
            var result = returnService.retgoodInfomation(cartid);
            return Json(result);
        }

        /// <summary>
        /// 退貨商品更新備註的資訊
        /// </summary>
        /// <param name="cartid">訂單編號</param>
        /// <returns>返回更新備註的資訊</returns>
        public JsonResult retgoodNote(string cartid)
        {
            TWNewEgg.API.Service.ReturnService returnService = new Service.ReturnService();
            var result = returnService.retgoodNote(cartid);
            return Json(result);
        }

        /// <summary>
        /// 更新退貨資料與狀態
        /// </summary>
        /// <param name="updateRetGoodsInfo">欲更新資訊</param>
        /// <returns>返回更新結果</returns>
        [HttpPost]
        public JsonResult UpdateRetGoods(UpdateRetGoodsInfo updateRetGoodsInfo)
        {
            TWNewEgg.API.Service.ReturnService returnService = new Service.ReturnService();
            Models.ActionResponse<bool> result = returnService.UpdateRetGoods(updateRetGoodsInfo);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更新備註資訊
        /// </summary>
        /// <param name="userID">使用者ID</param>
        /// <param name="cartID">訂單編號</param>
        /// <param name="updateNote">新增備註</param>
        /// <returns>回傳更新結果</returns>
        [HttpPost]
        public JsonResult UpdateRetGoodsNote(int userID, string cartID, string updateNote)
        {
            TWNewEgg.API.Service.ReturnService returnService = new Service.ReturnService();
            Models.ActionResponse<bool> result = returnService.UpdateRetGoodsNote(userID, cartID, updateNote);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 廠商已查看派車明細備註
        /// </summary>
        /// <param name="userID">使用者ID</param>
        /// <param name="cartID">訂單編號</param>
        /// <returns>返回執行結果</returns>
        [HttpPost]
        public JsonResult HasBeenViewed(int userID, string cartID)
        {
            TWNewEgg.API.Service.ReturnService returnService = new Service.ReturnService();
            Models.ActionResponse<bool> result = returnService.HasBeenViewed(userID, cartID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
