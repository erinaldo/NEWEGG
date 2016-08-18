using AutoMapper;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using KendoGridBinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Models;
using TWNewEgg.API.View.Attributes;
using TWNewEgg.API.View.Service;
using TWNewEgg.DB.TWBACKENDDB.Models;
using TWNewEgg.DB.TWSQLDB.Models;

namespace TWNewEgg.API.View.Controllers
{
    // 依據 BSATW-173 廢四機需求增加
    // 癈四機回收四聯單-----------add by bruce 20160505
    public partial class OrderListController : Controller
    {



        [HttpPost]
        [Filter.PermissionFilter]
        [Filter.LoginAuthorize]
        public JsonResult GetDiscard4ItemPage(string salesorderCode)
        {
            TWNewEgg.API.View.Service.AES aes = new Service.AES();
            TWNewEgg.API.Models.UserLogin userloginInfo = new UserLogin();
            userloginInfo.UserEmail = aes.AesDecrypt(Request.Cookies["UEM"].Value);

            string user_name = userloginInfo.UserEmail;

            // 依據 BSATW-173 廢四機需求增加
            // 癈四機回收四聯單-----------add by bruce 20160505
            ViewBag.salesorderCode = salesorderCode;

            Connector connector = new Connector();
            //檢查有沒有資料            
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>> list_result = null;
            list_result = connector.GetData_Discard4("", "", salesorderCode, "");
            //不存在時建立癈四機回收四聯單
            if (list_result.IsSuccess && list_result.Body.Count() == 0)
            {
                connector.InitData_Discard4("", "", salesorderCode, user_name);
            }


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






            ViewBag.user_name = user_name;


            resultView = html;

            return Json(new { isSuccess = true, subpageHTML = resultView });
        }



    }
}
