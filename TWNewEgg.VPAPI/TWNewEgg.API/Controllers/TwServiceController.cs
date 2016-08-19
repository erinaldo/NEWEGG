using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Attributes;
using TWNewEgg.API.Models;
using Newtonsoft.Json;

namespace TWNewEgg.API.Controllers
{
    public class TwServiceController : Controller
    {
        //
        // GET: /TwService/

        TWNewEgg.API.Service.TWService twService = new Service.TWService();

        [HttpPost]
        public JsonResult PriceAPI(List<int> updateItemIDs)
        {
            TWNewEgg.API.Models.ActionResponse<List<int>> result = new ActionResponse<List<int>>();
            result = twService.PriceAPI(updateItemIDs);
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ImageProcess(List<string> picturesURL, string FullSizefilePath, string filePath, int ID)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();

            TWNewEgg.API.Service.ImageService imgService = new Service.ImageService();
            result = imgService.ImageProcess(picturesURL, FullSizefilePath, filePath, ID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ImageProcessItemAndProduct(List<string> picturesURL, string FullSizefilePath, string filePath, int ItemID,int productID,string ProductPath)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();

            TWNewEgg.API.Service.ImageService imgService = new Service.ImageService();
            result = imgService.ImageProcessItemAndProduct(picturesURL, FullSizefilePath, filePath, ItemID, productID, ProductPath,true);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult InvoiceAPI(string code)
        {
            Service.TWService twser = new Service.TWService();
            twser.UpdateInvoiceAPI(true, code);

            return null;
        }

        [HttpPost]
        public JsonResult PurviewOpen(string UserEmail, string Type)
        {
            TWNewEgg.DB.TWSellerPortalDBContext db = new DB.TWSellerPortalDBContext();
            string result = string.Empty;

            string purviewType = Type.ToLower();
            var userInfo = db.Seller_User.Where(x=>x.UserEmail == UserEmail.ToLower()).FirstOrDefault();

            switch(purviewType.ToLower())
            {
                case "pm":
                case "bsa":
                    userInfo.GroupID = 5;
                    userInfo.PurviewType = "G";
                    break;
                case "parttime":
                    userInfo.GroupID = 5;
                    userInfo.PurviewType = "U";

                    int referenceUserID = db.Seller_User.Where(x => x.GroupID == 5 && x.PurviewType == "U").Select(r=>r.UserID).FirstOrDefault();
                    List<TWNewEgg.DB.TWSELLERPORTALDB.Models.User_Purview> referenceUserPurview = db.User_Purview.Where(x => x.UserID == referenceUserID).ToList();
                    List<TWNewEgg.DB.TWSELLERPORTALDB.Models.User_Purview> makeUserPurview = this.makPTPurview(referenceUserPurview, userInfo.UserID);
                    makeUserPurview.ForEach(n => db.User_Purview.Add(n));
                    break;
            }
          
            try
            {
                db.Entry(userInfo).State = System.Data.EntityState.Modified;
                db.SaveChanges();

                result = "權限建立成功";
            }
            catch (Exception ex)
            {
                result = "Exception: " +ex.Message + ", " + ex.StackTrace;               
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private List<DB.TWSELLERPORTALDB.Models.User_Purview> makPTPurview(List<DB.TWSELLERPORTALDB.Models.User_Purview> referenceUserPurview,int UserID)
        {
            List<DB.TWSELLERPORTALDB.Models.User_Purview> result = new List<DB.TWSELLERPORTALDB.Models.User_Purview>();

            foreach (var index in referenceUserPurview)
            {
                DB.TWSELLERPORTALDB.Models.User_Purview tempUserPurview = new DB.TWSELLERPORTALDB.Models.User_Purview();

                tempUserPurview.Enable = index.Enable;
                tempUserPurview.FunctionID = index.FunctionID;
                tempUserPurview.InDate = DateTime.Now;
                tempUserPurview.UpdateDate = DateTime.Now;
                tempUserPurview.UserID = UserID;
                tempUserPurview.UpdateUserID = 61753;
                tempUserPurview.InUserID = 61753;

                result.Add(tempUserPurview);
            }

            return result;
        }

    }
}
