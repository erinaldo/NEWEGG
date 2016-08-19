using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.API.View.Controllers
{
    public class ItemBatchEditController : Controller
    {
        //
        // GET: /ItemBatchEdit/
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetToken(string account)
        {
            TWNewEgg.API.View.Service.NetPacketUtilities _NetPacketUtilities = new Service.NetPacketUtilities();
            string fromIP = _NetPacketUtilities.GetUserIPAddress();
            TWNewEgg.API.Models.ActionResponseToken<string> tokenResult = new API.Models.ActionResponseToken<string>();
            logger.Info(" account: " + account + " ,fromIP: " + fromIP);
            try
            {
                var getTokenResult = TWNewEgg.Framework.ServiceApi.Processor.Request<TWNewEgg.API.Models.ActionResponseToken<string>, TWNewEgg.API.Models.ActionResponseToken<string>>("AuthTokenService", "getToken", account, fromIP);
                if (string.IsNullOrEmpty(getTokenResult.error) == false)
                {
                    tokenResult.Code = ((int)TWNewEgg.API.Models.CodeStatue.System_Error).ToString().Length == 1 ? "0" + ((int)TWNewEgg.API.Models.CodeStatue.System_Error).ToString() : ((int)TWNewEgg.API.Models.CodeStatue.System_Error).ToString();
                    tokenResult.Msg = TWNewEgg.API.Models.CodeStatue.System_Error.ToString().Replace("_", " ");
                    tokenResult.IsSuccess = false;
                }
                else
                {
                    if (getTokenResult.results.Code.Length == 1)
                    {
                        getTokenResult.results.Code = "0" + getTokenResult.results.Code;
                    }
                    tokenResult = getTokenResult.results;
                }
            }
            catch (Exception ex)
            {
                tokenResult.Code = ((int)TWNewEgg.API.Models.CodeStatue.System_Error).ToString().Length == 1 ? "0" + ((int)TWNewEgg.API.Models.CodeStatue.System_Error).ToString() : ((int)TWNewEgg.API.Models.CodeStatue.System_Error).ToString();
                tokenResult.Msg = TWNewEgg.API.Models.CodeStatue.System_Error.ToString().Replace("_", " ");
                tokenResult.IsSuccess = false;
                logger.Error(ex.ToString());
            }

            return Json(tokenResult, JsonRequestBehavior.AllowGet);
        }
    }
}
