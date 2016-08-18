using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.Models.DomainModels.Message;
using TWNewEgg.Models.ViewModels.Cart;
using TWNewEgg.Models.ViewModels.Message;

////依據 BSATW-173 廢四機需求增加
////廢四機同意---------------------------add by bruce 20160516
//using TWNewEgg.Models.DomainModels.Discard4;

namespace TWNewEgg.ECWeb.Controllers.Api
{
    public class CartStep2Controller : ApiController
    {
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        [HttpPost]
        public ECWebResponse Step2(CartStep1Data step1Data)
        {
            try
            {
                ECWebResponse res;

                var getCartTempResult = Processor.Request<CartTempDM, CartTempDM>("CartTempServices.CartTempService", "GetCartTempBySN", step1Data.SerialNumber);
                if (getCartTempResult.error != null)
                {
                    throw new Exception(getCartTempResult.error);
                }

                CartTempDM cartTempDM = ModelConverter.ConvertTo<CartTempDM>(step1Data);
                cartTempDM.Status = (int)CartTempDM.StatusEnum.Step2;
                var updateCartTempResult = Processor.Request<ResponseMessage<CartTempDM>, ResponseMessage<CartTempDM>>("CartTempServices.CartTempService", "UpdateCartTemp", cartTempDM);
                if (updateCartTempResult.error != null)
                {
                    throw new Exception(updateCartTempResult.error);
                }
                
                ResponseMessage<CartTempDM> resMsg = updateCartTempResult.results;
                if (resMsg.IsSuccess)
                {
                    res = new ECWebResponse { Status = (int)ECWebResponse.StatusCode.成功, Data = resMsg.Data };
                }
                else 
                {
                    switch (resMsg.Error.Code)
                    {
                        case (int)CartTempDM.UpdateResultErrorCode.處理中:
                            res = new ECWebResponse { Status = (int)ECWebResponse.StatusCode.處理中, Message = "處理中，請稍候。"};
                            break;
                        case (int)CartTempDM.UpdateResultErrorCode.連線逾時:
                            res = new ECWebResponse { Status = (int)ECWebResponse.StatusCode.系統錯誤, Message = "親愛的用戶您好！由於您離開電腦時間過長，請按確認鍵系統即將為您更新資訊，進行重新整理！" };
                            break;
                        case (int)CartTempDM.UpdateResultErrorCode.系統錯誤:
                            res = new ECWebResponse { Status = (int)ECWebResponse.StatusCode.系統錯誤, Message = "連線異常，系統將自動導回購物車頁。" };
                            break;
                        default :
                            res = new ECWebResponse { Status = (int)ECWebResponse.StatusCode.系統錯誤, Message = "連線異常，系統將自動導回購物車頁。" };
                            break;
                    }
                }

                ////依據 BSATW-173 廢四機需求增加
                ////記錄廢四機同意---------------------------add by bruce 20160516
                //int salesOrderGroupID = resMsg.Data.SalesOrderGroupID.GetValueOrDefault();
                //salesOrderGroupID = resMsg.Data.ID;
                //string agreedDiscard4 = step1Data.AgreedDiscard4;
                //string user_name = resMsg.Data.AccountID.ToString();
                //ResponseMessage<Discard4DM> dd4_info = new ResponseMessage<Discard4DM>();                
                //var dd4_result = Processor.Request<ResponseMessage<Discard4DM>, ResponseMessage<Discard4DM>>("Discard4Service", "InitData", salesOrderGroupID, agreedDiscard4, user_name);
                //if (dd4_result.error == null) dd4_info = dd4_result.results;
                ////記錄廢四機同意---------------------------add by bruce 20160516


                return res;
            }
            catch (Exception e)
            {
                return new ECWebResponse { Message = e.Message, Status = (int)ECWebResponse.StatusCode.系統錯誤 };
            }
        }
    }
}
