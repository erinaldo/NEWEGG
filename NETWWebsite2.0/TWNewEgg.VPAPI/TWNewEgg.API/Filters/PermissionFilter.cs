using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.API.Filters
{
    public class PermissionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            controllerName += "Controller";
            string actionName = filterContext.ActionDescriptor.ActionName;
            string auth = filterContext.HttpContext.Request.Headers["Authorization"];
            string token = filterContext.HttpContext.Request.Headers["Token"];

            //Check UserID & AccessToken
            bool hasPermission = false;
            DB.TWBackendDBContext backendDB = new DB.TWBackendDBContext();
            DB.TWBACKENDDB.Models.API_User user = backendDB.API_User.Where(x => x.AccessToken == token && x.AuthKey == auth).FirstOrDefault();
            if (user != null)
            {
                //Check Permission
                DB.TWBACKENDDB.Models.API_Action action = backendDB.API_Action.Where(x => x.ControllerName.ToLower() == controllerName.ToLower() && x.ActionName.ToLower() == actionName.ToLower()).FirstOrDefault();
                if (action != null)
                {
                    DB.TWBACKENDDB.Models.API_Purview purview = backendDB.API_Purview.Where(x => x.UserID == user.UserID && x.ActionID == action.ActionID).FirstOrDefault();
                    if (purview != null)
                    {
                        if (purview.IsEnable())
                        {
                            hasPermission = true;
                        }
                    }
                }
            }
            if (hasPermission == false)
            {
                //Give it an Error Message as Response
                JsonResult jr = new JsonResult();
                jr.Data = "Permission Error";
                filterContext.Result = jr;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}