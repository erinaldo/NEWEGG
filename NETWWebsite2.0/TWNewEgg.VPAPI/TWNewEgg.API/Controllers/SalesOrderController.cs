using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;
using log4net.Config;
using System.Transactions;

namespace TWNewEgg.API.Controllers
{
    public class SalesOrderController : Controller
    {
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);  //2014.7.8 log4net add by ice
        //
        // GET: /SalesOrder/


        /// <summary>
        /// 查詢訂單(Query Sales Order)
        /// </summary>
        /// <param name="condition">查詢訂單的各項條件</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult QueryOrderInfos(Models.QueryCartCondition condition)
        {
            Service.SalesOrderService sos = new Service.SalesOrderService();
            var result = sos.QueryOrderInfos(condition);
            return Json(result,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 編輯訂單
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult EditOrderInfo(Models.OrderInfo order)
        {
            Service.SalesOrderService sos = new Service.SalesOrderService();
            var result = sos.EditOrderInfo(order);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SendPackage(List<DB.TWSELLERPORTALDB.Models.Seller_DelvTrack> delvTrack)
        {
            Service.SalesOrderService sos = new Service.SalesOrderService();
            
            bool IsCurrentStepSucess = true;
            Models.ActionResponse<bool> result = new Models.ActionResponse<bool>();
            Models.ActionResponse<bool> SendPackageResult = new Models.ActionResponse<bool>();

            //新增包裹
            if (IsCurrentStepSucess)
            {
                // 加入遞送包裹資料至 SellerPortal DB
                SendPackageResult = sos.SendPackage(delvTrack);
                IsCurrentStepSucess = SendPackageResult.IsSuccess;
            }

            //修改遞送狀態為 已出貨
            if (IsCurrentStepSucess)
            {               
                List<string> allSOCodes = sos.GetAllSOCodeByProcessIDs(delvTrack.Select(r => r.ProcessID).ToList());
                foreach (var code in allSOCodes)
                {
                    int delvStatus = (int)Models.OrderInfo.EnumDelvStatus.已出貨;
                    string updateUser = delvTrack.Select(r => r.UpdateUserID).FirstOrDefault().ToString();
                    
                    // 修改 後台遞送包裹資料 (修改 Cart 資料) 押入 Tracking No. 由 TrackingNumAPI 做 2014.9.17 
                    //sos.UpdateDelvStatus(code, delvStatus, updateUser);

                    //2014.7.8 增加是否有呼叫wmsAPI add by ice
                    log.Info("CartID: " + code + "; DelvStatus:" + delvStatus + "; Update User: " + updateUser + " - call wms API before");

                    //2014.6.20 呼叫WMS trancking number API add by ice begin
                    Service.TWService twser = new Service.TWService();
                    result = twser.TrackingNumAPI(code, delvTrack, delvStatus, updateUser);
                    //2014.6.20 呼叫WMS trancking number API add by ice end

                    // 2016.05.25 改由 TrackingNum 押完時，繼續完成押發票的流程，不再由此處觸發
                    // Tracking Num 押入後，觸發押發票 API
                    //twser.UpdateInvoiceAPI(result.IsSuccess, code);

                    //2014.7.8 增加是否有呼叫wmsAPI add by ice
                    log.Info("CartID: " + code + "; Update User: " + updateUser + " - call wms API after and messege = " + result.Msg);
                }               
            }
            else
            {
                result = SendPackageResult;
                log.Error(result.Msg);
            }

            
            if (result.IsSuccess)       //2014.6.25 呼叫wms API，增加此判斷是否成功 add by ice
              result.Finish(true, 0, "OK", true);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult UpdateDelvStatus(string soCode, int delvStatus, string sellerID, string updateUser)
        {
            Models.ActionResponse<List<Models.OrderInfo>> result = new Models.ActionResponse<List<Models.OrderInfo>>();

            Service.SalesOrderService sos = new Service.SalesOrderService();

            //修改遞送狀態
            var updateDelvStatusResult = sos.UpdateDelvStatus(soCode, delvStatus, updateUser);

            //取得修改後的完整OrderInfo
            if (updateDelvStatusResult.IsSuccess)
            {
                Models.QueryCartCondition condition = new Models.QueryCartCondition()
                {
                    SOCode = soCode,
                    SellerID = sellerID
                };

                result = sos.QueryOrderInfos(condition);
            }
            else
            {
                result.Finish(updateDelvStatusResult.IsSuccess, updateDelvStatusResult.Code, updateDelvStatusResult.Msg, null);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult InvoiceAPI(string code)
        {
            TWNewEgg.API.Models.ActionResponse<List<string>> updateResult = new Models.ActionResponse<List<string>>();

            Service.TWService twser = new Service.TWService();
            updateResult = twser.UpdateInvoiceAPI(true, code);

            return Json(updateResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 寄送商家新訂單email
        /// </summary>
        /// <param name="OrderInfo"></param>
        /// <returns></returns>
        //[HttpPost]
        //public JsonResult MailSellerNewOrder(Models.OrderInfo OrderInfo)
        //{
        //    Service.SalesOrderService sos = new Service.SalesOrderService();
        //    var result = sos.EditOrderInfo(order);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        
        //[HttpPost]
        //public JsonResult QueryCart(Models.QueryCartCondition condition)
        //{
        //    Service.SalesOrderService sos = new Service.SalesOrderService();
        //    var result = sos.QueryCart(condition);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// 寄送訂單取消通知信
        /// </summary>
        /// <param name="orderNumber">訂單編號</param>
        /// <returns>成功及失敗訊息</returns>
        [HttpPost]
        public JsonResult SalesOrderCancel(string orderNumber)
        {
            Service.ProcessOrderNumService servicePONS = new Service.ProcessOrderNumService();
            API.Models.ActionResponse<Dictionary<string, string>> result = servicePONS.ProcessVoidOrderNumMail(orderNumber);

            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 遞送包裹撈取貨運公司API
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult APIQueryShipCarrier()
        {
            Models.ActionResponse<List<string>> result = new Models.ActionResponse<List<string>>();
            Service.SalesOrderService sos = new Service.SalesOrderService();
            return Json(sos.QueryShipCarrier(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 配達
        /// </summary>
        /// <param name="soCode"></param>
        /// <param name="delvStatus"></param>
        /// <param name="cartStatus"></param>
        /// <param name="sellerID"></param>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult Arrival(string soCode, int delvStatus, int cartStatus, string sellerID, string updateUser)
        {
            Models.ActionResponse<List<Models.OrderInfo>> result = new Models.ActionResponse<List<Models.OrderInfo>>();
            Service.SalesOrderService sos = new Service.SalesOrderService();

            bool IsCurrentStepSucess = true;


            using (TransactionScope scope = new TransactionScope())
            {
                //修改遞送狀態
                if (IsCurrentStepSucess)
                {
                    var updateDelvStatusResult = sos.UpdateDelvStatus(soCode, delvStatus, updateUser);
                    IsCurrentStepSucess = updateDelvStatusResult.IsSuccess;

                    // 2014/10/17補增加LOG功能 add by Ted
                    log.Info("配達功能 DelvStatus更新是否成功:" + IsCurrentStepSucess.ToString() + " " + updateDelvStatusResult.Msg);
                }

                //修改訂單狀態
                if (IsCurrentStepSucess)
                {
                    var updateCartStatusResult = sos.UpdateCartStatus(soCode, cartStatus, updateUser);
                    IsCurrentStepSucess = updateCartStatusResult.IsSuccess;

                    // 2014/10/17補增加LOG功能 add by Ted
                    log.Info("配達功能 CartStatus更新是否成功:" + IsCurrentStepSucess.ToString() + " " + updateCartStatusResult.Msg);
                }

                if (IsCurrentStepSucess)
                {
                    scope.Complete();
                }
            }

            //取得修改後的完整OrderInfo
            //if (IsCurrentStepSucess)
            //{
                Models.QueryCartCondition condition = new Models.QueryCartCondition()
                {
                    SOCode = soCode,
                    SellerID = sellerID
                };

                result = sos.QueryOrderInfos(condition);
            //}

            // 2014/10/17補增加LOG功能 add by Ted
            log.Info("配達功能 執行是否成功:" + IsCurrentStepSucess.ToString() + " " + result.Msg);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 匯出 Excel 訂單列表
        /// </summary>
        /// <param name="dataInfo">匯出 Excel 訂單列表 Model</param>
        /// <param name="queryCartCondition">訂單查詢篩選條件model</param>
        /// <returns>成功及失敗訊息</returns>
        [HttpPost]
        public JsonResult DownloadSalesOrderList(API.Models.OrderInfo.DownloadSalesOrderListModel dataInfo)
        {
            Models.ActionResponse<string> result = new Models.ActionResponse<string>();
            Service.SalesOrderService serviceSOS = new Service.SalesOrderService();

            //2014/08/04撈取所有資料(不分頁)
            dataInfo.queryCartCondition.PageIndex = 0;
            dataInfo.queryCartCondition.PageSize = 65536; //目前EXCEL匯出的限制，65536筆資料，將來可能因應需求更改。

            //查詢所有資料
            var result_queryAllData = serviceSOS.QueryOrderInfos(dataInfo.queryCartCondition);
            if (result_queryAllData.IsSuccess)
            {
                dataInfo.dataList = result_queryAllData.Body;
                result = serviceSOS.DownloadSalesOrderList(dataInfo);
            }
            else //fail //result_001.IsSuccess == false
            {
                result.Code = result_queryAllData.Code;
                result.Body = null;
                result.IsSuccess = result_queryAllData.IsSuccess;
                result.Msg = result_queryAllData.Msg;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 提醒出貨通知信
        /// </summary>
        /// <param name="MailSeller">是否寄信給Seller</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult Mail_RemindSellerToSendPackage(bool MailSeller)
        {
            Service.SalesOrderService sos = new Service.SalesOrderService();
            var result = sos.Mail_RemindSellerToSendPackage(MailSeller);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
