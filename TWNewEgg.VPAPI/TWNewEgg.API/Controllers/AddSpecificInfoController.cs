using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.API.Controllers
{
    /// <summary>
    /// Add Specific Info (APIController)
    /// </summary>
    
    //add by thisway
    public class AddSpecificInfoController : Controller
    {
        //[Filters.PermissionFilter]
        //[Attributes.ActionDescriptionAttribute("商品創建")]
        public JsonResult Create(Models.SpecificInfo ProductSpecificInfo)
        {
            Models.ActionResponse<Models.SpecificInfoResult> r = null;
            if (ProductSpecificInfo != null)
            {
                Service.AddSpecificInfoService a = new Service.AddSpecificInfoService();
                r = a.Create(ProductSpecificInfo);
            }
            return Json(r, JsonRequestBehavior.AllowGet);
        }
    }
}
