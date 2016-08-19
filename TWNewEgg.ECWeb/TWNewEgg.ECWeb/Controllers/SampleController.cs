using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.ECWeb.PrivilegeFilters;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DomainModels.Home;
using TWNewEgg.Models.DomainModels.MobileStore;
using TWNewEgg.Models.DomainModels.Product;
using TWNewEgg.Models.DomainModels.Store;
using TWNewEgg.Models.ViewModels.Product;
using TWNewEgg.Models.DomainModels.News;
using TWNewEgg.Models.DomainModels.Media;
using TWNewEgg.Models.DomainModels.GroupBuy;

namespace TWNewEgg.ECWeb.Controllers
{
    [AllowNonSecures]
    //[AllowAnonymous]
    public class SampleController : Controller
    {
        //
        // GET: /Sample/
        public ActionResult Test(int number)
        {
            ViewBag.TestStart = DateTime.Now;
            ViewBag.test = Processor.Request<List<ProductDetail>, List<ProductDetailDM>>("TestService", "report", number).results;
            ViewBag.TestEnd = DateTime.Now;
            return View();
        }
        public ActionResult Test2(int number)
        {
            ViewBag.TestStart = DateTime.Now;
            ViewBag.test = Processor.Request<List<ProductDetail>, List<ProductDetailDM>>("TestService2", "report", number).results;
            ViewBag.TestEnd = DateTime.Now;
            return View("Test");
        }
        public ActionResult Test3(string comID, string methodName)
        {
            ViewBag.TestStart = DateTime.Now;
            ViewBag.test = Processor.Request<string, string>(comID, methodName).results;
            ViewBag.TestEnd = DateTime.Now;
            return View();
        }

        public string MultiRequest(int number)
        {

            var testtt = TWNewEgg.Framework.ServiceApi.Processor.Request<List<ProductDetail>, List<ProductDetailDM>>("TestService", "report", number);

            //int a = 5;
            //string b = "total : ";
            //List<int> aaa = new List<int>() { 0, 2, 3 };
            //TWNewEgg.Models.ViewModels.Product.ProductDetail pp = new Models.ViewModels.Product.ProductDetail();
            //pp.ManufactureID = 999;
            //pp.Name = "3roijogiorjgrweg";
            //List<TWNewEgg.Models.ViewModels.Product.ProductDetail> test = new List<TWNewEgg.Models.ViewModels.Product.ProductDetail>();
            //TWNewEgg.Models.ViewModels.Product.ProductDetail test1 = new TWNewEgg.Models.ViewModels.Product.ProductDetail();
            //test1.ManufactureID = 8888;
            //test1.Name = "654948321389981";
            //TWNewEgg.Models.ViewModels.Product.ProductDetail test2 = new TWNewEgg.Models.ViewModels.Product.ProductDetail();
            //test2.ManufactureID = 9849846;
            //test2.Name = "asegfwgrwegherg";
            //test.Add(test1);
            //test.Add(test2);
            //var testtt = TWNewEgg.Framework.ServiceApi.Processor.Request<string, string>("TestService", "test", a, b, aaa, pp);
            return testtt.error + " - results: " + testtt.results.Count;
        }
        public string MultiRequest2(int number)
        {

            int a = number;
            string b = "total : ";
            List<int> aaa = new List<int>() { 0, 2, 3 };
            TWNewEgg.Models.ViewModels.Product.ProductDetail pp = new TWNewEgg.Models.ViewModels.Product.ProductDetail();
            pp.ManufactureID = 999;
            pp.Name = "3roijogiorjgrweg";
            List<TWNewEgg.Models.ViewModels.Product.ProductDetail> test = new List<TWNewEgg.Models.ViewModels.Product.ProductDetail>();
            TWNewEgg.Models.ViewModels.Product.ProductDetail test1 = new TWNewEgg.Models.ViewModels.Product.ProductDetail();
            test1.ManufactureID = 8888;
            test1.Name = "654948321389981";
            TWNewEgg.Models.ViewModels.Product.ProductDetail test2 = new TWNewEgg.Models.ViewModels.Product.ProductDetail();
            test2.ManufactureID = 9849846;
            test2.Name = "asegfwgrwegherg";
            test.Add(test1);
            test.Add(test2);
            var testtt = TWNewEgg.Framework.ServiceApi.Processor.Request<List<TWNewEgg.Models.DomainModels.Product.ProductDetailDM>, List<TWNewEgg.Models.DomainModels.Product.ProductDetailDM>>("TestService", "test5", a, b, aaa, test);
            return testtt.error + " - results: " + testtt.results.Count;
        }
        public string MultiRequest3(int number)
        {

            var testtt = TWNewEgg.Framework.ServiceApi.Processor.Request<List<ProductDetail>, List<ProductDetailDM>>("TestService", "report2", number);

            //int a = 5;
            //string b = "total : ";
            //List<int> aaa = new List<int>() { 0, 2, 3 };
            //TWNewEgg.Models.ViewModels.Product.ProductDetail pp = new Models.ViewModels.Product.ProductDetail();
            //pp.ManufactureID = 999;
            //pp.Name = "3roijogiorjgrweg";
            //List<TWNewEgg.Models.ViewModels.Product.ProductDetail> test = new List<TWNewEgg.Models.ViewModels.Product.ProductDetail>();
            //TWNewEgg.Models.ViewModels.Product.ProductDetail test1 = new TWNewEgg.Models.ViewModels.Product.ProductDetail();
            //test1.ManufactureID = 8888;
            //test1.Name = "654948321389981";
            //TWNewEgg.Models.ViewModels.Product.ProductDetail test2 = new TWNewEgg.Models.ViewModels.Product.ProductDetail();
            //test2.ManufactureID = 9849846;
            //test2.Name = "asegfwgrwegherg";
            //test.Add(test1);
            //test.Add(test2);
            //var testtt = TWNewEgg.Framework.ServiceApi.Processor.Request<string, string>("TestService", "test", a, b, aaa, pp);
            return testtt.error + " - results: " + testtt.results.Count;
        }

