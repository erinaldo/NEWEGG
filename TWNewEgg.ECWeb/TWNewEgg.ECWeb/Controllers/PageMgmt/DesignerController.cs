using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using TWNewEgg.Common;
using TWNewEgg.ECWeb.ActionFilters;
using TWNewEgg.ECWeb.Auth;
using TWNewEgg.ECWeb.PrivilegeFilters;
using TWNewEgg.ECWeb.Resources.PageMgmt;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels.PageMgmt;
using TWNewEgg.Models.DomainModels.PageMgmt;

namespace TWNewEgg.ECWeb.Controllers.PageMgmt
{
    [AllowNonSecures]
    public class DesignerController : Controller
    {
        [IsPageMgmtEditor]
        public ActionResult Index()
        {
            List<PageInfo> pages = Processor.Request<List<PageInfo>, List<PageInfo>>("PageDBUtil", "getPages").results;
            return View(pages);
        }

        /// <summary>
        /// 儲存頁面
        /// </summary>
        /// <param name="page">頁面資訊</param>
        /// <returns>執行結果</returns>
        [HttpPost]
        [IsPageMgmtEditor]
        public ContentResult SavePage(DSPageInfo page)
        {
            string response = "您沒有此權限";
            try
            {
                PageInfo savePage = Processor.Request<PageInfo, PageInfo>("PageDBUtil", "getPage", page.PageID).results;
                List<PageInfo> pages = Processor.Request<List<PageInfo>, List<PageInfo>>("PageDBUtil", "getPages").results.Where(x => x.Path == page.Path).ToList();
                if (savePage.Path != page.Path && pages.Count > 0)
                {
                    response = "網頁名稱已存在";
                }
                else
                {
                    Processor.Request<bool, bool>("PageDBUtil", "savePage", page);
                    string url = Url.Action("EditPage", "Designer", new { area = "PageMgmt", Path = page.Path, isEditPage = "true" });
                    HttpResponse.RemoveOutputCacheItem(url);
                    url = Url.Action("EditPage", "Designer", new { area = "PageMgmt", Path = page.Path, isEditPage = "false" });
                    HttpResponse.RemoveOutputCacheItem(url);
                    response = "success";
                }
            }
            catch (Exception e)
            {
                //logger.Error(e.ToString());
                response = "fail: " + e.Message + ", method: " + e.TargetSite.Name;
            }

            return Content(response);
        }

        /// <summary>
        /// 刪除頁面
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPost]
        [IsPageMgmtEditor]
        public ActionResult DeletePage(PageInfo page)
        {
            string response = "您沒有此權限";
            try
            {
                var result = Processor.Request<PageData, PageData>("PageMgmtAdapter", "DeletePage", page);
                if (result.error != null)
                {
                    throw new Exception(result.error);
                }

                response = "success";
            }
            catch (Exception e)
            {
                response = "fail:" + e.Message;
            }

            return Content(response);
        }

        /// <summary>
        /// 取消編輯，回到前一次上線的狀態，或維持原狀
        /// </summary>
        /// <param name="page">路徑</param>
        /// <returns>執行結果</returns>
        [HttpPost]
        public ActionResult CancelEdit(PageInfo page)
        {
            string response = "您沒有此權限";
            try
            {
                PageData lastActivePage = Processor.Request<PageData, PageData>("PageMgmtAdapter", "GetActivePage", page.Path).results;
                if (lastActivePage == null)
                {
                    response = "無法回復編輯前的頁面，因為本頁面從未正式上線。";
                }

                var result = Processor.Request<PageData, PageData>("PageMgmtAdapter", "CancelEdit", page);
                if (result.error != null)
                {
                    throw new Exception(result.error);
                }

                string url = Url.Action("EditPage", "Designer", new { area = "PageMgmt", Path = page.Path, isEditPage = "true" });
                HttpResponse.RemoveOutputCacheItem(url);
                url = Url.Action("EditPage", "Designer", new { area = "PageMgmt", Path = page.Path, isEditPage = "false" });
                HttpResponse.RemoveOutputCacheItem(url);
                response = "success";
            }
            catch (Exception e)
            {
                response = "fail:" + e.Message;
            }

            return Content(response);
        }

        /// <summary>
        /// 顯示客戶端看的頁面
        /// </summary>
        /// <param name="Path">路徑</param>
        /// <returns>頁面</returns>
        [AllowAnonymous]
        public ActionResult LandingPage(string Path)
        {
            if (Path.IndexOf("template") != -1)
            {
                return Redirect("/");
            }

            ViewBag.Path = Path;
            return View();
        }

