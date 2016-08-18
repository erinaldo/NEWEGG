using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.ECWeb_Mobile
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new TWNewEgg.ECWeb.PrivilegeFilters.AccountAuthorizeAttribute());
            filters.Add(new TWNewEgg.ECWeb.PrivilegeFilters.RequireSecures());
        }
    }
}