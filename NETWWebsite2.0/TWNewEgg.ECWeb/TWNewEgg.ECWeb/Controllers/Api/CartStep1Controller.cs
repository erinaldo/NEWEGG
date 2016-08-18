using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TWNewEgg.ECWeb.Auth;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.Models.ViewModels.Cart;
using TWNewEgg.Models.ViewModels.Message;

namespace TWNewEgg.ECWeb.Controllers.Api
{
    public class CartStep1Controller : ApiController
    {
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        [HttpPost]
        public ECWebResponse CreateCartTemp(CartStep1Data step1Data)
        {
            try
            {
                logger.Info("Start---------AccountID:" + NEUser.ID + " CartType:" + step1Data.CartTypeID);
                var genSNResult = Processor.Request<string, string>("CartTempServices.CartTempService", "GenerateSerialNumber", NEUser.ID, step1Data.CartTypeID);
                if (genSNResult.error != null)
                {
                    throw new Exception(genSNResult.error);
                }

                string serialNumber = genSNResult.results;
                step1Data.SerialNumber = serialNumber;
                CartTempDM InitialCartTempDM = ModelConverter.ConvertTo<CartTempDM>(step1Data);
                InitialCartTempDM.AccountID = NEUser.ID;
                InitialCartTempDM.Status = 0;

                var createCartTempResult = Processor.Request<CartTempDM, CartTempDM>("CartTempServices.CartTempService", "CreateCartTemp", InitialCartTempDM);
                if (createCartTempResult.error != null)
                {
                    throw new Exception(createCartTempResult.error);
                }

                CartTempDM cartTemp = createCartTempResult.results;

                return new ECWebResponse { Data = cartTemp, Status = (int)ECWebResponse.StatusCode.成功 };
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return new ECWebResponse { Message = e.Message, Status = (int)ECWebResponse.StatusCode.系統錯誤 };
            }
        }
    }
}
