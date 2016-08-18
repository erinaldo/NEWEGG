using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace TWNewEgg.ECWeb_Mobile
{
    public class ApiFilterConfig
    {
        public static void RegisterGlobalApiFilters(HttpFilterCollection filters)
        {
            filters.Add(new TWNewEgg.ECWeb.PrivilegeFilters.Api.RequireHttpsAndAuthsAttribute());
        }
    }
}