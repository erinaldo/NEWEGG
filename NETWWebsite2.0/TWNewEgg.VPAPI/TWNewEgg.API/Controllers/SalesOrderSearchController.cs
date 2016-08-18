using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Models;
using TWNewEgg.API.Service;
using TWNewEgg.DB.TWBACKENDDB.Models;

namespace TWNewEgg.API.Controllers
{
    public class SalesOrderSearchController : Controller
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        /// <summary>
        /// 取得訂單主單清單
        /// </summary>
        /// <param name="searchCondition">搜尋絛件</param>
        /// <returns>訂單主單清單</returns>
        public JsonResult GetMainOrder(MainOrderSearchCondition searchCondition)
        {
            ActionResponse<MainOrderResult> result = new ActionResponse<MainOrderResult>();
            SalesOrderSearchService salesOrderSearchService = new SalesOrderSearchService();

            try
            {
                result = salesOrderSearchService.GetMainOrder(searchCondition);
            }
            catch (Exception ex)
            {
                logger.Info(string.Format("取得訂單主單清單失敗(exception); ErrorMessage = {0}; Exception = {1}.", GetExceptionMessage(ex), ex.ToString()));
                result.Finish(false, (int)ResponseCode.Error, "資料讀取失敗", null);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 訂單子單資料蒐集
        /// </summary>
        /// <param name="cartID">訂單編號</param>
        /// <returns>返回蒐集結果</returns>
        [HttpPost]
        public JsonResult OrderDetail(string cartID)
        {
            ActionResponse<OrderDetail> result = new ActionResponse<OrderDetail>();
            SalesOrderSearchService salesOrderSearchService = new SalesOrderSearchService();
            try
            {
                result = salesOrderSearchService.OrderDetail(cartID);
            }
            catch (Exception ex)
            {
                logger.Error("訂單編號 : " + cartID + " 系統意外結束 : [ErrorMessage] " + ex.ToString());
                result.IsSuccess = false;
                result.Code = (int)ResponseCode.Error;
                result.Msg = "訂單編號 : " + cartID + " 系統執行失敗，請與客服聯繫";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取得訂單編號的完整資訊
        /// </summary>
        /// <param name="cartID">訂單編號</param>
        /// <returns>返回查詢結果</returns>
        [HttpPost]
        public JsonResult GetCartInfo(string cartID)
        {
            TWNewEgg.API.Models.ActionResponse<TinyCart> result = new ActionResponse<TinyCart>();
            SalesOrderSearchService salesOrderSearchService = new SalesOrderSearchService();
            try
            {
                result = salesOrderSearchService.GetCartInfo(cartID);
            }
            catch (Exception ex)
            {
                logger.Error("訂單編號 : " + cartID + " 取得訂單資訊，系統意外結束 : [ErrorMessage] " + ex.ToString());
                result.Finish(false, (int)ResponseCode.Error, ex.Message, null);
            }
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更新訂單遞送狀態
        /// </summary>
        /// <param name="cartID">訂單編號</param>
        /// <param name="delvStatus">訂單出貨狀態</param>
        /// <param name="updateUser">updateUser</param>
        /// <returns>返回結果</returns>
        [HttpPost]
        public JsonResult UpdateCartDelvStatus(string cartID, int delvStatus, string updateUser)
        {
            ActionResponse<TinyCart> result = new ActionResponse<TinyCart>();
            SalesOrderSearchService salesOrderSearchService = new SalesOrderSearchService();
            try
            {
                result = salesOrderSearchService.UpdateCartDelvStatus(cartID, delvStatus, updateUser);
            }
            catch (Exception ex)
            {
                logger.Error("訂單編號 : " + cartID + " 更新訂單資訊，系統意外結束 : [ErrorMessage] " + ex.ToString());
                result.Finish(false, (int)ResponseCode.Error, ex.Message, null);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 取得 Exception 錯誤訊息
        /// </summary>
        /// <param name="ex">Exception 內容</param>
        /// <returns>Exception 錯誤訊息</returns>
        private string GetExceptionMessage(System.Exception ex)
        {
            string errorMessage = string.Empty;

            if (ex.Message.IndexOf("See the inner exception for details.") != -1)
            {
                errorMessage = ex.InnerException.Message;

                if (errorMessage.IndexOf("See the inner exception for details.") != -1)
                {
                    errorMessage = GetExceptionMessage(ex.InnerException);
                }
            }
            else
            {
                errorMessage = ex.Message;
            }

            return errorMessage;
        }
    }
}
