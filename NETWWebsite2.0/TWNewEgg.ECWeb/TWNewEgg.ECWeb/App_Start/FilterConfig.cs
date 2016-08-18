using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.ECWeb
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new PrivilegeFilters.AccountAuthorizeAttribute());
            filters.Add(new PrivilegeFilters.RequireSecures());
            filters.Add(new TWNewEgg.ActionFilters.AddResponseHeaders());
        }
    }
}