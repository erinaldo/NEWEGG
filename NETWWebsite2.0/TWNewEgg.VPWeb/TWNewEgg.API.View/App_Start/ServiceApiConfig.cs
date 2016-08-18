using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.Framework.ServiceApi.Configuration;

namespace TWNewEgg.API.View
{
    public class ServiceApiConfig
    {
        public static void Bootstrapper()
        {
            ConfigurationManager.InitServiceApi();
        }
    }
}