        public ActionResult TestDaniel()
        {
            //List<int> indexList = null;
            //indexList.Add(0);
            //var ret = TWNewEgg.Framework.ServiceApi.Processor.Request<StoreInfo, StoreInfo>("StoreService", "GetStoreInfo", 737, indexList);
            //var ret2 = TWNewEgg.Framework.ServiceApi.Processor.Request<List<ShopWindow>, List<ShopWindow>>("StoreService", "GetShopWindows", 737, null);
            //var ret3 = TWNewEgg.Framework.ServiceApi.Processor.Request<OptionStoreInfo, OptionStoreInfo>("StoreService", "GetOptionStoreInfo", 1100, 0, 0, "", "PriceAsc");
            //var ret4 = TWNewEgg.Framework.ServiceApi.Processor.Request<OptionStoreListZone, OptionStoreListZone>("StoreService", "GetOptionStoreListZone", 1100, 0, 0, "", "");
            //var ret5 = TWNewEgg.Framework.ServiceApi.Processor.Request<List<OptionStoreItemCell>, List<OptionStoreItemCell>>("StoreService", "GetOptionStoreItems", indexList);
            //var ret6 = TWNewEgg.Framework.ServiceApi.Processor.Request<HomeStoreInfo, HomeStoreInfo>("HomeStoreService", "GetHomeStoreInfo", indexList);
            //var ret7 = TWNewEgg.Framework.ServiceApi.Processor.Request<MStoreInfo, MStoreInfo>("MobileStoreService", "GetMobileStoreInfo", 735, 0);
            var ret8 = TWNewEgg.Framework.ServiceApi.Processor.Request<List<MStoreItemCell>, List<MStoreItemCell>>("MobileStoreService", "GetMobileStoreItems", 735, 0);

            //ViewBag.Result = ret.results;
            ViewBag.Result = null;
            return View();
        }

        //public ActionResult TestYiting(int? ID)
        public ActionResult TestYiting(int? ID, int? ID2, int? m = 0)
        {

            switch (m)
            {
                case 0:
                    var ret = TWNewEgg.Framework.ServiceApi.Processor.Request<List<MStoreItemCell>, List<MStoreItemCell>>("MobileStoreService", "GetMobileStoreItems", ID, ID2);
                    ViewBag.Result = ret.results;
                    break;
                case 1:
                    var ret1 = TWNewEgg.Framework.ServiceApi.Processor.Request<MStoreInfo, MStoreInfo>("MobileStoreService", "GetMobileStoreInfo", ID, 0);
                    ViewBag.Result = ret1.results;
                    break;
                case 2:
                    int itemID = 57377;
                    var ret2 = Processor.Request<List<GroupBuyViewInfo>, List<GroupBuyViewInfo>>("GroupBuyService", "QueryViewInfo", new GroupBuyQueryCondition { ItemID = itemID, PageNumber = 1, PageSize = 1000 }, ("1_1"));
                    //var ret2 = Processor.Request<List<GroupBuyViewInfo>, List<GroupBuyViewInfo>>("GroupBuyService", "QueryViewInfo", new GroupBuyQueryCondition { GroupBuyID = 439, PageNumber = 1, PageSize = 100 }, ("1_1"));
                    ViewBag.Result = ret2.results;
                    //if (Object.ReferenceEquals(ret2.results.GetType(), new List<GroupBuyViewInfo>().GetType())) itemID = 1;
                    break;
            }

            //List<int> indexList = new List<int>();
            //indexList.Add(1);
            //var ret2 = TWNewEgg.Framework.ServiceApi.Processor.Request<List<MStoreItemCell>, List<MStoreItemCell>>("MobileStoreService", "GetMobileStoreItems", 7, ID);
            //var ret2 = TWNewEgg.Framework.ServiceApi.Processor.Request<List<ShopWindow>, List<ShopWindow>>("MobileStoreService", "GetShopWindows", 737, new List<int> { 1, 2 });
            //var ret2 = TWNewEgg.Framework.ServiceApi.Processor.Request<MediaListInfo, MediaListInfo>("MediaService", "GetMediaList", 1);
            //var ret2 = TWNewEgg.Framework.ServiceApi.Processor.Request<NewsListInfo, NewsListInfo>("NewsService", "GetNewsList", 1);
            //var ret2 = TWNewEgg.Framework.ServiceApi.Processor.Request<NewsInfo, NewsInfo>("NewsService", "GetNewsInfo", 3);
            //var ret2 = TWNewEgg.Framework.ServiceApi.Processor.Request<NewsCell, NewsCell>("NewsService", "GetNewsCell", ID);
            //var ret2 = TWNewEgg.Framework.ServiceApi.Processor.Request<StoreInfo, StoreInfo>("StoreService", "GetStoreInfo", 737, null);
            //var ret2 = TWNewEgg.Framework.ServiceApi.Processor.Request<List<ShopWindow>, List<ShopWindow>>("StoreService", "GetShopWindows", 737, new List<int> { 1, 2 });

            //ViewBag.Result = ret2.results;
            //ViewBag.Result = null;
            return View();
        }

