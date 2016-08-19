using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using TWNewEgg.ECWeb.Auth;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.Models.ViewModels.Cart;
using TWNewEgg.Models.ViewModels.Message;
using System.Web.Http;
using TWNewEgg.Models.DomainModels.Message;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace TWNewEgg.ECWeb.Controllers.Api
{
    public class CartStep3Controller : ApiController
    {
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        [HttpPost]
        public ECWebResponse Step3(CartStep2Data step2Data)
        {
            try
            {
                ECWebResponse res;
                var getCartTempResult = Processor.Request<CartTempDM, CartTempDM>("CartTempServices.CartTempService", "GetCartTempBySN", step2Data.SerialNumber);
                if (getCartTempResult.error != null)
                {
                    throw new Exception(getCartTempResult.error);
                }

                CartTempDM cartTempDM = getCartTempResult.results;
                cartTempDM.Status = (int)CartTempDM.StatusEnum.Step3;
                ResponseMessage<CartTempDM> resMsg = this.UpdateCartTemp(cartTempDM);
                if (resMsg.IsSuccess)
                {
                    Controllers.CartController cartController = new Controllers.CartController(HttpContext.Current);
                    res = (ECWebResponse)cartController.Step3(step2Data).Data;
                    if (res.Status == (int)ECWebResponse.StatusCode.成功 || res.Status == (int)ECWebResponse.StatusCode.轉頁付款)
                    {
                        if (res.Data != null)
                        {
                            int salesOrderGroupID = 0;
                            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                            // 序列化
                            string szJson = jsonSerializer.Serialize(res.Data);
                            try
                            {
                                // 反序列化
                                SGTempModel getSGTempModel = JsonConvert.DeserializeObject<SGTempModel>(szJson);
                                salesOrderGroupID = getSGTempModel.SOGroupId;
                            }
                            catch (Exception ex)
                            {
                                int.TryParse(szJson, out salesOrderGroupID);
                            }

                            cartTempDM.SalesOrderGroupID = salesOrderGroupID;
                        }

                        cartTempDM.Status = (int)CartTempDM.StatusEnum.SOCreated;
                        this.UpdateCartTemp(cartTempDM);
                    }
                    else
                    {
                        cartTempDM.Status = (int)CartTempDM.StatusEnum.Step2;
                        this.UpdateCartTemp(cartTempDM);
                    }
                }
                else
                {
                    switch (resMsg.Error.Code)
                    {
                        case (int)CartTempDM.UpdateResultErrorCode.處理中:
                            res = new ECWebResponse { Status = (int)ECWebResponse.StatusCode.處理中, Message = "處理中，請稍候。" };
                            break;
                        case (int)CartTempDM.UpdateResultErrorCode.連線逾時:
                            res = new ECWebResponse { Status = (int)ECWebResponse.StatusCode.返回購物車頁, Message = "購物車連線逾時，系統將自動導回購物車頁。" };
                            break;
                        case (int)CartTempDM.UpdateResultErrorCode.系統流程錯誤:
                            res = new ECWebResponse { Status = (int)ECWebResponse.StatusCode.流程錯誤, Message = "不允許的動作。" };
                            break;
                        case (int)CartTempDM.UpdateResultErrorCode.系統錯誤:
                            res = new ECWebResponse { Status = (int)ECWebResponse.StatusCode.系統錯誤, Message = "連線異常，系統將自動導回購物車頁。" };
                            break;
                        default:
                            res = new ECWebResponse { Status = (int)ECWebResponse.StatusCode.系統錯誤, Message = "連線異常，系統將自動導回購物車頁。" };
                            break;
                    }
                }
                
                return res;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return new ECWebResponse { Message = "連線異常，系統將自動導回購物車頁。", Status = (int)ECWebResponse.StatusCode.系統錯誤 };
            }
        }

        private class SGTempModel
        {
            public int SOGroupId { get; set; }
        }

        private ResponseMessage<CartTempDM> UpdateCartTemp(CartTempDM cartTempDM)
        {
            var updateCartTempResult = Processor.Request<ResponseMessage<CartTempDM>, ResponseMessage<CartTempDM>>("CartTempServices.CartTempService", "UpdateCartTemp", cartTempDM);
            if (updateCartTempResult.error != null)
            {
                throw new Exception(updateCartTempResult.error);
            }

            ResponseMessage<CartTempDM> resMsg = updateCartTempResult.results;
            return resMsg;
        }
    }
}
