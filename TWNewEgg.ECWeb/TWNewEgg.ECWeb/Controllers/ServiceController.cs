using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TWNewEgg.ECWeb.PrivilegeFilters;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DomainModels.Activity;
using TWNewEgg.Models.ViewModels.Activity;

namespace TWNewEgg.ECWeb.Controllers
{
    [AllowNonSecures]
    [AllowAnonymous]
    public class ServiceController : Controller
    {
        //
        // GET: /Service/

        public ActionResult FAQ()
        {
            string activityName = "FAQ";
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ActivitySectionInfor ActivitySectionInforData = new ActivitySectionInfor();
            ActivityVM rltActivityVM = new ActivityVM();
            var results = Processor.Request<ActivityVM, ActivityDM>("ActivityService", "GetActivityByName", activityName);
            if (string.IsNullOrEmpty(results.error))
            {
                rltActivityVM = results.results;
                if (rltActivityVM.ShowType == 1)
                {
                    return RedirectToAction("Index", "Home");
                }
                #region 解析Json ActivitySectionInfor
                rltActivityVM.SectionInfor = rltActivityVM.SectionInfor ?? string.Empty;
                rltActivityVM.SectionInfor = rltActivityVM.SectionInfor.ToLower().Replace(@"""", "").Replace(@"\", "").Replace(@"null", "false");
                ActivitySectionInforData = serializer.Deserialize<ActivitySectionInfor>(rltActivityVM.SectionInfor);
                AutoMapper.Mapper.Map(ActivitySectionInforData, rltActivityVM);
                #endregion
                rltActivityVM.HtmlContext = HttpUtility.HtmlDecode(rltActivityVM.HtmlContext);
            }
            else
            {
                rltActivityVM.Header = "true";
                rltActivityVM.Footer = "true";
                rltActivityVM.Topper = "true";
                rltActivityVM.Bottomer = "true";
                rltActivityVM.HtmlContext = "";
            }
            return View(rltActivityVM);
        }

        public ActionResult AboutShopping()
        {
            string activityName = "AboutShopping";
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ActivitySectionInfor ActivitySectionInforData = new ActivitySectionInfor();
            ActivityVM rltActivityVM = new ActivityVM();
            var results = Processor.Request<ActivityVM, ActivityDM>("ActivityService", "GetActivityByName", activityName);
            if (string.IsNullOrEmpty(results.error))
            {
                rltActivityVM = results.results;
                if (rltActivityVM.ShowType == 1)
                {
                    return RedirectToAction("Index", "Home");
                }
                #region 解析Json ActivitySectionInfor
                rltActivityVM.SectionInfor = rltActivityVM.SectionInfor ?? string.Empty;
                rltActivityVM.SectionInfor = rltActivityVM.SectionInfor.ToLower().Replace(@"""", "").Replace(@"\", "").Replace(@"null", "false");
                ActivitySectionInforData = serializer.Deserialize<ActivitySectionInfor>(rltActivityVM.SectionInfor);
                AutoMapper.Mapper.Map(ActivitySectionInforData, rltActivityVM);
                #endregion
                rltActivityVM.HtmlContext = HttpUtility.HtmlDecode(rltActivityVM.HtmlContext);
            }
            else
            {
                rltActivityVM.Header = "true";
                rltActivityVM.Footer = "true";
                rltActivityVM.Topper = "true";
                rltActivityVM.Bottomer = "true";
                rltActivityVM.HtmlContext = "";
            }
            return View(rltActivityVM);
        }

        public ActionResult AboutShipping()
        {
            string activityName = "AboutShipping";
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ActivitySectionInfor ActivitySectionInforData = new ActivitySectionInfor();
            ActivityVM rltActivityVM = new ActivityVM();
            var results = Processor.Request<ActivityVM, ActivityDM>("ActivityService", "GetActivityByName", activityName);
            if (string.IsNullOrEmpty(results.error))
            {
                rltActivityVM = results.results;
                if (rltActivityVM.ShowType == 1)
                {
                    return RedirectToAction("Index", "Home");
                }
                #region 解析Json ActivitySectionInfor
                rltActivityVM.SectionInfor = rltActivityVM.SectionInfor ?? string.Empty;
                rltActivityVM.SectionInfor = rltActivityVM.SectionInfor.ToLower().Replace(@"""", "").Replace(@"\", "").Replace(@"null", "false");
                ActivitySectionInforData = serializer.Deserialize<ActivitySectionInfor>(rltActivityVM.SectionInfor);
                AutoMapper.Mapper.Map(ActivitySectionInforData, rltActivityVM);
                #endregion
                rltActivityVM.HtmlContext = HttpUtility.HtmlDecode(rltActivityVM.HtmlContext);
            }
            else
            {
                rltActivityVM.Header = "true";
                rltActivityVM.Footer = "true";
                rltActivityVM.Topper = "true";
                rltActivityVM.Bottomer = "true";
                rltActivityVM.HtmlContext = "";
            }
            return View(rltActivityVM);
        }

        public ActionResult AboutPayment()
        {
            string activityName = "AboutPayment";
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ActivitySectionInfor ActivitySectionInforData = new ActivitySectionInfor();
            ActivityVM rltActivityVM = new ActivityVM();
            var results = Processor.Request<ActivityVM, ActivityDM>("ActivityService", "GetActivityByName", activityName);
            if (string.IsNullOrEmpty(results.error))
            {
                rltActivityVM = results.results;
                if (rltActivityVM.ShowType == 1)
                {
                    return RedirectToAction("Index", "Home");
                }
                #region 解析Json ActivitySectionInfor
                rltActivityVM.SectionInfor = rltActivityVM.SectionInfor ?? string.Empty;
                rltActivityVM.SectionInfor = rltActivityVM.SectionInfor.ToLower().Replace(@"""", "").Replace(@"\", "").Replace(@"null", "false");
                ActivitySectionInforData = serializer.Deserialize<ActivitySectionInfor>(rltActivityVM.SectionInfor);
                AutoMapper.Mapper.Map(ActivitySectionInforData, rltActivityVM);
                #endregion
                rltActivityVM.HtmlContext = HttpUtility.HtmlDecode(rltActivityVM.HtmlContext);
            }
            else
            {
                rltActivityVM.Header = "true";
                rltActivityVM.Footer = "true";
                rltActivityVM.Topper = "true";
                rltActivityVM.Bottomer = "true";
                rltActivityVM.HtmlContext = "";
            }
            return View(rltActivityVM);
        }

        public ActionResult AboutService()
        {
            string activityName = "AboutService";
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ActivitySectionInfor ActivitySectionInforData = new ActivitySectionInfor();
            ActivityVM rltActivityVM = new ActivityVM();
            var results = Processor.Request<ActivityVM, ActivityDM>("ActivityService", "GetActivityByName", activityName);
            if (string.IsNullOrEmpty(results.error))
            {
                rltActivityVM = results.results;
                if (rltActivityVM.ShowType == 1)
                {
                    return RedirectToAction("Index", "Home");
                }
                #region 解析Json ActivitySectionInfor
                rltActivityVM.SectionInfor = rltActivityVM.SectionInfor ?? string.Empty;
                rltActivityVM.SectionInfor = rltActivityVM.SectionInfor.ToLower().Replace(@"""", "").Replace(@"\", "").Replace(@"null", "false");
                ActivitySectionInforData = serializer.Deserialize<ActivitySectionInfor>(rltActivityVM.SectionInfor);
                AutoMapper.Mapper.Map(ActivitySectionInforData, rltActivityVM);
                #endregion
                rltActivityVM.HtmlContext = HttpUtility.HtmlDecode(rltActivityVM.HtmlContext);
            }
            else
            {
                rltActivityVM.Header = "true";
                rltActivityVM.Footer = "true";
                rltActivityVM.Topper = "true";
                rltActivityVM.Bottomer = "true";
                rltActivityVM.HtmlContext = "";
            }
            return View(rltActivityVM);
        }

        public ActionResult MemberService()
        {
            string activityName = "MemberService";
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ActivitySectionInfor ActivitySectionInforData = new ActivitySectionInfor();
            ActivityVM rltActivityVM = new ActivityVM();
            var results = Processor.Request<ActivityVM, ActivityDM>("ActivityService", "GetActivityByName", activityName);
            if (string.IsNullOrEmpty(results.error))
            {
                rltActivityVM = results.results;
                if (rltActivityVM.ShowType == 1)
                {
                    return RedirectToAction("Index", "Home");
                }
                #region 解析Json ActivitySectionInfor
                rltActivityVM.SectionInfor = rltActivityVM.SectionInfor ?? string.Empty;
                rltActivityVM.SectionInfor = rltActivityVM.SectionInfor.ToLower().Replace(@"""", "").Replace(@"\", "").Replace(@"null", "false");
                ActivitySectionInforData = serializer.Deserialize<ActivitySectionInfor>(rltActivityVM.SectionInfor);
                AutoMapper.Mapper.Map(ActivitySectionInforData, rltActivityVM);
                #endregion
                rltActivityVM.HtmlContext = HttpUtility.HtmlDecode(rltActivityVM.HtmlContext);
            }
            else
            {
                rltActivityVM.Header = "true";
                rltActivityVM.Footer = "true";
                rltActivityVM.Topper = "true";
                rltActivityVM.Bottomer = "true";
                rltActivityVM.HtmlContext = "";
            }
            return View(rltActivityVM);
        }

    }
}