        public ActionResult Index()
        {
            int a = 5;
            string b = "total : ";
            List<int> aaa = new List<int>() { 0, 2, 3 };
            TWNewEgg.Models.ViewModels.Product.ProductDetail pp = new TWNewEgg.Models.ViewModels.Product.ProductDetail();
            pp.ManufactureID = 999;
            pp.Name = "3roijogiorjgrweg";
            List<TWNewEgg.Models.ViewModels.Product.ProductDetail> test = new List<TWNewEgg.Models.ViewModels.Product.ProductDetail>();
            TWNewEgg.Models.ViewModels.Product.ProductDetail test1 = new TWNewEgg.Models.ViewModels.Product.ProductDetail();
            test1.ManufactureID = 8888;
            test1.Name = "654948321389981";
            TWNewEgg.Models.ViewModels.Product.ProductDetail test2 = new TWNewEgg.Models.ViewModels.Product.ProductDetail();
            test2.ManufactureID = 9849846;
            test2.Name = "asegfwgrwegherg";
            test.Add(test1);
            test.Add(test2);
            Random randdd = new Random();
            List<ComplexProduntDM> test3 = new List<ComplexProduntDM>();
            for (int i = 0; i < 5; i++)
            {
                ComplexProduntDM newPDM = new ComplexProduntDM();
                newPDM.byteTest = Convert.ToByte(randdd.Next(00, 99));
                newPDM.shortTest = Convert.ToInt16(randdd.Next(00, 99));
                newPDM.intTest = randdd.Next(00, 99);
                newPDM.longTest = Convert.ToInt64(randdd.Next(00, 99));
                newPDM.floatTest = Convert.ToSingle(randdd.Next(00, 99));
                newPDM.doubleTest = Convert.ToDouble(randdd.Next(00, 99));
                newPDM.decimalTest = Convert.ToDecimal(randdd.Next(00, 99));
                newPDM.stringTest = randdd.Next(00, 99).ToString() + randdd.Next(00, 99).ToString() + randdd.Next(00, 99).ToString() + randdd.Next(00, 99).ToString() + randdd.Next(00, 99).ToString();
                newPDM.boolTest = Convert.ToBoolean(randdd.Next(0, 1));
                newPDM.dateTimeTest = DateTime.Now;

                newPDM.byteNullTest = null;
                newPDM.shortNullTest = null;
                newPDM.intNullTest = null;
                newPDM.longNullTest = null;
                newPDM.floatNullTest = null;
                newPDM.doubleNullTest = null;
                newPDM.decimalNullTest = null;
                newPDM.boolNullTest = null;
                newPDM.dateTimeNullTest = null;

                newPDM.complexProductDMTest = new List<ComplexProduntDM>();
                newPDM.complexProductDMNullTest = null;

                ComplexProduntDM newPDM2 = new ComplexProduntDM();
                newPDM2.byteTest = Convert.ToByte(randdd.Next(00, 99));
                newPDM2.shortTest = Convert.ToInt16(randdd.Next(00, 99));
                newPDM2.intTest = randdd.Next(00, 99);
                newPDM2.longTest = Convert.ToInt64(randdd.Next(00, 99));
                newPDM2.floatTest = Convert.ToSingle(randdd.Next(00, 99));
                newPDM2.doubleTest = Convert.ToDouble(randdd.Next(00, 99));
                newPDM2.decimalTest = Convert.ToDecimal(randdd.Next(00, 99));
                newPDM2.stringTest = randdd.Next(00, 99).ToString() + randdd.Next(00, 99).ToString() + randdd.Next(00, 99).ToString() + randdd.Next(00, 99).ToString() + randdd.Next(00, 99).ToString();
                newPDM2.boolTest = Convert.ToBoolean(randdd.Next(0, 1));
                newPDM2.dateTimeTest = DateTime.Now;

                newPDM2.byteNullTest = null;
                newPDM2.shortNullTest = null;
                newPDM2.intNullTest = null;
                newPDM2.longNullTest = null;
                newPDM2.floatNullTest = null;
                newPDM2.doubleNullTest = null;
                newPDM2.decimalNullTest = null;
                newPDM2.boolNullTest = null;
                newPDM2.dateTimeNullTest = null;

                newPDM2.complexProductDMTest = new List<ComplexProduntDM>();
                newPDM2.complexProductDMNullTest = null;

                newPDM.complexProductDMTest.Add(newPDM2);
                test3.Add(newPDM);
            }
            //var c = new { e = 9, f = "10" };
            ViewBag.ccTimeStart = DateTime.Now;
            ViewBag.cc = TWNewEgg.Framework.ServiceApi.Processor.Request<string, string>("TestService", "test", a, b, aaa, pp).results;
            ViewBag.ccTimeEnd = DateTime.Now;
            ViewBag.ddTimeStart = DateTime.Now;
            ViewBag.dd = TWNewEgg.Framework.ServiceApi.Processor.Request<List<string>, List<string>>("TestService", "test2").results;
            ViewBag.ddTimeEnd = DateTime.Now;
            ViewBag.eeTimeStart = DateTime.Now;
            ViewBag.ee = TWNewEgg.Framework.ServiceApi.Processor.Request<TWNewEgg.Models.DomainModels.Product.ProductDetailDM, TWNewEgg.Models.DomainModels.Product.ProductDetailDM>("TestService", "test3", a, b, aaa, pp).results;
            ViewBag.eeTimeEnd = DateTime.Now;
            ViewBag.ffTimeStart = DateTime.Now;
            ViewBag.ff = TWNewEgg.Framework.ServiceApi.Processor.Request<List<TWNewEgg.Models.DomainModels.Product.ProductDetailDM>, List<TWNewEgg.Models.DomainModels.Product.ProductDetailDM>>("TestService", "test4", a, b, aaa, pp).results;
            ViewBag.ffTimeEnd = DateTime.Now;
            ViewBag.ggTimeStart = DateTime.Now;
            ViewBag.gg = TWNewEgg.Framework.ServiceApi.Processor.Request<List<TWNewEgg.Models.DomainModels.Product.ProductDetailDM>, List<TWNewEgg.Models.DomainModels.Product.ProductDetailDM>>("TestService", "test5", a, b, aaa, test).results;
            ViewBag.ggTimeEnd = DateTime.Now;
            ViewBag.hhTimeStart = DateTime.Now;
            ViewBag.hh = TWNewEgg.Framework.ServiceApi.Processor.Request<string, string>("TestService", "test6", null).results;
            ViewBag.hhTimeEnd = DateTime.Now;
            ViewBag.iiTimeStart = DateTime.Now;
            ViewBag.ii = TWNewEgg.Framework.ServiceApi.Processor.Request<List<TWNewEgg.Models.DomainModels.Product.ComplexProduntDM>, List<TWNewEgg.Models.DomainModels.Product.ComplexProduntDM>>("TestService", "test7", a, b, aaa, test3).results;
            ViewBag.iiTimeEnd = DateTime.Now;
            //var ee = TWNewEgg.Framework.ServiceApi.Processor.Request<TWNewEgg.Models.ViewModels.Product.ProductDetail, TWNewEgg.Models.DomainModels.Product.ProductDetailDM>("TWNewEgg.Services.TestService", "test3", a, b, aaa, pp);
            //var ff = TWNewEgg.Framework.ServiceApi.Processor.Request<List<TWNewEgg.Models.ViewModels.Product.ProductDetail>, List<TWNewEgg.Models.DomainModels.Product.ProductDetailDM>>("TWNewEgg.Services.TestService", "test4", a, b, aaa, pp);
            //var gg = TWNewEgg.Framework.ServiceApi.Processor.Request<List<TWNewEgg.Models.ViewModels.Product.ProductDetail>, List<TWNewEgg.Models.DomainModels.Product.ProductDetailDM>>("TWNewEgg.Services.TestService", "test5", a, b, aaa, test);
            return View();
        }

        public string aa { get; set; }
        public string bb { get; set; }

    }
}
