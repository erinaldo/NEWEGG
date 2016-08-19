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
    public class ItemTempController : Controller
    {
        Service.TempService tmpService = new Service.TempService();

        [HttpPost]
        public JsonResult Search(TWNewEgg.API.Models.ItemSketchSearchCondition itemSearch, bool boolDefault)
        {
            TWNewEgg.DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            API.Models.ActionResponse<List<ItemSketch>> result = tmpService.ItemList(itemSearch, boolDefault);
            // 若回傳有資料，進行規格品的排除
            //if (result.Body != null)
            //{
            //    result.Body = tmpService.DistinctTempGroup(itemSearch, result.Body);
            //    var groupItems = db.ItemGroupDetailProperty.Where(x => x.SellerID == itemSearch.SellerID && x.ItemTempID.HasValue).Select(r => r.ItemTempID.Value).ToList();
            //    int itemGroupCount = db.ItemTemp.Where(x => groupItems.Contains(x.ID) && x.ItemStatus != 99).Count();
            //    result.Msg = (Convert.ToInt32(result.Msg) - itemGroupCount).ToString();
                
            //}

            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteTemp(List<int> DeleteItems)
        {
            API.Models.ActionResponse<List<string>> result = new ActionResponse<List<string>>();

            result = tmpService.DeleteTemp(DeleteItems);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // Bill 新架構有問題，先 Mark 返回舊版 Connector
        //[HttpPost]
        //public API.Models.ActionResponse<List<string>> EditDetailTemp(TWNewEgg.API.Models.ItemSketch EditModel)
        //{
        //    var checkresult = tmpService.check(EditModel, true);

        //    if (checkresult.IsSuccess == true)
        //    {
        //        API.Models.ActionResponse<List<string>> result = tmpService.TempDetailEdit(EditModel);

        //        return Json(result,JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return checkresult;

        //    }
        //}

        [HttpPost]
        public JsonResult EditDetailTemp(TWNewEgg.API.Models.ItemSketch EditModel)
        {
            var checkresult = tmpService.check(EditModel, true);

            if (checkresult.IsSuccess == true)
            {
                API.Models.ActionResponse<List<string>> result = tmpService.TempDetailEdit(EditModel);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(checkresult, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public JsonResult EditListTemp(List<TWNewEgg.API.Models.ItemSketch> UpdateListItemTemp, bool checkAll = false)
        {
            TWNewEgg.API.Models.ItemSketch EditModel = new ItemSketch();
            ActionResponse<List<string>> result = new ActionResponse<List<string>>();
            // check field
            result = tmpService.check(UpdateListItemTemp, checkAll);

            if (result.IsSuccess == true)
            {
                result = tmpService.TempListEdit(UpdateListItemTemp);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult BatchEditDetailTemp(List<TWNewEgg.API.Models.ItemSketch> EditModel, int UserID, int CurrentSellerID)
        {

            API.Models.ActionResponse<List<string>> result = tmpService.BatchTempDetailEdit(EditModel, UserID, CurrentSellerID);

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult ProductPropertyTemp(int productTempID)
        {
            TWNewEgg.API.Service.ProductPorpertyTempService productPorpertyTempService = new TWNewEgg.API.Service.ProductPorpertyTempService();

            ActionResponse<List<SaveProductProperty>> result = productPorpertyTempService.GetProductPropertyTemp(productTempID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetProperty(int categoryID)
        {
            TWNewEgg.API.Service.ProductPorpertyTempService productPorpertyTempService = new TWNewEgg.API.Service.ProductPorpertyTempService();

            ActionResponse<List<PropertyResult>> result = productPorpertyTempService.GetProperty(categoryID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ItemTempBatchCreationJson(List<ItemSketch> batchItemTempCreation, string userEmail, string passWord)
        {
            ActionResponse<List<string>> result = new ActionResponse<List<string>>();
            TWNewEgg.API.Service.ExamineBatchService EBService = new Service.ExamineBatchService();

            result = EBService.ItemTempBatchService(batchItemTempCreation, userEmail, passWord);
            return Json(result);
        }
    }
}
