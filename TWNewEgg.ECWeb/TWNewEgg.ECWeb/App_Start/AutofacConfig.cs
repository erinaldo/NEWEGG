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
using TWNewEgg.PaymentGateway.Service;
using TWNewEgg.PaymentGateway.Interface;
using TWNewEgg.HiTrustRepoAdapters;
using TWNewEgg.HiTrustRepoAdapters.Interface;
using TWNewEgg.DAL.Repository;
using TWNewEgg.DAL.Interface;
using System.Configuration;
using TWNewEgg.DAL.DbContextFactory;
using TWNewEgg.Models.DBModels;
using TWNewEgg.Website.ECWeb.Service;
using TWNewEgg.ECWeb.Services.OldCart.NeweggService;
using TWNewEgg.PaymentGateway.Service.NCCC;
using TWNewEgg.NCCCRepoAdapters;
using TWNewEgg.NCCCRepoAdapters.Interface;
using TWNewEgg.FileServices;
using TWNewEgg.FileServices.Interface;
using TWNewEgg.StorageServices.Interface;
using TWNewEgg.StorageServices;

namespace TWNewEgg.ECWeb.App_Start
{
    public class AutofacStart
    {
        public static void Bootstrapper()
        {
            AutofacConfig.builder.RegisterType(typeof(AesService)).As(typeof(IAesService));
            AutofacConfig.builder.RegisterType(typeof(AesOld)).As(typeof(IAes));
            AutofacConfig.builder.RegisterType(typeof(PlaceOrder)).As(typeof(IPlaceOrder));
            AutofacConfig.builder.RegisterType(typeof(FileService)).As(typeof(IFileService));
            AutofacConfig.builder.RegisterType(typeof(StorageService)).As(typeof(ICloudStorageAdapter));
            AutofacConfig.builder.RegisterType(typeof(AzureStorageService)).As(typeof(ICloudStorage));
            AutofacConfig.builder.RegisterType(typeof(AzureCDNAdapter)).As(typeof(ICDNAdapter));

            #region TWNewEgg.PaymentGateway
            AutofacConfig.builder.RegisterGeneric(typeof(TWSqlRepository<>)).As(typeof(IRepository<>));
            string twSqlConnectStr = ConfigurationManager.ConnectionStrings["TWSqlDBConnection"].ConnectionString;
            if (twSqlConnectStr != null)
            {
                AutofacConfig.builder.RegisterType(typeof(DbContextFactory<TWSqlDBContext>))
                    .WithParameter("connectionString", twSqlConnectStr)
                    .As(typeof(IDbContextFactory<TWSqlDBContext>))
                    .InstancePerLifetimeScope();
            }
            AutofacConfig.builder.RegisterType(typeof(AllPaymentService)).As(typeof(IAllPay));
            AutofacConfig.builder.RegisterType(typeof(NCCCPaymentService)).As(typeof(INCCC));
            AutofacConfig.builder.RegisterType(typeof(HiTrustPaymentService)).As(typeof(IHiTrust));
            AutofacConfig.builder.RegisterType(typeof(HiTrustRepoAdapter)).As(typeof(IHiTrustRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(NCCCRepoAdapter)).As(typeof(INCCCRepoAdapter));
            #endregion

            AutofacConfig.builder.RegisterFilterProvider();

            AutofacConfig.builder.RegisterControllers(Assembly.GetExecutingAssembly());
            AutofacConfig.SetAutofacContainer(AutofacConfig.builder.Build());
            DependencyResolver.SetResolver(new AutofacDependencyResolver(AutofacConfig.GetAutofacComtainer()));
        }
    }
}