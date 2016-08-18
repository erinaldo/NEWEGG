using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.Framework.ServiceApi.Configuration;

namespace TWNewEgg.ECWeb_Mobile.App_Start
{
    public class ServiceApiConfig
    {
        public static void Bootstrapper()
        {
            ConfigurationManager.InitServiceApi();
        }
    }
}