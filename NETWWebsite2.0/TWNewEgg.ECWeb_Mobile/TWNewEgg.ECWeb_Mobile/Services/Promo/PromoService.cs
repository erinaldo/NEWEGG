using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.Models.ViewModels.Login;
using TWNewEgg.Models.ViewModels.Account;
using TWNewEgg.ECWeb.PrivilegeFilters.Core;
using TWNewEgg.ECWeb.PrivilegeFilters.Models;
using TWNewEgg.Models.ViewModels.Register;
using TWNewEgg.Framework.HttpMethod;
using TWNewEgg.Framework.Common.Cryptography;
using TWNewEgg.Framework.ServiceApi.Configuration;
using System.Text.RegularExpressions;
using TWNewEgg.CookiesUtilities;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.ECWeb.PrivilegeFilters;

namespace TWNewEgg.ECWeb_Mobile.Services.PromoService
{
    public class PromoService
    {
        public static List<TWNewEgg.Models.ViewModels.DeviceAd.HomePromoProjectVM> GetPromoTop()
        {
            //1003	促銷文案---------------add by bruce 20160705
            TWNewEgg.ECWeb_Mobile.Controllers.Api.HomePromoProjectController hpp = new Controllers.Api.HomePromoProjectController();
            List<TWNewEgg.Models.ViewModels.DeviceAd.HomePromoProjectVM> list_hpp = new List<Models.ViewModels.DeviceAd.HomePromoProjectVM>();
            if (hpp.Get().Count > 0) list_hpp = hpp.Get();
            return list_hpp;
        }

    }

}