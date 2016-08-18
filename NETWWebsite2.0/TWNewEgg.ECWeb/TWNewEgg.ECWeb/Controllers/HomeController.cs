using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.ECWeb.Auth;
using TWNewEgg.ECWeb.PrivilegeFilters;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.ViewModels;
using TWNewEgg.Models.DomainModels.Home;
using TWNewEgg.Models.DomainModels.Store;


namespace TWNewEgg.ECWeb.Controllers
{
    [AllowNonSecures]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            var auth = NEUser.IsAuthticated;
            var test = NEUser.Email;
            var test2 = NEUser.Browser;
           
            //ViewBag.test = Processor.Request<List<TWNewEgg.Models.ViewModels.Advertising.AdvEvent>, List<TWNewEgg.Models.DomainModels.Advertising.AdvEvent>>("Service.AdvEventDbService", "GetAdvEventByIDs", ids).results;
            return View();
        }

        /// <summary>
        /// 首頁六大主題場館區塊.
        /// </summary>
        /// <returns>首頁六大主題場館頁面.</returns>
        public PartialViewResult ShopWindowArea()
        {
            List<int> index = null;
            List<HomeShopWindow> viewModel = Processor.Request<List<HomeShopWindow>, List<HomeShopWindow>>("HomeStoreService", "GetHomeShopWindows", index).results;

            return PartialView(viewModel);
        }

        public ActionResult SubCategory()
        {
            return View();
        }

        /// <summary>
        /// 取得首頁TopBanner
        /// </summary>
        /// <returns>PartialView</returns>
        public ActionResult GetTopAdvertise()
        {
            List<TWNewEgg.Models.ViewModels.Advertising.AdvEventType> listAdvEventType = null;
            List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay> listAdvEventDisplay = null;
            Dictionary<int, Dictionary<TWNewEgg.Models.ViewModels.Advertising.AdvEventType, List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay>>> objDictResult = null;
            Dictionary<TWNewEgg.Models.ViewModels.Advertising.AdvEventType, List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay>> objDictItem = null;
            int numSort = 0;

            //根據AdvTypeCode取得所有AdvType的列表
            listAdvEventType = Processor.Request<List<TWNewEgg.Models.ViewModels.Advertising.AdvEventType>, List<TWNewEgg.Models.DomainModels.Advertising.AdvEventType>>("Service.AdvEventTypeReposity", "GetActiveAdvEventTypeListByAdvType", 3).results;
            if (listAdvEventType != null)
            {
                //將AdvEventType以Country欄位作排序
                listAdvEventType = listAdvEventType.OrderBy(x => x.Country).ToList();

                //取得每一個AdvEventType下的AdvEvent, 並組好放到Dictionary
                objDictResult = new Dictionary<int, Dictionary<TWNewEgg.Models.ViewModels.Advertising.AdvEventType, List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay>>>();
                numSort = 1;
                foreach (TWNewEgg.Models.ViewModels.Advertising.AdvEventType objAdvType in listAdvEventType)
                {
                    //根據AdvTypeCode取得所有AdvType的列表, 並以Country欄位作為排序
                    listAdvEventDisplay = Processor.Request<List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay>, List<TWNewEgg.Models.DomainModels.Advertising.AdvEventDisplay>>("Service.AdvEventItemService", "GetActiveAdvEventDisplayListByAdvEventTypeId", objAdvType.ID).results;
                    //若此AdvEventType下有廣告,再存入Dictionary
                    if (listAdvEventDisplay != null)
                    {
                        objDictItem = new Dictionary<TWNewEgg.Models.ViewModels.Advertising.AdvEventType, List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay>>();
                        objDictItem.Add(objAdvType, listAdvEventDisplay);
                        objDictResult.Add(numSort, objDictItem);
                        numSort++;
                    }
                }
            }
            return PartialView("Partial_AdvTopBanner", objDictResult);
        }

        /// <summary>
        /// 取得首頁GridBanner
        /// </summary>
        /// <returns>PartialView</returns>
        public ActionResult GetGridAdvertise()
        {
            List<TWNewEgg.Models.ViewModels.Advertising.AdvEventType> listAdvEventType = null;
            List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay> listAdvEventDisplay = null;
            Dictionary<int, Dictionary<TWNewEgg.Models.ViewModels.Advertising.AdvEventType, List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay>>> objGridDictResult = null;
            Dictionary<TWNewEgg.Models.ViewModels.Advertising.AdvEventType, List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay>> objGridDictItem = null;
            int numSort = 0;
            listAdvEventType = Processor.Request<List<TWNewEgg.Models.ViewModels.Advertising.AdvEventType>, List<TWNewEgg.Models.DomainModels.Advertising.AdvEventType>>("Service.AdvEventTypeReposity", "GetActiveAdvEventTypeListByAdvType", 4).results;
            if (listAdvEventType != null)
            {
                listAdvEventType = listAdvEventType.OrderBy(x => x.Country).ToList();
                objGridDictResult = new Dictionary<int, Dictionary<TWNewEgg.Models.ViewModels.Advertising.AdvEventType, List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay>>>();
                numSort = 1;
                foreach (TWNewEgg.Models.ViewModels.Advertising.AdvEventType objAdvType in listAdvEventType)
                {
                    //根據AdvTypeCode取得所有AdvType的列表, 並以Country欄位作為排序
                    listAdvEventDisplay = Processor.Request<List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay>, List<TWNewEgg.Models.DomainModels.Advertising.AdvEventDisplay>>("Service.AdvEventItemService", "GetActiveAdvEventDisplayListByAdvEventTypeId", objAdvType.ID).results;
                    //若此AdvEventType下有廣告,再存入Dictionary
                    if (listAdvEventDisplay != null)
                    {
                        objGridDictItem = new Dictionary<TWNewEgg.Models.ViewModels.Advertising.AdvEventType, List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay>>();
                        objGridDictItem.Add(objAdvType, listAdvEventDisplay);
                        objGridDictResult.Add(numSort, objGridDictItem);
                        numSort++;
                    }
                }
            }
            return PartialView("Partial_AdvGridBanner", objGridDictResult);
        }

        public ActionResult GetLeftMenu()
        {
            List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem> listTreeItem = null;

            listTreeItem = Processor.Request<List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem>, List<TWNewEgg.Models.DomainModels.Category.Category_TreeItem>>("Service.BaseService", "XML2Tree", "XML\\Category_Menu_ALL.XML").results;

            
            return PartialView("Partial_CategoryMenu", listTreeItem);
        }

        /// <summary>
        /// 取得關鍵字
        /// </summary>
        /// <param name="CategoryId"></param>
        /// <returns></returns>
        public ActionResult GetHotWords(int CategoryId)
        {
           List<TWNewEgg.Models.ViewModels.HotWords.HotWords> listHotWords = null;
           listHotWords = Processor.Request<List<TWNewEgg.Models.ViewModels.HotWords.HotWords>, List<TWNewEgg.Models.DomainModels.HotWords.HotWords>>("HotWordsService", "GetActive", CategoryId).results;
           return PartialView("_HotWords", listHotWords);
            
           
          
        }

        //登入問候語------------------------add by bob 20160412
        public string GetGreetingWords()
        {
            string greetingWords = "";
            DateTime dateTimeNow = DateTime.Now;
            var result = Processor.Request<List<TWNewEgg.Models.ViewModels.GreetingWords.HomeGreetingWordsVM>, List<TWNewEgg.Models.DomainModels.GreetingWords.HomeGreetingWordsDM>>("HomeGreetingWordsService", "GetShow", dateTimeNow);
            if (result.results != null)
            {
                if (DateTime.Compare(dateTimeNow, result.results[0].StartDate) >= 0 && DateTime.Compare(dateTimeNow, result.results[0].EndDate) < 0)
                {
                    greetingWords = "Hi, {email}, <br>" + result.results[0].Description;
                }
            }
            else
            {
                greetingWords = "Hi, {email}，您好！";
            }
            return greetingWords;
        }

        //問候卡------------------------add by bob 20160414
        public string GetGreetingWords2()
        {
            string greetingWords = "";
            DateTime dateTimeNow = DateTime.Now;
            var result = Processor.Request<List<TWNewEgg.Models.ViewModels.GreetingWords.HomeGreetingWordsVM>
                , List<TWNewEgg.Models.DomainModels.GreetingWords.HomeGreetingWordsDM>>("HolidayGreetingWordsService", "GetShow", dateTimeNow);

            if (result.results != null)
            {
                if (DateTime.Compare(dateTimeNow, result.results[0].StartDate) >= 0 && DateTime.Compare(dateTimeNow, result.results[0].EndDate) < 0)
                {
                    greetingWords = result.results[0].ImageUrl;
                }
            }
            else
            {
                greetingWords = string.Empty;
            }
            return greetingWords;
        }


        /// <summary>
        /// 問候卡.
        /// </summary>
        /// <returns>問候卡.</returns>
        public PartialViewResult GreetingWords2()
        {
            return PartialView("GreetingWords2");
        }


    }
}
