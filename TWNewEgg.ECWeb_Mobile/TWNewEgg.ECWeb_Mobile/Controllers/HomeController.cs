using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.ECWeb.PrivilegeFilters;
using TWNewEgg.ECWeb_Mobile.Auth;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.ViewModels.Advertising;

using TWNewEgg.Models.WebsiteMappingRules;

namespace TWNewEgg.ECWeb_Mobile.Controllers
{
    [AllowNonSecures]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            /*var auth = NEUser.IsAuthticated;
            var test = NEUser.Email;
            var test2 = NEUser.Browser;*/

            // 依據 BSATW-177 手機改版需求增加---------------add by bruce 20160701
            //1000	手機首頁輪播  
            TWNewEgg.ECWeb_Mobile.Controllers.Api.HomeSliderBannerController hsb = new Api.HomeSliderBannerController();
            List<TWNewEgg.Models.ViewModels.DeviceAd.HomeSilderBannerVM> list_hsb = hsb.Get();
            ////提供資料給reactjs的index.html時加入DEV_ImageServerUrl---------------add by bruce 20160704
            //foreach (var each_info in list_hsb)
            //    each_info.imgUrl = ImagesUrlChangerules.ImagesUrladddomainname(each_info.imgUrl);

            //1001	生活提案-大圖
            TWNewEgg.ECWeb_Mobile.Controllers.Api.HomeLifeProjectController hlp = new Api.HomeLifeProjectController();
            List<TWNewEgg.Models.ViewModels.DeviceAd.HomeLifeProjectVM> list_hlp = hlp.Get();
            ////提供資料給reactjs的index.html時加入DEV_ImageServerUrl---------------add by bruce 20160704
            //foreach (var each_info in list_hlp)
            //    each_info.sub.imgUrl = ImagesUrlChangerules.ImagesUrladddomainname(each_info.sub.imgUrl);


            //1004	美國直購
            TWNewEgg.ECWeb_Mobile.Controllers.Api.HomeShopUSController hsus = new Api.HomeShopUSController();
            List<TWNewEgg.Models.ViewModels.DeviceAd.HomeShopUsVM> list_hsus = hsus.Get();
            ////提供資料給reactjs的index.html時加入DEV_ImageServerUrl---------------add by bruce 20160704
            //foreach (var each_info in list_hsus)
            //    each_info.imgUrl = ImagesUrlChangerules.ImagesUrladddomainname(each_info.imgUrl);


            ViewBag.ListHomeSliderBanner = list_hsb;
            ViewBag.ListHomeLifeProject = list_hlp;
            ViewBag.ListHomeShopUS = list_hsus;
            // 依據 BSATW-177 手機改版需求增加---------------add by bruce 20160701


            // 依據 BSATW-177 手機改版需求增加---------------add by bruce 20160704
            //1002	全館分類---------------add by bruce 20160704
            TWNewEgg.ECWeb_Mobile.Controllers.Api.HomeCategoryMapController hcmap = new Api.HomeCategoryMapController();
            List<TWNewEgg.Models.ViewModels.DeviceAd.HomeCategoryMapVM> list_hcmap = hcmap.Get();            
            ////提供資料給reactjs的index.html時加入DEV_ImageServerUrl---------------add by bruce 20160704
            //foreach (var each_info in list_hcmap)
            //    each_info.imgUrl = ImagesUrlChangerules.ImagesUrladddomainname(each_info.imgUrl);

            ViewBag.ListHomeCategoryMap = list_hcmap;


            ////1003	促銷文案---------------add by bruce 20160704
            //TWNewEgg.ECWeb_Mobile.Controllers.Api.HomePromoProjectController hpp = new Api.HomePromoProjectController();
            //List<TWNewEgg.Models.ViewModels.DeviceAd.HomePromoProjectVM> list_hpp = hpp.Get();
            //ViewBag.ListHomePromoProject = list_hpp;


            return View();
        }
    }
}
