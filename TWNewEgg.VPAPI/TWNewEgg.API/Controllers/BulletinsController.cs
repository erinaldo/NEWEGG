using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Models;
using TWNewEgg.Framework.Autofac;
using Autofac;

namespace TWNewEgg.API.Controllers
{
    public class BulletinsController : Controller
    {
        // 記錄訊息
        private static ILog log = LogManager.GetLogger(typeof(BulletinsController));

        private TWNewEgg.BulletinsMessageService.Interface.IBulletinsMessage BulletinsService;

        public BulletinsController()
        {
            using (var scope = AutofacConfig.Container.BeginLifetimeScope())
            {
                this.BulletinsService = scope.Resolve<TWNewEgg.BulletinsMessageService.Interface.IBulletinsMessage>();
            }
        }

        //
        // GET: /Bulletins/

        [HttpPost]
        public JsonResult ReadXml()
        {
            ActionResponse<string> result = new ActionResponse<string>();
            try
            {
                var getResult = this.BulletinsService.GetBulletinsMessage("VendorPortal");

                result.Code = getResult.Code;
                result.Body = getResult.Body.MessageContent;
                result.Msg = getResult.Body.Updated.ToString();
                result.IsSuccess = getResult.IsSuccess;
            }
            catch (Exception ex)
            {
                string inneerMsg = GetExceptionMessage(ex);
                log.Error("Error Msg: " + ex.Message + ", " + ex.StackTrace + ", InneerMsg: " + inneerMsg);

                result.Code = (int)ResponseCode.AccessError;
                result.Msg = "發生意外錯誤，請稍後在試!";
                result.IsSuccess = false;
                result.Body = null;
            }

            return Json(result);
        }

        public JsonResult WriteXml(string xmlContent, int userid, int updateNumber)
        {
            ActionResponse<string> result = new ActionResponse<string>();

            try
            {
                var getResult = this.BulletinsService.GetBulletinsMessage("VendorPortal");
                TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BulletinsMessage updateModel = new DB.TWSELLERPORTALDB.Models.Seller_BulletinsMessage();

                updateModel.ID = getResult.Body.ID;
                updateModel.FromSystem = "vendorportal";
                updateModel.MessageContent = xmlContent;
                updateModel.UpdateUser = userid.ToString();
                updateModel.Updated = updateNumber;

                result = this.BulletinsService.UpdateBulletinsMessage(updateModel);
            }
            catch (Exception ex)
            {
                string inneerMsg = GetExceptionMessage(ex);
                log.Error("Error Msg: " + ex.Message + ", " + ex.StackTrace + ", InneerMsg: " + inneerMsg);

                result.Code = (int)ResponseCode.AccessError;
                result.Msg = "發生意外錯誤，請稍後在試!";
                result.IsSuccess = false;
                result.Body = null;
            }

            return Json(result);
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateBulletins(TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BulletinsMessage CreateModel)
        {
            TWNewEgg.API.Models.ActionResponse<string> result = new ActionResponse<string>();

            try
            {
                result = BulletinsService.CreateBulletinsMessage(CreateModel);
            }
            catch (Exception ex)
            {
                log.Error("Error Msg: " + ex.Message + ", " + ex.StackTrace + ", InneerMsg: " + GetExceptionMessage(ex));
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private string GetExceptionMessage(System.Exception ex)
        {
            if (ex.InnerException != null)
            {
                return this.GetExceptionMessage(ex.InnerException);
            }
            else
            {
                return ex.Message;
            }
        }

        #region 舊版讀取 XML 版本

        //[Attributes.ActionDescriptionAttribute("公佈欄")]
        //[HttpPost]
        //public JsonResult ReadXml()
        //{
        //    TWNewEgg.API.Service.XmlServer xmlserver = new Service.XmlServer();
        //    ActionResponse<string> result = new ActionResponse<string>();
        //    result = xmlserver.readXML();
        //    return Json(result);
        //}
        //public JsonResult WriteXml(string xmlContent, int userid, int updateNumber)
        //{
        //    TWNewEgg.API.Service.XmlServer xmlserver = new Service.XmlServer();
        //    ActionResponse<string> result = new ActionResponse<string>();
        //    result = xmlserver.writeXml(xmlContent, userid, updateNumber);
        //    return Json(result);
        //}

        #endregion

    }
}
