using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.ECWeb.PrivilegeFilters;

namespace TWNewEgg.ECWeb.PrivilegeFilters
{
    public class RequireSecures : RequireHttpsAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                if (!CheckSecures.CheckSSLConnection(filterContext.HttpContext))
                {
                    string url = GetSSLUrl(filterContext.HttpContext);
                    //filterContext.Result = new RedirectResult(url);
                    filterContext.HttpContext.Response.Redirect(url);
                    base.OnAuthorization(filterContext);
                    return;
                }
            }

            bool skipSecures = filterContext.ActionDescriptor.IsDefined(typeof(AllowNonSecuresAttribute), inherit: true)
                                     || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowNonSecuresAttribute), inherit: true);

            if (skipSecures)
            {
                return;
            }

            if (!CheckSecures.CheckSSLConnection(filterContext.HttpContext))
            {
                string url = GetSSLUrl(filterContext.HttpContext);
                //filterContext.Result = new RedirectResult(url);
                filterContext.HttpContext.Response.Redirect(url);
                base.OnAuthorization(filterContext);
                return;
            }
            #region Void Local
            //if (!filterContext.HttpContext.Request.IsLocal)
            //{
            //    // when connection to the application is local, don't do any HTTPS stuff
            //    if (!CheckSecures.CheckSSLConnection(filterContext.HttpContext))
            //    {
            //        string url = GetSSLUrl(filterContext.HttpContext);
            //        filterContext.Result = new RedirectResult(url);
            //        return;
            //    }
            //}
            //else
            //{
            //    if (!CheckSecures.CheckSSLConnection(filterContext.HttpContext))
            //    {
            //        string url = GetSSLUrl(filterContext.HttpContext);
            //        filterContext.Result = new RedirectResult(url);
            //        return;
            //    }
            //}
            #endregion
        }
        private string GetSSLUrl(HttpContextBase httpContext)
        {
            int sslPort = int.Parse(System.Configuration.ConfigurationManager.AppSettings["ECWebHttpsPort"]);
            return "https://" +
                        System.Configuration.ConfigurationManager.AppSettings["ECWebHttpsDomain"] +
                        ((sslPort == 443 ? "" : string.Format(":{0}", sslPort.ToString()))) +
                        httpContext.Request.RawUrl;
        }
    }
}
