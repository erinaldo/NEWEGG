using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;

namespace TWNewEgg.API.Controllers
{
    public class SellerBasicInfoController : Controller
    {
        /// <summary>
        /// [Get] Get Seller BasicInfo
        /// </summary>
        /// <param name="Seller">Search table Seller BasicInfo by keyword(Seller)</param>
        /// <param name="type">Search table Seller BasicInfo seller by whitch type</param>
        
        [HttpGet]
        public JsonResult GetSeller_BasicInfo(string Seller, int type)
        {
            Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo> BasicInfo = null;

            Service.SellerBasicInfoService SBS = new Service.SellerBasicInfoService();

            BasicInfo = SBS.GetSeller_BasicInfo(Seller, type);

            return Json(BasicInfo, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSeller_BasicInfoWithFinancialByID(string sellerID)
        {
            Models.ActionResponse<TWNewEgg.API.Models.Seller_BasicInfoWithFinancial> info = null;

            Service.SellerBasicInfoService SBS = new Service.SellerBasicInfoService();

            info = SBS.GetSeller_BasicInfoWithFinancialByID(sellerID);

            return Json(info, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSeller_BasicInfoWithFinancialByEmail(string sellerEmail)
        {
            Models.ActionResponse<List<TWNewEgg.API.Models.Seller_BasicInfoWithFinancial>> info = null;

            Service.SellerBasicInfoService SBS = new Service.SellerBasicInfoService();

            info = SBS.GetSeller_BasicInfoWithFinancialByEmail(sellerEmail);

            return Json(info, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllSeller_BasicInfoWithFinancial()
        {
            Models.ActionResponse<List<TWNewEgg.API.Models.Seller_BasicInfoWithFinancial>> allInfo = null;

            Service.SellerBasicInfoService SBS = new Service.SellerBasicInfoService();

            allInfo = SBS.GetAllSeller_BasicInfoWithFinancial();

            return Json(allInfo, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// [Post] Save Seller BasicProInfo into Seller_BasicInfo
        /// </summary>
        /// <param name="BasicProInfo">Save Seller BasicProInfo into Seller_BasicInfo</param>
        
        [HttpPost]
        public JsonResult SaveSeller_BasicProInfo(API.Models.Seller_BasicProInfo BasicProInfo)
        {
            Models.ActionResponse<string> massage = null;

            Service.SellerBasicInfoService SBS = new Service.SellerBasicInfoService();

            massage = SBS.SaveSeller_BasicProInfo(BasicProInfo);

            return Json(massage, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// [Post] Save Seller BasicafterInfo into Seller_BasicInfo
        /// </summary>
        /// <param name="BasicafterInfo">Save Seller BasicafterInfo into Seller_BasicInfo</param>
        
        [HttpPost]
        public JsonResult SaveSeller_BasicafterInfo(API.Models.Seller_BasicafterInfo BasicafterInfo)
        {
            Models.ActionResponse<string> massage = null;

            Service.SellerBasicInfoService SBS = new Service.SellerBasicInfoService();

            massage = SBS.SaveSeller_BasicafterInfo(BasicafterInfo);

            return Json(massage, JsonRequestBehavior.AllowGet);
        }

        
        // GET controller
        [HttpGet]
        public JsonResult CheckSellerNameUnique(string SellerID, string SellerName)
        {
            Models.ActionResponse<bool> IsUnique = new Models.ActionResponse<bool>();

            Service.SellerBasicInfoService SBS = new Service.SellerBasicInfoService();

            IsUnique = SBS.CheckSellerNameUnique(SellerID, SellerName);

            return Json(IsUnique, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// [Post] Save Seller LogoImage
        /// </summary>
        /// <param name="SellerLogo">Save Seller LogoImage</param>

        [HttpPost]
        public JsonResult SaveSellerLogoImage(API.Models.SellerLogoInfo LogoImageInfo)
        {
            Models.ActionResponse<string> massage = null;

            Service.SellerBasicInfoService SBS = new Service.SellerBasicInfoService();

            massage = SBS.SaveSellerLogoImage(LogoImageInfo);

            return Json(massage, JsonRequestBehavior.AllowGet);
        }
    }
}