        /// <summary>
        /// 取得所有用頁面編輯製作的頁面list
        /// </summary>
        /// <returns>頁面list的JSON格式</returns>
        [HttpPost]
        [IsPageMgmtEditor]
        public JsonResult getSiteMap()
        {
            List<PageInfo> pages = Processor.Request<List<PageInfo>, List<PageInfo>>("PageDBUtil", "getPages").results;

            return Json(pages);
        }

        /// <summary>
        /// 取得最新上線的的頁面
        /// </summary>
        /// <param name="Path">頁面的路徑(名稱)</param>
        /// <returns>頁面資訊JSON</returns>
        [HttpPost]
        [AllowAnonymous]
        public JsonResult GetActivePage(string Path)
        {
            PageData pageData = Processor.Request<PageData, PageData>("PageMgmtAdapter", "GetActivePage", Path).results;

            return Json(pageData);
        }

        /// <summary>
        /// 編輯頁面，取得最新編輯中的頁面
        /// </summary>
        /// <param name="Path">路徑</param>
        /// <param name="isEditPage">頁面是否在編輯中</param>
        /// <returns>頁面資訊JSON</returns>
        [HttpPost]
        [IsPageMgmtEditor]
        public JsonResult EditPage(string path, bool isEditPage)
        {
            string msg = "ERROR";
            JsonResult result;
            PageData data = new PageData();
            var editPageResult = Processor.Request<PageData, PageData>("PageMgmtAdapter", "EditPage", path, isEditPage);
            if(editPageResult.error == null){
                msg = "success";
                data = editPageResult.results;
            }

            result = Json(new { msg = msg, data = data });
            //msg = "unavailable"; //沒有權限
            return result;
        }

        /// <summary>
        /// 頁面送審
        /// </summary>
        /// <param name="page">路徑</param>
        /// <returns>執行結果</returns>
        [HttpPost]
        [IsPageMgmtEditor]
        public ContentResult AuditPage(PageInfo page)
        {
            string response = "";
            var auditResult = Processor.Request<string, string>("PageMgmtAdapter", "AuditPage", page);
            if (auditResult.error != null) {
                response = "fail:" + auditResult.error;
            }
            
            string url = Url.Action("EditPage", "Designer", new { area = "PageMgmt", Path = page.Path, isEditPage = "true" });
            HttpResponse.RemoveOutputCacheItem(url);
            url = Url.Action("EditPage", "Designer", new { area = "PageMgmt", Path = page.Path, isEditPage = "false" });
            HttpResponse.RemoveOutputCacheItem(url);

            response = auditResult.results;

            return Content(response);
        }

        [HttpPost]
        [IsPageMgmtEditor]
        public ContentResult AddPage(PageInfo page)
        {
            string response = "您沒有此權限";
            try
            {
                var result = Processor.Request<string, string>("PageMgmtAdapter", "AddPage", page);
                if (result.error != null)
                {
                    throw new Exception(result.error);
                }

                response = "success";
            }
            catch (Exception e)
            {
                response = "fail:" + e.ToString();
            }

            return Content(response);
        }

        [HttpPost]
        [IsPageMgmtEditor]
        public ContentResult LaunchPage(PageInfo page)
        {
            string response = "";
            var auditResult = Processor.Request<string, string>("PageMgmtAdapter", "LaunchPage", page);
            if (auditResult.error != null)
            {
                response = "fail:" + auditResult.error;
            }

            string url = Url.Action("GetActivePage", "Designer", new { area = "PageMgmt", Path = page.Path });
            HttpResponse.RemoveOutputCacheItem(url);
            url = Url.Action("EditPage", "Designer", new { area = "PageMgmt", Path = page.Path, isEditPage = "true" });
            HttpResponse.RemoveOutputCacheItem(url);
            url = Url.Action("EditPage", "Designer", new { area = "PageMgmt", Path = page.Path, isEditPage = "false" });
            HttpResponse.RemoveOutputCacheItem(url);

            response = auditResult.results;

            return Content(response);
        }

        /// <summary>
        /// 取得資源檔
        /// </summary>
        /// <returns>資源檔JSON</returns>
        [HttpPost]
        [IsPageMgmtEditor]
        public JsonResult GetResx()
        {
            ResourceSet pagemgmtResx = ResourceService.GetResourceSet(PageMgmtResource.ResourceManager);
            ResourceSet propertyResx = ResourceService.GetResourceSet(PropertyResource.ResourceManager);
            ResourceSet sitemapResx = ResourceService.GetResourceSet(SiteMapResource.ResourceManager);
            return Json(new { pagemgmtResx = pagemgmtResx, propertyResx = propertyResx, sitemapResx = sitemapResx });
        }
    }
}
