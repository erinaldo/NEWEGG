using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.API.Controllers
{
    /// <summary>
    /// Add Basic Info (APIController)
    /// 創建商品基礎訊息
    /// </summary>
    
    //add by Ian & Thisway
    public class AddBasicInfoController : Controller
    {
        //[Filters.PermissionFilter]
        //[Attributes.ActionDescriptionAttribute("商品創建")]
        public JsonResult Create(Models.BasicInfo ProductBasicinfo)
        {
             Models.ActionResponse<Models.BasicInfoResult> r = null;
             if (ProductBasicinfo.ManufacturerName != null)
             {
                 Service.AddBasicInfoService a = new Service.AddBasicInfoService();
                 r = a.Create(ProductBasicinfo);
             }
             else if (ProductBasicinfo.ManufacturerName == null)
             {
                 Service.AddBasicInfoService a = new Service.AddBasicInfoService();
                 r = a.GetManufacturer();
             }
             else
             {
                
             }
            return Json(r, JsonRequestBehavior.AllowGet);
        }
    }
}
