using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Autofac.Integration.WebApi;
using Autofac.Integration.Mvc;
using TWNewEgg.Framework.Autofac;
using System.Web.Mvc;
using System.Reflection;
using TWNewEgg.AccountEnprypt;
using TWNewEgg.AccountEnprypt.Core;
using TWNewEgg.AccountEnprypt.Interface;

namespace TWNewEgg.ECWeb_Mobile.App_Start
{
    public class AutofacStart
    {
        public static void Bootstrapper()
        {
            AutofacConfig.builder.RegisterType(typeof(AesService)).As(typeof(IAesService));
            AutofacConfig.builder.RegisterType(typeof(AesOld)).As(typeof(IAes));

            AutofacConfig.builder.RegisterFilterProvider();

            AutofacConfig.builder.RegisterControllers(Assembly.GetExecutingAssembly());
            AutofacConfig.SetAutofacContainer(AutofacConfig.builder.Build());
            DependencyResolver.SetResolver(new AutofacDependencyResolver(AutofacConfig.GetAutofacComtainer()));
        }
    }
}