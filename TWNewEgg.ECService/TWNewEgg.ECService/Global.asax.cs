using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TWNewEgg.ECService.App_Start;

namespace TWNewEgg.ECService
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            System.Data.Entity.Database.SetInitializer<TWNewEgg.Models.DBModels.TWSqlDBContext>(null);
            System.Data.Entity.Database.SetInitializer<TWNewEgg.Models.DBModels.TWBackendDBContext>(null);
            System.Data.Entity.Database.SetInitializer<TWNewEgg.DB.TWSqlDBContext>(null);
            System.Data.Entity.Database.SetInitializer<TWNewEgg.DB.TWBackendDBContext>(null);

            //Log4Net 設定檔
            string log4netPath = Server.MapPath("~/Configurations/log4net.config");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(log4netPath));

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutofacStart.Bootstrapper();
            AutoMapperConfig.Bootstrapper();
            ServiceApiConfig.Bootstrapper();
        }
    }
}
