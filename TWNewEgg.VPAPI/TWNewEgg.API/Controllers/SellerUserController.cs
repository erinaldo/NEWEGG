using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.API.Controllers
{
    public class SellerUserController : Controller
    {

        /// <summary>
        /// [Get] Get Seller User
        /// </summary>
        /// <param name="Seller">search Table Seller_User by seller</param>
        /// <param name="type">search Table Seller_User seller by type</param>

        [HttpGet]
        public JsonResult GetSeller_User(string User, int type)
        {
            Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_User>> UserTB = null;

            Service.SellerUserService SUS = new Service.SellerUserService();

            UserTB = SUS.GetSeller_User(User, type);

            return Json(UserTB, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// [Post] Save Seller User
        /// </summary>
        /// <param name="User">search Table Seller_User by seller</param>
        
        [HttpPost]
        public JsonResult SaveSeller_User(DB.TWSELLERPORTALDB.Models.Seller_User User)
        {
            Models.ActionResponse<string> massage = null;

            Service.SellerUserService SUS = new Service.SellerUserService();

            massage = SUS.SaveSeller_User(User);

            return Json(massage, JsonRequestBehavior.AllowGet);
        }

        // GET api/<controller>
        [HttpGet]
        public JsonResult CheckSellerUserEmailUnique(string Email)
        {
            Models.ActionResponse<bool> IsEmailUnique = new Models.ActionResponse<bool>();

            Service.SellerUserService SUS = new Service.SellerUserService();

            IsEmailUnique = SUS.CheckSellerUserEmailUnique(Email);

            return Json(IsEmailUnique, JsonRequestBehavior.AllowGet);
        }

    }
}
