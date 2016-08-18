using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.View.Attributes;
using TWNewEgg.API.Models;
using TWNewEgg.DB.TWSELLERPORTALDB.Models;
using AutoMapper;

//using TWNewEgg.API.View.KendoCRUDService.Common;

namespace TWNewEgg.API.View.Controllers
{
    /// <summary>
    /// 依據 BSATW-173 廢四機需求增加
    /// 癈四機回收四聯單-----------add by bruce 20160505
    /// </summary>
    public class Discard4Controller : Controller
    {

        //log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        public ActionResult Index()
        {
            return View();
        }



        /// <summary>
        /// 初始化癈四機回收四聯單
        /// </summary>
        /// <param name="salesorderCode">LBO</param>
        /// <param name="user_name">建立者</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult InitData(string salesorderCode, string user_name)
        {
            JsonResult json_data = new JsonResult();
            try
            {
                Connector connector = new Connector();
                var list_result = connector.InitData_Discard4("", "", salesorderCode, user_name);
                json_data = this.Json(list_result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                json_data.Data = ex.Message;
            }
            return json_data;
        }

        /// <summary>
        /// 儲存即有的或新的資料
        /// </summary>
        /// <param name="list_info">
        /// List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>
        /// </param>
        /// <param name="user_name">建立者</param>
        /// <returns></returns>
        [HttpPost]
        //public JsonResult Save(List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM> list_info)
        public JsonResult Save(string json_str, List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM> list_info)
        {
            
            //取得 SaveModel 資料
            //var list_info = new List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>();
            //list_info = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>>("List_Data", ControllerExtensions.RequestType.POST);
            //list_info = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>>(json_str);

            JsonResult json_data = new JsonResult();
            try
            {
                // 取得 cookie 資訊
                TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();
                string user_name = sellerInfo.UserEmail.ToString();

                //string user_name = string.Empty;

                //時區問題將時間調整成加8小時--------------------add by bruce 20160526
                foreach (var each_info in list_info)
                {
                    if (each_info.InstalledDate.Value == null) continue;
                    //each_info.InstalledDate = new DateTime(each_info.InstalledDate.Value.Year, each_info.InstalledDate.Value.Month, each_info.InstalledDate.Value.Day, 23, 59, 59);
                    DateTime now_date = each_info.InstalledDate.Value.AddHours(8);
                    each_info.InstalledDate = now_date;
                }
                //時區問題將時間調整成加8小時--------------------add by bruce 20160526

                Connector connector = new Connector();
                var list_result = connector.Save_Discard4("", "", list_info, user_name);
                list_result.IsSuccess = true;
                json_data = this.Json(list_result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                json_data.Data = ex.Message;
            }
            return json_data;
        }

        /// <summary>
        /// 取得資料
        /// </summary>
        /// <param name="salesorderCode">LBO</param>
        /// <param name="user_name">建立者</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetData(string salesorderCode)
        {
            JsonResult json_data = new JsonResult();
            try
            {
                string user_name = "";
                Connector connector = new Connector();
                TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>> list_result = null;
                list_result = connector.GetData_Discard4("", "", salesorderCode, user_name);
                json_data = this.Json(list_result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                json_data.Data = ex.Message;
            }
            return json_data;
        }

        [HttpPost]
        [Filter.PermissionFilter]
        [Filter.LoginAuthorize]
        public JsonResult GetDiscard4ItemPage(string salesorderCode)
        {
            // 依據 BSATW-173 廢四機需求增加
            // 癈四機回收四聯單-----------add by bruce 20160505
            ViewBag.salesorderCode = salesorderCode;

            Connector connector = new Connector();
            //檢查有沒有資料            
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>> list_result = null;
            list_result = connector.GetData_Discard4("", "", salesorderCode, "");

            string resultView = RenderView("Discard4Item");

            string html = resultView;

            ////http://blog.amastaneh.com/2011/06/c-html-minification.html
            ///// Solution A
            //html = Regex.Replace(html, @"\n|\t", " ");
            //html = Regex.Replace(html, @">\s+<", "><").Trim();
            //html = Regex.Replace(html, @"\s{2,}", " ");

            ///// Solution B
            //html = Regex.Replace(html, @"(?<=[^])\t{2,}|(?<=[>])\s{2,}(?=[<])|(?<=[>])\s{2,11}(?=[<])|(?=[\n])\s{2,}", "");
            //html = Regex.Replace(html, @"[ \f\r\t\v]?([\n\xFE\xFF/{}[\];,<>*%&|^!~?:=])[\f\r\t\v]?", "$1");
            //html = html.Replace(";\n", ";");

            ///// Solution C
            //html = Regex.Replace(html, @"[a-zA-Z]+#", "#");
            //html = Regex.Replace(html, @"[\n\r]+\s*", string.Empty);
            //html = Regex.Replace(html, @"\s+", " ");
            //html = Regex.Replace(html, @"\s?([:,;{}])\s?", "$1");
            //html = html.Replace(";}", "}");
            //html = Regex.Replace(html, @"([\s:]0)(px|pt|%|em)", "$1");

            /// Remove comments
            //html = Regex.Replace(html, @"/\*[\d\D]*?\*/", string.Empty);


            TWNewEgg.API.View.Service.AES aes = new Service.AES();
            TWNewEgg.API.Models.UserLogin userloginInfo = new UserLogin();
            userloginInfo.UserEmail = aes.AesDecrypt(Request.Cookies["UEM"].Value);

            string user_name = userloginInfo.UserEmail;

            ViewBag.user_name = user_name;


            resultView = html;

            return Json(new { isSuccess = true, subpageHTML = resultView });
        }

        /// <summary>
        /// 將該 View 轉成 string
        /// </summary>
        /// <param name="view_name">View 的名稱</param>
        /// <returns>返回 string</returns>
        private string RenderView(string view_name)
        {
            string result = string.Empty;
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, view_name);
                ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                result = sw.GetStringBuilder().ToString();
            }

            return result;
        }

    }
}
