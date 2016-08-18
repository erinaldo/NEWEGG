using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.API.Controllers
{
    public class SellerNotificationController : Controller
    {
        /// <summary>
        /// [Get] Get Seller Notification
        /// </summary>
        /// <param name="Seller">Search table Seller Notification by keyword(Seller)</param>
        /// <param name="type">Search table Seller Notification seller by whitch type</param>
        
        [HttpGet]
        public JsonResult GetSeller_Notification(string Seller, int type)
        {
            Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_Notification>> Notification = null;

            Service.SellerNotificationService SNS = new Service.SellerNotificationService();

            Notification = SNS.GetSeller_Notification(Seller, type);

            return Json(Notification, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// [Post] Save Seller Notification
        /// </summary>
        /// <param name="Notification">Save Seller Notification Data</param>
        
        [HttpPost]
        public JsonResult SaveSeller_Notification(List<DB.TWSELLERPORTALDB.Models.Seller_Notification> Notification)
        {
            Models.ActionResponse<List<string>> massage = null;

            Service.SellerNotificationService SNS = new Service.SellerNotificationService();

            massage = SNS.SaveSeller_Notification(Notification);

            return Json(massage, JsonRequestBehavior.AllowGet);
        }

    }
}
