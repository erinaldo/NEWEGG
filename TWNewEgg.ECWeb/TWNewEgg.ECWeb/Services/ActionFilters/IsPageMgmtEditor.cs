using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TWNewEgg.ECWeb.Auth;

namespace TWNewEgg.ECWeb.ActionFilters
{
    public class IsPageMgmtEditor: ActionFilterAttribute
    {
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                string[] validEmails = System.Configuration.ConfigurationManager.AppSettings["PageMgmtEditorList"].Split(',');
                if (!validEmails.Any(NEUser.Email.Contains))
                {
                    filterContext.Result = GetResult(filterContext);
                }
            }
            catch (Exception e)
            {
                filterContext.Result = new RedirectResult("/");
                logger.Error(e.ToString());
            }
        }

        private ActionResult GetResult(ActionExecutingContext filterContext)
        {
            string noPermissionMsg = "您沒有權限使用此功能。";
            ActionResult result = new ContentResult { Content = noPermissionMsg };
            string actionName = filterContext.ActionDescriptor.ActionName;
            Type controllerType = filterContext.Controller.GetType();
            var method = controllerType.GetMethod(actionName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            var returnType = method.ReturnType;
            if (returnType.Equals(typeof(JsonResult)))
            {
                result = new JsonResult
                {
                    Data = new { msg = noPermissionMsg}
                };
            }
            else if (returnType.Equals(typeof(ContentResult)))
            {
                result = new ContentResult
                {
                    Content = noPermissionMsg
                };
            }
            else if(returnType.Equals(typeof(ActionResult)))
            {
                result = new RedirectResult("/");
            }

            return result;
        }
    }
}
