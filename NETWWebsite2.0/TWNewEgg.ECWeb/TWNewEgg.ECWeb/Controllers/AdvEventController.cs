using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.ECWeb.PrivilegeFilters;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.ViewModels;

namespace TWNewEgg.ECWeb.Controllers
{
    [AllowNonSecures]
    [AllowAnonymous]
    public class AdvEventController : Controller
    {
        //
        // GET: /AdvEvent/

        public ActionResult Index(int advEventTypeID)
        {
            List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay> listAdvEventDisplay = null;
            listAdvEventDisplay = Processor.Request<List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay>, List<TWNewEgg.Models.DomainModels.Advertising.AdvEventDisplay>>("Service.AdvEventItemService", "GetActiveAdvEventDisplayListByAdvEventTypeId", advEventTypeID).results;
            ViewBag.advEventTypeID = advEventTypeID;
            
            switch (advEventTypeID)
            {
                case 28:
                    return PartialView("Partial_AdvBottomer", listAdvEventDisplay);
                    break;
                case 31:
                    return PartialView("Partial_AdvRightTopBanner", listAdvEventDisplay);
                    break;
                case 33:
                    return PartialView("Partial_AdvLogBanner", listAdvEventDisplay);
                    break;
                case 34:
                    return PartialView("Partial_AdvLogBanner", listAdvEventDisplay);
                    break;
                case 35:
                    return PartialView("Partial_AdvHeaderBanner", listAdvEventDisplay);
                    break;
                case 36:
                    return PartialView("Partial_AdvActivityBanner", listAdvEventDisplay);
                    break;
                case 37:
                    return PartialView("Partial_AdvLeftTopBanner", listAdvEventDisplay);
                    break;

                case 49:
                    return PartialView("Partial_AdvActivityTopBanner", listAdvEventDisplay);
                    break;
                default:
                    return PartialView("");
                    break;
            }
            //return PartialView("_Bottomer", listAdvEventDisplay);
        }

        public ActionResult Partial_AdvHeaderBanner()
        {
            List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay> listAdvEventDisplay = null;
            listAdvEventDisplay = Processor.Request<List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay>, List<TWNewEgg.Models.DomainModels.Advertising.AdvEventDisplay>>("Service.AdvEventItemService", "GetActiveAdvEventDisplayListByAdvEventTypeId", 35).results; //49.35
            return PartialView("Partial_AdvHeaderBanner", listAdvEventDisplay);
        }

        public ActionResult Partial_AdvBottomer()
        {
            List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay> listAdvEventDisplay = null;
            listAdvEventDisplay = Processor.Request<List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay>, List<TWNewEgg.Models.DomainModels.Advertising.AdvEventDisplay>>("Service.AdvEventItemService", "GetActiveAdvEventDisplayListByAdvEventTypeId", 28).results;
            return PartialView("Partial_AdvBottomer", listAdvEventDisplay);
        }

        public ActionResult Partial_AdvRightTopBanner()
        {
            List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay> listAdvEventDisplay = null;
            listAdvEventDisplay = Processor.Request<List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay>, List<TWNewEgg.Models.DomainModels.Advertising.AdvEventDisplay>>("Service.AdvEventItemService", "GetActiveAdvEventDisplayListByAdvEventTypeId", 31).results;
            return PartialView("Partial_AdvRightTopBanner", listAdvEventDisplay);
        }

        public ActionResult Partial_AdvLeftTopBanner()
        {
            List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay> listAdvEventDisplay = null;
            listAdvEventDisplay = Processor.Request<List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay>, List<TWNewEgg.Models.DomainModels.Advertising.AdvEventDisplay>>("Service.AdvEventItemService", "GetActiveAdvEventDisplayListByAdvEventTypeId", 37).results; //51.37
            return PartialView("Partial_AdvRightTopBanner", listAdvEventDisplay);
        }

        public ActionResult Partial_AdvLogBanner()
        {
            List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay> listAdvEventDisplay = null;
            listAdvEventDisplay = Processor.Request<List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay>, List<TWNewEgg.Models.DomainModels.Advertising.AdvEventDisplay>>("Service.AdvEventItemService", "GetActiveAdvEventDisplayListByAdvEventTypeId", 33).results;
            return PartialView("Partial_AdvLogBanner", listAdvEventDisplay);
        }
        
        public ActionResult Partial_AdvGuestLogBanner()
        {
            List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay> listAdvEventDisplay = null;
            listAdvEventDisplay = Processor.Request<List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay>, List<TWNewEgg.Models.DomainModels.Advertising.AdvEventDisplay>>("Service.AdvEventItemService", "GetActiveAdvEventDisplayListByAdvEventTypeId", 34).results;
            return PartialView("Partial_AdvLogBanner", listAdvEventDisplay);
        }

        public ActionResult Partial_AdvActivityBanner()
        {
            List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay> listAdvEventDisplay = null;
            listAdvEventDisplay = Processor.Request<List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay>, List<TWNewEgg.Models.DomainModels.Advertising.AdvEventDisplay>>("Service.AdvEventItemService", "GetActiveAdvEventDisplayListByAdvEventTypeId", 36).results;
            return PartialView("Partial_AdvActivityBanner", listAdvEventDisplay);
        }

        public ActionResult Partial_AdvTopHeadBanner()
        {
            List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay> listAdvEventDisplay = null;
            listAdvEventDisplay = Processor.Request<List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay>, List<TWNewEgg.Models.DomainModels.Advertising.AdvEventDisplay>>("Service.AdvEventItemService", "GetActiveAdvEventDisplayListByAdvEventTypeId", 49).results; //49.35
            return PartialView("Partial_AdvWholeSiteTopBanner", listAdvEventDisplay);
        }

        public ActionResult Partial_AdvActivityTopBanner()
        {
            List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay> listAdvEventDisplay = null;
            listAdvEventDisplay = Processor.Request<List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay>, List<TWNewEgg.Models.DomainModels.Advertising.AdvEventDisplay>>("Service.AdvEventItemService", "GetActiveAdvEventDisplayListByAdvEventTypeId", 49).results; //49.35
            return PartialView("Partial_AdvActivityTopBanner", listAdvEventDisplay);
        }
        //次分類頁左下角全站廣告
        public ActionResult Partial_CategoryLeftBanner()
        {
            List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay> listAdvEventDisplay = null;
            listAdvEventDisplay = Processor.Request<List<TWNewEgg.Models.ViewModels.Advertising.AdvEventDisplay>, List<TWNewEgg.Models.DomainModels.Advertising.AdvEventDisplay>>("Service.AdvEventItemService", "GetActiveAdvEventDisplayListByAdvEventTypeId", 54).results;
            return PartialView("Partial_AdvRightTopBanner", listAdvEventDisplay);
        }

    }
}
