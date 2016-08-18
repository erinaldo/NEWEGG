using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.API.Controllers
{
    public class SellerContactInfoController : Controller
    {
        // 需回傳多筆 ( 以此分主要 & 非主要 )
        /// <summary>
        /// [Get] Get Seller ContactAddress
        /// </summary>
        /// <param name="Seller">Search table Seller ContactAddress by keyword(Seller)</param>
        
        [HttpGet]
        public JsonResult GetSeller_ContactInfo(string Seller)
        {
            Models.ActionResponse<List<API.Models.Seller_ContactInfoData>> ContactAddress = null;

            TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_ContactInfo a ;

            Service.SellerContactInfoService SCA = new Service.SellerContactInfoService();

            ContactAddress = SCA.GetSeller_ContactInfo(Seller);

            return Json(ContactAddress, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// [Post] Save Seller ContactAddress
        /// </summary>
        /// <param name="ContactAddress">Save Seller ContactAddress Data</param>
        
        [HttpPost]
        public JsonResult SaveSeller_ContactInfo(API.Models.Seller_ContactInfoData ContactAddress)
        {
            Models.ActionResponse<string> massage = null;

            Service.SellerContactInfoService SCA = new Service.SellerContactInfoService();

            massage = SCA.SaveSeller_ContactInfo(ContactAddress);

            return Json(massage, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// [Post] Delete Seller ContactAddress
        /// </summary>
        /// <param name="ContactAddress">Delete Seller ContactAddress Data</param>
        
        [HttpPost]
        public JsonResult DeleteSeller_ContactInfo(API.Models.Seller_ContactInfoData ContactAddress)
        {
            Models.ActionResponse<API.Models.Seller_ContactInfoData> massage = null;

            Service.SellerContactInfoService SCA = new Service.SellerContactInfoService();

            massage = SCA.DeleteSeller_ContactInfo(ContactAddress);

            return Json(massage, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// [Get] table Seller_ContactType Info
        /// </summary>

        [HttpGet]
        public JsonResult GetSeller_ContactType()
        {
            Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_ContactType>> massage = null;

            Service.SellerContactInfoService SCA = new Service.SellerContactInfoService();

            massage = SCA.GetSeller_ContactType();

            return Json(massage, JsonRequestBehavior.AllowGet);
        }
    }
}
