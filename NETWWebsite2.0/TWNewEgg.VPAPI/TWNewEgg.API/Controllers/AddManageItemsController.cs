using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Models;

namespace TWNewEgg.API.Controllers
{
    /*---------- add by thisway ----------*/
    /// <summary>
    /// Add Manage Items (APIController)
    /// <para>Website Page:Create Items / Manage Items</para>
    /// </summary>
    public class AddManageItemsController : Controller
    {
        //[Attributes.ActionDescription("商品創建/選擇商品類別")]
        public JsonResult GetProductCategory(Models.Category ManageItems)
        {
            Models.ActionResponse<List<Models.CategoryResult>> result = null;

            Service.GetCategoryService AMIS = new Service.GetCategoryService();
            result = AMIS.GetProductCategory(ManageItems);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
    /*---------- end by thisway ----------*/
}
