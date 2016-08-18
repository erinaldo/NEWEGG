using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Models;

namespace TWNewEgg.API.Controllers
{
    public class QueryWebConfigController : Controller
    {
        /// <summary>
        /// 取得 API AppSetting 設定值
        /// </summary>
        /// <param name="configSetting"></param>
        /// <returns>Return API APPConfigSetting</returns>
        [HttpGet]
        public JsonResult GetAPIWebConfig(string configSetting)
        {
            string returnWebSetting = string.Empty;
            try
            {
                returnWebSetting = System.Configuration.ConfigurationManager.AppSettings[configSetting];
            }
            catch (Exception)
            {
                returnWebSetting = string.Empty;               
            }

            return Json(returnWebSetting, JsonRequestBehavior.AllowGet);
        }
    }
   
}
