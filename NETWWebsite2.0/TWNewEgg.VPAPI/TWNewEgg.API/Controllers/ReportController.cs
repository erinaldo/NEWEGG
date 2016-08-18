using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.API.Controllers
{
    public class ReportController : Controller
    {
        //
        // GET: /Report/

        public JsonResult Index(Models.LoginInfo loginInfo)
        {
            Models.ActionResponse<Models.LoginResult> result = new Models.ActionResponse<Models.LoginResult>();
            result.Code = 0;
            result.IsSuccess = true;
            result.Msg = "Success";
            result.Body = new Models.LoginResult();
            result.Body.UserName = loginInfo.UserName;
            result.Body.Token = "TestToken_"+loginInfo.Password;
            return Json(result,JsonRequestBehavior.AllowGet);
        }

    }
}
