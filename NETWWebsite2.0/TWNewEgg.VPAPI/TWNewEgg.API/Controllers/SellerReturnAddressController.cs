using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.API.Controllers
{
    public class SellerReturnAddressController : Controller
    {
        /// <summary>
        /// [Get] Get Seller ReturnAddress
        /// </summary>
        /// <param name="Seller">Search table Seller ReturnAddress by keyword(Seller)</param>
        /// <param name="type">Search table Seller ReturnAddress seller by whitch type</param>
        
        [HttpGet]
        public JsonResult GetSeller_ReturnAddress(string Seller,int type)
        {
            Models.ActionResponse<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_ReturnInfo> ReturnAddress = null;

            Service.SellerReturnAddressService SRA = new Service.SellerReturnAddressService();

            ReturnAddress = SRA.GetSeller_ReturnAddress(Seller, type);

            return Json(ReturnAddress, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// [Post] Save Seller ReturnAddress
        /// 1. If table Seller_ReturnAddress have data , save table
        /// 2. If table Seller_ReturnAddress have not data , create table
        /// </summary>
        /// <param name="ReturnAddress">Save ReturnAddress Data in Seller_ReturnAddress</param>
        
        [HttpPost]
        public JsonResult SaveSeller_ReturnAddress(TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_ReturnInfo ReturnAddress)
        {
            Models.ActionResponse<string> massage = null;

            Service.SellerReturnAddressService SRA = new Service.SellerReturnAddressService();

            massage = SRA.SaveSeller_ReturnAddress(ReturnAddress);

            return Json(massage, JsonRequestBehavior.AllowGet);
        }

    }
}
