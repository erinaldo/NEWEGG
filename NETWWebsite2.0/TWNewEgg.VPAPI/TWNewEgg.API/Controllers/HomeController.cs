using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Models;

namespace TWNewEgg.API.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 取得該使用者可用功能清單
        /// </summary>
        /// <param name="userLoginResult">使用者登入資訊</param>
        /// <returns>返回該使用者可用功能清單</returns>
        [HttpGet]
        public JsonResult GetMenuList(Models.UserLoginResult userLoginResult)
        {
            TWNewEgg.API.Service.LeftMenuService leftMenuService = new Service.LeftMenuService();
            //Models.ActionResponse<List<MenuList>> menuListResult = leftMenuService.GetLeftMenu(userLoginResult);
            Models.ActionResponse<List<MenuList>> menuListResult = leftMenuService.GetLeftMenuV2(userLoginResult);
            return Json(menuListResult, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult CopyItemSketOldToNew(int oldSellerid, int? neweggSellerid)
        {
            string returnMsg = string.Empty;
            TWNewEgg.DB.TWSqlDBContext dbFrount = new DB.TWSqlDBContext();
            List<TWNewEgg.DB.TWSQLDB.Models.ItemSketch> listItemSKetch = new List<DB.TWSQLDB.Models.ItemSketch>();
            listItemSKetch = dbFrount.ItemSketch.Where(p => p.SellerID == oldSellerid).ToList();
            listItemSKetch.ForEach(p =>
            {
                p.SellerID = neweggSellerid.Value;
                p.ProducttempID = null;
                p.itemtempID = null;
                p.CreateDate = DateTime.Now;
                p.CreateDate = DateTime.Now;
                p.Status = 0;
                p.CreateUser = "Sys";
            });
            foreach (var item in listItemSKetch)
            {
                dbFrount.Entry(item).State = System.Data.EntityState.Added;
            }
            try
            {
                dbFrount.SaveChanges();
                returnMsg = "Success: 增加了" + listItemSKetch.Count;
            }
            catch (Exception ex)
            {
                returnMsg = ex.ToString();
            }
            return Json(returnMsg, JsonRequestBehavior.AllowGet);
        }
    }
}
