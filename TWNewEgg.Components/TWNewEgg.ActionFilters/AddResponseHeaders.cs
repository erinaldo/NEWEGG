using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TWNewEgg.Framework.Cache;
using TWNewEgg.ActionFilters.Model;

namespace TWNewEgg.ActionFilters
{
    public class AddResponseHeaders : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (filterContext != null)
            {
                var responseHeads = CacheConfiguration.Instance.GetFromCache<VarnishHeads>(FilterConst.VARNISHHEADLOC, null, false);
                if (responseHeads != null && responseHeads.varnishHeads != null && responseHeads.varnishHeads.Count > 0)
                {
                    var routeData = filterContext.RouteData;
                    string controllerName = routeData.GetRequiredString("controller");
                    string actionName = routeData.GetRequiredString("action");
                    var currentContrAction = responseHeads.varnishHeads.Where(x => x.controllerName == controllerName && x.actionName == actionName).FirstOrDefault();
                    if (currentContrAction != null &&
                        currentContrAction.responseHeads != null &&
                        currentContrAction.responseHeads.Count > 0)
                    {
                        for (int i = 0; i < currentContrAction.responseHeads.Count; i++)
                        {
                            filterContext.HttpContext.Response.AppendHeader(currentContrAction.responseHeads[i].headName, currentContrAction.responseHeads[i].headValue);
                        }
                    }
                }
            }
            base.OnResultExecuted(filterContext);
        }
    }
}
