using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.API.Controllers
{
    
    public class APIUserController : Controller
    {
        [Attributes.ActionDescriptionAttribute("API帳號登入")]
        public JsonResult Login(Models.LoginInfo loginInfo)
        {
            Models.ActionResponse<Models.LoginResult> r = null;
            if (loginInfo.UserName != null && loginInfo.Password != null)
            {
                Service.APIUserService accountService = new Service.APIUserService();
                r = accountService.Login(loginInfo);
            }
            
            return Json(r,JsonRequestBehavior.AllowGet);
        }

        [Attributes.ActionDescriptionAttribute("API帳號詳細資料")]
        [Filters.PermissionFilter]
        public JsonResult UserDetail(Models.LoginInfo loginInfo)
        {
            Models.ActionResponse<DB.TWBACKENDDB.Models.API_User> r = null;
            if (loginInfo.UserName != null && loginInfo.Password != null)
            {
                Service.APIUserService accountService = new Service.APIUserService();
                r = accountService.UserDetail(loginInfo);
                r.Msg = "詳細資料";
            }

            return Json(r, JsonRequestBehavior.AllowGet);
        }
    }
}
