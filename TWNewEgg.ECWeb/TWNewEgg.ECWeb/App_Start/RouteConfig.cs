using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TWNewEgg.ECWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Error",
                url: "Opps/404",
                defaults: new { controller = "Error", action = "Error" }
            );

            routes.MapRoute(
                name: "AwardList",
                url: "Activity/AwardList",
                defaults: new { controller = "Activity", action = "AwardList" }
            );

            routes.MapRoute(
                name: "GetAwardList",
                url: "Activity/GetAwardList",
                defaults: new { controller = "Activity", action = "GetAwardList" }
            );

            routes.MapRoute(
                name: "ActivityIndex",
                url: "Activity/Index",
                defaults: new { controller = "Activity", action = "Index" }
            );
            
            routes.MapRoute(
               name: "AwardDetial",
               url: "Activity/AwardDetial",
               defaults: new { controller = "Activity", action = "AwardDetial" }
           );

            routes.MapRoute(
                name: "Activity",
                url: "Activity/{name}",
                defaults: new { controller = "Activity", action = "Show" }
            );

            routes.MapRoute(
                name: "Act",
                url: "Act/{name}",
                defaults: new { controller = "Act", action = "Deal" }
            );

            routes.MapRoute(
                name: "landingpage",
                url: "p/{Path}",
                defaults: new { controller = "Designer", action = "landingpage", Path = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "PageMgmt",
                url: "Designer/{action}/{Path}/{isEditPage}",
                defaults: new { controller = "Designer", action = "Index", Path = UrlParameter.Optional, isEditPage = UrlParameter.Optional }
            );
        }
    }
}
