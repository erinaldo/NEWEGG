using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.API.Controllers
{
    public class SellerUserGroupRelationshipSearchController : Controller
    {
        // GET api/<controller>
        [HttpGet]
        public JsonResult GetUser_GroupByUser(string user, int type)
        {
            Models.ActionResponse<DB.TWSELLERPORTALDB.Models.User_Group> UserGroup = null;

            Service.SellerUserGroupRelationshipSearchService SUGSearch = new Service.SellerUserGroupRelationshipSearchService();

            UserGroup = SUGSearch.GetUser_GroupByUser(user, type);

            return Json(UserGroup, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUser_GroupBySeller(string seller, int type)
        {
            Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.User_Group>> UserGroup = null;

            Service.SellerUserGroupRelationshipSearchService SUGSearch = new Service.SellerUserGroupRelationshipSearchService();

            UserGroup = SUGSearch.GetUser_GroupBySeller(seller, type);

            return Json(UserGroup, JsonRequestBehavior.AllowGet);
        }
    }
}
