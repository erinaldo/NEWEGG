using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using TWNewEgg.API.View.Service;

namespace TWNewEgg.API.View.Controllers
{
    public class AddressParseController : Controller
    {
        //
        // GET: /AddressParse/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 地址解析功能
        /// </summary>
        /// <param name="country"></param>
        /// <param name="address"></param>
        /// <param name="city"></param>
        /// <param name="zipcode"></param>
        /// <returns></returns>
        [HttpGet]
        [Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        public string[] ParseAddress(string country, string address, string city, string zipcode)
        {
            string[] addressInfo = { "", "", "", "" };

            TWNewEgg.API.View.Service.AddressParseService addService = new Service.AddressParseService();
            var addInfo = addService.SplitCityAddress(false, country, city, address, zipcode);

            addressInfo[0] = addInfo.County;
            addressInfo[1] = addInfo.City;
            addressInfo[2] = addInfo.Address + addInfo.Addr;
            addressInfo[3] = addInfo.ZipCode;

            return addressInfo;
            //return Json(new { County = addressInfo[0],City = addressInfo[1], addr = addressInfo[2] },JsonRequestBehavior.AllowGet);
        }

    }
}
