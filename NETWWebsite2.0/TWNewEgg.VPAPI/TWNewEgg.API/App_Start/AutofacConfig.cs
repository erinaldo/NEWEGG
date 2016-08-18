using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.WebApi;
using Autofac.Integration.Mvc;
using TWNewEgg.DAL.DbContextFactory;
using TWNewEgg.DAL.Interface;
using TWNewEgg.DAL.Repository;

namespace TWNewEgg.API
{
    public class AutofacStart
    {
        public static void Bootstrapper()
        {
            #region -- 公佈欄 --

            TWNewEgg.Framework.Autofac.AutofacConfig.builder.RegisterType(typeof(TWNewEgg.BulletinsMessageService.Service.BulletinsMessageService)).As(typeof(TWNewEgg.BulletinsMessageService.Interface.IBulletinsMessage));

            #endregion
            #region AuthTokenService
           
            #endregion
            TWNewEgg.Framework.Autofac.AutofacConfig.builder.RegisterGeneric(typeof(TWSqlRepository<>)).As(typeof(IRepository<>));
            TWNewEgg.Framework.Autofac.AutofacConfig.builder.RegisterGeneric(typeof(TWSELLERPORTALDBRepository<>)).As(typeof(ITWSELLERPORTALDBRepository<>));

            #region sellerportal Table RepoAdapter
            TWNewEgg.Framework.Autofac.AutofacConfig.builder.RegisterType(typeof(TWNewEgg.Seller_UserRepoAdapters.Seller_UserRepoAdapters))
                .As(typeof(TWNewEgg.Seller_UserRepoAdapters.Interface.ISeller_UserRepoAdapters));

            TWNewEgg.Framework.Autofac.AutofacConfig.builder.RegisterType(typeof(TWNewEgg.Seller_BasicInfoRepoAdapters.Seller_BasicInfoRepoAdapters))
                .As(typeof(TWNewEgg.Seller_BasicInfoRepoAdapters.Interface.ISeller_BasicInfoRepoAdapters));

            TWNewEgg.Framework.Autofac.AutofacConfig.builder.RegisterType(typeof(TWNewEgg.User_PurviewRepoAdapter.User_PurviewRepoAdapters))
                .As(typeof(TWNewEgg.User_PurviewRepoAdapters.Interface.IUser_PurviewRepoAdapters));

            TWNewEgg.Framework.Autofac.AutofacConfig.builder.RegisterType(typeof(TWNewEgg.Seller_PurviewRepoAdapter.Seller_PurviewRepoAdapters))
                .As(typeof(TWNewEgg.Seller_PurviewRepoAdapters.Interface.ISeller_PurviewRepoAdapters));

            TWNewEgg.Framework.Autofac.AutofacConfig.builder.RegisterType(typeof(TWNewEgg.Group_PurviewRepoAdapter.Group_PurviewRepoAdapters))
                .As(typeof(TWNewEgg.Group_PurviewRepoAdapters.Interface.IGroup_PurviewRepoAdapters));

            TWNewEgg.Framework.Autofac.AutofacConfig.builder.RegisterType(typeof(TWNewEgg.EDI_Seller_Function_LocalizedResRepoAdapters.EDI_Seller_Function_LocalizedResRepoAdapters))
                .As(typeof(TWNewEgg.EDI_Seller_Function_LocalizedResRepoAdapters.Interface.IEDI_Seller_Function_LocalizedResRepoAdapters));

            TWNewEgg.Framework.Autofac.AutofacConfig.builder.RegisterType(typeof(TWNewEgg.EDI_Seller_FunctionRepoAdapters.EDI_Seller_FunctionRepoAdapters))
                .As(typeof(TWNewEgg.EDI_Seller_FunctionRepoAdapters.Interface.IEDI_Seller_FunctionRepoAdapters));

            TWNewEgg.Framework.Autofac.AutofacConfig.builder.RegisterType(typeof(TWNewEgg.Seller_ActionRepoAdapters.Seller_ActionRepoAdapters))
                .As(typeof(TWNewEgg.Seller_ActionRepoAdapters.Interface.ISeller_ActionRepoAdapters));

            TWNewEgg.Framework.Autofac.AutofacConfig.builder.RegisterType(typeof(TWNewEgg.EDI_FunctionCategory_LocalizedResRepoAdapters.EDI_FunctionCategory_LocalizedResRepoAdapters))
                .As(typeof(TWNewEgg.EDI_FunctionCategory_LocalizedResRepoAdapters.Interface.IEDI_FunctionCategory_LocalizedResRepoAdapters));

            
            #endregion
            #region About Azure
            TWNewEgg.Framework.Autofac.AutofacConfig.builder.RegisterType(typeof(TWNewEgg.BackendService.Service.AzureService))
                .As(typeof(TWNewEgg.BackendService.Interface.IAzureService));
            TWNewEgg.Framework.Autofac.AutofacConfig.builder.RegisterType(typeof(TWNewEgg.StorageServices.AzureStorageService))
                .As(typeof(TWNewEgg.StorageServices.Interface.ICloudStorage));
            TWNewEgg.Framework.Autofac.AutofacConfig.builder.RegisterType(typeof(TWNewEgg.StorageServices.AzureCDNAdapter))
                .As(typeof(TWNewEgg.StorageServices.Interface.ICDNAdapter));
            TWNewEgg.Framework.Autofac.AutofacConfig.builder.RegisterType(typeof(TWNewEgg.PurgeQueueAdapters.PurgeQueueAdapters))
                .As(typeof(TWNewEgg.PurgeQueueAdapters.Interface.IPurgeQueueAdapters));
            #endregion
            #region Register DbContext
            string twSqlConnectStr = System.Configuration.ConfigurationManager.ConnectionStrings["TWSqlDBConnection"].ConnectionString;
            if (twSqlConnectStr != null)
            {
                TWNewEgg.Framework.Autofac.AutofacConfig.builder.RegisterType(typeof(DbContextFactory<TWNewEgg.Models.DBModels.TWSqlDBContext>))
                    .WithParameter("connectionString", twSqlConnectStr)
                    .As(typeof(IDbContextFactory<TWNewEgg.Models.DBModels.TWSqlDBContext>))
                    .InstancePerLifetimeScope();
            }

            string twBackendConnectStr = System.Configuration.ConfigurationManager.ConnectionStrings["TWBackendDBConnection"].ConnectionString;
            if (twBackendConnectStr != null)
            {
                TWNewEgg.Framework.Autofac.AutofacConfig.builder.RegisterType(typeof(DbContextFactory<TWNewEgg.Models.DBModels.TWBackendDBContext>))
                    .WithParameter("connectionString", twBackendConnectStr)
                    .As(typeof(IDbContextFactory<TWNewEgg.Models.DBModels.TWBackendDBContext>))
                    .InstancePerLifetimeScope();
            }
            string twSellerSqlConnectStr = System.Configuration.ConfigurationManager.ConnectionStrings["TWSellerPortalDBConnection"].ConnectionString;
            if (twSellerSqlConnectStr != null)
            {
                TWNewEgg.Framework.Autofac.AutofacConfig.builder.RegisterType(typeof(DbContextFactory<TWNewEgg.VendorModels.DBModels.TWSellerPortalDBContext>))
                    .WithParameter("connectionString", twSellerSqlConnectStr)
                    .As(typeof(IDbContextFactory<TWNewEgg.VendorModels.DBModels.TWSellerPortalDBContext>))
                    .InstancePerLifetimeScope();
            }

            #endregion

            TWNewEgg.Framework.Autofac.AutofacConfig.builder.RegisterControllers(Assembly.GetExecutingAssembly());
            TWNewEgg.Framework.Autofac.AutofacConfig.SetAutofacContainer(TWNewEgg.Framework.Autofac.AutofacConfig.builder.Build());
            DependencyResolver.SetResolver(new AutofacDependencyResolver(TWNewEgg.Framework.Autofac.AutofacConfig.GetAutofacComtainer()));

            
        }
    }
}