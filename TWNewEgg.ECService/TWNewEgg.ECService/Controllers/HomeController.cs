using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.Framework.Autofac;
using TWNewEgg.CartRepoAdapters.Interface;
using System.IO;
using System.Text;

namespace TWNewEgg.ECService.Controllers
{
    public partial class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetDetail(string SOCode)
        {
            var SOInfoRepoServices = AutofacConfig.Container.Resolve<IDBSOInfoRepoAdapter>();
            SOInfoRepoServices.GetDBSOInfo(SOCode);
            return View();
        }

        public JsonResult AzureContentTypeList(string container, string blob, string contentType)
        {
            var SOInfoRepoServices = AutofacConfig.Container.Resolve<TWNewEgg.StorageServices.Interface.IAure>();
            var result = SOInfoRepoServices.changeFileContentTypeFileByBatch(container, blob, contentType);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AzureContentType(string containerName, string contentType, string blob)
        {
            try
            {

                //SOInfoRepoServices.ChangeFileContentTypeForBlob(containerName, contentType, blob);
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.ToString(), JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 會計文件Log
        /// [e10ecsrv01網址]/Home/LogManager?logName=20160525&logType=Finance
        /// </summary>
        /// <param name="logName"></param>
        /// <returns></returns>
        public FileResult LogManager(string logName, string logType = "")
        {
            string strLogName = string.Format("{0}.log", logName);
            //string strPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/LogFiles/Finance/"), strLogName);
            string strPath = System.Web.HttpContext.Current.Server.MapPath("~/LogFiles/");

            if (!string.IsNullOrWhiteSpace(logType))
                strPath = Path.Combine(strPath, logType);

            strPath = Path.Combine(strPath, strLogName);

            EnumerableStreamResult result = new EnumerableStreamResult();

            if (System.IO.File.Exists(strPath))
            {
                IEnumerable<string> content = FileReadLines(strPath);
                result = new EnumerableStreamResult(content, "text/plain", strLogName);
            }

            return result;
        }

        /// <summary>
        /// 讀取大檔案
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private IEnumerable<string> FileReadLines(string path)
        {
            List<string> col = new List<string>();
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 0x1000, FileOptions.SequentialScan))
            using (var sr = new StreamReader(fs, Encoding.UTF8))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

    }
}
