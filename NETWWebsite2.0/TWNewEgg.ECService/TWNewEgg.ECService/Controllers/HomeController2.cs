using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.Framework.Autofac;
using TWNewEgg.CartRepoAdapters.Interface;

namespace TWNewEgg.ECService.Controllers
{
    public partial class HomeController : Controller
    {
        //
        // GET: /Home/
  
        public ActionResult Index2()
        {

            //var service = AutofacConfig.Container.Resolve<TWNewEgg.Discard4Services.Interface.IDiscard4ItemService>();
            //string salesorderCode="LBO160422000028";
            //string user_name = "bh96";
            //List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM> list = service.InitData(salesorderCode, user_name);


            //var service = AutofacConfig.Container.Resolve<TWNewEgg.GreetingWordsServices.Interface.IHolidayGreetingWordsService>();
            //List<TWNewEgg.Models.DomainModels.GreetingWords.HolidayGreetingWordsDM> list = service.GetAll();

            //var service2 = AutofacConfig.Container.Resolve<TWNewEgg.GreetingWordsServices.Interface.IHomeGreetingWordsService>();
            //List<TWNewEgg.Models.DomainModels.GreetingWords.HomeGreetingWordsDM> list2 = service2.GetAll();


            ////test------------add by bruce 20160614
            //var service = AutofacConfig.Container.Resolve<TWNewEgg.DeviceAdServices.Interface.IDeviceAdSetService>();
            //List<TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdSetDM> list = service.GetAll();

            //var service3 = AutofacConfig.Container.Resolve<TWNewEgg.DeviceAdServices.Interface.IDeviceAdContentService>();
            //List<TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdContentDM> list3 = service3.GetAll();
            ////test------------add by bruce 20160614

            var search_container = new TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdSearchDM();
            search_container.ShowAll = "show";
           

            //取得手機版廣告目錄
            var service = AutofacConfig.Container.Resolve<TWNewEgg.DeviceAdServices.Interface.IDeviceAdSetService>();            
            //search_container.DeviceAdSetIDs.Add(0);
            search_container.Flag = "phone";
            search_container.ShowAll = "show";
            var list = service.GetMenu(search_container);
            //取得手機版廣告內容
            var service2 = AutofacConfig.Container.Resolve<TWNewEgg.DeviceAdServices.Interface.IDeviceAdContentService>();

            foreach (var each_info in list.ListAdMenu) {

                search_container.DeviceAdSetIDs.Clear();
                search_container.DeviceAdSetIDs.Add(each_info.ID);
                search_container.Flag = string.Empty;
                search_container.ShowAll = "show";
                var list_each_result = service2.GetShow(search_container);

                //foreach (var each_info2 in list_each_result.ListAdContent)
                //    Response.Write(each_info2.Name + "\r");

            }

            //search_container.DeviceAdSetIDs.Clear();
            //search_container.DeviceAdSetIDs.Add(1007);
            //search_container.Flag = string.Empty;
            //var list_content = service2.GetShow(search_container);

            Response.Write(Json(list, JsonRequestBehavior.AllowGet).ToString());

            return View();
        }


        public ActionResult Index3()
        {

            var search_container = new TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceSearchDM();
            search_container.OrderByID = true;
            //search_container.FinanStatus = "I";
            search_container.SellerIDs.Add(407);

            var service = AutofacConfig.Container.Resolve<TWNewEgg.SellerServices.Interface.ISellerCorrectionPriceService>();
            var list = service.GetGroupBy(search_container);

            Response.Write(Json(list, JsonRequestBehavior.AllowGet).ToString());

            return View();
        }
       
    }
}
