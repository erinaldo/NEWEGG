using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.API.View.Controllers
{
    public class MultilanguageController : Controller
    {
        //
        // POST: /Multilanguage/Index

        /// <summary>
        /// Resources Language Controller  
        /// </summary>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(string _language, string _url)
        {
            /// <summary>
            /// Add Cookie
            /// Restructuring Url　
            /// </summary>
            /// <param name="MyLang">Cookie name</param>
            /// <param name="delimiterChars">Cutting URL words</param>
            /// <param name="way">Store URL Array</param>
            /// <returns>URL</returns>
            HttpCookie MyLang = new HttpCookie("MyLang");
            MyLang.Value = _language.Trim();
            MyLang.Expires.AddMinutes(30);
            Response.Cookies.Add(MyLang);

            if (_url == "/")
            {
                return Json(_url);
            }
            char[] delimiterChars = { '/' };
            string[] way = _url.Split(delimiterChars);


            return Json(_url);
        }
    }
}
