using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.Models.ViewModels.Product;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.ECWeb.PrivilegeFilters;
using TWNewEgg.ECWeb.Utility;
using TWNewEgg.Models.ViewModels.Item;
using TWNewEgg.Models.DomainModels.Item;

namespace TWNewEgg.ECWeb.Controllers
{
    [AllowNonSecures]
    [AllowAnonymous]
    public class FlashController : Controller
    {
        //
        // GET: /NewEggFlash/

        public ActionResult Index()
        {
            List<TWNewEgg.Models.ViewModels.GroupBuy.GroupBuyViewInfo> results = new List<TWNewEgg.Models.ViewModels.GroupBuy.GroupBuyViewInfo>();
            TWNewEgg.Models.ViewModels.GroupBuy.GroupBuyViewInfo gpbService = new TWNewEgg.Models.ViewModels.GroupBuy.GroupBuyViewInfo();
            ViewBag.gbBuyDisableImageUrl = gpbService.GroupBuyDisableImage;
            ViewBag.gbBuyImageUrl = gpbService.GroupBuyEnableImage;
            ViewBag.gbEndImageUrl = gpbService.GroupBuyEndImage;
            ViewBag.gbSoldOutImageUrl = gpbService.GroupBuySoldOutImage;
            ViewBag.gbPageNumber = 2;
            return View(GetByNumber(0, 99, 1).Data);
            //return View();
        }
        public JsonResult Get(int GroupBuyID = 0)
        {
            List<TWNewEgg.Models.ViewModels.GroupBuy.GroupBuyViewInfo> infoList = new List<TWNewEgg.Models.ViewModels.GroupBuy.GroupBuyViewInfo>();
            try
            {
                TWNewEgg.Models.DomainModels.GroupBuy.GroupBuyQueryCondition condition = new TWNewEgg.Models.DomainModels.GroupBuy.GroupBuyQueryCondition();
                condition.PageSize = 30;
                condition.PageNumber = 1;
                condition.GroupBuyID = GroupBuyID;
                //infoList = gpbService.QueryViewInfo(condition);
                var result = Processor.Request<List<TWNewEgg.Models.ViewModels.GroupBuy.GroupBuyViewInfo>, List<TWNewEgg.Models.DomainModels.GroupBuy.GroupBuyViewInfo>>("GroupBuyService", "QueryViewInfo", condition, (condition.PageSize.ToString() + "_" + condition.PageNumber.ToString()));

                if (string.IsNullOrEmpty(result.error))
                {
                    infoList = result.results;
                    foreach (var singleInfo in infoList)
                    {
                        var listImgUrl = new List<string>();
                        listImgUrl = ConvertUrl(singleInfo.ItemID);
                        if (listImgUrl.Count > 0)
                        {
                            singleInfo.ImgUrl = listImgUrl[0];
                        }
                    }
                }
            }
            catch (Exception e)
            { }
            return Json(infoList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGBData(int GroupBuyID = 0, int pageSize = 8, int pageNumber = 1)
        {
            List<TWNewEgg.Models.ViewModels.GroupBuy.GroupBuyViewInfo> results = new List<TWNewEgg.Models.ViewModels.GroupBuy.GroupBuyViewInfo>();

            return View("GBItem", GetByNumber(GroupBuyID, pageSize, pageNumber).Data);
        }

        public JsonResult GetByNumber(int GroupBuyID = 0, int pageSize = 8, int pageNumber = 1)
        {
            List<TWNewEgg.Models.ViewModels.GroupBuy.GroupBuyViewInfo> infoList = new List<TWNewEgg.Models.ViewModels.GroupBuy.GroupBuyViewInfo>();
            try
            {
                //GroupBuyService.Service.GroupBuyService gpbService = new GroupBuyService.Service.GroupBuyService();
                TWNewEgg.Models.DomainModels.GroupBuy.GroupBuyQueryCondition condition = new TWNewEgg.Models.DomainModels.GroupBuy.GroupBuyQueryCondition();
                condition.PageSize = pageSize;
                condition.PageNumber = pageNumber;
                condition.GroupBuyID = GroupBuyID;
                //infoList = gpbService.QueryViewInfo(condition);
                var result = Processor.Request<List<TWNewEgg.Models.ViewModels.GroupBuy.GroupBuyViewInfo>, List<TWNewEgg.Models.DomainModels.GroupBuy.GroupBuyViewInfo>>("GroupBuyService", "QueryViewInfo", condition, (condition.PageSize.ToString() + "_" + condition.PageNumber.ToString()));
                if (string.IsNullOrEmpty(result.error))
                {
                    infoList = result.results;
                    foreach (var singleInfo in infoList)
                    {
                        var listImgUrl = new List<string>();
                        listImgUrl = ConvertUrl(singleInfo.ItemID);
                        if (listImgUrl.Count > 0)
                        {
                            singleInfo.ImgUrl = listImgUrl[0];
                        }
                    }
                }
            }
            catch (Exception e)
            { }
            return Json(infoList, JsonRequestBehavior.AllowGet);
        }

        public List<string> ConvertUrl(int ItemID)
        {
            Dictionary<int, List<ItemUrl>> itemUrlDictionary = Processor.Request<Dictionary<int, List<ItemUrl>>, Dictionary<int, List<ImageUrlReferenceDM>>>("ItemImageUrlService", "GetItemImagePath", new List<int> { ItemID }).results;
            var listImgUrl = new List<string>();
            foreach (ItemUrl singleImgUrl in itemUrlDictionary[ItemID].Where(x => x.Size == 640))
            {
                if (singleImgUrl.ImageUrl.IndexOf("newegg.com/") >= 0)
                {
                    listImgUrl.Add(singleImgUrl.ImageUrl);
                }
                else
                {
                    listImgUrl.Add(ImageUtility.GetImagePath(singleImgUrl.ImageUrl));
                }
            }
            return listImgUrl;
        }

        //public ActionResult GetGroupBuyRightBanner()
        //{
        //    Dictionary<int, object> objDictResult = null;
        //    List<TWNewEgg.Models.ViewModels.Advertising.AdvEventType> listAdvType = null;
        //    List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay> listAdvEventDisplay = null;

        //    listAdvType = Processor.Request<List<TWNewEgg.Models.ViewModels.Advertising.AdvEventType>, List<TWNewEgg.Models.DomainModels.Advertising.AdvEventType>>("Service.AdvEventTypeReposity", "GetActiveAdvEventTypeListByCountryAndAdvType", 0, 21).results;
        //    listAdvEventDisplay = Processor.Request<List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay>, List<TWNewEgg.Models.DomainModels.Advertising.AdvEventDisplay>>("Service.AdvEventItemService", "GetActiveAdvEventDisplayListByAdvEventTypeId", advEventTypeID).results;
                                                                                                                                                                                                                                                                                                 
        //    //listAdvEventType objAdvTypeService = null;
        //    //AdvEventItemService objAdvEventService = null;
        //    //AdvEventType objAdvType = null;
        //    //List<TWNewEgg.AdvService.Models.AdvEventDisplay> listAdvEventDisplay = null;
            
        //    //objAdvTypeService = new AdvEventTypeReposity();
        //    listAdvType = listAdvEventType.GetActiveAdvEventTypeListByCountryAndAdvType(0, (int)listAdvEventDisplay.AdvTypeOption.GroupBuyRightBanner);

        //    if (listAdvType != null && listAdvType.Count > 0)
        //    {
        //        objDictResult = new Dictionary<int, object>();

        //        //AdvType作排序, 將以最新的AdvType作為顯示依據
        //        //objAdvType = listAdvType.OrderByDescending(x => x.StartDate).First();
        //        //objDictResult.Add(1, objAdvType);

        //        //objAdvEventService = new AdvEventItemService();
        //        //listAdvEventDisplay = objAdvEventService.GetActiveAdvEventDisplayListByAdvEventTypeId(objAdvType.ID);
        //        //objDictResult.Add(2, listAdvEventDisplay);

        //        // 根據連線方式決定圖檔機
        //        if (this.Request.Url.Scheme.ToUpper().Equals("HTTPS"))
        //            objDictResult.Add(3, System.Configuration.ConfigurationManager.AppSettings.Get("SSLImages"));
        //        else
        //            objDictResult.Add(3, System.Configuration.ConfigurationManager.AppSettings.Get("Images"));
        //    }

        //    return PartialView("Partial_GroupBuyRightBanner", objDictResult);
        //}

    }
}
